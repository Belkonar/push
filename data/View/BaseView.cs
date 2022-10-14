using System.Text.Json.Serialization;

namespace data.View;

public class BaseView
{
    [JsonPropertyName("created")]
    public DateTime Created { get; set; }
    
    [JsonPropertyName("updated")]
    public DateTime Updated { get; set; }
}