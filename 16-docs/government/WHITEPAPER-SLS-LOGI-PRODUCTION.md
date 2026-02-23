# WHITEPAPER: SOVEREIGN LOGISTICS SYSTEM (SLS) & TOKEN LOGi  
**Production 100% · Continental Delivery Engine (CDE)**

Sovereign Government of Ierahkwa Ne Kanienke · Logística Soberana · Nodo 8545

**Las Américas y el Caribe son nuestro territorio.** La logística soberana (SLS/CDE) opera en y para este territorio.

---

## 1. VISIÓN GENERAL

El **Sovereign Logistics System (SLS)** y el **Continental Delivery Engine (CDE)** son la infraestructura de logística propia del ecosistema Ierahkwa Futurehead: **envíos locales (ciudad) e internacionales en las Américas y el Caribe — nuestro territorio** — sin dependencias de terceros. Todo corre en el **Nodo 8545** (Ierahkwa Sovereign Blockchain — ISB), con **soberanía de datos**, **costos internos** y **token nativo LOGi** sobre el mismo blockchain que el resto de tokens (IGT, plataforma).

- **Independencia de carga:** el sistema asigna protocolos de manejo (temperatura, seguridad, prioridad) según el tipo de producto.
- **Interoperabilidad:** Agro, Manufactura, Healthcare y otros departamentos usan la misma base logística.
- **Cero APIs externas:** sin Google Maps, AWS, ni servicios de terceros; rutas y nodos propios.

---

## 2. ARQUITECTURA

### 2.1 Componentes

| Componente | Descripción |
|------------|-------------|
| **SLS** | Sovereign Logistics System — manifiestos, trazabilidad, protocolos por tipo de carga. |
| **CDE** | Continental Delivery Engine — órdenes de envío (local vs cross-border), aduana soberana. |
| **LOGi** | Token en ISB para tarifas, recompensas y pagos dentro del ecosistema logístico. |
| **Nodo 8545** | Mamey Node (blockchain + APIs). Módulo `Logistics-Independent` montado en `/api/v1/logistics`. |

### 2.2 Zonas y nodos (red propia)

- **NORTH:** Ierahkwa-Node, Mexico-Hub, Quebec-Station  
- **CENTRAL:** Panama-Pass, Guatemala-Link  
- **CARIBBEAN:** Hispaniola-Port, Cuba-Terminal, Boriken-Dock  
- **SOUTH:** Amazon-Base, Andes-Post, Pampa-Center  

Las rutas se determinan por **origen/destino** (prefijo de región); no se consultan mapas externos.

### 2.3 Modos de transporte y puerta a puerta

Los deliveries cubren **tres modos de transporte** y el estándar es **puerta a puerta** (door-to-door).

| Modo | Código | Uso |
|------|--------|-----|
| **Vuelos (aéreo)** | AIR | Rutas largas, urgente, islas, cruce de regiones. |
| **Botes (marítimo / fluvial)** | SEA | Caribe, costas, ríos (Amazonas, etc.), carga a granel. |
| **Terrestre** | LAND | Camión, furgoneta, moto; última milla y rutas continentales. |

- **Puerta a puerta:** Todos los envíos son **door-to-door** por defecto: recogida en origen y entrega en destino (puerta del destinatario). Logit en cada punto hace la última milla hasta la puerta.
- En manifiesto y orden se puede enviar `transportMode` (AIR | SEA | LAND) y `doorToDoor` (por defecto `true`). API: `GET /api/v1/logistics/transport-modes`.

### 2.4 Flujo de un envío

1. **Creación:** manifiesto SLS u orden CDE (origen, destino, carga, opcional tipo de producto).  
2. **Protocolo:** asignación automática por nombre de carga o `productType` (DRY_GOODS, REFRIGERATED, FROZEN, MEDICINAL, HIGH_VALUE, HEMP_AGRO, HAZARDOUS).  
3. **Tarifa:** calculada en LOGi (base local/internacional × multiplicador del protocolo); cobro on-chain si el citizen tiene balance.  
4. **Tránsito:** registro de llegada a cada nodo (`move`); historial en `nodeHistory`.  
5. **Aduana:** para envíos Cross-Border / CONTINENTAL_BRIDGE, paso por aduana soberana (`customs`).  
6. **Entrega:** marcado `deliver`; señal al Banco BHBK para liberación de fondos; recompensa en LOGi al owner (desde treasury).  

---

## 3. PROTOCOLOS POR TIPO DE PRODUCTO

| Protocolo | Temperatura | Seguridad | Prioridad | Multiplicador tarifa |
|-----------|-------------|-----------|-----------|----------------------|
| DRY_GOODS | AMBIENT | STANDARD | NORMAL | 1.0 |
| REFRIGERATED | REFRIGERATED_2_8C | STANDARD | NORMAL | 1.5 |
| FROZEN | FROZEN_18C | HIGH | EXPRESS | 2.0 |
| MEDICINAL | CONTROLLED | HIGH | EXPRESS | 1.8 |
| HIGH_VALUE | AMBIENT | HIGH | EXPRESS | 1.6 |
| HEMP_AGRO | AMBIENT | STANDARD | NORMAL | 1.2 |
| HAZARDOUS | SPECIAL | HIGH | CRITICAL | 2.5 |

La detección puede ser **automática** (palabras clave en el nombre de la carga) o **explícita** (`productType` en la API).

---

## 4. TOKEN LOGi (ISB)

