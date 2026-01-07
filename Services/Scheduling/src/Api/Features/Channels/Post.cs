using Contracts;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.Channels;

public class Post(IDocumentSession session)
    : Endpoint<Channel, Results<Created, Conflict>>
{
    public override void Configure()
    {
        AllowAnonymous();
        Post("/channels");
    }

    public override async Task<Results<Created, Conflict>> ExecuteAsync(
        Channel req,
        CancellationToken ct)
    {
        var existing = await session.LoadAsync<Channel>(req.Id, ct);
        if (existing != null)
        {
            return TypedResults.Conflict();
        }

        session.Store(req);
        await session.SaveChangesAsync(ct);
        return TypedResults.Created($"/channels/{req.Id}");
    }
}
