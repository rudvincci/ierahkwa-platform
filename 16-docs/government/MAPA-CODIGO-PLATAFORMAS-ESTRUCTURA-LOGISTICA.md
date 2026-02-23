# Mapa: código, estructura y logística de las plataformas

**Sovereign Government of Ierahkwa Ne Kanienke**  
Para no perderte: dónde está el código de cada plataforma, cómo está estructurado todo y cómo funciona (logística).

---

## Lo mejor (Mamey + RuddieSolution)

**Recomendación oficial:** **Opción A ya** (orquestación + APIs), **Opción B como rumbo** (Node con arquitectura estilo Mamey). Ver [OPCIONES-UNION-MAMEY-RUDDIESOLUTION.md](OPCIONES-UNION-MAMEY-RUDDIESOLUTION.md).

---

## 1. Dónde está el código de las plataformas

| Qué | Ruta exacta | Qué contiene |
|-----|-------------|----------------|
| **HTML de cada plataforma** | `RuddieSolution/platform/*.html` | Un archivo .html por pantalla (bdet-bank.html, forex.html, wallet.html, admin.html, casino.html, etc.). También subcarpetas: `platform/docs/`. |
| **Rutas URL → archivo** | `RuddieSolution/node/platform-routes.js` | Lista `ROUTES`: cada ruta (ej. `/bdet`, `/forex`) apunta a un archivo (ej. `bdet-bank.html`). **Aquí se define qué URL sirve qué HTML.** |
| **Servidor que lo sirve** | `RuddieSolution/node/server.js` | Express: monta `express.static` en `/platform` (cualquier .html bajo platform/) y usa `platform-routes.js` para rutas cortas (/bdet, /forex, etc.). |
| **Data de las plataformas** | `RuddieSolution/platform/data/` | `platform-links.json` (enlaces del hub), `government-departments.json`, `config.json`, commercial-services-*.json, etc. |
| **Estilos y JS compartidos** | `RuddieSolution/platform/assets/` | `unified-styles.css`, `unified-core.js`, `visual-enhance.js`. Las páginas los referencian para look & feel unificado. |
| **Config del hub** | `RuddieSolution/platform/config.json` | Configuración del index/hub: secciones, botones, primaryPlatforms. |
| **Hub principal (index)** | `RuddieSolution/platform/index.html` | La página principal que enlaza a todas las plataformas. |

**Resumen:** El “código” de cada plataforma es un **.html** en `RuddieSolution/platform/`. La “lista” de qué URL abre qué .html está en **`RuddieSolution/node/platform-routes.js`**. El servidor es **`RuddieSolution/node/server.js`** (puerto 8545).

---

## 2. Estructura de la plataforma

```
RuddieSolution/
├── node/
│   ├── server.js              ← Servidor principal (8545); monta /platform y registra rutas
│   ├── platform-routes.js     ← ROUTES: /bdet → bdet-bank.html, /forex → forex.html, etc.
│   ├── routes/                ← Otras rutas API (platform-auth, platforms-api, etc.)
│   └── ...
├── platform/
│   ├── index.html             ← Hub principal (lista de plataformas)
│   ├── *.html                 ← Una página por plataforma (bdet-bank.html, forex.html, ...)
│   ├── assets/                ← unified-styles.css, unified-core.js, visual-enhance.js
│   ├── data/                  ← platform-links.json, government-departments.json, config
│   ├── config.json            ← Config del hub (secciones, botones)
│   └── docs/                  ← Páginas bajo /platform/docs/...
└── platform-services.json     ← Listado de servicios (raíz RuddieSolution)
```

- **Rutas cortas** (ej. `/bdet`, `/forex`) → definidas en `platform-routes.js` → sirven el .html desde `platform/`.
- **Ruta directa** → `http://localhost:8545/platform/cualquier.html` → express.static sirve `platform/cualquier.html`.
- **Lista dinámica de todas las páginas** → API `GET /api/platform/all-pages` (server.js descubre todos los .html bajo platform/).

---

## 3. Logística (flujo: qué llama a qué)

| Paso | Qué pasa |
|------|----------|
| 1 | Usuario abre `http://localhost:8545` o `http://localhost:8545/platform` → server.js sirve `platform/index.html`. |
| 2 | index.html (hub) muestra tarjetas/botones; los enlaces vienen de `platform/data/platform-links.json` o de `config.json` (y se pueden pedir por API: `GET /api/platform/links`). |
| 3 | Usuario hace clic en una plataforma (ej. BDET Bank) → va a `/bdet-bank` o `/bdet` → server.js usa platform-routes → responde con `platform/bdet-bank.html`. |
| 4 | Cada .html puede cargar `assets/unified-styles.css` y `assets/unified-core.js`; y llama a las APIs del Node (ej. `/api/v1/bdet/`, `/api/platform/links`, etc.). |
| 5 | APIs de plataforma en server.js: `/api/platform/links`, `/api/platform/all-pages`, `/api/platform/departments`, `/api/platform/health`, `/api/platform/commercial-services`, etc. La data se lee de `platform/data/`. |

**En una frase:** El navegador pide una URL → el Node (server.js + platform-routes) entrega un .html de `platform/` → ese .html usa assets y llama a APIs del mismo Node.

---

## 4. Archivos clave para no perderte

| Si buscas… | Abre |
|------------|------|
| Lista de todas las rutas cortas (URL → HTML) | `RuddieSolution/node/platform-routes.js` (array `ROUTES`) |
| Dónde se monta /platform y se registran rutas | `RuddieSolution/node/server.js` (buscar "platform", "platform-routes", "registerPlatformRoutes") |
| Enlaces del hub / dashboards | `RuddieSolution/platform/data/platform-links.json` y `RuddieSolution/platform/config.json` |
| Lista de todas las .html (dinámica) | En el navegador: `GET http://localhost:8545/api/platform/all-pages` |
| Documentación de URLs por categoría | `RuddieSolution/PLATAFORMAS-8545.md` |
| Estado y verificación de plataformas | `RuddieSolution/ESTADO-PLATAFORMAS-SERVICIOS.md` |
| Páginas con diseño unificado | `RuddieSolution/platform/UNIFIED-PAGES.md` |

---

## 5. Resumen

- **Código de las plataformas** = archivos `.html` en `RuddieSolution/platform/`; la relación URL → archivo está en `RuddieSolution/node/platform-routes.js`.
- **Estructura** = Node (server.js + platform-routes) sirve `platform/` (HTML + assets + data); config y enlaces en `platform/config.json` y `platform/data/`.
- **Logística** = Usuario → Node (8545) → rutas cortas o `/platform/*.html` → HTML → APIs del Node (`/api/platform/*`, `/api/v1/*`).

Si algo no lo encuentras, el primer lugar a mirar es **`RuddieSolution/node/platform-routes.js`** (qué URL abre qué .html) y **`RuddieSolution/platform/`** (los .html y la carpeta `data/`).

*Última actualización: febrero 2026.*
