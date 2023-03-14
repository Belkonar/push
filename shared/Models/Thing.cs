using System.Text.Json.Serialization;

namespace shared.Models;

public class Thing
{
    public Guid? Id { get; set; }
    
    public string Name { get; set; }

    public Guid OrganizationId { get; set; }
    
    public Dictionary<string, string> Metadata { get; set; } = new();

    public Dictionary<string, string> PrivateMetadata { get; set; } = new();
    
    public Deployable? Deployable { get; set; }
    
    public Dictionary<string, string> InternalData { get; set; } = new();
}