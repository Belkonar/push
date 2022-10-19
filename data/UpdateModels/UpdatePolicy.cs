using System.Text.Json.Serialization;

namespace data.UpdateModels;

public class UpdatePolicy
{
    [JsonPropertyName("policy")]
    public string Policy { get; set; }
}