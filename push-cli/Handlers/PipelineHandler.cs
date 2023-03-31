using System.CommandLine;
using shared.Interfaces;
using shared.Models.Pipeline;

namespace push_cli.Handlers;

public class PipelineHandler
{
    private readonly IPipelineController _pipelineService;

    public PipelineHandler(IPipelineController pipelineService)
    {
        _pipelineService = pipelineService;
    }

    private async Task Upload(DirectoryInfo dir, string version)
    {
        if (!dir.Exists)
        {
            throw new FileNotFoundException("directory selected not found");
        }
        
        var pipelineLocation = Path.Join(dir.ToString(), "pipeline.json");
        var infoLocation = Path.Join(dir.ToString(), "info.json");
        
        if (!File.Exists(pipelineLocation))
        {
            throw new FileNotFoundException("Pipeline doesn't exist at target");
        }

        if (!File.Exists(infoLocation))
        {
            throw new FileNotFoundException("Info doesn't exist at target");
        }
        
        var pipeline = await JsonHelper.GetFile<PipelineVersionCode>(pipelineLocation);
        var info = await JsonHelper.GetFile<Info>(infoLocation);

        var files = new Dictionary<string, string>();

        foreach (var file in info.Files)
        {
            var path = Path.Join(dir.ToString(), file.Value);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"file {file.Value} doesn't exist lol");
            }
    
            files[file.Key] = Convert.ToBase64String(await File.ReadAllBytesAsync(path));
        }

        var body = new PipelineVersion
        {
            Id = new PipelineVersionKey()
            {
                Version = version,
                PipelineId = info.Id,
            },
            PipelineCode = pipeline,
            Files = files
        };

        try
        {
            await _pipelineService.UpdatePipelineVersion(info.Id, version, body);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        Console.WriteLine("done!");
    }

    public void Setup(RootCommand rootCommand)
    {
        var directoryArgument = new Argument<DirectoryInfo>(
            "directory",
            description: "The directory to use",
            getDefaultValue: () => new DirectoryInfo(Directory.GetCurrentDirectory()));

        var versionOption = new Option<string>(
            "--version",
            description: "Version of the pipeline to upload",
            getDefaultValue: () => "v1.0.0");
        
        versionOption.AddAlias("-v");

        var pipelineCommand = new Command("pipeline");

        rootCommand.AddCommand(pipelineCommand);

        var pipelineUploadCommand = new Command(
            "upload",
            description: "Upload a pipeline version");
        
        pipelineUploadCommand.AddArgument(directoryArgument);
        pipelineUploadCommand.AddOption(versionOption);

        pipelineCommand.AddCommand(pipelineUploadCommand);

        pipelineUploadCommand.SetHandler(async (dir, version) =>
        {
            await Upload(dir, version);
        }, directoryArgument, versionOption);
    }
}