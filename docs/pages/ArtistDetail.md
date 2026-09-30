# ArtistDetail.razor
Route: `/artists/{Id:int}`
Popis: Detail interpreta — informace a seznam alb přes vazbu `ArtistAlbums`.
Platforma: Web

## Hotovo ✅
- Karta informací (název, sort name, aktivní od/do, skóre, web, artist key)
- Alba přes `ArtistAlbums` (GuidId), fallback podle `ArtistDisplay.Contains(Name)`
- Empty-state „Interpret nenalezen“ s návratem na seznam

## Chybí / Rozpracováno ⚠️
- Nefunkční — tabulky neexistují
- Odkazy z `MusicDashboard`/`SearchPage` přichází s GuidId, route je `{Id:int}` → 404
- Fallback `Contains(Name)` u krátkých jmen („Yes“, „Air“) natáhne nesouvisející alba
- Chybí skladby interpreta (`ArtistSongs`), žánry, editace z detailu, obrázek (`EntityImages` existuje)

## Návrhy na vylepšení 💡
- Druhá route `@page "/artists/g/{GuidId}"` nebo sjednotit celý projekt na GuidId
- Fallback porovnávat přesně (`ArtistDisplay == Name`) nebo úplně vypustit
- Diskografie jako timeline podle roku vydání
- Tlačítko Upravit přímo z detailu

## Brainstorming poznámky
- `FindAsync(Id)` — int PK nemusí přežít, až tabulky vytvoří agenti se standardním Guid schématem
