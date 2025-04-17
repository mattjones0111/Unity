using Marten;

namespace Api.Host.Features.Artist;

public class ArtistRegistry : MartenRegistry
{
    public ArtistRegistry()
    {
        For<Contracts.V1.Artist>()
            .DocumentAlias("artist")
            .Identity(x => x.Id);
    }
}