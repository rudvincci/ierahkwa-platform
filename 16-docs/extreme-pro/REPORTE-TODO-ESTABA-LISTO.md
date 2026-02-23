# TODO ESTABA LISTO — Inventario y Guía

**Sistema:** Sovereign Government of Ierahkwa Ne Kanienke  
**Objetivo:** Que el **Sistema Bancario Completo** (200+ APIs) y el **Editor AI / IERAHKWA AI Platform** (100% producción) estén localizados, documentados e integrados para no perderse.

---

## 1. Sistema Bancario Completo (10,676 líneas | 200+ API)

### Dónde está
- **Archivo:** `RuddieSolution/node/banking-bridge.js`
- **Puerto por defecto:** `3001` (`BRIDGE_PORT=3001`)
- **Función:** Conecta Mamey Node con la API Banking .NET (localhost:5000) y **atiende de forma local** todas las rutas bancarias (cards, mobile, remittances, bills, atm, insurance, investments, loyalty, interbank, forex, 2FA).

### Módulos y endpoints (resumen)

| Módulo | Ejemplos de rutas |
|--------|-------------------|
| **Tarjetas** | `POST /api/cards/issue`, `GET /api/cards/citizen/:id`, `POST /api/cards/:id/transaction`, `POST /api/cards/:id/block`, `POST /api/cards/:id/pay`, `GET /api/cards/types` |
| **Mobile Banking** | `POST /api/mobile/login`, `GET /api/mobile/dashboard`, `POST /api/mobile/transfer`, `GET /api/mobile/qr/:accountId` |
| **Remesas SWIFT** | `POST /api/remittances/send`, `GET /api/remittances/track/:id`, `GET /api/remittances/rates`, `GET /api/remittances/swift-codes`, `POST /api/remittances/calculate` |
| **Pago de servicios** | `POST /api/bills/pay`, `POST /api/bills/schedule`, `GET /api/bills/billers`, `POST /api/bills/recharge` |
| **2FA** | `POST /api/auth/otp/generate`, `POST /api/auth/otp/verify`, `POST /api/auth/totp/setup`, `POST /api/auth/device/trust`, `GET /api/auth/session/validate` |
| **ATM** | `GET /api/atm/locations`, `POST /api/atm/:id/withdraw`, `POST /api/atm/:id/deposit`, `POST /api/atm/:id/balance` |
| **Seguros** | `GET /api/insurance/products`, `POST /api/insurance/purchase`, `POST /api/insurance/claim`, `GET /api/insurance/policies/:id` |
| **Inversiones** | `GET /api/investments/funds`, `POST /api/investments/account`, `POST /api/investments/buy`, `POST /api/investments/sell`, `GET /api/investments/portfolio` |
| **Lealtad** | `GET /api/loyalty/:citizenId`, `POST /api/loyalty/:id/earn`, `POST /api/loyalty/:id/redeem`, `GET /api/loyalty/rewards`, `GET /api/loyalty/tiers` |
| **Forex** | `GET /api/forex/rates`, `POST /api/forex/trade`, `GET /api/forex/history/:id` |
| **Interbancario** | `POST /api/interbank/send`, `GET /api/interbank/track/:id`, `GET /api/interbank/networks` |

Lista detallada en **SISTEMA-BANCARIO-REPORTE-COMPLETO.md**.

### Cómo arrancar
```bash
cd RuddieSolution/node
BRIDGE_PORT=3001 node banking-bridge.js
```
Con PM2 (ecosystem.config.js): `pm2 start ecosystem.config.js` — incluye `ierahkwa-banking-bridge` en 3001.

### Integración con Mamey Node (8545)
El **server.js** (8545) hace **proxy** de estas rutas a `http://localhost:3001` cuando el bridge está en marcha. Así las apps que corren en 8545 pueden llamar a `/api/cards`, `/api/mobile`, etc. en el mismo origen.

---

## 2. Editor AI / IERAHKWA AI Platform (100% Producción)

### Dónde está
- **Frontend:** `RuddieSolution/platform/editor-complete.html` (Monaco, File Explorer, Tabs, Terminal, Chat IA, Autocompletado, Git, Code Analysis, Auto-guardado)
- **Backend:** `RuddieSolution/platform/api/editor-api.js`
- **Puerto por defecto:** `3002` (`PORT=3002`) — se usa 3002 para no chocar con banking-bridge en 3001.
- **Ruta corta en 8545:** `/editor` → sirve `editor-complete.html`.

