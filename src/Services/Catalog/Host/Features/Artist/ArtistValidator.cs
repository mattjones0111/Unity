using FastEndpoints;
using FluentValidation;

namespace Api.Host.Features.Artist;

public class ArtistValidator : Validator<Contracts.V1.Artist>
{
    public ArtistValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
