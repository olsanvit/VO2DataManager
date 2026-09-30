# SportDetail.razor
Route: `/sport/{Sport}`
Popis: Generický detail libovolného sportu — týmy, ligy, hráči a zápasy.
Platforma: Web

## Hotovo ✅
- **Nová stránka 2026-09-12** — nahrazuje potřebu psát pro každý sport vlastní stránku
- Schéma-agnostické čtení přes `to_jsonb(t) ->> 'Sloupec'`: chybějící sloupec vrátí NULL místo chyby, takže funguje i pro sporty bez `Name` (AmFootball) nebo bez `Country`
- Sport z URL se ověřuje regexem a proti `pg_class` — do SQL se nedostane nic neověřeného
- Každá záložka ve vlastním try/catch; chybějící tabulka označí jen svou sekci
- Hledání, zvýraznění skóre u zápasů, odkaz na zdroj

## Chybí / Rozpracováno ⚠️
- Zápasy zobrazují jen `Name`/skóre/status — sporty s denormalizovanými `HomeTeamName`/`AwayTeamName` (AmFootball) by šly zobrazit lépe
- Basketbal má skóre v `MatchTeams`, ne v `Matches` → na generické stránce se u něj skóre nezobrazí (proto zůstává i dedikovaná `/basketball`)
- Bez stránkování (`LIMIT 500`), bez řazení kliknutím, bez detailu jednotlivého týmu
- Tabulky `Standings`, `Venues`, `MatchStats` zatím nejsou

## Návrhy na vylepšení 💡
- Pátá záložka „Pořadí" z `<Sport>Standings`
- Detail týmu `/sport/{key}/team/{guid}` se zápasy a bilancí
- Sloučit s `/basketball` a `/amfootball` — dedikované stránky nechat jen tam, kde generická nestačí

## Brainstorming poznámky
- `to_jsonb` trik je klíč k práci s agentními tabulkami: schéma se mezi sporty liší a mění se v čase
- Alternativa by byla introspekce `information_schema` a stavba SQL na míru — složitější a pomalejší
