# SongDetail.razor
Route: `/songs/{Id:int}`
Popis: Detail skladby — interpreti (primární/featuring) a alba, na kterých skladba vyšla.
Platforma: Web

## Hotovo ✅
- Interpreti přes `ArtistSongs` s příznakem `IsPrimary`, řazení primární první
- Alba přes `AlbumSongs` seřazená podle disku a stopy
- Formát délky m:ss

## Chybí / Rozpracováno ⚠️
- Nefunkční — tabulky neexistují
- Bez `AsNoTracking()` (na rozdíl od AlbumDetail)
- Route mismatch GuidId vs int

## Návrhy na vylepšení 💡
- Odkazy na YouTube/Spotify vyhledávání podle názvu + interpreta
- Zobrazit, v kolika albech/kompilacích skladba je

## Brainstorming poznámky
- `IsPrimary == 1` — int místo bool ze scaffoldu, typická stopa po generovaném schématu
