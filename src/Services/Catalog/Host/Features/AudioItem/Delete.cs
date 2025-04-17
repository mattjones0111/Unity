using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Host.Data;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Host.Features.AudioItem;

[HttpDelete("audio-items/{id}")]
[AllowAnonymous]
public class Delete : EndpointWithoutRequest<NoContent>
{
    readonly IDocumentSession session;

    public Delete(IDocumentSession session)
    {
        this.session = session;
    }

    public override async Task<NoContent> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        session.Delete<Contracts.V1.AudioItem>(id);
        await session.SaveChangesAsync(ct);

        await new AudioItemDeleted(id).PublishAsync(Mode.WaitForNone, ct);

        return TypedResults.NoContent();
    }
}
