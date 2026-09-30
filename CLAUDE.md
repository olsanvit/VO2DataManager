# VO2 Data Manager — CLAUDE.md

## Projekt

Blazor Server aplikace nad databází **AIData** (pg16 na QNAPu). AIData plní AI agenti (sportovní data managery,
katalogy) a aplikace slouží k prohlížení a správě těchto dat + k monitoringu samotných agentů.

Whitelist aplikace (informativní): přístup povoluje jen `olsanskyvitek@gmail.com` přidáním do whitelistu
(`Authentication:AccessMode = Whitelist`).

> **Pozor:** Původní záměr byl hudební modul (Artists/Albums/Songs). Tyto tabulky v AIData **zatím neexistují** —
> vzniknou postupně přes AI proces. Není to bug (potvrzeno uživatelem 2026-08-06), ale stránky hudebního modulu
> dnes padají. Skutečný obsah AIData jsou agenti, ~40 sportů a doménové katalogy (viz níže).

## Technologie

- **.NET 10 Blazor Server** (`@rendermode InteractiveServer`)
- **Entity Framework Core** přes `IDbContextFactory<AppDbContextAiData>` (+ `AppDbContextAiDataIdentity` pro Identity)
- **Bootstrap 5** + **Bootstrap Icons** (`bi bi-*`), Bootswatch téma z `Theme:Url`
- **ApexCharts** (Blazor-ApexCharts), registrované i **MudBlazor** a **Radzen** (přepínač `UiLibrarySwitcher`)
- **Serilog** → konzole, soubor `Logs/`, a `Warning+` do tabulky `Logs` v AIData
- **SharedServices** — git submodul (modely, DbContexty, UI komponenty, auth `AddMabAuth`)

## Databáze AIData — jak ji číst

- ~494 tabulek v `public`. Většinu vytvářejí agenti se **standardním schématem**: PK `Guid` (uuid), `CreatedAt`,
  `UpdatedAt`, `IsDeleted`, `IsActive`, `NormalizedName`, `SearchText`, `SourceName`, `SourceUrl`, `Metadata` (jsonb)
  + ~20 skórovacích sloupců (`FinalScore`, `ConfidenceScore`…). Doménové sloupce jsou až za nimi (např. `Name`, `Country`).
- Sporty: `<Sport>{Leagues,Teams,Players,Matches,MatchTeams,MatchStats,Standings,Venues}` pro ~40 sportů
  (Football, IceHockey, Basketball, AmFootball, Tennis, Padel, Roundnet…). Většina má jednotky až desítky řádků.
  Pozor na nekonzistence mezi sporty: `AmFootballTeams` nemá `Name` (jen `NormalizedName`), `AmFootballMatches`
  má skóre přímo (`HomeScore`/`AwayScore`), `BasketballMatches` ne — skóre je v `BasketballMatchTeams` (`Role` home/away + `Score`).
- **Statusy agentů nejsou `ok`/`error`.** Reálné hodnoty v `AgentRunReports.Status`: `degraded` (nejčastější),
  `completed_with_warnings`, `blocked`, `partial`, `success`, `completed`, `halted_for_review`, `investigation`,
  a u ~27 agentů `NULL`. Klasifikuj rodinami (regex `error|blocked|halted|investigation|fail` = chyba,
  `degraded|warning|partial|ready` = varování, `^(ok|success|completed)$` = OK), ne rovností na `"ok"`.
- Agenti: `AgentSchedules`, `AgentRunReports`, `AgentCatalog`, `AgentEntities`, `SportProgress`, `PromptVersionPin`.
- Katalogy: `world_entity_catalog`, `popculture_catalog`, `catalog_suggestions`, Vehicles/VIN, Aircraft, Ship, Rail, Pin, Games, Metin2.

