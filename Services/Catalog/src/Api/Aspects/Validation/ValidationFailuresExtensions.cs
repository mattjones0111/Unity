using FastEndpoints;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Api.Aspects.Validation;

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
