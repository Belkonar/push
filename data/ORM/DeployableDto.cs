using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using data.Models;

namespace data.ORM;

[Table("deployable")]
public class DeployableDto : BaseDto
{
    [Key, Column("id")]
    public Guid Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("thing")]
    public Guid ThingId { get; set; }
    
    [Column("contents", TypeName = "jsonb")]
    public Deployable Contents { get; set; }
}