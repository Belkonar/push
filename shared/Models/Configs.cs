using System.Text.Json.Serialization;

namespace shared.Models;

public class Configs
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("data")]
    public Dictionary<string, string> Data { get; set; } = new();
}