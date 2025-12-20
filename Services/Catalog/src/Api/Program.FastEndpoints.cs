using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProblemDetails = FastEndpoints.ProblemDetails;

namespace Api;

public static class FastEndpointExtensions
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services)
    {
        services.AddFastEndpoints(options =>
        {
            options.Assemblies = [typeof(Program).Assembly];
            options.DisableAutoDiscovery = true;
        })
        .SwaggerDocument();

        services.Configure<JsonOptions>(o =>
        {
            ApplyOptions(o.JsonSerializerOptions);
        });

        return services;
    }

    public static WebApplication UseFastEndpoints(this WebApplication app)
    {
        app.UseFastEndpoints(x =>
        {
            ApplyOptions(x.Serializer.Options);

            x.Errors.ResponseBuilder = (failures, _, _) => failures.ToProblemDetails();
        });

        return app;
    }

    static void ApplyOptions(JsonSerializerOptions options)
    {
        options.AllowTrailingCommas = true;
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new TimeOnlyConverter());
        options.PropertyNameCaseInsensitive = true;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }
}

public static class ValidationFailureExtensions
{
    public static ProblemDetails ToProblemDetails(
        this IEnumerable<ValidationFailure> failures,
        HttpStatusCode defaultStatus = HttpStatusCode.BadRequest)
    {
        var theFailures = failures.ToArray();

        var mostCommon = theFailures
            .Where(x => x.ErrorCode.Is4xxHttpErrorCode())
            .GroupBy(x => x.ErrorCode)
            .Select(grp => new { key = int.Parse(grp.Key), count = grp.Count() })
            .MaxBy(grp => grp.count)?
            .key;

        return new ProblemDetails(
            theFailures,
            mostCommon ?? (int)defaultStatus);
    }
}

public static class StringExtensions
{
    // ReSharper disable once InconsistentNaming
    public static bool Is4xxHttpErrorCode(this string? value)
    {
        return !string.IsNullOrEmpty(value)
               && int.TryParse(value, out var code)
               && code >= 100 && code <= 599;
    }
}

public class TimeOnlyConverter : JsonConverter<TimeOnly?>
{
    readonly string serializationFormat;

    public TimeOnlyConverter() : this(null)
    {
    }

    public TimeOnlyConverter(string? serializationFormat)
    {
        this.serializationFormat = serializationFormat ?? "HH:mm:ss";
    }

    public override TimeOnly? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return TimeOnly.ParseExact(value!, serializationFormat);
    }

    public override void Write(
        Utf8JsonWriter writer,
        TimeOnly? value,
        JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value.ToString(serializationFormat));
    }
}
