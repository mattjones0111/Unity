using Api.Data;
using Api.Features.AudioFiles;
using Api.Middleware;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Api;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseLogging();

            builder.Services.AddDbContext<CatalogDbContext>(options =>
            {
                string? connectionString = builder.Configuration.GetConnectionString("catalog");
                options.UseNpgsql(
                    connectionString, c =>
                    {
                        c.EnableRetryOnFailure();
                    })
                    .EnableSensitiveDataLogging();
            });

            builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();
            builder.Services.AddHealthChecks();
            builder.Services.AddEndpoints();

            builder.Services.AddCritterStack(
                builder.Configuration.GetConnectionString("catalog"));

            builder.Services.AddSingleton<IAudioStore>(_ =>
                new AzureAudioStore(
                    builder.Configuration.GetConnectionString("azure")!));

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
            app.MapHealthChecks("/hc");

            app.UseFastEndpoints()
                .UseSwaggerGen();

            await app.ExecuteStartupTasksAsync();

            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Failed to start application.");
            return -1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
