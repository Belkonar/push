using AutoMapper.Configuration.Annotations;

namespace data.ORM;

public class BaseDto
{
    public DateTime Created { get; set; }
    
    public DateTime Updated { get; set; }
}