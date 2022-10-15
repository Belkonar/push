using System.Text.Json.Serialization;

namespace data.UpdateModels;

public class UpdateOrganization
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}