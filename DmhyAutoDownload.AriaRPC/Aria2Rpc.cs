using System.Net.WebSockets;
using DmhyAutoDownload.AriaRPC.Models.Results;
using Microsoft.VisualStudio.Threading;
using StreamJsonRpc;

namespace DmhyAutoDownload.AriaRPC;

public class Aria2Rpc: IDisposable
{
    private readonly string _address;
    private readonly string _token;
    
    private JsonRpc? _rpc;
    
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
        }
    }

    private async Task InitAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Initializing Aria2 RPC...");
        Console.WriteLine("Connecting to web socket...");
        var socket = new ClientWebSocket
        {
            Options =
            {
                // disable the pong messages which would cause invalid requests
                // and cause the connection to be dropped
                KeepAliveInterval = Timeout.InfiniteTimeSpan  
            }
        };
        
        await socket.ConnectAsync(new Uri(_address), cancellationToken);
        Console.WriteLine("Connected to web socket. Establishing JSON-RPC protocol...");

        var rpc = new JsonRpc(new WebSocketMessageHandler(socket));
        
        
        rpc.StartListening();
        
        rpc.Disconnected += OnRpcDisconnected;
        
        _rpc = rpc;
        var version = (await GetVersionAsync()).version;
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
    
    private async Task<T> CallNoLogAsync<T>(string method, params object[] args)
    {
        return await _rpc!.InvokeAsync<T>(method, args);
    }
    
    private async Task<T> CallAsync<T>(string method, params object[] args)
    {
        Console.WriteLine($"Calling RPC method: {method} with args: {string.Join(", ", args)}");
        return await CallNoLogAsync<T>(method, args);
    }
    
    private async Task<T> CallProtectedAsync<T>(string method, params object[] args)
    {
        Console.WriteLine($"Calling protected RPC method: {method} with args: {string.Join(", ", args)}");
        return await CallNoLogAsync<T>(method, [$"token:{_token}", .. args]);
    }

    public async Task<GetVersionResult> GetVersionAsync()
    {
        return await CallProtectedAsync<GetVersionResult>("aria2.getVersion");
    }
    
    public async Task<string> AddUriAsync(string uri, params string[] mirrors)
    {
        var uris = mirrors.Prepend(uri).ToArray();
        return await CallProtectedAsync<string>("aria2.addUri", [uris]);
    }
    
    #endregion
}