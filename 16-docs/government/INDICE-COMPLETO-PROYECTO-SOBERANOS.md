# Índice completo del proyecto — Sovereign Government of Ierahkwa Ne Kanienke

**Office of the Prime Minister · Sistema completo en esta carpeta**

Este documento es la **referencia maestra** del proyecto. Todo está en esta carpeta; **no se borra nada** hasta confirmar que todo está bien organizado y transferido.

**Memoria unificada:** Carpetas, proyectos y plataformas están definidos aquí (y en la regla `.cursor/rules/indice-completo-proyecto-soberanos.mdc`). Con esto se puede trabajar sin cargar toda la estructura en la máquina: la referencia vive en el índice y en la regla.

---

## Memoria unificada — Carpetas · Proyectos · Plataformas

| Qué | Dónde está la lista | Uso |
|-----|---------------------|-----|
| **Carpetas raíz** | Sección "Carpetas en la raíz" más abajo (Núcleo, .NET, Infra, Comercio, Backup, Otros). | No abrir 80 carpetas; consultar este bloque. |
| **Proyectos** | **Núcleo:** RuddieSolution (node + platform + scripts + servers + services). **Servicios .NET:** una carpeta = un proyecto (AIFraudDetection, AdvocateOffice, AppBuilder, … hasta WorkflowEngine). **Otros:** Mamey, MAMEY-FUTURES, ierahkwa-shop, forex-trading-server, etc. Lista completa en "Carpetas en la raíz". | Un proyecto = una carpeta; puertos en docs/TODO-LO-QUE-CORRE-ONLINE.md. |
| **Plataformas (páginas web)** | Todas en **`RuddieSolution/platform/`** (y subcarpetas como `platform/docs/`). Lista dinámica: **`GET /api/platform/all-pages`** (Node 8545). Enlaces curados: `platform/data/platform-links.json` → **`GET /api/platform/links`**. | No listar a mano; usar API o platform-links. Abrir todas: `./scripts/abrir-todas-plataformas-chrome.sh`. |

---

## Principio fundamental

- **TODO PROPIO. NADA DE 3ra COMPAÑÍA.**  
  Ver [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) en la raíz.  
  Cripto Node nativo, APIs propias, self-hosted; sin Google, AWS, Stripe, etc.

---

## Núcleo operativo (lo que corre)

| Qué | Dónde | Puerto / URL |
|-----|-------|----------------|
| **Node principal (API + platform)** | `RuddieSolution/node/server.js` | **8545** — http://localhost:8545 |
| **Banking Bridge** | `RuddieSolution/node/banking-bridge.js` | **3001** |
| **Platform (HTML/estáticos)** | `RuddieSolution/platform/` | Servido por Node en /platform |
| **Arrancar todo** | `./start.sh` (raíz) o `RuddieSolution/scripts/start-full-stack.sh` | Node, Bridge, 8080, 4001–4600, 6000–6400, etc. |
| **Parar todo** | `./stop-all.sh` | |

**Para demo:** abrir desde **http://localhost:8545** (no 8080) para que los botones y la API funcionen. Ver [docs/LISTO-PARA-DEMO.md](LISTO-PARA-DEMO.md).

---

## Carpetas en la raíz del proyecto (todas son del proyecto)

