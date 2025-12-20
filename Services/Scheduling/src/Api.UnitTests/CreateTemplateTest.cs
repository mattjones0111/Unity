using Api.UnitTests.ComponentTests;
using AwesomeAssertions;
using Contracts;
using Refit;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Api.UnitTests;

public class CreateTemplateTest(AppFactory appFactory)
    : IntegrationContext(appFactory)
{
    [Fact]
    public async Task Test()
    {
        var jsonSettings = new JsonSerializerOptions
        {
            TypeInfoResolver = new PolymorphicTypeResolver()
        };

        var client = RestService.For<ISchedulingClient>(
            factory.CreateClient(),
            new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(jsonSettings)
            });

        var template = new ProgrammeTemplate(
            Id: Guid.NewGuid(),
            Title: "Breakfast",
            Duration: TimeSpan.FromHours(1),
            Items:
            [
                new CommentTemplateItem(Guid.NewGuid(), "This is a comment.")
            ]);

        var response = await client.CreateProgrammeTemplateAsync(template);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var location = response.Headers.Location?.ToString();
        location.Should().NotBeNullOrWhiteSpace();
        var id = Guid.Parse(location.Split('/').Last());
        var readBack = await client.GetProgrammeTemplateAsync(id);

        readBack.IsSuccessful.Should().BeTrue();
        readBack.Content.Should().NotBeNull();
    }
}
