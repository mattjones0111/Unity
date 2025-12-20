using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using NAudio.FileFormats.Wav;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.AudioFiles;

public static class AudioStoreErrors
{
    public const string AudioItemExists = "Audio item already exists.";
    public const string AudioItemDoesNotExist = "Audio item does not exist.";
}

public interface IAudioStore
{
    Task<Result<CreateAudioResult>> CreateAudioAsync(CancellationToken ct = default);

    Task<Result> AppendAudioAsync(Guid id, Stream stream, CancellationToken ct = default);

    Task<Result> FinalizeAudioAsync(Guid id, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<TimeSpan>> GetLengthAsync(Guid id, CancellationToken ct = default);

    Task<Result> GetStream(Guid id, Stream stream, CancellationToken ct = default);
}

public record CreateAudioResult(Guid Id);

public sealed class AzureAudioStore(string connectionString) : IAudioStore
{
    private readonly BlobContainerClient storeClient = new(connectionString, "audio");

    public async Task<Result<CreateAudioResult>> CreateAudioAsync(
        CancellationToken ct = default)
    {
        var id = Guid.NewGuid();
        var name = $"{id}.wav";

        try
        {
            await storeClient.CreateIfNotExistsAsync(cancellationToken: ct);
        }
        catch (Exception ex)
        {
            return Result<CreateAudioResult>.Failure(ex);
        }

        var blobClient = storeClient.GetAppendBlobClient(name);
        var exists = await blobClient.ExistsAsync(ct);

        while (exists)
        {
            id = Guid.NewGuid();
            exists = await blobClient.ExistsAsync(ct);
        }

        try
        {
            await blobClient.CreateAsync(
                new BlobHttpHeaders
                {
                    ContentType = "audio/wav"
                },
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            return Result<CreateAudioResult>.Failure(ex);
        }
        
        return Result<CreateAudioResult>.Success(new CreateAudioResult(id));
    }

    public async Task<Result> AppendAudioAsync(Guid id, Stream stream, CancellationToken ct = default)
    {
        var name = $"{id}.wav";
        var blobClient = storeClient.GetAppendBlobClient(name);
        var exists = await blobClient.ExistsAsync(ct);

        if (!exists)
        {
            return Result.Failure(AudioStoreErrors.AudioItemDoesNotExist);
        }

        try
        {
            await blobClient.AppendBlockAsync(stream, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            return Result<CreateAudioResult>.Failure(ex);
        }

        return Result.Success();
    }

    public async Task<Result> FinalizeAudioAsync(Guid id, CancellationToken ct = default)
    {
        var name = $"{id}.wav";
        var blobClient = storeClient.GetAppendBlobClient(name);
        var exists = await blobClient.ExistsAsync(ct);

        if (!exists)
        {
            return Result.Failure(AudioStoreErrors.AudioItemDoesNotExist);
        }

        try
        {
            await blobClient.SealAsync(cancellationToken: ct);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex);
        }

        return Result.Success();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        var name = $"{id}.wav";
        var blobClient = storeClient.GetAppendBlobClient(name);
        var response = await blobClient.ExistsAsync(ct);
        return response.Value;
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var name = $"{id}.wav";
        var blobClient = storeClient.GetAppendBlobClient(name);
        await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
        return Result.Success();
    }

    public async Task<Result<TimeSpan>> GetLengthAsync(Guid id, CancellationToken ct = default)
    {
        var name = $"{id}.wav";
        var blobClient = storeClient.GetAppendBlobClient(name);
        var exists = await blobClient.ExistsAsync(cancellationToken: ct);
        if (!exists.Value)
        {
            return Result<TimeSpan>.Failure(AudioStoreErrors.AudioItemDoesNotExist);
        }
        var downloadResponse = await blobClient.OpenReadAsync(cancellationToken: ct);

        var reader = new WaveFileChunkReader();
        reader.ReadWaveHeader(downloadResponse);
        var length = Convert.ToDouble((decimal)reader.DataChunkLength / reader.WaveFormat.AverageBytesPerSecond);
        return Result<TimeSpan>.Success(TimeSpan.FromSeconds(length));
    }

    public async Task<Result> GetStream(Guid id, Stream stream, CancellationToken ct = default)
    {
        var name = $"{id}.wav";
        var blobClient = storeClient.GetAppendBlobClient(name);
        var exists = await blobClient.ExistsAsync(cancellationToken: ct);
        if (!exists.Value)
        {
            return Result<Stream>.Failure(AudioStoreErrors.AudioItemDoesNotExist);
        }

        await blobClient.DownloadToAsync(stream, cancellationToken: ct);
        return Result.Success();
    }
}
