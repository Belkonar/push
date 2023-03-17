namespace shared.Models.Pipeline;

public class PipelineVersionCode
{
    public bool CreateDeployment { get; set; } = true;
    
    public List<Stage> Stages { get; set; } = new();

    public List<Step> Steps { get; set; } = new();
    
    public List<PipelineFile> Files { get; set; } = new();
    
    public List<StepParameter> Parameters { get; set; } = new();
}