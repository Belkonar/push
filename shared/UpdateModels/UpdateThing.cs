using System.Text.Json.Serialization;

namespace shared.UpdateModels;

public class UpdateThing
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("organization")]
    public Guid OrganizationId { get; set; }
}