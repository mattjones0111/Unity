using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Host.Contracts.V1;
using Api.Host.Data;
using Api.Host.Ports;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Host.Features.AudioItem;

[HttpPut("/audio-items")]
[AllowAnonymous]
public class Create : Endpoint<CreateAudioItem, Results<ProblemDetails, NotFound, Created>>
{
    readonly IStoreAudio audioStore;
    readonly IDocumentSession session;

    public Create(
        IStoreAudio audioStore,
        IDocumentSession session)
    {
        this.audioStore = audioStore;
        this.session = session;
    }

    public override async Task<Results<ProblemDetails, NotFound, Created>> ExecuteAsync(
        CreateAudioItem req,
        CancellationToken ct)
    {
        await audioStore.MoveAsync(req.UploadId, ct);

        var duration = await audioStore.GetDurationAsync(req.UploadId, ct);
        var document = req.AudioItem with { Id = req.UploadId, Duration = duration };
        session.Store(document);
        await session.SaveChangesAsync(ct);

        await new AudioItemCreated(
            document.Id,
            document.Title,
            document.CategoryPaths)
            .PublishAsync(Mode.WaitForNone, ct);
        
        var url = $"/audio-items/{req.UploadId}";
        return TypedResults.Created(url);
    }
}
