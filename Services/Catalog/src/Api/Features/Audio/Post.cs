using Api.Features.AudioFiles;
using Contracts;
using FastEndpoints;
using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.Audio;

public class Post(IAudioStore audioStore, IDocumentSession session)
    : Endpoint<AudioItem, Results<Created, Conflict, NotFound, InternalServerError>>
{
    public override void Configure()
    {
        AllowAnonymous();
        Post("/audio");
    }

    public override async Task<Results<Created, Conflict, NotFound, InternalServerError>> ExecuteAsync(
        AudioItem req,
        CancellationToken ct)
    {
        var audioLength = await audioStore.GetLengthAsync(req.AudioFileId, ct);
        if (!audioLength.IsSuccess)
        {
            return audioLength.Error switch
            {
                AudioStoreErrors.AudioItemDoesNotExist => TypedResults.NotFound(),
                _ => TypedResults.InternalServerError()
            };
        }

        var existing = await session.LoadAsync<AudioItem>(req.Id, ct);
        if (existing != null)
        {
            return TypedResults.Conflict();
        }

        foreach (var artist in req.Artists)
        {
            var foundArtist = await session.LoadAsync<Artist>(artist.Artist.Id, ct);
            if (foundArtist == null)
            {
                return TypedResults.NotFound();
            }
        }

        req = req with { Duration = audioLength.Value };
        session.Store(req);
        await session.SaveChangesAsync(ct);

        await new AudioItemCreatedEvent(req).PublishAsync(Mode.WaitForNone, ct);

        return TypedResults.Created();
    }
}

public record AudioItemCreatedEvent(AudioItem Item) : IEvent;

public class AudioItemValidator : Validator<AudioItem>
{
    public AudioItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty();
    }
}
