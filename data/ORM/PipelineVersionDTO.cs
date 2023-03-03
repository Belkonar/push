using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using shared.Models.Pipeline;

namespace data.ORM;

[Table("pipeline_version")]
[PrimaryKey("Version", "PipelineId")]
public class PipelineVersionDTO : BaseDto
{
    [Column("version")]
    public string Version { get; set; }
    
    [Column("pipeline_id")]
    public Guid PipelineId { get; set; }
    
    [Column("contents", TypeName = "jsonb")]
    public PipelineVersionContents Contents { get; set; }
}