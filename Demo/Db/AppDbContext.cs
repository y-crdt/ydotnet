using Microsoft.EntityFrameworkCore;
using YDotNet.Server.EntityFramework;

namespace Demo.Db;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseYDotNet();
        base.OnModelCreating(modelBuilder);
    }
}
