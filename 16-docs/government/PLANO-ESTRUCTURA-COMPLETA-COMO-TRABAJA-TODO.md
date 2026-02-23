# Plano de estructura completa — Cómo trabaja todo
**Sovereign Government of Ierahkwa Ne Kanienke · 2026-02-05**

Este documento describe **cómo está armado el sistema** y **cómo trabaja** de punta a punta: capas, flujos, archivos de configuración y orquestación.

---

## 1. Vista general (capas)

```
                    ┌─────────────────────────────────────────────────────────┐
                    │  USUARIO / NAVEGADOR                                      │
                    └───────────────────────────┬─────────────────────────────┘
                                                 │
         ┌───────────────────────────────────────┼───────────────────────────────────────┐
         │                                       │                                        │
         ▼                                       ▼                                        ▼
┌─────────────────┐                   ┌─────────────────┐                    ┌─────────────────┐
│  Node :8545     │                   │  Platform :8080  │                    │  APIs directas  │
│  (Puerta única) │                   │  (Estático/Leader)│                    │  (:3000, :5054…) │
│  /health        │                   │  /leader-control  │                    │  Swagger, etc.  │
│  /platform/*    │                   │  /admin, HTML     │                    │                 │
│  /bdet-bank     │                   └────────┬─────────┘                    └────────┬────────┘
│  /api/v1/*      │                            │                                       │
│  /rpc           │                            │                                       │
└────────┬────────┘                            │                                       │
         │                                     │                                       │
         │  proxy / rutas                      │  sirve HTML desde                      │
         │  platform-routes.js                 │  RuddieSolution/platform/             │
         │  + APIs en server.js                │  server.js (Express)                  │
         │                                     │                                       │
         └─────────────────────────────────────┼───────────────────────────────────────┘
                                               │
                    ┌──────────────────────────┴──────────────────────────┐
                    │  CAPA DE SERVICIOS (cada uno en su puerto)          │
                    │  Config: RuddieSolution/config/services-ports.json  │
                    └──────────────────────────┬─────────────────────────┘
                                               │
    ┌──────────────────────────────────────────┼──────────────────────────────────────────┐
    │              │              │             │              │              │            │
    ▼              ▼              ▼             ▼              ▼              ▼            ▼
 Banking      Platform      Jerarquía      Trading       Oficina      Gobierno   Seguridad  Blockchain
 Bridge       servers       bancaria       (:5054…)      (:5055…)     (:5090…)   (:5120…)   (:5140…)
 :3001        4001-4600     6000-6400
```

- **Usuario** entra por **Node :8545** (la mayoría de rutas) o por **Platform :8080** (Leader, admin, estático).
- **Node 8545** es la puerta principal: sirve HTML de `platform/` vía `platform-routes.js`, expone `/api/v1/*`, `/rpc`, `/health`, y hace proxy a Banking Bridge y otros.
- **Servicios backend** están definidos en `services-ports.json` y cada uno escucha en su puerto; los arranca `start.sh`.

---

## 2. Orquestación: cómo se enciende todo

| Paso | Quién | Qué hace |
|------|--------|----------|
| 0 | `stop-all.sh` (opcional) | Mata procesos en todos los puertos usados (8545, 3001, 8080, 4001-4600, 6000-6400, 5054-5097, etc.). |
| 1 | `start.sh` | Pausa 2 s, instala deps Node si faltan. |
| 2 | `start.sh` | Arranca **Node** (server.js) en **8545** y **Banking Bridge** (banking-bridge.js) en **3001** (o con PM2 si existe). |
| 3 | `start.sh` | Arranca **Platform** (platform/server.js) en **8080**. |
| 4 | `start.sh` | Arranca servicios **multilenguaje** (Rust 8590, Go 8591, Python 8592) vía script en `RuddieSolution/servers/` o stubs. |
| 5 | `start.sh` | Arranca **platform servers** (BDET 4001, TradeX 4002, SIIS 4003, Clearing 4004, bancos 4100-4400, AI Hub 4500, Gov 4600) con `start-all-platforms.sh`. |
| 6 | `start.sh` | Arranca **jerarquía bancaria** (6000-6400) con `banking-hierarchy-server.js` o `start-banking-hierarchy.sh`. |
| 7 | `start.sh` | Arranca **Forex** :5200, **.NET** (Platform API 3000, NET10 5071, Advocate 3010, AppBuilder 5062, IDOFactory 5097, SpikeOffice 5056, RnBCal 5055, TradeX 5054, FarmFactory 5061, ProjectHub 7070, MeetingHub 7071, CitizenCRM 5090, Tax 5091, Voting 5092, ServiceDesk 5093, DocumentFlow 5080, ESignature 5081, OutlookExtractor 5082, BioMetrics 5120, DigitalVault 5121, AI Fraud 5144, DeFi 5140, NFT 5141, DAO 5142, Bridge 5143). |
| 8 | `start.sh` | Arranca **stubs** (License 5094, AI 5300/5301/5302, Rust 8590) si los servicios reales no están. |

**Archivos clave de orquestación:**
- Raíz: `start.sh`, `stop-all.sh`
- RuddieSolution: `RuddieSolution/servers/start-all-platforms.sh`, `start-banking-hierarchy.sh`, `banking-hierarchy-server.js`, `stub-health-server.js`

---

## 3. Configuración (fuentes de verdad)

