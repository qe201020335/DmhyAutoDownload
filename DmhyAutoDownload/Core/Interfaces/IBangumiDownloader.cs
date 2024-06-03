namespace DmhyAutoDownload.Core.Interfaces;

public interface IBangumiDownloader
{
    Task InitializeAsync();
    
    Task DownloadAsync(string uri);

    Task<string> GetInfo();
}