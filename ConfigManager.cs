using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DmhyAutoDownload;

public class ConfigManager
{
    private const string ConfPath = @"./Config/DmhyAutoDownload.json";
    private const string ConfDir = @"./Config";
    private bool _initialized;

    private Configuration? _config;
    internal Configuration Config => _config ?? InitConfig();

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
            Console.WriteLine(e);
            throw;
        }
    }

    internal void FetchEnvVar()
    {
        if (_config == null) return;
        var addr = Environment.GetEnvironmentVariable("RPCADDR");
        if (addr != null)
        {
            Console.WriteLine($"RPCADDR: {addr}");
            _config.AriaRpc = addr;
        }

        var token = Environment.GetEnvironmentVariable("ARIATOKEN");
        if (token != null)
        {
            Console.WriteLine($"ARIATOKEN: {token}");
            _config.AriaToken = token;
        }
    }

    internal void SaveConfig()
    {
        if (_config == null) return;
        try
        {
            using var file = File.CreateText(ConfPath);
            var serializer = new JsonSerializer();
            serializer.Serialize(file, _config);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}