| Archivo | Qué define |
|---------|------------|
| **RuddieSolution/config/services-ports.json** | Todos los servicios y sus puertos (core, platform_servers, trading, banking_hierarchy, office, government, document, security, blockchain, ai, multilang). Lo usan el Node (service-registry), scripts de test y reportes. |
| **RuddieSolution/node/platform-routes.js** | Rutas URL → archivo HTML. Ej: `/bdet-bank` → `bdet-bank.html`, `/forex` → `forex.html`. El Node (server.js) registra estas rutas y sirve los HTML desde `RuddieSolution/platform/`. |
| **RuddieSolution/platform/data/platform-links.json** | Enlaces de la UI (version-badges, dashboards, headerNav, quickActions, services). Qué se muestra en índices y menús. |
| **RuddieSolution/platform/data/government-departments.json** | Lista de departamentos de gobierno (id, name, symbol, category). 41 actuales, meta 103. |
| **RuddieSolution/node/data/estado-final-sistema.json** | Estado declarado del sistema (módulos, declaración, validación, One Love, tactical). Consumido por APIs y por Atabey/Estado Final. |

---

## 4. Flujo de una petición típica

**Ejemplo: usuario abre “BDET Bank” en el navegador**

1. Usuario pide `http://localhost:8545/bdet-bank`.
2. **Node (8545)** recibe la petición; Express tiene registrada la ruta con `platform-routes.js`: `path: '/bdet-bank'` → `file: 'bdet-bank.html'`.
3. Node hace `res.sendFile(path.join(platformDir, 'bdet-bank.html'))` → se sirve `RuddieSolution/platform/bdet-bank.html`.
4. El HTML puede llamar a APIs: por ejemplo `http://localhost:8545/api/v1/...` o proxy a `http://localhost:3001` (Banking Bridge). Esas APIs están definidas en server.js o en banking-bridge.js.

**Ejemplo: health check de un servicio**

1. Script o monitor pide `http://localhost:4004/health` (Clearing House).
2. El proceso que escucha en **4004** (clearing-house-server.js) responde con JSON `{ status: 'ok', ... }`.
3. No pasa por Node 8545; es conexión directa al puerto del servicio. La lista de URLs de health sale de `services-ports.json`.

---

## 5. Dónde está el código (por capa)

| Capa | Ubicación principal |
|------|----------------------|
| **Puerta única (Node)** | RuddieSolution/node/server.js, platform-routes.js |
| **API bancaria** | RuddieSolution/node/banking-bridge.js |
| **Frontend estático / Leader** | RuddieSolution/platform/server.js, *.html en RuddieSolution/platform/ |
| **Servidores plataforma (Node)** | RuddieSolution/servers/*.js (bdet-bank-server.js, tradex-server.js, siis-settlement-server.js, clearing-house-server.js, central-bank-server.js, ai-hub-server.js, government-portal-server.js) |
| **Jerarquía bancaria** | RuddieSolution/servers/banking-hierarchy-server.js |
| **Trading / DeFi / Oficina / Gobierno** | Carpetas en raíz: TradeX/, NET10/, FarmFactory/, IDOFactory/, RnBCal/, SpikeOffice/, AppBuilder/, ProjectHub/, MeetingHub/, CitizenCRM/, TaxAuthority/, VotingSystem/, ServiceDesk/, DocumentFlow/, ESignature/, OutlookExtractor/, BioMetrics/, DigitalVault/, AIFraudDetection/, DeFiSoberano/, NFTCertificates/, GovernanceDAO/, MultichainBridge/ |
| **Platform API .NET** | platform-dotnet/IERAHKWA.Platform/ |
| **Config y datos** | RuddieSolution/config/services-ports.json, RuddieSolution/platform/data/*.json, RuddieSolution/node/data/*.json |

---

## 6. Datos y persistencia

| Dato | Dónde se guarda |
|------|------------------|
| Cuentas / transacciones bancarias | RuddieSolution/node/data/bdet-bank/*.json, bridge-persistence |
| Estado de tests / reportes | RuddieSolution/node/data/reporte-testing-global.json, estado-final-sistema.json |
| Configuración de servicios | RuddieSolution/config/services-ports.json |
| Departamentos | RuddieSolution/platform/data/government-departments.json |
| Enlaces y menús | RuddieSolution/platform/data/platform-links.json |
| AI Hub / Atabey | RuddieSolution/node/data/ai-hub/, data/atabey/ |
| IPTV, public affairs, etc. | RuddieSolution/node/data/iptv/, public-affairs/ |

Todo es **archivo (JSON)** o bases de datos que cada servicio .NET use por su cuenta; el Node no impone una BD central.

---

## 7. Seguridad en el flujo

- **Node (8545):** Helmet (CSP, cabeceras seguras), CORS configurable, rate-limit, compresión, body parser. Rutas sensibles pueden usar middleware JWT (`middleware/jwt-auth.js`).
- **Banking Bridge (3001):** Lógica bancaria; puede validar y sanitizar inputs.
- **Servicios:** Cada uno expone `/health` o `/api/health`; no comparten sesión con el Node; el Node hace proxy o el frontend llama directo según diseño.
- **Fortress / Phantom:** Honeypots, port knocking, kill-switch (documentado en security-fortress y docs de Atabey).

---

## 8. Resumen en una frase

**El usuario entra por Node :8545 (o Platform :8080); el Node sirve HTML desde `platform/` según `platform-routes.js` y expone APIs; el resto de servicios corren en sus puertos (definidos en `services-ports.json`) y los arranca `start.sh` en orden: Node/Bridge/Platform → multilang → platform servers → banking hierarchy → .NET y stubs.**

Para ver la lista exacta de servicios y puertos, abrir **RuddieSolution/config/services-ports.json**.  
Para ver todas las rutas URL → HTML, abrir **RuddieSolution/node/platform-routes.js**.

---

*Plano de estructura completa · 2026-02-05 · Sovereign Government of Ierahkwa Ne Kanienke*
