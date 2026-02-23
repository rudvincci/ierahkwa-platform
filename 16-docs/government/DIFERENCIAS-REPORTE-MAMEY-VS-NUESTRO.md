# Diferencias: Reporte técnico Mamey (v4.0) vs nuestro proyecto

**Sovereign Government of Ierahkwa Ne Kanienke**  
Basado en **MameyFutureNode & Portal — Technical Report v4.0 (2026-02-09)** comparado con nuestro stack (RuddieSolution, Node 8545, BDET, SIIS, platform).

---

## 1. Arquitectura

| Aspecto | Mamey (reporte) | Nuestro proyecto |
|--------|-------------------|------------------|
| **Raíz de confianza** | **Kernel Chain** exclusivo: registra dominios, emite **Kernel Receipts**; toda comunicación entre dominios pasa por el Kernel (firmado, con expiración, anclado a bloque). | Un **nodo principal** (8545) que expone RPC + REST + platform; no hay una “Kernel Chain” separada que firme receipts entre dominios. |
| **Dominios** | **Aislamiento por dominio:** cadenas separadas (Kernel, Government, Banking con BIIS / Central / Commercial). Un fallo en Banking no ve datos de Government. | BDET, SIIS, 4 bancos centrales (Águila, Quetzal, Cóndor, Caribe), Government, etc. integrados en el mismo Node y en módulos (government-banking.js, bdet-server, routers). Menos aislamiento físico por cadena. |
| **Comunicación entre dominios** | Solo vía **Kernel Receipt**: Domain A → Kernel (CreateReceipt) → Domain B (VerifyReceipt). Sin llamadas directas entre dominios. | APIs REST y módulos en el mismo proceso; llamadas directas entre servicios (ej. BDET ↔ SIIS) sin capa de “receipts” del Kernel. |

---

## 2. Stack tecnológico

| Capa | Mamey (reporte) | Nuestro proyecto |
|------|-------------------|------------------|
| **Blockchain / nodo** | **Rust** (~765 .rs, ~300k líneas): nodos por dominio (Kernel, Government, BIIS, Central, Commercial). | **Node.js** (server.js, Express): un servidor 8545 que incluye RPC tipo Ethereum, REST, platform estática y lógica de banca/gobierno. |
| **Portal / UI** | **.NET 9 Blazor Server** (módulos Kernel, Government, Banking). Portal en puerto **7295**. | **HTML/JS** estático en `RuddieSolution/platform/` servido por el Node 8545; sin Blazor. |
| **Inter-servicio** | **gRPC** entre Portal y nodos Rust. | HTTP/REST y rutas Express; opcionalmente otros servicios .NET en puertos distintos. |
| **Almacenamiento** | **LMDB** (estado cadena), **PostgreSQL** (read models, workflow), **RocksDB** (alternativa). | JSON en disco (gov-accounts, gov-transactions, vip-transactions, bank-registry, blockchain-state, etc.) y opcionalmente otras DB. |
| **Criptografía** | **Ed25519**, **Dilithium** (post-cuántico), **Kyber**, **Blake2b/3**, **AES-256-GCM**, **Argon2**. | Crypto nativo Node.js; no hay post-cuántico documentado en el reporte nuestro. |

---

## 3. Banca y liquidación

| Concepto | Mamey (reporte) | Nuestro proyecto |
|----------|------------------|------------------|
| **Interbancario** | **BIIS** (Bank of Indigenous International Settlements): **solo enrutamiento**, no tiene balances; Central Bank hace la liquidación y otorga finalidad. | **SIIS** (Ierahkwa / IISB): liquidación internacional; BDET y 4 bancos centrales; bank-registry, government-banking, bdet-server. |
| **Modelo** | Tres partes: Banco emisor → BIIS → Banco Central (liquidación) → Banco receptor. Central Bank = autoridad de liquidación. | Jerarquía bancaria (puertos 6000–6400), BDET, Clearing; datos en node/data/bdet-bank, bank-registry.json. |
| **CBDC** | CBDC explícito: emitido por Central Bank, legal tender, reversible por orden judicial, KYC. | Tokens IGT (IGT-20) en nodo; BDET como banco de desarrollo; no se nombra “CBDC” en la misma forma que el reporte Mamey. |
| **Puertos (Mamey)** | gRPC: Kernel 50050, Government 50070, BIIS 50060, Central 50061, Commercial 50062; HTTP: Portal 7295, biis-core 50140, central-core 50142, commercial-core 50144. | Node 8545, Bridge 3001, Platform 8080, plataformas 4001–4600, banca 6000–6400, .NET en varios puertos (5000, 5054, etc.). |

