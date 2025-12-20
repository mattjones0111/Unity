using Contracts;
using Marten;

namespace Api.Features.Artists;

public class ArtistRegistry : MartenRegistry
{
    public ArtistRegistry()
    {
        For<Artist>()
            .DocumentAlias("artist")
            .Identity(x => x.Id);
    }
}
