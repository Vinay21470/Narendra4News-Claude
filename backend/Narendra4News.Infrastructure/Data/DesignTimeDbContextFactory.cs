using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Narendra4News.Infrastructure.Data;

// Migration generation does not start the API, seed users, or need production secrets.
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(
            Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION_STRING")
            ?? "Server=localhost;Database=N4ncDesignOnly;Integrated Security=True;TrustServerCertificate=True");
        return new ApplicationDbContext(options.Options);
    }
}
