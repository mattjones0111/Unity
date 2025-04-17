using Api.Host.Adapters;
using Api.Host.Ports;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api.Host;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddFastEndpoints();

        builder.AddCritterStack(builder.Configuration);

        builder.Services.AddTransient<IStoreAudio>(_ =>
            new AzureAudioStore(
                builder.Configuration.GetConnectionString("audio-store")!));
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseFastEndpoints();
        
        app.Run();
    }
}