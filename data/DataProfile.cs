using AutoMapper;
using data.ORM;
using data.View;

namespace data;

public class DataProfile : Profile
{
    public DataProfile()
    {
        CreateMap<OrganizationView, OrganizationDto>()
            .Setup();

        CreateMap<ThingView, ThingDto>()
            .Setup();
    }
    
}