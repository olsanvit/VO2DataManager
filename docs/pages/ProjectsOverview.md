# ProjectsOverview.razor
Route: `/admin/projects`
Popis: Karty všech homelab projektů s odkazy na prod, dev a GitHub a s poznámkami k nálezům.
Platforma: Web

## Hotovo ✅
- 12 karet (ikona, popis, stav, prod/dev/GitHub odkazy, poznámky s CRITICAL/PENDING nálezy)
- Responzivní grid

- **Opraveno 2026-09-13:** všechny URL přepsány na domény z `~/deploy-to-qnap.sh` ověřené v DNS — dřívější odkazy (`vo2datamanager.*`, `mab.*`, `simulategames.*`, `te.*`, `vin.*` a všechny `*-dev.*`) vůbec neexistovaly; dev odkaz zůstal jen u ClubManageru, ostatní dev domény v DNS nejsou

## Chybí / Rozpracováno ⚠️
- Data jsou natvrdo v C# poli → tiše zastarávají (nálezy z července)
- Žádný živý stav — „Aktivní“ je text, ne výsledek pingu
- Chybí VO2Info, Metin2Bausia, SimulateCar a TransitTycoon, které deploy skript i paměť znají; chybí vo2info, TopEleven podrobnosti
- Stav CI buildů, poslední commit, otevřené nálezy — nic z toho tu není

## Návrhy na vylepšení 💡
- Živý health: HTTP status prod/dev URL (data z `url-monitor` rutiny nebo `status.vo2info.cz`)
- Poslední CI běh přes GitHub API
- Definice projektů do JSON / DB tabulky místo kódu
- Počet otevřených nálezů z auditu per projekt

## Brainstorming poznámky
- Mohlo by nahradit ruční „Seznam aktivních projektů“ v memory — jediný zdroj pravdy o projektech
- Pozor na zobrazování CRITICAL nálezů (hardcoded hesla) — i v admin sekci je to citlivá informace
