using System.Text.Json;
using api.Services;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using MongoDB.Driver;
using shared;
using shared.Models;
using shared.Models.Job;
using shared.UpdateModels;

namespace api.Logic;

public class JobLogic
{
    private readonly IDistributedCache _cache;
    private readonly IMongoDatabase _mongoDatabase;
    private readonly UserService _userService;

    public JobLogic(IDistributedCache cache, IMongoDatabase mongoDatabase, UserService userService)
    {
        _cache = cache;
        _mongoDatabase = mongoDatabase;
        _userService = userService;
    }
    
    public async Task<List<Job>> GetJobByStatus(string status)
    {
        var collection = _mongoDatabase.GetCollection<Job>("jobs");

        var filter = Builders<Job>.Filter
            .Eq(x => x.Status, status);

        return await collection.Find(filter).ToListAsync();
    }

    public async Task<Job> GetJob(Guid id)
    {
        var collection = _mongoDatabase.GetCollection<Job>("jobs");

        var filter = Builders<Job>.Filter
            .Eq(x => x.Id, id);

        var job = await collection.Find(filter).FirstOrDefaultAsync();

        if (job == null)
        {
            throw new FileNotFoundException();
        }

        return job;
    }

    public async Task<Job> GetSafeJob(Guid id)
    {
        var collection = _mongoDatabase.GetCollection<Job>("jobs");
        
        var filter = Builders<Job>.Filter
            .Eq("_id", id);
        
        var projection = Builders<Job>.Projection
            .Exclude(x => x.Parameters)
            .Exclude(x => x.Files)
            .Exclude("Steps.Parameters")
            .Exclude("Steps.StepInfo");

        return await collection.Find(filter).Project<Job>(projection).FirstOrDefaultAsync();
    }
    
    public async Task<List<Job>> GetSafeJobs(Guid? id)
    {
        var collection = _mongoDatabase.GetCollection<Job>("jobs");

        FilterDefinition<Job> filter;

        if (id.HasValue)
        {
            filter = Builders<Job>.Filter
                .Eq(x => x.ThingId, id);
        }
        else
        {
            filter = Builders<Job>.Filter.Empty;
        }

        var projection = Builders<Job>.Projection
            .Exclude(x => x.Parameters)
            .Exclude(x => x.Files)
            .Exclude("Steps.Parameters")
            .Exclude("Steps.StepInfo");

        // var filter = Builders<BsonDocument>.Filter
        //     .Eq("_id", id);

        return await collection.Find(filter)
            .SortByDescending(x => x.Created)
            .Limit(10)
            .Project<Job>(projection)
            .ToListAsync();
    }

    public async Task UpdateStepOutput(Guid id, int ordinal, SimpleValue output)
    {
        var key = $"step-output.{id}.{ordinal}";
        
        await _cache.SetStringAsync(key, output.Value, new DistributedCacheEntryOptions()
        {
            // each call will reset this so the cache will really last 5 minutes
            // after the step finishes execution
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });
    }

    public async Task UpdateStatus(Guid id, UpdateStatus status)
    {
        var collection = _mongoDatabase.GetCollection<Job>("jobs");

        var filter = Builders<Job>.Filter
            .Eq(x => x.Id, id);
        
        var update = Builders<Job>.Update
            .Set(x => x.Status, status.Status)
            .Set(x => x.StatusReason, status.StatusReason);

        await collection.UpdateOneAsync(filter, update);
    }
    
    public async Task UpdateStepStatus(Guid id, int ordinal, UpdateStatus status)
    {
        var collection = _mongoDatabase.GetCollection<Job>("jobs");

        var filter = Builders<Job>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Job>.Update
            .Set("Steps.$[s].Status", status.Status)
            .Set("Steps.$[s].StatusReason", status.StatusReason);
        
        var arrayFilters = new[]
        {
            new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("s.Ordinal", ordinal)),
        };

