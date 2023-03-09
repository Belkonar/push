using System.Text.Json.Serialization;

namespace shared.Models.Pipeline;

public class PipelineFile
{
    [JsonPropertyName("key")]
    public string Key { get; set; }
    
    [JsonPropertyName("location")]
    public string Location { get; set; }

    [JsonPropertyName("isBinary")]
    public bool IsBinary { get; set; } = false;
    
    /// <summary>
    /// If this is true, users will *not* be able to
    /// provide their own version of this file
    /// </summary>
    [JsonPropertyName("isFixed")]
    public bool IsFixed { get; set; } = false;
}