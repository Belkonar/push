using System.Text.Json.Serialization;

namespace api.Models;

public class UserProfile
{
    [JsonPropertyName("permissions")]
    public List<string> Permissions { get; set; }
}