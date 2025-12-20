using Contracts;
using FastEndpoints;
using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.ProgrammeTemplates;

public class Post(IDocumentSession session)
    : Endpoint<ProgrammeTemplate, Results<Created, Conflict>>
{
    public override void Configure()
    {
        AllowAnonymous();
        Post("/programme-templates");
    }

    public override async Task<Results<Created, Conflict>> ExecuteAsync(
        ProgrammeTemplate req,
        CancellationToken ct)
    {
        session.Store(req);
        await session.SaveChangesAsync(ct);
        return TypedResults.Created($"/programme-templates/{req.Id}");
    }
}

public class ProgrammeTemplateValidator : Validator<ProgrammeTemplate>
{
    public ProgrammeTemplateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero);
    }
}
