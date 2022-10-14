namespace data.ORM;

/// <summary>
/// Basic k/v config structure.
/// </summary>
public class ConfigDto : BaseDto
{
    public string Key { get; set; }
    public string Value { get; set; }
}