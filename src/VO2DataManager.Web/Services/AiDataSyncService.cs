using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedServices;
using System.Text;

namespace VO2DataManager.Services;

/// <summary>
/// Denní služba, která porovnává aktuální schéma databáze AIData
/// s očekávaným stavem (dle DbContextu) a loguje rozdíly.
///
/// Spouštění: každý den přes WebsiteTask "AiDataSchemaSync".
/// Při detekci nových/odstraněných tabulek zapíše varování do logu.
/// Pro re-scaffold spusť ručně:
///   dotnet ef dbcontext scaffold ... --force
/// </summary>
public class AiDataSyncService
{
    private readonly IDbContextFactory<AppDbContextAiData> _factory;
    private readonly ILogger<AiDataSyncService> _log;
    private readonly IConfiguration _config;

    public AiDataSyncService(
        IDbContextFactory<AppDbContextAiData> factory,
        ILogger<AiDataSyncService> log,
        IConfiguration config)
    {
        _factory = factory;
        _log = log;
        _config = config;
    }

    // ────────────────────────────────────────────────────────────────────────
    // Hlavní metoda — volaná denně ze scheduleru / WebsiteTask
    // ────────────────────────────────────────────────────────────────────────

    public async Task RunDailyAsync(CancellationToken ct = default)
    {
        _log.LogInformation("AiDataSyncService: spuštěn schema check.");

        await using var db = await _factory.CreateDbContextAsync(ct);

        // Tabulky evidované v DbContextu
        var contextTables = db.Model.GetEntityTypes()
            .Select(e => e.GetTableName()!)
            .Where(t => !string.IsNullOrEmpty(t))
            .OrderBy(t => t)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Tabulky skutečně v databázi (information_schema)
        var dbTables = await db.Database
            .SqlQueryRaw<string>(
                "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE' ORDER BY table_name")
            .ToListAsync(ct);

        var dbTableSet = dbTables.ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Rozdíly
        var newInDb       = dbTableSet.Except(contextTables).OrderBy(x => x).ToList();
        var removedFromDb = contextTables.Except(dbTableSet).OrderBy(x => x).ToList();

        var sb = new StringBuilder();
        sb.AppendLine($"=== AIData Schema Sync – {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC ===");
        sb.AppendLine($"Context tables : {contextTables.Count}");
        sb.AppendLine($"DB tables      : {dbTableSet.Count}");

        if (newInDb.Count > 0)
        {
            sb.AppendLine($"\n⚠ NOVÉ tabulky v DB (chybí v DbContextu) [{newInDb.Count}]:");
            foreach (var t in newInDb)
                sb.AppendLine($"  + {t}");
        }

        if (removedFromDb.Count > 0)
        {
            sb.AppendLine($"\n⚠ CHYBĚJÍCÍ tabulky (jsou v DbContextu, ale ne v DB) [{removedFromDb.Count}]:");
            foreach (var t in removedFromDb)
                sb.AppendLine($"  - {t}");
        }

        if (newInDb.Count == 0 && removedFromDb.Count == 0)
        {
            sb.AppendLine("\n✅ Schéma je v pořádku — žádné rozdíly.");
            _log.LogInformation("AiDataSyncService: {Report}", sb.ToString());
        }
        else
        {
            _log.LogWarning("AiDataSyncService: {Report}", sb.ToString());
            // Connection string pro scaffold — bez hesla v logu
            var scaffoldCs = _config.GetConnectionString("AiDataConnection") ?? "<viz appsettings — AiDataConnection>";
            _log.LogWarning(
                "AiDataSyncService: Pro aktualizaci modelů spusť:\n" +
                "dotnet ef dbcontext scaffold \"{Cs}\" " +
                "Npgsql.EntityFrameworkCore.PostgreSQL " +
                "-s src/VO2DataManager.Web -p src/SharedServices/SharedServices " +
                "--output-dir Models/AiData --context AppDbContextAiDataScaffold " +
                "--context-dir . --namespace SharedServices.Models.AiData " +
                "--context-namespace SharedServices --no-onconfiguring --force",
                scaffoldCs);
        }

        _log.LogInformation("AiDataSyncService: dokončen.");
    }
}
