using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DmhyAutoDownload;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var services = builder.Services;
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // DI
        var configManager = new ConfigManager();
        services.AddSingleton(configManager).AddSingleton(configManager.InitConfig())
            .AddSingleton<DownloadManager, DownloadManager>()
            .AddSingleton<BangumiManager, BangumiManager>();
        
        // extra services
        services.AddHostedService<RefresherService>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        string url;
        using (var serviceScope = app.Services.CreateScope())
        {
            var serviceProvider = serviceScope.ServiceProvider;

            var config = serviceProvider.GetRequiredService<ConfigManager>();
            url = config.Config.ListenOn;
        }

        app.Run(url);
    }
}