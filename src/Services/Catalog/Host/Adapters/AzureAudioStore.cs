using System;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Api.Host.Ports;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace Api.Host.Adapters;

public sealed class AzureAudioStore : IStoreAudio
{
    const string StagingContainerName = "audio-staging";
    const string MainContainerName = "audio";
    const decimal BytesPerMillisecond = 176.4M;

    readonly BlobServiceClient blobServiceClient;

    public AzureAudioStore(string connectionString)
    {
        blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<UploadAudioResult> UploadBlockAsync(
        Guid id,
        Stream stream,
        int part,
        CancellationToken ct = default)
    {
        if (!stream.CanRead)
        {
            throw new InvalidOperationException("Stream is not readable.");
        }

        var containerClient = blobServiceClient.GetBlobContainerClient(StagingContainerName);
        var blockBlobClient = containerClient.GetBlockBlobClient(id.ToString());
        string blockId = Convert.ToBase64String(BitConverter.GetBytes(part));
        await blockBlobClient.StageBlockAsync(
            base64BlockId: blockId,
            content: stream,
            cancellationToken: ct
        );

        return UploadAudioResult.Success(blockId);
    }

    public async Task CompleteUploadAsync(
        Guid id,
        string[] blockIds,
        CancellationToken ct = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(StagingContainerName);
        var blockBlobClient = containerClient.GetBlockBlobClient(id.ToString());
        await blockBlobClient.CommitBlockListAsync(blockIds, cancellationToken: ct);
    }

    public async Task MoveAsync(Guid id, CancellationToken ct = default)
    {
        var sourceClient = blobServiceClient.GetBlobContainerClient(StagingContainerName);
        var destinationClient = blobServiceClient.GetBlobContainerClient(MainContainerName);
        var sourceBlob = sourceClient.GetBlobClient(id.ToString());
        var destinationBlob = destinationClient.GetBlobClient(id.ToString());
        
        await destinationBlob.StartCopyFromUriAsync(sourceBlob.Uri, cancellationToken: ct);

        while ((await destinationBlob.GetPropertiesAsync(cancellationToken: ct)).Value.CopyStatus == CopyStatus.Pending)
        {
            await Task.Delay(100, ct);
        }

        await sourceBlob.DeleteIfExistsAsync(cancellationToken: ct);
    }

    public async Task<TimeSpan> GetDurationAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(MainContainerName);
        var blobClient = containerClient.GetBlobClient(id.ToString());
        var props = await blobClient.GetPropertiesAsync(cancellationToken: ct);
        var milliseconds = (long)(props.Value.ContentLength / BytesPerMillisecond);
        return props.HasValue ? (TimeSpan.FromMilliseconds(milliseconds)) : TimeSpan.Zero;
    }

    public Task<Stream> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
