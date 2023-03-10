using AutoMapper;
using data;
using data.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using shared.Models.Job;
using shared.UpdateModels;
using shared.View;

namespace api.Logic;

public class JobLogic
{
    private readonly MainContext _context;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public JobLogic(MainContext context, IMapper mapper, IDistributedCache cache)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
    }
    
    public async Task<List<Job>> GetJobByStatus(string status)
    {
        throw new NotImplementedException();
    }

    public async Task<JobView> GetJob(Guid id)
    {
        var job = await _context.Jobs.FindAsync(id);
        
        if (job == null)
        {
            throw new FileNotFoundException();
        }
        
        return _mapper.Map<JobDto, JobView>(job);
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
        var job = await _context.Jobs.FindAsync(id);
        
        if (job == null)
        {
            throw new FileNotFoundException();
        }

        job.Status = status.Status;
        job.StatusReason = status.StatusReason;

        await _context.SaveChangesAsync();
    }
    
    // NOTE: This method is sensitive to race conditions when async workflows are made.
    // When I switch to mongo this will be no longer a problem
    public async Task UpdateStepStatus(Guid id, int ordinal, UpdateStatus status, bool updateJob = false)
    {
        var job = await _context.Jobs.FindAsync(id);
        
        if (job == null)
        {
            throw new FileNotFoundException();
        }

        var step = job.Contents.Steps.FirstOrDefault(x => x.Ordinal == ordinal);

        if (step == null)
        {
            throw new FileNotFoundException();
        }

        step.Status = status.Status;
        step.StatusReason = status.StatusReason;
        
        _context.Mark(job);

        if (updateJob)
        {
            job.Status = status.Status;
            job.StatusReason = status.StatusReason;
        }
        
        await _context.SaveChangesAsync();
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
        var job = await _context.Jobs.FindAsync(id);
        if (job == null)
        {
            throw new FileNotFoundException();
        }

        var step = job.Contents.Steps.FirstOrDefault(x => x.Ordinal == ordinal);
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