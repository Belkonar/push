using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using shared.Models.Job;

namespace data.ORM;

[Table("job")]
public class JobDto : BaseDto
{
    [Key, Column("id")]
    public Guid Id { get; set; }

    [Column("thing")]
    public Guid ThingId { get; set; }

    [Column("status")]
    public string Status { get; set; } = "pending";
    
    [Column("statusReason")]
    public string StatusReason { get; set; } = "";

    [Column("contents", TypeName = "jsonb")]
    public Job Contents { get; set; }
}