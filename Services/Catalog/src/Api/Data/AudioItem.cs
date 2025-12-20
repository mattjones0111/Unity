using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Data;

public class AudioItem
{
    public Guid Id { get; set; }
    public string[] Categories { get; set; }
    [MaxLength(200)]
    public string Title { get; set; }
    [MaxLength(200)]
    public string? Artist { get; set; }
    public TimeSpan Duration { get; set; }
}
