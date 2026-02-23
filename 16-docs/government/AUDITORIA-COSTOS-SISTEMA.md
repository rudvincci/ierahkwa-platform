# AUDITORÍA DE COSTOS – SISTEMA IERAHKWA

**Sovereign Government of Ierahkwa Ne Kanienke**  
**Fecha:** 19 Enero 2026  
**Alcance:** Ecosistema completo (blockchain, APIs, plataformas, móvil, infraestructura)

---

## 1. INVENTARIO DE COMPONENTES

### 1.1 Núcleo Blockchain e infraestructura

| Componente | Tecnología | Descripción |
|------------|------------|-------------|
| Ierahkwa Futurehead Mamey Node | Node.js, Express | Blockchain ISB, RPC, API REST, 109 tokens IGT |
| Nodos respaldo (Alpha, Beta, Gamma, Delta) | — | 4 regiones (NA, EU, APAC, LATAM) |
| CryptoHost System | — | Hot/cold wallet, HSM, multi-sig |
| Cross-Chain Bridge | — | 6–7 cadenas (ETH, BSC, Polygon, AVAX, Arbitrum, Optimism, Solana, BTC) |
| Oracles | Chainlink, Pyth | Precios, VRF, automatización |

### 1.2 Aplicaciones Node.js

| Aplicación | Puerto / uso | Dependencias principales |
|------------|--------------|---------------------------|
| node (Mamey) | 8545 | express, cors, helmet, compression, uuid, crypto-js |
| ierahkwa-shop | — | fastify, socket.io, bcryptjs |
| inventory-system | — | express, sql.js, exceljs, pdfkit, archiver, ejs |
| pos-system | — | express, bcryptjs, express-session |
| smart-school-node | — | express, sequelize, mysql2, pg, jsonwebtoken, nodemailer |
| forex-trading-server | — | — |
| image-upload | — | — |
| server (raíz) | — | — |

### 1.3 APIs y servicios .NET

| Proyecto | Capa | Uso |
|----------|------|-----|
| TradeX | API, Core, Infra | Exchange, Swagger |
| DocumentFlow | API, Core, Infra | Documentos, workflow |
| NET10 | API, Core, Infra | Servicios NET 10 |
| SpikeOffice | API, Core, Infra | Dashboard, empleados, multi-tenant, SQLite |
| HRM | API, Core, Infra | RRHH, nómina, asistencia, etc. |
| FarmFactory | API, Core, Infra | Depósitos, pools |
| IDOFactory | API, Core, Infra | Ofertas iniciales |
| AdvocateOffice | — | Legal, casos, clientes |
| AppointmentHub | API, Core | Citas |
| AuditTrail | API, Core, Infra | Auditoría |
| BudgetControl | API, Core, Infra | Presupuesto |
| CitizenCRM | API, Core, Infra | CRM ciudadanos |
| ContractManager | API, Core, Infra | Contratos |
| DataHub | API, Core, Infra | Datos |
| DigitalVault | API, Core, Infra | Bóveda |
| ESignature | API, Core, Infra | Firma electrónica |
| FormBuilder | API, Core, Infra | Formularios |
| MeetingHub | API, Core, Infra | Reuniones |
| NotifyHub | API, Core, Infra | Notificaciones |
| ProcurementHub | API, Core, Infra | Compras |
| ProjectHub | API, Core, Infra | Proyectos |
| ReportEngine | API, Core, Infra | Reportes |
| ServiceDesk | API, Core, Infra | Mesa de ayuda |
| AssetTracker | API, Core, Infra | Activos |
| SmartSchool | Web, Persistence, Application, Domain | Escuela, contabilidad, Forex |
| InventoryManager | WinForms / .NET | Inventario desktop |
| OutlookExtractor | API, Infra | Extracción correo |
| RnBCal | — | Calendario |
| AppBuilder | — | Constructor de apps |

### 1.4 Plataforma web (HTML/JS)

| Tipo | Cantidad | Ejemplos |
|------|----------|----------|
| Páginas platform/ | 41 | index, vip-transactions, citizen-membership, tradex, bdet-bank, bridge, cryptohost, central-banks, siis, leader-control, admin, wallet, forex, etc. |
| Otras HTML raíz | 4+ | IERAHKWA_PLATFORM_V1, ADMIN_FOREX_INVESTMENT, commerce-business-dashboard, RECIBIR_CRYPTOHOST_CONVERTIR_USDT, etc. |
| IERAHKWA-PLATFORM-DEPLOY | 7+ | core (index, leader-control, monitor, support-ai, sw), family-portal |
| Páginas tokens | 100+ | 1 por token IGT (info, etc.) |

