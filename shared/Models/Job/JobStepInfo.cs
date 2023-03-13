namespace shared.Models.Job;

public class JobStepInfo
{
    public string Docker { get; set; }
    
    public bool Remote { get; set; } = true;
    
    public bool Persist { get; set; } = true;
    
    public List<string> Commands { get; set; } = new();
    
    public ExecutorResponse? Output { get; set; } = null;
}