### ⚠️ `AppDbContextAiData` je pro sporty zastaralý scaffold
Entity `AmericanFootball*`, `BasketballTeam`, `BasketballMatch` atd. mapují **staré** tabulky/sloupce (`id`, `created_at`,
`ExternalSource`, `HomeParticipant`…), které v DB nejsou → EF dotaz spadne. Pro agentní tabulky proto používej
**raw SQL přes `db.Database.SqlQueryRaw<TRow>()` s lokální řádkovou třídou** (vzor: `Agents.razor`, `Basketball.razor`,
`AmFootball.razor`). Název tabulky do SQL skládej jen z konstant v kódu, nikdy z uživatelského vstupu.
Filtruj `"IsDeleted" = false`. Typy: `uuid`→`Guid`, `date`→`DateOnly?`, `timestamptz`→`DateTimeOffset?`, `numeric`→`decimal?`.

`EnsureCreatedAsync()` v `Program.cs` nic nevytvoří — DB už tabulky má, takže EF ji považuje za existující.

### Diagnostika DB
- MCP server **`qnap-ai`** (`https://mcp.vo2info.cz/AI`) = přesně AIData: `list_tables`, `get_table_schema`,
  `run_select_sql` (read-only, max 500 řádků), `table_stats`.
- Sloupce, které v některých sportech chybí, čti přes `to_jsonb(t) ->> 'Sloupec'` — vrátí NULL místo chyby
  (vzor: `SportDetail.razor`, `LogsPage.razor`).
- Existenci tabulek ověřuj přes `pg_class`, ne `information_schema` (ta skrývá tabulky bez GRANTu).
- `count(*)` na velkých tabulkách přes MCP umí timeoutovat — pro odhad použij `pg_stat_user_tables.n_live_tup`.
- Role: prod `vo2data_usr`, agenti `AgentAI`, vlastník většiny tabulek `roundnet`.
  **`roundnet` je superuser pg16 — v aplikaci ho nepoužívat, jen pro migrace** (`DesignTimeDbContextFactory`).
  Lokální DEBUG (`DefaultConnection1QNAP` v User Secrets) na něm dnes pořád jede → převést na vlastní roli.

## Pages

| URL | Soubor | Funkce | Stav (2026-09-12) |
|-----|--------|--------|------|
| `/` | `Home.razor` | Dashboard (počty hudby + donut) | ⚠️ načte se, ale bez hudebních tabulek je prázdný |
| `/agents` | `Agents.razor` | Přehled agentů, filtry, auto-refresh | ✅ raw SQL; 2026-09-12 opravena klasifikace statusů |
| `/agents/{AgentName}` | `AgentDetail.razor` | Historie běhů, blokátory | ✅ funguje (raw SQL) |
| `/basketball` | `Basketball.razor` | Týmy, ligy, zápasy, sezóny | ✅ raw SQL, build OK + dotazy ověřeny proti AIData 2026-09-12 |
| `/amfootball` | `AmFootball.razor` | Týmy, ligy, zápasy | ✅ raw SQL, build OK + dotazy ověřeny proti AIData 2026-09-12 |
| `/search` | `SearchPage.razor` | Hledání napříč agenty a sporty | ✅ raw SQL per kategorie, ověřeno 2026-09-12 |
| `/music-dashboard` | `MusicDashboard.razor` | Hudební přehled | ⚠️ chybí tabulky → ukáže prázdný stav, nepadá |
| `/artists`, `/albums`, `/songs` | `Artists/Albums/Songs.razor` | CRUD + CSV export | ❌ chybí tabulky (skryté v menu) |
| `/artists/{Id:int}` atd. | `*Detail.razor` | Detail + vazby | ❌ chybí tabulky |
| `/import-artists`, `/import-albums`, `/import-songs` | `Import*.razor` | CSV import (Admin) | ❌ chybí tabulky, nejsou v menu |
| `/achievements` | `AchievementsPage.razor` | Grid achievementů | ⚠️ ~100 definic, nikde se nevolá `Unlock()` |
| `/admin` | `Admin/AdminDashboard.razor` | Stav AIData, agenti, uživatelé | ✅ přepsáno na raw SQL, ověřeno 2026-09-12 |
| `/sports` | `Sports.razor` | Přehled ~40 sportů a jejich pokrytí daty | ✅ nová stránka 2026-09-12 |
| `/sport/{Sport}` | `SportDetail.razor` | Generický detail sportu (týmy/ligy/hráči/zápasy) | ✅ nová stránka 2026-09-12 |
| `/admin/logs` | `Admin/LogsPage.razor` | Serilog Warning+ z tabulky `Logs` | ✅ nová stránka 2026-09-12 |
| `/admin/access` | `UserAccessPage` (SharedServices) | Správa whitelistu | ✅ existovala, jen chyběl odkaz v menu |
| `/admin/audit` | `Admin/AuditReport.razor` | iframe na autorizovaný `/admin/audit-report` | ⚠️ obsah statický z 2026-07-11 |
| `/admin/projects` | `Admin/ProjectsOverview.razor` | Karty projektů | ✅ hardcoded data |

