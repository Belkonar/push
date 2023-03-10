using scheduler;
using scheduler.Logic;
using scheduler.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<Github>();
        services.AddTransient<JobLogic>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
