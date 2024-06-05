using DmhyAutoDownload.Core.Data;
using DmhyAutoDownload.Core.Data.Models;
using DmhyAutoDownload.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;

namespace DmhyAutoDownload.Core.Extensions;

public static class HostAppExtensions
{
    public static async Task DoCorePreRunChoresAsync(this IHost app)
    {
        await app.MigrateDatabaseAsync();
        await app.ImportOldDataAsync();
    }
    
    private static async Task MigrateDatabaseAsync(this IHost app)
    {
        Console.WriteLine("Migrating database");
        using var serviceScope = app.Services.CreateScope();
        await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<CoreDbContext>();
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Database migrated");
    }
    
    private static async Task ImportOldDataAsync(this IHost app)
    {
        Console.WriteLine("Importing old data");
        using var serviceScope = app.Services.CreateScope();
        var path = Path.GetFullPath(@"./Config/DmhyAutoDownload.json");
        try
        {
            if (File.Exists(path))
            {
                using var file = File.OpenText(path);
                var oldBangumis = JObject.Parse(await file.ReadToEndAsync())["Bangumis"]?.ToObject<List<Bangumi>>();
                if (oldBangumis != null && oldBangumis.Count > 0)
                {
                    var bangumiRepo = serviceScope.ServiceProvider.GetRequiredService<IBangumiRepository>();
                    foreach (var bangumi in oldBangumis)
                    {
                        await bangumiRepo.TryAddBangumiAsync(bangumi);
                    }
                }
                else
                {
                    Console.WriteLine("Old data not found");
                }
            }
            else
            {
                Console.WriteLine("Old data not found");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to import old data: {e}");
        }
    }
    
}