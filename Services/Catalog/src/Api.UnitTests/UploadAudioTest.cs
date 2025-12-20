using Api.UnitTests.ComponentTests;
using AwesomeAssertions;
using Contracts;
using Refit;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Api.UnitTests;

public class UploadAudioTest(AppFactory appFactory)
    : IntegrationContext(appFactory)
{
    [Fact]
    public async Task CanUploadAudioFile()
    {
        var audioItemId = Guid.NewGuid();

        var client = RestService.For<ICatalogClient>(factory.CreateClient());

        var artist = new Artist(Guid.NewGuid(), "Barney Sausage");
        var createArtistResponse = await client.CreateArtistAsync(artist);
        createArtistResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await client.CreateAndUploadAudioAsync(
            new AudioItem(
                audioItemId,
                Guid.NewGuid(),
                "This is a test",
                [],
                [new ArtistContext(artist, ArtistContextTypes.Performer)],
                TimeSpan.FromMinutes(1),
                []),
            File.OpenRead("Resources/10s-1ktone-stereo.wav"),
            CancellationToken.None);

        response.IsSuccessStatusCode.Should().BeTrue();

        var readBack = await client.GetAudioItemAsync(audioItemId, CancellationToken.None);
        readBack.Content.Should().NotBeNull();
        readBack.Content.Duration.Should().Be(TimeSpan.FromSeconds(10));

        var queryCatalog = await client.QueryAsync();
        queryCatalog.Should().Contain(x => x.Title == "This is a test");
    }
}
