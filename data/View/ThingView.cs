using System.Text.Json.Serialization;
using data.Models;

namespace data.View;

public class ThingView : BaseView
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("contents")]
    public Thing Contents { get; set; }
}