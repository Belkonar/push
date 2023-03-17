namespace generator.Models;

public class ReflController
{
    public string Name { get; set; }

    public HashSet<string> Namespaces { get; set; } = new();

    public List<ReflMethod> Methods { get; set; } = new();
    public string ShortName { get; set; }
}

public class ReflMethod
{
    public string Name { get; set; }
    public string HttpMethod { get; set; }
    public string FullType { get; set; }
    public List<ReflParam> Parameters { get; set; } = new();
    public string Route { get; set; }

    public bool IsGet
    {
        get
        {
            return HttpMethod == "Get";
        }
    }

    public string ReturnType
    {
        get
        {
            if (FullType.StartsWith("Task<"))
            {
                var t = FullType.Remove(0, 5);
                return t.Remove(t.Length - 1);
            }
            else
            {
                return FullType;
            }
        }
    }

    public bool HasReturn => FullType.StartsWith("Task<");
    public string BodyName { get; set; }
    public string Query { get; set; }

    public string DefaultResponse
    {
        get
        {
            if (ReturnType.StartsWith("List"))
            {
                return $"new {ReturnType}()";
            }
            else
            {
                return null!;
            }
        }
    }
}

public class ReflParam
{
    public string Name { get; set; }
    
    public string FullType { get; set; }
}