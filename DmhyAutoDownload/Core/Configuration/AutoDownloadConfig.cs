namespace DmhyAutoDownload.Core.Configuration;

public class AutoDownloadConfig
{
    internal const string Section = "AutoDownload";
    
    public string AriaWsRpcAddr { get; set; } = "ws://localhost:6800/jsonrpc";
    
    public string AriaSecretToken { get; set; } = "";

    public int RefreshDelaySeconds { get; set; } = 21600;
}