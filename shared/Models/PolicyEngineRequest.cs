using System.Text.Json;
using System.Text.Json.Serialization;

namespace shared.Models;

public class PolicyEngineDocument
{
    [JsonPropertyName("use")]
    public string? Use { get; set; }
    
    [JsonPropertyName("inherit")]
    public string? Inherit { get; set; }
    
    [JsonPropertyName("policies")]
    public List<PolicyEnginePolicy> Policies { get; set; }
}

public class PolicyEngineRequest
{
    [JsonPropertyName("policies")]
    public List<PolicyEnginePolicy> Policies { get; set; }
    
    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }
}

public class PolicyEnginePolicy
{
    [JsonPropertyName("permission")]
    public string Permission { get; set; }

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "allow";
    
    // This is basically a bag so that I can kinda deal with stuff lol
    [JsonPropertyName("rules")]
    public IEnumerable<JsonElement> Rules { get; set; }
}
