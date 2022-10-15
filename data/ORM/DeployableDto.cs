using data.Models;

namespace data.ORM;

public class DeployableDto : BaseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Deployable Contents { get; set; }
}