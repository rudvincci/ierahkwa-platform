# Nuestros servicios y banco — Todo en el Node

**Sovereign Government of Ierahkwa Ne Kanienke**  
Principio: **nuestros servicios y banco** — todos los servicios de la plataforma y la banca están en nuestro Node: IERAHKWA Futurehead Mamey Node (`RuddieSolution/node/server.js`), puerto **8545**.

---

## Node sin certificado — Dónde va

**Nuestro Node por defecto corre en HTTP (sin certificado SSL).** Ubicación única:

| Dónde | Valor |
|-------|--------|
| **Script** | `RuddieSolution/node/server.js` |
| **Puerto** | `8545` (o `process.env.PORT`) |
| **Protocolo** | **HTTP** (no HTTPS) |
| **URL base** | `http://127.0.0.1:8545` o `http://localhost:8545` |
| **Env** | No definir `USE_HTTPS=true` en `.env` (o dejarlo comentado) |

- **Arranque:** `./start.sh` o `cd RuddieSolution/node && node server.js`.
- **Health:** `http://localhost:8545/health`.
- **Status:** `status.sh` usa `NODE_HOST="127.0.0.1:8545"` y `NODE_URL="http://$NODE_HOST"` — "nodo nuestro, sin certificado".
- **Config de puertos:** `RuddieSolution/config/services-ports.json` → `core_services.node_mamey` (port 8545, script `node/server.js`).

Si quieres HTTPS con certificado propio, pon `USE_HTTPS=true` en `.env` y configura `node/ssl/ssl-config.js` (cert en `node/ssl/certs/propio/`). Sin eso, el Node queda **sin certificado** en el puerto indicado.

---

## Regla

**Nuestros servicios y banco.**  
La plataforma no depende de servicios externos para operar: APIs, rutas de plataforma, estáticos, datos y **banca** (BDET, wallet, forex, VIP, central banks, SIIS) se sirven desde el mismo proceso Node (Mamey Node). Sin certificados ajenos; código propio.

---

## Qué sirve el Node (server.js)

### Origen

- **Aplicación:** `RuddieSolution/node/server.js`
- **Puerto:** `process.env.PORT || 8545`
- **Nombre:** IERAHKWA Futurehead Mamey Node (Blockchain Node Server)

### Rutas de plataforma (HTML)

Registradas en `node/platform-routes.js` y montadas en el Node. Ejemplos (todas bajo el mismo origen `:8545`):

| Ruta | Archivo |
|------|---------|
| `/`, `/platform` | index / platform |
| `/bdet-bank`, `/wallet`, `/forex` | banca, wallet, forex |
| `/vip-transactions`, `/central-banks`, `/siis-settlement` | VIP, bancos centrales, settlement |
| `/social-media`, `/dating` | social, dating (IGT-DATING) |
| `/casino`, `/lotto`, `/raffle` | gaming |
| `/departments`, `/citizen-launchpad` | gobierno, launchpad |
| `/security-fortress`, `/atabey-platform` | seguridad, ATABEY |
| `/voting`, `/rewards`, `/token-factory` | gobernanza, tokens |
| … | Ver `platform-routes.js` para la lista completa |

Todas estas URLs se resuelven en el mismo Node; no hay redirección a otros dominios para la plataforma principal.

### APIs (prefijo `/api` o `/api/v1/...`)

Montadas en el mismo `server.js`; son **servicios nuestros Node**:

| Prefijo | Servicio |
|---------|----------|
| `/api/platform-auth` | Autenticación plataforma |
| `/api/v1/iptv` | IPTV |
| `/api/v1/public-affairs` | SOV-SPAN, asuntos públicos |
| `/api/v1/social` | Red social soberana |
| `/api/v1/dating` | Dating (IGT-DATING) |
| `/api/v1/streaming` | Streaming (video/música) |
| `/api/v1/market` | Marketplace |
| `/api/v1/mamey` | Mamey Futures |
| `/api/v1/bdet` | Government banking |
| `/api/v1/payment` | Pagos soberanos (IGT/BDET/ISB — sin Stripe, sin certificados ajenos) |
| `/api/v1/face` | Face recognition propio |
| `/api/v1/watchlist` | Watchlist |
| `/api/v1/atabey/status` | ATABEY, vigilancia |
| `/api/v1/security/*` | Seguridad, nubes, endpoint, anomaly |
| `/api/v1/sovereignty/*` | Estado, tokens, ecosistema |
| `/api/v1/dao/*` | DAO, propuestas, votos |
| `/api/v1/quantum` | Cifrado cuántico |
| `/api/v1/vpn`, `/api/v1/tor` | VPN, Tor |
| `/api/v1/telecom/*` | Telecom, móvil, VoIP, internet propio, **AI Voice Agents** (campañas + BDET) |
| `/api/casino` | Casino |
| `/api/ai-hub` | AI Hub |
| `/api/v1/licenses` | Licencias |
| `/api/v1/kyc` | KYC |
| … | Más módulos en `server.js` (modules/, routes/) |

Todas son rutas del mismo proceso Node.

### Estáticos servidos por el Node

El Node sirve (express.static) desde la raíz del repo o desde `node/`:

- `/platform` → `RuddieSolution/platform`
- `/tokens` → `tokens/`
- `/data` → `RuddieSolution/node/data` (o equivalente)
- `/tradex` → TradeX.API/wwwroot
- `/ierahkwa-shop` → ierahkwa-shop
- `/chat`, `/pos` → ierahkwa-shop/public/chat, pos
- `/image-upload` → image-upload
- `/DocumentFlow`, `/ESignature`, `/AssetTracker`, etc. → respectivos proyectos en raíz
- `/docs` → docs

Todo bajo el mismo host/puerto del Node.

---

## Resumen

- **Nuestros servicios y banco:** un solo proceso Node (server.js) expone:
  - Rutas de plataforma (HTML),
  - APIs (`/api`, `/api/v1/...`), incluidas banca (BDET, wallet, forex, VIP, licenses, KYC),
  - Estáticos (platform, tokens, shop, chat, pos, docs, etc.).
- **Todo eso son nuestros servicios y banco:** mismo código, mismo servidor, sin depender de terceros para servir la plataforma ni la banca.
- En `config.json` puede haber enlaces a otros puertos (por ejemplo RnBCal, AppBuilder, SpikeOffice) para funcionalidades opcionales; el núcleo de la plataforma y el banco está **todo en el Node**.

Ver [PRINCIPIO-TODO-PROPIO.md](PRINCIPIO-TODO-PROPIO.md) y [CODIGO-PROPIO.md](CODIGO-PROPIO.md).

---

## Backend propio — Resumen

**Sí: todo este Node es nuestro propio backend.** No es un producto de terceros (no TruLern ni otro SaaS). Código en nuestro repo: `RuddieSolution/node/server.js`, módulos en `node/modules/`, `node/routes/`, `node/services/`. Por defecto corre en **HTTP en el puerto 8545** (sin certificado). Para comprobar estado: `./status.sh`.
