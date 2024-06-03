using System.Net.WebSockets;
using DmhyAutoDownload.AriaRPC.Models.Results;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;

namespace DmhyAutoDownload.AriaRPC;

public class Aria2Rpc: IDisposable
{
    private readonly string _address;
    private readonly string _token;
    
    private readonly Mutex _mutex = new();
    
    private JsonRpc? _rpc;
    private IAria2Server _server;
    private IAria2System _system;
    
    #region Setup and Teardown
    private Aria2Rpc(string address, string token)
    {
        _address = address;
        _token = token;
    }
    
    public void Dispose()
    {
        try
        {
            _rpc?.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _rpc = null;
            _server = null!;
            _system = null!;
        }
    }

    private async Task InitAsync()
    {
        Console.WriteLine("Initializing Aria2 RPC...");
        Console.WriteLine("Connecting to web socket...");
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var socket = new ClientWebSocket();
        
        await socket.ConnectAsync(new Uri(_address), cancellationToken);
        Console.WriteLine("Connected to web socket. Establishing JSON-RPC protocol...");

        var rpc = new JsonRpc(new WebSocketMessageHandler(socket));
        
        _server = rpc.Attach<IAria2Server>(new JsonRpcProxyOptions
        {
            MethodNameTransform = AriaMethodNameTransforms.ScopedMethod("aria2"),
            EventNameTransform = AriaMethodNameTransforms.ScopedMethod("aria2")
        });
        _system = rpc.Attach<IAria2System>(new JsonRpcProxyOptions
        {
            MethodNameTransform = AriaMethodNameTransforms.ScopedMethod("system"),
            EventNameTransform = AriaMethodNameTransforms.ScopedMethod("system")
        });
        rpc.StartListening();
        _rpc = rpc;
        var version = (await _server.GetVersionAsync($"token:{_token}")).version;
        Console.WriteLine($"RPC initialized. Aric2 version: {version}");
    }
    
    public static async Task<Aria2Rpc> CreateAsync(string address, string token)
    {
        var rpc = new Aria2Rpc(address, token);
        await rpc.InitAsync();
        return rpc;
    }
    #endregion
    
    #region RPC Methods

    public async Task<GetVersionResult> GetVersionAsync()
    {
        return await _server.GetVersionAsync($"token:{_token}");
    }
    
    public async Task<string> AddUriAsync(string uri, params string[] mirrors)
    {
        var uris = mirrors.Prepend(uri).ToArray();
        return await _server.AddUriAsync($"token:{_token}", uris);
    }
    
    #endregion
}