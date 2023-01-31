using System.Text.Json.Serialization;

namespace shared.UpdateModels;

public class UpdateOrganization
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}