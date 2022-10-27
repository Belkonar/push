using System.Text.Json;
using System.Text.Json.Serialization;

namespace api.Models;

public class UserProfile
{
    [JsonPropertyName("permissions")]
    public List<string> Permissions { get; set; }
    
    [JsonPropertyName("user")]
    public JsonDocument user { get; set; }
}