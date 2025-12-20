using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.AudioFiles;

public class Post(IAudioStore audioStore)
    : EndpointWithoutRequest<Results<Created, InternalServerError>>
{
    public override void Configure()
    {
        AllowAnonymous();
        Post("/audio-files");
    }

    public override async Task<Results<Created, InternalServerError>> ExecuteAsync(
        CancellationToken ct)
    {
        var result = await audioStore.CreateAudioAsync(ct);

        if (result.IsSuccess)
        {
            return TypedResults.Created($"/audio-files/{result.Value!.Id}");
        }

        return TypedResults.InternalServerError();
    }
}
