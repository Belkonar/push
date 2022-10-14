using data.Models;

namespace data.ORM;

/// <summary>
/// Top level resource as far as resource hierarchy.
/// </summary>
public class OrganizationDto : BaseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Organization Contents { get; set; }
}