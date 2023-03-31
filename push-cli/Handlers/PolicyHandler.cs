using System.CommandLine;
using shared.Models;

namespace push_cli.Handlers;

public class PolicyHandler
{
    private readonly HttpClient _httpClient;

    public PolicyHandler(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("api");
    }
    
    private async Task Sync(DirectoryInfo dir)
    {
        var files = dir
            .GetFiles("*.yaml", SearchOption.AllDirectories)
            .Select(x => x.ToString())
            .ToList();
        
        foreach (var file in files)
        {
            var policy = new Policy()
            {
                Yaml = await File.ReadAllTextAsync(file)
            };

            var parts = Path.GetFileName(file).Split('.');

            if (parts.Length == 3)
            {
                policy.Ordinal = Convert.ToInt32(parts[0]);
                policy.Key = parts[1];
            }
            else
            {
                policy.Key = parts[0];
            }
        }
    }

    public void Setup(RootCommand rootCommand)
    {
        var directoryArgument = new Argument<DirectoryInfo>(
            "directory",
            description: "The directory to use",
            getDefaultValue: () => new DirectoryInfo(Directory.GetCurrentDirectory())
        );
        
        var policyCommand = new Command("policy");

        rootCommand.AddCommand(policyCommand);

        var policySyncCommand = new Command("sync");
        policySyncCommand.AddArgument(directoryArgument);

        policyCommand.AddCommand(policySyncCommand);

        policySyncCommand.SetHandler(async (dir) =>
        {
            await Sync(dir);
        }, directoryArgument);
    }
}
