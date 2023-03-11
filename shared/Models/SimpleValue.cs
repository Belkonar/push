using System.Text.Json.Serialization;

namespace shared.View;

public class SimpleValue
{
    [JsonPropertyName("value")]
    public string Value { get; set; }
}