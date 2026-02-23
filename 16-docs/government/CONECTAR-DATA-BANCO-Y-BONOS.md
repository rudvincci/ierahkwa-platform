# Conectar la data del banco y de los bonos al proyecto

**Sovereign Government of Ierahkwa Ne Kanienke**  
Dónde está la data, qué APIs la exponen y cómo conectarla (frontend, scripts, otros servicios).

---

## 1. Data del banco (BDET / bancos)

### Dónde está la data

| Data | Archivo / carpeta | Descripción |
|------|-------------------|-------------|
| **Registro de bancos** | `RuddieSolution/node/data/bank-registry.json` | Bancos centrales y regionales (BDET, Águila, Quetzal, Cóndor, Caribe, etc.). Una sola fuente en el Node. |
| **Cuentas gobierno (BDET)** | `RuddieSolution/node/data/bdet-bank/gov-accounts.json` | Cuentas del tesoro, ministerios, reservas de tokens. Lo usa `government-banking.js` y `bdet-server.js`. |
| **Transacciones gobierno** | `RuddieSolution/node/data/bdet-bank/gov-transactions.json` | Historial de depósitos, transferencias, retiros. |
| **Solicitudes banco** | `RuddieSolution/node/data/bank-submissions/` | SIP-BDET-*.json (submissions). |

### APIs que ya exponen la data (Node 8545)

| Endpoint | Qué devuelve |
|----------|----------------|
| `GET /api/v1/bdet/bank-registry` | Lista de bancos desde `bank-registry.json`. |
| `GET /api/v1/bdet/status` | Estado del módulo BDET (si está cargado). |
| `GET /api/v1/bdet-server/health` | Salud del servidor BDET. |
| `GET /api/v1/bdet-server/api/gov-accounts` | Cuentas gobierno (desde gov-accounts.json). |
| `GET /api/v1/bdet-server/api/gov-transactions` | Transacciones gobierno (desde gov-transactions.json). |
| Rutas de `government-banking.js` montadas en `/api/v1/bdet` | Cuentas, transferencias, depósitos, departamentos (según el módulo). |

### Cómo conectar la data del banco al proyecto

1. **Frontend (platform):**  
   - Hacer `fetch` al Node (mismo origen si abres desde `http://localhost:8545`).  
   - Ejemplo: `fetch('/api/v1/bdet/bank-registry')` o `fetch('/api/v1/bdet-server/api/gov-accounts')`.  
   - Las páginas que ya usan esta data: `platform/bdet-bank.html`, `platform/bank-worker.html`, `platform/treasury-dashboard.html`, etc. Usan el mismo Node 8545.

2. **Otro servicio (misma máquina o red):**  
   - `BDET_API_URL=http://localhost:8545/api/v1/bdet` (ej. en `workforce-soberano.js`, `pagos-soberano.js`).  
   - Llamar a los endpoints anteriores desde tu script o servicio.

3. **Añadir o editar data:**  
   - **bank-registry:** editar `RuddieSolution/node/data/bank-registry.json` y reiniciar no es necesario; el siguiente `GET /api/v1/bdet/bank-registry` lee el archivo.  
   - **Cuentas/transacciones BDET:** se modifican vía el módulo Government Banking (crear cuenta, depósito, transferencia) y se persisten en `bdet-bank/gov-accounts.json` y `gov-transactions.json`.

4. **Importar data externa (ej. CSV) al banco:**  
   - Crear un script o ruta que lea tu archivo, valide y llame a las funciones del módulo (o escriba con cuidado en los JSON) y luego reinicie o deje que el Node use los JSON actualizados. No pisar el formato que esperan `government-banking.js` y `bdet-server.js`.

---

## 2. Data de los bonos (bonds)

### Dónde está la data

- **VIP Transactions (incluye bonos):** `RuddieSolution/node/data/vip-transactions.json`  
  - Contiene `transactions`, `assets` (entre ellos `assets.bonds`) y `stats`.  
  - Las transacciones de tipo **bond** se clasifican en `assets.bonds` y se usan en reportes y en la UI de VIP.

### APIs que ya exponen la data de bonos (Node 8545)

| Endpoint | Qué devuelve |
|----------|----------------|
| `GET /api/v1/vip/transactions` | Lista de todas las transacciones VIP (incluidas las de tipo bond). |
| `GET /api/v1/vip/stats` | Estadísticas y conteos por tipo de activo (incl. `bonds`). |
| `GET /api/v1/vip/report` | Reporte financiero VIP; incluye `byAssetType.bonds` (count y value). |
| `POST /api/v1/vip/transactions` | Crear una transacción; con `type: 'bond'` se añade a la data de bonos y se persiste en `vip-transactions.json`. |

### Cómo conectar la data de bonos al proyecto

1. **Mostrar bonos en la plataforma:**  
   - La página `platform/vip-transactions.html` ya consume `GET /api/v1/vip/transactions` y `GET /api/v1/vip/stats`.  
   - Filtrar en frontend por `transaction.type === 'bond'` (o `'bonds'`) para listar solo bonos.

2. **Incorporar bonos desde un sistema externo (ej. “bondo” o CSV):**  
   - **Opción A (recomendada):** Llamar a `POST /api/v1/vip/transactions` con body:
     ```json
     {
       "type": "bonds",
       "description": "Descripción del bono",
       "value": 1000000,
       "origin": "Origen",
       "destination": "Destino",
       "data": { "isin": "XX123", "emitter": "Tesorería" }
     }
     ```
     (Usar `type: "bonds"` para que se clasifique en `assets.bonds` y aparezca en reportes.)
     Así la data del bono queda en `vip-transactions.json` y en reportes/stats.
   - **Opción B:** Script que lea tu archivo (CSV/JSON), valide y haga un `POST` por cada bono al endpoint anterior.

3. **Solo lectura desde otro servicio:**  
   - `fetch('http://localhost:8545/api/v1/vip/transactions')` o `.../api/v1/vip/report` y filtrar/agregar por tipo bond en tu código.

---

## 3. Resumen rápido

| Quieres… | Dónde está la data | Cómo conectar |
|----------|--------------------|----------------|
| Lista de bancos | `node/data/bank-registry.json` | `GET /api/v1/bdet/bank-registry` |
| Cuentas y movimientos BDET | `node/data/bdet-bank/gov-accounts.json`, `gov-transactions.json` | `GET /api/v1/bdet-server/api/gov-accounts`, `.../gov-transactions` o rutas en `/api/v1/bdet` |
| Bonos (VIP) | `node/data/vip-transactions.json` → `assets.bonds` | `GET /api/v1/vip/transactions`, `GET /api/v1/vip/report`; añadir: `POST /api/v1/vip/transactions` con `type: 'bonds'` |
| Usar desde la web | Mismo origen 8545 | Abrir platform desde `http://localhost:8545` y usar los endpoints anteriores. |

Todo lo anterior usa **solo el Node (8545)** y archivos bajo `RuddieSolution/node/data/`. Principio: todo propio, sin depender de servicios externos para esta data.
