namespace shared.Models.Job;

public class JobStepParameter
{
    public string Name { get; set; }
    
    public string Kind { get; set; } = "string";
    
    public string SubKind { get; set; } = "";
    
    public string Value { get; set; }
}