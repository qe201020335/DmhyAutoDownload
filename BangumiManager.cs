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

    public BangumiManager(ILogger<BangumiManager> logger, Configuration configuration, DownloadManager downloadManager)
    {
        _logger = logger;
        _config = configuration;
        _downloadManager = downloadManager;
    }

    internal async Task RefreshAndPush()
    {
        foreach (var bangumi in _config.Bangumis)
        {
            await RefreshAndPush(bangumi);
        }
    }

    internal async Task RefreshAndPush(Bangumi bangumi)
    {
        Console.WriteLine($"{bangumi.Name}, query: {bangumi.QueryKeyWord}, regex: {bangumi.Regex}");
        SyndicationFeed feed = null;
        Regex regex = new Regex(bangumi.Regex);
        try
        {
            using var xmlReader = XmlReader.Create(QUERY_URL + bangumi.QueryKeyWord);
            feed = SyndicationFeed.Load(xmlReader);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        foreach (var item in feed.Items)
        {
            var match = regex.Match(item.Title.Text);
            if (match.Success)
            {
                Console.Write($"{match.Groups[bangumi.RegexGroupIndex + 1]}  {item.Title.Text}  ");
                var magnet = item.Links.FirstOrDefault(link => link.Uri.Scheme.Contains("magnet"))?.Uri;
                if (magnet == null)
                {
                    Console.WriteLine("Magnet Line Not Found!");
                }
                else
                {
                    Console.WriteLine(magnet.AbsoluteUri.Substring(0, 50) + "...");
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
    }

    private async Task Push(Bangumi bangumi, Uri magnet)
    {
        if (bangumi.DownloadedEps.Contains(magnet.ToString())) return;

        await _downloadManager.Push(magnet);
        // bangumi.DownloadedEps.Add(magnet.ToString());
    }
}