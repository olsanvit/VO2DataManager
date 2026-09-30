# Agents.razor
Route: `/agents`
Popis: Přehled všech AI agentů — plán běhu, stav posledního běhu, zpoždění, verze promptu, highlights.
Platforma: Web

## Hotovo ✅
- Raw SQL: `AgentSchedules` LEFT JOIN LATERAL poslední `AgentRunReports` — funguje proti produkční DB (92 agentů, ~730 běhů)
- 6 klikacích stat karet (OK / Chyba / Blokováni / Bez dat / Po termínu / Celkem) = filtry
- Filtr tlačítky, přepínač „Zobrazit deaktivované“, textové hledání (název, typ)
- Barvení řádků podle zpoždění (>0 h žlutě, >48 h červeně), deaktivované poloprůhledně
- Auto-refresh každých 300 s s odpočtem + ruční „Obnovit“
- Řazení v SQL: chyby → blokované → po termínu → OK → bez dat → neaktivní

- **Opraveno 2026-09-12:** klasifikace statusů podle reálných hodnot (`degraded`, `partial`, `success`, `blocked`…) — dřív karta „OK" ukazovala vždy 0, protože se porovnávalo na doslovné `"ok"`
- Karta „Blokováni" nahrazena kartou „Varování"; řazení v SQL sjednoceno se stejnou klasifikací

## Chybí / Rozpracováno ⚠️
- `PromptVersion == "10.0.0"` je pořád natvrdo jako „aktuální" — číst z `PromptVersionPin`
- Badge ukazuje surový status agenta; 13 různých hodnot je pro rychlý přehled hodně
- `PromptVersion == "10.0.0"` natvrdo jako „aktuální“ — governance je 11.x, všichni agenti svítí žlutě
- Timer tiká každou sekundu a volá `StateHasChanged()` → 1 SignalR render/s na klienta jen kvůli číslu odpočtu
- `IntervalHours` se načítá, ale nezobrazuje; je to `string`, nedá se s ním počítat
- Filtry a hledání nejsou v URL — nejde poslat odkaz „všichni chybující“
- `LoadAsync` volá `StateHasChanged()` z timeru mimo `InvokeAsync` (přes `_loading = true`) — potenciální threading problém
- Není export, není trend (kolikrát za sebou selhal)

## Návrhy na vylepšení 💡
- Aktuální verzi promptu číst z `PromptVersionPin`
- Odpočet řešit v JS/CSS (nebo refresh bez viditelného odpočtu), server renderovat jen při reloadu dat
- Sloupec „série“ posledních 5 běhů (✅✅❌❌✅) — jeden `array_agg` v LATERAL
- Deep-link filtry `?filter=Chyba&q=football`
- Seskupení podle `AgentType` (sport data managery, katalogy, monitoring)
- Export CSV pro reporting

## Brainstorming poznámky
- Spolu s `AgentDetail` nejcennější část aplikace — jediná, která má reálná data v objemu
- Propojit s `SportProgress` — u sportovních agentů ukázat, kolik dat skutečně naplnili
- Zvážit ntfy upozornění přímo odsud (tlačítko „pošli mi report“)
