using Microsoft.Extensions.Hosting;

namespace DmhyAutoDownload.Core.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder UseAutoDownloadCore(this IHostApplicationBuilder builder)
    {
        builder.Services.UseAutoDownloadCoreService();
        return builder;
    }
    
    // for non application, generic host builder
    public static IHostBuilder UseAutoDownloadCore(this IHostBuilder hostBuilder) =>
        hostBuilder.ConfigureServices((hostBuilderContext, services) =>
        {
            services.UseAutoDownloadCoreService();
        });
}