using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DmhyAutoDownload;

public class RefresherService : IHostedService, IDisposable
{
    private readonly ILogger<RefresherService> _logger;
    private readonly Configuration _config;
    private readonly BangumiManager _bangumiManager;

    private Timer? _timer;
    private Task? _refreshTask;
    private CancellationTokenSource? _tokenSource;

    public RefresherService(ILogger<RefresherService> logger, Configuration configuration,
        BangumiManager bangumiManager)
    {
        _logger = logger;
        _config = configuration;
        _bangumiManager = bangumiManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RefresherService starting");
        _timer = new Timer(Refresh, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(_config.RefreshDelay));
        return Task.CompletedTask;
    }

    private void Refresh(object? state)
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
                _logger.LogWarning("Failed to clean up previous refresh attempt: {Message}", e.Message);
                _logger.LogDebug("{Ex}", e);
            }
        }

        _logger.LogInformation("RefresherService begin refresh");
        _tokenSource = new CancellationTokenSource();
        _refreshTask = Task.Factory.StartNew(() => { _bangumiManager.Test().Wait(_tokenSource.Token); });
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