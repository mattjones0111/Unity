using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Host.Ports;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authorization;

namespace Api.Host.Features.AudioItem;

[HttpPost("/audio-items/{id}/audio")]
[AllowAnonymous]
public class AppendAudio : EndpointWithoutRequest
{
    readonly IDocumentSession session;
    readonly IStoreAudio audioStore;

    public AppendAudio(
        IDocumentSession session,
        IStoreAudio audioStore)
    {
        this.session = session;
        this.audioStore = audioStore;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        
        var audioItem = await session.LoadAsync<Contracts.V1.AudioItem>(id, ct);

        if (Files.Count == 0)
        {
            AddError("No audio content provided.", errorCode: "400");
            return;
        }

        if (audioItem == null)
        {
            AddError("Audio item not found.", errorCode: "404");
            return;
        }

        var file = Files[0];
        await audioStore.UploadAudioPartAsync(
            id,
            file.OpenReadStream(),
            ct);

        await SendNoContentAsync(ct);
    }
}
