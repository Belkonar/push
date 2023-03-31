using System.Diagnostics;
using System.Reflection;
using System.Text;
using api;
using generator.Models;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using MongoDB.Driver.Linq;

namespace generator;

public class Generation
{
    public void Run()
    {
        var helper = new TypeHelper();

        var assemblies = helper.GetTypes("api.Controllers");
        var interfaceTemplate = Handlebars.Compile(File.ReadAllText("interface.handlebars"));
        var serviceTemplate = Handlebars.Compile(File.ReadAllText("service.handlebars"));

        foreach (var t in assemblies)
        {
            var controller = new ReflController()
            {
                Name = t.Name,
                ShortName = t.Name.Replace("Controller", "")
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
                reflMethod.Route = getAttr.Template!;

                var queries = method.GetParameters()
                    .Where(x => x.GetCustomAttribute<FromQueryAttribute>() != null)
                    .Select(x => $"{x.Name}={{{x.Name}}}")
                    .ToList();

                if (queries.Count > 0)
                {
                    reflMethod.Query = "?" + string.Join("&", queries);
                }

                var bodyType = method
                    .GetParameters()
                    .FirstOrDefault(x => x.GetCustomAttribute<FromBodyAttribute>() != null);

                if (bodyType != null)
                {
                    reflMethod.BodyName = bodyType.Name!;
                }

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
            
            controller.Namespaces.Add("shared.Interfaces");
            controller.Namespaces.Add("System.Net.Http.Json");
            
            File.WriteAllText($"../shared/services/{controller.ShortName}Service.cs", serviceTemplate(controller));
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
        return toString
            .Replace("String", "string")
            .Replace("Int32", "int");
    }

    private string GetCleanName(string input)
    {
        return input.Replace("`1", "")
            .Replace("`2", "");
    }

    private string MapMethod(string input)
    {
        return input switch
        {
            "GET" => "Get",
            "POST" => "Post",
            "PUT" => "Put",
            "DELETE" => "Delete",
            _ => throw new Exception("METHOD NOT FOUND")
        };
    }
}
