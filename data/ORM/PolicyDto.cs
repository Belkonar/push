using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.ORM;

/// <summary>
/// Global policies for enterprise and reuse 
/// </summary>
/// <remarks>No view model for KV based tables</remarks>
[Table("policy")]
public class PolicyDto : BaseDto
{
    [Key, Column("key")]
    public string Key { get; set; }
    
    [Column("policy")]
    public string Policy { get; set; }
}