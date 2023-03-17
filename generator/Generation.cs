using System.Diagnostics;
using System.Reflection;
using System.Text;
using api;
using generator.Models;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc.Routing;

namespace generator;

public class Generation
{
    public void Run()
    {
        var helper = new TypeHelper();

        var assemblies = helper.GetTypes("api.Controllers");
        var interfaceTemplate = Handlebars.Compile(File.ReadAllText("interface.handlebars"));

        foreach (var t in assemblies)
        {
            var controller = new ReflController()
            {
                Name = t.Name
            };

            if (controller.Name == "ErrorController")
            {
                // don't map that one
                continue;
            }

            foreach (var method in t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
            {
                var reflMethod = new ReflMethod()
                {
                    Name = method.Name,
                    FullType = GetFullType(method.ReturnType, controller.Namespaces)
                };
        
                controller.Methods.Add(reflMethod);
        
                var getAttr = method.GetCustomAttribute<HttpMethodAttribute>();

                if (getAttr == null)
                {
                    continue;
                }

                reflMethod.HttpMethod = MapMethod(getAttr.HttpMethods.First());

                foreach (var parameter in method.GetParameters())
                {
                    var param = new ReflParam()
                    {
                        Name = parameter.Name,
                        FullType = GetFullType(parameter.ParameterType, controller.Namespaces)
                    };
                    
                    reflMethod.Parameters.Add(param);
                }
            }

            controller.Namespaces.Remove("System.Threading.Tasks");
            controller.Namespaces.Remove("System.Collections.Generic");
            controller.Namespaces.Remove("System");
            controller.Namespaces.Remove("shared");
            
            File.WriteAllText($"../shared/interfaces/I{t.Name}.cs", interfaceTemplate(controller));
        }
    }

    public string GetFullType(Type t, HashSet<string> spaces)
    {
        var builder = new StringBuilder();
        spaces.Add(t.Namespace!);

        if (t.IsGenericType)
        {
            builder.Append(GetCleanName(t.Name));

            var typeParams = t.GenericTypeArguments
                .Select(x => GetFullType(x, spaces))
                .ToList();

            builder.Append($"<{string.Join(",", typeParams)}>");
        }
        else
        {
            builder.Append(t.Name);
        }

        return Fix(builder.ToString());
    }

    private string Fix(string toString)
    {
        toString = toString
            .Replace("String", "string")
            .Replace("Int32", "int");

        return toString;
    }

    private string GetCleanName(string input)
    {
        return input.Replace("`1", "")
            .Replace("`2", "");
    }

    private string MapMethod(string input)
    {
        switch(input)
        {
            case "GET":
                return "Get";
            case "POST":
                return "Post";
            case "PUT":
                return "Put";
            case "DELETE":
                return "Delete";
            default:
                throw new Exception("METHOD NOT FOUND");
        }
    }
}