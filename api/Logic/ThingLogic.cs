using AutoMapper;
using data;
using data.ORM;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using shared.Models;
using shared.Models.Job;
using shared.Models.Pipeline;
using shared.UpdateModels;
using shared.View;

namespace api.Logic;

public class ThingLogic
{
    private readonly IMapper _mapper;
    private readonly MainContext _mainContext;
    private readonly PipelineLogic _pipelineLogic;

    public ThingLogic(IMapper mapper, MainContext mainContext, PipelineLogic pipelineLogic)
    {
        _mapper = mapper;
        _mainContext = mainContext;
        _pipelineLogic = pipelineLogic;
    }
    
    public async Task<List<ThingView>> GetThings()
    {
        return (await _mainContext.Things.ToListAsync())
            .Select(x => _mapper.Map<ThingDto, ThingView>(x))
            .ToList();
    }
    
    public async Task<ThingView> GetThing(Guid id)
    {
        var thing = await _mainContext.Things.FindAsync(id);
        
        if (thing == null)
        {
            throw new FileNotFoundException();
        }
        
        return _mapper.Map<ThingDto, ThingView>(thing);
    }

    public async Task<ThingView> CreateThing(UpdateThing thingView)
    {
        var request = _mapper.Map<UpdateThing, ThingDto>(thingView);
        request.Id = Guid.NewGuid();
        request.Contents = new Thing();

        await _mainContext.AddAsync(request);
        await _mainContext.SaveChangesAsync();

        return _mapper.Map<ThingDto, ThingView>(request);
    }

    public async Task<ThingView> UpdateThing(Guid id, ThingView thingView)
    {
        var thing = await _mainContext.Things.FindAsync(id);
        
        if (thing == null)
        {
            throw new FileNotFoundException();
        }

        _mapper.Map(thingView, thing);
        await _mainContext.SaveChangesAsync();
        
        return _mapper.Map<ThingDto, ThingView>(thing);
    }

    /// <summary>
    /// Does an upsert for the deployable resource
    /// </summary>
    /// <param name="id">id of the thing (not the deployable)</param>
    /// <param name="deployableView">deployable model</param>
    /// <returns></returns>
    public async Task<DeployableView> UpdateDeployable(Guid id, DeployableView deployableView)
    {
        var deployable = await _mainContext.Deployables.FirstOrDefaultAsync(x => x.ThingId == id);

        if (deployable == null)
        {
            deployable = _mapper.Map<DeployableView, DeployableDto>(deployableView);
            deployable.Id = Guid.NewGuid();
            await _mainContext.AddAsync(deployable);
        }
        else
        {
            _mapper.Map(deployableView, deployable);
            // TODO: This may not be needed
            _mainContext.Check(deployable);
            _mainContext.Mark(deployable);
        }

        await _mainContext.SaveChangesAsync();

        return _mapper.Map<DeployableDto, DeployableView>(deployable);
    }

    public async Task<DeployableView> GetDeployable(Guid id)
    {
        var deployable = await _mainContext.Deployables.FirstOrDefaultAsync(x => x.ThingId == id);

        if (deployable == null)
        {
            return new DeployableView()
            {
                Contents = new Deployable(),
                ThingId = id
            };
        }
        else
        {
            return _mapper.Map<DeployableDto, DeployableView>(deployable);
        }
    }

    /// <summary>
    /// Start a pipeline job with the given source reference
    /// </summary>
    /// <param name="id">id of the thing (not the deployable)</param>
    /// <param name="reference">the source reference</param>
    /// TODO: Add the job viewmodel and DTO stuff here
    public async Task<Job> StartDeployment(Guid id, string reference)
    {
        var thing = await _mainContext.Things.FindAsync(id);

        if (thing == null)
        {
            throw new FileNotFoundException("Thing not found");
        }

        var deployable = await _mainContext.Deployables.FirstOrDefaultAsync(x => x.ThingId == id);

        if (deployable == null)
        {
            throw new FileLoadException("deployable not found");
        }

        if (!deployable.Contents.PipelineId.HasValue)
        {
            throw new Exception("Pipeline not selected");
        }
        
        // Don't need to handle the null because that's caught in the method here
        var pipeline = await _pipelineLogic.GetVersionByConstraint(deployable.Contents.PipelineId.Value, deployable.Contents.PipelineConstraint);
        var pipelineCode = pipeline.Contents.PipelineCode;

        var job = new Job()
        {
            Id = ObjectId.GenerateNewId(),
            ThingId = id
        };

        job.SourceReference = reference;
        job.PipelineId = pipeline.PipelineId;
        job.PipelineVersion = pipeline.Version;

        foreach (var globalParameter in pipeline.Contents.PipelineCode.Parameters)
        {
            var param = _mapper.Map<StepParameter, JobStepParameter>(globalParameter);
            param.Name = $"root.{globalParameter.Name}";
            
            if (deployable.Contents.Variables.ContainsKey(param.Name))
            {
                param.Value = deployable.Contents.Variables[param.Name];
            }
            
            job.Parameters.Add(param);
        }

        var ordinal = 0;
        foreach (var stage in pipelineCode.Stages)
        {
            job.Stages.Add(new JobStage()
            {
                Name = stage.Name
            });

            foreach (var step in stage.Steps)
            {
                var jobStep = new JobStep()
                {
                    Name = $"{stage.Name} - {step.Name}",
                    Step = step.Step,
                    Ordinal = ordinal++,
                };
                
                var stepCode = pipelineCode.Steps.FirstOrDefault(x => x.Name == step.Step);

                if (stepCode == null)
                {
                    // In here are built-in steps. So a special kind
                    foreach (var parameter in step.Parameters)
                    {
                        jobStep.Parameters.Add(new JobStepParameter()
                        {
                            Name = parameter.Key,
                            Value = parameter.Value,
                            Kind = "managed"
                        });
                    }
                }
                else
                {
                    // Here we reverse the loop to only pull stuff we actually need.

                    foreach (var parameter in stepCode.Parameters)
                    {
                        var newParameter = _mapper.Map<StepParameter, JobStepParameter>(parameter);
                        // TODO: template stuff for param
                        if (step.Parameters.ContainsKey(parameter.Name))
                        {
                            newParameter.Value = step.Parameters[parameter.Name];    
                        }
                        else
                        {
                            var paramKey = $"{stage.Name}.{step.Name}.{parameter.Name}";
                            
                            if (deployable.Contents.Variables.ContainsKey(paramKey))
                            {
                                newParameter.Value = deployable.Contents.Variables[paramKey];
                            }
                            else
                            {
                                // We know ahead of time that this step will fail, but that's OK
                                // as everything before that will work.
                                jobStep.Status = "invalid";
                                jobStep.StatusReason = $"Param \"{paramKey}\" missing";
                            }
                        }
                        
                        jobStep.Parameters.Add(newParameter);
                    }

                    jobStep.StepInfo = _mapper.Map<Step, JobStepInfo>(stepCode);
                }
                
                job.Steps.Add(jobStep);
            }
        }

        // TODO: Save Job
        
        return job;
    }
}