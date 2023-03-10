using System.IO.Compression;
using System.Net.Http.Json;
using Amazon.S3;
using Amazon.S3.Model;
using shared;
using shared.Models.Job;

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
        
        // _configuration.GetValue<string>("BucketName")

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
        using var endingLocation = new TempFile();
        using var dockerSpace = new TempFolder();

        var volumeDir = dockerSpace.CreateRandomFolder();
        
        var getRequest = new GetObjectRequest()
        {
            BucketName = _configuration.GetValue<string>("BucketName"),
            Key = job.Id.ToString(),
        };
        
        using (var getResponse = await s3Client.GetObjectAsync(getRequest))
        {
            await getResponse.WriteResponseStreamToFileAsync(startingLocation.FilePath, false, new CancellationToken());
        }
        
        ZipFile.ExtractToDirectory(startingLocation.FilePath, volumeDir);
        File.WriteAllText(Path.Join(volumeDir, "myfile"), "my content");

        var dockerfile = new DockerBuilder(dockerSpace);
        
        dockerfile.From(step.StepInfo.Docker);
        
        dockerfile.SetupScript(step.StepInfo.Commands);
        
        foreach (var file in Directory.GetFiles(volumeDir))
        {
            Console.WriteLine(file);
        }
        dockerfile.WorkDirVolume(volumeDir, "/app");
        
        dockerfile.CreateFile();
        
        Executor.Execute(dockerfile.GetBuildConfig());

        var response = Executor.Execute(dockerfile.GetRunConfig());
        Console.WriteLine(response);
        
        ZipFile.CreateFromDirectory(volumeDir, endingLocation.FilePath);
        
        var uploadRequest = new PutObjectRequest
        {
            BucketName = _configuration.GetValue<string>("BucketName"),
            Key = job.Id.ToString(),
            FilePath = endingLocation.FilePath,
        };

        await s3Client.PutObjectAsync(uploadRequest);
    }
}