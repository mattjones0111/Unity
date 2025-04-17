using System;

namespace Api.Host.Contracts.V1;

[MimeType("artist")]
public record Artist(Guid Id, string Name);

public record ArtistSlim(Guid Id, string Name);
