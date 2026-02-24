# Plataformas HTML — Ierahkwa Ne Kanienke

> 189 sovereign platform UIs + Portal Central, organized into 10 NEXUS mega-portals.
> Digital sovereignty for 72M indigenous people across 19 nations and 574 tribal nations.

## Architecture

```
02-plataformas-html/
├── index.html              ← Portal Central (hub connecting all NEXUS)
├── shared/
│   ├── ierahkwa.css        ← Shared design system
│   └── ierahkwa.js         ← Search, filters, counters, theme toggle
├── nexus-orbital/           ← 🛰️ Telecomunicaciones & Satelites
├── nexus-escudo/            ← 🔒 Defensa & Ciberseguridad
├── nexus-cerebro/           ← ⚛️ AI & Quantum Computing
├── nexus-tesoro/            ← 🏦 Finanzas & WAMPUM CBDC
├── nexus-voces/             ← 📱 Social Media & Comunicaciones
├── nexus-consejo/           ← 🏛️ Gobierno & Justicia
├── nexus-tierra/            ← 🌍 Naturaleza & Recursos
├── nexus-forja/             ← 💻 Desarrollo Tecnologico
├── nexus-urbe/              ← 🏙️ Ciudad Inteligente
├── nexus-raices/            ← 🎭 Cultura & Economia
└── {179 individual platforms}/
    └── index.html           ← Self-contained HTML platform
```

## NEXUS Mega-Portals

| NEXUS | Color | Domain | Platforms |
|-------|-------|--------|-----------|
| 🛰️ Orbital | `#00bcd4` | Telecomunicaciones & Satelites | 17 |
| 🔒 Escudo | `#f44336` | Defensa & Ciberseguridad | 12 |
| ⚛️ Cerebro | `#7c4dff` | AI, Quantum & Data | 15 |
| 🏦 Tesoro | `#ffd600` | Finanzas & Blockchain | 14 |
| 📱 Voces | `#e040fb` | Social Media & Lenguas | 10 |
| 🏛️ Consejo | `#1565c0` | Gobierno & Justicia | 16 |
| 🌍 Tierra | `#43a047` | Naturaleza & Recursos | 19 |
| 💻 Forja | `#00e676` | Desarrollo Tech | 10 |
| 🏙️ Urbe | `#ff9100` | Ciudad & Servicios | 13 |
| 🎭 Raices | `#d4a853` | Cultura & Economia | 13 |

## Design System

### CSS Variables

| Variable | Value | Usage |
|----------|-------|-------|
| `--bg` | `#09090d` | Page background |
| `--bg2` | `#111116` | Cards, nav, footer |
| `--bg3` | `#1a1a20` | Badges, inputs |
| `--bg4` | `#232330` | Borders, dividers |
| `--gold` | `#d4a853` | Brand accent |
| `--txt` | `#e8e4df` | Primary text |
| `--txt2` | `#8a8694` | Secondary text |
| `--brd` | `#2a2a36` | Borders |
| `--r` | `10px` | Border radius |

### Tag Colors

| Tag | Class | Color |
|-----|-------|-------|
| AI | `.tag.ai` | `#7c4dff` |
| WAMPUM | `.tag.wam` | `#d4a853` |
| SAT | `.tag.sat` | `#00bcd4` |
| BLOCKCHAIN | `.tag.bc` | `#4a9eff` |
| QUANTUM | `.tag.qt` | `#e84040` |

### Components

- `.skip` / `.skip-nav` — Skip navigation link
- `nav` + `.brand` + `.btn` — Sticky navigation
- `.hero` + `.badge` — Hero section with badge
- `.counters` + `.counter` — Stat counters
- `.dash` + `.dash-card` — Dashboard mini-cards
- `.section` + `.sub` — Content sections
- `.grid` + `.card` — Feature card grids
- `.tags` + `.tag` — Technology tag badges
- `.connections` + `.conn-grid` + `.conn` — NEXUS interconnection cards

## Accessibility (GAAD)

Every platform includes:

1. Skip navigation link (`<a href="#main" class="skip">`)
2. `<main id="main">` landmark
3. `aria-hidden="true"` on decorative emojis
4. `aria-label` on sections and interactive elements
5. `prefers-reduced-motion: reduce` media query
6. `:focus-visible` outline styling
7. Semantic HTML (`<article>`, `<section>`, `<nav>`)
8. Responsive design (mobile-first)

## Template Patterns

### Pattern A: NEXUS Mega-Portal (~80-100 lines)

- Minified single-line CSS in `<style>`
- Multiple content sections with sub-platform cards
- Tags: AI, WAMPUM, SAT, BLOCKCHAIN, QUANTUM
- Interconnection section linking to other NEXUS
- ~9-12 KB per file

### Pattern B: Individual Platform (~100-160 lines)

- Multi-line formatted CSS in `<style>`
- Hero + badge + stats (4 items)
- 10 feature cards in `.grid`
- ~6-10 KB per file

## How to Add a New Platform

1. Create directory: `02-plataformas-html/{name}-soberan{o|a}/`
2. Create `index.html` following Pattern B template
3. Set `:root { --accent: {color} }` for the platform accent
4. Include GAAD: skip-nav, aria-hidden, reduced-motion, focus-visible
5. Add 10 feature cards in the `.grid`
6. Add platform card to Portal Central `index.html`
7. Optional: Link `shared/ierahkwa.css` and `shared/ierahkwa.js`

## Shared Resources

### `shared/ierahkwa.css` (~7KB)
Complete design system with all variables, components, responsive breakpoints, light theme, and GAAD accessibility. For new platforms, use `<link rel="stylesheet" href="../shared/ierahkwa.css">` instead of inline styles.

### `shared/ierahkwa.js` (~5KB)
Vanilla JS with progressive enhancement:
- Search and filter (Portal Central)
- Counter animation (count-up on scroll)
- Smooth scroll for anchor links
- Optional dark/light theme toggle
- Navigation active states
- Card hover interactions

## Stats

- **189** platform HTML files + Portal Central
- **10** NEXUS mega-portals
- **179** individual platforms
- **~2.5 MB** total HTML content
- **Zero** external dependencies
