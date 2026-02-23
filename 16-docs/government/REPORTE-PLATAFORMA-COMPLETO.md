# REPORTE COMPLETO — PLATAFORMA IERAHKWA SOVEREIGN

**Fecha:** 2 de febrero de 2026  
**Ámbito:** Plataforma unificada, AI, Security Fortress, Quantum, Finance, servidores y APIs.

---

## 1. RESUMEN EJECUTIVO

La plataforma IERAHKWA integra en un solo ecosistema:

- **Plataforma Unificada** — Una sola página con AI, Security Fortress, Quantum y Finance (todo en uno).
- **Equipo soberano** — AI Platform + Security Fortress + Quantum trabajando juntos vía Command Center.
- **Nodo principal** — Node en puerto **8545** (Mamey Node) que sirve frontend, APIs y health.

**URL base local:** `http://localhost:8545`

---

## 2. PLATAFORMA UNIFICADA (TODO EN UNO)

| Elemento | Descripción |
|-----------|-------------|
| **URL** | `http://localhost:8545/platform/plataforma-unificada.html` |
| **Archivo** | `RuddieSolution/platform/plataforma-unificada.html` |
| **Contenido** | Una sola página con pestañas e iframes. Sin enlaces externos: todo embebido. |

### Vistas

- **Inicio:** Grid 2×2 con iframes de AI Platform, Security Fortress, Quantum Platform, Finance Platform.
- **Pestañas:** AI | Security Fortress | Quantum | Finance (cada una a pantalla completa).
- **Barra inferior:** Estado en vivo (AI, Security, Quantum) desde `/api/command-center`.

### Acceso desde el Dashboard

- Botón **"Plataforma Unificada"** en la franja "Soporte de toda la plataforma".
- Badge **"UNIFICADA"** en el header del dashboard (`index.html`).

---

## 3. EQUIPO SOBERANO (AI + SECURITY + QUANTUM)

### Command Center (backend)

| Endpoint | Método | Descripción |
|----------|--------|-------------|
| `/api/command-center` | GET | Estado unificado: `ai`, `security`, `quantum`, `recentAlerts`. |
| `/api/command-center/alert` | POST | Publicar alerta (body: `source`, `level`, `message`). |

**Respuesta GET `/api/command-center`:**

- `team`: `"AI + Security + Quantum"`
- `ai`: `status`, `aiHub`, `workers`
- `security`: `ghost`, `healthScore`, `activeNodes`
- `quantum`: `status`, `available`
- `recentAlerts`: últimas 10 alertas compartidas entre AI y Security.

### Integración entre plataformas

| Plataforma | Vista equipo | Dónde se muestra el equipo |
|------------|--------------|----------------------------|
| **AI Platform** | Abre AI + Security en 2 pestañas | Card "AI + Security Fortress", alertas Security, sidebar "Equipo Soberano". |
| **Security Fortress** | Abre AI + Security en 2 pestañas | Panel "AI Guardian", alertas AI, badge → AI Platform. |
| **Quantum Platform** | Abre AI + Security + Quantum en 3 pestañas | Strip "Equipo: AI | Security". |
| **Finance Platform** | Abre AI + Security + Quantum en 3 pestañas | Botón "Vista equipo" en header. |

### Alertas compartidas

- **POST** `/api/command-center/alert` con `source`: `"ai"` o `"security"`.
- AI Platform muestra alertas con `source === 'security'`.
- Security Fortress muestra alertas con `source === 'ai'`.

---

## 4. PLATAFORMAS PRINCIPALES

| Plataforma | URL | Archivo | Descripción |
|------------|-----|---------|-------------|
| **Dashboard** | `/platform/` o `/platform/index.html` | `RuddieSolution/platform/index.html` | Entrada principal, health, favoritos, búsqueda. |
| **Plataforma Unificada** | `/platform/plataforma-unificada.html` | `plataforma-unificada.html` | Todo en uno: AI, Security, Quantum, Finance. |
| **AI Platform** | `/platform/ai-platform.html` | `ai-platform.html` | AI Studio, chat, code, Command Center, enlace a Security. |
| **Security Fortress** | `/platform/security-fortress.html` | `security-fortress.html` | Ghost Mode, Phantom, AI Guardian, servidores, Command Center. |
| **Quantum Platform** | `/platform/quantum-platform.html` | `quantum-platform.html` | Post-quantum, health en vivo, servidores, latencia, Quantum API status. |
| **Finance Platform** | `/platform/finance-platform.html` | `finance-platform.html` | TradeX, Forex, Banking, DeFi, tokens, enlaces a AI/Quantum/Security. |

---

## 5. SERVICIOS CORE (PUERTO 8545)

El servidor Node (`RuddieSolution/node/server.js`) en **8545** expone, entre otros:

| Ruta | Descripción |
|------|-------------|
| `/health` | Health del nodo (uptime, memoria). |
| `/api/health/all` | Health de todos los servicios (health-monitor). |
| `/api/health/core` | Sistemas core. |
| `/api/health/stats` | Estadísticas de health. |
| `/api/ai-hub/*` | AI Hub (Atabey, family, workers, market, trading-signals). |
| `/api/ai-hub/health` | Health del AI Hub. |
| `/api/ai-hub/atabey/workers` | Lista de workers Atabey. |
| `/api/v1/ghost/status` | Estado Ghost Mode (red, nodos). |
| `/api/v1/ghost/health` | Health de la red Ghost. |
| `/api/v1/quantum/status` | Estado público de Quantum (sin JWT). |
| `/api/v1/quantum/*` | Cripto post-cuántica (JWT). |
| `/api/command-center` | Estado unificado AI + Security + Quantum. |
| `/api/command-center/alert` | Publicar alerta del equipo. |
| `/api/docs` | Swagger/OpenAPI. |

