using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace shared.Models;

public class Configs
{
    public string Id { get; set; }
    
    // Work on this post MVP
    //[BsonSerializer(typeof(EncryptionSerializer<JsonElement>))]
    public BsonDocument Data { get; set; } = new();
}