using api.Services;
using AutoMapper;
using MongoDB.Bson;
using shared.Models;
using shared.UpdateModels;
using MongoDB.Driver;

namespace api.Logic;

public class OrganizationLogic
{
    private readonly PermissionService _permissionService;
    private readonly IMongoDatabase _mongoDatabase;

    public OrganizationLogic(PermissionService permissionService, IMongoDatabase mongoDatabase)
    {
        _permissionService = permissionService;
        _mongoDatabase = mongoDatabase;
    }
    
    public async Task<List<Organization>> GetAll()
    {
        var collection = _mongoDatabase.GetCollection<Organization>("organizations");

        return await collection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<Organization> Update(Guid id, UpdateOrganization body)
    {
        var collection = _mongoDatabase.GetCollection<Organization>("organizations");
        
        var filter = Builders<Organization>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Organization>.Update
            .Set(x => x.Name, body.Name);

        await collection.UpdateOneAsync(filter, update);
        
        return await Get(id);
    }

    public async Task<Organization> UpdateMetadata(Guid id, Dictionary<string, string> body)
    {
        var collection = _mongoDatabase.GetCollection<Organization>("organizations");
        
        var filter = Builders<Organization>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Organization>.Update
            .Set(x => x.Metadata, body);

        await collection.UpdateOneAsync(filter, update);

        return await Get(id);
    }

    public async Task<Organization> UpdatePrivateMetadata(Guid id, Dictionary<string, string> body)
    {
        var collection = _mongoDatabase.GetCollection<Organization>("organizations");
        
        var filter = Builders<Organization>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Organization>.Update
            .Set(x => x.PrivateMetadata, body);

        await collection.UpdateOneAsync(filter, update);
        
        return await Get(id);
    }

    public async Task<Organization> UpdatePolicy(Guid id, string body)
    {
        var collection = _mongoDatabase.GetCollection<Organization>("organizations");
        
        var filter = Builders<Organization>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Organization>.Update
            .Set(x => x.Policy, body);

        await collection.UpdateOneAsync(filter, update);
        
        return await Get(id);
    }

    public async Task<Organization> Create(UpdateOrganization body)
    {
        var collection = _mongoDatabase.GetCollection<Organization>("organizations");

        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = body.Name
        };

        await collection.InsertOneAsync(org);

        return org;
    }

    public async Task<Organization> Get(Guid id)
    {
        var collection = _mongoDatabase.GetCollection<Organization>("organizations");
        
        var filter = Builders<Organization>.Filter
            .Eq(x => x.Id, id);
        
        var organization = await collection.Find(filter).FirstOrDefaultAsync();

        if (organization == null)
        {
            throw new FileNotFoundException();
        }

        return organization;
    }

    public async Task UpdateVariable(Guid id, UpdateOrganizationVariable variable)
    {
        var collection = _mongoDatabase.GetCollection<Organization>("organizations");
        
        var filter = Builders<Organization>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Organization>.Update
            .Set(x => x.Variables[variable.Key], variable.Value);

        await collection.UpdateOneAsync(filter, update);
    }
}