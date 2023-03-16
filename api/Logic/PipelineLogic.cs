using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using shared;
using shared.Models.Pipeline;
using shared.View;

namespace api.Logic;

public class PipelineLogic
{
    private readonly IMongoDatabase _database;

    public PipelineLogic(IMongoDatabase database)
    {
        _database = database;
    }
    
    public async Task<List<Pipeline>> GetPipelines(Guid? org)
    {
        var collection = _database.GetCollection<Pipeline>("pipelines");

        return await collection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<Pipeline> GetPipeline(Guid id)
    {
        var collection = _database.GetCollection<Pipeline>("pipelines");

        var filter = Builders<Pipeline>.Filter
            .Eq(x => x.Id, id);

        var pipeline = await collection.Find(filter).FirstOrDefaultAsync();

        if (pipeline == null)
        {
            throw new FileNotFoundException();
        }

        return pipeline;
    }
    
    public async Task<string> GetLatestMajor(Guid id)
    {
        // Issue here is that if there are zero versions this may fail
        return (await GetVersions(id))
            .Select(x => new Semver(x)) // Convert to semver
            .Where(x => x.IsValid) // Only take valid versions, no dev
            .ToList() // rasterize it
            .OrderDescending() // Reverse order
            .FirstOrDefault() // Get the top
            ?.GetConstraint() ?? ""; // get the constraint for the most recent version or blank so it doesn't error
    }

    // I don't need to bother ordering this since I can do it in the UI
    // Code re-use across boundries is fire.
    public async Task<List<string>> GetVersions(Guid id)
    {
        var collection = _database.GetCollection<PipelineVersion>("pipeline_versions");

        var filter = Builders<PipelineVersion>.Filter
            .Eq(x => x.Id.PipelineId, id);

        return await collection.Find(filter)
            .Project(x => x.Id.Version)
            .ToListAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="constraint"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task<PipelineVersion> GetVersionByConstraint(Guid id, string constraint)
    {
        var versions = (await GetVersions(id)).AsEnumerable();
        
        if (constraint.EndsWith('.'))
        {
            versions = versions.Where(x => x.StartsWith(constraint));
        }
        else
        {
            // this may seem stupid but it makes everything cleaner.
            versions = versions.Where(x => x == constraint);
        }
        
        var filtered = versions
            .Select(x => new Semver(x))
            .Where(x => x.IsValid)
            .OrderDescending()
            .FirstOrDefault()
            ?.ToString() ?? "";
        
        if (string.IsNullOrEmpty(filtered))
        {
            throw new FileNotFoundException("no version matching constraint");
        }

        return await GetVersion(id, filtered);
    }
    
    public async Task<Pipeline> CreatePipeline(Pipeline data)
    {
        // We'll likely get a dummy ID but make a not crap one
        data.Id = Guid.NewGuid();
        
        var collection = _database.GetCollection<Pipeline>("pipelines");

        await collection.InsertOneAsync(data);

        return data;
    }

    // TODO: Use a specific update model for pipelines
    public async Task<Pipeline> UpdatePipeline(Guid id, Pipeline data)
    {
        data.Id = id;
        
        var collection = _database.GetCollection<Pipeline>("pipelines");

        var filter = Builders<Pipeline>.Filter
            .Eq(x => x.Id, id);

        await collection.ReplaceOneAsync(filter, data);

        return data;
    }

    // TODO: Handle calculation of parameters and comparing them against a real version
    // TODO: Once we are going to go live, actually enforce versions
    public async Task<PipelineVersion> UpdatePipelineVersion(Guid id, string key, PipelineVersion data)
    {
        data.Id = new PipelineVersionKey()
        {
            PipelineId = id,
            Version = key
        };
        
        data.CompiledParameters = CalculateParams(data.PipelineCode);

        var collection = _database.GetCollection<PipelineVersion>("pipeline_versions");

        var filter = Builders<PipelineVersion>.Filter
            .Eq(x => x.Id, data.Id);

        var old = await collection.Find(filter).FirstOrDefaultAsync();

        if (old == null)
        {
            Console.WriteLine("write a new version");
            await collection.InsertOneAsync(data);
        }
        else
        {
            await collection.ReplaceOneAsync(filter, data);
        }

        return data;
    }

    public List<StepParameter> CalculateParams(PipelineVersionCode version)
    {
        var parameters = version.Parameters.ToList();

        foreach (var stage in version.Stages)
        {
            foreach (var step in stage.Steps)
            {
                var actualStep = version.Steps.FirstOrDefault(x => x.Name == step.Step);

                if (actualStep == null)
                {
                    // Managed step
                    continue;
                }
                
                foreach (var stepParameter in actualStep.Parameters)
                {
                    if (!step.Parameters.ContainsKey(stepParameter.Name))
                    {
                        // Need to add this to the options since it's not filled out
                        var newStepParameter = stepParameter.Clone();
                        newStepParameter.Name = $"{stage.Name}>{step.Name}>{stepParameter.Name}";
                        
                        parameters.Add(newStepParameter);
                    }
                }
            }
        }
        
        return parameters;
    }

    public async Task<PipelineVersion> GetVersion(Guid id, string versionValue)
    {
        var collection = _database.GetCollection<PipelineVersion>("pipeline_versions");
        
        var filter = Builders<PipelineVersion>.Filter
            .Eq(x => x.Id, new PipelineVersionKey()
            {
                PipelineId = id,
                Version = versionValue
            });

        var version = await collection.Find(filter).FirstOrDefaultAsync();

        if (version == null)
        {
            throw new Exception("wot in the fucj");
        }

        return version;
    }

    public async Task<Dictionary<string,string>> GetFiles(Guid id, string version)
    {
        var collection = _database.GetCollection<PipelineVersion>("pipeline_versions");
        
        var filter = Builders<PipelineVersion>.Filter
            .Eq(x => x.Id, new PipelineVersionKey()
            {
                PipelineId = id,
                Version = version
            });

        return await collection.Find(filter)
            .Project(x => x.Files)
            .FirstOrDefaultAsync() ?? new Dictionary<string, string>();
    }
}