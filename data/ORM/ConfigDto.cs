using System.ComponentModel.DataAnnotations;

namespace data.ORM;

/// <summary>
/// Basic k/v config structure.
/// </summary>
public class ConfigDto : BaseDto
{
    [Key]
    public string Key { get; set; }
    public string Value { get; set; }
}