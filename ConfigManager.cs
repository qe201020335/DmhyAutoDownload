using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DmhyAutoDownload;

public class ConfigManager
{
    private const string ConfPath = @"./Config/DmhyAutoDownload.json";
    private const string ConfDir = @"./Config";
    private bool _initialized;

    private Configuration? _config;
    internal Configuration Config => _config ?? InitConfig();
    private readonly ILogger<ConfigManager> _logger;

    public ConfigManager(ILogger<ConfigManager> logger)
    {
        _logger = logger;
    }

    internal Configuration InitConfig()
    {
        try
        {
            if (File.Exists(ConfPath))
            {
                using var file = File.OpenText(ConfPath);
                var serializer = new JsonSerializer();
                _config = (Configuration?)serializer.Deserialize(file, typeof(Configuration)) ?? throw new Exception();
            }
            else
            {
                _config = new Configuration();

                if (!Directory.Exists(ConfDir))
                {
                    Directory.CreateDirectory(ConfDir);
                }
            }

            FetchEnvVar();
            SaveConfig();
            _initialized = true;
            return Config;
        }
        catch (Exception e)
        {
            _logger.LogError("Cannot initialize configuration: {Message}", e.Message);
            _logger.LogDebug("{Ex}", e);
            throw;
        }
    }

    internal void FetchEnvVar()
    {
        if (_config == null) return;
        var addr = Environment.GetEnvironmentVariable("RPCADDR");
        if (addr != null)
        {
            _logger.LogInformation("ENV | RPC Address: {Addr}", addr);
            _config.AriaRpc = addr;
        }

        var token = Environment.GetEnvironmentVariable("ARIATOKEN");
        if (token != null)
        {
            _logger.LogInformation("ENV | RPC Token: {Token}", token);
            _config.AriaToken = token;
        }

        var listen = Environment.GetEnvironmentVariable("LISTEN");
        if (listen != null)
        {
            _logger.LogInformation("ENV | HTTP Listen: {Listen}", listen);
            _config.ListenOn = listen;
        }
    }

    internal void SaveConfig()
    {
        _logger.LogInformation("Saving config");
        if (_config == null) return;
        try
        {
            using var file = File.CreateText(ConfPath);
            var serializer = new JsonSerializer();
            serializer.Serialize(file, _config);
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error saving config: {Message}", e.Message);
            _logger.LogDebug("{Ex}", e);
        }
    }
}