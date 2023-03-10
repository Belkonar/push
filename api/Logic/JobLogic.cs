using AutoMapper;
using data;
using data.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
    
    public async Task<List<JobView>> GetJobByStatus(string status)
    {
        return (
                await _context.Jobs
                    .Where(x => x.Status == status)
                    .ToListAsync()
            )
            .Select(x => _mapper.Map<JobDto, JobView>(x))
            .ToList();
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