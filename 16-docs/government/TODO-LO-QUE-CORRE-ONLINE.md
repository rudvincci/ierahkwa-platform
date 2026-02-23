# Todo lo que corre en línea — hasta subir a iCloud

**Sovereign Government of Ierahkwa Ne Kanienke**  
Lista única de servicios, puertos y procesos que se levantan cuando la plataforma está “online”. Úsala como checklist antes de poner el sistema en iCloud o en un servidor.

---

## Cómo se arranca todo

| Comando | Qué levanta |
|--------|-------------|
| `./start.sh` | **Todo**: Node (8545), Bridge (3001), Platform (8080), servidores de plataforma (4001–4600), jerarquía bancaria (6000–6400), .NET si hay `dotnet`, Forex (5200), servicios multilenguaje (8590–8592) |
| `./GO-LIVE-PRODUCTION.sh` | Verificación pre-live + PM2 (Node, Bridge, Editor API) + opcionalmente lo mismo que start.sh según el script |
| `./stop-all.sh` | Detiene **todos** los puertos listados abajo |

---

## Tabla de puertos y servicios (todo lo que puede estar “online”)

### Core (siempre con start.sh)

| Puerto | Servicio | Proceso / Origen | URL típica |
|--------|----------|------------------|------------|
| **8545** | Node API principal | `server.js` (PM2: ierahkwa-node-server) | http://localhost:8545 — /health, /platform, toda la API |
| **3001** | Banking Bridge | `banking-bridge.js` (PM2: ierahkwa-banking-bridge) | http://localhost:3001 — /api/cards, /api/mobile, etc. |
| **3002** / **3005** | Editor API | `platform/api/editor-api.js` (PM2: ierahkwa-editor-api) | Para /editor en 8545 |
| **8080** | Platform frontend | `RuddieSolution/platform/server.js` | http://localhost:8080 — Leader Control, admin, estáticos |

### Servidores de plataforma (start-all-platforms.sh → logs/platform-servers.log)

| Puerto | Servicio |
|--------|----------|
| **4001** | BDET Bank |
| **4002** | TradeX Exchange |
| **4003** | SIIS Settlement |
| **4004** | Clearing House |
| **4100** | Banco Águila (Norte) |
| **4200** | Banco Quetzal (Centro) |
| **4300** | Banco Cóndor (Sur) |
| **4400** | Banco Caribe (Taínos) |
| **4500** | AI Hub / ATABEY |
| **4600** | Government Portal |

### Jerarquía bancaria (start-banking-hierarchy.sh → logs/banking-hierarchy.log)

| Puerto | Servicio |
|--------|----------|
| **6000** | SIIS |
| **6001** | Clearing |
| **6002** | Global |
| **6003** | Receiver |
| **6010** | BDET Master |
| **6100** | Águila Norte |
| **6200** | Quetzal Centro |
| **6300** | Cóndor Sur |
| **6400** | Caribe |

### .NET (si existe `dotnet` y las carpetas del repo)

| Puerto | Servicio |
|--------|----------|
| **3000** | IERAHKWA Platform API (Cryptohost, BDET, Bridge, Trading, Tokens) |
| **3010** | Advocate Office |
| **5000** | Banking .NET (IerahkwaBanking.NET10) |
| **5054** | TradeX .NET |
| **5055** | RnBCal |
| **5056** | SpikeOffice |
| **5061** | FarmFactory |
| **5062** | AppBuilder |
| **5071** | NET10 Banking |
| **5090** | CitizenCRM |
| **5091** | TaxAuthority |
| **5092** | VotingSystem |
| **5093** | ServiceDesk |
| **5097** | IDO Factory |
| **5080** | DocumentFlow |
| **5081** | ESignature |
| **5082** | OutlookExtractor |
| **5120** | BioMetrics |
| **5121** | DigitalVault |
| **5140** | DeFi Soberano |
| **5141** | NFT Certificates |
| **5142** | Governance DAO |
| **5143** | Multichain Bridge |
| **5144** | AI Fraud Detection |
| **7070** | ProjectHub |
| **7071** | MeetingHub |

### Otros

| Puerto | Servicio |
|--------|----------|
| **5200** | Forex Trading Server |
| **8590, 8591, 8592** | Servicios multilenguaje (Rust/Go/Python) o stubs |

---

## Resumen por “qué corre” cuando haces ./start.sh

