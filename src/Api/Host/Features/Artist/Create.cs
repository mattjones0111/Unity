using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Host.Features.Artist;

[HttpPut("/artists")]
[AllowAnonymous]
public class Create : Endpoint<Contracts.V1.Artist, Created>
{
    readonly IDocumentSession session;

    public Create(IDocumentSession session)
    {
        this.session = session;
    }

    public override async Task<Created> ExecuteAsync(
        Contracts.V1.Artist req,
        CancellationToken ct)
    {
        req = req with { Id = Guid.NewGuid() };
        session.Store(req);
        await session.SaveChangesAsync(ct);

        var url = $"/artists/{req.Id}";
        return TypedResults.Created(url);
    }
}

public class ArtistRegistry : MartenRegistry
{
    public ArtistRegistry()
    {
        For<Contracts.V1.Artist>()
            .DocumentAlias("artist")
            .Identity(x => x.Id);
    }
}

public class ArtistValidator : Validator<Contracts.V1.Artist>
{
    public ArtistValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
