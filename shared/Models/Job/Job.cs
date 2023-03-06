using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace shared.Models.Job;

/*
 * Job statuses are
 * pending, ready, active, done, error
 */

public class Job
{
    [JsonPropertyName("stages")]
    public List<JobStage> Stages { get; set; } = new();

    [JsonPropertyName("steps")]
    public List<JobStep> Steps { get; set; } = new();
    
    [JsonPropertyName("sourceControlUri")]
    public string SourceControlUri { get; set; }
    
    [JsonPropertyName("sourceReference")]
    public string SourceReference { get; set; }
}

/// <summary>
/// Stages for this model are pretty much just for display purposes
/// </summary>
public class JobStage
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class JobStep
{
    [JsonPropertyName("ordinal")]
    public int Ordinal { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "pending";

    [JsonPropertyName("statusReason")]
    public string StatusReason { get; set; } = "";
    
    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("parameters")]
    public List<JobStepParameter> Parameters { get; set; } = new();

    [JsonPropertyName("stepInfo")]
    public JobStepInfo? StepInfo { get; set; } = null;
}

public class JobStepInfo
{
    [JsonPropertyName("docker")]
    public string Docker { get; set; }
   
    [JsonPropertyName("remote")]
    public bool Remote { get; set; } = true;

    [JsonPropertyName("persist")]
    public bool Persist { get; set; } = true;
    
    [JsonPropertyName("commands")]
    public List<string> Commands { get; set; } = new();
}

public class JobStepParameter
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "string";
    
    [JsonPropertyName("subKind")]
    public string SubKind { get; set; } = "";
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
}