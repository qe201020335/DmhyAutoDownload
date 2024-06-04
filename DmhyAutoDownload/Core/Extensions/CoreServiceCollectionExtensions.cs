using DmhyAutoDownload.Core.Configuration;
using DmhyAutoDownload.Core.Data;
using DmhyAutoDownload.Core.Downloaders;
using DmhyAutoDownload.Core.Interfaces;
using DmhyAutoDownload.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DmhyAutoDownload.Core.Extensions;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection ConfigureCoreData(this IServiceCollection services)
    {
        var path = services.BuildServiceProvider().GetRequiredService<AutoDownloadConfig>().SqliteDbPath;
        services.AddDbContext<CoreDbContext>(
            options => options.UseSqlite($"Data Source={path}"));
        return services;
    }

    public static IServiceCollection AddCoreDependencyGroup(this IServiceCollection services)
    {
        services
            .AddSingleton<ConfigManager, ConfigManager>()
            .AddSingleton<Config>(provider => provider.GetRequiredService<ConfigManager>().Config)
            .AddSingleton<IBangumiDownloader, AriaRPCDownloader>()
            .AddScoped<IBangumiRepository, BangumiRepository>()
            .AddSingleton<BangumiManager, BangumiManager>()
            .AddSingleton<RefresherService, RefresherService>();

        return services;
    }

    public static IServiceCollection RegisterCoreService(this IServiceCollection services)
    {
        services
            .AddHostedService<RefresherService>()
            .AddHostedService<DownloaderInfoCheck>();

        return services;
    }

    public static WebApplication RegisterCoreAppEvents(this WebApplication app)
    {
        app.Lifetime.ApplicationStopping.Register(() =>
        {
            // save config when shutting down
            using var serviceScope = app.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            serviceProvider.GetRequiredService<ConfigManager>().SaveConfig();
        });
        return app;
    }
}