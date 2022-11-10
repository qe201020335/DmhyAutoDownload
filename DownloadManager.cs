namespace DmhyAutoDownload;

public class DownloadManager
{
    internal static readonly DownloadManager Instance = new ();
    
    
    private DownloadManager()
    {
        
    }
    
    internal async Task Push(Uri uri)
    {
        // todo push aria rpc
    }
}