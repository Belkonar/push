using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace shared;

// WARN: Serializers are syncronous so this method will slow your stuff down.
public class EncryptionSerializer<T> : SerializerBase<T>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
    {
        var json = JsonSerializer.Serialize(value);
        context.Writer.WriteString(json);
    }
    
    public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var json = JsonSerializer.Deserialize<T>(context.Reader.ReadString());
        return json;
    }
}