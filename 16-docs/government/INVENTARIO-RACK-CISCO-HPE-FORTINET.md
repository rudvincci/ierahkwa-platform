# Inventario — Racks Cisco, HPE ProLiant, Fortinet

**Sovereign Government of Ierahkwa Ne Kanienke**  
Equipos observados: red, seguridad, cómputo y almacenamiento propios.

---

## 1. Resumen por tipo

| Tipo | Modelos observados | Uso sugerido en stack soberano |
|------|--------------------|---------------------------------|
| **Firewall** | FortiGate **90D**, **100D** | Perímetro, VPN, segmentación (zonas BDET/SWIFT/plataforma) |
| **Switch gestionado** | Cisco SF300-24MP, SF300-24PP, SF300-24P; Catalyst 2960S, 3550 | LAN, VLANs, QoS, PoE donde aplique |
| **Router** | Cisco 800 Series, 2800 Series, **4300 Series** (ISR) | WAN, DMZ, opcional KVM/VM en ISR 4k |
| **Servidores** | HPE ProLiant EC200a, EC200p (múltiples por rack) | Node (8545), Banking Bridge (3001), Rust SWIFT (8590), BDET/SIIS, plataforma |
| **Almacenamiento** | Enclosure 5 bays (hot-swap) | Datos, backups, VM/datos BDET |

---

## 2. Mapeo a servicios IERAHKWA

- **FortiGate 90D / 100D:** Reglas para segmentar tráfico a:
  - Zona **SWIFT:** solo host(s) del servicio Rust (8590) y consumidores (Node, Bridge).
  - Zona **banca:** 3001, 4001–4600 (BDET, SIIS, etc.).
  - Zona **plataforma:** 8545, 8080; no exponer 8590 a internet.
- **Cisco switches:** VLANs por zona (opcional), QoS para priorizar tráfico a 8545/3001/8590.
- **Cisco 4300 (ISR):** Si usas IOS XE con KVM, se puede alojar una VM pequeña (ej. relay o proxy); el resto de servicios en HPE.
- **ProLiant EC200a / EC200p:** Asignar por host:
  - **Host 1:** Node (8545) + Banking Bridge (3001) + platform estático.
  - **Host 2:** Servicio Rust SWIFT (8590) + opcional Go/Python (8591, 8592).
  - **Hosts 3+:** BDET server, TradeX, SIIS, Clearing, AI Hub, etc., según `config/services-ports.json`.
- **Enclosure 5 bays:** Almacenamiento local (DB, backups, logs). Ver sección 3.

---

## 3. Alerta: “ERROR REBUILD” en enclosure

En uno de los equipos de almacenamiento (5 bays) se ve el indicador **ERROR REBUILD**:

- Suele indicar **fallo de un disco** o **rebuild de RAID en curso**.
- **Acciones recomendadas:**
  1. Entrar a la utilidad de gestión del enclosure (por navegador, si tiene IP, o por software del fabricante).
  2. Comprobar estado del RAID y de cada disco (SMART, estado de array).
  3. Si hay disco fallido: reemplazarlo y dejar que el array reconstruya (rebuild).
  4. Asegurar **backups** de datos críticos en otro medio o en otro nodo antes de tocar el array.

Si me indicas marca/modelo exacto del enclosure, se puede documentar la secuencia de comprobación paso a paso.

---

## 4. Referencia rápida de modelos vistos

| Equipo | Notas |
|--------|--------|
| Cisco SF300-24MP / 24PP / 24P | 24 puertos gestionados, PoE/PoE+ en modelos P/PP; ideal para VLANs y QoS |
| Cisco Catalyst 2960S / 3550 | Capa 2/3; 24 puertos + uplinks SFP |
| Cisco 800 Series | Router de sucursal / edge |
| Cisco 2800 Series | Router modular IOS |
| **Cisco 4300 Series** | ISR con IOS XE; opción KVM para VM |
| FortiGate **90D** / **100D** | UTM, firewall, VPN; 100D mayor throughput y capacidad |
| HPE ProLiant EC200a / EC200p | Microservers; varios por rack para reparto de servicios |

---

## 5. Siguientes pasos sugeridos

1. **Red:** Definir VLANs o zonas (SWIFT, banca, plataforma) en FortiGate y en los Cisco SF300/Catalyst; documentar qué puertos van a qué hosts.
2. **Servicios:** Asignar cada servicio de `config/services-ports.json` a un host ProLiant concreto (lista en este doc o en un RUNBOOK).
3. **Almacenamiento:** Resolver ERROR REBUILD del 5-bay; verificar backups y política de RAID.
4. **SWIFT:** Mantener servicio Rust (8590) en un host dedicado o compartido solo con Node/Bridge; reglas de firewall según `docs/CISCO-SWIFT-SOBERANO.md`.

---

*Inventario basado en equipos observados. Actualizar con números de serie y direcciones IP en documento interno de operaciones.*
