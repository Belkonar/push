using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace shared.Models;

public class Policy
{
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("ordinal")]
    public int Ordinal { get; set; } = 1;
    
    [JsonPropertyName("yaml")]
    public string Yaml { get; set; }
}