# Índice de todo el código del proyecto

**Sovereign Government of Ierahkwa Ne Kanienke**  
Referencia única para localizar todo el código: Node, .NET (C#), Rust, Solidity, HTML/JS, scripts y demás.

> Detalle de Node, blockchain y flujos: ver **`MAPA-CODIGO-NODE-BLOCKCHAIN-Y-DEMAS.md`**.

---

## Resumen por lenguaje / stack

| Lenguaje / stack | Ubicaciones principales |
|------------------|-------------------------|
| **Node.js** | `RuddieSolution/node/`, `RuddieSolution/platform/api/`, `RuddieSolution/servers/`, `ierahkwa-shop/`, `pos-system/`, `smart-school-node/`, `inventory-system/`, `forex-trading-server/`, `server/`, `image-upload/` |
| **C# / .NET** | `NET10/`, `TradeX/`, `FarmFactory/`, `platform-dotnet/`, `Mamey/` (servicios), Gov: `AssetTracker/`, `AuditTrail/`, `CitizenCRM/`, etc. |
| **Rust** | `Mamey/core/MameyNode/` (nodo blockchain) |
| **Solidity** | `DeFiSoberano/contracts/` |
| **HTML / JS / CSS** | `RuddieSolution/platform/*.html`, `RuddieSolution/platform/assets/*.js`, `RuddieSolution/node/public/*.html` |
| **Shell / Bash** | `scripts/*.sh`, `start.sh`, `status.sh`, raíz y módulos |
| **TypeScript / React** | `mobile-app/` |
| **Python** | `scripts/install-python-ml.sh`, referencias en docs |
| **EJS** | `inventory-system/`, `smart-school-node/` |

---

## 1. Node.js

### 1.1 Servidor principal (puerto 8545 y 3001)

| Ruta | Descripción |
|------|-------------|
| **RuddieSolution/node/server.js** | Servidor principal: RPC blockchain, AI Hub, ATABEY, APIs, módulos. |
| **RuddieSolution/node/banking-bridge.js** | Banco, chat, bankers, trading; cientos de endpoints. |
| **RuddieSolution/node/ecosystem.config.js** | PM2: server.js (8545) + banking-bridge.js (3001). |
| **RuddieSolution/node/genesis.json** | Bloque genesis (chainId 77777). |

### 1.2 API y rutas

| Ruta | Descripción |
|------|-------------|
| **RuddieSolution/node/api/ai-code-generator.js** | `/api/ai` — código, chat, analyze; perfiles full/fast. |
| **RuddieSolution/node/api/ai-studio.js** | `/api/ai-studio` — generación código, web, app, api, bot. |
| **RuddieSolution/node/routes/banking.js** | Rutas banking. |
| **RuddieSolution/node/routes/casino-api.js** | API casino. |
| **RuddieSolution/node/routes/iptv-api.js** | API IPTV. |
| **RuddieSolution/node/routes/kyc-api.js** | API KYC. |
| **RuddieSolution/node/routes/platform-auth.js** | Autenticación plataforma. |
| **RuddieSolution/node/routes/platforms-api.js** | APIs de plataformas. |
| **RuddieSolution/node/routes/sicb-mamey-stubs.js** | Stubs SICB/Mamey. |

### 1.3 AI y ATABEY

| Ruta | Descripción |
|------|-------------|
| **RuddieSolution/node/ai-hub/** | atabey-master-controller.js, atabey-system.js, data-collector.js, learning-engine.js, project-registry.js, world-intelligence.js, index.js. |
| **RuddieSolution/node/ai/** | ai-orchestrator.js, ai-banker.js, ai-banker-bdet.js, ai-trader.js, ai-integrations.js. |
| **RuddieSolution/node/services/ai-soberano.js** | Ollama local; perfiles full (modelo 6) y fast (modelo 9); getProfileForPlatform. |
| **RuddieSolution/node/services/anomaly-ai.js** | Detección de anomalías. |

### 1.4 Módulos de negocio

| Ruta | Descripción |
|------|-------------|
| **RuddieSolution/node/modules/** | defense-system, sovereign-vpn, multi-hop-vpn, government-banking, check-deposit, defi-lending, mamey-futures, marketplace, quantum-encryption, tor-integration, sovereign-ai, sovereign-financial-center, recurring-collections-api, revenue-aggregator, whistleblower, zkp-privacy, premium-pro, streaming-platform, social-network, gaming-platform, metaverse, membership-referral, monetization-engine, platform-revenue, y más. |

### 1.5 Servicios e infra

| Ruta | Descripción |
|------|-------------|
| **RuddieSolution/node/services/** | email.js, email-soberano.js, sms.js, sms-soberano.js, kms.js, kyc.js, payments.js, pdf-generator.js, health-monitor.js, service-registry.js, bridge-persistence.js, face-recognition-propio.js, provider-soberano.js, storage-soberano.js, pagos-soberano.js, recurring-collections.js, index-soberano.js, etc. |
| **RuddieSolution/node/middleware/** | jwt-auth.js, rate-limit.js, circuit-breaker.js, metrics.js. |
| **RuddieSolution/node/logging/** | centralized-logger.js. |
| **RuddieSolution/node/ghost/** | ghost-mode.js. |
| **RuddieSolution/node/telecom/** | satellite-system.js, citizen-phone-numbers.js, communication-network.js, internet-propio.js, signal-mobile.js, smart-cell.js, voip-telephone.js. |
| **RuddieSolution/node/database/** | db-config.js, mongo-schemas.js. |
| **RuddieSolution/node/websocket/** | handlers.js. |

### 1.6 Otros servicios Node

| Ruta | Descripción |
|------|-------------|
| **RuddieSolution/platform/api/** | platform-api.js, editor-api.js, dashboard-api.js (API del front platform). |
| **RuddieSolution/servers/** | services-runner.js, package.json. |
| **ierahkwa-shop/** | E-commerce Node. |
| **pos-system/** | Punto de venta Node. |
| **smart-school-node/** | Escuela Node + EJS. |
| **inventory-system/** | Inventario Node + EJS. |
| **forex-trading-server/** | Servidor forex. |
| **server/** | Servidor Node genérico. |
| **image-upload/** | Subida de imágenes. |

---

## 2. C# / .NET

### 2.1 Finanzas y trading

| Ruta | Descripción |
|------|-------------|
| **NET10/** | API .NET 10 — banking, swap, pools, farming. |
| **TradeX/** | Trading, staking, wallet. |
| **FarmFactory/** | Staking, farms, rewards. |
| **RuddieSolution/IerahkwaBanking.NET10/** | Banking.Auth, Banking.API, Banking.Core, Banking.Infrastructure, Banking.Blockchain, Banking.MameyNode. |

### 2.2 Gobierno y operaciones

| Ruta | Descripción |
|------|-------------|
| **AssetTracker/** | Gestión de activos. |
| **AuditTrail/** | Auditoría. |
| **BudgetControl/** | Control presupuestario. |
| **CitizenCRM/** | CRM ciudadanos. |
| **ContractManager/** | Contratos. |
| **DataHub/** | Data warehouse. |
| **DigitalVault/** | Bóveda digital. |
| **DocumentFlow/** | Flujo documental. |
| **ESignature/** | Firma electrónica. |
| **FormBuilder/** | Formularios. |
| **NotifyHub/** | Notificaciones. |
| **ProcurementHub/** | Compras/licitaciones. |
| **ReportEngine/** | Reportes. |
| **ServiceDesk/** | Mesa de ayuda. |
| **TaxAuthority/** | Autoridad fiscal. |
| **VotingSystem/** | Votación. |
| **WorkflowEngine/** | Motor de flujos. |

### 2.3 Otros .NET

| Ruta | Descripción |
|------|-------------|
| **platform-dotnet/** | API .NET plataforma. |
| **AdvocateOffice/** | Oficina abogacía. |
| **AppBuilder/** | Constructor de apps. |
| **AppointmentHub/** | Citas/reuniones. |
| **BioMetrics/** | Biometría. |
| **DeFiSoberano/DeFi.API, DeFi.Core, DeFi.Infrastructure** | API DeFi C#. |
| **AIFraudDetection/** | Detección fraude. |
| **GovernanceDAO/** | DAO. |
| **MultichainBridge/** | Puente multichain. |
| **NFTCertificates/** | NFT/certificados. |
| **HRM/** | RRHH. |
| **SmartSchool/** | Educación. |
| **SpikeOffice/** | Oficina. |
| **MeetingHub/** | Reuniones. |
| **Mamey/services/** | Servicios C# (compliance, identity, notifications, treasury, workflows). |
| **MAMEY-FUTURES/** | Futures C#. |

---

## 3. Rust

| Ruta | Descripción |
|------|-------------|
| **Mamey/core/MameyNode/** | Nodo blockchain (Cargo.toml, src/, blockchain/, consensus/, crypto/, network/, storage/). |

---

## 4. Solidity y blockchain

| Ruta | Descripción |
|------|-------------|
| **DeFiSoberano/contracts/** | IerahkwaToken.sol, SovereignGovernance.sol, SovereignStaking.sol, SovereignToken.sol, SovereignVault.sol. |
| **DeFiSoberano/** | hardhat.config.js, scripts/deploy.js, API DeFi C#. |

---

## 5. Frontend: HTML, JS, CSS

### 5.1 Plataforma (dashboards y páginas)

| Ruta | Descripción |
|------|-------------|
| **RuddieSolution/platform/*.html** | atabey-platform.html, security-fortress.html, leader-control.html, ai-platform.html, support-ai.html, citizen-portal.html, bdet-bank (vía URL), government-portal.html, admin.html, monitor.html, backup-department.html, casino.html, forex.html, voting.html, wallet.html, vip-transactions.html, video-call.html, secure-chat.html, notifications.html, whistleblower.html, watchlist-alerta-proteccion.html, y decenas de páginas más. |
| **RuddieSolution/platform/assets/** | unified-core.js, unified-styles.css, platform-registry.js, platform-buttons.js, platform-api-client.js, auth-session.js, security-layer.js, notifications.js, unified-header.js, unified-load.js, api-client.js, platform-apis.js, platform-gate.js, tenant-context.js, todos-juntos.js, etc. |
| **RuddieSolution/platform/api/** | platform-api.js, editor-api.js, dashboard-api.js (servidores Express locales para el front). |

### 5.2 Páginas servidas por Node

| Ruta | Descripción |
|------|-------------|
| **RuddieSolution/node/public/*.html** | index.html, bdet-accounts.html, check-deposit.html, financial-center.html, kms.html, live-connect.html, mega-dashboard.html, system-health.html, treasury-dashboard.html, etc. |

### 5.3 Raíz

| Ruta | Descripción |
|------|-------------|
| **IERAHKWA_PLATFORM_V1.html**, **IERAHKWA_FOREX_INVESTMENT.html**, **mega-dashboard.html** | Páginas en raíz. |

---

## 6. Scripts (Shell, Node)

| Ruta | Descripción |
|------|-------------|
| **scripts/instalar-ollama.sh** | Instalar Ollama y modelos 6 y 9. |
| **scripts/backup-agentes.sh**, **backup-platforms.sh**, **backup-soberano.sh**, **backup-todas-plataformas.sh** | Backups. |
| **scripts/status-soberano.sh** | Estado servicios soberanos. |
| **scripts/deploy.sh**, **deploy-a-servidores-fisicos.sh**, **ejecutar-en-servidor-fisico.sh** | Despliegue. |
| **scripts/install-cron-production.sh**, **install-cron-colas.sh**, **install-backup-agentes-cron.sh**, **setup-cron-backups.sh** | Cron y producción. |
| **scripts/health-alert-check.sh**, **production-ready-check.sh**, **verify-24-7.sh**, **test-live.sh** | Comprobaciones. |
| **scripts/setup-ssl.sh** | SSL. |
| **scripts/install-python-ml.sh** | Python/ML. |
| **scripts/auditar-soberania.js**, **procesar-colas.sh** | Auditoría y colas. |
| **start.sh**, **status.sh**, **stop-all.sh** (raíz) | Arranque y estado global. |

---

## 7. Mobile y otros

| Ruta | Descripción |
|------|-------------|
| **mobile-app/** | React Native (.tsx, .ts). |
| **tokens/** | token.json por IGT, whitepapers (datos y algo de HTML). |
| **docs/** | .md, .html, openapi.yaml (documentación). |
| **kubernetes/** | YAML K8s. |
| **systemd/** | Servicios systemd. |
| **tests/** | Tests C#. |
| **quantum/** | Código/HTML quantum. |
| **ai/** | HTML y .md. |

---

## 8. Documentos de referencia

| Documento | Contenido |
|-----------|-----------|
| **MAPA-CODIGO-NODE-BLOCKCHAIN-Y-DEMAS.md** | Mapa detallado Node, blockchain, “todo lo demás”. |
| **docs/INDEX-DOCUMENTACION.md** | Índice de documentación por módulo. |
| **REFERENCIA-RAPIDA-CODIGO.md** | Referencia rápida de código. |
| **PUERTOS-SERVICIOS.md** | Puertos por servicio. |

---

*Índice de todo el código del proyecto — Ierahkwa. Actualizar cuando se añadan módulos o rutas nuevas.*
