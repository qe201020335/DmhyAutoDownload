using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DmhyAutoDownload.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder RegisterAppConfiguration(this IHostApplicationBuilder builder, string[] args)
    {
        builder.Configuration
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", true, false)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, false)
            .AddEnvironmentVariables()
            .AddCommandLine(args);
        return builder;
    }
}