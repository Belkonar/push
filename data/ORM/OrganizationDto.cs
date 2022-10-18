using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using data.Models;

namespace data.ORM;

/// <summary>
/// Top level resource as far as resource hierarchy.
/// </summary>
[Table("organization")]
public class OrganizationDto : BaseDto
{
    [Key, Column("id")]
    public Guid Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("contents", TypeName = "jsonb")]
    public Organization Contents { get; set; }
}