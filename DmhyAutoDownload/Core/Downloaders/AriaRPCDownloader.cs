using DmhyAutoDownload.AriaRPC;
using DmhyAutoDownload.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace DmhyAutoDownload.Core.Downloaders;

public class AriaRPCDownloader: IBangumiDownloader, IDisposable
{
    private Aria2Rpc _rpc = null!;
    
    private readonly ILogger<AriaRPCDownloader> _logger;
    
    
    public AriaRPCDownloader(Configuration config, ILogger<AriaRPCDownloader> logger)
    {
        _logger = logger;
        _logger.LogInformation("Initializing AriaRPCDownloader");
        _rpc = Aria2Rpc.Create(config.AriaRpc, config.AriaToken);
    }

    public void Dispose()
    {
        _rpc?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task DownloadAsync(string uri)
    {
        var res = await _rpc.AddUriAsync(uri);
        _logger.LogInformation("Added download task, res: {Code}", res);
    }
    
    public async Task<string> GetInfoAsync()
    {
        _logger.LogInformation("Getting Info from Aria2 RPC");
        var version = await _rpc.GetVersionAsync();
        return $"Aria2 ({version.version}) with features: {string.Join(", ", version.enabledFeatures)}";
    }
}