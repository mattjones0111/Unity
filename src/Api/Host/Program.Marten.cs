﻿using System;
using Api.Host.Features.Artist;
using Api.Host.Features.AudioItem;
using JasperFx.CodeGeneration;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Weasel.Core;

namespace Api.Host;

public static class MartenExtensions
{
    public static void AddCritterStack(
        this WebApplicationBuilder builder,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("unity-audio") ??
                               throw new Exception("The connection string was not found.");

        builder.Services.AddMarten(options =>
        {
            options.Connection(connectionString);
            options.DatabaseSchemaName = "unity_audio";
            options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
            options.UseSystemTextJsonForSerialization(
                enumStorage: EnumStorage.AsString,
                casing: Casing.SnakeCase);

            options.Schema.Include<AudioItemRegistry>();
            options.Schema.Include<ArtistRegistry>();
        })
        .OptimizeArtifactWorkflow(TypeLoadMode.Static)
        .UseLightweightSessions()
        .ApplyAllDatabaseChangesOnStartup();
    }
}
