# REPORTE COMPLETO – TODO EL SOFTWARE Y COSTOS

**Sovereign Government of Ierahkwa Ne Kanienke**  
**Fecha:** 19 Enero 2026  
**Contenido:** Inventario de todo el software desarrollado y costos asociados (desarrollo, licencias, infra, servicios, mantenimiento)

---

# PARTE 1 – INVENTARIO DE TODO EL SOFTWARE

---

## 1. BLOCKCHAIN E INFRAESTRUCTURA

| # | Software | Ubicación | Tecnología | Descripción |
|---|----------|-----------|------------|-------------|
| 1 | **Ierahkwa Futurehead Mamey Node** | `node/server.js` | Node.js, Express | Blockchain ISB, Chain ID 77777, RPC JSON-RPC, minado de bloques, cuentas, gas 0 |
| 2 | Genesis + estado | `node/genesis.json`, `node/ierahkwa-futurehead-mamey-node.json` | JSON | Configuración de red y genesis |
| 3 | **API REST** | `node/server.js` | Express | /api/v1/stats, /api/v1/tokens, /api/v1/backup/create |
| 4 | **Token Factory API** | `node/server.js` | — | POST /api/v1/tokens/create, GET /api/v1/tokens/custom |
| 5 | **Bridge API** | `node/server.js` | — | /api/v1/bridge/chains, /api/v1/bridge/quote, /api/v1/bridge/transfer |
| 6 | **Membership API** | `node/server.js` | — | /api/v1/membership/register, /api/v1/membership/stats, /api/v1/membership/invest |
| 7 | **Voting API** | `node/server.js` | — | /api/v1/voting/proposals, /api/v1/voting/vote |
| 8 | **Gamification API** | `node/server.js` | — | /api/v1/gamification/achievements, /api/v1/gamification/leaderboard |
| 9 | **VIP Transactions API** | `node/server.js` | — | /api/v1/vip/transactions, /api/v1/vip/stats, /api/v1/vip/report, /api/v1/vip/modules |
| 10 | **i18n API** | `node/server.js` | — | /api/v1/i18n/:lang (EN, ES, MOH, TAI) |
| 11 | **Rutas estáticas y HTML** | `node/server.js` | — | /, /platform, /vip, /tradex, /bridge, /membership, /voting, /rewards, /token-factory, /bdet-bank, /cryptohost, /central-banks, /admin, /analytics, /backup, /monitor, etc. |
| 12 | **CryptoHost (lógica)** | `node/server.js` + `cryptohost-infrastructure.json` | — | Hot/cold, multi-sig, HSM (ref.), bridges 6–7 cadenas |
| 13 | **Oracles (integración)** | `cryptohost-infrastructure.json` | — | Chainlink, Pyth |

**Subtotal desarrollo (Blockchain/Infra):** 630 h · **47 250 USD**

---

## 2. APLICACIONES NODE.JS

| # | Software | Ubicación | Tecnología | Descripción |
|---|----------|-----------|------------|-------------|
| 14 | **ierahkwa-shop** | `ierahkwa-shop/` | Fastify, Socket.io, bcryptjs | E‑commerce, POS, chat en tiempo real, IGT-MARKET |
| 15 | **inventory-system** | `inventory-system/` | Express, sql.js, exceljs, pdfkit, archiver, ejs | Inventario, export Excel/PDF, categorías, movimientos |
| 16 | **pos-system** | `pos-system/` | Express, bcryptjs, express-session | POS restaurante, mesas, reportes, multi‑idioma |
| 17 | **smart-school-node** | `smart-school-node/` | Express, Sequelize, mysql2, pg, jsonwebtoken, nodemailer | Escuela multi-tenant, alumnos, profesores, cuotas, facturas, auth |
| 18 | **forex-trading-server** | `forex-trading-server/` | Node.js | Servidor de trading Forex |
| 19 | **image-upload** | `image-upload/` | Node.js | Carga y gestión de imágenes |
| 20 | **server (raíz)** | `server/` | Node.js | Servidor auxiliar |

**Subtotal desarrollo (Node.js):** 750 h · **56 250 USD**

---

## 3. APIs Y SERVICIOS .NET

