namespace YDotNet.Server.EntityFramework;

using System;
using System.ComponentModel.DataAnnotations;

public sealed class YDotNetDocument
{
    [Key]
    required public string Id { get; set; }

    required public byte[] Data { get; set; }

    public DateTime? Expiration { get; set; }
}
