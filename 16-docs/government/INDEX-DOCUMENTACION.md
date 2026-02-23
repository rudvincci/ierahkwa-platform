# ÍNDICE DE DOCUMENTACIÓN

## Ierahkwa Futurehead Platform
### Documentación Completa de Todos los Módulos

---

## 📚 DOCUMENTACIÓN GENERAL

| Documento | Descripción | Ubicación |
|-----------|-------------|-----------|
| **Estado del backend propio** | Resumen: qué es nuestro backend, dónde está (8545, HTTP sin cert), cómo arrancar y comprobar | `docs/ESTADO-BACKEND.md` |
| **Nuestros servicios y banco (todo en el Node)** | Node sin certificado, rutas, APIs, backend 100% propio | `docs/SERVICIOS-NUESTRO-NODE.md` |
| **Plano de estructura (cómo trabaja todo)** | Capas, flujo, orquestación start.sh, config, dónde está cada código | `docs/PLANO-ESTRUCTURA-COMPLETA-COMO-TRABAJA-TODO.md` |
| **Índice de toda la documentación** | Índice maestro por tema (plano, operación, seguridad, soberanía, IA, referencia) | `docs/DOCUMENTACION-COMPLETA-INDICE.md` |
| **Índice de todo el código** | Referencia única: Node, .NET, Rust, Solidity, HTML/JS, scripts | `INDICE-CODIGO-PROYECTO.md` (raíz) |
| Mapa Node y blockchain | Dónde está cada tipo de código (Node, blockchain, demás) | `MAPA-CODIGO-NODE-BLOCKCHAIN-Y-DEMAS.md` (raíz) |
| Arquitectura Completa | Visión general del sistema | `docs/PLATFORM-ARCHITECTURE.md` |
| Manual de Usuario | Guía para usuarios | `docs/MANUAL-USUARIO.md` |
| Instalación | Guía de instalación | `docs/MANUAL-INSTALACION-CONFIGURACION.md` |
| Documentación Técnica | Detalles técnicos | `docs/DOCUMENTACION-TECNICA.md` |
| Soberanía y reconocimiento | Visión, legalidad, Américas unidas | `docs/SOBERANIA-RECONOCIMIENTO-LEGAL.md` |
| **Interconexión global** | Ellos vienen a nosotros; nosotros somos la interconexión global (hub) | `docs/INTERCONEXION-GLOBAL.md` |
| **Referencias herramientas ecosistema** | Repos trending: nanobot (Clawdbot, Python), claude-mem (plugin contexto Claude, TypeScript) | `docs/REFERENCIAS-HERRAMIENTAS-ECOSISTEMA.md` |
| **Casino — La mejor plataforma del mundo, todos pueden jugar** | IA, RNG, Chetu referencia, capacidades (pagos, crypto, apuestas deportivas, cumplimiento), API `/api/casino/plataforma-mundial` | `docs/CASINO-PLATAFORMA-MUNDIAL.md` |
| **Ecosistema modular Futurehead** | Hub, departamentos (Casino, Real Estate, Travel, Luxury Exchange), puente, firewall, herramientas (MedusaJS, NuxGame, Amadeus, Uniswap, etc.), Progreso Citizen (Bronce/Oro/Diamante), API `/api/v1/sovereignty/ecosistema-futurehead` | `docs/ECOSISTEMA-MODULAR-FUTUREHEAD.md` |
| **Arquitectura de soberanía — Rol del blockchain** | Qué va al blockchain (ISB): identidad, gobernanza, tokens, certificación. Qué no migrar: Banco, SWIFT, TradeX, Casino (cada uno en su lugar). Hub único + Sovereignty Bridge. **Migración:** si se hizo con otro protocolo, hay que migrar a nosotros (ISB). API `GET /api/v1/sovereignty/migracion-protocolos` | `docs/ARQUITECTURA-SOBERANIA-ROL-BLOCKCHAIN.md` |
| **Reglas y licencias para todos** | Reglas generales y por tipo (ciudadanos, plataformas, departamentos, banco, casino, exchange). Categorías de licencias (Sovereign License Authority). API `GET /api/v1/sovereignty/reglas-y-licencias` | `docs/REGLAS-Y-LICENCIAS-PARA-TODOS.md` |
| **Próximos pasos soberanía** | Migración tokens, quemas, Sovereignty Bridge, auditoría licencias, Real Estate/Travel, staking. Bridge: `POST /api/v1/sovereignty/bridge/event`, `GET /api/v1/sovereignty/bridge/events` | `docs/PROXIMOS-PASOS-SOBERANIA.md` |
| **Whitepaper Futurehead Trust (2026)** | Visión, tres pilares (Real Estate, Casino Bet, Lifestyle), modelo Citizen (Starter/Owner/Tycoon), firewall, tokenómica (60% reserva, 15% experiencias), próximos pasos. API `/api/v1/sovereignty/whitepaper-futurehead` | `docs/WHITEPAPER-FUTUREHEAD-TRUST-ECOSYSTEM-2026.md` |
| **MultiEstate (CodeCanyon) — Sin certificar · Real Estate multitenant soberano** | MultiEstate no se certifica (licencia 3ra, pagos externos). Cómo sí: plataforma multitenant real estate en nuestro Node + BDET/IGT + pagos soberanos + IGT-REALTY. | `docs/MULTIESTATE-REAL-ESTATE-SOBERANO.md` |
| **Estaty – App Flutter Real Estate y conexión con nuestro Node** | Conexión Estaty (Flutter) al Node: HTTP/HTTPS, certificado propio, API que espera la app (propiedades, proyectos, agentes). | `docs/ESTATY-REAL-ESTATE-APP-NODE.md` |
| **Plan de Implementación Futurehead (2026)** | 5 fases: (1) Cimiento legal y tokenización, (2) Hub casino/wallets segregadas, (3) Viajes/Amadeus/Disney, (4) Exchange lujo Ferrari/Botes, (5) Dashboard y gobernanza. API `/api/v1/sovereignty/plan-implementacion` | `docs/PLAN-IMPLEMENTACION-FUTUREHEAD-2026.md` |
| **Beneficios para empleados (Trust Companies y Citizens)** | Incentivos para empleados: Trust Companies y Citizens pueden rentar/ofrecer bonos, Disney, Rent-a-Car, lujo y formación. API `/api/v1/sovereignty/beneficios-empleados` | `docs/BENEFICIOS-EMPLEADOS-TRUST-CITIZENS.md` |
| **Ofertas corporativas para Citizens** | Sirve para ofrecer más cosas corporativas a los Citizens: incentivos, viajes, formación, lujo y gobernanza a nivel empresa. API `/api/v1/sovereignty/ofertas-corporativas` | `docs/OFERTAS-CORPORATIVAS-PARA-CITIZENS.md` |
| **Software algoritmos apuestas deportivas** | Qué es, por qué usarlo, características, 12 mejores programas, cómo elegir, FAQ (Smartico, ZCode, OddsJam, etc.) | `docs/APUESTAS-DEPORTIVAS-SOFTWARE-ALGORITMOS.md` |
| Evidencia legal (PDF) | Dónde guardar el PDF de soberanía | `docs/legal/README.md` |
| Próximos pasos técnicos | Node 18+, start.sh, deploy, live | `docs/PROXIMOS-PASOS-TECNICOS.md` |
| **Producción lista** | Checklist único: start.sh, config, monitoreo — sin duplicaciones | `docs/PRODUCTION-LISTO.md` |
| **100% Production (checklist definitivo)** | Go/no-go: env, datos (incl. bank-registry), arranque, verificación script, 1 Settlement + 4 nodos, BDET back, seguridad | `docs/PLATAFORMA-100-PRODUCTION.md` |
| **100% Production Live** | Checklist datos + endpoints; verificar-production-live.js; /api/v1/production/status y /ready | `docs/100-PRODUCTION-LIVE.md` |
| **Certificados SSL/TLS** | Rutas nginx, Let's Encrypt (certbot), self-signed, comercial; renovación | `docs/CERTIFICADOS-SSL-TLS.md` |
| **Preparar todo READY** | Un comando: prepare-ready.sh (pre-vuelo + datos + verificación live); flujo listo para producción | `docs/PREPARAR-READY.md` |
| **De mi parte: comprobar y salir a production live hoy** | Pasos concretos (terminal + navegador) y URLs para comprobar y poner en vivo | `docs/DE-MI-PARTE-PRODUCTION-LIVE-HOY.md` |
| **Asegurar 100% y production en cada plataforma** | Dos verificaciones (links/rutas/data + datos+endpoints); script `scripts/asegurar-100-production.sh`; cada negocio su página, mismo backend | `docs/ASEGURAR-100-PRODUCTION-CADA-PLATAFORMA.md` |
| **Carpeta negocio Futurehead Trust** | Todo el negocio independiente: docs + data (ecosistema, whitepaper, plan, beneficios, ofertas corporativas, casino) | `futurehead-trust-negocio/README.md` |
| **Pruebas reales** | Documentación honesta: qué se probó, qué demuestra cada prueba, resultados reales | `docs/DOCUMENTACION-PRUEBAS-REALES.md` |
| **Reporte global** | Velocidad, seguridad, resistencia, fortaleza y comparación con el mercado (competencia) | `docs/REPORTE-GLOBAL-VELOCIDAD-SEGURIDAD-RESISTENCIA-FORTALEZA-Y-MERCADO.md` |
| **Plan de Negocios — Tokens pre-launch + whitepaper** | Cada token IGT debe tener página de pre-lanzamiento y whitepaper propio; script `scripts/generar-whitepapers-faltantes.js` para generar whitepapers desde token.json | `docs/PLAN-NEGOCIOS-TOKENS-PRE-LAUNCH-WHITEPAPER.md` |
| **Plan de atractivo de inversión por token** | Qué debe tener cada token para que los inversionistas quieran invertir: propuesta de valor, "por qué invertir", caso de inversión en whitepaper, plantilla y checklist | `docs/PLAN-INVERSION-ATRACTIVO-POR-TOKEN.md` |
| **Principio: Negocios separados — Dashboard único — Backend compartido** | Solo en el main dashboard se mezcla todo; cada negocio tiene su propia página independiente; todos comparten el mismo backend y data para comunicaciones | `docs/PRINCIPIO-NEGOCIOS-SEPARADOS-DASHBOARD-UNICO.md` |
| **Qué más implementaría (recomendado)** | Priorizado: tokens (caso de inversión, API lista), dashboard, producción (404, smoke test), seguridad, IA/BDET, RWA | `docs/QUE-MAS-IMPLEMENTARIA-RECOMENDADO.md` |
| **Qué más implementamos (lista priorizada)** | Lista única: estado dashboard, caso de inversión, CORS/JWT, backup, BDET, Logit/Blockchage, RWA, developer portal | `docs/QUE-MAS-IMPLEMENTAMOS.md` |
| **Monitoreo y renta por plataforma** | Monitoreo: un lugar por plataforma (sin mezclar). `GET /api/v1/admin/monitoring?platform=casino|bdet|treasury|financial-center`. Renta: dashboard unificado en `/platform`; cada plataforma ve solo sus servicios. `GET /api/v1/platform/rent?platform=...`. Ver `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md` (Reglas, licencias y enlaces). | `node/public/admin-monitoring.html`, `platform/index.html#commercialServicesSection` |
| **Qué más implementamos — Siguiente ola** | Resumen reciente (monitoreo, renta) y lista priorizada: estado producción, auditoría licencias, developer portal, formularios BDET, compliance, CORS/JWT, backup. | `docs/QUE-MAS-IMPLEMENTAMOS-SIGUIENTE.md` |
| **Qué implementamos para ser los mejores globales** | Roadmap priorizado: visibilidad (estado prod, health AI Hub, developer portal), BDET formularios/compliance, tokens, RWA, SDKs, MameyNode, SICB. Referencias a FALTANTES y reporte global. | `docs/QUE-IMPLEMENTAMOS-PARA-SER-LOS-MEJORES-GLOBALES.md` |
| **Stack de software libre** | Ejemplos (Linux, Firefox, LibreOffice, PostgreSQL/MySQL, WordPress), ventajas/desventajas, qué usa el proyecto y mitigaciones. | `docs/SOFTWARE-LIBRE-STACK.md` |
| **LibreOffice headless (opcional)** | Conversión DOCX/ODT/ODS → PDF en servidor con software libre. | `docs/LIBREOFFICE-CONVERSION.md` |

