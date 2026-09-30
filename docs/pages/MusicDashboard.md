# MusicDashboard.razor
Route: `/music-dashboard`
Popis: Hudební přehled — žánrový donut, top 10 interpretů podle počtu alb, posledních 20 alb, tabulka žánrů.
Platforma: Web

## Hotovo ✅
- Paralelní načítání přes `Task.WhenAll` (counts, žánry, top interpreti, poslední alba)
- Donut top 15 žánrů, tabulka všech žánrů, top 10 interpretů s počtem alb, posledních 20 alb
- Odkaz v NavMenu („Hudba — přehled“)

- **Opraveno 2026-09-12:** místo pádu se ukáže prázdný stav s vysvětlením a odkazem na `/agents`
- Paralelní dotazy mají každý vlastní DbContext (`QueryAsync`) — EF Core souběh nad jednou instancí nepodporuje

## Chybí / Rozpracováno ⚠️
- Obsah stránky zůstává nefunkční, dokud hudební tabulky nevzniknou — teď jen slušně mlčí
- Odkazy na detaily pořád míří na `GuidId` proti routám `{Id:int}`
- Odkazy `/artists/@GuidId` a `/albums/@GuidId` neodpovídají routám `{Id:int}` → 404 i kdyby data byla
- Donut natvrdo `Mode.Dark`
- Je v NavMenu, přestože modul je jinak skrytý

## Návrhy na vylepšení 💡
- Do vzniku tabulek skrýt z NavMenu nebo ukázat empty-state místo pádu
- Pro paralelismus vytvořit samostatný DbContext pro každý dotaz (`DbFactory.CreateDbContextAsync()` per task)
- Sjednotit odkazy na GuidId (nebo přidat druhou routu u detailů)
- Graf alb podle dekád / roku vydání, heatmapa „interpret × dekáda“

## Brainstorming poznámky
- Stránka duplikuje část `Home` — až bude hudba existovat, `Home` = rozcestník AIData, `MusicDashboard` = modul
- Top interpreti se dohledávají druhým dotazem po načtení ID — šlo by jedním JOINem
