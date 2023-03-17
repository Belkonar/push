using MongoDB.Bson;

namespace shared.Models;

public class Audit
{
    // I don't care about this one since it'll never be needed for updates
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    
    public string ResourceType { get; set; }
    
    public string? ResourceId { get; set; }
    
    public string Action { get; set; }

    public Dictionary<string, string> Data { get; set; } = new();
    
    public string Subject { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;
    
    public BsonDocument Profile { get; set; }
}