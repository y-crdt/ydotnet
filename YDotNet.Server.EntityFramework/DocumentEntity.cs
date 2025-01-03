namespace YDotNet.Server.EntityFramework;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("YDotNetDocuments")]
public sealed class DocumentEntity
{
    [Key]
    required public string Id { get; set; }

    required public byte[] Data { get; set; }

    public DateTime? Expiration { get; set; }
}
