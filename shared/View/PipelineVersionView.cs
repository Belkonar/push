using System.Text.Json.Serialization;

namespace shared.View;

public class PipelineVersionView : BaseView
{
    // This is not generated but user supplied, semver for real versions
    // and anything else for dev versions
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    
}