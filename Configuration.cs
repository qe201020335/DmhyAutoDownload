using Newtonsoft.Json;

namespace DmhyAutoDownload;

public class Configuration
{
    [JsonProperty("AriaRpc")]
    public string AriaRpc = "http://localhost:6800/jsonrpc";
    
    [JsonProperty("AriaToken")]
    public string AriaToken = "";

    [JsonProperty("RefreshDelay")]
    public int RefreshDelay = 21600;

    [JsonProperty("Bangumis")]
    public readonly List<Bangumi> Bangumis = new();
}

public class Bangumi
{
    [JsonProperty("Name")]
    public string Name { get; set; } = "";
    
    [JsonProperty("Regex")]
    public string Regex { get; set; } = "";

    [JsonProperty("RegexGroupIndex")]
    public int RegexGroupIndex { get; set; } = 0;

    [JsonProperty("QueryKeyWord")]
    public string QueryKeyWord { get; set; } = "";

    [JsonProperty("DownloadedEps")]
    public HashSet<string> DownloadedEps { get; set; } = new();

}