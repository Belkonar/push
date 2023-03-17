namespace shared.Models;

public class GithubConfig
{
    public string AppId { get; set; }
// #pragma warning disable CS8618
    public string ApiRoot { get; set; }
// #pragma warning restore CS8618
    public string? PemLocation { get; set; } = "";
}