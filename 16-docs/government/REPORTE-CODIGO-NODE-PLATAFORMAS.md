# Reporte: código Node y plataformas

**Sovereign Government of Ierahkwa Ne Kanienke**  
Estado del código del servidor Node (8545) y de las plataformas HTML. Fecha: febrero 2026.

---

## 1. Resumen numérico

| Concepto | Cantidad |
|----------|----------|
| **Archivos .js en Node** | ~416 |
| **Rutas cortas (URL → HTML)** | 159 (en `platform-routes.js`) |
| **Páginas HTML (plataformas)** | 298 (en `RuddieSolution/platform/`) |
| **Módulos Node** | 102 (`node/modules/*.js`) |
| **Servicios Node** | 38 (`node/services/*.js`) |
| **Rutas API (archivos)** | 69 (`node/routes/*.js`) |
| **Endpoints /api/platform/** | 12+ (health, links, all-pages, departments, etc.) |
| **Data de plataforma (JSON)** | 27 archivos en `platform/data/` |
| **Assets compartidos (CSS/JS)** | 47 en `platform/assets/` |

---

## 2. Node — dónde está el código

### 2.1 Entrada principal

| Archivo | Función |
|---------|---------|
| `RuddieSolution/node/server.js` | Servidor Express (puerto 8545), monta `/platform`, APIs, rutas sensibles, lockdown, Fortress. |
| `RuddieSolution/node/platform-routes.js` | Define las **159 rutas cortas** (ej. `/bdet` → `bdet-bank.html`). `registerPlatformRoutes(app)` registra todas en Express. |
| `RuddieSolution/node/banking-bridge.js` | Puente bancario (puerto 3001). |

### 2.2 Carpetas principales

| Carpeta | Archivos .js | Uso |
|---------|--------------|-----|
| `node/routes/` | 69 | APIs por dominio (platform-auth, platforms-api, banking, kyc, casino, compliance-watch, etc.). |
| `node/modules/` | 102 | Lógica de negocio (government-banking, mamey-futures, marketplace, quantum-encryption, sovereign-ai, etc.). |
| `node/services/` | 38 | Servicios (health-monitor, kyc, payments, pdf-generator, kms, email-soberano, i18n, etc.). |
| `node/middleware/` | — | audit-sensitive, webhook-verify, admin-ip-allowlist, performance. |
| `node/ai-hub/` | — | Atabey, project-registry, data-collector. |
| `node/public/` | — | Archivos estáticos servidos por el Node (p. ej. financial-center.html). |
| `node/data/` | — | Data del banco, VIP, estado, blockchain (JSON). |
| `node/logging/` | — | centralized-logger. |
| `node/lib/` | — | crypto-native, audit-utils, projections-tables. |

### 2.3 APIs de plataforma (server.js)

| Endpoint | Función |
|----------|---------|
| `GET /api/platform/health` | Estado de servicios de la plataforma. |
| `GET /api/platform/links` | Enlaces del hub (`platform/data/platform-links.json`). |
| `GET /api/platform/all-pages` | Lista de todas las .html bajo `platform/` (recursivo). |
| `GET /api/platform/departments` | Departamentos (`government-departments.json`). |
| `GET /api/platform/overview` | Resumen (links + departments). |
| `GET /api/platform/services` | Servicios. |
| `GET /api/platform/uptime` | Historial de uptime. |
| `GET /api/platform/commercial-services` | Servicios comerciales (monthly). |
| `GET /api/platform/commercial-services/rental` | Alquiler. |
| `GET /api/platform/commercial-services-rental` | Renta (alternativo). |
| `GET /api/platform/commercial-services-monthly` | Mensual. |
| `GET /api/platform/servicios-comerciales-renta` | Renta (ES). |
| `GET /api/platform/commerce-types-global-rental` | Tipos de comercio global. |
| `GET /api/platform/modules` | Módulos. |
| `GET /api/platform/tokens` | Tokens. |

---

## 3. Plataformas — dónde está el código

### 3.1 HTML (páginas)

- **Ruta:** `RuddieSolution/platform/*.html` y subcarpetas (p. ej. `platform/docs/`).
- **Total:** 298 archivos .html.
- **Acceso:** Rutas cortas vía `platform-routes.js` (ej. `/bdet-bank`, `/forex`) o directo `http://localhost:8545/platform/<nombre>.html`.
- **Hub:** `platform/index.html` (página principal que enlaza a todas).

### 3.2 Rutas cortas (resumen por categoría)

- **Banca:** `/bdet`, `/bdet-bank`, `/bank-worker`, `/wallet`, `/forex`, `/vip`, `/vip-transactions`, `/central-banks`, `/siis`, `/treasury`, `/depository`, etc.
- **Gobierno / seguridad:** `/security-fortress`, `/leader-control`, `/admin`, `/documents`, `/debt-collection`, `/sovereignty`.
- **Mamey / trading:** `/mamey-futures`, `/futurehead-group`, `/trading`, `/commodities`.
- **Plataformas:** `/gaming`, `/casino`, `/lotto`, `/raffle`, `/documents`, `/login`, `/cryptohost`, `/net10`, `/dao`, `/spike-office`, `/citizen-crm`, `/tax`, `/biometrics`, `/project-hub`, `/meeting-hub`, `/ai-hub`, `/atabey-dashboard`, `/quantum`, `/monitor`, etc.
- **Lista completa:** en `RuddieSolution/node/platform-routes.js` (array `ROUTES`).

### 3.3 Data de plataforma

- **Ruta:** `RuddieSolution/platform/data/`.
- **27 archivos JSON**, entre ellos:
  - `platform-links.json` — enlaces del hub.
  - `government-departments.json` — departamentos.
  - `config.json` (en `platform/`) — configuración del hub.
  - `commercial-services-rental.json`, `commercial-services-monthly.json`, `commerce-types-global-rental.json` — servicios comerciales.
  - `platform-landing-info.json`, `TEXTO-OFICIAL-PLATAFORMAS.json`, `platform-dashboards.json`, `platform-module-map.json`, etc.

### 3.4 Assets (estilos y JS compartidos)

- **Ruta:** `RuddieSolution/platform/assets/`.
- **47 archivos**, destacados:
  - `unified-styles.css` — estilos unificados.
  - `unified-core.js` — utilidades, API, notificaciones, AI panel, tablas, gráficos.
  - `platform-api-client.js`, `api-client.js` — cliente API.
  - `visual-enhance.js` — mejoras visuales.
  - Bootstrap, Chart.js, Leaflet (subcarpetas).
  - Iconos, favicon, PWA.

---

## 4. Flujo (logística)

1. Usuario abre `http://localhost:8545` o `http://localhost:8545/platform` → `server.js` sirve `platform/index.html`.
2. El hub usa `platform-links.json` (o API `GET /api/platform/links`) para mostrar enlaces.
3. Clic en una plataforma → URL corta (ej. `/bdet-bank`) → `platform-routes.js` → `server.js` sirve `platform/bdet-bank.html`.
4. Cada .html puede cargar `assets/unified-styles.css`, `unified-core.js` y llamar a las APIs del Node (`/api/platform/*`, `/api/v1/*`, etc.).

---

## 5. Referencias

- **Mapa detallado:** [MAPA-CODIGO-PLATAFORMAS-ESTRUCTURA-LOGISTICA.md](MAPA-CODIGO-PLATAFORMAS-ESTRUCTURA-LOGISTICA.md).
- **Índice del proyecto:** [INDICE-COMPLETO-PROYECTO-SOBERANOS.md](INDICE-COMPLETO-PROYECTO-SOBERANOS.md).
- **Ruta única (no se movió código):** [RUTA-UNICA-PROYECTO.md](RUTA-UNICA-PROYECTO.md).

*Office of the Prime Minister — febrero 2026.*