### 1.5 Móvil

| App | Stack |
|-----|-------|
| ierahkwa-mobile | React Native 0.73, React 18, React Navigation, ethers, axios, i18next, SVG, Vector Icons |

### 1.6 Bases de datos y almacenamiento

| Uso | Tecnología |
|-----|------------|
| Node (Shop, POS, etc.) | SQLite, archivos JSON |
| SmartSchool | MySQL / PostgreSQL (Sequelize) |
| SpikeOffice | SQLite (spikeoffice.db) |
| .NET (varios) | SQL Server / PostgreSQL / SQLite según proyecto |
| Blockchain | Estado en memoria + persistencia según diseño |

### 1.7 Integraciones y servicios externos

| Servicio | Uso |
|----------|-----|
| SWIFT / bancos corresponsales | 250+ (ref. exchange-trading-deck) |
| Exchanges (Binance, Coinbase, Kraken, etc.) | 10+ para IGT |
| DeFi (Uniswap, PancakeSwap, SushiSwap) | Liquidez IGT |
| Nodemailer | Correo (SmartSchool, etc.) |
| Stripe (ierahkwa-shop) | Pagos (si está activo) |

---

## 2. COSTOS DE DESARROLLO (estimados)

Tasa de referencia: **75 USD/h** (blended senior/mid).  
Horas son aproximadas por tipo de componente.

### 2.1 Blockchain e infraestructura

| Concepto | Horas | Costo (USD) |
|----------|-------|-------------|
| Nodo Mamey (blockchain, RPC, API, tokens, genesis) | 180 | 13 500 |
| Lógica de bloques, cuentas, tx, gas | 120 | 9 000 |
| APIs (tokens, bridge, membership, voting, gamification, backup, VIP) | 150 | 11 250 |
| CryptoHost (concepto, integración, seguridad) | 80 | 6 000 |
| Bridge multi-chain (lógica, mensajería) | 100 | 7 500 |
| **Subtotal Infra** | **630** | **47 250** |

### 2.2 Aplicaciones Node.js

| Aplicación | Horas | Costo (USD) |
|------------|-------|-------------|
| node (Mamey) | incluido arriba | — |
| ierahkwa-shop (e‑commerce, POS, chat, Fastify, Socket.io) | 200 | 15 000 |
| inventory-system (inventario, Excel, PDF, SQL.js) | 120 | 9 000 |
| pos-system (POS, mesas, reportes) | 80 | 6 000 |
| smart-school-node (multi-tenant, Sequelize, rutas, auth) | 250 | 18 750 |
| forex-trading-server | 60 | 4 500 |
| image-upload, server (raíz) | 40 | 3 000 |
| **Subtotal Node** | **750** | **56 250** |

### 2.3 APIs .NET (cada uno: API + Core + Infra)

| Proyecto | Horas | Costo (USD) |
|----------|-------|-------------|
| TradeX | 90 | 6 750 |
| DocumentFlow | 80 | 6 000 |
| NET10 | 70 | 5 250 |
| SpikeOffice | 100 | 7 500 |
| HRM | 150 | 11 250 |
| FarmFactory | 60 | 4 500 |
| IDOFactory | 70 | 5 250 |
| AdvocateOffice | 120 | 9 000 |
| AppointmentHub | 50 | 3 750 |
| AuditTrail | 55 | 4 125 |
| BudgetControl | 50 | 3 750 |
| CitizenCRM | 70 | 5 250 |
| ContractManager | 65 | 4 875 |
| DataHub | 55 | 4 125 |
| DigitalVault | 60 | 4 500 |
| ESignature | 75 | 5 625 |
| FormBuilder | 60 | 4 500 |
| MeetingHub | 55 | 4 125 |
| NotifyHub | 50 | 3 750 |
| ProcurementHub | 55 | 4 125 |
| ProjectHub | 55 | 4 125 |
| ReportEngine | 55 | 4 125 |
| ServiceDesk | 55 | 4 125 |
| AssetTracker | 50 | 3 750 |
| SmartSchool (Web + 4 proyectos) | 320 | 24 000 |
| InventoryManager (WinForms) | 150 | 11 250 |
| OutlookExtractor | 80 | 6 000 |
| RnBCal, AppBuilder | 100 | 7 500 |
| **Subtotal .NET** | **2 131** | **159 825** |

