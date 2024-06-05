using DmhyAutoDownload.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DmhyAutoDownload.Core.Services;

public class DownloaderInfoCheck: IHostedService
{
    private readonly IBangumiDownloader _downloader;

    private readonly ILogger<DownloaderInfoCheck> _logger;
    
    public DownloaderInfoCheck(IBangumiDownloader downloader, ILogger<DownloaderInfoCheck> logger)
    {
        _downloader = downloader;
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var info = await _downloader.GetInfoAsync();
        _logger.LogInformation("Services started using bangumi downloader: {Info}", info);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}