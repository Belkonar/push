using System.IO.Compression;
using System.Net.Http.Json;
using Amazon.S3;
using Amazon.S3.Model;
using scheduler.Services;
using shared;
using shared.View;

namespace scheduler.Logic;

public class JobLogic
{
    private readonly Github _github;
    private readonly IHttpClientFactory _factory;
    private readonly IConfiguration _configuration;

    public JobLogic(Github github, IHttpClientFactory factory, IConfiguration configuration)
    {
        _github = github;
        _factory = factory;
        _configuration = configuration;
    }
    
    public async Task HandlePendingJobs()
    {
        var client = _factory.CreateClient("api");

        var response = await client.GetFromJsonAsync<List<JobView>>("/job?status=pending");

        if (response == null || response.Count == 0)
        {
            return;
        }

        foreach (var job in response)
        {
            await HandlePendingJob(job);
        }
    }

    public async Task HandlePendingJob(JobView job)
    {
        if (string.IsNullOrWhiteSpace(job.Contents.SourceControlUri) ||
            string.IsNullOrWhiteSpace(job.Contents.SourceControlUri))
        {
            return;
        }
        
        Console.WriteLine(job.Id);
        
        var code = await _github.GetReference(job.Contents.SourceControlUri, job.Contents.SourceReference);

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

        using IAmazonS3 client = new AmazonS3Client(Amazon.RegionEndpoint.USEast2);
        
        // aws s3 rm s3://{bucket-name} --recursive # to clean stuff up
        
        var request = new PutObjectRequest
        {
            BucketName = _configuration.GetValue<string>("BucketName"),
            Key = job.Id.ToString(),
            FilePath = finalArtifactLocation.FilePath,
        };
        
        var response = await client.PutObjectAsync(request);

        var listResponse = await client.ListBucketsAsync();
    }

    public async Task HandleReadyJobs()
    {
        
    }
}