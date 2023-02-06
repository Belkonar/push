using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using shared.Models.Pipeline;

namespace cli;

public class JsonHelper
{
    public static async Task<T> GetFile<T>(string path)
    {
        var jsonFile = File.ReadAllBytes(path);

        using var m = new MemoryStream(jsonFile);
        var pipeline = await JsonSerializer.DeserializeAsync<T>(m);
        m.Close();
        return pipeline;
    }

    public static bool IsValid(object o)
    {
        List<ValidationResult> results = new List<ValidationResult>(); 
        
        var valid = Validator.TryValidateObject(o, new ValidationContext(o), results, true);

        if (!valid)
        {
            Console.WriteLine("validation errors were found");
            foreach (var validationResult in results)
            {
                Console.WriteLine(validationResult);
            }

            return false;
        }

        return true;
    }
}