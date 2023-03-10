using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using shared.Models.Job;

namespace data.ORM;

[Table("job")]
public class JobDto : BaseDto
{
    

    [Column("contents", TypeName = "jsonb")]
    public Job Contents { get; set; } = new ();
}