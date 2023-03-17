using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using api.Models;
using shared;

namespace api.Services;

public class OpaService
{
    public async Task<OpaResponseMain> Query(string query, OpaInputDocument input)
    {
        using var tempDir = new TempFolder();
        
        await File.WriteAllTextAsync(tempDir.GetFile("policy.rego"), query);

        using Process process = new Process();
        
        process.StartInfo.FileName = "opa";
        process.StartInfo.Arguments = @"eval --format raw -d policy.rego --stdin-input ""data""";
        process.StartInfo.WorkingDirectory = tempDir.Dir;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
            
        process.Start();

        var inputJson = JsonSerializer.Serialize(input);
        
        Console.WriteLine(inputJson);

        await process.StandardInput.WriteLineAsync(inputJson);
        process.StandardInput.Close();
        
        await process.WaitForExitAsync();

        var response = await process.StandardOutput.ReadToEndAsync();

        if (process.ExitCode != 0)
        {
            // TODO: Error handling
            Console.WriteLine(response);
            return new OpaResponseMain();
        }

        var r = JsonSerializer.Deserialize<OpaResponse>(response.Trim());
        
        return r!.Main!;
    }

    public async Task<bool> HasPermission(string query, OpaInputDocument input, string permissionKey)
    {
        var main = await Query(query, input);

        return main.HasKey(permissionKey);
    }
}

public class OpaResponse
{
    [JsonPropertyName("main")]
    public OpaResponseMain? Main { get; set; }
}

public class OpaResponseMain
{
    [JsonPropertyName("parentPolicy")]
    public string? ParentPolicy { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }

    public List<string> Keys()
    {
        return ExtensionData?.Keys.ToList() ?? new List<string>();
    }

    public bool HasKey(string key)
    {
        return ExtensionData?.ContainsKey(key) ?? false;
    }
}