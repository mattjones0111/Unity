using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contracts;

public record Channel(
    Guid Id,
    string Name);

public record ProgrammeTemplate(
    Guid Id,
    string Title,
    TimeSpan Duration,
    TemplateItem[] Items,
    Dictionary<string, string>? Properties = null);

[JsonDerivedType(typeof(CommentTemplateItem), "comment")]
[JsonDerivedType(typeof(AudioCriteriaTemplateItem), "audio-criteria")]
[JsonDerivedType(typeof(AudioItemTemplateItem), "audio-item")]
public record TemplateItem(
    string Description,
    TimeSpan Duration);

public record CommentTemplateItem(
    string Comment)
    : TemplateItem(Comment, TimeSpan.Zero);

public record AudioCriteriaTemplateItem(
    string Query)
    : TemplateItem(Query, TimeSpan.Zero);

public record AudioItemTemplateItem(
    TimeSpan Duration,
    Guid AudioItemId,
    string Title)
    : TemplateItem(Title, Duration);

public record ScheduleTemplate(
    Guid Id,
    Channel Channel,
    DayOfWeek DayOfWeek,
    TimeOnly Start,
    Guid ProgrammeTemplateId);

public record GenerateLogRequest(
    Guid ChannelId,
    DateTime Start,
    DateTime End);
