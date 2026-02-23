# Software soberano para red y seguridad

**Sovereign Government of Ierahkwa Ne Kanienke**  
Objetivo: no depender de Fortinet, Cisco ni otros proveedores regulados por otros gobiernos. Stack 100% código abierto o propio.

---

## 1. Reemplazo por equipo

| Equipo actual | Software propietario / regulado | Reemplazo soberano (open source) |
|---------------|----------------------------------|-----------------------------------|
| **FortiGate 90D / 100D** | FortiOS (Fortinet, USA) | **OPNsense** o **pfSense** (BSD, comunidad) |
| **Cisco SF300 / Catalyst** | Cisco IOS (USA) | **VyOS** (GNU/Linux) o **OpenWrt** (en hardware compatible) |
| **Cisco 800 / 2800 / 4300** | Cisco IOS / IOS XE (USA) | **VyOS** o **Linux + FRR** (routing) |
| **HPE ProLiant** | No obligatorio cambiar HW | **Debian/Ubuntu Server** o **AlmaLinux** + nuestro stack (Node, Rust, etc.) |
| **Almacenamiento (enclosure)** | Firmware propietario (si aplica) | **ZFS** o **Linux mdadm/LVM** en servidor Linux conectado |

Ninguno de los reemplazos está controlado por un único gobierno; las comunidades y fundaciones son internacionales.

---

## 2. Firewall / UTM — OPNsense o pfSense

Reemplazan **FortiGate** (Fortinet).

| | OPNsense | pfSense |
|---|----------|--------|
| **Base** | FreeBSD | FreeBSD |
| **Licencia** | BSD | Apache 2.0 |
| **Web UI** | Sí | Sí |
| **VPN** | OpenVPN, WireGuard, IPsec | OpenVPN, WireGuard, IPsec |
| **IDS/IPS** | Suricata | Suricata, Snort |
| **Proxy** | Squid, filtro contenido | Squid |

**Recomendación:** OPNsense (desvío de la rama comercial de pfSense). Instalación en un PC/x86 o VM dedicada; las reglas de firewall se migran desde FortiGate (zonas SWIFT, banca, plataforma).

Configs de ejemplo en: **`sovereign-network/opnsense/`**.

---

## 3. Routing y switching — VyOS o Linux + FRR

Reemplazan **Cisco IOS** para routing y, donde el hardware lo permita, lógica de capa 3.

| | VyOS | Linux + FRR |
|---|------|-------------|
| **Base** | Debian | Cualquier Linux |
| **Formato** | CLI tipo Cisco | Config en archivos + CLI FRR |
| **Routing** | BGP, OSPF, estático | BGP, OSPF, estático (FRR) |
| **Firewall** | Integrado (nftables) | nftables / iptables |

- **VyOS:** imagen instalable en x86 o en máquina virtual; se puede desplegar en un ProLiant o en un appliance.
- **Cisco físico:** si se mantiene el hardware Cisco, se puede usar solo como **switch capa 2** (VLANs básicas) y poner el routing y firewall en OPNsense/VyOS/Linux.

Configs de ejemplo en: **`sovereign-network/vyos/`** y **`sovereign-network/linux-frr/`**.

---

## 4. Servidores (ProLiant) — Linux + stack propio

En **HPE ProLiant EC200a/EC200p** no hace falta software propietario:

- **SO:** Debian 12, Ubuntu 22.04 LTS o AlmaLinux 9 (todas abiertas).
- **Servicios IERAHKWA:** Node (8545), Banking Bridge (3001), Rust SWIFT (8590), etc., según `RuddieSolution/config/services-ports.json`.
- **Hipervisor (opcional):** KVM + libvirt (open source); no dependencia de VMware/Hyper-V.

Script de referencia: **`scripts/instalar-stack-soberano-servidor.sh`**.

---

## 5. Almacenamiento

- **Enclosure 5 bays:** si el firmware es propietario, opción mínima: usarlo en modo JBOD/passthrough y gestionar RAID/volúmenes en Linux (mdadm, ZFS).
- **Soberano 100%:** servidor Linux con discos directos + **ZFS** (OpenZFS) o **mdadm + LVM**; backups a otro nodo o medio bajo nuestro control.

---

## 6. Resumen de dependencias eliminadas

| Antes (regulados por otros gobiernos) | Después (soberano) |
|---------------------------------------|--------------------|
| Fortinet (FortiGate) | OPNsense / pfSense |
| Cisco IOS / IOS XE | VyOS o Linux + FRR |
| (Opcional) VMware / Windows | KVM, Linux |
| Firmware NAS propietario | ZFS / mdadm en Linux |

---

## 7. Orden de implementación sugerido

1. **Fase 1 — Firewall:** Instalar OPNsense en un equipo o VM; replicar reglas actuales del FortiGate (zonas, NAT, VPN); conmutar tráfico al nuevo firewall y validar.
2. **Fase 2 — Servidores:** Asegurar que todos los ProLiant corran Linux + stack IERAHKWA; ningún servicio crítico en Windows ni en hipervisor propietario.
3. **Fase 3 — Routing:** Introducir VyOS (o Linux+FRR) en un nodo; migrar rutas estáticas/BGP desde Cisco; usar Cisco solo como L2 si se mantiene.
4. **Fase 4 — Almacenamiento:** Definir política de almacenamiento con ZFS o mdadm en Linux; backups y réplicas en infra propia.

---

## 8. Referencia de archivos en el repo

| Qué | Dónde |
|-----|--------|
| Estrategia (este doc) | `docs/SOBERANO-SOFTWARE-RED-Y-SEGURIDAD.md` |
| Configs OPNsense (ejemplo) | `sovereign-network/opnsense/` |
| Configs VyOS (ejemplo) | `sovereign-network/vyos/` |
| Firewall Linux (nftables) | `sovereign-network/linux-nftables/` |
| Instalador stack en servidor | `scripts/instalar-stack-soberano-servidor.sh` |
| Inventario físico | `docs/INVENTARIO-RACK-CISCO-HPE-FORTINET.md` |
| Cisco + SWIFT | `docs/CISCO-SWIFT-SOBERANO.md` |

---

*Sovereign Government of Ierahkwa Ne Kanienke — Red y seguridad propias, sin dependencia de tres compañías reguladas por otros gobiernos.*
