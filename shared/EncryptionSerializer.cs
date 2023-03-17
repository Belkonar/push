using System.Text.Json;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace shared;

// WARN: Serializers are syncronous so this method will slow your stuff down.
public class EncryptionSerializer<T> : SerializerBase<T>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
    {
        var json = JsonSerializer.Serialize(value);

        context.Writer.WriteStartDocument();
        context.Writer.WriteName("value");
        context.Writer.WriteString(json);

        context.Writer.WriteEndDocument();
    }
    
    public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        context.Reader.ReadStartDocument();

        var json = JsonSerializer.Deserialize<T>(context.Reader.ReadString());
        
        context.Reader.ReadEndDocument();
        
        return json!;
    }
}