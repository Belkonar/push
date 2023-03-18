namespace shared.Models;

public class Stat
{
    public string Kind { get; set; }

    public long Value { get; set; } = 1;
    
    public DateTime Timestamp { get; set; } = DateTime.Now;

    public Dictionary<string, string> Metadata { get; set; } = new();
}