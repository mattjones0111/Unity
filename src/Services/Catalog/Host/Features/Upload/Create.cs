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

[HttpPut("/uploads")]
[AllowAnonymous]
public class Create : EndpointWithoutRequest<Results<ProblemDetails, Created>>
{
    private readonly IStoreAudio audioStore;

    public Create(IStoreAudio audioStore)
    {
        this.audioStore = audioStore;
    }

    public override async Task<Results<ProblemDetails, Created>> ExecuteAsync(
        CancellationToken ct)
    {
        if (Files.Count == 0)
        {
            AddError("Request contains no files.");
            return ValidationFailures.ToProblemDetails();
        }
        
        Guid id = Guid.NewGuid();
        var uploadResult = await audioStore.UploadBlockAsync(
            id,
            Files[0].OpenReadStream(),
            0,
            ct);

        HttpContext.Response
            .Headers
            .Append("x-audio-block-id", uploadResult.BlockId);

        return TypedResults.Created($"/uploads/{id}");
    }
}
