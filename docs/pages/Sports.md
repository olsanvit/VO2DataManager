# Sports.razor
Route: `/sports`
Popis: Přehled všech sportů v AIData s počty týmů, lig, hráčů a zápasů.
Platforma: Web

## Hotovo ✅
- **Nová stránka 2026-09-12.** Seznam sportů se odvozuje z názvů tabulek `<Sport>Teams` v `pg_stat_user_tables` — nový sport se objeví sám, bez zásahu do kódu
- Vazební tabulky `<Sport>MatchTeams` jsou z detekce vyloučené (jinak vznikal fiktivní sport „BasketballMatch")
- Počty týmů/lig/hráčů/zápasů, přepínač „jen sporty s daty", hledání, proklik na `/sport/{key}`
- CamelCase názvy se zobrazují čitelně („AmFootball" → „Am Football")

## Chybí / Rozpracováno ⚠️
- Počty jsou odhady z `pg_stat_user_tables`, ne přesné `count(*)` — po importu mohou být nepřesné, dokud neproběhne ANALYZE
- Nezobrazuje `SportProgress` (273 řádků se skóre pokrytí) — ten by dal lepší obrázek než holé počty
- Chybí datum posledního zápisu agenta pro daný sport

## Návrhy na vylepšení 💡
- Sloupec „poslední aktualizace" z `max(UpdatedAt)` napříč tabulkami sportu
- Napojení na `/agents` — u sportu ukázat, který data manager ho plní a v jakém je stavu
- Barevný indikátor pokrytí (Tier 1 výsledky / Tier 2 statistiky / Tier 3 formy)

## Brainstorming poznámky
- Tohle je stránka, která poprvé ukazuje skutečný rozsah AIData — ~40 sportů místo dvou napevno naprogramovaných
- Většina sportů má jednotky řádků; hodnota je v tom vidět, kde agenti stojí
