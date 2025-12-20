using Api.Features.ProgrammeTemplates;
using Contracts;
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
            throw new ConfigurationErrorsException("Connection string 'scheduling' not configured.");
        }

        services.AddMarten(opts =>
        {
            opts.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
            opts.UseSystemTextJsonForSerialization(
                enumStorage: EnumStorage.AsString,
                configure: opt =>
                {
                    opt.AllowOutOfOrderMetadataProperties = true;
                    opt.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    opt.TypeInfoResolver = new PolymorphicTypeResolver();
                });

            opts.Connection(connectionString);
            opts.DatabaseSchemaName = "scheduling";

            opts.Schema.Include<ProgrammeTemplateRegistry>();
        })
        .UseLightweightSessions()
        .ApplyAllDatabaseChangesOnStartup();

        services.CritterStackDefaults(x =>
        {
            x.ServiceName = "scheduling";

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            x.Production.ResourceAutoCreate = AutoCreate.None;
        });

        return services;
    }
}
