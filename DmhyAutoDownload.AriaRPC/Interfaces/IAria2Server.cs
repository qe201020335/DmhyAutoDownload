using DmhyAutoDownload.AriaRPC.Models;
using DmhyAutoDownload.AriaRPC.Models.Results;

namespace DmhyAutoDownload.AriaRPC;

public interface IAria2Server 
{
    // event EventHandler<DownloadEventArgs> OnDownloadStart;
    // event EventHandler<DownloadEventArgs> OnDownloadPause;
    // event EventHandler<DownloadEventArgs> OnDownloadStop;
    // event EventHandler<DownloadEventArgs> OnDownloadComplete;
    // event EventHandler<DownloadEventArgs> OnDownloadError;
    // event EventHandler<DownloadEventArgs> OnBtDownloadComplete;
    
    Task<string> AddUriAsync(string secret, string[] uris);
    Task<GetVersionResult> GetVersionAsync(string secret);
}