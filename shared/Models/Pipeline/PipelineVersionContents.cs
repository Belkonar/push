using System.Text.Json.Serialization;

namespace shared.Models.Pipeline;

public class PipelineVersionContents
{
    /// <summary>
    /// Pre made files for the pipeline 
    /// </summary>
    /// TODO: In the future this will be just keys to an S3 bucket
    [JsonPropertyName("files")]
    public Dictionary<string,string> Files { get; set; }

    /// <summary>
    /// This is a calculated value for UI purposes
    ///
    /// providing it will do nothing, as it will be cleared and re-calculated on save
    /// </summary>
    [JsonPropertyName("compiledParameters")]
    public List<StepParameter> CompiledParameters { get; set; } = new();

    /// <summary>
    /// The actual pipeline code (in json)
    /// </summary>
    [JsonPropertyName("pipelineCode")]
    public PipelineVersion PipelineCode { get; set; }
}