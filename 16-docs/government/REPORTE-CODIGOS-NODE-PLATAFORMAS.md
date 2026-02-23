# Reporte: códigos Node y plataformas

**Sovereign Government of Ierahkwa Ne Kanienke**  
Estado del código Node (servidor, rutas, módulos, servicios) y de las plataformas (HTML, rutas URL, data, assets).  
*Generado: febrero 2026.*

---

## 1. Node — Resumen

| Concepto | Cantidad / Ubicación |
|----------|----------------------|
| **Servidor principal** | `RuddieSolution/node/server.js` (puerto 8545) |
| **Archivos .js en node/** | **416** archivos |
| **Rutas definidas en server.js** | ~719 referencias (get/post/use) |
| **Módulos** | **102** en `node/modules/` |
| **Rutas API (carpeta routes)** | **69** en `node/routes/` |
| **Servicios** | **38** en `node/services/` |
| **Rutas de plataforma (URL → HTML)** | **159** en `node/platform-routes.js` |

---

## 2. Node — Estructura de carpetas

| Carpeta | Contenido |
|---------|-----------|
| `RuddieSolution/node/` | Raíz: server.js, platform-routes.js, banking-bridge.js, ai-hub/, api/, lib/, logging/, middleware/, public/, scripts/, telecom/ |
| `node/modules/` | 102 módulos (government-banking, mamey-futures, sovereign-ai, quantum-encryption, marketplace, virtual-cards, etc.) |
| `node/routes/` | 69 APIs (platform-auth, platforms-api, banking, kyc-api, casino-api, compliance-watch-api, etc.) |
| `node/services/` | 38 servicios (health-monitor, kms, kyc, payments, pdf-generator, i18n, service-registry, etc.) |
| `node/ai-hub/` | atabey-system.js, data-collector.js, project-registry.js, index.js, etc. |
| `node/middleware/` | audit-sensitive, rate-limit, jwt-auth, webhook-verify, etc. |
| `node/data/` | Data del banco, VIP, bonos, estado (JSON) |

---

## 3. Plataformas — Resumen

| Concepto | Cantidad / Ubicación |
|----------|----------------------|
| **Páginas HTML** | **298** en `RuddieSolution/platform/` (incl. subcarpetas como docs/) |
| **Rutas cortas (URL → archivo)** | **159** en `node/platform-routes.js` (ej. /bdet → bdet-bank.html, /forex → forex.html) |
| **Data (platform/data)** | **27** archivos (platform-links.json, government-departments.json, commercial-services-*.json, etc.) |
| **Assets (platform/assets)** | **47** archivos (unified-styles.css, unified-core.js, visual-enhance.js, bootstrap, chartjs, etc.) |

---

## 4. APIs de plataforma (Node)

Endpoints en `server.js` para las plataformas:

| Endpoint | Uso |
|----------|-----|
| `GET /api/platform/health` | Estado de servicios |
| `GET /api/platform/links` | Enlaces (platform-links.json) |
| `GET /api/platform/all-pages` | Lista de todas las .html (descubrimiento recursivo) |
| `GET /api/platform/departments` | Departamentos (government-departments.json) |
| `GET /api/platform/overview` | Resumen links + departments |
| `GET /api/platform/services` | Servicios |
| `GET /api/platform/uptime` | Historial uptime |
| `GET /api/platform/commercial-services` | Servicios comerciales |
| `GET /api/platform/commercial-services-rental` | Renta por plataforma |
| `GET /api/platform/commercial-services-monthly` | Servicios mensuales |
| `GET /api/platform/servicios-comerciales-renta` | Renta (es) |
| `GET /api/platform/commerce-types-global-rental` | Tipos de comercio/renta |
| `GET /api/platform/modules` | Módulos |
| `GET /api/platform/tokens` | Tokens |

---

## 5. Rutas cortas (ejemplos) — platform-routes.js

| URL | Archivo servido |
|-----|-----------------|
| /, /platform | index.html |
| /bdet, /bdet-bank | bdet-bank.html |
| /forex | forex.html |
| /wallet | wallet.html |
| /vip, /vip-transactions | vip-transactions.html |
| /central-banks, /4-banks | central-banks.html |
| /siis, /settlement | siis-settlement.html |
| /admin | admin.html |
| /login | login.html |
| /casino, /lotto, /raffle | casino.html, lotto.html, raffle.html |
| /gaming | gaming-platform.html |
| /atabey, /atabey-dashboard | atabey-dashboard.html |
| /security-fortress | security-fortress.html |
| /mamey-futures, /futures | mamey-futures.html |
| /project-hub | project-hub.html |
| /documents | documents.html |
| ... | 159 rutas en total |

---

## 6. Archivos clave

| Qué | Ruta |
|-----|------|
| Servidor | `RuddieSolution/node/server.js` |
| Rutas URL → HTML | `RuddieSolution/node/platform-routes.js` |
| Hub (index) | `RuddieSolution/platform/index.html` |
| Enlaces del hub | `RuddieSolution/platform/data/platform-links.json` |
| Config del hub | `RuddieSolution/platform/config.json` |
| Estilos unificados | `RuddieSolution/platform/assets/unified-styles.css` |
| Core JS compartido | `RuddieSolution/platform/assets/unified-core.js` |

---

## 7. Resumen numérico

| Tipo | Cantidad |
|------|----------|
| Archivos .js en node/ | 416 |
| Módulos (node/modules) | 102 |
| Rutas API (node/routes) | 69 |
| Servicios (node/services) | 38 |
| Páginas HTML (platform/) | 298 |
| Rutas cortas (platform-routes.js) | 159 |
| Archivos data (platform/data) | 27 |
| Archivos assets (platform/assets) | 47 |

*Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister.*
