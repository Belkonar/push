using System.Text.Json.Serialization;

namespace shared.View;

public class PolicyView : BaseView
{
    [JsonPropertyName("key")]
    public string Key { get; set; }
    
    [JsonPropertyName("policy")]
    public string Policy { get; set; }
}