using System.ComponentModel.DataAnnotations;

namespace shared.Models.Job;

public class JobStep
{
    public int Ordinal { get; set; }

    public string Status { get; set; } = "pending";

    public string StatusReason { get; set; } = "";
    
    [Required]
    public string Name { get; set; }
    
    public List<JobStepParameter> Parameters { get; set; } = new();

    public JobStepInfo? StepInfo { get; set; } = null;

    public string Step { get; set; }

    public string Stage { get; set; }

    public List<string> Approvals { get; set; } = new();
    
    public int RequiredApprovals { get; set; } = 0;
}