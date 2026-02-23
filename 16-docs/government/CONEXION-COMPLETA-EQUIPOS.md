# Conexión completa – Cómo conectar todo el equipo

**Gobierno Soberano de Ierahkwa Ne Kanienke**  
**Fecha:** 13 de Febrero de 2026

Guía para conectar físicamente y configurar: 5 racks de producción + equipos de bodega (NODO 1–6).

---

## 0. TOPOLOGÍA TODO JUNTO (existente + bodega)

```
                                    INTERNET (ISP)
                                          │
                                          ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                     SAGEMCOM (192.168.0.1)                                  │
│              Solo 4–5 puertos → necesitas SWITCH DE DISTRIBUCIÓN             │
└─────────────────────────────────────────────────────────────────────────────┘
                                          │
                                          ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│              CISCO 4200 o CATALYST 3560 (CORE / DISTRIBUCIÓN)               │
│              10.0.0.1 · uplink al Sagemcom · 24–48 puertos                  │
└─────────────────────────────────────────────────────────────────────────────┘
   Puertos 1–9 (cada uno → un rack o NODO)
        │
        ├─ P1 → RACK 1 (FortiGate)    ├─ P6 → NODO 1 (switches + HP/IBM)
        ├─ P2 → RACK 2 (FortiGate)   ├─ P7 → NODO 2 (Great Lakes + servidores)
        ├─ P3 → RACK 3 (FortiGate)   ├─ P8 → NODO 3 (Great Lakes + servidores)
        ├─ P4 → RACK 4 (FortiGate)   └─ P9 → NODO 6 (edge, Ruckus, MikroTik)
        └─ P5 → RACK 5 (switch, comparte FortiGate con Rack 4)
```

**Resumen:** 1 switch Core recibe internet del Sagemcom y reparte a 9 puntos: 5 racks existentes + 4 NODOs bodega.

### Tabla de cables – TODO JUNTO

| Cable | Desde | Hasta |
|-------|-------|-------|
| C-SAG-CORE | Sagemcom LAN 1 | Cisco 4200 / Catalyst 3560 puerto 1 |
| C-CORE-R1 | Core puerto 2 | FortiGate Rack 1 WAN |
| C-CORE-R2 | Core puerto 3 | FortiGate Rack 2 WAN |
| C-CORE-R3 | Core puerto 4 | FortiGate Rack 3 WAN |
| C-CORE-R4 | Core puerto 5 | FortiGate Rack 4 WAN |
| C-CORE-N1 | Core puerto 6 | NODO 1 switch core (Catalyst 3560 o 2960) |
| C-CORE-N2 | Core puerto 7 | NODO 2 switch principal |
| C-CORE-N3 | Core puerto 8 | NODO 3 switch principal |
| C-CORE-N6 | Core puerto 9 | NODO 6 switch |
| C-R4-R5 | FortiGate Rack 4 LAN | Switch Rack 5 (Rack 5 comparte con Rack 4) |
| R1-FG | FortiGate R1 LAN | Cisco 800 R1 WAN |
| R1-CS | Cisco 800 R1 LAN | Switch R1 puerto 1 |
| ... | (igual en R2, R3, R4) | ... |

### Asignación de equipos – TODO JUNTO

| Ubicación | Equipos a montar |
|-----------|------------------|
| **Core (1 switch)** | Cisco 4200 o Catalyst 3560G 48pt – recibe internet del Sagemcom y reparte a todos |
| **Rack 1** | FortiGate 90D, Cisco 800, Catalyst 2960 Plus, 7 ProLiant EC200a, NAS 5 bahías, UPS |
| **Rack 2** | FortiGate 90D, Cisco 800, Catalyst 2960G, 7 ProLiant EC200a, NAS 5 bahías, UPS |
| **Rack 3** | FortiGate 100D, Cisco 800, SF300 24pt, 5 ProLiant EC200a, NAS 5 bahías, UPS |
| **Rack 4** | FortiGate 100D, Cisco 800, SF300 24pt, 5 HP G4, 2 NAS, UPS |
| **Rack 5** | Switch (conectado a Rack 4), 5 HP G4, NAS, UPS |
| **NODO 1** | 2 Catalyst 3560G (core del NODO), 8–16 Catalyst 2960 (access), servidores HP DL360, IBM x3650 |
| **NODO 2** | Switches Catalyst 2960, servidores HP DL360 G7, blade enclosures, storage arrays |
| **NODO 3** | Igual que NODO 2 – HP, IBM, Dell Precision |
| **NODO 6** | Switch, Ruckus ZoneDirector, MikroTik, equipos edge |

### Conexiones dentro de cada NODO (bodega)

```
Core (uplink desde distribución)
    │
    ▼
Switch principal del NODO (Catalyst 3560 o 2960 48pt)
    │
    ├── Switch access 1 (Catalyst 2960 24pt) ──► Servidores 1–24
    ├── Switch access 2 (Catalyst 2960 24pt) ──► Servidores 25–48
    ├── Switch access 3 (...) ──► más servidores
    └── ...
```

---

## 1. Vista general del flujo (solo 5 racks existentes)

