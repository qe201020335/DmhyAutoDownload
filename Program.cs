
namespace DmhyAutoDownload;
internal class Program
{
    public static async Task Main(string[] args)
    {
        
        Console.WriteLine("Hello, World!");
        ConfigManager.Instance.InitConfig();

        await BangumiManager.Instance.RefreshAndPush();
        
        
        ConfigManager.Instance.SaveConfig();
    }
}