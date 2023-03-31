using System.CommandLine;
using push_cli.Handlers;
using shared.Interfaces;
using shared.services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<PolicyHandler>();
        services.AddSingleton<PipelineHandler>();
        
        services.AddHttpClient("api", client =>
        {
            client.BaseAddress = new Uri("http://localhost:5183");
        });

        services.AddSingleton<IPipelineController, PipelineService>();
        services.AddSingleton<IPolicyController, PolicyService>();
    })
    .Build();

var policyHandler = host.Services.GetService<PolicyHandler>()!;
var pipelineHandler = host.Services.GetService<PipelineHandler>()!;

var rootCommand = new RootCommand("Push CLI");

policyHandler.Setup(rootCommand);
pipelineHandler.Setup(rootCommand);

await rootCommand.InvokeAsync(args);