Podrobná analýza každé stránky: `docs/pages/<Stránka>.md`.

## Konvence

### Form binding
Každá stránka s formulářem má privátní vnitřní třídu `*FormModel` (např. `SongFormModel`, `ArtistFormModel`, `AlbumFormModel`) pro izolaci stavu formuláře od entit EF.

### Search pattern
```csharp
private string _search = string.Empty;
// markup:
// @bind="_search" @bind:event="oninput" @bind:after="OnSearchChanged"

private async Task OnSearchChanged()
{
    _page = 1;
    await LoadAsync();
}

// v LoadAsync():
if (!string.IsNullOrWhiteSpace(_search))
    query = query.Where(e => e.Name != null && e.Name.Contains(_search));
```

### Export CSV pattern
```csharp
@inject IJSRuntime JS
// ...
private async Task ExportCsvAsync()
{
    await using var db = await DbFactory.CreateDbContextAsync();
    var all = await db.Songs.OrderBy(s => s.Title).ToListAsync();
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("Název,Rok vydání,Délka (s)");
    foreach (var s in all)
        sb.AppendLine($"\"{s.Title?.Replace("\"", "\"\"")}\",{s.ReleaseYear},{s.DurationSeconds}");
    var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
    await JS.InvokeVoidAsync("downloadBase64File", "skladby.csv", "text/csv", Convert.ToBase64String(bytes));
}
```
JS helper `downloadBase64File` je registrován globálně (`js/theme.js` nebo analogický soubor).

### Delete confirm
Používá `<ConfirmDialog @ref="_confirmDialog" />` komponentu z SharedServices.UI:

```csharp
var confirmed = await _confirmDialog.ShowAsync("Smazat záznam", "Opravdu chcete smazat...?");
if (confirmed) await DeleteAsync(id);
```

### Loading stav
```razor
@if (_loading)
{
    <PageLoadingSpinner />
}
```

### Paginace
```razor
<Paginator TotalItems="_total" PageSize="_pageSize" CurrentPage="_page"
           OnPageChanged="async p => { _page = p; await LoadAsync(); }" />
```

### Toast notifikace
```csharp
@inject ToastService Toast
// ...
Toast.ShowSuccess("Uloženo", "Záznam byl úspěšně uložen.");
Toast.ShowError("Chyba", $"Nepodařilo se: {ex.Message}");
```

## Známé problémy

- **Hesla v historii veřejného repa:** soubory jsou od 2026-09-12 odtrackované a v `.gitignore`, ale staré
  commity je pořád obsahují → role `roundnet` (superuser pg16) i `vo2data_usr` **čekají na rotaci**.

- Odkazy z `MusicDashboard`/`SearchPage` na detaily používají `GuidId`, ale routy jsou `{Id:int}` → 404.
- `Agents`/`AgentDetail`: timer volá `StateHasChanged` každou sekundu kvůli odpočtu.
  (ntfy odkaz i natvrdo zadaná verze promptu opraveny 2026-09-12 — nejnovější verze se bere z dat.)
- `AiDataSyncService` je zaregistrovaný, ale nikdo ho nevolá.
- Startup musí snést nedostupnou DB: migrace, `EnsureCreated` i `AdminUserSeeder` jsou v `try/catch`. Seeder dřív nebyl →
  po nečistém restartu QNAPu (pg16 v recovery) kontejner padal dokola a neodpovídal ani `/health` (opraveno 2026-09-14,
  na produkci se projeví až dalším deployem).
