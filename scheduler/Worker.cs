using scheduler.Logic;

namespace scheduler;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _provider;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly JobLogic _jobLogic;

    public Worker(ILogger<Worker> logger, IServiceProvider provider, IHostApplicationLifetime hostApplicationLifetime, JobLogic jobLogic)
    {
        _logger = logger;
        _provider = provider;
        _hostApplicationLifetime = hostApplicationLifetime;
        _jobLogic = jobLogic;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _jobLogic.HandleReadyJobs(); // This basically just drops jobs on nomad if a step is ready
            await _jobLogic.HandleApprovalJobs();
            
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
