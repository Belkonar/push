using System.Text.Json.Serialization;

namespace cli;

public class Info
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
}