### API del editor (15 endpoints)
- **Files:** `GET /api/files/tree`, `GET /api/files/read`, `POST /api/files/save`, `POST /api/files/create`, `DELETE /api/files/delete`, `POST /api/files/mkdir`
- **AI:** `POST /api/ai/chat`, `POST /api/ai/completions`, `POST /api/ai/code/generate`
- **Terminal:** `POST /api/terminal/execute`
- **Git:** `GET /api/git/status`, `POST /api/git/commit`
- **Code:** `POST /api/code/analyze`
- **Platform:** `GET /api/platform/modules`, `GET /api/platform/services`, `GET /api/platform/config`
- **Health:** `GET /api/health`

### Cómo arrancar
```bash
cd RuddieSolution/platform
./start.sh
```
O desde `platform/api`: `PORT=3002 node editor-api.js`.  
Abre: `http://localhost:3002/editor-complete.html` o, vía 8545: `http://localhost:8545/editor`.

### Integración con Mamey Node (8545)
- La ruta **`/editor`** en 8545 sirve `editor-complete.html`.
- `editor-complete.html` usa **`API_BASE = window.location.origin`** en localhost: las llamadas a `/api/files`, `/api/ai/chat`, etc. van a 8545 cuando se abre desde 8545.
- El **server.js** hace **proxy** de `/api/files`, `/api/terminal`, `/api/git`, `/api/code`, `/api/platform`, `/api/ai/chat`, `/api/ai/completions` a `http://localhost:3002`. Si el editor-api está en 3002, el editor funciona desde `http://localhost:8545/editor` sin CORS.

---

## 3. Cómo usar todo junto

### Opción A – Solo Mamey Node (8545)
- `./start.sh` → 8545.
- **/editor** funciona si además está corriendo **editor-api en 3002**; si no, las llamadas a /api/files etc. devolverán 503.
- **/api/cards, /api/mobile, etc.** funcionan si está **banking-bridge en 3001**; si no, 503.

### Opción B – Todo integrado (recomendado)
1. **Terminal 1 – Mamey Node**
   ```bash
   ./start.sh
   ```
2. **Terminal 2 – Banking Bridge**
   ```bash
   cd RuddieSolution/node && BRIDGE_PORT=3001 node banking-bridge.js
   ```
3. **Terminal 3 – Editor API**
   ```bash
   cd RuddieSolution/platform && PORT=3002 node api/editor-api.js
   ```

Luego:
- Plataforma: `http://localhost:8545/platform`
- Editor: `http://localhost:8545/editor`
- APIs bancarias: `http://localhost:8545/api/cards/...`, `http://localhost:8545/api/mobile/...`, etc.

### Opción C – PM2 (ecosystem: prender todo)
```bash
cd RuddieSolution/node
pm2 start ecosystem.config.js
```
El **ecosystem.config.js** ya incluye:
- **ierahkwa-node-server** (8545)
- **ierahkwa-banking-bridge** (3001)
- **ierahkwa-editor-api** (3002)

### Opción D – start-all.sh (ecosystem / procesos)
En la raíz: `./start-all.sh`. Con PM2: arranca el ecosystem (Node + Bridge + Editor API). Sin PM2: lanza Node, Banking Bridge, Editor API, Banking .NET y Platform en background. Funciona con `node/` y `platform/` en la raíz o dentro de `RuddieSolution/`.

---

## 4. Resumen de puertos

| Servicio | Puerto | Archivo |
|----------|--------|---------|
| Mamey Node | 8545 | `RuddieSolution/node/server.js` |
| Banking Bridge | 3001 | `RuddieSolution/node/banking-bridge.js` |
| Editor API | 3002 | `RuddieSolution/platform/api/editor-api.js` |
| Banking .NET (opcional) | 5000 | IerahkwaBanking.NET10 / BDET .NET |

---

## 5. Referencias
- **Sistema bancario detallado:** `SISTEMA-BANCARIO-REPORTE-COMPLETO.md`
- **Editor / producción:** `RuddieSolution/platform/REPORTE-PRODUCCION-100.md`, `REPORTE-PRODUCCION-COMPLETO.md`
- **Índice general:** `RuddieSolution/INDICE.md`
- **Plataformas y rutas:** `RuddieSolution/PLATAFORMAS-8545.md`

---

*Documento de inventario. Actualizar cuando se añadan servicios o cambien puertos.*
