using DmhyAutoDownload.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace DmhyAutoDownload.Core.Services;

public class DownloaderInitializer: IHostedService
{
    private readonly IBangumiDownloader _downloader;
    
    public DownloaderInitializer(IBangumiDownloader downloader)
    {
        _downloader = downloader;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _downloader.InitializeAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}