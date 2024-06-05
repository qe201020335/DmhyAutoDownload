using DmhyAutoDownload.Core.Data;
using DmhyAutoDownload.Core.Data.Models;
using DmhyAutoDownload.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace DmhyAutoDownload.Core.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MigrateDatabase(this WebApplication app)
    {
        Console.WriteLine("Migrating database");
        using var serviceScope = app.Services.CreateScope();
        await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<CoreDbContext>();
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Database migrated");
    }
    
    public static async Task ImportOldData(this WebApplication app)
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