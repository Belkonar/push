using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.ORM;

/// <summary>
/// Basic k/v config structure.
/// </summary>
/// <remarks>No view model for KV based tables</remarks>
[Table("config")]
public class ConfigDto : BaseDto
{
    [Key, Column("key")]
    public string Key { get; set; }
    
    [Column("value")]
    public string Value { get; set; }
}