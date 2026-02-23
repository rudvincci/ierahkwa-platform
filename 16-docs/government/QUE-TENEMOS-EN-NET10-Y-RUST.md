# Qué tenemos en .NET 10 y en Rust

**Sovereign Government of Ierahkwa Ne Kanienke**  
Resumen de todo lo que está en **.NET** (incl. .NET 10) y en **Rust** en el proyecto.

---

## 1. .NET (C#) — Resumen

Hay **más de 150 proyectos .csproj**. Muchos usan **.NET 10** (net10.0); algunos Mamey usan **.NET 8** (net8.0).

### 1.1 .NET 10 (net10.0) — Lo nuestro

| Carpeta / Proyecto | Qué es | Puerto / nota |
|--------------------|--------|----------------|
| **NET10/** | Plataforma DeFi: Swap, Liquidity Pools, Yield Farming, Contribution Graph | 5071 |
| **RuddieSolution/IerahkwaBanking.NET10/** | Sistema bancario soberano: WAMPUM, 4 bancos centrales (Águila, Quetzal, Cóndor, Caribe), bancos nacionales, integración Mamey Node | 5000, Swagger |
| **AIFraudDetection/** | Detección de fraude | API |
| **BioMetrics/** | Biometría | 5120 |
| **MultichainBridge/** | Bridge multichain | 5143 |
| **GovernanceDAO/** | DAO | 5142 |
| **NFTCertificates/** | NFT | 5141 |
| **DeFiSoberano/** | DeFi | 5140 |
| **VotingSystem/** | Votación | 5092 |
| **TaxAuthority/** | Autoridad fiscal | 5091 |
| **TradeX/** | Trading | 5054 |
| **platform-dotnet/IERAHKWA.Platform/** | Plataforma IERAHKWA en .NET | |
| **SmartSchool/** (ForexInvestment.*) | Inversión Forex, persistencia | net10.0 |
| **AdvocateOffice, AppBuilder, AppointmentHub, AuditTrail, CitizenCRM, ContractManager, DataHub, DigitalVault, DocumentFlow, ESignature, FarmFactory, FormBuilder, HRM, IDOFactory, MeetingHub, NET10 (API), NotifyHub, ProjectHub, ReportEngine, RnBCal, ServiceDesk, SpikeOffice, WorkflowEngine, …** | Servicios por dominio (cada uno con .API, .Core, .Infrastructure) | Puertos en docs/TODO-LO-QUE-CORRE-ONLINE.md |

### 1.2 .NET 8 (net8.0) — Sobre todo Mamey

| Dónde | Qué |
|-------|-----|
| **Mamey/services/** | Blog, Documents (FileReader, ExcelReader), Treasury, Compliance (TreatyValidators, WhistleblowerReports, ZeroKnowledgeProofs), Identity (Government.Identity), Workflows (Sagas), Notifications |
| **Mamey/core/** | MameyFramework, Mamey.TemplateEngine, MameyLockSlot |
| **plataformas-finales/net10/** | PlataformaSoberana.Net10.csproj (nombre net10 pero target net8.0) |

### 1.3 Estructura típica .NET

- **Carpeta raíz** (ej. NET10, IerahkwaBanking.NET10, DeFiSoberano): solución .sln.
- **Subcarpetas:** API (Controllers, Program.cs), Core (Models, Interfaces), Infrastructure (Services).
- **Puertos:** cada API suele tener puerto propio (5071 NET10, 5000 Banking, etc.). Lista completa en `docs/TODO-LO-QUE-CORRE-ONLINE.md`.

---

## 2. Rust — Resumen

Hay **4 proyectos con Cargo.toml** (Rust):

| Proyecto | Ruta | Qué es |
|----------|------|--------|
| **Mamey Node** | `Mamey/core/MameyNode/` | Nodo blockchain soberano de alto rendimiento: Axum, RocksDB/Sled, libp2p, cripto (ed25519, x25519, AES-GCM, ChaCha20, Argon2). Binario: `mamey-node`. |
| **ierahkwa-swift-service** | `RuddieSolution/services/rust/` | Servicio SWIFT MT/MX: parser y crypto (Actix-web). |
| **security-platform** | `RuddieSolution/services/security-platform/` | Una sola plataforma en Rust: Quantum, AI, Fortress, Vigilancia, Telecom (Actix-web). |
| **plataforma-soberana-rust** | `plataformas-finales/rust/` | API de comparación / demo en Rust (Actix-web). |

### 2.1 Detalle Rust

- **Mamey Node:** dependencias tokio, axum, serde, sha2/sha3, ed25519-dalek, rocksdb, libp2p, tracing, prometheus. Es el núcleo blockchain del ecosistema Mamey.
- **RuddieSolution/services/rust:** SWIFT (MT/MX), quick-xml, actix-web.
- **RuddieSolution/services/security-platform:** seguridad unificada en un solo servicio Rust.
- **plataformas-finales/rust:** proyecto pequeño de API en Rust.

---

## 3. Tabla global: .NET vs Rust

| Tipo | Cantidad aprox. | Dónde |
|------|------------------|-------|
| **Proyectos .NET (.csproj)** | 156 | Raíz (NET10, AIFraudDetection, AdvocateOffice, …), Mamey/services y Mamey/core, RuddieSolution/IerahkwaBanking.NET10, platform-dotnet, plataformas-finales/net10 |
| **.NET 10 (net10.0)** | Mayoría de los APIs en raíz + IerahkwaBanking.NET10 + platform-dotnet + SmartSchool | Ver 1.1 |
| **.NET 8 (net8.0)** | Servicios Mamey (Blog, Documents, Treasury, Compliance, Identity, etc.) + plataformas-finales/net10 | Ver 1.2 |
| **Proyectos Rust (Cargo.toml)** | 4 | Mamey/core/MameyNode, RuddieSolution/services/rust, RuddieSolution/services/security-platform, plataformas-finales/rust |

---

## 4. Referencias

- **Puertos y servicios:** `docs/TODO-LO-QUE-CORRE-ONLINE.md`
- **NET10 DeFi:** `NET10/README.md` (puerto 5071)
- **Banking .NET 10:** `RuddieSolution/IerahkwaBanking.NET10/README.md` (puerto 5000, integración Mamey Node)
- **Mamey (Rust + .NET):** carpeta `Mamey/`; nodo Rust en `Mamey/core/MameyNode/`
- **Índice completo:** `docs/INDICE-COMPLETO-PROYECTO-SOBERANOS.md`

*Última actualización: febrero 2026.*