| # | Software | Ubicación | Capas | Descripción |
|---|----------|-----------|-------|-------------|
| 21 | **TradeX** | `TradeX/` | API, Core, Infra | Exchange, Swagger |
| 22 | **DocumentFlow** | `DocumentFlow/` | API, Core, Infra | Documentos, carpetas, plantillas, workflow |
| 23 | **NET10** | `NET10/` | API, Core, Infra | Servicios NET 10 |
| 24 | **SpikeOffice** | `SpikeOffice/` | API, Core, Infra | Dashboard, empleados, multi-tenant, SQLite |
| 25 | **HRM** | `HRM/` | API, Core, Infra | RRHH, nómina, asistencia, vacaciones, préstamos, reclutamiento, proyectos |
| 26 | **FarmFactory** | `FarmFactory/` | API, Core, Infra | Depósitos, pools agrícolas |
| 27 | **IDOFactory** | `IDOFactory/` | API, Core, Infra | Ofertas iniciales (IDO) |
| 28 | **AdvocateOffice** | `AdvocateOffice/` | API, Data, wwwroot | Legal, casos, clientes, citas, facturación, patrocinadores |
| 29 | **AppointmentHub** | `AppointmentHub/` | API, Core | Citas |
| 30 | **AuditTrail** | `AuditTrail/` | API, Core, Infra | Auditoría y trazabilidad |
| 31 | **BudgetControl** | `BudgetControl/` | API, Core, Infra | Presupuesto |
| 32 | **CitizenCRM** | `CitizenCRM/` | API, Core, Infra | CRM ciudadanos |
| 33 | **ContractManager** | `ContractManager/` | API, Core, Infra | Contratos |
| 34 | **DataHub** | `DataHub/` | API, Core, Infra | Datos |
| 35 | **DigitalVault** | `DigitalVault/` | API, Core, Infra | Bóveda digital |
| 36 | **ESignature** | `ESignature/` | API, Core, Infra | Firma electrónica |
| 37 | **FormBuilder** | `FormBuilder/` | API, Core, Infra | Formularios dinámicos |
| 38 | **MeetingHub** | `MeetingHub/` | API, Core, Infra | Reuniones |
| 39 | **NotifyHub** | `NotifyHub/` | API, Core, Infra | Notificaciones |
| 40 | **ProcurementHub** | `ProcurementHub/` | API, Core, Infra | Compras y procura |
| 41 | **ProjectHub** | `ProjectHub/` | API, Core, Infra | Proyectos |
| 42 | **ReportEngine** | `ReportEngine/` | API, Core, Infra | Motor de reportes |
| 43 | **ServiceDesk** | `ServiceDesk/` | API, Core, Infra | Mesa de ayuda |
| 44 | **AssetTracker** | `AssetTracker/` | API, Core, Infra | Activos |
| 45 | **SmartSchool** | `SmartSchool/` | Web, Persistence, Application, Domain | Escuela .NET, contabilidad, módulo Forex |
| 46 | **InventoryManager** | `InventoryManager/` | WinForms | Inventario desktop (Windows) |
| 47 | **OutlookExtractor** | `OutlookExtractor/` | API, Infra | Extracción de correo Outlook |
| 48 | **RnBCal** | `RnBCal/` | — | Calendario R’n’B |
| 49 | **AppBuilder** | `AppBuilder/` | — | Constructor de aplicaciones |

**Subtotal desarrollo (.NET):** 2 131 h · **159 825 USD**

---

## 4. PLATAFORMA WEB (HTML/JS)

### 4.1 Páginas en `platform/` (41)

