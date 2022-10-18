using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper.Configuration.Annotations;

namespace data.ORM;

public class BaseDto
{
    [Column("created")]
    public DateTime Created { get; set; }
    
    [Column("updated")]
    public DateTime Updated { get; set; }
}