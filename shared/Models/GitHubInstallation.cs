using System.Text.Json.Serialization;

namespace shared.Models;

public class GithubInstallation
{
    [JsonPropertyName("target_type")]
    public string TargetType { get; set; }
    
    [JsonPropertyName("access_tokens_url")]
    public string AccessTokenUrl { get; set; }
    
    [JsonPropertyName("account")]
    public GithubInstallationAccount Account { get; set; }
}

public class GithubInstallationAccount
{
    [JsonPropertyName("login")]
    public string Login { get; set; }
}

public class GithubToken
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}
