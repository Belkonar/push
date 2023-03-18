using System.Text.Json.Serialization;

namespace shared.Models.Nomad;

public class NomadJobRequest
{
    [JsonPropertyName("Job")]
    public NomadJob Job { get; set; } = new NomadJob();
}

public class NomadJob
{
    [JsonPropertyName("ID")]
    public string Id { get; set; }
    
    [JsonPropertyName("Name")]
    public string Name { get; set; }
    
    [JsonPropertyName("Type")]
    public string Type { get; set; } = "batch";
    
    [JsonPropertyName("Region")]
    public string Region { get; set; } = "global";
    
    [JsonPropertyName("Datacenters")]
    public List<string> DataCenters { get; set; } = new List<string>() { "dc1" };
    
    [JsonPropertyName("TaskGroups")]
    public List<NomadJobTaskGroup> TaskGroups { get; set; } = new List<NomadJobTaskGroup>();
}

public class NomadJobTaskGroup
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = "run step";

    [JsonPropertyName("Tasks")]
    public List<NomadTask> Tasks { get; set; } = new List<NomadTask>();
}

public class NomadTask
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = "run step";
    
    [JsonPropertyName("Driver")]
    public string Driver { get; set; } = "raw_exec";

    [JsonPropertyName("Config")]
    public NomadTaskConfig Config { get; set; } = new();

    // [JsonPropertyName("Resources")]
    // public NomadResourceBlock Resources { get; set; } = new();
}

public class NomadResourceBlock
{
    [JsonPropertyName("CPU")]
    public int Cpu { get; set; } = 110;
    
    [JsonPropertyName("MemoryMB")]
    public int Memory { get; set; } = 300;
}

public class NomadTaskConfig
{
    [JsonPropertyName("command")]
    public string Command { get; set; }
    
    [JsonPropertyName("args")]
    public List<string> Arguments { get; set; }
}