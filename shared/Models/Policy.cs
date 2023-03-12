using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace shared.View;

public class Policy
{
    [BsonId]
    public string Key { get; set; }
    
    public string PolicyText { get; set; }
}