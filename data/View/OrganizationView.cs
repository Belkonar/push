using System.Text.Json.Serialization;
using data.Models;

namespace data.View;

public class OrganizationView : BaseView
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("contents")]
    public Organization Contents { get; set; }
}