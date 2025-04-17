using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Api.Host.Aspects.Validation;
using Api.Host.Data;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Api.Host.Features.AudioItem;

[HttpPut("/audio-items")]
[AllowAnonymous]
public class Create : Endpoint<Contracts.V1.AudioItem, Results<ProblemDetails, Created>>
{
    readonly IDocumentSession session;
    readonly AudioCatalogDbContext db;

    public Create(
        IDocumentSession session,
        AudioCatalogDbContext db)
    {
        this.session = session;
        this.db = db;
    }

    public override async Task<Results<ProblemDetails, Created>> ExecuteAsync(
        Contracts.V1.AudioItem req,
        CancellationToken ct)
    {
        if (req.CategoryPaths.Length > 0)
        {
            var categoryPaths = await db.Categories
                .Select(x => x.Path)
                .ToHashSetAsync(ct);
        
            if (req.CategoryPaths.Any(x => !categoryPaths.Contains(x)))
            {
                AddError(
                    p => p.CategoryPaths,
                    "Category path not found.",
                    ((int)HttpStatusCode.NotFound).ToString());
                
                return ValidationFailures.ToProblemDetails();
            }
        }

        if (req.Artists.Length > 0)
        {
            // validate artists
        }
        
        req = req with { Id = Guid.NewGuid() };
        session.Store(req);
        await session.SaveChangesAsync(ct);

        await new AudioItemCreated(req.Id, req.Title, req.CategoryPaths)
            .PublishAsync(Mode.WaitForNone, ct);
        
        var url = $"/audio-items/{req.Id}";

        return TypedResults.Created(url);
    }
}
