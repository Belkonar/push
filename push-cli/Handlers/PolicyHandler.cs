using System.CommandLine;
using shared.Interfaces;
using shared.Models;

namespace push_cli.Handlers;

public class PolicyHandler
{
    private readonly IPolicyController _policyService;

    public PolicyHandler(IPolicyController policyService)
    {
        _policyService = policyService;
    }
    
    private async Task Sync(DirectoryInfo dir)
    {
        var policies = dir
            .GetFiles("*.yaml", SearchOption.AllDirectories)
            .Select(x => x.ToString())
            .Select(file =>
            {
                var policy = new Policy()
                {
                    Yaml = File.ReadAllText(file)
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

                return policy;
            })
            .ToList();

        await _policyService.Sync(policies);
    }

    public void Setup(RootCommand rootCommand)
    {
        var directoryArgument = new Argument<DirectoryInfo>(
            "directory",
            description: "The directory to use",
            getDefaultValue: () => new DirectoryInfo(Directory.GetCurrentDirectory()));
        
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
