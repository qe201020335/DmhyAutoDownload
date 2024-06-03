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
    
    private readonly Configuration _config;
    
    public AriaRPCDownloader(Configuration config, ILogger<AriaRPCDownloader> logger)
    {
        _config = config;
        _logger = logger;
    }

    public void Dispose()
    {
        _rpc?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing AriaRPCDownloader");
        _rpc = await Aria2Rpc.CreateAsync(_config.AriaRpc, _config.AriaToken);
        _logger.LogInformation("AriaRPCDownloader initialized - {Info}", await GetInfo());
    }

    public async Task DownloadAsync(string uri)
    {
        var res = await _rpc.AddUriAsync(uri);
        _logger.LogInformation("Added download task, res: {Code}", res);
    }
    
    public async Task<string> GetInfo()
    {
        var version = await _rpc.GetVersionAsync();
        return $"Aria2 ({version.version}) with features: {string.Join(", ", version.enabledFeatures)}";
    }
}