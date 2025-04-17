using System.Collections.Generic;
using System.Linq;
using System.Net;
using FastEndpoints;
using FluentValidation.Results;

namespace Api.Host.Aspects.Validation;

public static class ValidationFailuresExtensions
{
    public static ProblemDetails ToProblemDetails(
        this List<ValidationFailure> failures,
        HttpStatusCode defaultStatus = HttpStatusCode.BadRequest)
    {
        var mostCommon = failures
            .Where(x => x.ErrorCode.Is4xxHttpErrorCode())
            .GroupBy(x => x.ErrorCode)
            .Select(grp => new { key = int.Parse(grp.Key), count = grp.Count() })
            .MaxBy(grp => grp.count)?
            .key;

        return new ProblemDetails(
            failures,
            mostCommon ?? (int)defaultStatus);
    }
}

public static class StringExtensions
{
    public static bool Is4xxHttpErrorCode(this string? errorCode)
    {
        if (errorCode == null)
        {
            return false;
        }

        if (!int.TryParse(errorCode, out int errorCodeInt))
        {
            return false;
        }
        
        return errorCodeInt is >= 400 and <= 499;
    }
}