| # | Archivo | URL / Uso |
|---|---------|-----------|
| 50 | index.html | Dashboard principal |
| 51 | vip-transactions.html | Transacciones VIP, 13 módulos, reporte faltantes |
| 52 | citizen-membership.html | Membership, tiers, inversión, referidos |
| 53 | rewards.html | Gamificación, recompensas |
| 54 | voting.html | Governance, votación |
| 55 | analytics-dashboard.html | Analytics en tiempo real |
| 56 | bridge.html | Cross-chain bridge |
| 57 | token-factory.html | Crear tokens IGT |
| 58 | citizen-launchpad.html | Launchpad ciudadano |
| 59 | departments.html | Departamentos |
| 60 | backup-department.html | Departamento de backup |
| 61 | debt-collection.html | Cobro de deudas |
| 62 | financial-instruments.html | Instrumentos financieros, bonos, flujos |
| 63 | central-banks.html | 4 Bancos Centrales |
| 64 | atm-manufacturing.html | Manufactura ATM |
| 65 | bitcoin-hemp.html | Bitcoin Hemp |
| 66 | mamey-futures.html | Mamey Futures |
| 67 | futurehead-group.html | Futurehead Group |
| 68 | siis-settlement.html | SIIS Settlement |
| 69 | sovereignty-education.html | Soberanía y educación |
| 70 | security-fortress.html | Security Fortress |
| 71 | leader-control.html | Panel Primer Ministro |
| 72 | settings.html | Configuración |
| 73 | video-call.html | Videollamada |
| 74 | monitor.html | Monitor |
| 75 | citizen-crm.html | CRM ciudadanos |
| 76 | documents.html | Documentos |
| 77 | esignature.html | Firma electrónica |
| 78 | wallet.html | Wallet |
| 79 | forex.html | Forex |
| 80 | cryptohost.html | CryptoHost |
| 81 | tradex.html | TradeX Exchange |
| 82 | bdet-bank.html | BDET Bank |
| 83 | login.html | Login |
| 84 | notifications.html | Notificaciones |
| 85 | secure-chat.html | Chat seguro |
| 86 | chat.html | Chat |
| 87 | government-portal.html | Portal gobierno |
| 88 | support-ai.html | Soporte AI |
| 89 | health-dashboard.html | Dashboard salud |
| 90 | admin.html | Admin |

### 4.2 Otros archivos web

| # | Archivo | Ubicación | Descripción |
|---|---------|-----------|-------------|
| 91 | vip-modules.json | `platform/` | 13 módulos VIP (Alexandrite, API to API, Bonos, CryptoHost, IBAN MT103, SWIFT, Rubí, STP MT103, Visa/MC, Venezuela, WISE, TRANSMITED IP, EXPORT) |
| 92 | IERAHKWA_PLATFORM_V1.html | raíz | Plataforma consolidada v1 |
| 93 | ADMIN_FOREX_INVESTMENT.html | raíz | Admin Forex Investment |
| 94 | IERAHKWA_FOREX_INVESTMENT.html | raíz | Forex Investment |
| 95 | commerce-business-dashboard.html | raíz | Dashboard comercio |
| 96 | RECIBIR_CRYPTOHOST_CONVERTIR_USDT.html | raíz | Recibir CryptoHost y convertir a USDT |

### 4.3 IERAHKWA-PLATFORM-DEPLOY

| # | Archivo | Ubicación | Descripción |
|---|---------|-----------|-------------|
| 97 | index.html | `IERAHKWA-PLATFORM-DEPLOY/core/` | Core |
| 98 | leader-control.html | `core/` | Líder |
| 99 | monitor.html | `core/` | Monitor |
| 100 | support-ai.html | `core/` | Soporte AI |
| 101 | performance.js, security.js | `core/` | Performance y seguridad |
| 102 | sw.js | `core/` | Service Worker |
| 103 | family-portal.html | `family-system/` | Portal familia |
| 104 | deploy.sh | raíz deploy | Script de despliegue |

### 4.4 Tokens IGT

| # | Descripción | Ubicación | Cantidad |
|---|-------------|-----------|----------|
| 105 | Tokens IGT (token.json + index.html por token) | `tokens/` | 103+ (01-IGT-PM a 29-IGT-MST y más) |

**Subtotal desarrollo (Web):** 792 h · **59 400 USD**

---

## 5. MÓVIL (React Native)

| # | Software | Ubicación | Tecnología | Descripción |
|---|----------|-----------|------------|-------------|
| 106 | **ierahkwa-mobile** | `mobile-app/` | React Native 0.73, React 18, React Navigation, ethers, axios, i18next, SVG | Dashboard, Wallet, Trade, Governance, Rewards, Bridge, 4 idiomas |

**Subtotal desarrollo (Móvil):** 350 h · **26 250 USD**

---

## 6. SCRIPTS, DEVOPS Y DOCUMENTACIÓN

### 6.1 Scripts

