using Marten;

namespace Api.Host.Features.AudioItem;

public class AudioItemRegistry : MartenRegistry
{
    public AudioItemRegistry()
    {
        For<Contracts.V1.AudioItem>()
            .DocumentAlias("audioitem")
            .Identity(x => x.Id);
    }
}