### Entrada de referencia (todo lo del proyecto + otras cosas en la comp)
- **00-PROYECTO-SOBERANOS/** — Carpeta de entrada: LEEME.md (dónde está todo, otras carpetas en la comp, disco externo); OTRAS-COSAS-EN-LA-COMP.md (para anotar rutas de la Mac que no son el repo). El proyecto entero está en la raíz; esta carpeta solo agrupa la referencia.

### Núcleo y plataforma
- **RuddieSolution/** — Núcleo: Node (8545), platform (HTML/JS), scripts, config, data, servicios. **Nuestro** pilar Node.
- **Mamey/** — Software Mamey (blockchain/nodo Rust + .NET). **Nuestro** pilar Mamey; mismo proyecto soberano.
- **MAMEY-FUTURES/** — Mamey Futures.
- **tokens/** — Tokens (IGT-PM, IGT-MFA, etc.).
- **docs/** — Toda la documentación (este índice, TODO-LO-QUE-CORRE-ONLINE, LISTO-PARA-DEMO, etc.).
- **scripts/** — Scripts globales (auditoría, abrir plataformas Chrome, todo-funcionando-production, crear-duplicacion-demo, etc.).

### APIs y servicios .NET (por carpeta)
- **AIFraudDetection/** — Detección de fraude.
- **AdvocateOffice/** — Oficina del abogado (3010).
- **AppBuilder/** — AppBuilder API (5062).
- **AppointmentHub/** — Citas.
- **AssetTracker/** — Seguimiento de activos.
- **AuditTrail/** — Auditoría.
- **BioMetrics/** — Biometría (5120).
- **BiometricAuth/** — Autenticación biométrica.
- **BudgetControl/** — Control presupuestario.
- **CitizenCRM/** — CRM ciudadano (5090).
- **CitizenPortal/** — Portal ciudadano.
- **ContractManager/** — Contratos.
- **DataHub/** — Data Hub.
- **DeFiSoberano/** — DeFi (5140).
- **DigitalVault/** — Bóveda digital (5121).
- **DocumentFlow/** — Flujo documental (5080).
- **ESignature/** — Firma electrónica (5081).
- **FarmFactory/** — FarmFactory (5061).
- **FormBuilder/** — Formularios.
- **GovernanceDAO/** — DAO (5142).
- **HRM/** — Recursos humanos.
- **IDOFactory/** — IDO Factory (5097).
- **InventoryManager/** — Inventarios.
- **MeetingHub/** — Reuniones (7071).
- **MultichainBridge/** — Bridge multichain (5143).
- **NET10/** — NET10 Banking (5071, 5000).
- **NFTCertificates/** — NFT (5141).
- **NotifyHub/** — Notificaciones.
- **OutlookExtractor/** — Extractor Outlook (5082).
- **ProcurementHub/** — Compras.
- **ProjectHub/** — Proyectos (7070).
- **ReportEngine/** — Motor de reportes.
- **RnBCal/** — RnBCal (5055).
- **ServiceDesk/** — Mesa de servicio (5093).
- **SmartSchool/** — Escuela inteligente.
- **SpikeOffice/** — SpikeOffice (5056).
- **TaxAuthority/** — Autoridad fiscal (5091).
- **TradeX/** — Trading (5054).
- **VotingSystem/** — Votación (5092).
- **WorkflowEngine/** — Motor de flujos.

### Infra y despliegue
- **DEPLOY-SERVERS/** — Configuración servidores de despliegue.
- **IERAHKWA-INDEPENDENT/** — Copia independiente (node, platform, tokens, backup-system).
- **IERAHKWA-PLATFORM-DEPLOY/** — Deploy de plataforma (api, config, core).
- **IerahkwaBankPlatform/** — Plataforma bancaria.
- **kubernetes/** — K8s si aplica.
- **systemd/** — Unidades systemd.
- **sovereign-network/** — Red soberana.
- **quantum/** — Quantum.

### Comercio, tienda, otros
- **ierahkwa-shop/** — Tienda (POS, portal, chat).
- **forex-trading-server/** — Servidor Forex (5200).
- **image-upload/** — Subida de imágenes.
- **mobile-app/** / **MobileApp/** — App móvil.
- **platform-dotnet/** — Plataforma .NET.
- **plataformas-finales/** — Plataformas finales.
- **pos-system/** — POS.
- **smart-school-node/** — SmartSchool Node.
- **server/** — Servidor genérico.
- **inventory-system/** — Sistema de inventario.
- **futurehead-trust-negocio/** — Trust negocio.
- **products/** — Productos.
- **workspace/** — Espacio de trabajo.

### Backup y logs
- **auto-backup/** — Backups automáticos.
- **backup/** — Backups.
- **backup-2026-01-18/** — Backup fechado.
- **backups/** — Más backups.
- **logs/** — Logs.

### Otros (carpetas del proyecto)
- **Akwesasne/** — Contenido Akwesasne (Appointments, CLAN ID, First bank, ONE DRIVE).
- **TransactionCodes/** — Códigos de transacción.
- **KIKIALLUS STUFF/** — Kikiallus.
- **KIKILAUS/** — Kikilaus.
- **barotseland/** — Barotseland.
- **guarani/** — Guarani.
- **id and passaportes/** — ID y pasaportes.
- **ierahkwa-platform-soberano/** — Plataforma soberana.
- **indios snfcsm/** — Indios SNFCSM.
- **tests/** — Tests.
- **ai/** — AI.
- **.cursor/** — Configuración Cursor (rules, etc.).

---

## Estructura de RuddieSolution (resumen)

```
RuddieSolution/
├── node/              # Servidor Node (8545): server.js, routes, services, modules, ai-hub
├── platform/          # Frontend: HTML, assets (unified-styles.css, platform-api-client.js), data (platform-links.json, government-departments.json)
├── platform/docs/     # Documentos dentro de platform
├── scripts/          # start-full-stack.sh, start.sh, validar-pre-live.sh, etc.
├── servers/           # start-all-platforms.sh, start-banking-hierarchy.sh, start-stub-services.sh
├── services/          # python, go, rust, security-platform
├── config/            # Configuración
├── data/              # ai-hub, store-locator, telecom
├── IerahkwaBanking.NET10/  # Banking .NET (5000)
├── docs/              # Documentación RuddieSolution
├── backup-system/     # Backups
└── sdk/               # SDK
```

---

## Scripts clave (raíz y scripts/)

| Script | Uso |
|--------|-----|
| `./start.sh` | Arranca todo (Node, Bridge, Platform 8080, plataformas 4001–4600, banca 6000–6400, etc.) |
| `./stop-all.sh` | Para todos los servicios |
| `./start-demo.sh` / `./stop-demo.sh` | Entorno demo (9545, 9080) junto a producción |
| `./scripts/abrir-todas-plataformas-chrome.sh` | Abre en Chrome todas las .html de platform/ (incl. subcarpetas) |
| `./scripts/todo-funcionando-production.sh` | Verificación pre-live + servicios críticos |
| `./scripts/auditoria-prende-todo-y-verifica-live.sh` | Arranca + verifica LIVE 100% production |
| `./scripts/crear-duplicacion-demo.sh` | Copia el repo a otra carpeta para demo |
| **Extreme Pro (disco externo)** | |
| `./scripts/instalar-en-extreme-pro-para-trabajar.sh` | Copia todo al disco (rsync), luego setup; trabajar desde `/Volumes/Extreme Pro/soberanos-natives` |
| `./scripts/setup-en-extreme-pro.sh` | Ejecutar en el disco: .env si falta, npm install (node, platform, servers), logs |
| `./scripts/verificar-extreme-pro.sh` | Comprueba que el proyecto esté completo en el disco; reporte en `docs/verificacion-extreme-pro.txt` |
| `./scripts/backup-a-extreme-pro.sh` | Backup extra a `soberanos-backup` en el mismo disco |
| `./scripts/mudar-a-extreme-pro.sh` | Rsync a host remoto (EXTREME_PRO=usuario@host) y ejecuta setup vía SSH |
| `./scripts/instalar-dos-copias-en-disco.sh` | Deja **dos** copias en el disco: soberanos-natives (principal + setup) y soberanos-natives-copia2 (segunda); más rápido que hacerlo una a una. |

---

## Documentos clave en docs/

- **TODO-LO-QUE-CORRE-ONLINE.md** — Puertos, servicios, qué levanta start.sh.
- **LISTO-PARA-DEMO.md** — Cómo dejar todo listo para demo (8545, endpoints).
- **DEMO-Y-PRODUCTION-MISMO-TIEMPO.md** — Demo y producción a la vez.
- **EVITAR-EXPOSICION-CODIGO-WEB.md** — Sanitización 5xx en producción.
- **INDEX-DOCUMENTACION.md** — Índice de documentación.
- **INDICE-COMPLETO-PROYECTO-SOBERANOS.md** — Este documento (referencia maestra).
- **MUDAR-PROYECTO-A-EXTREME-PRO.md** — Llevar todo al disco Extreme Pro (instalar, trabajar desde disco, verificar); lista completa de qué va al disco remite a este índice.
- **LIBERAR-ESPACIO-DISCO-MAC.md** — Usar disco externo y Mac (una carpeta de trabajo, el otro backup); liberar espacio en Mac (`./scripts/liberar-espacio.sh`) y en el disco sin borrar nada del proyecto.
- **PROYECTO-COMPLETO-MEMORIA-EXTERNA.md** — Proyecto completo en disco: si algo no está ahí se daña. Qué debe ir al disco (todo el índice, .git incluido), qué no se copia (node_modules, logs, etc.) y qué puede quedarse solo en la comp (cachés/temporales).
- **MAPA-CODIGO-PLATAFORMAS-ESTRUCTURA-LOGISTICA.md** — Dónde está el código de las plataformas (platform/*.html, node/platform-routes.js, server.js), estructura (Node + platform/ + data/) y logística (flujo URL → HTML → APIs).
- **RUTA-UNICA-PROYECTO.md** — Todo el código está en una sola ruta (la raíz); no se unificaron ni movieron carpetas; si no ves código, abrir la raíz del proyecto, no solo 00-PROYECTO-SOBERANOS.
- **REPORTE-CODIGOS-NODE-PLATAFORMAS.md** — Reporte Node (server, 416 JS, 102 módulos, 69 routes, 38 services) y plataformas (298 HTML, 159 rutas cortas, APIs /api/platform/*, data y assets).
- **QUE-TENEMOS-EN-NET10-Y-RUST.md** — Qué hay en .NET / .NET 10 (NET10 DeFi, IerahkwaBanking.NET10, APIs por carpeta, Mamey .NET 8) y en Rust (Mamey Node, swift-service, security-platform, plataforma-soberana-rust).
- **VISTA-GLOBAL-DATA-BACKEND-FRONTEND-ADMIN-USER.md** — Vista global: data (node/data, platform/data), backend (Node 8545), frontend (platform/*.html), admin (/admin, /api/admin), user (login, dashboards, plataformas), cómo se conecta todo.
- **REPORTE-CODIGO-NODE-PLATAFORMAS.md** — Reporte de código Node (server.js, platform-routes.js, módulos, servicios, rutas API) y plataformas (HTML, data, assets, endpoints /api/platform/).
- **DATA-QUE-TENEMOS.md** — Inventario único de toda la data del proyecto: node/data (banco, VIP, bonos, estado, soberanía, carpetas por dominio), platform/data (links, departments, landings, servicios); APIs que la exponen; sin duplicar.
- **MEMORIA-UNIFICADA-LIBERAR-COMP.md** — Carpetas, proyectos y plataformas unificados en memoria (índice + regla); cómo trabajar sin tanto peso en la máquina.
- **CONECTAR-DATA-BANCO-Y-BONOS.md** — Cómo conectar la data del banco (bank-registry, bdet-bank, APIs) y de los bonos (VIP, vip-transactions.json, POST con type bonds) al proyecto.
- **DIFERENCIAS-REPORTE-MAMEY-VS-NUESTRO.md** — Comparativa con el Technical Report Mamey v4.0 (2026-02-09): Kernel Chain, receipts, Rust/.NET Blazor, BIIS vs nuestro SIIS/BDET, DIDs, gobernanza.
- **VEREDICTO-MAMEY-VS-NUESTRO-Y-QUE-UNIFICAR.md** — Quién está mejor en qué; qué unificar (documentación, vocabulario, lista de dominios); qué no unificar; plan sugerido.
- **OPCIONES-UNION-MAMEY-RUDDIESOLUTION.md** — Si se unen “como se debe” sin importar costo: A orquestación/APIs, B Node con arquitectura Mamey, C migración total Rust/Blazor, D Mamey núcleo + Node presentación.

---

## APIs de plataforma (Node 8545)

- `/api/platform/links` — Enlaces (platform-links.json).
- `/api/platform/departments` — Departamentos (government-departments.json).
- `/api/platform/all-pages` — Todas las páginas descubiertas (platform/ y subcarpetas).
- `/api/platform/overview` — Resumen (links + departments).
- `/api/platform/services` — Servicios.
- `/api/platform/health` — Salud.

---

## Ministry of Economy — Documents (en la memoria del proyecto)

- **Ministry of Economy** y **Ministry of Economy - Documents** forman parte del proyecto y deben considerarse en la memoria/referencia.
- **Relación con departamentos existentes:** economía se cubre con Ministry of Finance & Treasury (IGT-MFT), Ministry of Commerce & Trade (IGT-MCT), BudgetControl, National Treasury (IGT-NT). Los **documentos** oficiales de economía y gobierno se gestionan en:
  - **Plataformas:** `RuddieSolution/platform/documents.html`, `documents-platform.html`, `government-portal.html`; DocumentFlow (5080), ESignature (5081).
  - **Datos:** `RuddieSolution/platform/data/government-departments.json` (ministerios), `platform-landing-info.json`, `TEXTO-OFICIAL-PLATAFORMAS.json` (documentación oficial).
  - **Docs:** carpeta `docs/` en la raíz (documentación técnica y estratégica); `RuddieSolution/platform/docs/` para docs servidos desde la platform.
- Cualquier referencia a "Ministry of Economy" o "Ministry of Economy - Documents" en el código, rutas o documentación debe mantenerse alineada con este índice.

---

## Recordatorio

- **No borrar nada** hasta confirmar que todo está organizado y transferido.
- Todas las carpetas listadas arriba son **parte del proyecto** Sovereign Government of Ierahkwa Ne Kanienke.
- Para seguir trabajando en otro equipo: clonar/copiar toda la carpeta del proyecto y usar este índice + docs/ como referencia.
- **Para llevar todo al disco Extreme Pro:** ver [MUDAR-PROYECTO-A-EXTREME-PRO.md](MUDAR-PROYECTO-A-EXTREME-PRO.md); la lista de qué va al disco es este índice (no se duplica allí).

*Última actualización: febrero 2026.*
