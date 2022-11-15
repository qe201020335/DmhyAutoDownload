using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DmhyAutoDownload;

public class RefresherService : IHostedService, IDisposable
{
    private readonly ILogger<RefresherService> _logger;
    private Timer? _timer;
    private Task? _refreshTask;
    private CancellationTokenSource? _tokenSource;
    
    public RefresherService(ILogger<RefresherService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RefresherService starting");

        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(Configuration.Instance.RefreshDelay));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        try
        {
            _refreshTask?.Wait(5000);
            _tokenSource?.Cancel();
            _refreshTask?.Dispose();
        }
        catch (Exception e)
        {
            if (e is not AggregateException && e is not ObjectDisposedException)
            {
                Console.WriteLine(e);
            }
        }

        _logger.LogInformation("RefresherService begin refresh");
        _tokenSource = new CancellationTokenSource();
        _refreshTask = Task.Factory.StartNew(() =>
        {
            BangumiManager.Instance.Test().Wait(_tokenSource.Token);
        });
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RefresherService stopping");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}