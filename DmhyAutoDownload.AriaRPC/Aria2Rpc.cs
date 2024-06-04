using System.Net.WebSockets;
using DmhyAutoDownload.AriaRPC.Models.Results;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
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
        DisposeInternal();
        GC.SuppressFinalize(this);
    }

    private void DisposeInternal()
    {
        Console.WriteLine("Disposing Aria2 RPC...");
        try
        {
            _rpc!.Disconnected -= OnRpcDisconnected;
            _rpc.Dispose();
        }
        catch (NullReferenceException)
        {
            // ignored
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

    private async Task InitAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Initializing Aria2 RPC...");
        Console.WriteLine("Connecting to web socket...");
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
        
        rpc.Disconnected += OnRpcDisconnected;
        
        _rpc = rpc;
        var version = (await _server.GetVersionAsync($"token:{_token}")).version;
        Console.WriteLine($"RPC initialized. Aric2 version: {version}");
    }
    
    private CancellationTokenSource _resetCts = new();
    
    private void OnRpcDisconnected(object? sender, JsonRpcDisconnectedEventArgs e)
    {
        if (sender != _rpc)
        {
            return;
        }
        
        Console.WriteLine("Aria2 RPC got disconnected, resetting in 5 seconds...");
        _ = Task.Run(async () =>
        {
            await _resetCts.CancelAsync();
            var source = new CancellationTokenSource();
            var token = source.Token;
            _resetCts = source;
            var connected = false;
            while (!token.IsCancellationRequested && !connected)
            {
                try
                {
                    await Task.Delay(5000, token);
                    await ResetAsync(token);
                    connected = true;
                }
                catch (OperationCanceledException exception)
                {
                    break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Failed to reset Aria2 RPC: {exception}");
                    Console.WriteLine("Retrying in 5 seconds...");
                }
            }
        });
    }

    public async Task ResetAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Aria2 RPC Resetting...");
        DisposeInternal();
        await InitAsync(cancellationToken);
    }
    
    public static async Task<Aria2Rpc> CreateAsync(string address, string token)
    {
        var rpc = new Aria2Rpc(address, token);
        await rpc.InitAsync();
        return rpc;
    }
    
    public static Aria2Rpc Create(string address, string token)
    {
        var rpc = new Aria2Rpc(address, token);
        new JoinableTaskFactory(new JoinableTaskContext()).Run(() => rpc.InitAsync());
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