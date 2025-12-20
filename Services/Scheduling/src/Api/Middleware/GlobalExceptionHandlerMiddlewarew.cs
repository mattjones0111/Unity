using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Api.Middleware;

public class GlobalExceptionHandlerMiddleware(
    ILogger<GlobalExceptionHandlerMiddleware> logger)
    : IMiddleware
{
    const string ModelType = "https://tools.ietf.org/html/rfc7231#section-6.6.1";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        /*
        catch (ModelBindingException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            var problemDetails = new ProblemDetails
            {
                Type = ModelType,
                Title = "Binding Exception",
                Status = ex.StatusCode,
                Instance = context.Request.Path,
                Detail = $"There was an error binding the content. {ex.Message}"
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        */
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var problemDetails = new ProblemDetails
            {
                Type = ModelType,
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path,
                Detail = "An error occurred."
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
