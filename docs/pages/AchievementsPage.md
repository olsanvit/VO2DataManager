# AchievementsPage.razor
Route: `/achievements`
Popis: Grid achievementů aplikace a součet bodů.
Platforma: Web

## Hotovo ✅
- `AchievementsGrid` ze SharedServices (filtr kategorií, odemčeno X/Y, body)
- `VO2Achievements.All` — ~100 definic v kategoriích Začátky, Interpreti, Alba, Skladby, Speciální
- Registrace `AchievementService` s definicemi v `Program.cs`

## Chybí / Rozpracováno ⚠️
- **V celém projektu není jediné volání `Unlock()`** — stránka natrvalo ukazuje 0 bodů a vše zamčené
- Většina definic se váže na hudební modul, který neexistuje
- Není per-user perzistence (MAB už má `DbAchievementStore`)
- Některé definice nejdou měřit (datum narození / země interpreta — model tyto sloupce nemá)

## Návrhy na vylepšení 💡
- Buď stránku skrýt, nebo přepsat definice na měřitelné věci: „všichni agenti zelení“, „prošel jsem 10 sportů“, „první export“, „noční návštěva“
- Napojit `Unlock()` v `Home`, `SearchPage`, `Agents`, exportech
- Převzít `DbAchievementStore` a `AchievementCircuitInitializer` z MAB (řeší circuit scope)

## Brainstorming poznámky
- V MAB se 2026-09-11 ukázalo, že SSR layout ≠ circuit scope — interaktivní stránky dostávaly neinicializovaný `AchievementService`; stejné riziko platí tady
- Pro whitelist aplikaci s jedním hlavním uživatelem má gamifikace malou hodnotu — nízká priorita
