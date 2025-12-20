using System;
using System.Collections.Generic;

namespace Contracts;

public record AudioItem(
    Guid Id,
    Guid AudioFileId,
    string Title,
    Category[] Categories,
    ArtistContext[] Artists,
    TimeSpan Duration,
    Marker[] Markers,
    Dictionary<string, string>? Properties = null);

public record ArtistContext(Artist Artist, string Type);

public record Artist(
    Guid Id,
    string Name,
    Dictionary<string, string>? Properties = null);

public record ArtistSlim(Guid Id, string Name);

public record Category(string Path);

public record Marker(TimeSpan Offset, string Type);

public static class MarkerTypes
{
    public const string VocalStart = nameof(VocalStart);
    public const string Segue = nameof(Segue);
    public const string HookStart = nameof(HookStart);
    public const string HookEnd = nameof(HookEnd);
}

public static class ArtistContextTypes
{
    public const string Performer = nameof(Performer);
    public const string Composer = nameof(Composer);
    public const string Arranger = nameof(Arranger);
}

public record AudioSlim(
    Guid Id,
    string[] Categories,
    string Title,
    string? Artist,
    TimeSpan Duration);
