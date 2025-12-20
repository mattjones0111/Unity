using Microsoft.Extensions.Hosting;
using Serilog;

namespace Api;

public static class LoggingExtensions
{
    public static IHostBuilder UseLogging(this IHostBuilder builder)
    {
        builder.UseSerilog((builderContext, _, logConfig) =>
        {
            logConfig
                .ReadFrom.Configuration(builderContext.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Debug();
        });

        return builder;
    }
}
