using Newtonsoft.Json;

namespace DmhyAutoDownload;

public class Configuration
{
    [JsonIgnore] public static Configuration Instance;

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