# AgentDetail.razor
Route: `/agents/{AgentName}`
Popis: Detail jednoho agenta — plán, verze promptu, blokátory z posledního běhu, historie 20 běhů.
Platforma: Web

## Hotovo ✅
- Raw SQL nad `AgentSchedules` a `AgentRunReports` (funguje)
- Karty: typ, interval, příští běh (se zpožděním), verze promptu
- Panel blokátorů z posledního běhu: `Blockers`, `Schema_gaps`, `RecommendedNextStep`
- Fallback SQL, když sloupec `Schema_gaps` v DB chybí
- Historie posledních 20 běhů, auto-refresh 120 s, odkaz na ntfy topic

- **Opraveno 2026-09-12:** ntfy odkaz míří na `https://ntfy.vo2info.cz` místo LAN IP; badge používá stejnou klasifikaci statusů jako `/agents`

## Chybí / Rozpracováno ⚠️
- ntfy odkaz natvrdo na LAN `http://192.168.60.221:8225/` — mimo LAN nefunguje; veřejná adresa je `https://ntfy.vo2info.cz/{topic}`
- Data se načtou **dvakrát** při prvním renderu (`OnInitializedAsync` i `OnParametersSetAsync` volají `LoadAsync`)
- `Errors`/`Highlights` jen `text-truncate` s `title` — dlouhé chyby nejdou přečíst
- Historie fixně 20 běhů bez stránkování
- Stejný per-sekundový timer jako `/agents`; `PromptVersion` 10.0.0 natvrdo
- Agent, který v `AgentSchedules` není (překlep v URL), zobrazí jen prázdnou historii bez hlášky

## Návrhy na vylepšení 💡
- Rozklikávací řádek s plným reportem (Summary, Highlights, Errors, Blockers) nebo modal
- Graf úspěšnosti v čase (ApexCharts timeline / bar podle `Status`)
- Diff dvou běhů (co se změnilo v highlights/blockers)
- Odkaz na prompt v AgentsPromptsSkills (aps.vo2info.cz)
- Pro sportovní agenty panel „co agent naplnil“ — počty řádků v tabulkách jeho sportu

## Brainstorming poznámky
- Topic ntfy se odvozuje z názvu agenta (lowercase, `_`/mezera → `-`) — ověřit, že to odpovídá topicům, které agenti skutečně používají
- `OnParametersSetAsync` stačí sám — `OnInitializedAsync` jen nastaví timer
