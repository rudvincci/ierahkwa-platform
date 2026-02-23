# Arquitectura BHBK — Banco Central Indígena

**Objetivo 2026:** Que el ecosistema sea una **potencia soberana**. **Las Américas y el Caribe son nuestro territorio.** La arquitectura de BHBK se divide en **Departamentos Estratégicos** que operan de forma **independiente pero integrada** en ese territorio. El banco no es solo un "proveedor de servicios", sino el **Dueño de la Infraestructura** que protege la producción indígena.

---

## Punto único de administración

**Todo se administra desde Ierahkwa Futurehead BDET Bank (back).**  
Registro de bancos, settlement, 4 bancos centrales, tesorería, compliance, RWA y servicios al ciudadano se gestionan desde la interfaz y APIs del BDET Bank (`platform/bdet-bank.html` + Node + Banking Bridge). Un solo back para operar el ecosistema.

---

## 0. Jerarquía de nodos (International Settlement + 4 Bancos Centrales)

- **Un nodo** = **International Settlement (SIIS)** — liquidación internacional (puerto 8500, `NODE_ROLE=settlement`).
- **Debajo**, **4 nodos** = **bancos centrales independientes**: Águila (8545), Quetzal (8546), Cóndor (8547), Caribe (8548). Si cae uno, los otros tres siguen.

**En cada nodo:** los niveles **Regional, Nacional y Comercial** están **separados** (capas distintas: regionales, nacionales, comerciales), cada uno con su registro y lógica.

**Entre nodos:** **todos los nodos están conectados entre sí**. El nodo Settlement se conecta a los 4 bancos centrales; los 4 bancos centrales se conectan al Settlement y entre ellos (mesh). Clearing, SIIS y registro se coordinan por esa red.

Detalle: `docs/CUATRO-NODOS-REGIONES.md` y `node/ecosystem.4regions.config.js`.

---

## 1. Departamentos Internos del Banco (Estructura de Poder)

Estos departamentos son el **corazón del banco** y dictan las reglas para todos los sectores.

| Departamento | Función |
|--------------|---------|
| **Tesorería y Reservas** | Custodia los **Futurehead Trust Coins (FHTC)** y el oro/activos que respaldan la moneda. Gestiona la liquidez para que nunca falte capital para la siembra. |
| **Gestión de Riesgos y Cumplimiento** | Valida que cada planta medicinal cumpla con las leyes soberanas. Seguridad para industrias "sensibles" (ej. marihuana/hemp), evitando bloqueos externos. |
| **Activos Reales (RWA)** | Digitaliza la tierra indígena en NFTs. Si un agricultor usa su tierra como colateral, este departamento la administra. |
| **Tecnología y Nodo 8545** | Mantiene el servidor y la seguridad blockchain del banco. Es el cerebro que conecta el dinero con el delivery. |

---

## 2. Servicios Especializados (Servicios al Ciudadano)

No son departamentos, sino **servicios directos** que el banco ofrece a otros bots y a los ciudadanos.

| Servicio | Descripción |
|----------|-------------|
| **Banca de Futuros** | Gobiernos extranjeros compran cosechas adelantadas; inyecta dinero fresco al banco. |
| **Crédito Agrícola Dinámico** | Préstamos "planta por planta" con tasas preferenciales (1,5%) que se ajustan según la productividad. |
| **Fideicomisos Indígenas** | Protección del patrimonio de las comunidades y gestión de regalías de franquicias. |
| **Gateway de Pagos Multimoneda** | Cambio de tokens de productos (ej. IGT-HEMP) por dólares o euros al instante para inversores. |

---

## 3. Matriz de Operación (Interacción de Bots)

Cada **bot externo** se comunica con un **departamento específico** del banco.

| Bot Externo | Departamento del Banco | Servicio que recibe |
|-------------|------------------------|----------------------|
| **Bot Agricultura** | Dept. de Activos Reales (RWA) | Validación de colateral de tierra. |
| **Bot Logística** | Dept. de Tesorería | Liberación de pagos tras entrega confirmada. |
| **Bot Ciudadano** | Dept. de Tecnología (Nodo 8545) | Login unificado (SSO) y pago de recompensas. |
| **Bot Casino** | Dept. de Gestión de Riesgos | Auditoría de premios y custodia de fondos. |

---

## Resumen

- Los servicios de BHBK se organizan en **Departamentos Estratégicos** que operan de forma **integrada** dentro de la estructura del banco.
- Esto permite una **arquitectura cohesiva** y el **control sobre la infraestructura**.
- **IERAHKWA / BDET / Futurehead** implementan esta estructura en código: nodo 8545, APIs de tesorería, RWA, compliance y gateway de pagos.

**Referencias en código:** `RuddieSolution/node/server.js` (puerto 8545), `RuddieSolution/node/banking-bridge.js`, `RuddieSolution/platform/bdet-bank.html`, `docs/PLAN-IMPLEMENTACION-FUTUREHEAD-2026.md`.

**Negocios separados, dashboard único, backend compartido:** Cada negocio tiene su propia página; solo en el main dashboard se mezcla todo; todos comparten el mismo backend y data. Ver `docs/PRINCIPIO-NEGOCIOS-SEPARADOS-DASHBOARD-UNICO.md`.

---

## Comprobar que todo esté bien en el banco

- **Estado global:** `GET /api/v1/bdet/bank-status` — devuelve `ok: true` si el registro de bancos existe, tiene entradas, y el Banking Bridge (3001) responde. Usado por `status.sh` y por la verificación 100% production.
- **Registro de bancos:** `GET /api/v1/bdet/bank-registry` — lista de bancos (central, regional, nacional, comercial). Archivo: `node/data/bank-registry.json`.
- **Health:** Node `GET /health`, Banking Bridge `GET http://localhost:3001/api/health`. Si todo está bien: registro cargado, bridge online, nodo online.

## Reglas, licencias y enlaces

- **BDET Bank (interfaz):** `platform/bdet-bank.html` — punto único de administración (hub 8545).
- **Treasury:** Parte del banco; dashboard en `node/public/treasury-dashboard.html` (ruta `/treasury-dashboard.html`).
- **Reglas y licencias para todos:** Todas las entidades (incluidos los departamentos del banco) operan bajo licencia y reglas soberanas. Ver `docs/REGLAS-Y-LICENCIAS-PARA-TODOS.md` y API `GET /api/v1/sovereignty/reglas-y-licencias`.
- **Certificado de licencia (plantilla):** `docs/CERTIFICADO-LICENCIA.md`.
- **Autoridad de licencias:** `node/public/license-authority.html` y API `/api/v1/licenses` (status, types, check, issue, verify).
- **Monitoreo por plataforma (sin mezclar):** Cada plataforma (Casino, BDET, Treasury, Financial Center) tiene su vista de monitoreo en `admin-monitoring.html?platform=...`. API: `GET /api/v1/admin/monitoring?platform=casino|bdet|treasury|financial-center`.
- **Renta por plataforma:** Un solo dashboard unificado en `/platform` (#commercialServicesSection); cada plataforma muestra solo sus servicios de renta. API: `GET /api/v1/platform/rent?platform=casino|bdet|treasury|financial-center`.
