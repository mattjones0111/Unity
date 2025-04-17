using System.ComponentModel.DataAnnotations;

namespace Api.Host.Data;

public class Category
{
    [MaxLength(500)]
    public required string Path { get; set; }
    [MaxLength(50)]
    public required string Name { get; set; }
    [MaxLength(500)]
    public string? Parent { get; set; }
}