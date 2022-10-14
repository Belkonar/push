using data.Models;

namespace data.ORM;

/// <summary>
/// This is the primary resource. It represents a deployable.
/// All other things in the system roll up to this. This doesn't however
/// insinuate that it *must* have things that roll up to it.
/// </summary>
public class ThingDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Thing Contents { get; set; }
}