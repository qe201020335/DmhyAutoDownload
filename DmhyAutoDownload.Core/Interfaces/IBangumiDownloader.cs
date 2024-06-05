namespace DmhyAutoDownload.Core.Interfaces;

internal interface IBangumiDownloader
{
    Task DownloadAsync(string uri);

    Task<string> GetInfoAsync();
}