### 2.4 Plataforma web (HTML/JS)

| Concepto | Cantidad | Horas ud. | Horas | Costo (USD) |
|----------|----------|-----------|-------|-------------|
| Páginas platform/ (41) | 41 | 10 | 410 | 30 750 |
| Otras HTML (5) | 5 | 12 | 60 | 4 500 |
| IERAHKWA-PLATFORM-DEPLOY (7) | 7 | 8 | 56 | 4 200 |
| Páginas token (template + 103) | 103 | 2 | 206 | 15 450 |
| VIP Transactions, módulos, reportes | — | — | 60 | 4 500 |
| **Subtotal Web** | — | — | **792** | **59 400** |

### 2.5 Móvil (React Native)

| Concepto | Horas | Costo (USD) |
|----------|-------|-------------|
| App (navegación, ethers, i18n, UI, integración ISB) | 350 | 26 250 |
| **Subtotal Móvil** | **350** | **26 250** |

### 2.6 Scripts, DevOps y documentación

| Concepto | Horas | Costo (USD) |
|----------|-------|-------------|
| start/stop/status, install-autostart, launchd | 20 | 1 500 |
| auto-backup, auto-monitor | 25 | 1 875 |
| Docs (REPORTE-ESTADO, STARTUP, INDEX, etc.) | 40 | 3 000 |
| **Subtotal DevOps/Docs** | **85** | **6 375** |

---

## 3. TOTAL DESARROLLO

| Categoría | Horas | Costo (USD) |
|-----------|-------|-------------|
| Blockchain e infraestructura | 630 | 47 250 |
| Node.js | 750 | 56 250 |
| .NET | 2 131 | 159 825 |
| Web (HTML/JS) | 792 | 59 400 |
| Móvil | 350 | 26 250 |
| DevOps / documentación | 85 | 6 375 |
| **TOTAL DESARROLLO** | **4 738** | **355 350** |

---

## 4. LICENCIAS Y SOFTWARE

| Concepto | Tipo | Costo anual (USD) |
|----------|------|--------------------|
| Node.js, Express, React, .NET (runtime) | OSS | 0 |
| Paquetes npm (express, fastify, sequelize, etc.) | OSS (MIT, ISC, etc.) | 0 |
| NuGet (.NET) | OSS / Community | 0 |
| React Native, ethers | OSS | 0 |
| SQL Server (si se usa Standard/Enterprise) | Licencia | 1 500–15 000 |
| Visual Studio (si Professional/Enterprise) | Licencia | 0–2 500 |
| **Subtotal licencias (escenario OSS + SQL Standard)** | | **1 500–5 000** |

---

## 5. INFRAESTRUCTURA (estimación anual)

### 5.1 Servidores y red

| Concepto | Especificación | Costo anual (USD) |
|----------|----------------|-------------------|
| Nodo principal (VPS/dedicado) | 4 vCPU, 8 GB RAM, 200 GB | 600–1 200 |
| Nodos respaldo (4 × VPS) | 2 vCPU, 4 GB c/u | 800–1 600 |
| Servidor apps (Node + .NET) | 4 vCPU, 16 GB, 500 GB | 800–1 500 |
| Balanceador / proxy | Incluido o LB gestionado | 0–400 |
| **Subtotal servidores** | | **2 200–4 700** |

### 5.2 Bases de datos

| Concepto | Costo anual (USD) |
|----------|--------------------|
| MySQL/PostgreSQL gestionado (SmartSchool, etc.) | 200–600 |
| SQLite (local, sin licencia de hosting) | 0 |
| **Subtotal BD** | **200–600** |

### 5.3 Dominios, DNS, CDN

| Concepto | Costo anual (USD) |
|----------|--------------------|
| Dominios (ierahkwa.gov, etc.) | 50–200 |
| CDN / estáticos | 0–300 |
| **Subtotal** | **50–500** |

### 5.4 Seguridad y cumplimiento

| Concepto | Costo anual (USD) |
|----------|--------------------|
| Certificado SSL (Let’s Encrypt o similar) | 0–200 |
| WAF / DDoS básico | 0–500 |
| **Subtotal** | **0–700** |

### 5.5 Total infraestructura (anual)

