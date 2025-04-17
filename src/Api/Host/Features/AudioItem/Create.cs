using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Host.Features.AudioItem;

[HttpPut("/audio-items")]
[AllowAnonymous]
public class Create : Endpoint<Contracts.V1.AudioItem, Created>
{
    readonly IDocumentSession session;

    public Create(IDocumentSession session)
    {
        this.session = session;
    }

    public override async Task<Created> ExecuteAsync(
        Contracts.V1.AudioItem req,
        CancellationToken ct)
    {
        req = req with { Id = Guid.NewGuid() };
        session.Store(req);
        await session.SaveChangesAsync(ct);

        var url = $"/audio-items/{req.Id}";
        
        return TypedResults.Created(url);
    }
}
