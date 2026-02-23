# Referencia rápida – Código Node, blockchain y todo lo demás

**Solo rutas del código propio del proyecto** (sin `node_modules` ni `backups`).

---

## 1. Código Node (principal)

**Raíz:** `RuddieSolution/node/`

| Ruta | Descripción |
|------|-------------|
| **server.js** | Servidor principal, puerto 8545. RPC blockchain, AI Hub, ATABEY, APIs. |
| **banking-bridge.js** | Banco, chat, bankers, trading, puerto 3001. |
| **genesis.json** | Bloque genesis (chainId 77777). |
| **ierahkwa-futurehead-mamey-node.json** | Config del nodo. |
| **ecosystem.config.js** | PM2: server.js + banking-bridge.js. |
| **package.json** | Dependencias Node. |
| **ai/** | ai-orchestrator.js, ai-banker.js, ai-banker-bdet.js, ai-trader.js, ai-integrations.js |
| **ai-hub/** | atabey-master-controller.js, atabey-system.js, data-collector.js, learning-engine.js, project-registry.js, world-intelligence.js, index.js |
| **api/** | ai-code-generator.js, swagger.js |
| **auth/** | auth-system.js, routes.js |
| **modules/** | defense-system.js, sovereign-vpn.js, government-banking.js, check-deposit.js, **defi-lending.js**, **mamey-futures.js**, sovereign-financial-center.js, quantum-encryption.js, tor-integration.js, + más |
| **services/** | email.js, graphql.js, health-monitor.js, kms.js, kyc.js, live-connect-protocol.js, payments.js, pdf-generator.js, sms.js, storage.js, websocket.js |
| **middleware/** | circuit-breaker.js, jwt-auth.js, metrics.js, rate-limit.js |
| **logging/** | centralized-logger.js |
| **routes/** | banking.js |
| **database/** | db-config.js, mongo-schemas.js |
| **public/** | index.html, bdet-accounts.html, check-deposit.html, financial-center.html, kms.html, live-connect.html, mega-dashboard.html, system-health.html, treasury-dashboard.html |
| **data/** | ai-hub/, bdet-bank/, JSON de auditoría y depósitos |

---

## 2. Código blockchain

### En Node (server.js)

- **Estado:** `state` (chainId 77777, blockNumber, blocks, transactions, accounts, tokens, gasPrice, gasLimit).
- **RPC:** eth_chainId, eth_blockNumber, eth_gasPrice, eth_getBalance, eth_getTransactionCount, eth_getBlockByNumber, eth_sendTransaction, eth_getTransactionReceipt, etc.
- **Genesis:** `genesis.json` + `initGenesis()` en server.js.
- **Tokens:** `loadTokens()` desde carpeta `tokens/`.

### Mamey (Rust + C# + SDKs)

**Raíz:** `Mamey/`

| Ruta | Descripción |
|------|-------------|
| **core/MameyNode/** | Nodo Rust: Cargo.toml, src/main.rs |
| **core/MameyNode/src/blockchain/** | account.rs, block.rs, state.rs, token.rs, transaction.rs |
| **core/MameyFramework/Blockchain/** | MameyNodeClient.cs, IBlockchainClient.cs |
| **sdks/go/** | client.go |
| **sdks/python/** | mamey_sdk/ |
| **sdks/typescript/** | src/index.ts |
| **services/** | compliance, identity, notifications, treasury, workflows (C#) |
| **start-mamey.sh**, **stop-mamey.sh** | Scripts arranque/parada |

### DeFi Soberano (Solidity + C#)

**Raíz:** `DeFiSoberano/`

| Ruta | Descripción |
|------|-------------|
| **contracts/** | IerahkwaToken.sol, SovereignGovernance.sol, SovereignStaking.sol, SovereignToken.sol, SovereignVault.sol |
| **hardhat.config.js**, **scripts/deploy.js** | Hardhat |
| **DeFi.API/**, **DeFi.Core/**, **DeFi.Infrastructure/** | API y servicios C# |

### Otros blockchain

| Ruta | Descripción |
|------|-------------|
| **MultichainBridge/** | C# |
| **NFTCertificates/** | NFT / certificados |
| **GovernanceDAO/** | Governance DAO |

---

## 3. Todo lo demás

### Plataforma web (HTML)

| Ruta | Descripción |
|------|-------------|
| **RuddieSolution/platform/** | Dashboards HTML (ai-hub, atabey, bdet, security-fortress, leader-control, siis-settlement, etc.) |
| **IERAHKWA_PLATFORM_V1.html**, **IERAHKWA_FOREX_INVESTMENT.html**, **mega-dashboard.html** | Páginas raíz |

### Backend .NET (C#)

| Ruta | Descripción |
|------|-------------|
| **NET10/** | API .NET 10 – banking, swap, pools, farming |
| **TradeX/** | Trading, staking, wallet |
| **FarmFactory/** | Staking, farms, rewards |
| **platform-dotnet/** | API .NET |
| **AssetTracker**, **AuditTrail**, **BudgetControl**, **CitizenCRM**, **ContractManager**, **DataHub**, **DigitalVault**, **DocumentFlow**, **ESignature**, **FormBuilder**, **NotifyHub**, **ProcurementHub**, **ReportEngine**, **ServiceDesk**, **TaxAuthority**, **VotingSystem**, **WorkflowEngine** | Gov operations |
| **AdvocateOffice**, **AppBuilder**, **BioMetrics**, **HRM**, **SmartSchool**, **SpikeOffice**, etc. | Otros |

### Otros servicios Node

| Ruta | Descripción |
|------|-------------|
| **forex-trading-server/** | Node, public/ |
| **ierahkwa-shop/** | E-commerce |
| **pos-system/** | Point of sale |
| **smart-school-node/** | Node, EJS |
| **inventory-system/** | Node, EJS |
| **server/** | Node genérico |

### Mobile / frontend

| Ruta | Descripción |
|------|-------------|
| **mobile-app/** | React Native (.tsx, .ts) |

### Tokens

| Ruta | Descripción |
|------|-------------|
| **tokens/** | Carpetas 01-IGT-PM, 02-IGT-MFA, …; token.json, whitepaper |

### Scripts / DevOps / docs

| Ruta | Descripción |
|------|-------------|
| **scripts/** | start.sh, deploy-a-servidores-fisicos.sh, backup-agentes.sh, ejecutar-en-servidor-fisico.sh |
| **systemd/** | Servicios systemd |
| **kubernetes/** | YAML K8s |
| **docker-compose.yml** | Docker |
| **docs/** | Documentación .md, .html, openapi.yaml |
| **backup/** | Backups (agentes, IGT, platform, etc.) |

---

## Resumen en una línea

- **Node:** `RuddieSolution/node/` (server.js 8545, banking-bridge.js 3001, ai/, ai-hub/, modules/, services/, genesis.json).
- **Blockchain:** En `server.js` (state, RPC); `Mamey/` (Rust + C# + SDKs); `DeFiSoberano/` (Solidity + C#); MultichainBridge, NFTCertificates, GovernanceDAO.
- **Todo lo demás:** RuddieSolution/platform/, NET10, TradeX, FarmFactory, forex-trading-server, ierahkwa-shop, pos-system, mobile-app, tokens/, scripts/, docs/, backup/.

---

*Para más detalle ver: **MAPA-CODIGO-NODE-BLOCKCHAIN-Y-DEMAS.md**.*