---

## 🏛️ SOVEREIGN DEPARTMENTS (4 Bancos + SIIS)

| Departamento | README | Descripción |
|--------------|--------|-------------|
| 4 Central Banks | `platform/central-banks.html` | Sistema bancario soberano |
| SIIS Settlement | `platform/siis-settlement.html` | Liquidación internacional |
| Debt Collection | `platform/debt-collection.html` | Cobro de deudas G2G |
| Sovereignty Education | `platform/sovereignty-education.html` | Tratados y educación |

---

## 🚀 FUTUREHEAD GROUP

| Subsidiaria | README | Descripción |
|-------------|--------|-------------|
| Futurehead Group | `platform/futurehead-group.html` | Holding empresarial |
| Mamey Futures | `platform/mamey-futures.html` | Trading & Futures |
| Bitcoin Hemp | `platform/bitcoin-hemp.html` | Crypto & Agriculture |
| ATM Manufacturing | `platform/atm-manufacturing.html` | Hardware financiero |
| BDET Bank | `platform/bdet-bank.html` | Banco de desarrollo |
| **Arquitectura BHBK** | `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md` | Departamentos estratégicos del banco central indígena (Tesorería, RWA, Riesgos, Nodo 8545), servicios al ciudadano, matriz de bots |
| **BHBK Architecture (EN)** | `docs/BHBK-ARCHITECTURE-INDIGENOUS-CENTRAL-BANK.md` | Indigenous Central Bank: 2026 goal, SIIS + 4 nodes (Eagle/Quetzal/Condor/Caribbean), departments, citizen services, bot matrix |

