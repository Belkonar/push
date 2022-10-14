using System.Text.Json.Serialization;

namespace data.Models;

public class Thing
{
    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; set; } = new();

    [JsonPropertyName("private_metadata")]
    public Dictionary<string, string> PrivateMetadata { get; set; } = new();
}