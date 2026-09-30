using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SharedServices;

namespace VO2DataManager.Web;

/// <summary>
/// Design-time factory used by <c>dotnet ef migrations add</c> when the DI host
/// cannot be fully resolved at build time.
/// </summary>
public class AppDbContextAiDataDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContextAiData>
{
    /// <summary>
    /// Creates and returns a configured <see cref="AppDbContextAiData"/> instance for use by EF Core tooling.
    /// </summary>
    /// <param name="args">Command-line arguments passed by the EF Core tooling; not used.</param>
    /// <returns>A fully configured <see cref="AppDbContextAiData"/> instance ready for migration operations.</returns>
    public AppDbContextAiData CreateDbContext(string[] args)
    {
        // Connection string byl dřív hardcodovaný i s heslem — repo je public, takže se čte
        // z User Secrets (vývojářův stroj) nebo z proměnných prostředí, nikdy ze zdrojáku.
        var config = new ConfigurationBuilder()
            .AddUserSecrets<AppDbContextAiDataDesignTimeFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("DefaultConnection1QNAP")
              ?? config.GetConnectionString("DefaultConnection1")
              ?? throw new InvalidOperationException(
                     "Chybí connection string pro EF nástroje. Nastav ho přes " +
                     "'dotnet user-secrets set \"ConnectionStrings:DefaultConnection1\" \"...\"' " +
                     "nebo přes proměnnou ConnectionStrings__DefaultConnection1.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContextAiData>();
        optionsBuilder.UseNpgsql(cs);

        return new AppDbContextAiData(optionsBuilder.Options);
    }
}