---

## 📦 LOGÍSTICA SOBERANA (SLS / CDE)

| Recurso | Ubicación | Descripción |
|---------|-----------|-------------|
| **Blockchage · Logit · Mall Digital** | `docs/BLOCKCHAGE-LOGIT-MALL-DIGITAL.md` | Correo 100% digital (Blockchage), mall digital solo para deliveries, franquicia Logit en cada punto Américas (deliveries, recogido cash banco, seguros soberanos) |
| **Whitepaper SLS y LOGi (Production 100%)** | `docs/WHITEPAPER-SLS-LOGI-PRODUCTION.md` | Visión, arquitectura, protocolos por producto, token LOGi en ISB, APIs, seguridad y producción |
| Panel Logística | `platform/logistics.html` · rutas `/logistics`, `/cde`, `/sls`, `/delivery` | Continental Delivery Engine, manifiestos, movimiento en red, balance LOGi |
| API SLS | `/api/v1/logistics` (Nodo 8545) | Health, status, regions, nodes, transport-modes (AIR/SEA/LAND, door-to-door), deliveries, protocols, movement/network, token, manifest, order, track, move, customs, deliver |
| Token LOGi | ISB (mismo blockchain) · `platform-tokens.json` id 102 | Tarifas, recompensas y créditos on-chain; balance/transfer en `/api/v1/tokens/LOGi/...` |

