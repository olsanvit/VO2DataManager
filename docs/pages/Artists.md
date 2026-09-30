# Artists.razor
Route: `/artists`
Popis: CRUD seznam interpretů — inline formulář, stránkování, hledání, export CSV.
Platforma: Web (mobilní obdoba: `Mobile.ArtistsPage.md`)

## Hotovo ✅
- Formulář `*FormModel` (Název, Sort Name, Aktivní od/do, Skóre, Web, GuidId)
- Stránkování po 50 (`Paginator`), `UiSearchBar`, počet záznamů
- Mazání přes `ConfirmDialog`, `ToastService` pro výsledky
- Export CSV přes `downloadBase64File`
- Fallback řazení `SortName ?? Name`

## Chybí / Rozpracováno ⚠️
- **Nefunkční** — tabulka v AIData neexistuje (čeká na AI proces, není bug); stránka je skrytá z NavMenu
- Žádná validace (lze uložit prázdný název, GuidId je volný text bez kontroly formátu/unikátnosti)
- Hledání přes `Contains` = case-sensitive v PostgreSQL (jinde projekt používá `ILike`)
- Export načte celou tabulku do paměti; escapování neřeší CR/LF v hodnotách
- Chybí řazení kliknutím na hlavičku, bulk akce, `AsNoTracking()` u seznamu
- Import stránka existuje, ale odsud na ni nevede odkaz

## Návrhy na vylepšení 💡
- Generická komponenta `<CrudTable TEntity>` pro všechny tři seznamy (sdílí ~80 % kódu)
- Validace přes `EditForm` + DataAnnotations / FluentValidation (balíček už je v `_Imports`)
- `EF.Functions.ILike` místo `Contains`
- Tlačítko „Import CSV“ vedle „Export CSV“ (pro Admina)
- Stream export po dávkách místo `ToListAsync()` celé tabulky

## Brainstorming poznámky
- Až tabulky vzniknou z AI procesu, CRUD z UI může kolidovat se zápisy agentů — rozhodnout, kdo je vlastník dat (UI = jen oprava, agent = zdroj)
- Záznamy vytvořené agenty budou mít standardní schéma (Guid PK) — scaffold `Artist` s `int Id` nemusí sedět, ověřit po vzniku tabulek
- Achievementy `VO2_*_EXPORT`, `*_DELETE`, `*_EDIT`, `*_PAGE_2` se nikde neodemykají