- MAUI app se připojuje přímo k PostgreSQL s heslem v embedded `appsettings.json` a bez autentizace.

## Globální imports

> Komponenty ze SharedServices, které mají vlastní `@namespace` (`LangSwitcher`, `Login`… →
> `MercenariesAndBeasts.Infrastructure.Components.*`), potřebují vlastní `@using` — jinak je Razor bere jako
> neznámý HTML tag a jen varuje (`RZ10012`), nespadne. Opraveno 2026-09-12 pro `LangSwitcher`.

- `SharedServices/Components/_Imports.razor` — obsahuje `@using ApexCharts`, Blazored.* atd.
- `VO2DataManager.Web/_Imports.razor` — obsahuje SharedServices.Models, BlazorVO2DataManager.Components atd.

## Struktura projektu

```
src/
  VO2DataManager.Web/          # Hlavní Blazor Server app
    Components/
      Pages/                   # Stránky (viz tabulka výše), Admin/ podsložka
      Layout/                  # MainLayout, NavMenu
      UI/                      # Paginator, QuickLinkCard
    Achievements/              # VO2Achievements (definice)
    Services/                  # AiDataSyncService
    wwwroot/js/theme.js        # mj. downloadBase64File pro CSV export
  VO2DataManager.Mobile/       # MAUI Blazor Hybrid (Home, Artists, Albums)
  VO2DataManager.Tests/        # xUnit testy + smoke IntegrationTests
  SharedServices/              # Git submodul — modely, DbContext, UI komponenty
docs/pages/                    # Dokumentace a brainstorming per stránka
```

## Konfigurace (bez secrets)

`appsettings.json` má connection stringy prázdné — skutečné hodnoty se berou odjinud:

- **Dev:** User Secrets (`UserSecretsId` v `.csproj`) — `dotnet user-secrets set "ConnectionStrings:DefaultConnection1QNAP" "..."`.
- **Prod:** proměnná prostředí `ConnectionStrings__DefaultConnection1` nebo `appsettings.Production.json`
  přímo na serveru (soubor je v `.gitignore`, do repa nepatří).
- `DesignTimeDbContextFactory` pro `dotnet ef` čte User Secrets / env, connection string nemá v kódu.

## Deploy

`./deploy.sh [prod|dev]` → `~/deploy-to-qnap.sh vo2data`:

1. lokální `dotnet publish -c Release`,
2. `rsync -az --delete` do `/share/Public/BlazorVO2DataManager/publish` (dev: `.../BlazorVO2DataManagerdev/publish`),
3. `docker restart vo2datamanager` (dev `vo2datamanager-dev`) — kontejner mountuje publish jako `/app:ro`.

**Není to docker compose** — obecný postup `docker compose pull && up -d` pro tento projekt neplatí.

| | Prod | Dev |
|---|---|---|
| Doména | `https://datamanager.vo2info.cz` | `dev.datamanager.vo2info.cz` — v DNS zatím neexistuje |
| Port na QNAPu | `127.0.0.1:5001` | `127.0.0.1:5101` |
| Kontejner | `vo2datamanager` | `vo2datamanager-dev` — zatím nevytvořen |

- `appsettings.Production.json` žije **jen na QNAPu**. Ve skriptu je pro vo2data `KEEP_REMOTE_PROD_CFG=1`,
  aby ho `rsync --delete` nesmazal (appsettings.json v repu má connection stringy prázdné).
- Po deployi ověřit zevnitř QNAPu: `curl http://127.0.0.1:5001/health` — `docker ps` = kontejner běží ≠ appka žije.
- SSH: `ssh -i ~/.ssh/claude-qnap admin@192.168.60.221`, když LAN nereaguje, zkusit `admin@qnapol` (Tailscale).
- Před buildem/deployem kontrolovat `/tmp/claude-deploy.lock` (mladší 30 min = nespouštět).
