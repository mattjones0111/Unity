using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Host.Aspects.Validation;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Host;

public static class FastEndpointExtensions
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services)
    {
        services.AddFastEndpoints(options =>
        {
            options.Assemblies = new[] { typeof(Program).Assembly };
            options.DisableAutoDiscovery = true;
        });

        services.Configure<JsonOptions>(o =>
        {
            o.SerializerOptions.AllowTrailingCommas = true;
            o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            o.SerializerOptions.PropertyNameCaseInsensitive = true;
            o.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        return services;
    }

    public static WebApplication UseFastEndpoints(this WebApplication app)
    {
        app.UseFastEndpoints(x =>
        {
            x.Serializer.Options.AllowTrailingCommas = true;
            x.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
            x.Serializer.Options.PropertyNameCaseInsensitive = true;
            x.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            x.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            x.Errors.ResponseBuilder =
                new Func<List<ValidationFailure>, HttpContext, int, ProblemDetails>((failures, _, _) =>
                    failures.ToProblemDetails());
        });

        return app;
    }
}
