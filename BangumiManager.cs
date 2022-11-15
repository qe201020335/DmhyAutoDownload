using System.Xml;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace DmhyAutoDownload;

public class BangumiManager
{
    const string QUERY_URL = @"http://share.dmhy.org/topics/rss/rss.xml?keyword=";

    private readonly Configuration _config;
    private readonly DownloadManager _downloadManager;
    private readonly ILogger<BangumiManager> _logger;
    private readonly ConfigManager _configManager;

    public BangumiManager(ILogger<BangumiManager> logger, Configuration configuration, DownloadManager downloadManager, ConfigManager configManager)
    {
        _logger = logger;
        _config = configuration;
        _downloadManager = downloadManager;
        _configManager = configManager;
    }

    internal async Task RefreshAndPush()
    {
        foreach (var bangumi in _config.Bangumis)
        {
            await RefreshAndPush(bangumi);
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
            var match = regex.Match(item.Title.Text);
            if (match.Success)
            {
                var magnet = item.Links.FirstOrDefault(link => link.Uri.Scheme.Contains("magnet"))?.Uri;
                if (magnet == null)
                {
                    _logger.LogWarning("{Id} {Title}: Magnet Link Not Found!",
                        match.Groups[bangumi.RegexGroupIndex + 1], item.Title.Text);
                }
                else if (!bangumi.DownloadedEps.Contains(magnet.AbsoluteUri))
                {
                    _logger.LogInformation("{Id} {Title}: {Magnetic}", match.Groups[bangumi.RegexGroupIndex + 1],
                        item.Title.Text, magnet.AbsoluteUri.Substring(0, 50) + "...");
                    await Push(bangumi, magnet);
                }
            }
        }
    }

    internal async Task Test()
    {
        var bangumi = new Bangumi
        {
            Name = "Chainsaw Man",
            QueryKeyWord = "电锯人 Chainsaw Man",
            Regex = @"^.织梦字幕组..+(\d\d)集.+(简日双语|1080).+(简日双语|1080).+$"
        };

        await RefreshAndPush(bangumi);
        await _downloadManager.Test();
    }

    private async Task Push(Bangumi bangumi, Uri magnet)
    {
        if (bangumi.DownloadedEps.Contains(magnet.ToString())) return;

        try
        {
            await _downloadManager.Push(magnet);
            bangumi.DownloadedEps.Add(magnet.ToString());
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error pushing to aria: {Message}", e.Message);
            _logger.LogDebug("{Ex}", e);
        }
    }
}