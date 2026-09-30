# AdminDashboard.razor
Route: `/admin`
Popis: Admin přehled — počty uživatelů a dat, seznam uživatelů, top interpreti.
Platforma: Web

## Hotovo ✅
- `[Authorize(Roles = "Admin")]`, async načítání (opraven blokující `ToList()`)
- Kachle: uživatelé, interpreti, alba, skladby, Am. football ligy, baseball ligy
- Tabulka uživatelů s rolí

- **Přepsáno 2026-09-12** — místo neexistujících hudebních tabulek ukazuje reálný stav AIData: počet tabulek (540), z toho s daty (190), agenti OK / s varováním / s problémem, uživatelé s rolí a whitelistem, 15 nejobsazenějších tabulek
- Klasifikace agentů podle reálných statusů, ne podle doslovného `"ok"`
- `DISTINCT ON` místo korelovaného `max()` — původní dotaz timeoutoval přes 30 s
- Chyba načtení se vypíše jako upozornění, stránka nepadá

## Chybí / Rozpracováno ⚠️
- Uživatelé zůstávají read-only — whitelist se pořád nedá spravovat z UI
- Odhady řádků z `pg_stat_user_tables` mohou být po velkém importu nepřesné, dokud neproběhne ANALYZE
- Uživatelé jsou jen read-only — **nejde spravovat whitelist**, přestože je VO2DataManager whitelist aplikace
- „Top 20 interpretů dle abecedy“ nemá informační hodnotu
- Panel „Přehled AI dat“ duplikuje kachle nahoře

## Návrhy na vylepšení 💡
- Zaroutovat `UserAccessPage` ze SharedServices (`/admin/users`) — přidání na whitelist, role, reset hesla
- Kachle přes `pg_stat_user_tables` (odhad řádků, nespadne na chybějící tabulce)
- Panel „Zdraví dat“: tabulky se 0 řádky, poslední zápis agentů, velikost `Logs`/`AuditLog`
- Odkazy na Import stránky a na výstup `AiDataSyncService`

## Brainstorming poznámky
- `AspNetUsers` v AIData měl podle statistik 0 řádků — ověřit, kde Identity tabulky reálně žijí (prod `vo2data_usr` vs dev `roundnet`)