- **Node:** 8545, 3001, 3002 (o 3005), 8080  
- **Platform servers:** 4001, 4002, 4003, 4004, 4100, 4200, 4300, 4400, 4500, 4600  
- **Banking hierarchy:** 6000, 6001, 6002, 6003, 6010, 6100, 6200, 6300, 6400  
- **.NET:** los que existan en tu máquina (3000, 3010, 5000, 5054–5097, 5071, 5080–5082, 5120–5121, 5140–5144, 7070–7071)  
- **Forex:** 5200  
- **Multilang / stubs:** 8590, 8591, 8592  

`./stop-all.sh` mata procesos en **todos** esos puertos (incluidos los listados en el script).

---

## URLs principales (todo en un vistazo)

- **Plataforma unificada:** http://localhost:8545/platform/index.html  
- **Admin:** http://localhost:8545/platform/admin.html  
- **Leader Control:** http://localhost:8545/platform/leader-control.html (también :8080)  
- **Atabey (AI):** http://localhost:8545/platform/atabey-platform.html  
- **Security Fortress:** http://localhost:8545/platform/security-fortress.html  
- **Banco / BDET:** http://localhost:8545/platform/bdet-bank.html  
- **Health:** http://localhost:8545/health  

---

## Qué tener en cuenta para iCloud (o otro host)

1. **Código y configuración:** Todo el repo (o la parte que uses). No subas `.env` a Git; en iCloud guárdalo aparte o en copia segura.
2. **Datos vivos:** `RuddieSolution/node/data/` (usuarios, 2FA, AI hub, etc.). Incluir en backup antes de migrar.
3. **Logs:** `logs/` en la raíz y `RuddieSolution/node/logs/`. Opcional subirlos; recomendable no depender de iCloud para logs de producción.
4. **Puertos:** En un VPS/servidor los mismos puertos; en iCloud (solo almacenamiento) no “corre” nada: solo guardas código y datos para luego levantar en un servidor.

---

## Todo funcionando production (un solo flujo)

Para dejar **todo funcionando en production** en tu máquina:

1. **Verificación previa (env, JWT, CORS):**  
   `./scripts/pre-live-check.sh`  
   Si hay blockers (rojos), corrige `.env` según la guía que indica el script.

2. **Arrancar todo:**  
   `./start.sh`  
   Levanta Node, Bridge, Platform, servidores de plataforma, jerarquía bancaria y lo que tengas (.NET, Forex, etc.).

3. **Comprobar que production está OK:**  
   `./scripts/todo-funcionando-production.sh`  
   - Vuelve a pasar el pre-live-check.  
   - Comprueba que los servicios críticos (8545, 3001, 8080) respondan.  
   - Opcional: verificación detallada de servicios.  
   - Si todo va bien: imprime **TODO FUNCIONANDO PRODUCTION** y las URLs principales.  
   - Si algo falla: indica qué corregir o que ejecutes `./start.sh` y vuelvas a correr el script.

Resumen: **pre-live-check → start.sh → todo-funcionando-production.sh** = todo funcionando production.

---

## Auditoría y Demo + Production a la vez

- **Auditoría (prender todo y verificar LIVE 100%):**  
  `./scripts/auditoria-prende-todo-y-verifica-live.sh`  
  (Con `AUDITORIA_SKIP_START=1` solo verifica, sin ejecutar start.sh.)
- **Demo y Production al mismo tiempo:**  
  `./start.sh` → Production. Luego `./start-demo.sh` → Demo (9545, 3003, 9080). Ver [DEMO-Y-PRODUCTION-MISMO-TIEMPO.md](DEMO-Y-PRODUCTION-MISMO-TIEMPO.md).
- **Duplicación para guardar (segunda carpeta para demo):**  
  `./scripts/crear-duplicacion-demo.sh [ruta]` → crea copia en `../soberanos-natives-demo` por defecto.

---

## Comandos útiles

```bash
./start.sh              # Arrancar todo (producción)
./start-demo.sh         # Arrancar solo DEMO (9545, 3003, 9080) junto a Production
./stop-demo.sh          # Parar solo DEMO
./stop-all.sh           # Parar todo
./status.sh             # Estado de servicios (si existe)
./scripts/auditoria-prende-todo-y-verifica-live.sh   # Prender todo y verificar LIVE 100%
./scripts/pre-live-check.sh   # Verificación antes de ir a live
./scripts/todo-funcionando-production.sh   # Verificar que todo está production OK
lsof -i :8545           # Ver qué proceso usa el puerto 8545
pm2 list                # Si usas PM2: ver procesos
```

Con esta lista puedes revisar **todo lo que corre online** hasta que lo pongas en iCloud (o en el servidor final).
