using AutoMapper;
using data;
using data.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using shared.View;

namespace api.Logic;

public class JobLogic
{
    private readonly MainContext _context;
    private readonly IMapper _mapper;

    public JobLogic(MainContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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

    public async Task UpdateStepOutput(Guid id, int ordinal, string output)
    {
        // TODO: figure this out
    }
}