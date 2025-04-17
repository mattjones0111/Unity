using System.Linq;
using Api.Host.Contracts.V1;
using Api.Host.Data;
using FastEndpoints;
using FluentValidation;

namespace Api.Host.Features.AudioItem;

public class AudioItemValidator : Validator<Contracts.V1.AudioItem>
{
    public AudioItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty();

        When(
            x => x.Artists.Any(),
            () =>
            {
                RuleForEach(x => x.Artists)
                    .SetValidator(new AudioItemArtistValidator());
            });
    }
}

public class AudioItemArtistValidator : Validator<AudioItemArtist>
{
    public AudioItemArtistValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
