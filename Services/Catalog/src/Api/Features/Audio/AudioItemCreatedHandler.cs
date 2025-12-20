using Api.Data;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.Audio;

public class AudioItemCreatedHandler(IServiceScopeFactory scopeFactory)
    : IEventHandler<AudioItemCreatedEvent>
{
    public async Task HandleAsync(AudioItemCreatedEvent eventModel, CancellationToken ct)
    {
        var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        db.AudioItems.Add(new AudioItem
        {
            Id = eventModel.Item.Id,
            Title = eventModel.Item.Title,
            Artist = GetArtistName(eventModel.Item.Artists),
            Duration = eventModel.Item.Duration,
            Categories = eventModel.Item.Categories.Select(x => x.Path).ToArray()
        });

        await db.SaveChangesAsync(ct);
    }

    static string? GetArtistName(Contracts.ArtistContext[] artists)
    {
        if (artists.Length == 0)
        {
            return null;
        }

        return artists
            .FirstOrDefault(x => x.Type == Contracts.ArtistContextTypes.Performer)?
            .Artist
            .Name;
    }
}
