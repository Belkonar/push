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
        
        // For updates with smaller surfaces
        CreateMap<UpdateOrganization, OrganizationDto>();
        CreateMap<UpdatePolicy, PolicyDto>();
    }
    
}