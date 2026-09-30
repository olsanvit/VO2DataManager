# LogsPage.razor
Route: `/admin/logs`
Popis: Prohlížeč Serilog záznamů (Warning a výš) z tabulky Logs v AIData.
Platforma: Web

## Hotovo ✅
- **Nová stránka 2026-09-12** — 2 141 záznamů v tabulce `Logs` bylo doteď v aplikaci neviditelných
- Filtr podle úrovně (Warning/Error/Fatal), fulltext v textu a výjimce, rozbalovací stacktrace
- Sloupce se čtou přes `to_jsonb` — Serilog sink má napříč verzemi různé názvy (`message` vs `Message`)
- Jen pro roli Admin

## Chybí / Rozpracováno ⚠️
- Načítá posledních 300 záznamů bez stránkování
- Neukazuje `AuditLog` (8 177 řádků) — ten by si zasloužil vlastní záložku
- Filtrování probíhá v paměti, ne v SQL
- Chybí mazání/archivace starých logů

## Návrhy na vylepšení 💡
- Záložka „Audit" nad `AuditLog` (kdo co změnil)
- Filtr podle časového rozsahu a odkaz z chyby na příslušného agenta
- Počítadlo chyb za 24 h na admin dashboardu

## Brainstorming poznámky
- Logy píše aplikace do stejné DB, kterou zobrazuje — při výpadku DB nebude vidět nic, což je zrovna kdy je potřeba
