using Amazon.Runtime.Internal.Transform;
using api.Services;
using AutoMapper;
using MongoDB.Bson;
using shared.Models;
using shared.UpdateModels;
using MongoDB.Driver;
using shared.View;

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

    public async Task<Credential> CreateCredential(Guid id, Credential credential)
    {
        // Not really needed
        credential.OrganizationId = id;
        credential.Id = Guid.NewGuid();
        
        var collection = _mongoDatabase.GetCollection<Credential>("credentials");

        await collection.InsertOneAsync(credential);

        return credential;
    }

    public async Task<List<Credential>> GetCredentials(Guid id)
    {
        var collection = _mongoDatabase.GetCollection<Credential>("credentials");

        var filter = Builders<Credential>.Filter
            .Eq(x => x.OrganizationId, id);

        var project = Builders<Credential>.Projection
            .Exclude(x => x.Data);

        return await collection.Find(filter).Project<Credential>(project).ToListAsync();
    }
    
    public async Task<Credential> GetCredential(Guid id)
    {
        var collection = _mongoDatabase.GetCollection<Credential>("credentials");

        var filter = Builders<Credential>.Filter
            .Eq(x => x.Id, id);

        var credential =  await collection.Find(filter).FirstOrDefaultAsync();

        if (credential == null)
        {
            throw new FileNotFoundException();
        }

        return credential;
    }
    
    public async Task<CredentialBundle> GetCredentialBundle(Guid id)
    {
        var collection = _mongoDatabase.GetCollection<Credential>("credentials");

        var filter = Builders<Credential>.Filter
            .Eq(x => x.Id, id);

        var credential =  await collection.Find(filter).FirstOrDefaultAsync();

        if (credential == null)
        {
            throw new FileNotFoundException();
        }

        if (credential.Kind == "static")
        {
            return GetStaticBundle(credential);
        }

        // If it's a type I don't support return an empty bundle
        return new CredentialBundle();
    }

    private CredentialBundle GetStaticBundle(Credential credential)
    {
        var bundle = new CredentialBundle();

        foreach (var cred in credential.Data)
        {
            var parts = cred.Key.Split(":");

            if (parts[0] == "header")
            {
                bundle.Headers.Add(parts[1], cred.Value);
            }
            else
            {
                bundle.Files.Add(cred.Key, cred.Value);
            }
        }
        
        return bundle;
    }

    public async Task UpdateCredentialData(Guid id, Dictionary<string, string> data)
    {
        var collection = _mongoDatabase.GetCollection<Credential>("credentials");

        var filter = Builders<Credential>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Credential>.Update
            .Set(x => x.Data, data);

        await collection.UpdateOneAsync(filter, update);
    }
}