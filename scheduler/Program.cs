using scheduler;
using scheduler.Logic;
using scheduler.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<Github>();
        services.AddTransient<JobLogic>();
        services.AddHostedService<Worker>();

        services.AddHttpClient("api", client =>
        {
            client.BaseAddress = new Uri("http://localhost:5183");
        });
        
    })
    .Build();

host.Run();