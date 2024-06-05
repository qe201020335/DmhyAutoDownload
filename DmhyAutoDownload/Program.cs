using DmhyAutoDownload.Core.Extensions;
using DmhyAutoDownload.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DmhyAutoDownload;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
       
        builder
            .RegisterAppConfiguration(args)  // Register configuration sources
            .UseAutoDownloadCore(); 

        // Add services to the container.
        var services = builder.Services;
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddControllers()
            .AddNewtonsoftJson();

        // Build the app
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine("Development mode");
            app.UseSwagger();
            app.UseSwaggerUI();
            var option = new RewriteOptions();
            option.AddRedirect("^index\\.html$", "swagger");
            app.UseRewriter(option);
        }

        // Configure the HTTP request pipeline.
        // app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        await app.DoCorePreRunChoresAsync();
        
        await app.RunAsync();
    }
}