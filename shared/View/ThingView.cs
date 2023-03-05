using System.Text.Json.Serialization;
using shared.Models;

namespace shared.View;

public class ThingView : BaseView
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("organization")]
    public Guid OrganizationId { get; set; }
    
    [JsonPropertyName("contents")]
    public Thing Contents { get; set; }
}