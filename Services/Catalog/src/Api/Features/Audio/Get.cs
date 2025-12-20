using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Threading;
using System.Threading.Tasks;
using AudioItem = Contracts.AudioItem;

namespace Api.Features.Audio;

public class Get(IDocumentSession session)
    : EndpointWithoutRequest<Results<Ok<AudioItem>, NotFound>>
{
    public override void Configure()
    {
        AllowAnonymous();
        Get("/audio/{id}");
    }

    public override async Task<Results<Ok<AudioItem>, NotFound>> ExecuteAsync(
        CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var item = await session.LoadAsync<AudioItem>(id, token: ct);
        if (item == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(item);
    }
}
