using System;

namespace Api.Host.Contracts.V1;

[MimeType("audio-item")]
public record AudioItem(
    Guid Id,
    string Title,
    AudioItemArtist[] Artists,
    Marker[] Markers,
    string[] Categories);

public record AudioItemArtist(
    Guid Id,
    string Name);

public record Marker(
    string Type,
    uint Offset);

public static class MarkerTypes
{
    public const string VocalStart = nameof(VocalStart);
    public const string Segue = nameof(Segue);
}
