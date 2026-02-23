# 📊 REPORTE COMPLETO — PLATAFORMA IERAHKWA

**Fecha:** 2026-01-23  
**Base:** `http://localhost:8545`  
**Raíz:** `RuddieSolution/`

---

## Estado de servicios

**Todas las plataformas se sirven correctamente.** Las 86 páginas HTML de `platform/` son accesibles vía `/platform/<archivo>.html`; las rutas cortas (`/bdet-bank`, `/forex`, `/admin`, `/gaming`, etc.) están definidas en el servidor y entregan el HTML correspondiente. La configuración (`config.json`), el hub (`index.html`) y los enlaces editables (`platform/data/platform-links.json`) apuntan a destinos válidos.  
Detalle de la verificación: **ESTADO-PLATAFORMAS-SERVICIOS.md**.

---

## 1. RESUMEN EJECUTIVO

| Concepto | Valor |
|----------|--------|
| **Servidor principal** | Mamey Node — `node/server.js` (puerto **8545**) |
| **Plataformas HTML** | **86** en `platform/*.html` |
| **Rutas cortas (redirects)** | **100+** (ej. /bdet-bank, /forex, /gaming, /departments) |
| **APIs REST** | Node: `/api/config`, `/api/v1/*`, `/health`, `/rpc`, AI-Hub, Backup, Tokens, Bridge, Voting, Gamification, Notifications, Membership, etc. |
| **Config** | `platform/config.json` — 103 IGT Tokens, 15 headerNav, 11 quickActions, 12 departments, **51 services** |
| **Admin** | **16 pestañas** (Platform, Header Config, All Platforms, Monitor, Backup, Settings, Departments, Services, Header Nav, Quick Actions, Sections, Tokens, Health, Theme, **Links y Botones**, Export/Import) |
| **Links editables** | `platform/data/platform-links.json` + Admin → 🔗 LINKS Y BOTONES (version-badges, dashboards, headerNav, quickActions, services) |

---

## 2. ESTRUCTURA RuddieSolution/

```
RuddieSolution/
├── INDICE.md                    ← Índice y arranque
├── PLATAFORMAS-8545.md          ← Todas las URLs
├── REPORTE-LINKS-Y-BOTONES.md   ← Inventario links/botones
├── REPORTE-COMPLETO.md          ← Este reporte
├── commerce-business-dashboard.html
├── platform-services.json
│
├── node/                        ← Mamey Node :8545 (server.js)
├── platform/                    ← 86 HTML + config + data
│   ├── index.html
│   ├── admin.html
│   ├── config.json
│   ├── data/
│   │   └── platform-links.json
│   └── *.html (84 más)
│
├── scripts/                     ← up.sh, start.sh, abre-plataformas.sh, start-full-stack.sh
├── IerahkwaBanking.NET10/       ← API Banking .NET :5000 (opcional)
├── config/                      ← services-ports.json
├── data/                        ← ai-hub, atabey, collected-data, world-intelligence
├── backup-system/               ← auto-backup, backups/
├── deploy/                      ← docker, fly, railway, render
├── monitoring/                  ← prometheus, alerts
├── nginx/
├── servers/                     ← bdet, central-bank, tradex, siis, ai-hub, etc.
├── services/                    ← Go, Python, Rust (8590, 8591, 8592)
├── database/                    ← init, mongo-init
└── LEEME, README.md
```

---

## 3. SERVIDOR NODE (RuddieSolution/node/server.js)

| Parámetro | Valor |
|-----------|--------|
| **Puerto** | `process.env.PORT` o **8545** |
| **ROOT** | `path.join(__dirname, '..', '..')` (raíz del repo, dos niveles arriba de `node/`) |
| **Platform static** | `app.use('/platform', express.static(path.join(__dirname, '..', 'platform')))` |

