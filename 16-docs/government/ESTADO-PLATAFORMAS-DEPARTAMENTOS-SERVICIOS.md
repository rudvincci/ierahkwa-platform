# Estado actual — Plataformas, departamentos, productos y servicios

**Referencia rápida del ecosistema.**  
Actualizado: 2026-02-02

---

## 1. Plataformas (monitoreo y renta por plataforma)

Estas **4 plataformas** tienen monitoreo y servicios de renta **separados** (cada una su vista en `admin-monitoring.html?platform=...` y su sección de renta).

| Plataforma        | ID en API        | Monitoreo | Servicios de renta (IDs) |
|-------------------|------------------|-----------|---------------------------|
| **Casino**        | `casino`         | ✅        | casino, gaming            |
| **BDET (Banco)**  | `bdet`           | ✅        | banking, pos, inventory, ecommerce |
| **Treasury**      | `treasury`       | ✅        | forex, mamey-futures, backup |
| **Financial Center** | `financial-center` | ✅     | tradex, security, ai, quantum, crm, rnbcal, spike-office, appbuilder, document-flow, esignature, meeting-hub, voting, telecom, voip, internet, smart-cell, messaging, video-conf |

**APIs:**
- Monitoreo: `GET /api/v1/admin/monitoring?platform=casino|bdet|treasury|financial-center`
- Renta: `GET /api/v1/platform/rent?platform=casino|bdet|treasury|financial-center`

---

## 2. Departamentos del banco (BHBK — estructura interna)

Son el **corazón del banco**; no se mezclan con “plataformas” de producto.

| Departamento | Función |
|--------------|---------|
| **Tesorería y Reservas** | Custodia FHTC, oro/activos; liquidez para siembra. |
| **Gestión de Riesgos y Cumplimiento** | Valida plantas/leyes soberanas; industrias sensibles (hemp, etc.); evita bloqueos. |
| **Activos Reales (RWA)** | Digitaliza tierra indígena (NFTs); colateral de agricultores. |
| **Tecnología y Nodo 8545** | Servidor, seguridad blockchain, conexión dinero–delivery. |

**Punto único de administración:** BDET Bank (`platform/bdet-bank.html`) + Node 8545 + Banking Bridge.

---

## 3. Servicios especializados (al ciudadano)

No son departamentos; son **servicios directos** del banco a bots y ciudadanos.

| Servicio | Descripción |
|----------|-------------|
| **Banca de Futuros** | Gobiernos compran cosechas adelantadas; inyección de liquidez. |
| **Crédito Agrícola Dinámico** | Préstamos planta por planta, ~1,5%, ajuste por productividad. |
| **Fideicomisos Indígenas** | Patrimonio comunitario y regalías de franquicias. |
| **Gateway de Pagos Multimoneda** | Tokens (ej. IGT-HEMP) ↔ dólares/euros al instante. |

---

## 4. Productos / servicios de renta (catálogo comercial)

Todos en `platform/data/commercial-services-rental.json`. Agrupados por **plataforma** (ver §1).

| ID | Nombre | Plataforma típica | Notas |
|----|--------|-------------------|--------|
| casino | Casino API | Casino | |
| gaming | Gaming Platform | Casino | |
| banking | Banca API | BDET | |
| pos | POS | BDET | |
| inventory | Inventario | BDET | |
| ecommerce | E-commerce | BDET | |
| forex | Forex | Treasury | |
| mamey-futures | Mamey Futures | Treasury | |
| backup | Backup | Treasury | |
| tradex | TradeX | Financial Center | |
| security | Security Fortress | Financial Center | |
| ai | AI Hub | Financial Center | |
| quantum | Quantum Platform | Financial Center | |
| crm | CRM | Financial Center | |
| rnbcal | RnB Cal / Reservas | Financial Center | |
| spike-office | Spike Office | Financial Center | |
| appbuilder | App Builder | Financial Center | |
| document-flow | Document Flow | Financial Center | |
| esignature | E-Signature | Financial Center | |
| meeting-hub | Meeting Hub | Financial Center | |
| voting | Voting System | Financial Center | |
| telecom | Telecom | Financial Center | |
| voip | VoIP / Teléfono | Financial Center | |
| internet | Internet Propio | Financial Center | |
| smart-cell | Smart Cell 4G/5G | Financial Center | |
| messaging | Mensajería Segura | Financial Center | |
| video-conf | Video Conferencia | Financial Center | |

---

## 5. Dominios / categorías (lista maestra de plataformas)

Todas bajo **ATABEY** (una capa de seguridad común). Referencia: `platform/data/platform-links.json`.

| Dominio | Ejemplos |
|---------|----------|
| **Gobierno** | government-portal, admin, departments, sovereign-identity, licenses-department, compliance-center |
| **Bank** | bdet-bank, central-banks, wallet, vip-transactions, debt-collection |
| **Blockchain / DeFi** | blockchain-platform, tradex, net10, farmfactory, tokens |
| **Casino / Gaming** | casino, lotto, raffle, gaming-platform, sports-betting |
| **Social** | social-media, secure-chat, video-call, notifications |
| **AI** | ai-platform, support-ai, atabey-dashboard, app-studio, appbuilder |
| **Seguridad** | security-fortress, firewall-plus, face-recognition, watchlist, backup-department, incidentes-dr |
| **Ciudadano** | citizen-portal, user-dashboard, citizen-launchpad, contribution-graph |
| **Otros** | health-platform, education-platform, insurance-platform, services-platform, telecom, quantum-platform, forex, futurehead-group, siis-settlement |

---

## 6. Matriz de bots ↔ departamentos

| Bot externo | Departamento del banco | Servicio que recibe |
|-------------|------------------------|----------------------|
| Bot Agricultura | Activos Reales (RWA) | Validación colateral tierra |
| Bot Logística | Tesorería | Liberación de pagos tras entrega |
| Bot Ciudadano | Tecnología (Nodo 8545) | SSO, recompensas |
| Bot Casino | Gestión de Riesgos | Auditoría premios, custodia fondos |

---

## Resumen

- **4 plataformas** operativas para monitoreo y renta: Casino, BDET, Treasury, Financial Center.
- **4 departamentos** del banco: Tesorería, Riesgos/Cumplimiento, RWA, Tecnología.
- **4 servicios especializados** al ciudadano: Banca de Futuros, Crédito Agrícola, Fideicomisos, Gateway Multimoneda.
- **27 productos de renta** en catálogo, asignados a una de las 4 plataformas.
- **Lista maestra** por dominio: Gobierno, Bank, Blockchain, Casino/Gaming, Social, AI, Seguridad, Ciudadano, Otros.

**Docs relacionados:** `ARQUITECTURA-BHBK-DEPARTAMENTOS.md`, `LISTA-MAESTRA-PLATAFORMAS.md`, `PLATAFORMAS-8545.md`, `ESTADO-PLATAFORMAS-SERVICIOS.md`.
