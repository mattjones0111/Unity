using FastEndpoints;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.AudioFiles;

public class GetStream(IAudioStore store) : EndpointWithoutRequest
{
    public override void Configure()
    {
        AllowAnonymous();
        Get("/audio-files/{id}/stream");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        HttpContext.MarkResponseStart();
        HttpContext.Response.StatusCode = 200;
        HttpContext.Response.ContentType = "audio/wav";
        await store.GetStream(id, HttpContext.Response.Body, ct);
    }
}