**Cantidad aproximada de rutas en server.js:** ~144 (GET/POST/router).

---

## 6. MAPA DE PUERTOS (SERVIDORES)

### Core (arranque con `start.sh`)

| Puerto | Servicio | Script / Notas |
|--------|----------|-----------------|
| **8545** | Node Mamey (plataforma principal) | `node/server.js` |
| **3001** | Banking Bridge API | `node/banking-bridge.js` |
| **5000** | Banking .NET API | IerahkwaBanking.NET10 |
| **8080** | Platform Frontend | `platform/server.js` (si existe) |

### Platform servers (independientes)

| Puerto | Plataforma |
|--------|------------|
| 4001 | BDET Bank |
| 4002 | TradeX Exchange |
| 4003 | SIIS Settlement |
| 4004 | Clearing House |
| 4100–4400 | Bancos centrales (Águila, Quetzal, Cóndor, Caribe) |
| 4500 | AI Hub / ATABEY |
| 4600 | Government Portal |

### Trading, Office, Government, etc.

- **5054** TradeX · **5071** NET10 DeFi · **5061** FarmFactory · **5097** IDOFactory · **5200** Forex  
- **5055** RnBCal · **5056** SpikeOffice · **5060** AppBuilder · **7070** ProjectHub · **7071** MeetingHub  
- **5090** CitizenCRM · **5091** TaxAuthority · **5092** VotingSystem · **5093** ServiceDesk  
- **5080** DocumentFlow · **5081** ESignature · **5120** BioMetrics · **5121** DigitalVault  
- **5140** DeFi · **5141** NFT Certificates · **5142** Governance DAO · **5143** Multichain Bridge  
- **5300** AI Hub · **5301** AI Banker BDET · **5302** AI Trader  
- **8590** Rust SWIFT · **8591** Go Queue · **8592** Python ML  

Documentación detallada: `PUERTOS-SERVICIOS.md`.

---

## 7. INICIO DEL SISTEMA

```bash
# Desde la raíz del proyecto
./start.sh
```

- Arranca Node en **8545** (y opcionalmente Banking Bridge en **3001**).
- Si hay PM2, usa `ecosystem.config.js`; si no, procesos en background.
- Opcional: servicios en Rust/Go/Python (8590, 8591, 8592), .NET, etc.

---

## 8. ARCHIVOS CLAVE IMPLEMENTADOS / MODIFICADOS

| Archivo | Cambio |
|---------|--------|
| `RuddieSolution/platform/plataforma-unificada.html` | **Creado.** Página todo-en-uno con pestañas e iframes. |
| `RuddieSolution/platform/index.html` | Botón "Plataforma Unificada" y badge "UNIFICADA". |
| `RuddieSolution/platform/quantum-platform.html` | Health en vivo, servidores, Quantum API status, resumen, alertas, latencia, Vista equipo, strip Command Center. |
| `RuddieSolution/platform/ai-platform.html` | Base href, Security Fortress en header/sidebar, card Command Center, Vista equipo, alertas Security. |
| `RuddieSolution/platform/security-fortress.html` | AI Guardian (enlace a AI), Command Center en vivo, alertas AI, Vista equipo, AI Platform en dashboard. |
| `RuddieSolution/platform/finance-platform.html` | Base href, Vista equipo (AI + Security + Quantum). |
| `RuddieSolution/node/server.js` | `/api/command-center`, `/api/command-center/alert`, inclusión de `quantum` en command-center. |
| `RuddieSolution/node/modules/quantum-encryption.js` | `getStatus()` para `/api/v1/quantum/status` público. |
| `RuddieSolution/node/server.js` | Ruta pública `GET /api/v1/quantum/status`. |

---

## 9. URLs PRINCIPALES (LOCAL)

| Uso | URL |
|-----|-----|
| Dashboard | http://localhost:8545/platform/ |
| **Plataforma Unificada (todo en uno)** | http://localhost:8545/platform/plataforma-unificada.html |
| AI Platform | http://localhost:8545/platform/ai-platform.html |
| Security Fortress | http://localhost:8545/platform/security-fortress.html |
| Quantum Platform | http://localhost:8545/platform/quantum-platform.html |
| Finance Platform | http://localhost:8545/platform/finance-platform.html |
| Command Center (API) | http://localhost:8545/api/command-center |
| Health (nodo) | http://localhost:8545/health |
| Health (todos los servicios) | http://localhost:8545/api/health/all |
| API docs (Swagger) | http://localhost:8545/api/docs |

---

## 10. REGLAS Y CONVENCIONES

- **Base href:** Las páginas bajo `/platform/` usan `<base href="/platform/">` para que los enlaces funcionen al servirse desde 8545.
- **Origen API:** El frontend usa `window.location.origin` o `http://localhost:8545` para llamar a la API.
- **Puertos:** No reutilizar un mismo puerto para dos servicios (ver `PUERTOS-SERVICIOS.md`).

---

*Reporte generado para la Plataforma IERAHKWA Sovereign — Gobierno de Ierahkwa Ne Kanienke.*
