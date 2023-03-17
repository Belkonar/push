using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace api;

public class TypeHelper
{
    public List<Type> GetTypes(string space)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(t => t.GetTypes())
            .Where(t => t.IsClass && t.Namespace == space && t.GetCustomAttribute<ApiControllerAttribute>() != null)
            .ToList();
    }
}