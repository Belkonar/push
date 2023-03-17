using scheduler.Logic;

namespace scheduler;

/// <summary>
/// This worker handles pending jobs.
/// It's seperated out because pending jobs are expensive resource wise.
///
/// You can split these out and put this guy on a pretty high speed disk to help things out.
/// Regardless you'll need a decent amount of space.
/// </summary>
public class PendingWorker : BackgroundService
{
    private readonly ILogger<PendingWorker> _logger;
    private readonly IServiceProvider _provider;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public PendingWorker(ILogger<PendingWorker> logger, IServiceProvider provider, IHostApplicationLifetime hostApplicationLifetime)
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
            }
            
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
            
            Thread.Sleep(1000);
        }
        
        // If the loop dies, just kill the app, but don't bother if it's already requested
        if (!stoppingToken.IsCancellationRequested)
        {
            _hostApplicationLifetime.StopApplication();    
        }
    }
}