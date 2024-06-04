using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DmhyAutoDownload.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection BindConfiguration<T>(this IServiceCollection services, string section) where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(section))
        {
            throw new ArgumentException("Section must be provided", nameof(section));
        }

        return services.AddSingleton<T>(serviceProvider =>
            serviceProvider
                .GetRequiredService<IConfiguration>()
                .GetSection(section)
                .Get<T>() ?? new T()
        );
    }
}