using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Host.Contracts.V1;
using Api.Host.Features.AudioItem;
using Api.Host.Ports;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Host.Features.Upload;

[HttpPost("/uploads/{id}/complete")]
[AllowAnonymous]
public class Complete : Endpoint<AudioUpload, NoContent>
{
    private readonly IStoreAudio audioStore;

    public Complete(IStoreAudio audioStore)
    {
        this.audioStore = audioStore;
    }

    public override async Task<NoContent> ExecuteAsync(
        AudioUpload req,
        CancellationToken ct)
    {
        var id = Route<Guid>("id");
        await audioStore.CompleteUploadAsync(id, req.BlockIds, ct);

        await new AudioUploaded(id).PublishAsync(Mode.WaitForNone, ct);

        return TypedResults.NoContent();
    }
}