| # | Archivo | Descripción |
|---|---------|-------------|
| 107 | start.sh | Iniciar nodo Mamey |
| 108 | stop.sh | Detener nodo |
| 109 | status.sh | Estado del nodo |
| 110 | start.command | Inicio por doble clic (macOS) |
| 111 | install-autostart.sh | Instalar arranque automático (launchd) |
| 112 | uninstall-autostart.sh | Desinstalar autostart |
| 113 | start-all-services.sh | Iniciar nodo + plataforma (puertos 8545, 8080) |
| 114 | auto-backup.sh | Backup automático (intervalo config.) |
| 115 | auto-monitor.sh | Monitoreo |
| 116 | create-independent-copy.sh | Copia independiente del proyecto |
| 117 | generate-tokens.js | Generación de tokens IGT |

### 6.2 Documentación

| # | Archivo | Ubicación | Descripción |
|---|---------|-----------|-------------|
| 118 | REPORTE-ESTADO-COMPLETO.md | raíz | Estado de la plataforma, membership, bridge, tokens, móvil, i18n |
| 119 | REPORTE-COMPLETO-COSTOS-Y-HARDWARE.md | raíz | Costos + plan de inversión en hardware |
| 120 | AUDITORIA-COSTOS-SISTEMA.md | raíz | Auditoría de costos (desarrollo, licencias, infra, servicios) |
| 121 | STARTUP.md | raíz | Arranque (start, stop, autostart) |
| 122 | INDEX-DOCUMENTACION.md | docs/ | Índice de documentación |
| 123 | API-REFERENCE.md | docs/ | Referencia de APIs |
| 124 | DOCUMENTACION-TECNICA.md | docs/ | Documentación técnica |
| 125 | MANUAL-INSTALACION-CONFIGURACION.md | docs/ | Instalación y configuración |
| 126 | MANUAL-USUARIO.md | docs/ | Manual de usuario |
| 127 | PLANO-PLATAFORMA-01.md | docs/ | Plano de plataforma |
| 128 | PLATFORM-ARCHITECTURE.md | docs/ | Arquitectura |
| 129 | README-DOCUMENTACION.md | docs/ | README docs |
| 130 | IMPLEMENTACION-ESTADO.md | docs/ | Estado de implementación |
| 131 | LIBRERIAS-COMPONENTES.md | docs/ | Librerías y componentes |
| 132 | LEGADO-7-GENERACIONES-IERAHKWA.md | raíz | Legado 7 generaciones |
| 133 | IERAHKWA-FINAL-REPORT.md | raíz | Informe final |
| 134 | REPORTE-COMPLETO-PLATAFORMA.md | raíz | Reporte completo plataforma |
| 135 | REPORTE-PROGRESO-COMPLETO.md | raíz | Progreso |
| 136 | SOVEREIGN-PROTECTION-ARCHITECTURE.md | raíz | Arquitectura de protección |
| 137 | STACK-TECNOLOGICO-SOBERANO.md | raíz | Stack tecnológico |
| 138 | docs/index.html | docs/ | Índice HTML documentación |

### 6.3 Configuración

| # | Archivo | Descripción |
|---|---------|-------------|
| 139 | platform-config.json | Configuración de la plataforma |
| 140 | platform-services.json | Servicios de plataforma |
| 141 | exchange-trading-deck.json | Trading deck, exchanges, BDET, CryptoHost, bancos corresponsales |
| 142 | cryptohost-infrastructure.json | Infraestructura CryptoHost, DC, bridges, oracles, governance |

**Subtotal desarrollo (DevOps/Docs):** 85 h · **6 375 USD**

---

## 7. OTROS MÓDULOS

| # | Carpeta / archivo | Descripción |
|---|-------------------|-------------|
| 143 | ai/ | Integración AI (index.html, README) |
| 144 | quantum/ | Quantum (index, README) |
| 145 | IerahkwaBankPlatform/ | Plataforma banco (2 .cs) |
| 146 | auto-backup/ | Configuración y scripts de auto-backup |
| 147 | backup-system/ | Sistema de backups (LIVE-BACKUP, tar.gz por plataforma, departamentos, servicios) |
| 148 | logs/ | Logs del sistema |

---

# PARTE 2 – COSTOS DE TODO

---

## 8. COSTO DE DESARROLLO (POR CATEGORÍA)

