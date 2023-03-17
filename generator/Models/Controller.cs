namespace generator.Models;

public class ReflController
{
    public string Name { get; set; }

    public HashSet<string> Namespaces { get; set; } = new();

    public List<ReflMethod> Methods { get; set; } = new();
}

public class ReflMethod
{
    public string Name { get; set; }
    public string HttpMethod { get; set; }
    public string FullType { get; set; }
    public List<ReflParam> Parameters { get; set; } = new();
}

public class ReflParam
{
    public string Name { get; set; }
    
    public string FullType { get; set; }
}