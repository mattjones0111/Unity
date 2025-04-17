using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authorization;

namespace Api.Host.Features.AudioItem;

[HttpGet("/audio-items/{id}")]
[AllowAnonymous]
public class Get : EndpointWithoutRequest<Contracts.V1.AudioItem>
{
    private readonly IDocumentSession session;

    public Get(IDocumentSession session)
    {
        this.session = session;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var item = await session.LoadAsync<Contracts.V1.AudioItem>(id, ct);

        if (item == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(item, ct);
    }
}
