using AutoMapper;
using data.ORM;
using shared.UpdateModels;
using shared.View;

namespace data;

public class DataProfile : Profile
{
    public DataProfile()
    {
        CreateMap<OrganizationView, OrganizationDto>()
            .Setup();

        CreateMap<ThingView, ThingDto>()
            .Setup();
        
        CreateMap<PipelineView, PipelineDTO>()
            .Setup();
        
        CreateMap<PipelineVersionView, PipelineVersionDTO>()
            .Setup();
        
        CreateMap<DeployableView, DeployableDto>()
            .Setup();
        
        CreateMap<JobView, JobDto>()
            .Setup();
        
        // For updates with smaller surfaces
        CreateMap<UpdateOrganization, OrganizationDto>();
        CreateMap<UpdatePolicy, PolicyDto>();
        CreateMap<UpdateThing, ThingDto>();
    }
    
}