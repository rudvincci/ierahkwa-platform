# 1 nodo International Settlement + 4 nodos Bancos Centrales

## Jerarquía

- **Un nodo** = **International Settlement (SIIS)** — capa superior; liquidación internacional.
- **Debajo**, **4 nodos** = **bancos centrales independientes** (Águila, Quetzal, Cóndor, Caribe). Si cae uno, los otros tres siguen.

```
                    ┌─────────────────────────────────┐
                    │  International Settlement (SIIS) │  ← 1 nodo (puerto 8500)
                    │  NODE_ROLE=settlement             │
                    └──────────┬────────────┬───────────┘
                               │ conectado  │ conectado
         ┌─────────────────────┼────────────┼─────────────────────┐
         │                     │            │                     │
    ┌────▼────┐  ┌──────────────▼──┐  ┌──────▼─────┐  ┌────────────▼────┐
    │ Águila  │◄─┤ todos conectados ├─►│  Quetzal  │◄─┤ Cóndor ↔ Caribe │  ← 4 nodos
    │ 8545    │  │    entre sí     │  │   8546    │  │  8547  ↔  8548  │
    │ R/N/C*  │  └─────────────────┘  │   R/N/C*   │  │    R/N/C*        │
    └─────────┘  * Por nodo: Regional, Nacional, Comercial (separados)
```

**Por nodo:** en cada uno las capas **Regional, Nacional y Comercial** están **separadas** (registros y lógica distintos por nivel). **Entre nodos:** todos están **conectados entre sí** (Settlement ↔ cada central; centrales ↔ entre ellos).

### Por nodo: Regional, Nacional y Comercial (separados)

En **cada nodo** (Settlement y cada banco central) existen tres niveles **separados**:

| Nivel | Descripción |
|-------|-------------|
| **Regional** | Bancos/entidades regionales de esa región (p. ej. en Águila: regionales Norte). |
| **Nacional** | Entidades de alcance nacional dentro de esa región. |
| **Comercial** | Banca comercial y operadores comerciales del nodo. |

Cada nivel tiene su propio registro y lógica; no se mezclan. El admin de los tres niveles se hace desde **Ierahkwa Futurehead BDET Bank (back)**.

### Todos los nodos conectados entre sí

- **Settlement (8500)** ↔ conectado a los **4 bancos centrales** (8545–8548).
- **Cada banco central** ↔ conectado al **Settlement** y a los **otros 3 bancos centrales** (mesh).
- Clearing, SIIS y tráfico entre regiones fluyen por esa red; si un nodo cae, el resto sigue comunicado.

## Puertos y roles

| Nodo | Puerto | NODE_ROLE | NODE_REGION |
|------|--------|-----------|-------------|
| **International Settlement (SIIS)** | 8500 | `settlement` | `SIIS` |
| **Banco Central Águila** | 8545 | `central_bank` | `Aguila` |
| **Banco Central Quetzal** | 8546 | `central_bank` | `Quetzal` |
| **Banco Central Cóndor** | 8547 | `central_bank` | `Condor` |
| **Banco Central Caribe** | 8548 | `central_bank` | `Caribe` |

## Cómo levantar (PM2)

Desde el directorio del Node:

```bash
cd RuddieSolution/node
pm2 start ecosystem.4regions.config.js
```

Se arrancan:

- **1 nodo International Settlement** (8500)
- **4 nodos de bancos centrales** (8545–8548)
- **1 Banking Bridge** (3001) y **1 Editor API** (3002)

Comandos útiles:

```bash
pm2 list
pm2 logs ierahkwa-node-settlement
pm2 logs ierahkwa-node-aguila
pm2 restart ierahkwa-node-condor
```

## Health y roles

Cada nodo expone `role` y `region` en `/health`:

- **Settlement:** `GET http://localhost:8500/health` → `{ "ok": true, "service": "MameyNode", "role": "settlement", "region": "SIIS", "port": 8500, ... }`
- **Central banks:** `GET http://localhost:8545/health` → `{ "role": "central_bank", "region": "Aguila", "port": 8545, ... }` (y 8546, 8547, 8548 para Quetzal, Cóndor, Caribe).

Un balanceador puede usar `role` para enviar tráfico de liquidación al nodo settlement y tráfico regional a cada banco central.

## Producción: un nodo por máquina

En producción:

- **Una máquina** para el **International Settlement** (puerto 8500).
- **Una máquina por región** para cada **banco central** (8545, 8546, 8547, 8548).
- Delante: **balanceador** que haga health check a `/health` y envíe tráfico al settlement o a los central banks según `role` y `region`.

## Relación con la jerarquía bancaria

En `config/services-ports.json` la jerarquía bancaria define SIIS en 6000 y los 4 bancos centrales en 6100–6400. Los **nodos de aplicación** (8500 + 8545–8548) son la capa de proceso: 1 nodo de liquidación internacional y 4 nodos de banco central independientes, con alta disponibilidad (si cae uno de los 4, los otros tres siguen).

## Administración

Todo se administra desde **Ierahkwa Futurehead BDET Bank (back)**. El settlement, los 4 bancos centrales y el registro de bancos se operan desde la interfaz y APIs de BDET Bank.

## Referencias

- **Bancos unificado:** `docs/BANCOS-UNIFICADO-VS-MULTIPLE.md`
- **Arquitectura BHBK:** `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md`
- **Config PM2:** `RuddieSolution/node/ecosystem.4regions.config.js`
- **Health:** `server.js` — `NODE_ROLE`, `NODE_REGION` y `PORT` en la respuesta de `/health`.
