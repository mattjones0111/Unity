using System.Threading.Tasks;
using Api.Host.Adapters;
using Api.Host.Data;
using Api.Host.Ports;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api.Host;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddEndpoints();

        builder.AddCritterStack(builder.Configuration);

        builder.Services.AddTransient<IStoreAudio>(_ =>
            new AzureAudioStore(
                builder.Configuration.GetConnectionString("audio-store")!));
        
        builder.Services.AddDbContext<AudioCatalogDbContext>(options =>
        {
            string connectionString = builder.Configuration.GetConnectionString("unity-audio")!;
            options.UseNpgsql(
                connectionString,
                c =>
                {
                    c.EnableRetryOnFailure();
                })
                .EnableSensitiveDataLogging();
        });
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseFastEndpoints();

        await app.ExecuteStartupTasksAsync();

        await app.RunAsync();
    }
}