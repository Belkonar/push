using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace shared.Models;

public class Configs
{
    public string Id { get; set; }
    
    // Work on this post MVP
    //[BsonSerializer(typeof(EncryptionSerializer<Dictionary<string, string>>))]
    public Dictionary<string, string> Data { get; set; } = new();
}