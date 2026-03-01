# Changelog

All notable changes to the Ierahkwa Platform.

## [3.5.1] - 2026-02-25
### Added
- **2 New Platforms** — firewall-cuantico (Post-Quantum Firewall) and red-team (Offensive Security)
  - firewall-cuantico: WAF dashboard, 8 defense modules, PQC cryptography (ML-KEM-1024, ML-DSA-65, SLH-DSA, FrodoKEM, BIKE, Hybrid TLS), live firewall log simulator
  - red-team: 6 active operations, MITRE ATT&CK arsenal, vulnerability findings with severity badges
- **22 API Gateway Route Handlers** — Complete REST API coverage:
  - auth.js: FWID login, register, logout, refresh, profile (custom auth pattern)
  - bdet.js: Balance, transactions, transfer, stake, history (financial endpoints)
  - mail.js: Inbox, send, drafts, delete (email endpoints)
  - 19 CRUD route modules: social, search, video, music, lodging, artisan, commerce, invest, docs, map, voice, jobs, renta, wiki, edu, news, atabey, ai, chain
  - All routes use shared modules: asyncHandler, createLogger, AppError, createAuditLogger
- **7 Redirect Pages** for broken inter-NEXUS links:
  - portal-central, ciberseguridad-soberana, defensa-soberana, vigilancia-fronteriza, zk-identidad, cripto-soberana, emergencias-soberanas
  - Each uses `<meta http-equiv="refresh">` for instant redirect to correct platform
- **NEXUS Commercial Upgrade** — All 10 NEXUS platforms enhanced as sellable packages:
  - Pricing Tiers section: Member / Resident / Citizen plans with feature comparison
  - API Marketplace: Licensed API endpoints with rate limits and method badges
  - SLA Guarantees: Uptime, latency, capacity metrics with visual progress bars
  - Revenue Analytics Dashboard: MRR, API fees, client growth indicators
- **Portal Central Futurehead Migration** — Main entry point upgraded to Futurehead design system
  - Orbitron + Exo 2 typography, neon-green accent, glow effects on all cards
  - Shine sweep animation on platform cards, NEXUS card hover lift

### Changed
- **NEXUS CSS Enhancement** — All 10 NEXUS platforms:
  - Section headings: font-family 'Orbitron' applied to .svc-info h4, .platforms h2, .activity h2, .chart-container h2
  - Service cards: hover translateY(-3px) + glow box-shadow
  - Platform links: hover translateY(-2px) effect
  - Footers: v3.3.0 → v3.5.0
- **Version Alignment** — All shared JS files synchronized to v3.5.0:
  - sw.js: CACHE_NAME, STATIC_CACHE, API_CACHE caches renamed to v3.5.0
  - ierahkwa-api.js: v3.0.0 → v3.5.0
  - ierahkwa.js: v2.8.0 → v3.5.0
- Portal Central: v3.3.0 → v3.5.0, platform count 192 → 194
- AI Fortress JavaScript completed in nexus-escudo (42 AI engines, live dashboard)

### Fixed
- 30+ broken inter-platform links resolved via redirect pages
- API Gateway `MODULE_NOT_FOUND` errors eliminated (22 missing route files created)
- Duplicate `@keyframes pulse` removed from nexus-escudo (now in shared CSS)
- Version drift across shared JS files corrected (were scattered v2.8.0 / v3.0.0 / v3.3.0)

