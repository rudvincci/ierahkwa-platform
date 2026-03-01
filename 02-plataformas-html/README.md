# Plataformas HTML — Ierahkwa Ne Kanienke

> 190 sovereign platform UIs + Portal Central, organized into 15 NEXUS mega-portals.
> Digital sovereignty for 1B+ people across the Americas across 35+ countries and 574 tribal nations.
> v3.9.0 — 26 Feb 2026

## Architecture

```
02-plataformas-html/
├── index.html              ← Portal Central (hub connecting all 15 NEXUS)
├── shared/
│   ├── ierahkwa.css        ← Shared design system (Futurehead Edition)
│   ├── ierahkwa.js         ← Search, filters, counters, theme toggle
│   ├── ierahkwa-ai.js      ← Offline AI engine (ONNX Runtime Web)
│   ├── ierahkwa-api.js     ← Shared API client + auth
│   ├── manifest.json       ← PWA manifest (15 NEXUS shortcuts)
│   ├── sw.js               ← Service Worker (offline-first, 5 cache strategies)
│   └── fonts/              ← Self-hosted Orbitron + Exo 2 (zero CDN)
├── icons/                   ← 8 app icons + 15 NEXUS icons (SVG)
├── screenshots/             ← PWA install screenshots (SVG)
├── admin-dashboard/         ← Admin panel (61KB)
├── nexus-orbital/           ← 🛰️ Telecomunicaciones & Satelites (20)
├── nexus-escudo/            ← 🔒 Defensa & Ciberseguridad (18)
├── nexus-cerebro/           ← 🧠 AI & Quantum Computing (15)
├── nexus-tesoro/            ← 🏦 Finanzas & WAMPUM CBDC (20)
├── nexus-voces/             ← 📱 Social Media & Comunicaciones (10)
├── nexus-consejo/           ← 🏛️ Gobierno & Justicia (17)
├── nexus-tierra/            ← 🌍 Naturaleza & Recursos (18)
├── nexus-forja/             ← 💻 Desarrollo Tecnologico (20)
├── nexus-urbe/              ← 🏙️ Ciudad Inteligente (8)
├── nexus-raices/            ← 🎭 Cultura & Economia (6)
├── nexus-salud/             ← 🏥 Salud & Bienestar (7)
├── nexus-academia/          ← 🎓 Educacion Superior (5)
├── nexus-escolar/           ← 📚 Educacion K-12 (8)
├── nexus-entretenimiento/   ← 🎮 Entretenimiento & Deportes (8)
├── nexus-amparo/            ← 🛡️ Proteccion Social (10)
└── {190 individual platforms}/
    └── index.html           ← Self-contained HTML platform
```

## 15 NEXUS Mega-Portals

| NEXUS | Color | Domain | Platforms |
|-------|-------|--------|-----------|
| 🛰️ Orbital | `#00bcd4` | Telecomunicaciones & Satelites | 20 |
| 🔒 Escudo | `#f44336` | Defensa & Ciberseguridad | 18 |
| 🧠 Cerebro | `#7c4dff` | AI, Quantum & Data | 15 |
| 🏦 Tesoro | `#ffd600` | Finanzas & Blockchain | 20 |
| 📱 Voces | `#e040fb` | Social Media & Lenguas | 10 |
| 🏛️ Consejo | `#1565c0` | Gobierno & Justicia | 17 |
| 🌍 Tierra | `#43a047` | Naturaleza & Recursos | 18 |
| 💻 Forja | `#00e676` | Desarrollo Tech | 20 |
| 🏙️ Urbe | `#ff9100` | Ciudad & Servicios | 8 |
| 🎭 Raices | `#00FF41` | Cultura & Economia | 6 |
| 🏥 Salud | `#FF5722` | Salud & Bienestar | 7 |
| 🎓 Academia | `#9C27B0` | Educacion Superior | 5 |
| 📚 Escolar | `#1E88E5` | Educacion K-12 | 8 |
| 🎮 Entretenimiento | `#E91E63` | Entretenimiento & Deportes | 8 |
| 🛡️ Amparo | `#607D8B` | Proteccion Social | 10 |

## PWA & SEO

Every platform includes:

- `<meta name="description">` — Unique per platform
- `<meta name="theme-color">` — Platform accent color
- `<link rel="manifest">` — PWA manifest
- `<link rel="icon">` + `<link rel="apple-touch-icon">` — SVG icons
- `<link rel="canonical">` — Canonical URL
- Open Graph tags (`og:title`, `og:description`, `og:image`, `og:url`)
- Twitter Card tags (`twitter:card`, `twitter:title`, `twitter:description`)
- Service Worker registration — Offline-first caching

## Design System

### CSS Variables (Futurehead Edition v3.5.0)

