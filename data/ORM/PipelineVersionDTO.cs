using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using shared.Models.Pipeline;

namespace data.ORM;

[Table("pipeline_version")]
public class PipelineVersionDTO : BaseDto
{
    [Key, Column("id")]
    public string Id { get; set; }
    
    [Column("contents", TypeName = "jsonb")]
    public PipelineVersionContents Contents { get; set; }
}