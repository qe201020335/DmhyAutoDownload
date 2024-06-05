using DmhyAutoDownload.Core.Configuration;
using DmhyAutoDownload.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DmhyAutoDownload.Core.Services;

internal class RefresherService : IHostedService, IDisposable
{
    private readonly ILogger<RefresherService> _logger;
    private readonly int _refreshDelaySeconds;
    private readonly IBangumiManager _bangumiManager;

    private Timer? _timer;
    private Task? _refreshTask;
    private CancellationTokenSource? _tokenSource;

    public RefresherService(ILogger<RefresherService> logger, AutoDownloadConfig config,
        IBangumiManager bangumiManager)
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
        _logger.LogInformation("RefresherService triggering refresh");
        _bangumiManager.TriggerRefresh();
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