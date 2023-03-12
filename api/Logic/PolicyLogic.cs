using MongoDB.Bson;
using MongoDB.Driver;
using shared.View;
using shared.UpdateModels;

namespace api.Logic;

// TODO: This later lol { _id: { $regex: /^global/ } }
public class PolicyLogic
{
    private readonly IMongoDatabase _database;

    public PolicyLogic(IMongoDatabase database)
    {
        _database = database;
    }
    
    public async Task<List<Policy>> GetAll()
    {
        var collection = _database.GetCollection<Policy>("policies");

        return await collection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<string> GetByName(string name)
    {
        var collection = _database.GetCollection<Policy>("policies");

        var filter = Builders<Policy>.Filter
            .Eq(x => x.Key, name);

        return (await collection.Find(filter).FirstOrDefaultAsync())?.PolicyText ?? "";
    }

    public async Task<Policy> Update(string key, UpdatePolicy policy)
    {
        var collection = _database.GetCollection<Policy>("policies");

        var filter = Builders<Policy>.Filter
            .Eq(x => x.Key, key);

        var update = Builders<Policy>.Update
            .Set(x => x.PolicyText, policy.Policy);

        await collection.UpdateOneAsync(filter, update);

        return new Policy()
        {
            Key = key,
            PolicyText = policy.Policy
        };
    }
    
    public async Task<Policy> Create(string key)
    {
        var collection = _database.GetCollection<Policy>("policies");
        
        var policy = new Policy()
        {
            Key = key,
            PolicyText = ""
        };

        await collection.InsertOneAsync(policy);

        return policy;
    }

    public async Task Delete(string key)
    {
        var collection = _database.GetCollection<Policy>("policies");

        var filter = Builders<Policy>.Filter
            .Eq(x => x.Key, key);

        await collection.DeleteOneAsync(filter);
    }
}