# IERAHKWA FUTUREHEAD - MAPA DE PUERTOS Y SERVICIOS

Cada servicio tiene su **PUERTO DEDICADO** - No repetir para evitar conflictos.

---

## PLATFORM SERVERS (Independientes)

| Puerto | Plataforma | Script |
|--------|------------|--------|
| **4001** | BDET Bank | `servers/bdet-bank-server.js` |
| **4002** | TradeX Exchange | `servers/tradex-server.js` |
| **4003** | SIIS Settlement | `servers/siis-settlement-server.js` |
| **4004** | Clearing House | `servers/clearing-house-server.js` |
| **4100** | Banco Águila (Norte) | `servers/central-bank-server.js` |
| **4200** | Banco Quetzal (Centro) | `servers/central-bank-server.js` |
| **4300** | Banco Cóndor (Sur) | `servers/central-bank-server.js` |
| **4400** | Banco Caribe (Taínos) | `servers/central-bank-server.js` |
| **4500** | AI Hub / ATABEY | `servers/ai-hub-server.js` |
| **4600** | Government Portal | `servers/government-portal-server.js` |

**Iniciar todos:** `cd servers && ./start-all-platforms.sh`
**Detener todos:** `cd servers && ./stop-all-platforms.sh`

---

## CORE SERVICES (Siempre activos)

| Puerto | Servicio | Script | Health |
|--------|----------|--------|--------|
| **8545** | Node Mamey (Blockchain) | `node/server.js` | `/health` |
| **3001** | Banking Bridge API | `node/banking-bridge.js` | `/api/health` |
| **5000** | Banking .NET API | `IerahkwaBanking.NET10` | `/health` |
| **8080** | Platform Frontend | `platform/server.js` | `/health` |

---

## TRADING & EXCHANGE

| Puerto | Servicio | Descripción |
|--------|----------|-------------|
| **5054** | TradeX | Exchange principal |
| **5071** | NET10 DeFi | Plataforma DeFi |
| **5061** | FarmFactory | Yield Farming |
| **5097** | IDOFactory | Token Launches |
| **5200** | Forex Trading | Forex |

---

## BANKING HIERARCHY (Jerarquía Bancaria)

```
IERAHKWA FUTUREHEAD INTERNATIONAL SETTLEMENT (SIIS) - Puerto 6000
    │
    ├── Clearing House - Puerto 6001
    ├── Global Services - Puerto 6002
    ├── Receiver Bank - Puerto 6003
    │
    └── BDET Master - Puerto 6010
            │
            ├── 🦅 ÁGUILA (Norte) - Puerto 6100
            │       ├── Regional Norte  - 6101
            │       ├── Regional Sur    - 6102
            │       ├── Regional Este   - 6103
            │       └── Regional Oeste  - 6104
            │
            ├── 🐦 QUETZAL (Centro) - Puerto 6200
            │       ├── Regional Norte  - 6201
            │       ├── Regional Sur    - 6202
            │       ├── Regional Este   - 6203
            │       └── Regional Oeste  - 6204
            │
            ├── 🦅 CÓNDOR (Sur) - Puerto 6300
            │       ├── Regional Norte  - 6301
            │       ├── Regional Sur    - 6302
            │       ├── Regional Este   - 6303
            │       └── Regional Oeste  - 6304
            │
            └── 🌴 CARIBE (Taínos) - Puerto 6400
                    ├── Regional Norte  - 6401
                    ├── Regional Sur    - 6402
                    ├── Regional Este   - 6403
                    └── Regional Oeste  - 6404
```

---

## OFFICE SERVICES

| Puerto | Servicio |
|--------|----------|
| **5055** | RnBCal (Calendar) |
| **5056** | SpikeOffice |
| **5060** | AppBuilder |
| **7070** | ProjectHub |
| **7071** | MeetingHub |

---

## GOVERNMENT SERVICES

| Puerto | Servicio |
|--------|----------|
| **5090** | CitizenCRM |
| **5091** | TaxAuthority |
| **5092** | VotingSystem |
| **5093** | ServiceDesk |
| **5094** | License Authority |

---

## DOCUMENT & SECURITY

| Puerto | Servicio |
|--------|----------|
| **5080** | DocumentFlow |
| **5081** | ESignature |
| **5082** | OutlookExtractor |
| **5120** | BioMetrics |
| **5121** | DigitalVault |
| **5144** | AI Fraud Detection |

---

## BLOCKCHAIN & DEFI

| Puerto | Servicio |
|--------|----------|
| **5140** | DeFi Soberano |
| **5141** | NFT Certificates |
| **5142** | Governance DAO |
| **5143** | Multichain Bridge |

---

## AI SERVICES

| Puerto | Servicio |
|--------|----------|
| **5300** | AI Hub / ATABEY |
| **5301** | AI Banker BDET |
| **5302** | AI Trader |

---

## MULTI-LANGUAGE SERVICES

| Puerto | Servicio |
|--------|----------|
| **8590** | Rust SWIFT Service |
| **8591** | Go Queue Service |
| **8592** | Python ML Service |

---

## DATABASES

| Puerto | Servicio |
|--------|----------|
| **5432** | PostgreSQL |
| **6379** | Redis |
| **27017** | MongoDB |

---

## COMANDOS

```bash
# Iniciar todos los servicios (separados)
./start-all-separated.sh

# Detener todos
./stop-all.sh

# Ver configuración de puertos
cat config/services-ports.json

# Verificar servicios
./scripts/test-live.sh
```

---

## REGLA IMPORTANTE

**NUNCA usar el mismo puerto para dos servicios diferentes.**

Cada servicio tiene su puerto asignado en `config/services-ports.json`.
