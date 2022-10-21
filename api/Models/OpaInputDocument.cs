using System.Text.Json;
using System.Text.Json.Serialization;

namespace api.Models;

public class OpaInputDocument
{
    /// <summary>
    /// List of permissions that builds over sources
    /// </summary>
    [JsonPropertyName("permissions")]
    public List<string> Permissions { get; set; } = new List<string>();

    /// <summary>
    /// Local groups (only filled out for org level or below)
    /// </summary>
    [JsonPropertyName("groups")]
    public List<string> Groups { get; set; } = new List<string>();
    
    /// <summary>
    /// The users actual user profile
    /// </summary>
    [JsonPropertyName("profile")]
    public JsonDocument Profile { get; set; }
}