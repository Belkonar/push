using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper.Configuration.Annotations;

namespace data.ORM;

public class BaseDto
{
    [Column("created", TypeName = "timestamp")]
    public DateTime Created { get; set; }
    
    [Column("updated", TypeName = "timestamp")]
    public DateTime Updated { get; set; }
}