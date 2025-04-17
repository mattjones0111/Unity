using System;

namespace Api.Host.Contracts;

[AttributeUsage(AttributeTargets.Class)]
public class MimeTypeAttribute : Attribute
{
    public string MimeType { get; }

    public MimeTypeAttribute(
        string name,
        string vendor = "unity",
        int version = 1)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(vendor);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(version);

        MimeType = $"application/vnd.{vendor}.{name}.v{version}+json";
    }
}
