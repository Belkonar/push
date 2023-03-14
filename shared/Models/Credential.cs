namespace shared.Models;

public class Credential
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid OrganizationId { get; set; }
    
    public string Name { get; set; }
    
    public string Kind { get; set; }
    
    /// <summary>
    /// If this is true, uses only global policies for access control.
    /// </summary>
    public bool IsManaged { get; set; }

    public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
}