using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace shared.View;

public class Policy
{
    [BsonId]
    [JsonPropertyName("key")]
    public string Key { get; set; }
    
    [JsonPropertyName("policy")]
    public string PolicyText { get; set; }
}