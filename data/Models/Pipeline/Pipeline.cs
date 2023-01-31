using System.Text.Json;
using System.Text.Json.Serialization;

// use this with `using Pipeline = data.Models.Pipeline;` to handle collisions and make it cleaner
namespace data.Models.Pipeline;

public class Pipeline
{
    [JsonPropertyName("stages")]
    public List<Stage> Stages { get; set; } = new ();

    [JsonPropertyName("commands")]
    public List<Command> Commands { get; set; } = new();
    
    [JsonPropertyName("files")]
    public Dictionary<string, string> Files { get; set; } = new();
    [JsonPropertyName("parameters")]
    public List<CommandParameter> Parameters { get; set; } = new();
}

public class Stage
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("commands")]
    public List<StageCommand> Commands { get; set; } = new();
}

public class StageCommand
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonExtensionData]
    public IDictionary<string, JsonElement> Parameters { get; set; }
}

public class Command
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("parameters")]
    public List<CommandParameter> Parameters { get; set; } = new();

    [JsonPropertyName("docker")]
    public string Docker { get; set; }
   
    [JsonPropertyName("remote")]
    public bool Remote { get; set; } = true;

    [JsonPropertyName("persist")]
    public bool Persist { get; set; } = true;

    [JsonPropertyName("steps")]
    public List<string> Steps { get; set; } = new();
}

public class CommandParameter
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("local")]
    public bool Local { get; set; } = true;
    
    [JsonPropertyName("kind")]
    public string Kind { get; set; }
    
    [JsonPropertyName("subKind")]
    public string SubKind { get; set; }
}