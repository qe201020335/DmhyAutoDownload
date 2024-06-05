using DmhyAutoDownload.Core.Configuration;
using DmhyAutoDownload.Core.Data;
using DmhyAutoDownload.Core.Downloaders;
using DmhyAutoDownload.Core.Interfaces;
using DmhyAutoDownload.Core.Services;
using DmhyAutoDownload.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DmhyAutoDownload.Core.Extensions;

public static class CoreServiceCollectionExtensions
{
    internal static IServiceCollection UseAutoDownloadCoreService(this IServiceCollection services)
    {
        services
            .BindConfiguration<AutoDownloadConfig>(AutoDownloadConfig.Section)
            .ConfigureCoreData()
            .AddCoreDependencyGroup()
            .RegisterCoreService();

        return services;
    }
    
    private static IServiceCollection ConfigureCoreData(this IServiceCollection services)
    {
        var path = services.BuildServiceProvider().GetRequiredService<AutoDownloadConfig>().SqliteDbPath;
        services.AddDbContext<CoreDbContext>(
            options => options.UseSqlite($"Data Source={path}"));
        return services;
    }

    private static IServiceCollection AddCoreDependencyGroup(this IServiceCollection services)
    {
        services
            .AddSingleton<IBangumiDownloader, AriaRPCDownloader>()
            .AddScoped<IBangumiRepository, BangumiRepository>()
            .AddSingleton<IBangumiManager, BangumiManager>();

        return services;
    }

    private static IServiceCollection RegisterCoreService(this IServiceCollection services)
    {
        services
            .AddHostedService<RefresherService>()
            .AddHostedService<DownloaderInfoCheck>();

        return services;
    }
}