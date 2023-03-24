using MongoDB.Bson;

namespace shared.Models;

/// <summary>
/// The Audit model for audit records lol
/// </summary>
/// <remarks>
/// There's no ID because the intent is that you won't be able to `update` these.
/// </remarks>
public class Audit
{
    public string ResourceType { get; set; }
    
    public string? ResourceId { get; set; }
    
    public string Action { get; set; }

    public Dictionary<string, string> Data { get; set; } = new();
    
    public string Subject { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;
    
    public BsonDocument Profile { get; set; }
}