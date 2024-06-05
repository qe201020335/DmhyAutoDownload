using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using DmhyAutoDownload.Core.Data.Models;
using DmhyAutoDownload.Core.Interfaces;
using DmhyAutoDownload.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DmhyAutoDownload.Core;

internal class BangumiManager: IBangumiManager
{
    const string QUERY_URL = @"http://share.dmhy.org/topics/rss/rss.xml?keyword=";

    private readonly IServiceProvider _services;
    private readonly IBangumiDownloader _downloader;
    private readonly ILogger<BangumiManager> _logger;
    
    private CancellationTokenSource _refreshCts = new();

    public BangumiManager(ILogger<BangumiManager> logger, IBangumiDownloader downloader, IServiceProvider services)
    {
        _logger = logger;
        _downloader = downloader;
        _services = services;
    }
    
    public void TriggerRefresh()
    {
        _refreshCts.Cancel();
        var newCts = new CancellationTokenSource();
        _refreshCts = newCts;
        var newToken = newCts.Token;
        try
        {
            _ = Task.Run(async () =>
            {
                await RefreshAllAsync(newToken);
            }, newToken);
        }
        catch (Exception e)
        {
            _logger.LogCritical("Failed to start refresh: {Ex}", e);
        }
    }

    private async Task RefreshAllAsync(CancellationToken token)
    {
        _logger.LogInformation("Refreshing all unfinished bangumis");
        using var scope = _services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IBangumiRepository>();
        var unfinished = await repo.GetBangumisAsync(false);
        
        if (unfinished.Count == 0)
        {
            _logger.LogInformation("No unfinished bangumi found");
            return;
        }
        
        if (token.IsCancellationRequested) return;
        foreach (var bangumi in unfinished)
        { 
            if (token.IsCancellationRequested) return;
            await RefreshAsync(bangumi, token);
            await repo.TryUpdateBangumiAsync(bangumi);
        }
    }
    
    private async Task RefreshAsync(Bangumi bangumi, CancellationToken token)
    {
        _logger.LogDebug("{Name}, query: {QueryKeyWord}, regex: {Regex}", bangumi.Name, bangumi.QueryKeyWord,
            bangumi.Regex);
        SyndicationFeed feed;
        Regex regex = new Regex(bangumi.Regex);
        try
        {
            using var xmlReader = XmlReader.Create(QUERY_URL + bangumi.QueryKeyWord);
            feed = SyndicationFeed.Load(xmlReader);
        }
        catch (Exception e)
        {
            _logger.LogError("Error query bangumi: {Message}", e.Message);
            _logger.LogDebug("{Ex}", e);
            return;
        }
        
        if (token.IsCancellationRequested) return;

        foreach (var item in feed.Items)
        {
            _logger.LogDebug("{Title}", item.Title.Text);
            var match = regex.Match(item.Title.Text);
            if (token.IsCancellationRequested) return;
            _logger.LogDebug("{Match}", match.Success);
            if (match.Success)
            {
                var magnet = item.Links.FirstOrDefault(link => link.Uri.Scheme.Contains("magnet"))?.Uri;
                if (magnet == null)
                {
                    _logger.LogWarning("{Id} {Title}: Magnet Link Not Found!",
                        match.Groups[bangumi.RegexGroupIndex + 1], item.Title.Text);
                }
                else if (!bangumi.HadDownloaded(magnet.AbsoluteUri))
                {
                    _logger.LogInformation("{Id} {Title}: {Magnetic}", match.Groups[bangumi.RegexGroupIndex + 1],
                        item.Title.Text, magnet.AbsoluteUri.Substring(0, 50) + "...");
                    await DownloadEpAsync(bangumi, magnet);
                }
            }

            if (token.IsCancellationRequested) return;
        }
    }

    private async Task DownloadEpAsync(Bangumi bangumi, Uri magnet)
    {
        if (bangumi.HadDownloaded(magnet.AbsoluteUri)) return;

        try
        {
            await _downloader.DownloadAsync(magnet.ToString());
            bangumi.AddDownloaded(magnet.ToString());
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error adding download task: {Message}", e.Message);
            _logger.LogDebug("{Ex}", e);
        }
    }
}
