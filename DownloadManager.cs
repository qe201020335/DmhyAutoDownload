using Newtonsoft.Json.Linq;

namespace DmhyAutoDownload;

public class DownloadManager
{
    internal static readonly DownloadManager Instance = new();
    private string RpcAddress => Configuration.Instance.AriaRpc;
    private string AriaToken => Configuration.Instance.AriaToken;
    
    private readonly HttpClient _client = new();
    private DownloadManager()
    {
    }

    internal async Task Push(Uri uri)
    {
        // todo push aria rpc
    }

    internal async Task Test()
    {
        await Push("system.listMethods", false);
        await Task.Delay(1000);
        await Push("aria2.tellActive", true);
    }

    private async Task Push(string method, bool requireToken, params object[] param)
    {
        var req = new JObject(
            new JProperty("jsonrpc", "2.0"),
            new JProperty("id", "1234"),
            new JProperty("method", method));

        var parameter = requireToken ? new List<object>{$"token:{AriaToken}"} : new List<object>();
        parameter.AddRange(param);
        if (parameter.Count > 0)
        {
            var aaa = new JProperty("params", parameter);
            req.Add(aaa);
        }

        var res = await _client.PostAsync(RpcAddress, new StringContent(req.ToString()));
        res.EnsureSuccessStatusCode();
        Console.WriteLine(await res.Content.ReadAsStringAsync());
    }
}