using Contracts;
using FastEndpoints;
using FluentValidation;
using System;

namespace Api.Features.ProgrammeTemplates;

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
