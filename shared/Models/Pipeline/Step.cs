using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace shared.Models.Pipeline;

public class Step
{
    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("parameters")]
    public List<StepParameter> Parameters { get; set; } = new();

    [Required]
    [JsonPropertyName("docker")]
    public string Docker { get; set; }
   
    // TODO: This will be ignore for the demo
    [JsonPropertyName("remote")]
    public bool Remote { get; set; } = true;

    [JsonPropertyName("persist")]
    public bool Persist { get; set; } = true;

    [Required]
    [JsonPropertyName("commands")]
    public List<string> Commands { get; set; } = new();
}