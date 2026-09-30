# AlbumDetail.razor
Route: `/albums/{Id:int}`
Popis: Detail alba — tracklist přes `AlbumSongs` a interpreti přes `ArtistAlbums`.
Platforma: Web

## Hotovo ✅
- Tracklist: `AlbumSongs` → slovník `Songs` podle GuidId (bez N+1)
- Interpreti alba přes `ArtistAlbums`
- `AsNoTracking()` u všech dotazů

## Chybí / Rozpracováno ⚠️
- Nefunkční — tabulky neexistují
- GuidId vs `{Id:int}` route mismatch z jiných stránek
- Tracklist se neřadí podle `DiscNumber`/`TrackNumber` (SongDetail to dělá)
- Chybí celková délka alba, obal

## Návrhy na vylepšení 💡
- Řadit tracklist `DiscNumber, TrackNumber`, součet délky
- Proklik ze skladby na `SongDetail`, z interpreta na `ArtistDetail`

## Brainstorming poznámky
- Pokud vazba chybí (song GuidId neexistuje), řádek se ukáže s prázdnou skladbou — zvážit označení „nenalezeno“
