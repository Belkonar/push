using System.Text.Json.Serialization;

namespace shared.UpdateModels;

public class UpdateStatus
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    
    [JsonPropertyName("statusReason")]
    public string StatusReason { get; set; } = "";
}