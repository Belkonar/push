using System.IO.Compression;
using System.Net.Http.Json;
using Amazon.S3;
using Amazon.S3.Model;
using scheduler.Services;
using shared;
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
        var response = await _client.GetFromJsonAsync<List<Job>>("/job?status=pending");

        if (response == null || response.Count == 0)
        {
            return;
        }

        foreach (var job in response)
        {
            await HandlePendingJob(job);
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
        
        var code = await _github.GetReference(job.SourceControlUri, job.SourceReference);

        using var artifactLocation = new TempFile();
        using var finalArtifactLocation = new TempFile();
        using var destinationFolder = new TempFolder();

        await File.WriteAllBytesAsync(artifactLocation.FilePath, code);
        
        ZipFile.ExtractToDirectory(artifactLocation.FilePath, destinationFolder.Dir);
        
        // This is needed since github adds an extra folder for some weird reason
        var actualDestinationFolder = Directory.GetDirectories(destinationFolder.Dir)[0];
        
        // TODO: File adds/replacements
        
        ZipFile.CreateFromDirectory(actualDestinationFolder, finalArtifactLocation.FilePath);
        
        // We now have an artifact YAY!

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
        var response = await _client.GetFromJsonAsync<List<Job>>("/job?status=ready");

        if (response == null || response.Count == 0)
        {
            return;
        }

        foreach (var job in response)
        {
            await HandleReadyJob(job);
        }
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
}