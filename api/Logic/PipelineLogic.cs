using AutoMapper;
using data;
using data.ORM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using shared;
using shared.Models.Pipeline;
using shared.View;

namespace api.Logic;

public class PipelineLogic
{
    private readonly MainContext _mainContext;
    private readonly IMapper _mapper;

    public PipelineLogic(MainContext mainContext, IMapper mapper)
    {
        _mainContext = mainContext;
        _mapper = mapper;
    }
    
    public async Task<List<PipelineView>> GetPipelines(Guid? org)
    {
        var query = _mainContext.Pipelines.AsQueryable();

        if (org.HasValue)
        {
            query = query.Where(x => x.OrganizationId == org);
        }

        return (await query.ToListAsync())
            .Select(x => _mapper.Map<PipelineDTO, PipelineView>(x))
            .ToList();
    }

    public async Task<PipelineView> GetPipeline(Guid id)
    {
        var pipeline = await _mainContext.Pipelines.FindAsync(id);

        if (pipeline == null)
        {
            // Make an exception for this
            return null;
        }

        return _mapper.Map<PipelineDTO, PipelineView>(pipeline);
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
        return (await _mainContext.PipelineVersions
            .Where(x => x.PipelineId == id)
            .Select(x => x.Version)
            .ToListAsync());
    }

    public async Task<PipelineVersionView> GetVersionByConstraint(Guid id, string constraint)
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

        if (filtered.IsNullOrEmpty())
        {
            throw new FileNotFoundException("no version matching constraint");
        }

        var version = await _mainContext.PipelineVersions
            .FirstAsync(x => x.PipelineId == id && x.Version == filtered);
        
        return _mapper.Map<PipelineVersionDTO, PipelineVersionView>(version);
    }
    
    public async Task<PipelineView> CreatePipeline(PipelineView data)
    {
        data.Id = Guid.NewGuid();

        await _mainContext.AddAsync(_mapper.Map<PipelineView, PipelineDTO>(data));
        await _mainContext.SaveChangesAsync();
        
        return data;
    }

    // TODO: Use a specific update model for pipelines
    public async Task<PipelineView> UpdatePipeline(Guid id, PipelineView data)
    {
        var old = await _mainContext.Pipelines.FindAsync(id);

        if (old == null)
        {
            throw new Exception("not found");
        }

        _mapper.Map(data, old);
        _mainContext.Mark(old);

        await _mainContext.SaveChangesAsync();

        return data;
    }

    // TODO: Handle calculation of parameters and comparing them against a real version
    // TODO: Once we are going to go live, actually enforce versions
    public async Task<PipelineVersionView> UpdatePipelineVersion(Guid id, string key, PipelineVersionView data)
    {
        data.Contents.CompiledParameters = CalculateParams(data.Contents.PipelineCode);
        
        var old = await _mainContext.PipelineVersions
            .FirstOrDefaultAsync(x => x.PipelineId == id && x.Version == key);

        if (old == null)
        {
            Console.WriteLine("adding");
            await _mainContext.AddAsync(_mapper.Map<PipelineVersionView, PipelineVersionDTO>(data));
        }
        else
        {
            Console.WriteLine("updating");
            _mapper.Map(data, old);
            _mainContext.Mark(old);
        }

        await _mainContext.SaveChangesAsync();
        
        return data;
    }

    public List<StepParameter> CalculateParams(PipelineVersion version)
    {
        var parameters = version.Parameters.ToList();

        foreach (var stage in version.Stages)
        {
            foreach (var step in stage.Steps)
            {
                var actualStep = version.Steps.First(x => x.Name == step.Step);
                foreach (var stepParameter in actualStep.Parameters)
                {
                    if (!step.Parameters.ContainsKey(stepParameter.Name))
                    {
                        // Need to add this to the options since it's not filled out
                        var newStepParameter = stepParameter.Clone();
                        newStepParameter.Name = $"{step.Name}.${stepParameter.Name}";
                        
                        parameters.Add(newStepParameter);
                    }
                }
            }
        }
        
        return parameters;
    }
}