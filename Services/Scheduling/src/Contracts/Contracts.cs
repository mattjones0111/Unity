using System;
using System.Collections.Generic;
using System.Drawing;

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

public abstract record TemplateItem(
    Guid Id,
    TimeSpan Duration);

public record CommentTemplateItem(
    Guid Id,
    string Comment)
    : TemplateItem(Id, TimeSpan.Zero);

public record AudioCriteriaTemplateItem(
    Guid Id,
    string Query)
    : TemplateItem(Id, TimeSpan.Zero);

public record AudioItemTemplateItem(
    Guid Id,
    TimeSpan Duration,
    Guid AudioItemId)
    : TemplateItem(Id, Duration);

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