```
                              INTERNET (ISP)
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                     SAGEMCOM F@ST 3890 (192.168.0.1)                        │
│                     Router ISP – 4–5 puertos LAN                             │
└─────────────────────────────────────────────────────────────────────────────┘
        │              │              │              │              │
   Puerto 1       Puerto 2       Puerto 3       Puerto 4       Puerto 5
        │              │              │              │              │
        ▼              ▼              ▼              ▼              ▼
   [RACK 1]       [RACK 2]       [RACK 3]       [RACK 4]       Mac/Admin
   FortiGate      FortiGate      FortiGate      FortiGate      o Linksys
   → Cisco        → Cisco        → Cisco        → Cisco
   → Switch       → Switch       → Switch       → Switch
   → Servidores   → Servidores   → Servidores   → HP G4 × 10
```

---

## 2. Orden de conexión por rack (producción)

Cada rack sigue el mismo esquema:

| Paso | Origen | Destino | Cable |
|------|--------|---------|-------|
| 1 | Sagemcom LAN 1–4 | FortiGate WAN1 | RJ45 Cat6 |
| 2 | FortiGate LAN 1 | Cisco 800 GE0 (WAN) | RJ45 |
| 3 | Cisco 800 GE1 (LAN) | Switch puerto 1 (uplink) | RJ45 |
| 4 | Switch puertos 2–24 | Servidores ProLiant / HP G4 | RJ45 |
| 5 | Switch | NAS 5 bahías | RJ45 |
| 6 | UPS | Todos los equipos del rack | IEC |

---

## 3. Puertos por equipo

### FortiGate 90D / 100D
| Puerto | Función | Conectar a |
|--------|---------|------------|
| WAN1 | Entrada internet | Sagemcom |
| LAN 1–10 | Red interna | Cisco 800 WAN |
| Console | Consola | Cable USB-serial |

### Cisco 800 (o 2800/2900)
| Puerto | Función | Conectar a |
|--------|---------|------------|
| GE0 / WAN | Entrada | FortiGate LAN |
| GE1 / LAN | Salida | Switch puerto 1 |
| Console | Consola | Cable serial |

### Cisco Catalyst / SF300
| Puerto | Función | Conectar a |
|--------|---------|------------|
| 1 | Uplink | Cisco LAN |
| 2–24 (o 48) | Access | Servidores, NAS |

---

## 4. Esquema físico por rack

```
┌─────────────────────────────────────────────────────────────────┐
│                         RACK (de arriba abajo)                   │
├─────────────────────────────────────────────────────────────────┤
│  [1] FortiGate    WAN◄──Sagemcom    LAN──►Cisco                  │
│  [2] Cisco 800    WAN◄──FortiGate   LAN──►Switch port 1         │
│  [3] Switch       port 1◄──Cisco    ports 2-24──►Servidores     │
│  [4] ProLiant ×7  cable Ethernet a Switch                       │
│  [5] NAS 5 bahías cable Ethernet a Switch                       │
│  [6] Monitor      consola/KVM                                     │
│  [7] UPS          alimentación de todo                           │
└─────────────────────────────────────────────────────────────────┘
```

---

## 5. Integración de equipos de bodega (NODO 1–6)

### Opción A: NODOs como racks adicionales

Si incorporas los racks de bodega:

```
Sagemcom
    │
    ├── Rack 1 (existente)
    ├── Rack 2 (existente)
    ├── Rack 3 (existente)
    ├── Rack 4 (existente)
    ├── Rack 5 (comparte FortiGate con Rack 4)
    │
    ├── NODO 1 ──► Switch Core (Cisco Catalyst) ──► HP DL360, IBM, switches
    ├── NODO 2 ──► Switch Core ──► servidores
    ├── NODO 3 ──► Switch Core ──► servidores
    └── NODO 6 ──► Switch Core ──► edge/servicios
```

Necesitas más puertos LAN en el Sagemcom o un switch de distribución:

```
Sagemcom → Switch de distribución (Cisco 2960/4200) → N puertos
                                              ├── FortiGate Rack 1
                                              ├── FortiGate Rack 2
                                              ├── FortiGate Rack 3
                                              ├── FortiGate Rack 4
                                              ├── NODO 1 (Meraki MX80 o FortiGate)
                                              ├── NODO 2
                                              └── ...
```

### Opción B: NODOs detrás de un único FortiGate (más económico)

```
Sagemcom LAN 1 ──► FortiGate principal (o Cisco 4300)
                        │
                        ├── VLAN 10 ──► Switch core ──► Racks 1–5
                        ├── VLAN 20 ──► NODO 1 (switches → servidores HP/IBM)
                        ├── VLAN 21 ──► NODO 2
                        ├── VLAN 22 ──► NODO 3
                        └── VLAN 23 ──► NODO 6
```

---

## 6. Direccionamiento IP sugerido

Basado en `INFRASTRUCTURE-SETUP.md`:

