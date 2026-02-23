# Vista global: Data · Backend · Frontend · Admin · User

**Sovereign Government of Ierahkwa Ne Kanienke**  
Un solo mapa: dónde está la data, el backend, el frontend, qué es admin, qué es user y cómo se conecta todo.

---

## 1. DATA — Dónde vive la data

| Origen | Ruta | Quién la usa |
|--------|------|--------------|
| **Backend (Node)** | `RuddieSolution/node/data/` | server.js, módulos, APIs. Banco (bank-registry, bdet-bank/), VIP (vip-transactions.json), estado (blockchain-state, estado-final-sistema), AI (ai-hub/), settlements, compliance, gaming, etc. |
| **Frontend / Platform** | `RuddieSolution/platform/data/` | HTML y APIs del Node. platform-links.json, government-departments.json, commercial-services-*.json, config, landings, soberanía. |
| **Config global** | `RuddieSolution/platform/config.json` | Hub, secciones, primaryPlatforms, dominio. |
| **Inventario completo** | Ver [DATA-QUE-TENEMOS.md](DATA-QUE-TENEMOS.md) | Lista única de toda la data del proyecto. |

---

## 2. BACKEND — Todo en el Node (8545)

| Qué | Dónde | Puerto / URL base |
|-----|--------|-------------------|
| **Servidor** | `RuddieSolution/node/server.js` | **8545** — http://localhost:8545 |
| **Rutas API** | `node/routes/*.js` (69 archivos) | /api/v1/*, /api/platform/*, /api/admin/*, etc. |
| **Módulos** | `node/modules/*.js` (102) | Lógica de negocio (banca, VIP, Mamey Futures, AI, etc.) |
| **Servicios** | `node/services/*.js` (38) | KMS, KYC, payments, health, email, i18n, etc. |
| **Rutas plataforma (URL→HTML)** | `node/platform-routes.js` | 159 rutas cortas (/bdet, /forex, /admin, …) |
| **Banking Bridge** | `node/banking-bridge.js` | 3001 (opcional) |
| **Data** | `node/data/` | JSON que el backend lee/escribe |

**APIs principales:** `/api/platform/*`, `/api/v1/vip/*`, `/api/v1/bdet/*`, `/api/admin/*`, `/api/platform-auth/*`, `/api/v1/fortress/*`, etc.

---

## 3. FRONTEND — Todo en platform (HTML + assets)

| Qué | Dónde | Cómo se sirve |
|-----|--------|----------------|
| **Páginas** | `RuddieSolution/platform/*.html` (298) | Rutas cortas (platform-routes.js) o `/platform/<archivo>.html` |
| **Hub / index** | `platform/index.html` | `/` o `/platform` |
| **Estilos y JS** | `platform/assets/` | unified-styles.css, unified-core.js, visual-enhance.js, bootstrap, etc. |
| **Data para el front** | `platform/data/` | Consumida por HTML vía APIs del Node (/api/platform/links, etc.) |
| **Config** | `platform/config.json` | Secciones, botones, primaryPlatforms del hub |

El frontend **no corre en otro servidor**: lo sirve el mismo Node (8545) como estáticos desde `platform/`.

---

## 4. ADMIN — Rutas y pantallas de administración

| Tipo | Dónde | URL / archivo |
|------|--------|----------------|
| **Pantalla admin** | `platform/admin.html` | `/admin` |
| **APIs admin** | `server.js` (middleware adminIpAllowlist en /api/admin) | `/api/admin/*` |
| **Ejemplos** | | `/api/admin/audit-log`, `/api/admin/audit-log/export`, `/api/admin/sessions`, `/api/admin/lockdown`, `/api/admin/feature-flags`, `/api/admin/maintenance/status`, `/api/admin/security-events` |
| **Login admin** | `platform/login.html` + `/api/platform-auth/login` | `/login` |
| **Lockdown** | server.js | `/api/admin/lockdown` (bloqueo de plataforma) |

Solo para administradores; muchas rutas `/api/admin` pasan por allowlist de IP y/o auth.

---

## 5. USER — Rutas y pantallas de usuario

| Tipo | Dónde | URL / archivo |
|------|--------|----------------|
| **Login usuario** | `platform/login.html` | `/login` |
| **Dashboards usuario** | `platform/dashboard.html`, `user-dashboard.html`, `dashboard-full.html` | `/dashboard`, `/user-dashboard`, `/dashboard-full` |
| **Hub (todos)** | `platform/index.html` | `/`, `/platform` |
| **Plataformas públicas** | Todas las HTML con rutas cortas | `/bdet`, `/forex`, `/wallet`, `/vip`, `/casino`, `/documents`, `/project-hub`, etc. |
| **Auth** | `node/routes/platform-auth.js` | `/api/platform-auth/login`, `/api/platform-auth/session` |

Usuario final = todo lo que no es `/admin` ni `/api/admin`: dashboards, plataformas, login, y APIs públicas (/api/platform/links, /api/platform/health, etc.).

---

## 6. GLOBAL — Cómo se conecta todo

```
                    ┌─────────────────────────────────────────┐
                    │           Node (server.js :8545)          │
                    │  Backend: routes, modules, services      │
                    │  Data: node/data/ (lee/escribe)          │
                    └─────────────────────────────────────────┘
                                      │
         ┌────────────────────────────┼────────────────────────────┐
         │                            │                            │
         ▼                            ▼                            ▼
  /api/platform/*              /api/admin/*                 /api/v1/*, etc.
  /api/platform-auth/*         (admin only)                  (vip, bdet, fortress…)
         │                            │                            │
         │                            │                            │
         ▼                            ▼                            ▼
  Frontend (platform/)          admin.html                    Módulos / otros clientes
  · index.html (hub)            /admin                       · Banking Bridge, apps
  · *.html (298)
  · platform/data/ + config.json
         │
         ▼
  Usuario final: /login, /dashboard, /bdet, /forex, …
  Admin: /admin + /api/admin/*
```

| Capa | Qué es | Dónde |
|------|--------|--------|
| **Data** | Toda la data del sistema | `node/data/` (backend), `platform/data/` (front/config) |
| **Backend** | Un solo Node (8545): APIs, módulos, servicios | `RuddieSolution/node/` |
| **Frontend** | HTML + assets servidos por el Node | `RuddieSolution/platform/` |
| **Admin** | Pantalla admin + APIs /api/admin | `platform/admin.html`, `server.js` |
| **User** | Resto: hub, login, dashboards, plataformas | `platform/*.html` + APIs públicas |
| **Global** | Una base (Node), una platform (HTML), data en node/data y platform/data | Todo en la misma raíz del proyecto |

---

## 7. Resumen en una tabla

| | Data | Backend | Frontend | Admin | User |
|---|------|---------|----------|-------|------|
| **Dónde** | node/data, platform/data | node/server.js, routes, modules, services | platform/*.html, assets | admin.html, /api/admin | index, login, dashboard, resto HTML |
| **URL base** | — | http://localhost:8545 | mismo Node | /admin, /api/admin/* | /, /login, /dashboard, /bdet, … |
| **Referencia** | [DATA-QUE-TENEMOS.md](DATA-QUE-TENEMOS.md) | [REPORTE-CODIGOS-NODE-PLATAFORMAS.md](REPORTE-CODIGOS-NODE-PLATAFORMAS.md) | [MAPA-CODIGO-PLATAFORMAS-ESTRUCTURA-LOGISTICA.md](MAPA-CODIGO-PLATAFORMAS-ESTRUCTURA-LOGISTICA.md) | server.js (api/admin) | platform-routes.js, platform/*.html |

*Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister. Febrero 2026.*
