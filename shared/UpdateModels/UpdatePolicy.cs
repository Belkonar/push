using System.Text.Json.Serialization;

namespace shared.UpdateModels;

public class UpdatePolicy
{
    [JsonPropertyName("policy")]
    public string Policy { get; set; }
}