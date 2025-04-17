using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Api.Host.Ports;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

namespace Api.Host.Adapters;

public sealed class AzureAudioStore : IStoreAudio
{
    readonly string connectionString;

    public AzureAudioStore(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task UploadAudioPartAsync(
        Guid id,
        Stream stream,
        CancellationToken ct = default)
    {
        if (!stream.CanRead)
        {
            throw new InvalidOperationException("Stream is not readable.");
        }
        
        BlobServiceClient client = new(connectionString);
        BlobContainerClient containerClient = client.GetBlobContainerClient("audio");
        AppendBlobClient? appendClient = containerClient.GetAppendBlobClient(id.ToString());

        await appendClient.CreateIfNotExistsAsync(cancellationToken: ct);
        int maxBlockSize = appendClient.AppendBlobMaxAppendBlockBytes;
        long bytesLeft = stream.Length;
        byte[] buffer = new byte[maxBlockSize];
        while (bytesLeft > 0)
        {
            int blockSize = (int)Math.Min(bytesLeft, maxBlockSize);
            int bytesRead = await stream.ReadAsync(buffer.AsMemory(0, blockSize), ct);
            await using (MemoryStream memoryStream = new(buffer, 0, bytesRead))
            {
                await appendClient.AppendBlockAsync(memoryStream, cancellationToken: ct);
            }
            bytesLeft -= bytesRead;
        }
    }

    public Task<Stream> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