| Categoría | Horas | Tasa (USD/h) | Costo (USD) |
|-----------|-------|--------------|-------------|
| Blockchain e infraestructura | 630 | 75 | 47 250 |
| Aplicaciones Node.js | 750 | 75 | 56 250 |
| APIs y servicios .NET | 2 131 | 75 | 159 825 |
| Plataforma web (HTML/JS, tokens, deploy) | 792 | 75 | 59 400 |
| Móvil (React Native) | 350 | 75 | 26 250 |
| Scripts, DevOps, documentación | 85 | 75 | 6 375 |
| **TOTAL DESARROLLO** | **4 738** | **75** | **355 350** |

---

## 9. COSTO DE DESARROLLO (DETALLE POR COMPONENTE)

### 9.1 Blockchain e infra (47 250 USD)

| Concepto | Horas | Costo (USD) |
|----------|-------|-------------|
| Nodo Mamey (blockchain, RPC, API, tokens, genesis) | 180 | 13 500 |
| Lógica de bloques, cuentas, tx, gas | 120 | 9 000 |
| APIs (tokens, bridge, membership, voting, gamification, backup, VIP) | 150 | 11 250 |
| CryptoHost (concepto, integración, seguridad) | 80 | 6 000 |
| Bridge multi-chain | 100 | 7 500 |

### 9.2 Node.js (56 250 USD)

| Aplicación | Horas | Costo (USD) |
|------------|-------|-------------|
| ierahkwa-shop | 200 | 15 000 |
| inventory-system | 120 | 9 000 |
| pos-system | 80 | 6 000 |
| smart-school-node | 250 | 18 750 |
| forex-trading-server | 60 | 4 500 |
| image-upload, server | 40 | 3 000 |

### 9.3 .NET (159 825 USD)

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
| SmartSchool | 320 | 24 000 |
| InventoryManager (WinForms) | 150 | 11 250 |
| OutlookExtractor | 80 | 6 000 |
| RnBCal, AppBuilder | 100 | 7 500 |

### 9.4 Web (59 400 USD)

| Concepto | Horas | Costo (USD) |
|----------|-------|-------------|
| Páginas platform/ (41) | 410 | 30 750 |
| Otras HTML raíz (5) | 60 | 4 500 |
| IERAHKWA-PLATFORM-DEPLOY (7) | 56 | 4 200 |
| Páginas token (103) | 206 | 15 450 |
| VIP Transactions, módulos, reportes | 60 | 4 500 |

### 9.5 Móvil (26 250 USD)

| Concepto | Horas | Costo (USD) |
|----------|-------|-------------|
| ierahkwa-mobile (React Native) | 350 | 26 250 |

### 9.6 DevOps / documentación (6 375 USD)

| Concepto | Horas | Costo (USD) |
|----------|-------|-------------|
| start, stop, status, install-autostart, launchd | 20 | 1 500 |
| auto-backup, auto-monitor | 25 | 1 875 |
| Documentación (REPORTE-ESTADO, STARTUP, docs/, etc.) | 40 | 3 000 |

---

## 10. LICENCIAS (ANUAL)

| Concepto | Tipo | Costo anual (USD) |
|----------|------|--------------------|
| Node.js, Express, React, .NET runtime | OSS | 0 |
| Paquetes npm (MIT, ISC, etc.) | OSS | 0 |
| NuGet (.NET) | OSS / Community | 0 |
| React Native, ethers | OSS | 0 |
| SQL Server (si Standard/Enterprise) | Licencia | 1 500 – 15 000 |
| Visual Studio (si Pro/Enterprise) | Licencia | 0 – 2 500 |
| **Subtotal** | | **1 500 – 5 000** (escenario OSS + SQL estándar) |

---

## 11. INFRAESTRUCTURA (ANUAL)

| Concepto | Costo anual (USD) |
|----------|--------------------|
| Nodo principal (VPS/dedicado) | 600 – 1 200 |
| Nodos respaldo (4 × VPS) | 800 – 1 600 |
| Servidor apps (Node + .NET) | 800 – 1 500 |
| Balanceador / proxy | 0 – 400 |
| MySQL/PostgreSQL gestionado | 200 – 600 |
| Dominios, DNS, CDN | 50 – 500 |
| SSL, WAF, DDoS básico | 0 – 700 |
| **Total infraestructura** | **2 450 – 6 500** |