| Categoría | Costo anual (USD) |
|-----------|--------------------|
| Servidores | 2 200–4 700 |
| BD | 200–600 |
| Dominios / CDN | 50–500 |
| Seguridad | 0–700 |
| **TOTAL INFRAESTRUCTURA** | **2 450–6 500** |

---

## 6. SERVICIOS EXTERNOS (anual)

| Servicio | Costo anual (USD) |
|----------|--------------------|
| Chainlink (datos básicos; puede haber cap free) | 0–2 000 |
| Pyth | 0–1 000 |
| Nodemailer (SMTP, e.g. SendGrid/Mailgun) | 0–400 |
| Stripe (si activo; % + fijo) | 0–1 500 |
| SWIFT / banca (fees por transacción y conexión) | 2 000–20 000 |
| Exchanges (listado IGT; fees variables) | 5 000–50 000+ |
| **Subtotal servicios externos** | **7 000–74 900** |

---

## 7. MANTENIMIENTO Y SOPORTE (anual)

| Concepto | % sobre desarrollo o fijo | Costo anual (USD) |
|----------|----------------------------|--------------------|
| Mantenimiento (bugs, parches, dependencias) | 15% de 355 350 | 53 300 |
| Soporte L2 (usuarios, operaciones) | 80 h × 75 | 6 000 |
| **Subtotal mantenimiento** | | **59 300** |

---

## 8. CERTIFICACIONES Y AUDITORÍAS (puntual o anual)

| Concepto | Costo (USD) |
|----------|-------------|
| ISO 27001 (ref. cryptohost) | 15 000–40 000 |
| SOC 2 Type II | 20 000–60 000 |
| Auditoría de smart contracts / blockchain | 10 000–30 000 |
| **Subtotal (una vez o cada 2–3 años)** | **45 000–130 000** |

---

## 9. RESUMEN DE COSTOS

### 9.1 Una sola vez (desarrollo + certificaciones opcionales)

| Concepto | Costo (USD) |
|----------|-------------|
| **Desarrollo total** | **355 350** |
| Certificaciones (opcional, prorrateo 1 año) | 22 500–65 000 |
| **TOTAL UNA VEZ** | **377 850 – 420 350** |

### 9.2 Costos recurrentes (anual)

| Concepto | Rango anual (USD) |
|----------|--------------------|
| Licencias | 1 500–5 000 |
| Infraestructura | 2 450–6 500 |
| Servicios externos | 7 000–74 900 |
| Mantenimiento y soporte | 59 300 |
| **TOTAL RECURRENTE ANUAL** | **70 250–145 700** |

### 9.3 Resumen en una tabla

| Partida | Una vez (USD) | Anual (USD) |
|---------|----------------|-------------|
| Desarrollo | 355 350 | — |
| Licencias | — | 1 500–5 000 |
| Infraestructura | — | 2 450–6 500 |
| Servicios externos | — | 7 000–74 900 |
| Mantenimiento y soporte | — | 59 300 |
| Certificaciones (prorrateo opcional) | 22 500–65 000 | — |
| **TOTAL** | **377 850–420 350** | **70 250–145 700** |

---

## 10. VALOR DE REPOSICIÓN (orden de magnitud)

Si hubiera que reconstruir el sistema desde cero con el mismo alcance:

- **Desarrollo:** ~355 000 USD  
- **Primer año de operación (infra + servicios + mantenimiento):** ~70 000–146 000 USD  
- **Valor de reposición (desarrollo + año 1):** **~425 000–501 000 USD**

---

## 11. NOTAS

1. **Horas:** Son estimaciones por tipo de componente; no hay registro detallado de horas reales.
2. **Tasa:** 75 USD/h; en mercados premium (Canadá, EE. UU., UE) puede subir a 100–150 USD/h.
3. **Infra:** Asume cloud/VPS estándar; servidores dedicados o Tier IV suben el rango.
4. **SWIFT y exchanges:** Dependen de volumen y acuerdos; los rangos son ilustrativos.
5. **Certificaciones:** Se consideran puntuales o cada 2–3 años; el prorrateo es orientativo.
6. **Moneda:** Todo en USD.  
7. **Actualización:** Recomendado revisar esta auditoría al menos una vez al año.

---

**Sovereign Government of Ierahkwa Ne Kanienke**  
*Auditoría de costos del sistema – 19 Enero 2026*
