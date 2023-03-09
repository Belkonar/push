﻿// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using cli;
using shared;
using shared.Models.Pipeline;
using shared.View;

// use this to test
// dotnet run && docker build . -t tester && docker run tester

using var temp = new TempFolder(false);
var dockerfile = new DockerBuilder(temp);

dockerfile.From("node:16");
dockerfile.SetupScript(new List<string>()
{
    "echo 1;sleep 3",
    "echo 2;sleep 3",
    "echo 3;sleep 3",
    "echo 4;sleep 3",
    "echo 5;sleep 3",
    "echo 6;sleep 3",
    "echo 7;sleep 3",
    "echo 8;sleep 3",
    "echo 9;sleep 3",
});
dockerfile.WorkDirVolume("/Users/dotson/tester", "/app");

dockerfile.CreateFile();

Executor.Execute(dockerfile.GetBuildConfig());

var r = Executor.Execute(dockerfile.GetRunConfig(), async s =>
{
    Console.WriteLine("======= START =======");
    Console.WriteLine(s);
    Console.WriteLine("======== END ========");
});

//Console.WriteLine(r.Shared);

return;

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

// if (!JsonHelper.IsValid(pipeline))
// {
//     return;
// }

var files = new Dictionary<string, string>();

// TODO: Files

var body = new PipelineVersionView
{
    Version = "v1.0.0",
    PipelineId = info.Id,
    Contents = new PipelineVersionContents
    {
        PipelineCode = pipeline,
        Files = files
    }
};

using var http = new HttpClient();
using var response = await http.PostAsJsonAsync($"http://localhost:5183/pipeline/{info.Id}/version/{body.Version}", body);

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