using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DmhyAutoDownload;

public class DownloadManager
{
    private string RpcAddress => _config.AriaRpc;
    private string AriaToken => _config.AriaToken;
    private readonly HttpClient _client = new();

    private readonly ILogger<DownloadManager> _logger;
    private readonly Configuration _config;

    public DownloadManager(ILogger<DownloadManager> logger, Configuration config)
    {
        _logger = logger;
        _config = config;
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

        var parameter = requireToken ? new List<object> { $"token:{AriaToken}" } : new List<object>();
        parameter.AddRange(param);
        if (parameter.Count > 0)
        {
            var aaa = new JProperty("params", parameter);
            req.Add(aaa);
        }

        var res = await _client.PostAsync(RpcAddress, new StringContent(req.ToString()));
        res.EnsureSuccessStatusCode();
        _logger.LogTrace("{Res}", await res.Content.ReadAsStringAsync());
    }
}