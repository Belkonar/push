using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using shared.Models;

namespace api.Services;

public class ConfigService
{
    private readonly IMongoDatabase _database;

    public ConfigService(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task Set<T>(string key, T value)
    {
        var collection = _database.GetCollection<Configs>("configs");
        
        var filter = Builders<Configs>.Filter
            .Eq(x => x.Id, key);

        var config = new Configs()
        {
            Id = key,
            Data = value.ToBsonDocument()
        };

        await collection.ReplaceOneAsync(filter, config, new ReplaceOptions()
        {
            IsUpsert = true
        });
    }

    public async Task<T> Get<T>(string key) where T: new()
    {
        var collection = _database.GetCollection<Configs>("configs");

        var filter = Builders<Configs>.Filter
            .Eq(x => x.Id, key);

        var config = await collection.Find(filter).FirstOrDefaultAsync();

        if (config == null)
        {
            return new T();
        }

        return BsonSerializer.Deserialize<T>(config.Data);
    }
}