---

## 12. SERVICIOS EXTERNOS (ANUAL)

| Servicio | Costo anual (USD) |
|----------|--------------------|
| Chainlink | 0 – 2 000 |
| Pyth | 0 – 1 000 |
| SMTP (SendGrid/Mailgun, etc.) | 0 – 400 |
| Stripe (si activo) | 0 – 1 500 |
| SWIFT / banca | 2 000 – 20 000 |
| Exchanges (listado IGT) | 5 000 – 50 000+ |
| **Total servicios externos** | **7 000 – 74 900** |

---

## 13. MANTENIMIENTO Y SOPORTE (ANUAL)

| Concepto | Cálculo | Costo anual (USD) |
|----------|---------|--------------------|
| Mantenimiento (parches, deps, bugs) | 15% de 355 350 | 53 300 |
| Soporte L2 (usuarios, operaciones) | 80 h × 75 USD/h | 6 000 |
| **Total mantenimiento** | | **59 300** |

---

## 14. CERTIFICACIONES Y AUDITORÍAS (PUNTUAL, PRORRATEO 1 AÑO)

| Concepto | Costo (USD) | Prorrateo 1 año (USD) |
|----------|-------------|------------------------|
| ISO 27001 | 15 000 – 40 000 | 15 000 – 40 000 |
| SOC 2 Type II | 20 000 – 60 000 | 20 000 – 60 000 |
| Auditoría smart contracts / blockchain | 10 000 – 30 000 | 10 000 – 30 000 |
| **Subtotal (si se hace todo en año 1)** | **45 000 – 130 000** | **22 500 – 65 000** (prorrateo conservador) |

---

# PARTE 3 – RESUMEN TOTAL

---

## 15. UNA SOLA VEZ

| Partida | Costo (USD) |
|---------|-------------|
| **Desarrollo (todo el software)** | **355 350** |
| Certificaciones (opcional, prorrateo 1 año) | 22 500 – 65 000 |
| **TOTAL UNA VEZ** | **377 850 – 420 350** |

---

## 16. RECURRENTE ANUAL

| Partida | Costo anual (USD) |
|---------|--------------------|
| Licencias | 1 500 – 5 000 |
| Infraestructura | 2 450 – 6 500 |
| Servicios externos | 7 000 – 74 900 |
| Mantenimiento y soporte | 59 300 |
| **TOTAL RECURRENTE** | **70 250 – 145 700** |

---

## 17. TABLA GLOBAL – TODO EL COSTE

| Concepto | Una vez (USD) | Anual (USD) |
|----------|----------------|-------------|
| Desarrollo (software) | 355 350 | — |
| Certificaciones (opc.) | 22 500 – 65 000 | — |
| Licencias | — | 1 500 – 5 000 |
| Infraestructura | — | 2 450 – 6 500 |
| Servicios externos | — | 7 000 – 74 900 |
| Mantenimiento y soporte | — | 59 300 |
| **TOTAL** | **377 850 – 420 350** | **70 250 – 145 700** |

---

## 18. VALOR DE REPOSICIÓN (REFERENCIA)

Si hubiera que rehacer **todo el software** desde cero con el mismo alcance:

- **Desarrollo:** 355 350 USD  
- **Primer año (infra + servicios + mantenimiento):** 70 250 – 145 700 USD  
- **Valor de reposición (desarrollo + año 1):** **425 600 – 501 050 USD**

---

## 19. CONTEO DE COMPONENTES

| Tipo | Cantidad |
|------|----------|
| Componentes blockchain/infra | 13 |
| Aplicaciones Node.js | 7 |
| APIs/servicios .NET | 29 |
| Páginas HTML platform/ | 41 |
| Otros HTML + deploy | 16 |
| Tokens IGT (con página) | 103+ |
| App móvil | 1 |
| Scripts | 11 |
| Documentos (MD, HTML) | 22+ |
| Archivos de configuración | 4 |
| **Total ítems inventariados** | **148+** |

---

**Sovereign Government of Ierahkwa Ne Kanienke**  
*Reporte completo de software y costos – 19 Enero 2026*
