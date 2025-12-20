using Contracts;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.ProgrammeTemplates;

public class Get(IDocumentSession session)
    : EndpointWithoutRequest<Results<Ok<ProgrammeTemplate>, NotFound>>
{
    public override void Configure()
    {
        AllowAnonymous();
        Get("/programme-templates/{id}");
    }

    public override async Task<Results<Ok<ProgrammeTemplate>, NotFound>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await session.LoadAsync<ProgrammeTemplate>(id, ct);
        if (result == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }
}