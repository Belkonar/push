// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using cli;
using shared.Models.Pipeline;

// var jsonText = File.ReadAllText("jsonTest.json");
// var request = JsonSerializer.Deserialize<PolicyEngineRequest<SimpleValue>>(jsonText);
// var outputText = JsonSerializer.Serialize(request);
//
// Console.WriteLine(outputText);
//
// return;

if (args.Length == 0)
{
    Console.WriteLine("No Directory");
    return;
}

// The last thing in the list is the target
var target = args.First();

if (!Directory.Exists(target))
{
    Console.WriteLine("Directory supplied doesn't exist");
    return;
}

var version = "v1.0.0";

if (args.Length > 1)
{
    version = args[1];
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

var pipeline = await JsonHelper.GetFile<PipelineVersionCode>(pipelineLocation);
var info = await JsonHelper.GetFile<Info>(infoLocation);

var files = new Dictionary<string, string>();

foreach (var file in info.Files)
{
    var path = Path.Join(target, file.Value);
    if (!File.Exists(path))
    {
        throw new FileNotFoundException($"file {file.Value} doesn't exist lol");
    }
    
    files[file.Key] = Convert.ToBase64String(File.ReadAllBytes(path));
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

using var http = new HttpClient();
using var response = await http.PostAsJsonAsync($"http://localhost:5183/pipeline/{info.Id}/version/{body.Id.Version}", body);

try
{
    Console.WriteLine(await response.Content.ReadAsStringAsync());
    response.EnsureSuccessStatusCode();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

Console.WriteLine("wot");