        await collection.UpdateOneAsync(filter, update, new UpdateOptions()
        {
            ArrayFilters = arrayFilters
        });
    }

    public async Task ApproveStep(Guid id, int ordinal)
    {
        var collection = _mongoDatabase.GetCollection<Job>("jobs");

        var filter = Builders<Job>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Job>.Update
            .Push("Steps.$[s].Approvals", _userService.Subject);
        
        Console.WriteLine(_userService.Subject);
        Console.WriteLine(JsonSerializer.Serialize(_userService.Profile));
        
        var arrayFilters = new[]
        {
            new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("s.Ordinal", ordinal)),
        };

        await collection.UpdateOneAsync(filter, update, new UpdateOptions()
        {
            ArrayFilters = arrayFilters
        });

        await LogAudit(id);
    }

    private async Task LogAudit(Guid id)
    {
        var audit = new Audit()
        {
            Action = "sod-approval",
            ResourceType = "job",
            ResourceId = id.ToString(),
            Subject = _userService.Subject,
            Profile = BsonDocument.Parse(JsonSerializer.Serialize(_userService.Profile))
        };
        
        var collection = _mongoDatabase.GetCollection<Audit>("audit");
        
        await collection.InsertOneAsync(audit);
    }

    public async Task FinalizeStep(Guid id, int ordinal, ExecutorResponse response)
    {
        var collection = _mongoDatabase.GetCollection<Job>("jobs");
        
        var status = new UpdateStatus();

        if (response.ResponseCode != 0)
        {
            status.Status = "error";
            status.StatusReason = "step ended in failure";
        }
        else
        {
            status.Status = "success";
            status.StatusReason = "";
        }
        
        var filter = Builders<Job>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Job>.Update
            .Set("Steps.$[s].Status", status.Status)
            .Set("Steps.$[s].StatusReason", status.StatusReason)
            .Set("Steps.$[s].StepInfo.Output", response);
        
        var arrayFilters = new[]
        {
            new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("s.Ordinal", ordinal)),
        };

        await collection.UpdateOneAsync(filter, update, new UpdateOptions()
        {
            ArrayFilters = arrayFilters
        });

        if (response.ResponseCode != 0)
        {
            // just bubble the error
            await UpdateStatus(id, status);
        }
        else
        {
            await UpdateStatus(id, new UpdateStatus()
            {
                Status = "ready",
                StatusReason = ""
            });
        }
    }

    public async Task AddFeature(Guid id, JobFeature feature)
    {
        var collection = _mongoDatabase.GetCollection<Job>("jobs");

        var filter = Builders<Job>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Job>.Update
            .Push("Features", feature);

        await collection.UpdateOneAsync(filter, update);
    }

    /// <summary>
    /// The purpose of this is to get the logs. Either the cached run logs
    /// or the output logs from the ran step. Which you get is dependant on
    /// the status of the step.
    ///
    /// The general purpose of this is for the UI
    /// </summary>
    /// <param name="id">The Id of the job</param>
    /// <param name="ordinal">The ordinal of the step</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task<SimpleValue> GetStepOutput(Guid id, int ordinal)
    {
        var collection = _mongoDatabase.GetCollection<Job>("jobs");

        var filter = Builders<Job>.Filter
            .Eq(x => x.Id, id);

        var job = await collection.Find(filter).FirstOrDefaultAsync();

        if (job == null)
        {
            throw new FileNotFoundException();
        }
        
        var step = job.Steps.FirstOrDefault(x => x.Ordinal == ordinal);
        if (step == null)
        {
            throw new FileNotFoundException();
        }
        
        if (step.Status == "running")
        {
            var key = $"step-output.{id}.{ordinal}";
        
            return new SimpleValue()
            {
                Value = await _cache.GetStringAsync(key) ?? "No output yet"
            };
        }
        else
        {
            return new SimpleValue()
            {
                Value = step.StepInfo?.Output?.Shared ?? "No output yet"
            };
        }
    }
}