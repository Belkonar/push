using System.Text.Json.Serialization;

namespace shared.Models.Pipeline;

public class Step
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("parameters")]
    public List<StepParameter> Parameters { get; set; } = new();

    [JsonPropertyName("docker")]
    public string Docker { get; set; }
   
    [JsonPropertyName("remote")]
    public bool Remote { get; set; } = true;

    [JsonPropertyName("persist")]
    public bool Persist { get; set; } = true;

    [JsonPropertyName("steps")]
    public List<string> Steps { get; set; } = new();
}