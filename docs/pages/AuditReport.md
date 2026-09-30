# AuditReport.razor
Route: `/admin/audit`
Popis: Zobrazení statického HTML auditu všech projektů v iframe.
Platforma: Web

## Hotovo ✅
- `[Authorize(Roles = "Admin")]`, iframe na `/audit-report.html` přes celou výšku

## Chybí / Rozpracováno ⚠️
- `wwwroot/audit-report.html` (103 kB) je z **2026-07-11** — nikdo ho negeneruje, obsah je 2 měsíce starý
- Chybí datum vygenerování v UI
- **Soubor je podle kódu veřejně dostupný bez přihlášení** — leží ve `wwwroot`, `MapStaticAssets()` ho servíruje bez autorizace; `[Authorize]` chrání jen Razor stránku s iframem. Obsahuje CRITICAL nálezy všech projektů (hardcoded hesla, SQL injection). Živě neověřeno — QNAP 2026-09-11 neodpovídal

## Návrhy na vylepšení 💡
- Generovat report z `vo2-ai-agent-governance` při deployi, nebo nahradit živou stránkou nad `AuditLog` (7 286 řádků) a `AgentRunReports`
- Zobrazit „vygenerováno: datum“ a tlačítko „přegenerovat“
- Přesunout HTML mimo `wwwroot`, servírovat přes autorizovaný endpoint

## Brainstorming poznámky
- Existuje i Google Sheets audit (Razor Pages + Services & Methods) — dva zdroje pravdy o auditu
