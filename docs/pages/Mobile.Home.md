# Home.razor
Route: `/ (MAUI)`
Popis: Úvodní obrazovka MAUI aplikace — počty interpretů, alb a skladeb.
Platforma: Mobile (MAUI)

## Hotovo ✅
- 3 kachle s počty přes `IDbContextFactory<AppDbContextAiData>`
- Spinner při načítání

## Chybí / Rozpracováno ⚠️
- Nefunkční — hudební tabulky neexistují
- **Připojuje se přímo k PostgreSQL** s heslem v embedded `appsettings.json` — heslo jde vytáhnout z balíčku
- Žádná autentizace
- Navigace jen tři tlačítka v `MainLayout`

## Návrhy na vylepšení 💡
- Přestavět na klienta nad HTTP API webové aplikace (API zatím neexistuje)
- Mobilní obsah = monitoring agentů (nejužitečnější data na cestách)

## Brainstorming poznámky
- Otázka pro uživatele: má mobilní app vůbec pokračovat? Bez API je to bezpečnostní riziko
