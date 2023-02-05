// See https://aka.ms/new-console-template for more information

using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;
using shared.Models.Pipeline;

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

if (!File.Exists(pipelineLocation))
{
    Console.WriteLine("Pipeline doesn't exist at target");
    return;
}

var jsonFile = File.ReadAllBytes(pipelineLocation);

using var m = new MemoryStream(jsonFile);
var pipeline = await JsonSerializer.DeserializeAsync<PipelineVersion>(m);
m.Close();

List<ValidationResult> results = new List<ValidationResult>(); 

var valid = Validator.TryValidateObject(pipeline, new ValidationContext(pipeline), results, true);

if (!valid)
{
    Console.WriteLine("validation errors were found");
    foreach (var validationResult in results)
    {
        Console.WriteLine(validationResult);
    }
}

using var http = new HttpClient();

//http.PostAsJsonAsync()
