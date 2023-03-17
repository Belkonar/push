using AutoMapper;
using shared.Models.Job;
using shared.Models.Pipeline;

namespace shared;

public class DataProfile : Profile
{
    public DataProfile()
    {
        // For updates with smaller surfaces
        // CreateMap<UpdateOrganization, OrganizationDto>();
        // CreateMap<UpdatePolicy, PolicyDto>();
        // CreateMap<UpdateThing, ThingDto>();

        CreateMap<StepParameter, JobStepParameter>();
        CreateMap<Step, JobStepInfo>();
    }
    
}