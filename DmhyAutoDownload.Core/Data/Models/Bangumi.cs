using System.ComponentModel.DataAnnotations;
using DmhyAutoDownload.Core.Data.Attributes;
using DmhyAutoDownload.Core.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DmhyAutoDownload.Core.Data.Models;

[JsonObject(MemberSerialization.OptIn)]
[Index(nameof(Finished))]
[PrimaryKey(nameof(Name))]
public class Bangumi
{
    [JsonProperty("Name", Required = Required.Always)]
    public string Name { get; set; } = ""; // the primary key
    
    [JsonProperty("Regex", Required = Required.Always)]
    public string Regex { get; set; } = "";

    [JsonProperty("RegexGroupIndex", Required = Required.DisallowNull)]
    public int RegexGroupIndex { get; set; } = 0;

    [JsonProperty("QueryKeyWord", Required = Required.Always)]
    public string QueryKeyWord { get; set; } = "";

    [JsonProperty("DownloadedEps", Required = Required.DisallowNull)]
    [Mapped]
    private List<string> DownloadedEps { get; } = [];  // a hashset would be better but EF Core was throwing exceptions
    
    private HashSet<int> _aaa = [1, 2, 3, 5];
    
    [JsonProperty("Finished", Required = Required.DisallowNull)]
    public bool Finished { get; set; } = false;

    public bool HadDownloaded(string link)
    {
        return string.IsNullOrWhiteSpace(link) || DownloadedEps.Contains(HashUtils.GetHashString(link));
    }

    public void AddDownloaded(string link)
    {
        if (string.IsNullOrWhiteSpace(link)) return;
        var hash = HashUtils.GetHashString(link);
        if (!DownloadedEps.Contains(hash))
        {
            DownloadedEps.Add(hash);
        }
    }

}