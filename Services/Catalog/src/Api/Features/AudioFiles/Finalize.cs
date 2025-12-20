using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.AudioFiles;

public class Finalize(IAudioStore store)
    : EndpointWithoutRequest<Results<Ok, NotFound, InternalServerError>>
{
    public override void Configure()
    {
        AllowAnonymous();
        Post("/audio-files/{id}/finalize");
    }

    public override async Task<Results<Ok, NotFound, InternalServerError>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await store.FinalizeAudioAsync(id, ct);

        if (result.IsSuccess)
        {
            return TypedResults.Ok();
        }

        return result.Error switch
        {
            AudioStoreErrors.AudioItemDoesNotExist => TypedResults.NotFound(),
            _ => TypedResults.InternalServerError()
        };
    }
}
