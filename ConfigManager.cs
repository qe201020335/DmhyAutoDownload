using Newtonsoft.Json;

namespace DmhyAutoDownload;

public class ConfigManager
{
    internal static readonly ConfigManager Instance = new ();
    private const string ConfPath = @"./Config/DmhyAutoDownload.json";
    private const string ConfDir = @"./Config";

    private bool _initialized = false;
    private ConfigManager()
    {
        
    }

    internal void InitConfig()
    {
        try
        {
            if (File.Exists(ConfPath))
            {
                using var file = File.OpenText(ConfPath);
                var serializer = new JsonSerializer();
                Configuration.Instance = (Configuration?)serializer.Deserialize(file, typeof(Configuration)) ?? throw new Exception();
            }
            else
            {
                Configuration.Instance = new Configuration();

                if (!Directory.Exists(ConfDir))
                {
                    Directory.CreateDirectory(ConfDir);
                }
            }

            _initialized = true;

            FetchEnvVar();

            SaveConfig();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    internal void FetchEnvVar()
    {
        var addr = Environment.GetEnvironmentVariable("RPCADDR");
        if (addr != null)
        {
            Console.WriteLine($"RPCADDR: {addr}");
            Configuration.Instance.AriaRpc = addr;
        }
        var token = Environment.GetEnvironmentVariable("ARIATOKEN");
        if (token != null)
        {
            Console.WriteLine($"ARIATOKEN: {token}");
            Configuration.Instance.AriaToken = token;
        }
    }

    internal void SaveConfig()
    {
        if (!_initialized)
        {
            return;
        }
        try
        {
            using var file = File.CreateText(ConfPath);
            var serializer = new JsonSerializer();
            serializer.Serialize(file, Configuration.Instance);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}