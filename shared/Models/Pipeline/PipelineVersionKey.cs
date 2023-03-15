using System.Text.Json.Serialization;

namespace shared.Models.Pipeline;

public class PipelineVersionKey
{
    public string Version { get; set; }
    
    public Guid PipelineId { get; set; }
}