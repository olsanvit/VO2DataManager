# ImportAlbums.razor
Route: `/import-albums`
Popis: Hromadný import alb z CSV (jen Admin) — upload, náhled, uložení.
Platforma: Web

## Hotovo ✅
- `[Authorize(Roles = "Admin")]`, CSV formát `Název,Interpret,RokVydání,Label,ConfidenceScore`
- Upload do 5 MB přes `InputFile`, vlastní quoted-CSV parser bez knihoven
- Náhled prvních 20 řádků, počet chybných řádků
- Stažení vzorového souboru
- Uložení jedním `AddRange` + `SaveChangesAsync`

## Chybí / Rozpracováno ⚠️
- Nefunkční — cílová tabulka neexistuje
- **Není v NavMenu** ani v Admin sekci — dostupné jen ručním zadáním URL
- Čistý append bez deduplikace/upsertu — dvojí nahrání = duplicity
- `Split('\n')` rozbije CSV s novým řádkem uvnitř uvozovek
- Nezvládá BOM ani `;` oddělovač (výchozí export Excelu v CZ)
- Celý soubor v jedné transakci bez progressu

## Návrhy na vylepšení 💡
- Jedna generická `<CsvImporter TEntity>` komponenta pro všechny tři importy
- Upsert podle `GuidId` / normalizovaného názvu
- Detekce oddělovače a BOM, parsování po znacích přes celý obsah (ne po řádcích)
- Dávky po 500 s progress barem
- Odkaz z Admin dashboardu a ze seznamu

## Brainstorming poznámky
- Po vzniku tabulek z AI procesu zvážit, jestli ruční import vůbec dává smysl — mohl by se místo toho posílat do `DiscoveryQueue`/`JobQueue` agentům
- Achievementy `VO2_FIRST_IMPORT`, `VO2_SONG_IMPORT_100`, `VO2_PERFECT_IMPORT` se nikde neodemykají
