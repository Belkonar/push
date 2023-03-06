using System.Text.Json.Serialization;

namespace shared.UpdateModels;

public class UpdateOrganizationVariable
{
    [JsonPropertyName("key")]
    public string Key { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
}