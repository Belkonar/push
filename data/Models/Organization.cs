using System.Text.Json.Serialization;

namespace data.Models;

public class Organization
{
    /// <remarks>
    /// Requires org.update permission
    /// </remarks>
    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <remarks>
    /// Requires global.org.update.private_metadata permission
    /// </remarks>
    [JsonPropertyName("privateMetadata")]
    public Dictionary<string, string> PrivateMetadata { get; set; } = new();
    
    /// <remarks>
    /// Requires org.update.metadata permission
    /// </remarks>
    [JsonPropertyName("policy")]
    public string Policy { get; set; }
}