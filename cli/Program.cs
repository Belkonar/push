// See https://aka.ms/new-console-template for more information

using cli;
using shared.Models.Pipeline;
using shared.View;

if (args.Length == 0)
{
    Console.WriteLine("No Directory");
    return;
}

// The last thing in the list is the target
var target = args.Last();

if (!Directory.Exists(target))
{
    Console.WriteLine("Directory supplied doesn't exist");
    return;
}

var pipelineLocation = Path.Join(target, "pipeline.json");
var infoLocation = Path.Join(target, "info.json");

if (!File.Exists(pipelineLocation))
{
    Console.WriteLine("Pipeline doesn't exist at target");
    return;
}

if (!File.Exists(infoLocation))
{
    Console.WriteLine("Info doesn't exist at target");
    return;
}

var pipeline = await JsonHelper.GetFile<PipelineVersion>(pipelineLocation);
var info = await JsonHelper.GetFile<Info>(infoLocation);

if (!JsonHelper.IsValid(pipeline))
{
    return;
}

var files = new Dictionary<string, string>();

var body = new PipelineVersionView
{
    Version = "dev",
    PipelineId = info.Id,
    Contents = new PipelineVersionContents
    {
        PipelineCode = pipeline,
        Files = files
    }
};

using var http = new HttpClient();

//http.PostAsJsonAsync()
