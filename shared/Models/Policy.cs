using MongoDB.Bson.Serialization.Attributes;

namespace shared.Models;

public class Policy
{
    [BsonId]
    public string Key { get; set; }
    
    public string PolicyText { get; set; }
}