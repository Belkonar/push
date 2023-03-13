using scheduler.Logic;

namespace scheduler;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _provider;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public Worker(ILogger<Worker> logger, IServiceProvider provider, IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _provider = provider;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using (var scope = _provider.CreateAsyncScope())
            {
                var logic = scope.ServiceProvider.GetRequiredService<JobLogic>();
                await logic.HandlePendingJobs();
                await logic.HandleReadyJobs(); // This basically just drops jobs on nomad if a step is ready
                await logic.HandleApprovalJobs();
            }
            
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            
            Thread.Sleep(1000);
        }
        
        // If the loop dies, just kill the app, but don't bother if it's already requested
        if (!stoppingToken.IsCancellationRequested)
        {
            _hostApplicationLifetime.StopApplication();    
        }
    }
}
