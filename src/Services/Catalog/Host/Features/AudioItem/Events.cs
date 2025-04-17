using System;
using FastEndpoints;

namespace Api.Host.Features.AudioItem;

public record AudioUploaded(Guid Id) : IEvent;

public record AudioItemCreated(Guid Id, string Title, string[] CategoryPaths) : IEvent;

public record AudioItemDeleted(Guid Id) : IEvent;
