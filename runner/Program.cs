using runner;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<Runner>();
        
        services.AddHttpClient("api", client =>
        {
            client.BaseAddress = new Uri("http://localhost:5183");
        });
        
        services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>()
            .CreateClient("api"));
    })
    .Build();

var runner = host.Services.GetService<Runner>()!;
await runner.Run(args);