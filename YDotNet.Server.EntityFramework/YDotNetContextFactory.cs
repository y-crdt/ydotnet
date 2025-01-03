namespace YDotNet.Server.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class YDotNetContextFactory : IDesignTimeDbContextFactory<YDotNetContext>
{
    public YDotNetContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<YDotNetContext>();
        optionsBuilder.UseSqlite("Data Source=ydotnet.db");

        return new YDotNetContext(optionsBuilder.Options);
    }
}
