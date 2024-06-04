﻿using DmhyAutoDownload.Core.Configuration;
using DmhyAutoDownload.Core.Data;
using DmhyAutoDownload.Core.Extensions;
using DmhyAutoDownload.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DmhyAutoDownload;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Register configuration sources
        builder.RegisterAppConfiguration(args);

        // Add services to the container.
        var services = builder.Services;
        services
            .BindConfiguration<AutoDownloadConfig>(AutoDownloadConfig.Section)
            .ConfigureCoreData()
            .AddCoreDependencyGroup()
            .RegisterCoreService()
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

        app.RegisterCoreAppEvents();
        
        // Configure the HTTP request pipeline.
        // app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        // Do data migration
        using (var serviceScope = app.Services.CreateScope())
        {
            await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<CoreDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        
        await app.RunAsync();
    }
}