using System;
using FastEndpoints;

namespace Api.Host.Features.Artist;

public record ArtistCreated(Guid Id, string Name) : IEvent;