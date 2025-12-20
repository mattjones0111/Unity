using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.AudioFiles;

public class Delete(IAudioStore store) : EndpointWithoutRequest<Ok>
{
    public override void Configure()
    {
        AllowAnonymous();
        Delete("/audio-files/{id}");
    }

    public override async Task<Ok> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        await store.DeleteAsync(id, ct);
        return TypedResults.Ok();
    }
}