## [3.5.0] - 2026-02-25
### Added
- **Futurehead Design System** — Complete visual overhaul of all 191 platforms
  - ierahkwa.css v3.5.0: Orbitron + Exo 2 typography, neon-green (#00FF41) accent, gradient backgrounds
  - Shine sweep animation on card hover, glow effects, pulse/glow keyframes
  - `.status-dot` utility class with animated pulse indicator
  - Support for both NEXUS (`<nav>`) and inline platform (`<header>`) navigation patterns
  - New `.stats`, `.stat .val/.lbl`, `.section-title`, `.card-icon`, `.logo`, `.logo-icon` classes
  - CSS `@import` for Google Fonts (Orbitron 400-900, Exo 2 300-700)

### Changed
- **178 platforms migrated** from inline CSS to shared ierahkwa.css via `migrate-futurehead.sh`
  - Each platform retains its unique `--accent` color override
  - Inline `<style>` blocks reduced to single `:root{--accent:COLOR}` line
  - Consistent Futurehead visual language across all platforms
- CSS variables updated: backgrounds (#0a0e17 deep blue), borders (#1e3a5f), text (#ffffff), radius (16px)
- Cards now have translateY(-5px) hover lift + green glow box-shadow + shine effect
- Footer upgraded with 2px solid accent border-top + bg2 background
- Dashboard cards (.dash-card) and stats (.stat) now use Orbitron font
- 10 NEXUS platform footers → v3.5.0

## [3.4.0] - 2026-02-25
### Added
- @ierahkwa/shared v2.0.0 — 5 new infrastructure modules:
  - logger.js — Structured JSON logging with request tracing, child loggers, performance timers, redaction
  - error-handler.js — RFC 7807 Problem Details with AppError class, 25+ error codes, async handler wrapper
  - validator.js — Zero-dependency input validation (string, number, uuid, walletAddress, wampumAmount), pre-built schemas
  - resilience.js — Circuit breaker (CLOSED/OPEN/HALF_OPEN), retry with exponential backoff + jitter, timeout, bulkhead, resilient HTTP client
  - audit.js — Immutable audit trail with SHA-256 hash chain, BDET financial transaction logging, UNDRIP/ILO-169 compliant
- Health probes added to 19 K8s containers across 5 service manifests (raices, tesoro, tierra, urbe, voces)
  - readinessProbe: /health/ready with 5s initial delay, 10s period
  - livenessProbe verification on all containers

### Changed
- @ierahkwa/shared package.json: v1.0.0 → v2.0.0, added exports map for all modules

## [3.3.0] - 2026-02-25
### Added
- ierahkwa-ai.js (v1.0.0) — Offline AI engine using ONNX Runtime Web + WebAssembly
- 7 pre-configured ONNX models for on-device inference (sentiment, classification, translation, NER, image, embeddings)
- AI Model Server microservice (.NET 8, port 5050) for model distribution
- ia-soberana platform — interactive offline AI demo showcase
- Service Worker v3.3.0 with AI model caching (IndexedDB + Cache API)
- K8s manifest for ai-model-server with HPA, NetworkPolicy, securityContext
- OpenAPI spec for AI Model Server API (ai-models-api.yml)
- WebGPU support with automatic WASM-SIMD fallback
- 100% on-device inference — zero data transmission (UNDRIP/ILO-169 compliant)
- Enhanced offline modules for 14 priority platforms with IndexedDB sync:
  - Tier 1 (Critical): archivo-linguistico-soberano, healthcare-dashboard, agricultura-soberana, emergencias-soberano, education-dashboard
  - Tier 2 (High Value): bdet-bank (crypto-signed offline transactions), docs-soberanos, mapa-soberano (GeoJSON layers), comercio-soberano, mensajeria-soberana
  - Tier 3 (Moderate): sabiduria-soberana, biblioteca-soberana, noticia-soberana, correo-soberano
- Service Worker enhanced with OFFLINE_DATA_CACHE, priority API routing, background sync tags for 8 platform types
- Each offline module: IndexedDB data stores, offline banner indicator, automatic sync on reconnect, queue-based data submission

### Changed
- Platform count: 191 → 192 (added ia-soberana)
- Microservice count: 83 → 84 (added AIModelServer)
- Service Worker upgraded from v3.2.0 to v3.3.0
- Portal Central updated with new platform count

## [3.2.1] - 2026-02-25
### Fixed
- 37 Dockerfiles with empty COPY directives corrected with proper .csproj references
- 44 Dockerfiles with cosmetic WORKDIR issue cleaned up
- 3 Dockerfiles with ._ prefix in ENTRYPOINT fixed
- WAMPUM docker-compose password externalized to environment variable
- 191/191 HTML platforms now have aria-label (100% accessibility)

### Added
- fix-aria-labels.sh automation script
- fix-dockerfiles.sh automation script
- .env.example for WAMPUM blockchain configuration

## [3.2.0] - 2026-02-25
### Added
- 10 OpenAPI specifications (7,001 lines YAML, 53 services)
- 11 Grafana dashboards with Prometheus + Loki provisioning
- PWA support: manifest.json, service worker, offline-first
- WAMPUM Blockchain Engine (Go 1.22, Chain Id 574, PoS)
- MameyNode runtime (Fastify 4.26, cluster mode, HTTP/2)
- Kubernetes manifests for all 53 services with HPA
- inject-pwa.sh script for PWA deployment

## [3.1.0] - 2026-02-24
### Added
- 10 NEXUS mega-portals expanded with full platform listings
- Portal Central (index.html) with interactive SVG NEXUS map
- Investor audit presentation

## [3.0.0] - 2026-02-24
### Added
- ierahkwa-api.js (v3.0.0) - API client with tier-based auth
- 83 .NET 8 microservices with Dockerfiles
- 21 Node.js backend services

## [2.8.0] - 2026-02-24
### Added
- ierahkwa.css design system (v2.8.0, 307 lines)
- ierahkwa.js interactive features (v2.8.0, 188 lines)
- 189 sovereign HTML platforms
- Dark/light theme toggle
- Counter animations
- Search functionality