---

## 💰 FINANZAS & TRADING

| Recurso | Ubicación | Descripción |
|---------|-----------|-------------|
| Banca abierta y códigos | `docs/BANCA-ABIERTA-Y-CODIGO-REFERENCIA.md` | Open Banking, software abierto (Open Bank Project, MyBanco, Odoo), IBAN/SWIFT/código entidad |
| Estándares de cifrado y banca abierta | `docs/ESTANDARES-CIFRADO-BANCA-ABIERTA.md` | AES-256, TLS, VeraCrypt, OAuth/JWE, ISO 20022, mensajería (Signal), post-cuántico |
| CryptoHost y servidores globales | `docs/CRYPTOHOST-Y-SERVIDORES-GLOBALES-REFERENCIA.md` | CryptoHost (hosting cripto / procesador propio), ISO 20022/MT103, S2S, VeraCrypt, Proxmox VE |
| **Bancos: unificado vs múltiple** | `docs/BANCOS-UNIFICADO-VS-MULTIPLE.md` | Por qué unificar, main admin en el Node, registro central `node/data/bank-registry.json`, API `GET /api/v1/bdet/bank-registry` |
| **1 Settlement + 4 Bancos Centrales** | `docs/CUATRO-NODOS-REGIONES.md` | Un nodo = International Settlement (SIIS); 4 nodos = bancos centrales independientes (Águila, Quetzal, Cóndor, Caribe); PM2 `ecosystem.4regions.config.js`, `/health` con `role` y `region` |

