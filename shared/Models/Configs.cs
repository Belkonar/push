using MongoDB.Bson;

namespace shared.Models;

public class Configs
{
    public string Id { get; set; }
    
    // Work on this post MVP
    //[BsonSerializer(typeof(EncryptionSerializer<JsonElement>))]
    public BsonDocument Data { get; set; } = new();
}