using System.Net.Http.Headers;
using scheduler;
using scheduler.Logic;
using scheduler.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<Github>();
        services.AddTransient<JobLogic>();

        if (Environment.GetEnvironmentVariable("WORKER") == "true")
        {
            Console.WriteLine("Loading up worker");
            services.AddHostedService<Worker>();
        }

        if (Environment.GetEnvironmentVariable("PENDING_WORKER") == "true")
        {
            services.AddHostedService<PendingWorker>();    
        }

        services.AddHttpClient("api", client =>
        {
            client.BaseAddress = new Uri("http://localhost:5183");
        });
        
        services.AddHttpClient("nomad", client =>
        {
            client.BaseAddress = new Uri("http://localhost:4646");
        });

        services.AddHttpClient("github", client =>
        {
        });

    })
    .Build();

host.Run();