---

## 4. Identidad y gobernanza

| Aspecto | Mamey (reporte) | Nuestro proyecto |
|---------|------------------|------------------|
| **Identidad** | **DIDs** (did:mamey:gov:citizen:…), **Verifiable Credentials** (W3C), credenciales con expiración (ID, licencia, pasaporte). | Identidad y auth en el Node (JWT, sessions, KYC); no hay capa DID/VC documentada como en el reporte Mamey. |
| **Gobernanza** | **Multi-tier:** Critical (Board 5/7 + Executive 3/5), High (Executive 3/5), Medium (Operations 2/3), Low (Technical 1). | Roles y permisos en platform (ej. admin, Leader Control); no hay niveles de aprobación formales como en el reporte. |
| **Cumplimiento** | KYC/AML, Travel Rule (FATF 16), niveles de KYC (None a Full), particiones de privacidad (GDPR, HIPAA, PCI-DSS). | KYC y servicios en el Node; cumplimiento no descrito con el mismo detalle que en el reporte. |

---

## 5. Contratos inteligentes y más

| Aspecto | Mamey (reporte) | Nuestro proyecto |
|---------|------------------|------------------|
| **Smart contracts** | **WASM** (Wasmtime), ejecución por dominio, **solo receipts** para cruce de dominios, gas por fuel. | Lógica en Node (módulos, rutas); no hay capa WASM de contratos como en el reporte. |
| **Pagos** | 12 tipos (P2P, Merchant, Recurring, Subscription, Disbursement, Multisig, Remittance, Invoice, Bill, ACH, Wire, Internal); ISO 20022, SWIFT GPI, ACH/NACHA, SEPA, Fedwire, RTP, CHIPS. | Pagos y transferencias en BDET, VIP, banking-bridge; no documentados con la misma taxonomía. |
| **Lending / Trading / Bridge** | Módulos de lending (15+ productos), DEX (AMM + order book), Bridge L1–L2 (Ethereum, Bitcoin, rollups). | TradeX, DeFi, forex, Mamey Futures en el ecosistema; no un único documento técnico como el reporte. |
| **Observabilidad** | Prometheus, OpenTelemetry, Jaeger, Grafana, Alertmanager, trazado cross-domain. | Health, métricas y logs en el Node; no un stack observabilidad igual al del reporte. |
| **DR / Sharding** | RTO &lt;15 min, RPO &lt;1 min; réplica; reconstrucción de estado desde Kernel receipts; sharding con Beacon + shards. | Backups, scripts, datos en disco; no hay descripción equivalente de DR ni sharding. |

---

## 6. Resumen en una frase

- **Mamey (reporte v4.0):** Blockchain **regulada** con **Kernel como raíz de confianza**, dominios aislados, comunicación solo por **Kernel Receipts**, stack **Rust + .NET 9 Blazor**, BIIS/Central/Commercial, DIDs/VC, gobernanza multi-nivel y cumplimiento muy detallado.
- **Nuestro proyecto:** **Un nodo Node.js (8545)** que concentra API, platform y lógica de banca/gobierno; BDET, SIIS y 4 bancos centrales; **sin Kernel Chain ni receipts** entre dominios; **sin Blazor ni nodos Rust**; más integrado en un solo proceso y con muchas UIs HTML/JS.

Para comparativa de estructura y volumen (quién tiene más de qué): [COMPARATIVA-MAMEY-VS-RUDDIESOLUTION.md](COMPARATIVA-MAMEY-VS-RUDDIESOLUTION.md).  
Para nuestro blockchain y tokens: [DETALLES-PLATAFORMA-BLOCKCHAIN.md](DETALLES-PLATAFORMA-BLOCKCHAIN.md).

*Basado en MameyFutureNode & Portal — Technical Report v4.0 (2026-02-09). Sovereign Government of Ierahkwa Ne Kanienke.*
