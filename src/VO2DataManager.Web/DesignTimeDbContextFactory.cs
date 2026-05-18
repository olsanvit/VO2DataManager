using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedServices;

namespace VO2DataManager.Web;

/// <summary>
/// Design-time factory used by <c>dotnet ef migrations add</c> when the DI host
/// cannot be fully resolved at build time.
/// Provides a hard-coded connection string pointing to the development AIData PostgreSQL database.
/// </summary>
public class AppDbContextAiDataDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContextAiData>
{
    /// <summary>
    /// Creates and returns a configured <see cref="AppDbContextAiData"/> instance for use by EF Core tooling.
    /// The connection string is hard-coded to the development database and should not be used in production.
    /// </summary>
    /// <param name="args">Command-line arguments passed by the EF Core tooling; not used.</param>
    /// <returns>A fully configured <see cref="AppDbContextAiData"/> instance ready for migration operations.</returns>
    public AppDbContextAiData CreateDbContext(string[] args)
    {
        const string cs =
            "Host=100.99.239.94;Port=5432;Database=AIData;Username=roundnet;Password=kindred;" +
            "Pooling=true;Timeout=50;Command Timeout=120;Ssl Mode=Disable";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContextAiData>();
        optionsBuilder.UseNpgsql(cs);

        return new AppDbContextAiData(optionsBuilder.Options);
    }
}