| Red | VLAN | Uso | Rango |
|-----|------|-----|-------|
| 10.0.0.0/24 | 10 | Management | Routers, switches, FortiGate |
| 10.0.10.0/24 | 20 | Servidores | ProLiant, Node 8545, Bridge 3001 |
| 10.0.11.0/24 | 30 | Servicios | SWIFT 8590, Go 8591, Python 8592 |
| 10.0.12.0/24 | 40 | Base de datos | MongoDB, Redis |
| 10.0.20.0/24 | 50 | Mining / blockchain | HP G4 |
| 10.0.100.0/24 | 80 | DMZ | Servicios públicos |
| 10.0.200.0/24 | 90 | Guest / IoT | WiFi, cámaras |

### Servidores principales

| Servicio | IP | Puerto |
|----------|-----|--------|
| Node Mamey | 10.0.10.1 | 8545 |
| Banking Bridge | 10.0.10.1 | 3001 |
| SWIFT Rust | 10.0.10.4 | 8590 |
| Platform static | 10.0.10.1 | 8080 |

---

## 7. Equipos de bodega – qué conectar a qué

### Racks NODO 1, 2, 3, 6

| Rack | Contenido típico | Conexión |
|------|------------------|----------|
| NODO 1 | ~16 switches Catalyst 2960, routers | Switch uplink al core; servidores a switches |
| NODO 2 | Great Lakes 42U vacío/listo | Montar switches + servidores HP/IBM |
| NODO 3 | Great Lakes 42U vacío | Igual que NODO 2 |
| NODO 6 | Gabinete 9–12U | Edge o punto de acceso |

### Switches Cisco (bodega)

- **Catalyst 2960 (24/48 pt):** access – cada servidor a un puerto.
- **Catalyst 3560G:** core o distribución – uplinks a otros switches.
- **Cisco 4200:** core – uplink a Sagemcom o FortiGate.
- **Cables:** Uplink entre switches con puertos Gigabit o SFP.

### Servidores HP / IBM (bodega)

- Cada servidor: 1–2 cables Ethernet al switch del rack.
- IPs en 10.0.10.x o 10.0.11.x según servicio.
- Asignar servicios según `config/services-ports.json`.

### Routers y firewalls (bodega)

| Equipo | Función | Conexión |
|--------|---------|----------|
| Cisco 4200 | Core/WAN | Entre Sagemcom y switches |
| Meraki MX80 | Firewall edge | WAN←internet, LAN→switches NODO |
| MikroTik RB3011 | Router/VPN | Backup o segmento específico |
| Cisco ASA | Firewall | Perímetro o segmento DMZ |

### UPS y PDUs

- Cada rack: 1–2 UPS alimentando todo.
- PDUs: de UPS a equipos del rack.
- No mezclar circuitos entre racks si es posible.

---

## 8. Checklist de conexión

### Antes de conectar

- [ ] Todo apagado
- [ ] Cables etiquetados (SAG→FG1, FG1→C8-1, etc.)
- [ ] UPS conectado a la pared
- [ ] Consola (Mac + adaptador USB-serial) lista

### Por rack

- [ ] FortiGate WAN → Sagemcom (o switch de distribución)
- [ ] FortiGate LAN → Cisco WAN
- [ ] Cisco LAN → Switch puerto 1
- [ ] Servidores y NAS → Switch (puertos 2+)
- [ ] UPS alimentando todo el rack

### Configuración

- [ ] FortiGate: política LAN→WAN, NAT
- [ ] Cisco: DHCP o IP fija en LAN, NAT
- [ ] Port forwarding (80, 443, 8545, 3001) hacia Node

---

## 9. Port forwarding (para que llegue internet a la plataforma)

En el Sagemcom (o dispositivo que haga NAT):

| Puerto | Redirigir a | Servicio |
|--------|-------------|----------|
| 80 | 10.0.10.1 | HTTP |
| 443 | 10.0.10.1 | HTTPS |
| 8545 | 10.0.10.1 | Mamey Node |
| 3001 | 10.0.10.1 | Banking Bridge |
| 8080 | 10.0.10.1 | Platform static |

---

## 10. Documentos de referencia

| Documento | Contenido |
|-----------|-----------|
| `DEPLOY-SERVERS/PLANOS-CONEXION-COMPLETO.md` | Pasos detallados por rack |
| `DEPLOY-SERVERS/PLAN-CONEXION-Y-CONFIGURACION.md` | Opciones A/B, plan de acción |
| `DEPLOY-SERVERS/CONFIG-MINIMA-FORTIGATE-CISCO-INTERNET.md` | Config FortiGate y Cisco |
| `INFRASTRUCTURE-SETUP.md` | VLANs, Cisco ISR4331, direccionamiento |
| `docs/INVENTARIO-BODEGA-COSTOS-REALES-2026-02.md` | Inventario con costos |
| `DEPLOY-SERVERS/RACKS-INDEPENDIENTES.md` | Topología racks independientes |

---

## 11. Resumen – orden de conexión

1. **Sagemcom** encendido y con internet.
2. **Por rack:** Sagemcom → FortiGate WAN → Cisco WAN → Switch → Servidores.
3. **UPS** alimentando cada rack.
4. **Configurar** FortiGate y Cisco por consola.
5. **Probar** `ping 8.8.8.8` desde un servidor.
6. **Bodega:** Conectar NODOs como racks adicionales o por VLANs detrás del core.
