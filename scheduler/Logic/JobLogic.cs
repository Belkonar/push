using System.IO.Compression;
using System.Net.Http.Json;
using Amazon.S3;
using Amazon.S3.Model;
using scheduler.Services;
using shared;
using shared.Models;
using shared.Models.Job;
using shared.Models.Nomad;
using shared.UpdateModels;

namespace scheduler.Logic;

public class JobLogic
{
    private readonly Github _github;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _client;
    private readonly HttpClient _nomadClient;

    public JobLogic(Github github, IHttpClientFactory factory, IConfiguration configuration)
    {
        _github = github;
        _configuration = configuration;
        _client = factory.CreateClient("api");
        _nomadClient = factory.CreateClient("nomad");
    }
    
    public async Task HandlePendingJobs()
    {
        await HandleJobTypes("pending", HandlePendingJob);
    }

    private async Task HandleJobTypes(string status, Func<Job, Task> handler)
    {
        List<Job> jobs;

        try
        {
            jobs = await _client.GetFromJsonAsync<List<Job>>($"/job?status={status}") ?? new List<Job>();
        }
        catch (Exception e)
        {
            return;
        }

        foreach (var job in jobs)
        {
            try
            {
                await handler(job);
            }
            catch (Exception e)
            {
                await UpdateJobStatus(job, "error");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }

    public async Task HandlePendingJob(Job job)
    {
        if (string.IsNullOrWhiteSpace(job.SourceControlUri) ||
            string.IsNullOrWhiteSpace(job.SourceControlUri))
        {
            return;
        }
        
        Console.WriteLine(job.Id);
        
        _github.Setup(job.SourceControlUri);
        
        var code = await _github.GetReference(job.SourceControlUri, job.SourceReference);

        using var artifactLocation = new TempFile();
        using var finalArtifactLocation = new TempFile();
        using var destinationFolder = new TempFolder();

        await File.WriteAllBytesAsync(artifactLocation.FilePath, code);
        
        ZipFile.ExtractToDirectory(artifactLocation.FilePath, destinationFolder.Dir);
        
        // This is needed since github adds an extra folder for some weird reason
        var actualDestinationFolder = Directory.GetDirectories(destinationFolder.Dir)[0];
        
        // TODO: File adds/replacements
        var files = await _client.GetFromJsonAsync<Dictionary<string, string>>($"/pipeline/{job.PipelineVersion.PipelineId}/files/{job.PipelineVersion.Version}")
            ?? new Dictionary<string, string>(); // just to cover the nullable

        foreach (var file in job.Files)
        {
            var filePath = Path.Join(actualDestinationFolder, file.Location);

            if ((!File.Exists(filePath) || file.IsFixed) && files.ContainsKey(file.Key))
            {
                await File.WriteAllBytesAsync(filePath, Convert.FromBase64String(files[file.Key]));

                if (!file.IsBinary)
                {
                    // do the replacements on text files
                    var data = await File.ReadAllTextAsync(filePath);
                    data = ProcessTemplate(job.Parameters, data);
                    await File.WriteAllTextAsync(filePath, data);
                }
            }
        }
        
        ZipFile.CreateFromDirectory(actualDestinationFolder, finalArtifactLocation.FilePath);
        
        using IAmazonS3 s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast2);
        
        // aws s3 rm s3://{bucket-name} --recursive # to clean stuff up
        
        var request = new PutObjectRequest
        {
            BucketName = _configuration.GetValue<string>("BucketName"),
            Key = job.Id.ToString(),
            FilePath = finalArtifactLocation.FilePath,
        };
        
        await s3Client.PutObjectAsync(request);

        var updateStatus = new UpdateStatus()
        {
            Status = "ready",
            StatusReason = ""
        };

        var response = await _client.PostAsJsonAsync($"/job/{job.Id.ToString()}/status", updateStatus);
        response.EnsureSuccessStatusCode();
    }

    public async Task HandleReadyJobs()
    {
        await HandleJobTypes("ready", HandleReadyJob);
    }

    private async Task HandleReadyJob(Job job)
    {
        await UpdateJobStatus(job, "running");
        
        var steps = job.Steps
            .Where(x => x.Status == "pending")
            .OrderBy(x => x.Ordinal)
            .ToList();

        if (steps.Count == 0)
        {
            // No more pending steps!
            await UpdateJobStatus(job, "success");
            return;
        }

        var step = steps.First();

        if (step.RequiredApprovals > 0 && step.Approvals.Count < step.RequiredApprovals)
        {
            await UpdateJobStatus(job, "approval");
            await UpdateJobStepStatus(job, step.Ordinal, "approval");
            return;
        }

        if (step.StepInfo == null)
        {
            await UpdateJobStatus(job, "ready");
            await UpdateJobStepStatus(job, step.Ordinal, "success");

            if (step.Step == "sod")
            {
                // send the sod feature
                await _client.PostAsJsonAsync($"/job/{job.Id}/feature", new JobFeature()
                {
                    Name = "sod"
                });
            }
            
            if (step.Step == "deployment")
            {
                // send the sod feature
                await _client.PostAsJsonAsync($"/job/deployment", new DeploymentRecord()
                {
                    SourceReference = job.SourceReference,
                    ThingId = job.ThingId,
                    JobId = job.Id,
                    PipelineVersion = job.PipelineVersion,
                    Features = job.Features
                });
            }
            
            return;
        }
        
        // I got everything I need for nomad!
        var request = new NomadJobRequest();

        request.Job.Id = job.Id.ToString();
        request.Job.Name = job.Id.ToString();
        
        var taskGroup = new NomadJobTaskGroup();
        var task = new NomadTask
        {
            Config =
            {
                Command = "runner",
                Arguments = new List<string>()
                {
                    { job.Id.ToString() },
                    { step.Ordinal.ToString() }
                }
            }
        };
        
        taskGroup.Tasks.Add(task);
        request.Job.TaskGroups.Add(taskGroup);

        var nomadRequest = await _nomadClient.PostAsJsonAsync($"/v1/job/{job.Id}", request);
        nomadRequest.EnsureSuccessStatusCode();
    }

    private async Task UpdateJobStatus(Job job, string status)
    {
        using var response = await _client.PostAsJsonAsync($"/job/{job.Id.ToString()}/status", new UpdateStatus()
        {
            Status = status
        });

        response.EnsureSuccessStatusCode();
    }
    
    private async Task UpdateJobStepStatus(Job job, int ordinal, string status)
    {
        using var response = await _client.PostAsJsonAsync($"/job/{job.Id}/step/{ordinal}/status", new UpdateStatus()
        {
            Status = status
        });

        response.EnsureSuccessStatusCode();
    }
    
    // job/{job.Id}/step/{ordinal}/status
    
    static string ProcessTemplate(List<JobStepParameter> localParameters, string s)
    {
        var local = s;

        foreach (var parameter in localParameters)
        {
            local = local.Replace($"{{{parameter.Name}}}", parameter.Value);
        }
        
        return local;
    }

    public async Task HandleApprovalJobs()
    {
        await HandleJobTypes("approval", HandleApprovalJob);
    }

    private async Task HandleApprovalJob(Job job)
    {
        var step = job.Steps.FirstOrDefault(x => x.Status == "approval");

        if (step == null)
        {
            // racing to ready
            await UpdateJobStatus(job, "ready");
            return;
        }

        if (step.Approvals.Count >= step.RequiredApprovals)
        {
            await UpdateJobStatus(job, "ready");
            await UpdateJobStepStatus(job, step.Ordinal, "pending");
        }
    }
}