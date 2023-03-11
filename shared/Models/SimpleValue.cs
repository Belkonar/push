using System.Text.Json.Serialization;

namespace shared.Models;

public class SimpleValue
{
    [JsonPropertyName("value")]
    public string Value { get; set; }
}