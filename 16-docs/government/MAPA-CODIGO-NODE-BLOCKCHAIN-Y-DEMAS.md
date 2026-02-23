# Mapa del código – Node, blockchain y todo lo demás
## Dónde está cada tipo de código en el proyecto

**Fecha:** 28 de Enero de 2026  
**Gobierno Soberano de Ierahkwa Ne Kanienke**

> **Índice completo de código (todas las rutas y módulos):** ver **`INDICE-CODIGO-PROYECTO.md`** en la raíz del proyecto.

---

## 1. CÓDIGO NODE (principal)

**Ubicación:** `RuddieSolution/node/`

| Archivo / carpeta | Qué es |
|-------------------|--------|
| **server.js** | Servidor principal (puerto 8545). Carga AI Hub, ATABEY, módulos, RPC blockchain, APIs. |
| **banking-bridge.js** | Banco, chat, bankers, trading, 365+ endpoints (puerto 3001). |
| **ai/** | ai-orchestrator.js, ai-banker.js, ai-banker-bdet.js, ai-trader.js, ai-integrations.js |
| **ai-hub/** | atabey-master-controller.js, atabey-system.js, data-collector.js, learning-engine.js, project-registry.js, world-intelligence.js, index.js |
| **api/** | ai-code-generator.js, swagger.js |
| **auth/** | auth-system.js, routes.js |
| **modules/** | defense-system.js, sovereign-vpn.js, multi-hop-vpn.js, government-banking.js, check-deposit.js, defi-lending.js, mamey-futures.js, sovereign-ai.js, sovereign-financial-center.js, quantum-encryption.js, tor-integration.js, + 20 más |
| **services/** | email.js, graphql.js, health-monitor.js, kms.js, kyc.js, live-connect-protocol.js, payments.js, pdf-generator.js, sms.js, storage.js, websocket.js, etc. |
| **middleware/** | circuit-breaker.js, jwt-auth.js, metrics.js, rate-limit.js |
| **logging/** | centralized-logger.js |
| **ghost/** | ghost-mode.js |
| **mega/** | sovereign-cloud.js |
| **telecom/** | satellite-system.js |
| **websocket/** | handlers.js |
| **routes/** | banking.js |
| **database/** | db-config.js, mongo-schemas.js |
| **public/** | index.html, bdet-accounts.html, check-deposit.html, financial-center.html, kms.html, live-connect.html, mega-dashboard.html, system-health.html, treasury-dashboard.html |
| **data/** | ai-hub/, bdet-bank/, JSON de auditoría y depósitos |
| **genesis.json** | Bloque genesis (chainId 77777, gasLimit, alloc). |
| **ierahkwa-futurehead-mamey-node.json** | Config del nodo. |
| **ecosystem.config.js** | PM2: server.js (8545) + banking-bridge.js (3001). |
| **package.json** | Dependencias Node. |

---

## 2. CÓDIGO BLOCKCHAIN

### 2.1 En Node (server.js)

| Qué | Dónde |
|-----|--------|
| **Estado blockchain** | server.js: state (chainId 77777, blockNumber, blocks, transactions, accounts, tokens, gasPrice, gasLimit). |
| **RPC Ethereum-style** | server.js: eth_chainId, eth_blockNumber, eth_gasPrice, eth_getBalance, eth_getTransactionCount, eth_getBlockByNumber, eth_sendTransaction, eth_getTransactionReceipt, etc. |
| **Genesis** | server.js carga genesis.json; initGenesis() crea bloque 0. |
| **Tokens** | server.js: loadTokens() desde carpeta tokens/ (token.json por IGT). |
| **HTTP RPC** | server.js: POST /rpc con JSON-RPC (method, params). |

### 2.2 Mamey (Rust / C# / Go / Python / TypeScript)

**Ubicación:** `Mamey/`

| Qué | Dónde |
|-----|--------|
| **Nodo Rust** | Mamey/core/MameyNode/ (Cargo.toml, src/main.rs, blockchain/, consensus/, crypto/, network/, storage/). |
| **Blockchain (Rust)** | Mamey/core/MameyNode/src/blockchain/ (account.rs, block.rs, state.rs, token.rs, transaction.rs). |
| **Framework C#** | Mamey/core/MameyFramework/ (Blockchain/MameyNodeClient.cs, Core/). |
| **SDKs** | Mamey/sdks/go/, Mamey/sdks/python/, Mamey/sdks/typescript/. |
| **Servicios C#** | Mamey/services/ (compliance, identity, notifications, treasury, workflows). |
| **Scripts** | Mamey/start-mamey.sh, Mamey/stop-mamey.sh. |

### 2.3 DeFi / Smart contracts (Solidity)

**Ubicación:** `DeFiSoberano/`

| Qué | Dónde |
|-----|--------|
| **Contratos Solidity** | DeFiSoberano/contracts/ (IerahkwaToken.sol, SovereignGovernance.sol, SovereignStaking.sol, SovereignToken.sol, SovereignVault.sol). |
| **Hardhat** | DeFiSoberano/hardhat.config.js, scripts/deploy.js. |
| **API DeFi** | DeFiSoberano/DeFi.API/, DeFi.Core/, DeFi.Infrastructure/ (C#). |

### 2.4 Otros blockchain

| Qué | Dónde |
|-----|--------|
| **MultichainBridge** | MultichainBridge/ (C#). |
| **NFT / Certificados** | NFTCertificates/. |
| **Governance DAO** | GovernanceDAO/. |

---

## 3. TODO LO DEMÁS

### 3.1 Plataforma web (HTML / dashboards)

| Qué | Dónde |
|-----|--------|
| **Platform** | RuddieSolution/platform/ (HTML de dashboards: ai-hub, atabey, bdet, security-fortress, leader-control, siis-settlement, etc.). |
| **Páginas raíz** | IERAHKWA_PLATFORM_V1.html, IERAHKWA_FOREX_INVESTMENT.html, mega-dashboard.html. |

### 3.2 Backend .NET (C#)

| Qué | Dónde |
|-----|--------|
| **NET10** | NET10/ (API .NET 10 – banking, swap, pools, farming). |
| **TradeX** | TradeX/ (trading, staking, wallet). |
| **FarmFactory** | FarmFactory/ (staking, farms, rewards). |
| **platform-dotnet** | platform-dotnet/ (API .NET). |
| **Gov operations** | AssetTracker, AuditTrail, BudgetControl, CitizenCRM, ContractManager, DataHub, DigitalVault, DocumentFlow, ESignature, FormBuilder, NotifyHub, ProcurementHub, ReportEngine, ServiceDesk, TaxAuthority, VotingSystem, WorkflowEngine. |
| **Otros** | AdvocateOffice, AppBuilder, BioMetrics, HRM, SmartSchool, SpikeOffice, etc. |

### 3.3 Servicios Node (otros puertos)

| Qué | Dónde |
|-----|--------|
| **forex-trading-server** | forex-trading-server/ (Node, public/). |
| **ierahkwa-shop** | ierahkwa-shop/ (e-commerce). |
| **pos-system** | pos-system/ (point of sale). |
| **smart-school-node** | smart-school-node/ (Node, EJS). |
| **inventory-system** | inventory-system/ (Node, EJS). |
| **server/** | server/ (Node genérico). |

### 3.4 Mobile / frontend

| Qué | Dónde |
|-----|--------|
| **mobile-app** | mobile-app/ (React Native, .tsx, .ts). |

### 3.5 Tokens

| Qué | Dónde |
|-----|--------|
| **tokens/** | tokens/ (carpetas 01-IGT-PM, 02-IGT-MFA, …; token.json, whitepaper, etc.). |

### 3.6 Scripts / DevOps / docs

| Qué | Dónde |
|-----|--------|
| **scripts/** | start.sh, deploy-a-servidores-fisicos.sh, backup-agentes.sh, ejecutar-en-servidor-fisico.sh, etc. |
| **systemd/** | Servicios systemd. |
| **kubernetes/** | YAML K8s. |
| **docker-compose.yml** | Docker. |
| **docs/** | Documentación .md, .html, openapi.yaml. |
| **backup/** | Backups (agentes, IGT, platform, pos-system, SmartSchool, etc.). |

### 3.7 Otros

| Qué | Dónde |
|-----|--------|
| **ai/** | ai/ (HTML, .md en raíz). |
| **auto-backup/** | auto-backup.sh, config. |
| **tests/** | tests/ (C#). |
| **products/** | products/ (docs, .cs). |
| **quantum/** | quantum/. |
| **image-upload/** | image-upload/. |

---

## 4. RESUMEN EN UN DIBUJO

```
  CÓDIGO NODE (principal)
  RuddieSolution/node/
  ├── server.js          (8545 – RPC blockchain, AI, módulos, APIs)
  ├── banking-bridge.js  (3001 – banco, chat, bankers, trading)
  ├── genesis.json      (bloque genesis chainId 77777)
  ├── ai/, ai-hub/, api/, auth/, modules/, services/, middleware/, data/, public/

  CÓDIGO BLOCKCHAIN
  ├── En server.js       (state, eth_*, loadTokens, genesis)
  ├── Mamey/             (Rust node, C# framework, SDKs Go/Python/TS, servicios C#)
  ├── DeFiSoberano/      (Solidity contracts, Hardhat, API DeFi C#)
  ├── MultichainBridge/, NFTCertificates/, GovernanceDAO/

  TODO LO DEMÁS
  ├── RuddieSolution/platform/   (HTML dashboards)
  ├── NET10/, TradeX/, FarmFactory/, platform-dotnet/  (.NET)
  ├── forex-trading-server/, ierahkwa-shop/, pos-system/, smart-school-node/, inventory-system/
  ├── mobile-app/   (React Native)
  ├── tokens/       (IGT token.json, whitepapers)
  ├── scripts/, systemd/, kubernetes/, docker-compose.yml, docs/, backup/
  ├── AssetTracker, AuditTrail, CitizenCRM, SmartSchool, HRM, … (Gov .NET)
```

---

*Documento de referencia. Node, blockchain y todo lo demás – IERAHKWA.*