| Módulo | README | Puerto | Token |
|--------|--------|--------|-------|
| TradeX Exchange | `TradeX/README.md` | 5054 | IGT-EXCHANGE |
| NET10 DeFi | `NET10/README.md` | 5071 | IGT-DEFI |
| FarmFactory | `FarmFactory/README.md` | 5061 | IGT-STAKE |
| CryptoHost | `platform/cryptohost.html` | - | - |
| Forex Trading | `forex-trading-server/` | - | IGT-TRADE |

---

## 🛒 COMERCIO & NEGOCIOS

| Módulo | README | Puerto | Token |
|--------|--------|--------|-------|
| Ierahkwa Shop | `ierahkwa-shop/README.md` | 3100 | IGT-MCT |
| POS System | `pos-system/README.md` | - | - |
| Inventory | `InventoryManager/` | - | - |
| RnBCal | `RnBCal/README.md` | 5055 | - |

---

## 📄 DOCUMENTOS & COMUNICACIÓN

| Módulo | README | Puerto | Token |
|--------|--------|--------|-------|
| DocumentFlow | `DocumentFlow/README.md` | - | IGT-DOCFLOW |
| E-Signature | `ESignature/README.md` | - | IGT-ESIGN |
| OutlookExtractor | `OutlookExtractor/README.md` | - | - |

---

## 👥 RRHH & OFICINA

| Módulo | README | Puerto | Token |
|--------|--------|--------|-------|
| SpikeOffice | `SpikeOffice/README.md` | 5056 | IGT-MLE |
| HRM | `HRM/README.md` | - | - |

---

## 🎓 EDUCACIÓN & HERRAMIENTAS

| Módulo | README | Puerto | Token |
|--------|--------|--------|-------|
| SmartSchool | `SmartSchool/README.md` | - | IGT-ME |
| SmartSchool Node | `smart-school-node/README.md` | 3000 | IGT-EDU |
| AppBuilder | `AppBuilder/README.md` | 5060 | - |
| IDO Factory | `IDOFactory/README.md` | 5097 | IGT-LAUNCHPAD |

---

## ⚖️ LEGAL

| Módulo | README | Puerto | Token |
|--------|--------|--------|-------|
| Advocate Office | `AdvocateOffice/README.md` | 3010 | IGT-LEGAL |

---

## 🏛️ GOVERNMENT OPERATIONS

| Módulo | README | Descripción |
|--------|--------|-------------|
| AssetTracker | `AssetTracker/README.md` | Gestión de activos |
| AuditTrail | `AuditTrail/README.md` | Auditoría centralizada |
| BudgetControl | `BudgetControl/README.md` | Control presupuestario |
| CitizenCRM | `CitizenCRM/README.md` | CRM ciudadanos |
| ContractManager | `ContractManager/README.md` | Gestión contratos |
| DataHub | `DataHub/README.md` | Data warehouse |
| DigitalVault | `DigitalVault/README.md` | Bóveda digital |
| FormBuilder | `FormBuilder/README.md` | Constructor formularios |
| NotifyHub | `NotifyHub/README.md` | Notificaciones |
| ProcurementHub | `ProcurementHub/README.md` | Compras/licitaciones |
| ReportEngine | `ReportEngine/README.md` | Motor de reportes |
| ServiceDesk | `ServiceDesk/README.md` | Mesa de ayuda |

---

