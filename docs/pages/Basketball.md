# Basketball.razor
Route: `/basketball`
Popis: Basketbal — týmy, ligy, zápasy se skóre a sezóny soutěží z agentních tabulek `Basketball*`.
Platforma: Web

## Hotovo ✅
- **2026-09-11 přepsáno na raw SQL** — EF entity `BasketballTeam`/`BasketballMatch` čekaly staré sloupce (`id`, `ExternalSource`, `HomeParticipant`, `Score`), stránka padala
- Týmy: název, země, zdroj, aktualizace; hledání podle názvu i země
- Zápasy: pivot `BasketballMatchTeams` (Role home/away) přes `FILTER` → jeden řádek = jeden zápas se skóre, zvýraznění vítěze
- Sezóny: `BasketballLeagueSeasons` + liga + sezóna/rok
- Chyba načtení v panelu místo pádu

- **Ověřeno 2026-09-12:** build OK, pivot dotaz prověřen proti AIData (24 týmů, 34 zápasů, skóre i soutěž se plní správně)

## Chybí / Rozpracováno ⚠️
- Hráči (`BasketballPlayers`), soupisky, pořadí (`BasketballStandings`), statistiky zápasů nejsou zobrazené
- Žádný detail týmu ani zápasu
- Filtrování v paměti (dnes 24 týmů / 34 zápasů OK, při růstu přesunout do SQL)
- `SetTab` maže hledání — nečekané při přepínání tam a zpět

## Návrhy na vylepšení 💡
- Detail týmu `/sport/basketball/team/{guid}`: zápasy týmu, bilance, soupiska
- Tab Pořadí, až agent odblokuje Tier 2 (granty na Standings)
- Filtr podle soutěže (dropdown z LeagueSeasons)
- Generická `/sport/{key}` stránka — registr sportů s mapováním sloupců (Name vs NormalizedName, skóre v Matches vs MatchTeams)

## Brainstorming poznámky
- Schéma se mezi sporty liší (AmFootball má skóre v `Matches`, Basketball v `MatchTeams`) → generická stránka potřebuje per-sport konfiguraci nebo introspekci sloupců
- Paměť: Basketball agent měl granty jen na 9/29 tabulek (08-17) — aktuálně AgentAI vidí všech 494, ověřit, jestli se Tier 2 už plní
