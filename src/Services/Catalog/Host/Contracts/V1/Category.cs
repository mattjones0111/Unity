namespace Api.Host.Contracts.V1;

public record Category(
    string Slug,
    string Name,
    Category[] Children);
