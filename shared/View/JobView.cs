using System.Text.Json.Serialization;
using shared.Models.Job;

namespace shared.View;

public class JobView : BaseView
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("thing")]
    public Guid ThingId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "pending";
    
    [JsonPropertyName("statusReason")]
    public string StatusReason { get; set; } = "";

    [JsonPropertyName("contents")]
    public Job Contents { get; set; }
}