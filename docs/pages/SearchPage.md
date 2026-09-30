# SearchPage.razor
Route: `/search`
Popis: Globální fulltext přes interprety, alba, skladby, týmy amerického fotbalu a basketbalu s přepínáním kategorií.
Platforma: Web

## Hotovo ✅
- Debounce 400 ms přes `System.Timers.Timer`, `IDisposable` (opraven timer leak)
- `EF.Functions.ILike` — case-insensitive hledání
- Chipy kategorií (5), výsledky seskupené po kategoriích, `Take(20)` na dotaz, zobrazeno 10
- Minimum 2 znaky

- **Přepsáno 2026-09-12 na raw SQL** — hledá v 6 kategoriích (agenti, basketbal týmy/ligy/hráči, am. fotbal týmy, fotbal týmy) přes `ILIKE` s parametrem
- Každá kategorie ve vlastním try/catch → chybějící tabulka shodí jen svou sekci a vypíše se do upozornění
- Agenti mají proklik na `/agents/{name}`; ověřeno proti AIData (Manchester City/United, Football Data Manager)
- Debounce timer opraven na `AutoReset = false`

## Chybí / Rozpracováno ⚠️
- Hudební kategorie (interpreti/alba/skladby) vypuštěny — tabulky neexistují; vrátit, až vzniknou
- Pořád jen 6 z ~40 sportů a bez katalogů (`world_entity_catalog`, `popculture_catalog`)
- Výsledky bez stránkování (`LIMIT 20`), bez zvýraznění shody a bez `?q=` v URL
- Kategorie se dotazují sériově — při rozšíření na desítky tabulek to bude pomalé
- Týmy nemají žádný proklik
- Kategorie jsou hardcoded pole — v AIData je ~40 sportů + katalogy
- Chybí zvýraznění shody, „zobrazit další“, stav hledání v URL (`?q=`)
- Timer callback volá `InvokeAsync(SearchAsync)` — při rychlém psaní se může spustit víc dotazů souběžně nad stejnými poli (race)

## Návrhy na vylepšení 💡
- Hledat přes centralizované `SearchText` sloupce (má je každá agentní tabulka) — jeden `UNION ALL` přes whitelist tabulek
- Alternativně hledat v `world_entity_catalog` / `AgentEntities`
- Kategorie generovat dynamicky podle toho, které tabulky mají data
- Query string `?q=…&cat=…` pro sdílení a návrat zpět
- Klávesová zkratka `/` nebo `Ctrl+K` z libovolné stránky

## Brainstorming poznámky
- Kandidát na hlavní vstupní bod aplikace — v DB se 494 tabulkami je hledání užitečnější než menu
- Při rozšíření na desítky tabulek nutný `CancellationToken` na předchozí dotaz a limit souběhu
- Achievementy `VO2_FIRST_SEARCH`, `VO2_SEARCH_ALL` se nikde neodemykají
