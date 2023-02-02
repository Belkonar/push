namespace shared.Models.Pipeline;

public class PipelineFile
{
    public string Key { get; set; }
    public string Location { get; set; }
    public bool Replaceable { get; set; } = true;
}