namespace shared.View;

public class CredentialBundle
{
    public Dictionary<string, string> Headers { get; set; } = new();
    public Dictionary<string, string> Files { get; set; } = new();
}