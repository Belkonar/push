using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.ORM;

public class PipelineVersionDTO
{
    [Key, Column("id")]
    public Guid Id { get; set; }
}