namespace YDotNet.Server.EntityFramework;

using Microsoft.EntityFrameworkCore;

public class YDotNetContext : DbContext
{
    public YDotNetContext()
    {
    }

    public YDotNetContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<DocumentEntity> Documents { get; set; }
}
