using Marten;
using Contracts;

namespace Api.Features.Audio;

public class AudioItemRegistry : MartenRegistry
{
    public AudioItemRegistry()
    {
        For<AudioItem>()
            .DocumentAlias("audio_item")
            .Identity(x => x.Id);
    }
}