## 🤖 IA SOBERANA (modelos locales, todo propio)

| Documento | Descripción |
|-----------|-------------|
| Comparación de modelos LLM | `docs/MODELOS-LLM-COMPARACION.md` — Modelo 6 (full) y 9 (fast), asignación por plataforma |
| Alternativas GPT-4 código abierto | `docs/ALTERNATIVAS-GPT4-CODIGO-ABIERTO.md` — 12 alternativas (ColossalChat, Alpaca-LoRA, Vicuna, etc.) |
| Marcos y bibliotecas de IA | `docs/MARCOS-Y-BIBLIOTECAS-IA-REFERENCIA.md` — PyTorch, TensorFlow, LangChain, qué usar sin 3ros |
| Proyectos IA generativa (portafolio) | `docs/PROYECTOS-IA-GENERATIVA-PORTAFOLIO.md` — StableSAM, Alpaca-LoRA, Chat PDF, asistente voz, ML E2E |
| Modelos generativos (explicación) | `docs/MODELOS-GENERATIVOS-EXPLICACION.md` — Generativos vs discriminativos, tipos, ventajas/limitaciones |
| Modelos de machine learning | `docs/MODELOS-MACHINE-LEARNING-REFERENCIA.md` — Regresión, clasificación, árboles, clustering, métricas, Scikit-Learn |
| Servicio AI soberano | `RuddieSolution/node/services/ai-soberano.js` — Ollama, perfiles full/fast, `getProfileForPlatform` |
| Instalar Ollama + modelos 6 y 9 | `scripts/instalar-ollama.sh` |

---

## 🔬 TECNOLOGÍA AVANZADA

| Módulo | README | Descripción |
|--------|--------|-------------|
| Quantum Computer | `quantum/README.md` | Computación cuántica |
| IERAHKWA AI | `ai/README.md` | Inteligencia artificial |
| Mamey Node | `node/README.md` | Blockchain node |

---

## ⛓️ BLOCKCHAIN & TOKENS

| Recurso | Ubicación | Descripción |
|---------|-----------|-------------|
| 103 IGT Tokens | `tokens/` | Catálogo de tokens |
| Mamey Node | `node/README.md` | RPC Node |
| Genesis Block | `node/genesis.json` | Configuración inicial |

---

## 🔐 SEGURIDAD

| Módulo | README | Descripción |
|--------|--------|-------------|
| Security Fortress | `platform/security-fortress.html` | Ghost mode, AI Guardian |
| Leader Control | `platform/leader-control.html` | Panel del PM |
| Agentes de código y secretos | `docs/AGENTES-CODIGO-Y-SECRETOS-REFERENCIA.md` | OpenCode, Open SWE, Agent S, Secret-agent, Vault, Agent-security |
| Monitoreo cámaras y dispositivos | `docs/MONITOREO-CAMARAS-Y-DISPOSITIVOS-REFERENCIA.md` | NVR/VMS (ZoneMinder, go2rtc), MobSF, Wireshark, Rayhunter, LanScan, códigos Android |

---

## 💾 BACKUP

| Recurso | Ubicación | Descripción |
|---------|-----------|-------------|
| Backup Department | `platform/backup-department.html` | Sistema de respaldo |
| API Backup | `node/server.js` → `/api/v1/backup/*` | Endpoints de backup |
| Backups | `backup-system/backups/` | Archivos de backup |

---

## 📊 RESUMEN

| Categoría | Cantidad |
|-----------|----------|
| Módulos .NET | 25+ |
| Módulos Node.js | 10+ |
| Páginas HTML | 50+ |
| IGT Tokens | 103 |
| APIs | 15+ |
| Puertos activos | 12 |

---

## 🔗 ENLACES RÁPIDOS

- **Platform:** http://localhost:8545/platform
- **Tokens:** http://localhost:8545/tokens
- **Node RPC:** http://localhost:8545/rpc
- **TradeX:** http://localhost:5054
- **NET10:** http://localhost:5071
- **SpikeOffice:** http://localhost:5056

---

*Última actualización: 2026-01-28*

© 2026 Sovereign Government of Ierahkwa Ne Kanienke
