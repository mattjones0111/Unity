using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Host.Aspects.Validation;
using Api.Host.Ports;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Host.Features.Upload;

[HttpPost("/uploads/{id}/blocks/{blockId}")]
[AllowAnonymous]
public class Upload : EndpointWithoutRequest<Results<ProblemDetails, NoContent>>
{
    private readonly IStoreAudio audioStore;

    public Upload(IStoreAudio audioStore)
    {
        this.audioStore = audioStore;
    }

    public override async Task<Results<ProblemDetails, NoContent>> ExecuteAsync(
        CancellationToken ct)
    {
        if (Files.Count == 0)
        {
            AddError("Request contains no files.");
            return ValidationFailures.ToProblemDetails();
        }

        var id = Route<Guid>("id");
        var block = Route<int>("blockId");
        
        var uploadResult = await audioStore.UploadBlockAsync(
            id,
            Files[0].OpenReadStream(),
            block,
            ct);

        HttpContext.Response
            .Headers
            .Append("x-audio-block-id", uploadResult.BlockId);

        return TypedResults.NoContent();
    }
}
