using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using shared.Interfaces;
using shared.Models;
using shared.Models.Job;
using shared.Models.Pipeline;
using shared.Services;
using shared.UpdateModels;

namespace api.Logic;

public class ThingLogic
{
    private readonly PipelineLogic _pipelineLogic;
    private readonly IMongoDatabase _database;
    private readonly IMapper _mapper;
    private readonly JobLogic _jobLogic;
    private readonly IGithub _github;

    public ThingLogic(PipelineLogic pipelineLogic, IMongoDatabase database, IMapper mapper, JobLogic jobLogic, IGithub github)
    {
        _pipelineLogic = pipelineLogic;
        _database = database;
        _mapper = mapper;
        _jobLogic = jobLogic;
        _github = github;
    }
    
    public async Task<List<Thing>> GetThings()
    {
        var collection = _database.GetCollection<Thing>("things");
        
        return await collection.Find(new BsonDocument()).ToListAsync();
    }
    
    public async Task<Thing> GetThing(Guid id)
    {
        var collection = _database.GetCollection<Thing>("things");

        var filter = Builders<Thing>.Filter
            .Eq(x => x.Id, id);

        var thing = await collection.Find(filter).FirstOrDefaultAsync();

        if (thing == null)
        {
            throw new FileNotFoundException();
        }

        return thing;
    }

    public async Task<Thing> CreateThing(UpdateThing thingView)
    {
        var collection = _database.GetCollection<Thing>("things");
        
        var thing = new Thing()
        {
            Id = Guid.NewGuid(),
            Name = thingView.Name,
            OrganizationId = thingView.OrganizationId
        };

        await collection.InsertOneAsync(thing);

        return thing;
    }

    public async Task<Thing> UpdateThing(Guid id, Thing thingView)
    {
        var collection = _database.GetCollection<Thing>("things");
        
        var filter = Builders<Thing>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Thing>.Update
            .Set(x => x.Name, thingView.Name);

        await collection.UpdateOneAsync(filter, update);

        return await GetThing(id);
    }

    /// <summary>
    /// Does an upsert for the deployable resource
    /// </summary>
    /// <param name="id">id of the thing (not the deployable)</param>
    /// <param name="deployableView">deployable model</param>
    /// <returns></returns>
    public async Task<Deployable> UpdateDeployable(Guid id, Deployable deployableView)
    {
        var collection = _database.GetCollection<Thing>("things");
        
        var filter = Builders<Thing>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Thing>.Update
            .Set(x => x.Deployable, deployableView);

        await collection.UpdateOneAsync(filter, update);

        return deployableView;
    }

    public async Task<Deployable> GetDeployable(Guid id)
    {
        var collection = _database.GetCollection<Thing>("things");
        
        var filter = Builders<Thing>.Filter
            .Eq(x => x.Id, id);

        var deployable = await collection.Find(filter)
            .Project(x => x.Deployable)
            .FirstOrDefaultAsync();

        if (deployable != null)
        {
            return deployable;
        }

        return new Deployable();
    }

    /// <summary>
    /// Start a pipeline job with the given source reference
    /// </summary>
    /// <param name="id">id of the thing (not the deployable)</param>
    /// <param name="reference">the source reference</param>
    /// TODO: Add the job viewmodel and DTO stuff here
    public async Task<Job> StartDeployment(Guid id, string reference)
    {
        var thing = await GetThing(id);
        
        
        if (thing == null)
        {
            throw new FileNotFoundException("Thing not found");
        }

        var deployable = thing.Deployable;
        
        if (deployable == null)
        {
            throw new FileLoadException("deployable not found");
        }
        
        var commit = await _github.GetReference(thing.Deployable!.SourceControlUri, reference);
        
        if (!deployable.PipelineId.HasValue)
        {
            throw new Exception("Pipeline not selected");
        }
        
        // Don't need to handle the null because that's caught in the method here
        var pipeline = await _pipelineLogic.GetVersionByConstraint(deployable.PipelineId.Value, deployable.PipelineConstraint);
        var pipelineCode = pipeline.PipelineCode;
        
        var job = new Job()
        {
            Id = Guid.NewGuid(),
            ThingId = id,
            ThingName = thing.Name,
            Created = DateTime.Now
        };
        
        job.SourceControlUri = deployable.SourceControlUri;
        job.SourceReference = commit;
        job.PipelineVersion = pipeline.Id;
        job.Files = pipeline.PipelineCode.Files;
        
        foreach (var globalParameter in pipeline.PipelineCode.Parameters)
        {
            var param = _mapper.Map<StepParameter, JobStepParameter>(globalParameter);
            param.Name = $"root>{globalParameter.Name}";
            
            if (deployable.Variables.ContainsKey(globalParameter.Name))
            {
                param.Value = deployable.Variables[globalParameter.Name];
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
                    Name = $"{step.Name}",
                    Step = step.Step,
                    Stage = stage.Name,
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

                    if (step.Step == "sod")
                    {
                        jobStep.RequiredApprovals = 2;
                    }
                }
                else
                {
                    // Here we reverse the loop to only pull stuff we actually need.

                    var localParameters = job.Parameters.ToList();
        
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
                            var paramKey = $"{stage.Name}>{step.Name}>{parameter.Name}";
                            
                            if (deployable.Variables.ContainsKey(paramKey))
                            {
                                newParameter.Value = deployable.Variables[paramKey];
                            }
                            else
                            {
                                // We know ahead of time that this step will fail, but that's OK
                                // as everything before that will work.
                                jobStep.Status = "invalid";
                                jobStep.StatusReason = $"Param \"{paramKey}\" missing";
                            }
                        }
                        
                        // Reset the name so I can use it for replacements
                        newParameter.Name = $"parameters>{parameter.Name}";
                        
                        localParameters.Add(newParameter);
                        jobStep.Parameters.Add(newParameter);
                    }
        
                    jobStep.StepInfo = _mapper.Map<Step, JobStepInfo>(stepCode);

                    jobStep.StepInfo.Commands = jobStep.StepInfo.Commands
                        .Select(x => ProcessTemplate(localParameters, x)).ToList();
                }
                
                job.Steps.Add(jobStep);
            }
        }

        if (pipelineCode.CreateDeployment)
        {
            job.Stages.Add(new JobStage()
            {
                Name = "Post Deployment"
            });
        
            var deploymentStep = new JobStep()
            {
                Name = "deployment",
                Step = "deployment",
                Stage = "Post Deployment",
                Ordinal = ordinal,
            };
        
            job.Steps.Add(deploymentStep);
        }

        // TODO: Save Job
        var collection = _database.GetCollection<Job>("jobs");
        
        await collection.InsertOneAsync(job);

        return await _jobLogic.GetSafeJob(job.Id);
    }

    public async Task UpdateInternalData(Guid id, string key, string value)
    {
        var collection = _database.GetCollection<Thing>("things");
        
        var filter = Builders<Thing>.Filter
            .Eq(x => x.Id, id);

        var update = Builders<Thing>.Update
            .Set(x => x.InternalData[key], value);

        await collection.UpdateOneAsync(filter, update);
    }

    public static string ProcessTemplate(List<JobStepParameter> localParameters, string s)
    {
        var local = s;

        foreach (var parameter in localParameters)
        {
            local = local.Replace($"{{{parameter.Name}}}", parameter.Value);
        }
        
        return local;
    }
}