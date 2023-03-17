using System.Text.Json.Serialization;

namespace shared.Models;

public class PermissionQuery
{
    [JsonPropertyName("organization")]
    public Guid? Organization { get; set; }
    
    [JsonPropertyName("resource")]
    public Guid? Resource { get; set; }
    
    [JsonPropertyName("resourceType")]
    public string? ResourceType { get; set; }

    public PermissionQuery()
    {
        
    }

    public PermissionQuery(Guid organization)
    {
        Organization = organization;
    }
    
    public PermissionQuery(Guid organization, Guid resource, string resourceType)
    {
        Organization = organization;
        Resource = resource;
        ResourceType = resourceType;
    }
}