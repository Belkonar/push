using System.IO.Compression;
using System.Net.Http.Json;
using Amazon.S3;
using Amazon.S3.Model;
using scheduler.Services;
using shared;
using shared.Models.Job;
using shared.UpdateModels;

namespace scheduler.Logic;

public class JobLogic
{
    private readonly Github _github;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _client;

    public JobLogic(Github github, IHttpClientFactory factory, IConfiguration configuration)
    {
        _github = github;
        _configuration = configuration;
        _client = factory.CreateClient("api");
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
        
    }
}