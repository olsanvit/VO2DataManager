# AmFootball.razor
Route: `/amfootball`
Popis: Americký fotbal — týmy, ligy a zápasy z agentních tabulek `AmFootball*`.
Platforma: Web

## Hotovo ✅
- **2026-09-11 přepsáno na raw SQL** (stejný vzor jako `Agents.razor`) — dříve stránka vždy padala, protože EF entity `AmericanFootball*` míří na neexistující tabulky
- Taby Týmy / Ligy / Zápasy s počty, hledání u týmů a zápasů
- Zápasy: datum, domácí/hosté, skóre, liga + sezóna, status, zvýraznění vítěze (`WinnerTeamName`)
- Odkaz na zdroj (`SourceName` + `SourceUrl`), datum aktualizace
- Chyba načtení se ukáže v panelu místo pádu stránky

- **Ověřeno 2026-09-12:** build OK, SQL dotazy prověřeny proti AIData (6 týmů, 1 liga, 6 zápasů vč. historického APFA 1920)
- Zápasová tabulka `AmFootballMatches` má správně formátovaná jména („Edmonton Elks“), zatímco `AmFootballTeams` jen `NormalizedName` („edmonton elks“) — nekonzistence v datech agenta
- `WinnerTeamName` je u novějších zápasů `null`, zvýraznění vítěze se tedy většinou neuplatní

## Chybí / Rozpracováno ⚠️
- `AmFootballTeams`/`Leagues` nemají sloupec `Name` — zobrazuje se `NormalizedName` přes `ToTitleCase` („Bc Lions“ místo „BC Lions“)
- Tab Sezóny odstraněn — tabulka `AmFootballSeasons` neexistuje
- Hráči (`AmFootballPlayers`) a tabulky (`AmFootballStandings`) nejsou zobrazené
- Data jsou minimální: 6 týmů CFL, 1 liga, 6 zápasů (agenti je přesunuli z Football* 2026-09-01)

## Návrhy na vylepšení 💡
- Nahradit generickou stránkou `/sport/{key}` (viz Basketball) — tahle a Basketball jsou z 80 % totožné
- Požádat Am. football data managera, aby plnil `Name` (datová mezera ve schématu)
- Tab Pořadí z `AmFootballStandings`, až budou data

## Brainstorming poznámky
- EF entity `AmericanFootball*` v SharedServices jsou mrtvé — kandidát na odstranění z `AppDbContextAiData` (dopad na všechny projekty se submodulem)
- Pozor: `AdminDashboard` a `SearchPage` pořád používají `AmericanFootball*` přes EF