- **Nombre:** Logi · **Símbolo:** LOGi  
- **Blockchain:** Ierahkwa Sovereign Blockchain (ISB), mismo protocolo que IGT y tokens de plataforma.  
- **Decimales:** 9 (igual que el resto de tokens del nodo).  
- **Registro:** token en `state.tokens['LOGi']`; balances de holders en `state.accounts[address].storage`; treasury en `state.accounts[address].balance`.  

### 4.1 Uso

- **Tarifas de envío:** al crear manifiesto/orden, se deduce la tarifa (en LOGi) del `owner` y se acredita a `SLS_TREASURY`. Si no hay balance suficiente, el envío se crea con `feePending`.  
- **Recompensa por entrega:** al marcar un envío como entregado, se transfiere 1 LOGi desde `SLS_TREASURY` al `owner`.  
- **Créditos:** un operador puede acreditar LOGi desde el treasury a un citizen vía API (`POST /api/v1/logistics/token/credit`).  

### 4.2 Tarifas base (en LOGi)

- **Local:** 10 LOGi × multiplicador del protocolo.  
- **Internacional:** 50 LOGi × multiplicador del protocolo.  

### 4.3 APIs de token (en el nodo)

- `GET /api/v1/tokens/LOGi/balance/:holderId` — balance de un citizen o `SLS_TREASURY`.  
- `POST /api/v1/tokens/LOGi/transfer` — body `{ from, to, amount }` (amount en unidades mínimas).  

La logística usa el mismo chain; no hay ledger JSON separado.

---

## 5. SEGURIDAD Y PRODUCCIÓN (100% PRODUCTION)

- **Validación de entrada:** longitudes máximas (cargo 256, origin/destination 128, citizenId 64, dept 64); sanitización de cadenas; rechazo de caracteres de control.  
- **Tracking ID:** solo caracteres alfanuméricos, guión y guión bajo; longitud controlada (validación en GET/PUT/POST por `trackingId`).  
- **Límites de listado:** máximo 500 envíos en `GET /deliveries` y `GET /movement/network` para evitar sobrecarga.  
- **Errores:** en creación de manifiesto/orden, respuestas 500 con mensaje genérico; detalle solo en logs del servidor.  
- **Rate limiting:** el nodo aplica rate limiting a `/api`; el módulo SLS no expone datos sensibles en respuestas de error.  
- **Persistencia:** manifiestos en `node/data/logistics-manifests.json`; balances LOGi en estado del blockchain (ISB).  
- **BHBK:** cada envío lleva sello (seal) e insurance BHBK-BACKED/BHBK-SECURED; la señal de liberación de fondos al Banco es interna (log/hook).  

---

## 6. APIs REST (SLS)

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/v1/logistics/health` | Health check del módulo. |
| GET | `/api/v1/logistics/status` | Resumen: regiones, nodos, total manifiestos, activos, entregados. |
| GET | `/api/v1/logistics/regions` | Zonas de cobertura. |
| GET | `/api/v1/logistics/nodes` | Nodos de red. |
| GET | `/api/v1/logistics/transport-modes` | Modos de transporte (AIR, SEA, LAND) y puerta a puerta. |
| GET | `/api/v1/logistics/deliveries?status=...` | Lista de envíos (límite 500). |
| GET | `/api/v1/logistics/protocols` | Protocolos por tipo de producto y palabras clave. |
| GET | `/api/v1/logistics/movement/network?status=...` | Movimientos para visualización (límite 500). |
| GET | `/api/v1/logistics/token` | Info token LOGi y treasury. |
| GET | `/api/v1/logistics/token/balance/:citizenId` | Balance LOGi de un citizen. |
| POST | `/api/v1/logistics/token/credit` | Acreditar LOGi desde treasury (body: citizenId, amount). |
| POST | `/api/v1/logistics/manifest` | Crear manifiesto SLS (body: citizenId, dept, cargo, origin, destination, productType). |
| POST | `/api/v1/logistics/order` | Crear orden CDE (body: citizenId, item, origin, destination, isInternational, productType). |
| GET | `/api/v1/logistics/:trackingId` | Detalle de un envío. |
| PUT | `/api/v1/logistics/:trackingId/move` | Registrar llegada a nodo (body: currentNode). |
| POST | `/api/v1/logistics/:trackingId/customs` | Pasar aduana. |
| POST | `/api/v1/logistics/:trackingId/deliver` | Marcar entregado. |

---

## 7. INTEGRACIÓN CON BHBK

Según la arquitectura BHBK:

- **Bot Logística** se comunica con el **Departamento de Tesorería** para la liberación de pagos tras entrega confirmada.  
- El token LOGi vive en el **mismo blockchain** que mantiene el **Nodo 8545** (Tecnología y Nodo).  
- No se maneja dinero fiat en este módulo; el cobro/pago es en LOGi on-chain; la liquidación en moneda fiduciaria queda en el Banco (BDET/BHBK) vía señales internas o APIs de settlement.

---

## 8. REFERENCIAS

- **Código:** `RuddieSolution/node/modules/sovereign-logistics.js`, `RuddieSolution/node/services/logi-token-service.js`, `RuddieSolution/node/server.js` (init LOGi, balance/transfer), `RuddieSolution/node/data/platform-tokens.json` (registro LOGi).  
- **Panel:** `RuddieSolution/platform/logistics.html`; rutas `/logistics`, `/cde`, `/sls`, `/delivery`.  
- **Config:** `RuddieSolution/config/services-ports.json` (logistics_sls_cde, puerto 8545).  
- **Arquitectura BHBK:** `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md`.  

---

*Documento de referencia: Whitepaper SLS/LOGi Production · API base: `/api/v1/logistics` · Token: LOGi en ISB*
