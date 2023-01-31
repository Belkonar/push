using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using shared.Models;

namespace data.ORM;

/// <summary>
/// This is the primary resource. It represents a deployable.
/// All other things in the system roll up to this. This doesn't however
/// insinuate that it *must* have things that roll up to it.
/// </summary>
[Table("thing")]
public class ThingDto : BaseDto
{
    [Key, Column("id")]
    public Guid Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("organization")]
    public Guid OrganizationId { get; set; }
    
    [Column("contents", TypeName = "jsonb")]
    public Thing Contents { get; set; }
}