using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedServices;

namespace VO2DataManager.Web;

/// <summary>
/// Design-time factory — used by 'dotnet ef migrations add' when the DI host
/// cannot be fully resolved.
/// </summary>
public class AppDbContextAiDataDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContextAiData>
{
    public AppDbContextAiData CreateDbContext(string[] args)
    {
        const string cs =
            "Host=100.99.239.94;Port=5432;Database=AIData;Username=roundnet;Password=kindred;" +
            "Pooling=true;Timeout=50;Command Timeout=120;Ssl Mode=Disable";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContextAiData>();
        optionsBuilder.UseNpgsql(cs, o =>
        {
            o.MigrationsAssembly("SharedServices");
        });

        return new AppDbContextAiData(optionsBuilder.Options);
    }
}
