using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using DmhyAutoDownload.Core.Data.Models;
using DmhyAutoDownload.Core.Interfaces;
using DmhyAutoDownload.Core.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DmhyAutoDownload.Core;

public class BangumiManager
{
    const string QUERY_URL = @"http://share.dmhy.org/topics/rss/rss.xml?keyword=";

    private readonly Config _config;
    private readonly IBangumiDownloader _downloader;
    private readonly ILogger<BangumiManager> _logger;
    private readonly ConfigManager _configManager;

    public BangumiManager(ILogger<BangumiManager> logger, Config config, IBangumiDownloader downloader, ConfigManager configManager)
    {
        _logger = logger;
        _config = config;
        _downloader = downloader;
        _configManager = configManager;
    }

    internal async Task RefreshAndPush(bool skipFinished = true)
    {
        foreach (var bangumi in _config.BangumiList)
        {
            if (!skipFinished || !bangumi.Finished)
            {
                await RefreshAndPush(bangumi);
            }
        }

        _configManager.SaveConfig();
    }

    internal async Task RefreshAndPush(Bangumi bangumi)
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

        foreach (var item in feed.Items)
        {
            _logger.LogDebug("{Title}", item.Title.Text);
            var match = regex.Match(item.Title.Text);
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
                    await Push(bangumi, magnet);
                }
            }
        }
    }

    private async Task Push(Bangumi bangumi, Uri magnet)
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

public class Config
{
    [JsonProperty("Bangumis")]
    [JsonConverter(typeof(BangumiMapJsonConverter))]
    public readonly IDictionary<string, Bangumi> Bangumis = new Dictionary<string, Bangumi>();
    
    [JsonIgnore]
    public ICollection<Bangumi> BangumiList => Bangumis.Values;
    
}
