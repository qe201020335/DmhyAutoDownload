using DmhyAutoDownload.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DmhyAutoDownload.Core.Extensions;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCoreDependencyGroup(this IServiceCollection services)
    {
        var configManager =
            new ConfigManager(new Logger<ConfigManager>(LoggerFactory.Create(config => config.AddConsole())));
        services.AddSingleton(configManager).AddSingleton(configManager.Config)
            .AddSingleton<DownloadManager, DownloadManager>()
            .AddSingleton<BangumiManager, BangumiManager>()
            .AddSingleton<RefresherService, RefresherService>();

        return services;
    }

    public static IServiceCollection RegisterCoreService(this IServiceCollection services)
    {
        services.AddHostedService<RefresherService>();
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