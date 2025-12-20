using Contracts;
using FastEndpoints;
using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.Artists;

public class Post(IDocumentSession session)
    : Endpoint<Artist, Results<Created, Conflict>>
{
    public override void Configure()
    {
        AllowAnonymous();
        Post("/artists");
    }

    public override async Task<Results<Created, Conflict>> ExecuteAsync(
        Artist req,
        CancellationToken ct)
    {
        var existing = await session.LoadAsync<Artist>(req.Id, ct);
        if (existing != null)
        {
            return TypedResults.Conflict();
        }

        session.Store(req);
        await session.SaveChangesAsync(ct);
        return TypedResults.Created($"/artists/{req.Id}");
    }
}

public class ArtistValidator : Validator<Artist>
{
    public ArtistValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
