using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Host.Ports;

public interface IStoreAudio
{
    Task UploadAudioPartAsync(
        Guid id,
        Stream stream,
        CancellationToken ct = default);
    
    Task<Stream> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
