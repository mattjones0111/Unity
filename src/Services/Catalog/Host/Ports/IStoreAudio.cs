using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Host.Ports;

public interface IStoreAudio
{
    Task<UploadAudioResult> UploadBlockAsync(
        Guid id,
        Stream stream,
        int part,
        CancellationToken ct = default);

    Task CompleteUploadAsync(
        Guid id,
        string[] blockIds,
        CancellationToken ct = default);
    
    Task MoveAsync(Guid id, CancellationToken ct = default);
    
    Task<TimeSpan> GetDurationAsync(
        Guid id,
        CancellationToken ct = default);
    
    Task<Stream> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

public class UploadAudioResult
{
    public static UploadAudioResult Success(string blockId) => new(blockId);
    
    public string BlockId { get; }

    UploadAudioResult(string blockId)
    {
        BlockId = blockId;
    }
}
