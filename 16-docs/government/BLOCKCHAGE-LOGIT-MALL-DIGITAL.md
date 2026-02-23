# Blockchage · Mall Digital · Logit (Franquicia Américas)

**Sovereign Government of Ierahkwa Ne Kanienke**  
Correo 100% digital · Mall para deliveries · Franquicia local en cada punto de las Américas

**Las Américas y el Caribe son nuestro territorio.** Toda la red de correo (Blockchage), mall digital y logística (Logit, SLS/CDE) opera en y para este territorio soberano.

---

## Principio: Todo digital, cero papel

*¿Para qué hacer más papel cuando todo es digital?*  
El correo propio es **Blockchage** (blockchain + mail). El mall más importante es **digital** y se usa para **deliveries**. La red de última milla en las Américas es **Logit**: franquicia local en cada punto, con recogido de cash del banco y seguros soberanos para cada delivery.

---

## 1. Blockchage — Correo nuestro, 100% digital

- **Nombre:** Blockchage (blockchain + mail).
- **Función:** Correo oficial del ecosistema. **Solo digital** — sin papel, sin sobres físicos para comunicaciones internas o certificadas.
- **Tokens / servicios relacionados:**
  - **IGT-PSI** (Postal Service of Ierahkwa) — token gobierno para mail, paquetes y logística de comunicación.
  - **IGT-MAIL** (Ierahkwa Futurehead Mail) — mensajería y comunicaciones cifradas.
- **Trazabilidad:** Sello y registro on-chain (Ierahkwa Sovereign Blockchain — ISB). Entrega y notificación por SLS/CDE cuando aplica a paquetería.

**Referencias:** `tokens/40-IGT-PSI.json`, `tokens/83-IGT-MAIL.json`, `docs/WHITEPAPER-SLS-LOGI-PRODUCTION.md`.

---

## 2. Mall digital — El mall más importante es digital (solo deliveries)

- **Concepto:** El “mall” principal no es un centro comercial de tiendas físicas; es la **plataforma digital de comercio y entregas**.
- **Uso:** Solo para **deliveries** — pedidos, envíos, tracking y liquidación. No genera papel; todo se gestiona por SLS/CDE y BDET.
- **Integración:** Órdenes desde ciudadanos o comercios → Continental Delivery Engine (CDE) → Logit (última milla) → entrega con sello BHBK y seguros soberanos.

---

## 3. Logit — Franquicia local en cada punto de las Américas

**Las Américas y el Caribe son nuestro territorio.** **Logit** es la **franquicia local** en cada punto de ese territorio: mismo nombre, mismos protocolos, misma red soberana.

### 3.1 Funciones de cada punto Logit

| Función | Descripción |
|--------|-------------|
| **Deliveries** | Última milla de todos los envíos SLS/CDE. Crean órdenes, recogen, entregan y marcan `deliver` en la API. Cobran y reciben recompensas en **LOGi**. |
| **Recogido de cash del banco** | Punto autorizado para recoger efectivo (cash) del BDET/BHBK para circulación local: depósitos/retiros de ciudadanos, liquidez para comercios. |
| **Seguros en deliveries** | Cada delivery usa **seguros soberanos** (BHBK-BACKED / BHBK-SECURED). Logit opera bajo la póliza del ecosistema; no depende de aseguradoras externas. |

### 3.2 Tres modos de transporte: bote, avión, carros — tres tokens

Los deliveries se tokenizan en **tres tokens**, uno por modo de transporte (water, air, terrestre):

| Modo | Token | Código | Uso |
|------|-------|--------|-----|
| **Bote (water)** | **IGT-DELIVERY-SEA** | `SEA` | Marítimo, fluvial. Caribe, costas, ríos (Amazonas, etc.), carga a granel. |
| **Avión (air)** | **IGT-DELIVERY-AIR** | `AIR` | Rutas largas, urgente, islas, cruce de regiones. |
| **Carros (terrestre)** | **IGT-DELIVERY-LAND** | `LAND` | Camión, furgoneta, moto; última milla y rutas continentales. |

- **Puerta a puerta:** Todos los envíos SLS/CDE son **door-to-door** por defecto: recogida en origen y entrega en puerta del destinatario. Logit hace la última milla.
- **Combinación de modos:** Un mismo envío puede usar varios modos (ej. carros → avión → carros; o bote → carros). Cada tramo puede liquidarse con el token que corresponda (IGT-DELIVERY-LAND, IGT-DELIVERY-AIR, IGT-DELIVERY-SEA).

