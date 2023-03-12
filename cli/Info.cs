using System.Text.Json.Serialization;

namespace cli;

public class Info
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("files")]
    public Dictionary<string,string> Files { get; set; }
}