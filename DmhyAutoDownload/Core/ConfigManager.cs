using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DmhyAutoDownload.Core;

public class ConfigManager
{
    private const string ConfPath = @"./Config/DmhyAutoDownload.json";
    private const string ConfDir = @"./Config";
    private bool _initialized;

    private Config? _config;
    internal Config Config => _config ?? InitConfig();
    private readonly ILogger<ConfigManager> _logger;

    public ConfigManager(ILogger<ConfigManager> logger)
    {
        _logger = logger;
    }

    internal Config InitConfig()
    {
        if (_initialized && _config != null) return _config;
        try
        {
            if (File.Exists(ConfPath))
            {
                using var file = File.OpenText(ConfPath);
                var serializer = new JsonSerializer();
                _config = (Config?)serializer.Deserialize(file, typeof(Config)) ?? throw new Exception();
            }
            else
            {
                _config = new Config();

                if (!Directory.Exists(ConfDir))
                {
                    Directory.CreateDirectory(ConfDir);
                }
            }

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