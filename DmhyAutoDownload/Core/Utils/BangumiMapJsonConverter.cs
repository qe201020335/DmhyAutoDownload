using DmhyAutoDownload.Core.Data.Models;
using Newtonsoft.Json;

namespace DmhyAutoDownload.Core.Utils;

public class BangumiMapJsonConverter : JsonConverter<IDictionary<string, Bangumi>>
{
    public override void WriteJson(JsonWriter writer, IDictionary<string, Bangumi>? value, JsonSerializer serializer)
    {
        var aaa = value?.Values ?? new List<Bangumi>();
        serializer.Serialize(writer, aaa);
    }

    public override IDictionary<string, Bangumi> ReadJson(JsonReader reader, Type objectType, IDictionary<string, Bangumi>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var bangumiList = serializer.Deserialize<List<Bangumi>>(reader);
        var newMap = new Dictionary<string, Bangumi>(existingValue ?? new Dictionary<string, Bangumi>());
        foreach (var bangumi in bangumiList ?? new List<Bangumi>())
        {
            newMap[bangumi.Name] = bangumi;
        }
        return newMap;
    }
}