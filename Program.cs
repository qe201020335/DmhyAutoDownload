using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DmhyAutoDownload;
internal class Program
{
    private static async Task Test()
    {
        Console.WriteLine("Hello, World!");

        await BangumiManager.Instance.Test();
        await DownloadManager.Instance.Test();
        
        
    }
    public static void Main(string[] args)
    {
        ConfigManager.Instance.InitConfig();

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHostedService<RefresherService>();

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
        
        app.Run();
        
        ConfigManager.Instance.SaveConfig();

    }
}