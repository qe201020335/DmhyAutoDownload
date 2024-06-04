using System.ComponentModel.DataAnnotations;
using DmhyAutoDownload.Core.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DmhyAutoDownload.Core.Data.Models;

[JsonObject(MemberSerialization.OptIn)]
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Finished))]
public class Bangumi
{
    public string BangumiId { get; set; } // the primary key
    
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