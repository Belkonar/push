using AutoMapper;
using data;
using data.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using shared.Models.Job;
using shared.UpdateModels;
using shared.View;

namespace api.Logic;

public class JobLogic
{
    private readonly MainContext _context;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly IMongoDatabase _mongoDatabase;

    public JobLogic(MainContext context, IMapper mapper, IDistributedCache cache, IMongoDatabase mongoDatabase)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
        _mongoDatabase = mongoDatabase;
    }
    
    public async Task<List<Job>> GetJobByStatus(string status)
    {
        var collection = _mongoDatabase.GetCollection<Job>("jobs");

        var filter = Builders<Job>.Filter
            .Eq(x => x.Status, "pending");

        return await collection.Find(filter).ToListAsync();
    }

    public async Task<JobView> GetJob(Guid id)
    {
        // var job = await _context.Jobs.FindAsync(id);
        //
        // if (job == null)
        // {
        //     throw new FileNotFoundException();
        // }
        //
        // return _mapper.Map<JobDto, JobView>(job);

        throw new NotImplementedException();
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
    
    // TODO: updateJob
    public async Task UpdateStepStatus(Guid id, int ordinal, UpdateStatus status, bool updateJob = false)
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

        // This is mostly used on errors to bubble them up
        if (updateJob)
        {
            await UpdateStatus(id, status);
        }
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
        // var job = await _context.Jobs.FindAsync(id);
        // if (job == null)
        // {
        //     throw new FileNotFoundException();
        // }
        //
        // var step = job.Contents.Steps.FirstOrDefault(x => x.Ordinal == ordinal);
        // if (step == null)
        // {
        //     throw new FileNotFoundException();
        // }
        //
        // if (step.Status == "running")
        // {
        //     var key = $"step-output.{id}.{ordinal}";
        //
        //     return new SimpleValue()
        //     {
        //         Value = await _cache.GetStringAsync(key) ?? "No output yet"
        //     };
        // }
        // else
        // {
        //     return new SimpleValue()
        //     {
        //         Value = step.StepInfo?.Output?.Shared ?? "No output yet"
        //     };
        // }

        throw new NotImplementedException();
    }
}