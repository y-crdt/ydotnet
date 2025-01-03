namespace YDotNet.Server.EntityFramework;

using System;

public sealed class EFDocumentStorageOptions
{
    public Func<string, TimeSpan?>? Expiration { get; set; }

    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromMinutes(30);
}
