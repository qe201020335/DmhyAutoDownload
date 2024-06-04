namespace DmhyAutoDownload.Core.Interfaces;

public interface IBangumiDownloader
{
    Task DownloadAsync(string uri);

    Task<string> GetInfoAsync();
}