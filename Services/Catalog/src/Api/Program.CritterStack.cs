using Api.Features.Artists;
using Api.Features.Audio;
using JasperFx;
using JasperFx.CodeGeneration;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Text.Json.Serialization;
using Weasel.Core;

namespace Api;

public static class CritterStackBootstrapping
{
    public static IServiceCollection AddCritterStack(
        this IServiceCollection services,
        string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ConfigurationErrorsException("Connection string 'catalog' not configured.");
        }

        services.AddMarten(opts =>
        {
            opts.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
            opts.UseSystemTextJsonForSerialization(
                enumStorage: EnumStorage.AsString,
                configure: opt =>
                {
                    opt.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            opts.Connection(connectionString);
            opts.DatabaseSchemaName = "catalog";

            opts.Schema.Include<AudioItemRegistry>();
            opts.Schema.Include<ArtistRegistry>();
        })
        .UseLightweightSessions()
        .ApplyAllDatabaseChangesOnStartup();

        services.CritterStackDefaults(x =>
        {
            x.ServiceName = "catalog";

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            x.Production.ResourceAutoCreate = AutoCreate.None;
        });

        return services;
    }
}
