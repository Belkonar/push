using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.ORM;

public class PipelineDTO : BaseDto
{
    [Key, Column("id")]
    public Guid Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("description")]
    public string Description { get; set; }
    
    [Column("organization")]
    public Guid? OrganizationId { get; set; }
}