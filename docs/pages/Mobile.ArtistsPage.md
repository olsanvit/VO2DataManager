# ArtistsPage.razor
Route: `/artists (MAUI)`
Popis: Mobilní seznam interpretů — hledání a stránkování po 25.
Platforma: Mobile (MAUI)

## Hotovo ✅
- Hledání s `@bind:after`, stránkování Předchozí/Další po 25
- Přímý EF dotaz na AIData

## Chybí / Rozpracováno ⚠️
- Nefunkční — tabulka neexistuje
- Přímé DB připojení z mobilu (heslo v balíčku), bez autentizace
- `Contains` = case-sensitive hledání
- Bez detailu záznamu

## Návrhy na vylepšení 💡
- Napojit na API místo DB
- Pull-to-refresh, nekonečné scrollování místo stránkování

## Brainstorming poznámky
- Stránka Songs v mobilu chybí úplně
