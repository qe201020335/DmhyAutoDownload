using System.Diagnostics;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;

namespace DmhyAutoDownload;
internal class Program
{
    public static async Task Main(string[] args)
    {
        
        const string QUERY_URL = @"http://share.dmhy.org/topics/rss/rss.xml?keyword=";
        Console.WriteLine("Hello, World!");
        ConfigManager.Instance.InitConfig();

        // Configuration.Instance.Bangumis.Add(new()
        // {
        //     Name = "Chainsaw Man",
        //     QueryKeyWord = "电锯人 Chainsaw Man",
        //     Regex = @"^.织梦字幕组..+(\d\d)集.+(简日双语|1080).+(简日双语|1080).+$"
        // });

        foreach (var bangumi in Configuration.Instance.Bangumis)
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
                    Console.Write(item.Links.FirstOrDefault(link => link.Uri.Scheme.Contains("magnet"))?.Uri.AbsoluteUri.Substring(0, 50) + "...");
                    Console.WriteLine();
                }
            }
        }
        
        ConfigManager.Instance.SaveConfig();
    }
}