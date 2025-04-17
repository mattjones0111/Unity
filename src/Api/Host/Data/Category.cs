namespace Api.Host.Data;

public class Category
{
    public required string Path { get; set; }
    public required string Name { get; set; }
    public string? Parent { get; set; }
}