### Rutas principales

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/` | Dashboard Mamey Node |
| GET | `/index.html` | Dashboard |
| GET | `/platform` | Redirige a /platform/ (index) |
| GET | `/health` | Health check JSON |
| GET | `/ready`, `/live` | Liveness/readiness |
| GET | `/api/config` | config.json (platform) |
| POST | `/api/config` | Guardar config |
| POST | `/rpc` | RPC blockchain |
| GET | `/mega-dashboard.html`, `/live-connect.html` | Dashboards |

### APIs /api/v1

- **Node:** `/api/v1/node`, `/api/v1/stats`, `/api/v1/tokens`, `/api/v1/blocks`, `/api/v1/transactions`, `/api/v1/accounts/:address`
- **DAO:** `/api/v1/dao/proposals`, votes
- **AI-Hub:** `/api/ai-hub` (router)
- **Backup:** `/api/v1/backup/list`, create, restore, download, export, toggle, config, stats
- **Tokens:** `/api/v1/tokens/create`, `/api/v1/tokens/custom`
- **Bridge:** `/api/v1/bridge/chains`, tokens, deposit, withdraw, status, history
- **Analytics:** `/api/v1/analytics/pageview`, event, summary, realtime
- **Voting:** `/api/v1/voting/proposals`, vote
- **Gamification:** `/api/v1/gamification/profile`, daily, achievement, leaderboard, achievements
- **Notifications:** subscribe, unsubscribe, preferences, send, status
- **Membership:** `/api/v1/membership/register`, profile
- **Mamey Futures:** `/api/v1/mamey` (módulo)

### Static / montajes

- `/tokens` → ROOT/tokens  
- `/platform` → `__dirname/../platform`  
- `/AssetTracker`, `/AuditTrail`, `/BudgetControl`, `/CitizenCRM`, `/ContractManager`, `/DataHub`, `/DigitalVault`, `/FormBuilder`, `/NotifyHub`, `/ProcurementHub`, `/ReportEngine`, `/ServiceDesk` → ROOT  
- `/DocumentFlow`, `/ESignature`, `/OutlookExtractor` → ROOT  
- `/tradex` → ROOT/TradeX/.../wwwroot  
- `/ierahkwa-shop`, `/chat` (ierahkwa-shop/public/chat), `/pos-system`, `/forex-trading-server`, `/image-upload` → ROOT  
- `/SmartSchool`, `/SpikeOffice`, `/quantum`, `/ai`, `/IERAHKWA-PLATFORM-DEPLOY`, `/NET10` → ROOT  
- `/docs` → ROOT/docs  

### Redirects y sendFile (rutas cortas, ejemplos)

- `/bdet`, `/bdet-bank` → platform/bdet-bank.html o lógica BDET  
- `/central-banks`, `/4-banks` → central-banks  
- `/siis`, `/settlement` → siis-settlement  
- `/debt-collection`, `/deudas` → debt-collection  
- `/sovereignty`, `/soberania` → sovereignty-education  
- `/futurehead`, `/futurehead-group`, `/mamey`, `/mamey-futures`, `/trading`, `/futures`, `/commodities`, `/options` → futurehead / mamey-futures  
- `/bitcoin-hemp`, `/crypto` → bitcoin-hemp  
- `/atm`, `/atm-manufacturing` → atm-manufacturing  
- `/bank-worker`, `/global-banking`, `/banking` → bank-worker / bdet  
- `/security`, `/leader-control`, `/monitor` → security-fortress, leader-control, monitor  
- `/wallet`, `/forex` → wallet, forex  
- `/gaming`, `/casino`, `/lotto`, `/raffle` → gaming-platform, casino, lotto, raffle  
- `/documents`, `/login`, `/cryptohost`, `/net10`, `/farmfactory`, `/dao`, `/ido-factory`  
- `/spike-office`, `/rnbcal`, `/appbuilder`, `/esignature`, `/citizen-crm`  
- `/health-dashboard`, `/support-ai`, `/notifications`, `/settings`, `/video-call`, `/secure-chat`  
- `/contribution-graph`, `/biometrics`, `/budget-control`, `/chat`, `/dashboard`, `/dashboard-full`, `/user-dashboard`  
- `/digital-vault`, `/email-studio`, `/financial-instruments`, `/invoicer`, `/meeting-hub`, `/project-hub`  
- `/service-desk`, `/sistema-bancario`, `/smartschool`, `/social-codes`, `/sports-betting`, `/workflow`  
- `/animstorm-ai`, `/ai-hub`, `/atabey`, `/editor`, `/social-media`, `/app-ai-studio`  
- `/backup`, `/backup-department` → backup-department  
- `/departments`, `/103-departments`, `/depts` → departments  
- `/launchpad`, `/citizen-launchpad`, `/tokenize`, `/register-project` → citizen-launchpad  
- `/token-factory`, `/create-token`, `/bridge`, `/analytics`, `/voting`, `/governance`, `/rewards`, `/gamification`  
- `/membership`, `/citizen-membership`, `/members`, `/invest` → citizen-membership  
- `/commerce-business-dashboard.html`, `/platform-services.json`, `/RECIBIR_CRYPTOHOST_CONVERTIR_USDT.html`  

*(Lista completa en `PLATAFORMAS-8545.md` y en `node/server.js`.)*

---

## 4. PLATAFORMAS HTML (platform/*.html)

**Total: 86 archivos.**

| Categoría | Archivos (ejemplos) |
|-----------|----------------------|
| **Hub / Admin** | index.html, admin.html, login.html, dashboard.html, dashboard-full.html, user-dashboard.html |
| **Banca / Finanzas** | bdet-bank.html, bank-worker.html, forex.html, wallet.html, vip-transactions.html, sistema-bancario.html, financial-instruments.html |
| **Blockchain / DeFi** | blockchain-platform.html, tradex.html, net10-defi.html, farmfactory.html, cryptohost.html, bridge.html, token-factory.html, ido-factory.html, dao-governance.html |
| **Gobierno** | government-portal.html, departments.html, leader-control.html, sovereignty-education.html, siis-settlement.html, debt-collection.html, central-banks.html |
| **Futurehead / Negocios** | futurehead-group.html, mamey-futures.html, bitcoin-hemp.html, atm-manufacturing.html, backup-department.html |
| **Gaming** | gaming-platform.html, casino.html, lotto.html, raffle.html, sports-betting.html |
| **Social / Comunicación** | social-media.html, social-platform.html, social-media-codes.html, chat.html, secure-chat.html, video-call.html, notifications.html |
| **AI / Quantum** | ai-platform.html, ai-hub-dashboard.html, atabey-dashboard.html, support-ai.html, quantum-platform.html, animstorm-ai.html |
| **Salud / Seguros / Servicios** | health-platform.html, health-dashboard.html, insurance-platform.html, services-platform.html |
| **Educación / Oficina** | education-platform.html, smartschool.html, spike-office.html, rnbcal.html, app-studio.html, appbuilder.html, app-ai-studio.html |
| **Documentos / Legal** | documents.html, esignature.html, citizen-crm.html, citizen-launchpad.html, citizen-membership.html |
| **Proyectos / Reuniones** | project-hub.html, meeting-hub.html, service-desk.html, budget-control.html, digital-vault.html, workflow-engine.html |
| **Seguridad / Otros** | security-fortress.html, monitor.html, settings.html, biometrics.html, email-studio.html, invoicer.html, contribution-graph.html |
| **Analytics / Voting / Rewards** | analytics-dashboard.html, voting.html, rewards.html |
| **Plantilla** | template-unified.html |

---

## 5. CONFIGURACIÓN (platform/config.json)

| Sección | Cantidad | Descripción |
|---------|----------|-------------|
| **platform** | 1 | name, subtitle, version, logo, domain, footer |
| **stats** | 4 | tokens: 103, platforms: 50+, chainId: 777777, nodeStatus: LIVE |
| **headerNav** | **15** | GOV, ADMIN, BANK, BLOCKCHAIN, GAMING, SOCIAL, AI, QUANTUM, EDUCATION, HEALTH, INSURANCE, SERVICES, APP STUDIO, RUDDIE, SECURITY |
| **quickActions** | **11** | TRADEX, NET10 DEFI, FARMFACTORY, VIP, CASINO, SOCIAL, LOTTO, SHOP, SPIKE OFFICE, RnBCAL, APPBUILDER |
| **departments** | **12** | node-main, blockchain, bdet-bank, global-bank, tradex, net10, farmfactory, vip, shop, spikeoffice, rnbcal, appbuilder |
| **services** | **51** | gov, admin, bank, blockchain, gaming, social, ai, quantum, education, health, insurance, services, appstudio, security, leader, globalbank, tradex, net10, farmfactory, cryptohost, banking, tokens, casino, lotto, raffle, vip, videocall, securechat, chat, shop, pos, inventory, crm, rnbcal, appbuilder, spikeoffice, advocate, school-node, dao, forex, wallet, monetary, global-service, clearhouse, idofactory, documentflow, esignature, projecthub, meetinghub, images, portal |
| **platformSummary** | 7 | IGT Tokens, Government Departments, Service Platforms, Casino•Social•Lotto, Finance Tokens, .NET 10, Sovereign Power |
| **tokens** | **103** | IGT-PM … IGT-ESIGN (id 01–103) |
| **healthServices** | **7** | node (8545), tradex (5054), net10 (5071), farmfactory (5061), spikeoffice (5056), rnbcal (5055), appbuilder (5060) |
| **theme** | 10 | gold, goldDark, neonGreen, neonCyan, neonMagenta, neonPink, neonOrange, neonPurple, bgDark, bgCard |

---

## 6. ADMIN (platform/admin.html)

### Pestañas / paneles (16)

| # | ID panel | Tab |
|---|----------|-----|
| 1 | panel-platform | 🏛️ Platform |
| 2 | panel-headerConfig | 📐 HEADER CONFIG |
| 3 | panel-allplatforms | 🌐 ALL PLATFORMS |
| 4 | panel-monitor | 📊 Monitor |
| 5 | panel-backup | 🔄 Backup |
| 6 | panel-settings | ⚙️ Settings |
| 7 | panel-departments | 🏢 Departments |
| 8 | panel-services | 🔗 Services |
| 9 | panel-headerNav | 🧭 Header Nav |
| 10 | panel-quickActions | ⚡ Quick Actions |
| 11 | panel-sections | 📑 Sections |
| 12 | panel-summary | (Summary) |
| 13 | panel-tokens | 🪙 Tokens |
| 14 | panel-health | 💚 Health |
| 15 | panel-theme | 🎨 Theme |
| 16 | **panel-linksBotones** | **🔗 LINKS Y BOTONES** |
| 17 | panel-export | 📦 Export/Import |

### Acceso

- **URL:** `/admin`, `/platform/admin.html` o enlace ⚙️ ADMIN.  
- **Auth:** redirige a `/platform/login.html` si no hay sesión `ierahkwa_session` con `role` admin/superadmin.

---

## 7. LINKS Y BOTONES (edición en Admin)

- **Reporte detallado:** `REPORTE-LINKS-Y-BOTONES.md`  
- **Datos editables:** `platform/data/platform-links.json`  
- **Admin:** pestaña **🔗 LINKS Y BOTONES** — activar/desactivar, orden, sección, editar; Guardar (localStorage `ierahkwa_platform_links`), Exportar JSON, Cargar desde `platform-links.json`.

Resumen por tipo:

| Tipo | Cantidad aprox. | Secciones en Admin |
|------|-----------------|--------------------|
| version-badges | 24 | version-badges |
| open-dashboard | 13 | dashboard |
| headerNav | 15 | headerNav |
| quickActions | 11 | quickActions |
| services | 51 (43 en JSON) | services |

---

## 8. DATOS Y ASSETS

| Ruta | Contenido |
|------|-----------|
| `platform/config.json` | Configuración global (headerNav, quickActions, services, tokens, etc.) |
| `platform/data/platform-links.json` | Array de ítems para Links y Botones (id, label, url, platformKey, type, section, enabled, order) |
| `config/services-ports.json` | Puertos de servicios |
| `data/ai-hub/` | ai-learnings, atabey (conversations, family-members, preferences, schedules), collected-data, improvements-log, projects-registry, world-intelligence |
| `node/public/` | Static del Node |
| `platform/assets/` | unified-core.js, unified-styles.css |

---

## 9. SCRIPTS DE ARRANQUE (raíz / scripts/)

| Script | Uso |
|--------|-----|
| `./up` | Arranca servidor :8545 si hace falta, abre /platform, /, /bdet-bank, /forex |
| `./start.sh` | Inicia Mamey Node en primer plano |
| `./abre-plataformas.sh` | Abre 18 plataformas en Chrome (sin arrancar servidor) |
| `./start-full-stack.sh` | Node :8545 + .NET Banking :5000 |

*(Delegan a `RuddieSolution/scripts/`.)*

---

## 10. BACKUP Y DEPLOY

- **Backups:** `backup-system/` — `auto-backup.sh`, `backups/`, `install-backup-daemon.sh`  
- **API Backup:** `/api/v1/backup/*` (list, create, restore, download, export, toggle, config, stats)  
- **Deploy:** `deploy/` — Docker, Fly.io, Railway, Render, `digitalocean.md`  
- **Producción:** `platform/PRODUCTION-DEPLOYMENT.md` — requisitos, instalación, Docker, PM2, monitoreo.

---

## 11. DOCUMENTOS DE REFERENCIA

| Archivo | Descripción |
|---------|-------------|
| **INDICE.md** | Índice, arranque, estructura, URLs principales |
| **PLATAFORMAS-8545.md** | Lista de todas las URLs por categoría |
| **REPORTE-LINKS-Y-BOTONES.md** | Inventario links, botones, version-badges, dashboards, headerNav, quickActions, services |
| **REPORTE-COMPLETO.md** | Este reporte (todo el sistema) |
| **platform/PRODUCTION-DEPLOYMENT.md** | Guía de deployment en producción |

---

## 12. ESTADO RÁPIDO

- **Entorno local (Node :8545, platform/, config, BDET, rutas):** ✅ Operativo.  
- **Producción 24/7 (HTTPS, monitoreo, backups DB, alta disponibilidad):** ver `PRODUCTION-DEPLOYMENT.md` y checklists de go-live.

---

*Generado a partir de la estructura y archivos de RuddieSolution. Para URLs concretas, ver `PLATAFORMAS-8545.md`; para links y botones editables, `REPORTE-LINKS-Y-BOTONES.md`.*
