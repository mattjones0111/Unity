using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Contracts;

public interface ICatalogClient
{
    [Post("/audio-files")]
    Task<HttpResponseMessage> CreateAudioFileAsync(
        CancellationToken ct);

    [Multipart]
    [Patch("/audio-files/{id}")]
    Task<HttpResponseMessage> AppendToAudioFileAsync(
        Guid id,
        StreamPart stream,
        CancellationToken ct);

    [Post("/audio-files/{id}/finalize")]
    Task<HttpResponseMessage> FinalizeAudioFileAsync(
        Guid id,
        CancellationToken ct);

    [Delete("/audio-files/{id}")]
    Task<HttpResponseMessage> DeleteAudioFileAsync(
        Guid id,
        CancellationToken ct);

    [Post("/audio")]
    Task<HttpResponseMessage> CreateAudioItemAsync(
        AudioItem item,
        CancellationToken ct);

    [Get("/audio/{id}")]
    Task<ApiResponse<AudioItem>> GetAudioItemAsync(
        Guid id,
        CancellationToken ct);

    [Get("/audio")]
    Task<IEnumerable<AudioSlim>> QueryAsync(
        [Query] int pn = 1,
        [Query] int ps = 50,
        CancellationToken ct = default);

    [Post("/artists")]
    public Task<HttpResponseMessage> CreateArtistAsync(
        Artist artist,
        CancellationToken ct = default);
}

public static class CatalogClientExtensions
{
    public static async Task<HttpResponseMessage> CreateAndUploadAudioAsync(
        this ICatalogClient client,
        AudioItem item,
        Stream stream,
        CancellationToken ct)
    {
        var createResponse = await client.CreateAudioFileAsync(ct);

        if (!createResponse.IsSuccessStatusCode)
        {
            return createResponse;
        }

        var parsed = Guid.TryParse(createResponse.Headers.Location?.ToString().Split('/').Last(), out Guid id);

        if (!parsed)
        {
            return createResponse;
        }

        var uploadResponse = await client.AppendToAudioFileAsync(
            id,
            new StreamPart(stream, "file1", "application/octet-stream", "files"), ct);

        if (!uploadResponse.IsSuccessStatusCode)
        {
            return uploadResponse;
        }

        var finalizeResponse = await client.FinalizeAudioFileAsync(id, ct);

        if (!finalizeResponse.IsSuccessStatusCode)
        {
            return finalizeResponse;
        }

        item = item with { AudioFileId = id };
        return await client.CreateAudioItemAsync(item, ct);
    }
}
