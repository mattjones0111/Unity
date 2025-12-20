using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.AudioFiles;

public class Patch(IAudioStore store)
    : Endpoint<UploadAudioRequest, Results<Ok, NotFound>>
{
    public override void Configure()
    {
        AllowAnonymous();
        AllowFileUploads();
        Patch("/audio-files/{id}");
    }

    public override async Task<Results<Ok, NotFound>> ExecuteAsync(
        UploadAudioRequest req,
        CancellationToken ct)
    {
        var id = Route<Guid>("id");

        foreach (var file in Files)
        {
            await store.AppendAudioAsync(id, file.OpenReadStream(), ct);
        }

        return TypedResults.Ok();
    }
}

public record UploadAudioRequest(IFormFileCollection Files);