# VO2 Data Manager — CLAUDE.md

## Projekt

Blazor Server aplikace pro správu hudebních dat (interpreti, alba, skladby) napojená na databázi AIData.

## Technologie

- **.NET 10 Blazor Server** (`@rendermode InteractiveServer`)
- **Entity Framework Core** přes `IDbContextFactory<AppDbContextAiData>`
- **Bootstrap 5** + **Bootstrap Icons** (`bi bi-*`)
- **ApexCharts** (Blazor-ApexCharts) — donut/pie charty na dashboardu
- **SharedServices** — submodul sdílených modelů, UI komponent a služeb

## DbContext

```csharp
AppDbContextAiData
```

Registrovaný jako factory: `IDbContextFactory<AppDbContextAiData>`. Vzor použití:

```csharp
await using var db = await DbFactory.CreateDbContextAsync();
```

## Modely (hudební doména)

Namespace: `SharedServices.Models.AiData`

| Model    | Klíčové vlastnosti                                                                    |
|----------|---------------------------------------------------------------------------------------|
| `Artist` | `Id`, `Name`, `SortName`, `ActiveFromYear`, `ActiveToYear`, `ConfidenceScore`, `OfficialWebsiteUrl`, `GuidId` |
| `Album`  | `Id`, `Title`, `ArtistDisplay`, `ReleaseYear`, `ConfidenceScore`, `Label`, `GuidId`, `CreatedAt`, `UpdatedAt` |
| `Song`   | `Id`, `Title`, `ReleaseYear`, `DurationSeconds`, `CreatedAt`, `UpdatedAt`            |

## Pages

| URL        | Soubor                     | Funkce                                     |
|------------|----------------------------|--------------------------------------------|
| `/`        | `Home.razor`               | Dashboard — počty karet + ApexCharts donut |
| `/artists` | `Artists.razor`            | CRUD + search (Name) + Export CSV          |
| `/albums`  | `Albums.razor`             | CRUD + search (Title, ArtistDisplay) + Export CSV |
| `/songs`   | `Songs.razor`              | CRUD + search (Title) + Export CSV         |

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

## Globální imports

- `SharedServices/Components/_Imports.razor` — obsahuje `@using ApexCharts`, Blazored.* atd.
- `VO2DataManager.Web/_Imports.razor` — obsahuje SharedServices.Models, BlazorVO2DataManager.Components atd.

## Struktura projektu

```
src/
  VO2DataManager.Web/          # Hlavní Blazor Server app
    Components/
      Pages/                   # Stránky (Artists, Albums, Songs, Home)
      Layout/                  # MainLayout, NavMenu
      UI/                      # Lokální UI komponenty
    wwwroot/js/
  VO2DataManager.Tests/        # xUnit testy
  SharedServices/              # Git submodul — modely, DbContext, UI komponenty
    SharedServices/
      Models/AiData/           # Všechny EF modely (Artist, Album, Song, ...)
      Components/UI/           # ConfirmDialog, PageLoadingSpinner, Paginator, ToastService
      Services/
```
