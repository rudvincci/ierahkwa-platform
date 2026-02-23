# Hardware para confirmaciones de gas – transacciones blockchain
## ProLiant EC200a · HP G4 · Cisco · Ierahkwa Sovereign Blockchain

**Fecha:** 28 de Enero de 2026  
**Gobierno Soberano de Ierahkwa Ne Kanienke**

---

## Resumen

Se documenta el uso de **HP ProLiant EC200a**, **HP G4** (EliteDesk/ProDesk) y **Cisco** para las **confirmaciones de gas** de las transacciones en la **Ierahkwa Sovereign Blockchain** (v blockchain), con **conexión a internet** vía equipos Cisco. Estos servidores se estaban **preparando para producción en vivo** (live production); ver inventario de los 5 racks en `docs/RACKS-INVENTARIO-OFICIAL-2026-01-28.md`.

---

## Hardware asignado

| Equipo | Rol | Uso |
|--------|-----|-----|
| **HP ProLiant EC200a** | Nodos de confirmación / validación | Procesamiento y firma de confirmaciones de gas para transacciones. Alta disponibilidad. |
| **HP G4** (EliteDesk / ProDesk) | Nodos ligeros / confirmadores | Confirmaciones de gas, monitoreo de transacciones y estado de la red. |
| **Cisco** (routers / switches) | Red e internet | Conectando todo el stack (ProLiant, HP G4, Mamey Node) a internet; enlace estable para RPC, APIs y confirmaciones. |

---

## Flujo de confirmaciones de gas (v blockchain)

1. **Transacción enviada** → RPC (ej. `eth_sendRawTransaction`) al Mamey Node (puerto 8545).
2. **Gas** → El nodo usa `gasPrice`, `gasLimit`, `gasUsed` (ver `server.js` / estado blockchain).
3. **Confirmación** → Los ProLiant EC200a y HP G4 participan en la validación y emisión de confirmaciones de gas para las transacciones.
4. **Finalidad** → Transacción confirmada y registrada en la blockchain soberana (ISB).

---

## Integración con el código actual

- **RPC gas:** `eth_gasPrice`, `eth_estimateGas` y campos `gas` / `gasUsed` en bloques y receipts están en `RuddieSolution/node/server.js` (estado y métodos RPC).
- **Confirmaciones:** La lógica de confirmación puede delegar a servicios externos (ProLiant EC200a / HP G4) vía API o cola, manteniendo el mismo contrato de gas y receipts.

---

## Especificaciones de referencia (hardware)

| Modelo | Tipo | Uso recomendado |
|--------|------|------------------|
| **ProLiant EC200a** | Servidor edge / compacto | Nodo de confirmación de gas, alta disponibilidad, bajo consumo. |
| **HP EliteDesk / ProDesk G4** | Workstation / cliente robusto | Confirmador ligero, monitoreo, múltiples unidades para redundancia. |
| **Cisco** (routers, switches) | Red | Conectando todo a internet; enlace estable para RPC, APIs y confirmaciones blockchain. |

---

## Cisco – conexión a internet

- **Estado:** En curso la conexión de todo el stack con equipos **Cisco** a internet.
- **Objetivo:** Que ProLiant EC200a, HP G4 y Mamey Node tengan conectividad estable a internet (RPC público, APIs, confirmaciones, monitoreo).
- **Equipos típicos:** Routers Cisco (ISR/ASR o equivalentes), switches para LAN/datacenter, firewall si aplica.
- Registrar aquí modelos, IPs de gestión y diagrama de red cuando esté cerrado.

---

## Próximos pasos sugeridos

1. Definir en cada ProLiant EC200a / HP G4: IP, puerto y rol (confirmador primario/secundario).
2. Conectar estos equipos al Mamey Node (8545) o a un servicio intermedio que consulte `eth_gasPrice` y envíe confirmaciones.
3. **Cisco:** Completar cableado y configuración de routers/switches para que todo el stack salga a internet de forma estable.
4. Registrar en este doc las IP/hostnames y el diagrama de red cuando estén montados (incl. Cisco).

---

---

## Racks físicos (inventario oficial)

Los equipos anteriores están montados en **5 racks** (raquet) documentados en:

- **`docs/RACKS-INVENTARIO-OFICIAL-2026-01-28.md`** — Inventario rack por rack (ProLiant, HP G4, Cisco, UPS AKZOM, monitores) con foto de referencia, para que todo el trabajo sea **encontrable** y facturable.

---

*Documento vivo. Actualizar cuando se completen el montaje y la puesta en producción de ProLiant EC200a, HP G4 y la conexión Cisco a internet.*
