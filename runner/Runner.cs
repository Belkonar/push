using System.IO.Compression;
using System.Net.Http.Json;
using Amazon.S3;
using Amazon.S3.Model;
using shared;
using shared.Models;
using shared.Models.Job;
using shared.UpdateModels;
using shared.View;

namespace runner;

public class Runner
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;

    public Runner(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }
    
    // TODO: commander
    public async Task Run(string[] args)
    {
        var id = Guid.Parse(args[0]);
        var ordinal = Convert.ToInt32(args[1]);

        var job = await _client.GetFromJsonAsync<Job>($"/job/{id}");

        if (job == null)
        {
            return;
        }

        var step = job.Steps.FirstOrDefault(x => x.Ordinal == ordinal);

        if (step == null)
        {
            return;
        }
        
        Console.WriteLine("got everything");

        var updateStatusResponse = await _client.PostAsJsonAsync($"/job/{job.Id}/step/{ordinal}/status", new UpdateStatus()
        {
            Status = "running",
            StatusReason = ""
        });

        updateStatusResponse.EnsureSuccessStatusCode();
        
        if (step.StepInfo != null)
        {
            await ProcessDockerStep(job, step);
        }
    }

    private async Task ProcessDockerStep(Job job, JobStep step)
    {
        if (step.StepInfo == null) throw new ArgumentNullException(nameof(step));
        
        using IAmazonS3 s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast2);
        
        using var startingLocation = new TempFile();
        using var dockerSpace = new TempFolder();
        using var volumeDir = new TempFolder();

        var getRequest = new GetObjectRequest()
        {
            BucketName = _configuration.GetValue<string>("BucketName"),
            Key = job.Id.ToString(),
        };
        
        using (var getResponse = await s3Client.GetObjectAsync(getRequest))
        {
            await getResponse.WriteResponseStreamToFileAsync(startingLocation.FilePath, false, new CancellationToken());
        }
        
        ZipFile.ExtractToDirectory(startingLocation.FilePath, volumeDir.Dir);

        var dockerfile = new DockerBuilder(dockerSpace);
        
        dockerfile.From(step.StepInfo.Docker);
        
        dockerfile.Env("JOB_ID", job.Id.ToString());
        dockerfile.Env("JOB_REF", job.SourceReference);

        foreach (var param in step.Parameters)
        {
            if (param.Kind == "credential")
            {
                // http://localhost:5183/organization/credential/85821557-4844-414d-a4f6-5d21b9d07c21/bundle
                
                var bundle = await _client.GetFromJsonAsync<CredentialBundle>($"/organization/credential/{param.Value}/bundle");

                if (bundle != null)
                {
                    foreach (var header in bundle.Headers)
                    {
                        dockerfile.Env(header.Key, header.Value);
                    }

                    foreach (var file in bundle.Files)
                    {
                        dockerfile.CopyData(file.Key, file.Value);
                    }
                }
            }
        }
        
        // /etc/ssh/ssh_config
        if (step.StepInfo.Remote)
        {
            // dockerfile.SetupRemoteDocker();
        }
        
        dockerfile.SetupScript(step.StepInfo.Commands);

        dockerfile.WorkDirVolume(volumeDir.Dir, "/app");
        
        dockerfile.CreateFile();
        
        Executor.Execute(dockerfile.GetBuildConfig());

        var response = Executor.Execute(dockerfile.GetRunConfig(), async s =>
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return;
            }
            
            using var outputResponse = await _client.PutAsJsonAsync($"/job/{job.Id}/step/{step.Ordinal}/output", new SimpleValue()
            {
                Value = s
            });

            outputResponse.EnsureSuccessStatusCode();
        });

        if (step.StepInfo.Persist)
        {
            using var endingLocation = new TempFile();
        
            ZipFile.CreateFromDirectory(volumeDir.Dir, endingLocation.FilePath);
        
            var uploadRequest = new PutObjectRequest
            {
                BucketName = _configuration.GetValue<string>("BucketName"),
                Key = job.Id.ToString(),
                FilePath = endingLocation.FilePath
            };

            await s3Client.PutObjectAsync(uploadRequest);
        }

        using var finalizeResponse = await _client.PostAsJsonAsync($"/job/{job.Id}/step/{step.Ordinal}/finalize", response);

        finalizeResponse.EnsureSuccessStatusCode();
    }
}