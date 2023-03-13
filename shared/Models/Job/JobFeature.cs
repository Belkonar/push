namespace shared.Models.Job;

public class JobFeature
{
    public string Name { get; set; }
    
    public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
}