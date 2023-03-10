using System.IO.Compression;
using scheduler.Services;
using shared;
using shared.View;

namespace scheduler.Logic;

public class JobLogic
{
    private readonly Github _github;

    public JobLogic(Github github)
    {
        _github = github;
    }
    
    public async Task HandlePendingJobs()
    {
        
    }

    public async Task HandlePendingJob(JobView job)
    {
        var code = await _github.GetReference(job.Contents.SourceControlUri, job.Contents.SourceReference);

        using var artifactLocation = new TempFile();
        using var destinationFolder = new TempFolder();

        await File.WriteAllBytesAsync(artifactLocation.FilePath, code);
        
        ZipFile.ExtractToDirectory(artifactLocation.FilePath, destinationFolder.Dir);
        
        // This is needed since github adds an extra folder for some weird reason
        var actualDestinationFolder = Directory.GetDirectories(destinationFolder.Dir)[0];
        
        // TODO: File adds/replacements
        
        ZipFile.CreateFromDirectory(actualDestinationFolder, artifactLocation.FilePath);
        
        // We now have an artifact YAY!
    }

    public async Task HandleReadyJobs()
    {
        
    }
}