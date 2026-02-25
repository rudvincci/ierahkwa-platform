# IERAHKWA PLATFORM — COMPLETE CODEBASE AUDIT v2.8.0
## 24 February 2026

---

## 1. EXECUTIVE SUMMARY

| Metric | Value |
|--------|-------|
| **Total Repository Size** | 115 MB |
| **Total Files** | 18,356 |
| **Total Directories** | 21 top-level |
| **Programming Languages** | 15+ |
| **C# Projects (.csproj)** | 979 |
| **Solution Files (.sln)** | 167 |
| **Docker Files** | 103 Dockerfiles + 94 docker-compose |
| **HTML Platforms** | 190 (179 individual + 10 NEXUS + 1 Portal Central) |
| **Backend Services (Node.js)** | 20 |
| **.NET Microservices** | 41 |
| **Blockchain Services (FWID)** | 4 sub-services (730 C# files) |
| **Rust Crates** | 2 (mamey-node-rust + mamey-forge) |
| **SDKs** | 3 (Go, Python, TypeScript) |
| **Git Version** | v2.8.0 (commit b9834683) |

---

## 2. FILES BY LANGUAGE

| Language | Files | Purpose |
|----------|-------|---------|
| C# (.cs) | 9,365 | Backend microservices, banking, government, blockchain |
| Markdown (.md) | 1,405 | Documentation, whitepapers, technical specs |
| JSON (.json) | 1,180 | Configuration, package definitions, API schemas |
| HTML (.html) | 481 | Platform UIs, dashboards, portals |
| Shell (.sh/.bash) | 418 | DevOps scripts, deployment, backups |
| JavaScript (.js) | 380 | Backend services, tests, smart contracts |
| YAML (.yml/.yaml) | 255 | CI/CD, Docker, Kubernetes, configs |
| CSS (.css) | 211 | Stylesheets, design system |
| TypeScript (.ts) | 99 | SDKs, dashboard components |
| Python (.py) | 58 | AI, utilities, solution scripts |
| Protocol Buffers (.proto) | 51 | gRPC service definitions |
| Rust (.rs) | 42 | Blockchain node, smart contract forge |
| Cargo.toml | 10 | Rust project files |
| Solidity (.sol) | 9 | Smart contracts (ERC-20, Governance, Vault) |
| SQL (.sql) | 9 | Database migrations |
| Go (.go) | 6 | MameyNode SDK |
| package.json | 28 | Node.js project files |

---

## 3. DIRECTORY STRUCTURE & SIZE

| Directory | Size | Files | Description |
|-----------|------|-------|-------------|
| `08-dotnet/` | 17 MB | 13,196 | .NET ecosystem (microservices, banking, government, framework, UI) |
| `14-blockchain/` | 6.0 MB | 1,361 | FWID saga, quantum, tokens |
| `17-files-originales/` | 3.0 MB | 206 | Original files archive |
| `02-plataformas-html/` | 2.6 MB | 194 | Sovereign HTML platform UIs |
| `06-dashboards/` | 2.5 MB | 227 | Maestro dashboard + command center |
| `03-backend/` | 2.5 MB | 335 | Node.js backend services |
| `pitch/` | 1.4 MB | 89 | Investor pitch materials |
| `15-utilities/` | 856 KB | 1,027 | BaGetter, Barcode, Template Engine, BookStack |
| `16-docs/` | 708 KB | 410 | Extended documentation |
| `04-infraestructura/` | 276 KB | 39 | Docker, Kubernetes, Helm, Terraform |
| `12-rust/` | 200 KB | 72 | mamey-forge CLI + mamey-node-rust SDK |
| `13-ai/` | 180 KB | 21 | AI agent, code generator, future AI |
| `10-core/` | 76 KB | 24 | MameyNode (Rust), MameyFramework (C#), LockSlot, TemplateEngine |
| `07-scripts/` | 60 KB | 25 | DevOps: deploy, backup, security, SAST/SCA scanners |
| `11-sdks/` | 52 KB | 12 | Go SDK, Python SDK, TypeScript SDK |
| `mvp-voz-soberana/` | 40 KB | 6 | Voice sovereignty MVP |
| `e2e/` | 16 KB | 5 | End-to-end tests |
| `05-api/` | 4 KB | 4 | OpenAPI spec + proto contracts |
| `01-documentos/` | — | 62 | Legal, technical, whitepapers, investor docs |
| `09-assets/` | — | 1 | Brand assets (logo.svg) |
| `scripts/` | — | 2 | Root-level scripts |

---

## 4. DEPARTMENT: .NET ECOSYSTEM (08-dotnet/)

### 4.1 Architecture

```
08-dotnet/
├── IerahkwaMamey.sln        ← Master solution
├── Mamey.sln                 ← Core solution
├── banking/                  ← Banking system (INKG + Net10)
├── framework/                ← MameyFramework core library
├── government/               ← Government services
├── microservices/            ← 41 microservices
├── platform/                 ← IERAHKWA.Platform
└── ui/                       ← Blazor WebAssembly UI
```

### 4.2 Microservices (41 total)

| # | Service | C# Files | Projects | Domain |
|---|---------|----------|----------|--------|
| 1 | **SmartSchool** | 113 | 25 | Education ERP |
| 2 | **SpikeOffice** | 46 | 3 | Office Suite |
| 3 | **AppBuilder** | 39 | 3 | Low-code Platform |
| 4 | **InventoryManager** | 36 | 1 | Inventory System |
| 5 | **HRM** | 31 | 3 | Human Resources |
| 6 | **TradeX** | 24 | 3 | Trading Platform |
| 7 | **AdvocateOffice** | 23 | 1 | Legal Management |
| 8 | **IDOFactory** | 13 | 3 | Initial DEX Offering |
| 9 | **DocumentFlow** | 13 | 3 | Document Management |
| 10 | **compliance** | 12 | 6 | Regulatory Compliance |
| 11 | **CitizenCRM** | 12 | 3 | Citizen Relations |
| 12 | **ESignature** | 11 | 3 | Digital Signatures |
| 13 | **treasury** | 10 | 6 | Treasury Management |
| 14 | **RnBCal** | 10 | 3 | Calendar/Scheduling |
| 15 | **OutlookExtractor** | 10 | 3 | Email Processing |
| 16 | **notifications** | 8 | 2 | Notification Hub |
| 17 | **identity** | 8 | 2 | Identity Management |
| 18 | **MeetingHub** | 8 | 3 | Video Conferencing |
| 19 | **FarmFactory** | 7 | 3 | Agriculture Tech |
| 20 | **BioMetrics** | 7 | 5 | Biometric Auth |
| 21 | **WorkflowEngine** | 5 | 3 | Process Automation |
| 22 | **VotingSystem** | 5 | 3 | E-Voting |
| 23 | **TaxAuthority** | 5 | 3 | Tax Collection |
| 24 | **ServiceDesk** | 5 | 3 | IT Support |
| 25 | **ReportEngine** | 5 | 3 | Report Generation |
| 26 | **ProjectHub** | 5 | 3 | Project Management |
| 27 | **ProcurementHub** | 5 | 3 | Procurement |
| 28 | **NotifyHub** | 5 | 3 | Push Notifications |
| 29 | **FormBuilder** | 5 | 3 | Dynamic Forms |
| 30 | **DigitalVault** | 5 | 3 | Secure Storage |
| 31 | **DataHub** | 5 | 3 | Data Integration |
| 32 | **ContractManager** | 5 | 3 | Contract Lifecycle |
| 33 | **BudgetControl** | 5 | 3 | Budget Management |
| 34 | **AuditTrail** | 5 | 3 | Audit Logging |
| 35 | **AssetTracker** | 5 | 3 | Asset Management |
| 36 | **MultichainBridge** | 4 | 3 | Cross-chain Bridge |
| 37 | **DeFiSoberano** | 4 | 3 | Decentralized Finance |
| 38 | **TransactionCodes** | — | — | Transaction Encoding |
| 39 | **NFTCertificates** | 2 | 2 | NFT Certification |
| 40 | **GovernanceDAO** | 2 | 2 | DAO Governance |
| 41 | **AIFraudDetection** | 2 | 2 | AI Fraud Detection |

### 4.3 Banking System (08-dotnet/banking/)

| Component | Description |
|-----------|-------------|
| **INKG** | Full commercial banking system with DDD architecture |
| **Net10** | Secondary banking implementation |
| **Design docs** | 13 bounded contexts for commercial banking |
| **Infrastructure** | Docker, Helm charts, K8s configs, Consul, Tye |

Banking bounded contexts:
1. Digital Banking & Mobile Services
2. Master Trust Account
3. Account Management
4. Transaction Processing
5. Payment Systems
6. Key Telex Transfer
7. Savings Account
8. Loan & Credit Management
9. Regulatory Compliance & Reporting
10. Customer Service & Support
11. Wealth Management & Investment
12. Forex Exchange & Trade Finance
13. Risk Management & Security

### 4.4 Government Services (08-dotnet/government/)

| Component | Description |
|-----------|-------------|
| **citizen-api-gateway** | API Gateway for citizen services |
| **identity** | Government identity management |
| **monolith** | Monolithic government application |
| **portals** | Government portals |
| **pupitre** | Government desk/office system |

### 4.5 MameyFramework (08-dotnet/framework/)

Core platform framework with:
- Source code (`src/`), Tests (`tests/`), Tools (`tools/`)
- FWID sagas integration
- Database migrations
- Docker configs
- NuGet package publishing scripts
- Template system

### 4.6 Blazor UI (08-dotnet/ui/)

MameyNode.UI — Blazor WebAssembly frontend:
- `App.razor`, `_Imports.razor`
- `Pages/`, `Components/`, `Shared/`
- `Services/`, `Data/`
- `wwwroot/` static assets

---

## 5. DEPARTMENT: BLOCKCHAIN & FWID (14-blockchain/)

### 5.1 Architecture

```
14-blockchain/
├── future-wampum/
│   └── FutureWampumId/         ← FWID microservices saga
│       ├── Mamey.FWID.Identities/      (595 C# files)
│       ├── Mamey.FWID.Notifications/   (89 C# files)
│       ├── Mamey.FWID.Operations/      (30 C# files)
│       └── Mamey.FWID.Sagas/           (16 C# files)
├── quantum/                    ← Post-quantum cryptography
└── tokens/                     ← Token contracts
```

### 5.2 FWID (Future Wampum Identity) — 730 C# Files

The FWID system is the most complex single service in the codebase.

**Mamey.FWID.Identities** (595 files — largest service):
- Clean Architecture: Api, Application, Domain, Infrastructure, Contracts
- Full test coverage: Unit, Integration, End-to-End, Shared
- Polyglot persistence: PostgreSQL, MongoDB, Redis, MinIO
- gRPC services: BiometricService, PermissionSyncService
- 9 REST endpoints + 1 Permission Sync endpoint
- JWT + Certificate authentication
- Post-quantum crypto: ML-DSA-65 + ML-KEM-1024

**Mamey.FWID.Notifications** (89 files):
- Notification delivery (push, email, SMS, in-app)
- Event-driven architecture

**Mamey.FWID.Operations** (30 files):
- System operations, health checks, monitoring

**Mamey.FWID.Sagas** (16 files):
- Distributed transaction orchestration across FWID services

### 5.3 Smart Contracts (Solidity — 9 files)

Located in `08-dotnet/microservices/DeFiSoberano/contracts/`:

| Contract | Purpose |
|----------|---------|
| **IerahkwaToken.sol** | Native platform token (ERC-20) |
| **SovereignToken.sol** | Sovereign governance token |
| **SovereignGovernance.sol** | DAO governance logic |
| **SovereignVault.sol** | Treasury vault |
| **SovereignStaking.sol** | Staking rewards |

---

## 6. DEPARTMENT: BACKEND SERVICES (03-backend/)

20 Node.js backend services:

| # | Service | Files | Description |
|---|---------|-------|-------------|
| 1 | **smart-school-node** | 86 | Education system backend |
| 2 | **inventory-system** | 53 | Inventory management |
| 3 | **ierahkwa-shop** | 47 | E-commerce platform |
| 4 | **pos-system** | 28 | Point of sale |
| 5 | **mobile-app** | 22 | Mobile app backend |
| 6 | **image-upload** | 10 | Image processing service |
| 7 | **red-social** | 9 | Social network |
| 8 | **reservas** | 8 | Booking/reservations |
| 9 | **forex-trading-server** | 8 | Forex trading API |
| 10 | **plataforma-principal** | 7 | Main platform API |
| 11 | **voto-soberano** | 6 | Voting system |
| 12 | **vigilancia-soberana** | 6 | Surveillance system |
| 13 | **social-media** | 6 | Social media API |
| 14 | **shared** | 6 | Shared libraries |
| 15 | **empresa-soberana** | 6 | Enterprise management |
| 16 | **conferencia-soberana** | 6 | Video conferencing |
| 17 | **blockchain-api** | 6 | Blockchain API gateway |
| 18 | **trading** | 5 | Trading engine |
| 19 | **server** | 5 | Core server |
| 20 | **api-gateway** | 5 | API Gateway |

All services include:
- `package.json` with dependencies
- `Dockerfile` for containerization
- `__tests__/` with unit tests
- `.dockerignore` for build optimization

---

## 7. DEPARTMENT: RUST BLOCKCHAIN (12-rust/)

### 7.1 mamey-forge (20 .rs files)

Smart contract development toolkit:
- CLI commands: init, build, deploy, test, clean, query, call, info
- DevNet local environment
- Configuration management
- Template system: basic, token contracts
- Example contracts: hello-world, counter, escrow, erc20-token, nft

### 7.2 mamey-node-rust (9 .rs files)

Blockchain node SDK:
- Node service: consensus, networking, storage
- Banking service integration
- RPC service for external communication
- Build system with custom `build.rs`
- Smoke tests

---

## 8. DEPARTMENT: HTML PLATFORMS (02-plataformas-html/)

### 8.1 Overview

| Metric | Value |
|--------|-------|
| Total index.html files | 190 |
| Individual platforms | 179 |
| NEXUS mega-portals | 10 |
| Portal Central | 1 |
| Total HTML lines | 29,712 |
| Total HTML size | 2.29 MB |
| Shared CSS | ierahkwa.css (7 KB) |
| Shared JS | ierahkwa.js (5 KB) |

### 8.2 NEXUS Mega-Portals

| NEXUS | Color | Domain | Platforms | Size |
|-------|-------|--------|-----------|------|
| Orbital | `#00bcd4` | Telecomunicaciones & Satelites | 17 | 10,086 bytes |
| Escudo | `#f44336` | Defensa & Ciberseguridad | 12 | 9,375 bytes |
| Cerebro | `#7c4dff` | AI, Quantum & Data | 15 | 9,966 bytes |
| Tesoro | `#ffd600` | Finanzas & Blockchain | 14 | 10,247 bytes |
| Voces | `#e040fb` | Social Media & Lenguas | 10 | 9,773 bytes |
| Consejo | `#1565c0` | Gobierno & Justicia | 16 | 10,298 bytes |
| Tierra | `#43a047` | Naturaleza & Recursos | 19 | 10,087 bytes |
| Forja | `#00e676` | Desarrollo Tech | 10 | 9,668 bytes |
| Urbe | `#ff9100` | Ciudad & Servicios | 13 | 10,065 bytes |
| Raices | `#d4a853` | Cultura & Economia | 13 | 10,014 bytes |
| **TOTAL** | | | **139** | |

### 8.3 GAAD Accessibility Compliance

| Feature | Compliant | Total | Rate |
|---------|-----------|-------|------|
| Skip navigation | 188 | 190 | 98.9% |
| `prefers-reduced-motion` | 188 | 190 | 98.9% |
| `aria-hidden` decorative | 186 | 190 | 97.9% |
| `:focus-visible` outlines | 188 | 190 | 98.9% |
| `<main>` landmark | 188 | 190 | 98.9% |

Note: 2 files missing accessibility features are likely minimal stubs (landing pages with < 10 lines).

### 8.4 Design System

CSS Variables:
- Backgrounds: `--bg:#09090d`, `--bg2:#111116`, `--bg3:#1a1a20`, `--bg4:#232330`
- Brand: `--gold:#d4a853`, `--accent` (per-platform)
- Text: `--txt:#e8e4df`, `--txt2:#8a8694`
- Borders: `--brd:#2a2a36`, `--r:10px`

Tag Colors: AI `#7c4dff`, WAMPUM `#d4a853`, SAT `#00bcd4`, BLOCKCHAIN `#4a9eff`, QUANTUM `#e84040`

---

## 9. DEPARTMENT: INFRASTRUCTURE (04-infraestructura/)

39 files covering:
- Docker configurations
- Kubernetes manifests
- Helm charts
- Terraform IaC
- CI/CD pipeline definitions

---

## 10. DEPARTMENT: DASHBOARDS (06-dashboards/)

227 files:

| Component | Description |
|-----------|-------------|
| **Maestro** | Primary dashboard with LLM integration, FWID compliance workflows, memory optimization, MCP server |
| **dashboard-command-center.html** | Operations command center |
| **mamey-dashboard.html** | Main Mamey dashboard |

Maestro includes: Docker configs, Jest tests, TypeScript source, Claude/Cursor context configs, extensive documentation (20+ markdown files).

---

## 11. DEPARTMENT: AI (13-ai/)

21 files across 3 projects:

| Project | Description |
|---------|-------------|
| **agente-soberano** | AI agent for sovereign operations |
| **code-generator** | AI-powered code generation |
| **mamey-future-ai** | Future AI research & development |

---

## 12. DEPARTMENT: CORE (10-core/)

24 files — Foundation layer:

| Component | Language | Description |
|-----------|----------|-------------|
| **MameyNode** | Rust | Blockchain node: API, blockchain (account, block, state, token), config, consensus, crypto, network, storage |
| **MameyFramework** | C# | Core abstractions: Entity, IMameyService, Result, IBlockchainClient, MameyNodeClient |
| **MameyLockSlot** | C# | Distributed locking mechanism |
| **Mamey.TemplateEngine** | C# | Code/document template generation |

---

## 13. DEPARTMENT: SDKs (11-sdks/)

12 files across 3 languages:

| SDK | Language | Components |
|-----|----------|------------|
| **mamey/go** | Go | client, wallet, token, transaction, errors + tests |
| **mamey_sdk/python** | Python | SDK package with `__init__.py` |
| **mamey/typescript** | TypeScript | SDK with `index.ts` + `package.json` |

---

## 14. DEPARTMENT: API SPECIFICATIONS (05-api/)

4 files:

| File | Description |
|------|-------------|
| `openapi.yaml` | REST API specification |
| `API-SPECIFICATION.md` | Complete API documentation |
| `contracts/README.md` | Contract documentation |
| `protos/` | gRPC protocol buffer definitions |

---

## 15. DEPARTMENT: DOCUMENTATION (01-documentos/ + 16-docs/)

**01-documentos/** (62 files):
- `auditoria/` — Audit reports
- `inversores/` — Investor documentation
- `legal/` — Legal framework
- `tecnico/` — Technical specifications
- `whitepapers/` — Research papers

**16-docs/** (410 files):
- Extended documentation for all services

---

## 16. DEPARTMENT: DEVOPS SCRIPTS (07-scripts/)

25 files:

| Script | Purpose |
|--------|---------|
| `INSTALAR-TODO.sh` | Full installation script |
| `UNIFICAR-TODO.sh` | Unification/merge script |
| `UPGRADE-MAMEY.sh` | Platform upgrade |
| `deploy.sh` | Deployment automation |
| `backup-database.sh` | Database backups |
| `backup-todas-plataformas.sh` | Full platform backup |
| `setup-ssl.sh` | SSL certificate setup |
| `rotate-keys.sh` | Key rotation |
| `sast-scanner.js` | Static analysis security testing |
| `sast-autofix.js` | Auto-fix security issues |
| `sca-scanner.js` | Software composition analysis |
| `supply-chain-audit.js` | Supply chain security audit |
| `install-security.sh` | Security installation |
| `start-production.sh` | Production start |
| `stop-mamey.sh` | Platform shutdown |
| `generate-sln.sh` | Solution file generator |
| `audit-platforms.sh` | Platform auditor |
| `test-live.sh` | Live testing |

Tests: `sast-autofix.test.js`, `sast-scanner.test.js`

---

## 17. DEPARTMENT: UTILITIES (15-utilities/)

1,027 files across 7 projects:

| Project | Description |
|---------|-------------|
| **BaGetter** | NuGet package feed server |
| **Mamey.ApplicationName.Monolith** | Monolithic application template |
| **Mamey.Image.BackgroundRemoval** | AI background removal service |
| **Mamey.TemplateEngine** | Template generation engine |
| **Mamey.Templates** | Project templates |
| **MameyBarcode** | Barcode generation library |
| **bookstack** | Wiki/documentation platform |

---

## 18. DEPARTMENT: INVESTOR & PITCH (pitch/)

89 files — Investor-facing materials including pitch decks, financial projections, and market analysis.

---

## 19. CONTAINERIZATION SUMMARY

| Type | Count |
|------|-------|
| Dockerfiles | 103 |
| docker-compose files | 94 |
| Kubernetes manifests | In 04-infraestructura/ |
| Helm charts | In 08-dotnet/banking/inkg/helm/ |

---

## 20. TECHNOLOGY STACK SUMMARY

| Layer | Technologies |
|-------|-------------|
| **Frontend** | HTML5, CSS3, Blazor WASM, React (dashboards) |
| **Backend** | .NET 8/C#, Node.js, Express |
| **Blockchain** | Rust (MameyNode), Solidity (Smart Contracts), FWID saga |
| **Database** | PostgreSQL, MongoDB, Redis, MinIO (object storage) |
| **AI/ML** | Python, custom AI agents |
| **DevOps** | Docker, Kubernetes, Helm, Terraform |
| **Security** | Post-quantum crypto (ML-DSA-65, ML-KEM-1024), SAST/SCA scanners |
| **SDKs** | Go, Python, TypeScript |
| **Communication** | gRPC (51 proto files), REST (OpenAPI) |
| **Design** | Custom Ierahkwa design system (CSS variables, GAAD accessible) |
| **CI/CD** | GitHub Actions, custom scripts |

---

## 21. BLOCKCHAIN SPECS

| Spec | Value |
|------|-------|
| Chain Name | MameyNode |
| Chain ID | 777777 |
| Throughput | 12,847 TPS |
| Cryptography | Post-quantum: ML-DSA-65 + ML-KEM-1024 |
| Token | WAMPUM CBDC (720M max supply) |
| Smart Contracts | Solidity (5 contracts) + Rust (mamey-forge) |
| Consensus | Custom (in MameyNode core) |
| Bridge | MultichainBridge microservice |
| DeFi | DeFiSoberano (vault, staking, governance) |

---

## 22. QUALITY METRICS

| Metric | Status |
|--------|--------|
| Accessibility (GAAD) | 98.9% compliance across 190 HTML platforms |
| Test coverage | Unit + Integration + E2E in FWID (50+ test files) |
| Security scanning | SAST + SCA + supply chain audit scripts present |
| Documentation | README at root + platform HTML docs + 1,405 markdown files |
| Zero dependencies | HTML platforms use zero external CDNs/libraries |
| Responsive design | All platforms mobile-first |
| Reduced motion | 98.9% platforms support `prefers-reduced-motion` |
| Dark/Light theme | Shared ierahkwa.css supports `.light-theme` opt-in |

---

## 23. KNOWN ISSUES

1. **2 HTML platforms** missing GAAD accessibility features (likely minimal stubs)
2. **Blazor client projects** have build warnings (separate from core)
3. **Duplicate proto source warnings** in FWID (non-blocking)
4. **50 platforms** have fewer than 25 lines (stub/minimal implementations)

---

## 24. COMPLETE PLATFORM LIST BY NEXUS (139 categorized + 50 independent)

### NEXUS Orbital — Telecomunicaciones & Satelites (17)
satelite-soberano, telecom-soberano, starlink-soberano, estacion-terrena-soberana, gps-soberano, mesh-soberana, cdn-soberano, red-soberana, telefonia-soberana, observacion-terrestre-soberana, lanzamiento-soberano, aviacion-soberana, maritimo-soberano, navegador-soberano, programa-espacial-soberano, radio-soberana, internet-cuantico-soberano

### NEXUS Escudo — Defensa & Ciberseguridad (12)
ciberdefensa-soberana, seguridad-soberana, criptografia-militar-soberana, ejercito-soberano, inteligencia-soberana, vigilancia-soberana, drones-soberanos, frontera-digital-soberana, logistica-militar-soberana, flota-drones-soberana, identidad-digital-soberana, proteccion-datos-soberana

### NEXUS Cerebro — AI, Quantum & Data (15)
analitica-soberana, ml-soberano, datos-soberano, quantum-cloud-soberana, edge-ai-soberano, bci-soberano, robotica-soberana, automatizacion-soberana, genomica-soberana, busqueda-soberana, gemelo-digital-soberano, homomorfica-soberana, laboratorio-soberano, iot-soberano, cloud-soberana

### NEXUS Tesoro — Finanzas & Blockchain (14)
moneda-soberana-cbdc, tokenizacion-soberana, banco-central-soberano, pagos-soberano, invertir-soberano, renta-soberano, transferencia-soberana, seguros-soberano, pension-soberana, contratos-inteligentes-soberano, blockchain-soberana, credito-soberano, zk-identidad-soberana, fiscal-transparency

### NEXUS Voces — Social Media & Lenguas (10)
voz-soberana, mensajeria-soberana, traduccion-soberana, foro-soberano, canal-soberano, media-soberana, correo-soberano, cine-soberano, archivo-linguistico-soberano, noticia-soberana

### NEXUS Consejo — Gobierno & Justicia (16)
gobierno-soberano, justicia-digital-soberana, parlamento-soberano, registro-civil-soberano, democracia-liquida-soberana, diplomacia-soberana, aduana-soberana, inmigracion-soberana, pasaporte-soberano, auditoria-soberana, comision-electoral-soberana, normas-soberana, catastro-soberano, tribunal-digital-soberano, correccional-soberano, propiedad-intelectual-soberana

### NEXUS Tierra — Naturaleza & Recursos (19)
energia-soberana, agua-soberana, agricultura-soberana, pesca-soberana, ganaderia-soberana, meteorologia-soberana, fauna-soberana, geologia-soberana, ambiente-soberano, parques-soberano, residuos-soberano, nuclear-soberano, alimentacion-soberana, reciclaje-soberano, fertirrigacion-soberana, mineria-soberana, acuicultura-soberana, reforestacion-soberana, mapa-soberano

### NEXUS Forja — Desarrollo Tech (10)
dev-soberano, ide-soberano, repositorio-soberano, devops-soberano, lowcode-soberano, backend-soberano, plantillas-soberana, ecosistema-abierto, diseno-soberano, code-soberano

### NEXUS Urbe — Ciudad & Servicios (13)
hospital-campana-soberano, emergencias-soberano, bomberos-soberano, transito-soberano, urbanismo-soberano, vivienda-soberana, vehiculos-soberano, correo-postal-soberano, centro-operaciones-soberano, telemedicina-soberana, farmacia-soberana, transporte-publico-soberano, docs-soberanos

### NEXUS Raices — Cultura & Economia (13)
artesania-soberana, musica-soberana, patrimonio-cultural-soberano, museo-soberano, deporte-soberano, turismo-soberano, biblioteca-soberana, sabiduria-soberana, comercio-soberano, empresa-soberana, trabajo-soberano, desempleo-soberano, salud-mental-soberana

### Independent / Multi-NEXUS Platforms (50)
admin-dashboard, agente-soberano, archivo-eterno-soberano, bdet-bank, bdet-bank-payment-system, bdet-wallet, biometrica-soberana, blockchain-explorer, censo-soberano, cloud-soberana (also cerebro), code-generator, colaboracion-soberana, commerce-business-dashboard, conferencia-soberana, cortos-indigenas, discapacidad-soberana, education-dashboard, familia-soberana, fiscal-dashboard, flujos-soberano, healthcare-dashboard, hospedaje-soberano, humanitario-soberano, infographic, landing-ierahkwa, landing-page, llave-soberana, marketing-soberano, nube-soberana, ofimatica-soberana, orquestador-soberano, pitch-deck, portal-soberano, proyecto-soberano, recibir-cryptohost-convertir-usdt, red-soberana, sabiduria-soberana, seguridad-soberana, soberano-ecosystem, soberano-unificado, starlink-soberano, trading-dashboard, veteranos-soberano, vigilancia-soberana

---

## 25. VERSION HISTORY

| Version | Description | Commit |
|---------|-------------|--------|
| v2.5.0 | 88 platforms (15 frontier) | e9c6577f |
| v2.6.0-b1 | +26 platforms | 34e8fc80 |
| v2.6.0-b2 | +40 platforms | c3466541 |
| v2.7.0 | 10 NEXUS mega-portals | a0e43b59 |
| **v2.8.0** | **Portal Central + 25 platforms + shared CSS/JS + README** | **b9834683** |

---

**Ierahkwa Platform** — Digital Sovereignty for 72M Indigenous People
**19 Nations | 574 Tribal Nations | 190 Platforms | 18,356 Files**
**Copyright: Mamey Technologies (mamey.io) | License: AGPL-3.0**
