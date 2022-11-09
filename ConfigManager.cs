using Newtonsoft.Json;

namespace DmhyAutoDownload;

public class ConfigManager
{
    internal static readonly ConfigManager Instance = new ConfigManager();
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
            SaveConfig();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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