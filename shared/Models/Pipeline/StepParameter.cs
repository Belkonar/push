using System.Text.Json.Serialization;

namespace shared.Models.Pipeline;

public class StepParameter
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("local")]
    public bool Local { get; set; } = true;
    
    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "string";
    
    [JsonPropertyName("subKind")]
    public string SubKind { get; set; } = "";

    public StepParameter Clone()
    {
        return (StepParameter)MemberwiseClone();
    }
}