**Tokens:** `tokens/107-IGT-DELIVERY-SEA.json`, `tokens/108-IGT-DELIVERY-AIR.json`, `tokens/109-IGT-DELIVERY-LAND.json`.  
**API:** `transportMode` (`AIR` | `SEA` | `LAND`) en manifiesto y orden. `GET /api/v1/logistics/transport-modes` devuelve los modos.

### 3.3 Tokens: delivery local vs delivery internacional (dos tokens diferentes)

Los deliveries **locales** y **internacionales** usan **dos tokens IGT distintos**:

| Ámbito | Token | Uso |
|--------|-------|-----|
| **Delivery local** | **IGT-DELIVERY-LOC** (105) | Envíos locales (ciudad, misma región). Tarifas, pagos y representación del servicio local. |
| **Delivery internacional** | **IGT-DELIVERY-INT** (106) | Envíos internacionales (cross-border, continente). Tarifas, pagos y representación del servicio internacional. |

- **LOGi** (ISB) sigue siendo el token interno para tarifas on-chain, recompensas y liquidación entre Logit y el treasury SLS; puede convertirse o referenciar IGT-DELIVERY-LOC / IGT-DELIVERY-INT según el tipo de envío.
- **Referencia tokens:** `tokens/105-IGT-DELIVERY-LOC.json`, `tokens/106-IGT-DELIVERY-INT.json`.

### 3.4 Red y tokens

- **Red:** Misma red de nodos SLS (NORTH, CENTRAL, CARIBBEAN, SOUTH). Cada “punto Logit” es un nodo o hub de última milla.
- **Tres tokens por modo:** IGT-DELIVERY-SEA (bote), IGT-DELIVERY-AIR (avión), IGT-DELIVERY-LAND (carros). LOGi para tarifas/recompensas internas.
- **APIs:** `POST /api/v1/logistics/order`, `PUT /api/v1/logistics/:trackingId/move`, `POST /api/v1/logistics/:trackingId/deliver`; integración con BDET para cash pickup y con módulo de seguros para pólizas de envío.

### 3.5 Resumen

- **Blockchage** = correo digital (nuestro), sin papel.  
- **Mall digital** = el mall más importante es digital; solo se usa para deliveries.  
- **Logit** = franquicia local en cada punto de las Américas: deliveries (SLS/CDE); **tres tokens por modo:** **IGT-DELIVERY-SEA** (bote), **IGT-DELIVERY-AIR** (avión), **IGT-DELIVERY-LAND** (carros); además IGT-DELIVERY-LOC e IGT-DELIVERY-INT; LOGi; recogido de cash del banco (BDET/BHBK) y seguros soberanos.

---

## Referencias en código y docs

- **SLS / CDE / LOGi:** `docs/WHITEPAPER-SLS-LOGI-PRODUCTION.md`, `RuddieSolution/node/modules/sovereign-logistics.js`, `RuddieSolution/platform/logistics.html`.
- **BHBK / seguros en envíos:** `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md` (Bot Logística ↔ Tesorería; sello e insurance BHBK en cada envío).
- **BDET / cash:** `RuddieSolution/node/banking-bridge.js`, `RuddieSolution/platform/bdet-bank.html`.
- **Tokens correo/mail:** `tokens/40-IGT-PSI.json`, `tokens/83-IGT-MAIL.json`, `tokens/104-IGT-BLOCKCHAGE.json`.
- **Tokens delivery por modo:** `tokens/107-IGT-DELIVERY-SEA.json` (bote), `tokens/108-IGT-DELIVERY-AIR.json` (avión), `tokens/109-IGT-DELIVERY-LAND.json` (carros).
- **Exchange / trading:** Blockchage y los tokens de delivery (LOC, INT, SEA, AIR, LAND) están integrados en el exchange con **el mismo procedimiento integral que los demás IGT**: pares USD y USDT en `banking-bridge.js` (TRADING_PAIRS, SUPPORTED_CRYPTO), TradeX (TradingService), ierahkwa-shop (trading_pairs y token_registry). Los demás tokens IGT siguen el mismo procedimiento.

---

*Documento de referencia: Blockchage · Logit · Mall Digital — Correo digital, mall por deliveries, franquicia Américas con cash pickup y seguros soberanos.*
