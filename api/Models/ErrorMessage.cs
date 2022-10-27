using System.Text.Json.Serialization;

namespace api.Models;

public class ErrorMessage
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    public ErrorMessage(string message)
    {
        Message = message;
    }
}