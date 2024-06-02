using Newtonsoft.Json;
using DmhyAutoDownload.Core.Utils;

namespace DmhyAutoDownload.Core;

public class Configuration
{
    [JsonProperty("AriaRpc")]
    public string AriaRpc = "http://localhost:6800/jsonrpc";
    
    [JsonProperty("AriaToken")]
    public string AriaToken = "";

    [JsonProperty("RefreshDelay")]
    public int RefreshDelay = 21600;

    [JsonProperty("Bangumis")]
    [JsonConverter(typeof(BangumiMapJsonConverter))]
    public readonly IDictionary<string, Bangumi> Bangumis = new Dictionary<string, Bangumi>();
    
    [JsonIgnore]
    public ICollection<Bangumi> BangumiList => Bangumis.Values;
    
}

public class Bangumi
{
    [JsonProperty("Name", Required = Required.Always)]
    public string Name { get; set; } = "";
    
    [JsonProperty("Regex", Required = Required.Always)]
    public string Regex { get; set; } = "";

    [JsonProperty("RegexGroupIndex", Required = Required.DisallowNull)]
    public int RegexGroupIndex { get; set; } = 0;

    [JsonProperty("QueryKeyWord", Required = Required.Always)]
    public string QueryKeyWord { get; set; } = "";

    [JsonProperty("DownloadedEps", Required = Required.DisallowNull)]
    private HashSet<string> DownloadedEps { get; set; } = new();
    
    [JsonProperty("Finished", Required = Required.DisallowNull)]
    public bool Finished { get; set; } = false;

    public bool HadDownloaded(string link)
    {
        return string.IsNullOrWhiteSpace(link) || DownloadedEps.Contains(HashUtils.GetHashString(link));
    }

    public void AddDownloaded(string link)
    {
        if (string.IsNullOrWhiteSpace(link)) return;
        DownloadedEps.Add(HashUtils.GetHashString(link));
    }

}