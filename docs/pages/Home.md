# Home.razor
Route: `/`
Popis: Úvodní dashboard po přihlášení — počty interpretů/alb/skladeb, donut graf, rychlé odkazy na hudební modul.
Platforma: Web (mobilní obdoba: `Mobile.Home.md`)

## Hotovo ✅
- `[Authorize]`, `@rendermode InteractiveServer`
- 3× `UiStats` (interpreti, alba, skladby) + ApexCharts donut „Poměr obsahu“
- 3× `QuickLinkCard` na `/artists`, `/albums`, `/songs`
- Graceful fallback: `catch (Npgsql.PostgresException)` → `_musicModuleAvailable = false`, stránka nespadne

## Chybí / Rozpracováno ⚠️
- **V produkci je stránka prázdná** — hudební tabulky v AIData neexistují, takže se zobrazí jen nadpis „Dashboard“ bez vysvětlení
- Chybí empty-state („Hudební modul čeká na data od AI agentů“)
- Donut má natvrdo `Theme.Mode = Dark` a hardcoded barvy — ve světlém Bootswatch tématu (`flatly`) nevypadá dobře
- QuickLinkCards vedou na stránky, které dnes padají

## Návrhy na vylepšení 💡
- Předělat na přehled celé AIData: kachle „sporty s daty / celkem“, „agenti OK / chyba / po termínu“, „tabulek v DB“, „posledních 24 h běhů“
- Panel „Poslední běhy agentů“ (5 řádků z `AgentRunReports`) s prokliky na `/agents/{name}`
- Top sporty podle pokrytí z `SportProgress` (273 řádků, dnes nikde nezobrazeno)
- Hudební kachle ukazovat podmíněně jen když `_musicModuleAvailable`
- Barvy grafu brát z `ThemeService.BsThemeAttr` (dark/light)

## Brainstorming poznámky
- Počty přes `CountAsync()` sériově — při rozšíření na desítky tabulek použít jeden SQL dotaz nad `pg_stat_user_tables` (odhad, rychlé)
- Dashboard je jediná věc, kterou uživatel po loginu vidí → rozhoduje, jestli aplikaci vůbec otevře znovu
- Achievementy `VO2_VISIT_HOME`, `VO2_DONUT_CHART`, `VO2_STATS_CARD_*` se mají odemykat právě tady — nikde se nevolá `Unlock()`
