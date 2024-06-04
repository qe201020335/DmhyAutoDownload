using DmhyAutoDownload.Core.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DmhyAutoDownload.Core.Services;

public class RefresherService : IHostedService, IDisposable
{
    private readonly ILogger<RefresherService> _logger;
    private readonly int _refreshDelaySeconds;
    private readonly BangumiManager _bangumiManager;

    private Timer? _timer;
    private Task? _refreshTask;
    private CancellationTokenSource? _tokenSource;

    public RefresherService(ILogger<RefresherService> logger, AutoDownloadConfig config,
        BangumiManager bangumiManager)
    {
        _logger = logger;
        _refreshDelaySeconds = config.RefreshDelaySeconds;
        _bangumiManager = bangumiManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RefresherService starting, using refresh delay: {Delay} seconds", _refreshDelaySeconds);
        _timer = new Timer(Refresh, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(_refreshDelaySeconds));
        return Task.CompletedTask;
    }

    internal void Refresh(object? state)
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
        _refreshTask = Task.Factory.StartNew(() => { _bangumiManager.RefreshAndPush().Wait(_tokenSource.Token); });
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