| Variable | Value | Usage |
|----------|-------|-------|
| `--bg` | `#0a0e17` | Page background |
| `--bg2` | `#0d1a2d` | Cards, nav, footer |
| `--bg3` | `#142238` | Badges, inputs |
| `--bg4` | `#1e3a5f` | Borders, dividers |
| `--gold` | `#FFD700` | Brand gold |
| `--accent` | `#00FF41` | Neon green (override per-platform) |
| `--txt` | `#ffffff` | Primary text |
| `--txt2` | `#888888` | Secondary text |
| `--brd` | `#1e3a5f` | Borders |
| `--r` | `16px` | Border radius |

### Tag Colors

| Tag | Class | Color |
|-----|-------|-------|
| AI | `.tag.ai` | `#7c4dff` |
| WAMPUM | `.tag.wam` | `#FFD700` |
| SAT | `.tag.sat` | `#00bcd4` |
| BLOCKCHAIN | `.tag.bc` | `#4a9eff` |
| QUANTUM | `.tag.qt` | `#e84040` |

### Components

- `.skip` / `.skip-nav` — Skip navigation link
- `nav` + `.brand` + `.btn` — Sticky navigation with backdrop blur
- `.hero` + `.badge` — Hero section with badge and radial glow
- `.counters` + `.counter` — Stat counters (animated)
- `.stats` + `.stat` — Dashboard stat cards
- `.dash` + `.dash-card` — Dashboard mini-cards
- `.section` + `.sub` — Content sections
- `.grid` + `.card` — Feature card grids with 3D hover
- `.tags` + `.tag` — Technology tag badges
- `.connections` + `.conn-grid` + `.conn` — NEXUS interconnection cards

## Accessibility (GAAD)

Every platform includes:

1. Skip navigation link (`<a href="#main" class="skip-nav">`)
2. `<main id="main">` landmark
3. `aria-hidden="true"` on decorative emojis
4. `aria-label` on sections and interactive elements
5. `prefers-reduced-motion: reduce` media query
6. `:focus-visible` outline styling
7. Semantic HTML (`<article>`, `<section>`, `<nav>`)
8. Responsive design (mobile-first)
9. `lang="es"` on all HTML documents

## Offline AI Engine

`shared/ierahkwa-ai.js` provides on-device ML inference:

- 7 ONNX models (sentiment, classification, translation, NER, embeddings, image)
- WebGPU acceleration with WASM-SIMD fallback
- IndexedDB model storage
- Zero data leaves the device — 100% sovereign AI
- Compatible with UNDRIP/ILO-169 data sovereignty

## Service Worker

`shared/sw.js` implements 5 caching strategies:

1. **Cache-first** — Static assets (CSS, JS, fonts, icons)
2. **Network-first** — API requests (5s timeout)
3. **Stale-while-revalidate** — HTML pages
4. **AI model cache** — ONNX models (immutable by version)
5. **Offline data cache** — 14 priority platforms with IndexedDB sync

## Template Patterns

### Pattern A: NEXUS Mega-Portal

- Shared CSS + inline accent override
- Services grid, budget chart, platform list
- Activity log, API marketplace
- Interconexiones NEXUS (cross-links to all 14 other NEXUS)
- ~15-31 KB per file

### Pattern B: Individual Platform

- Shared CSS + inline accent override
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
7. Link `shared/ierahkwa.css` and `shared/ierahkwa.js`
8. Run `python3 shared/inject-meta.py` to add SEO + PWA tags

## Shared Resources

### `shared/ierahkwa.css` (~13KB)
Complete design system with all variables, components, responsive breakpoints, light theme, GAAD accessibility, and self-hosted fonts (Orbitron + Exo 2).

### `shared/ierahkwa.js` (~6KB)
Vanilla JS with progressive enhancement:
- Search and filter (Portal Central)
- Counter animation (count-up on scroll)
- Smooth scroll for anchor links
- Optional dark/light theme toggle
- Navigation active states
- Card hover interactions (3D tilt)

### `shared/ierahkwa-ai.js` (~28KB)
On-device AI inference engine:
- 7 ONNX models for NLP, vision, translation
- WebGPU + WASM-SIMD support
- IndexedDB model caching
- Zero data transmission

### `shared/ierahkwa-api.js` (~7KB)
Shared API client:
- JWT RS256 authentication
- Multi-tenant support (X-Tenant-Id)
- Tier-based feature flags
- Auto-refresh tokens

## Stats

- **190** platform HTML files + Portal Central
- **15** NEXUS mega-portals
- **212** total directories with content
- **7** redirect alias stubs
- **~3.5 MB** total HTML content
- **Zero** external dependencies (fonts self-hosted)
- **214** files with full SEO + PWA tags
- **100%** `lang="es"` compliance
