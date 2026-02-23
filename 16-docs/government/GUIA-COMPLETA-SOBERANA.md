# Guía completa — Infraestructura y servicios soberanos

**Sovereign Government of Ierahkwa Ne Kanienke**  
Todo en un solo documento: equipo, software propio, migración paso a paso y referencias.

---

## Índice

1. [Objetivo y alcance](#1-objetivo-y-alcance)
2. [Inventario de equipo](#2-inventario-de-equipo)
3. [¿Hay que abrir los equipos?](#3-hay-que-abrir-los-equipos)
4. [Qué reemplaza qué (software soberano)](#4-qué-reemplaza-qué-software-soberano)
5. [Fase 1 — Firewall (sustituir FortiGate)](#5-fase-1--firewall-sustituir-fortigate)
6. [Fase 2 — Servidores Linux y stack IERAHKWA](#6-fase-2--servidores-linux-y-stack-ierahkwa)
7. [Fase 3 — Routing (sustituir Cisco IOS)](#7-fase-3--routing-sustituir-cisco-ios)
8. [Fase 4 — Almacenamiento](#8-fase-4--almacenamiento)
9. [Servicios de aplicación (email, storage, AI, SMS)](#9-servicios-de-aplicación-email-storage-ai-sms)
10. [Red y SWIFT propio (Cisco + Rust)](#10-red-y-swift-propio-cisco--rust)
11. [Checklist y referencias](#11-checklist-y-referencias)

---

## 1. Objetivo y alcance

- **No depender** de Fortinet, Cisco ni otros proveedores regulados por otros gobiernos.
- **Firewall y routing:** software open source (OPNsense, VyOS, nftables).
- **Servidores:** Linux + nuestro stack (Node, Rust SWIFT, Banking Bridge, etc.).
- **Aplicación:** email, storage, pagos, AI y SMS propios (sin SendGrid, AWS, Stripe, OpenAI, Twilio obligatorios).

Esta guía enlaza inventario, estrategia, configs de ejemplo y scripts del repo.

---

## 2. Inventario de equipo

| Tipo | Modelos | Uso en stack soberano |
|------|--------|------------------------|
| **Firewall** | FortiGate 90D, 100D | Sustituir por OPNsense (Fase 1) |
| **Switch** | Cisco SF300-24MP/PP/P, Catalyst 2960S/3550 | VLANs, QoS; puede quedar como L2 |
| **Router** | Cisco 800, 2800, 4300 Series | Sustituir routing por VyOS (Fase 3) |
| **Servidores** | HPE ProLiant EC200a, EC200p | Linux + Node, Bridge, Rust, BDET (Fase 2) |
| **Almacenamiento** | Enclosure 5 bays | ZFS/mdadm en Linux (Fase 4) |

Detalle completo: **`docs/INVENTARIO-RACK-CISCO-HPE-FORTINET.md`**.

**Alerta:** Si el enclosure muestra "ERROR REBUILD", revisar estado del RAID y hacer backup antes de tocar el array.

---

## 3. ¿Hay que abrir los equipos?

Depende del equipo y de lo que quieras hacer.

### No hace falta abrirlos (solo acceso por red o consola)

- **FortiGate 90D/100D:** No es necesario abrir la caja para “sustituirlos”. Se reemplazan poniendo **otro equipo** con OPNsense (PC o VM) en la red y pasando el tráfico por ahí. Los FortiGate se pueden apagar y dejar de usar; no hace falta flashear ni abrirlos.
- **Cisco SF300, Catalyst, 800, 2800, 4300:** Para **solo configurarlos** (VLANs, QoS, rutas) o para **dejarlos como switch L2** no hay que abrirlos. Se accede por:
  - **Web:** IP del switch/router en el navegador.
  - **SSH/Telnet:** `ssh admin@<IP>` (usuario según modelo).
  - **Consola serial:** cable RJ-45/DB-9 a puerto CONSOLE; no requiere abrir la tapa.
- **ProLiant EC200a/EC200p:** Para instalar **Linux** y el stack soberano normalmente **no** hace falta abrir: instalas desde USB o PXE (IPMI/iLO si tiene). Solo abres si vas a cambiar discos, RAM o tarjetas.

### Sí hace falta abrirlos (solo en estos casos)

- **ProLiant:** Si añades o cambias **discos, RAM o NIC**: quitar la tapa, seguir las instrucciones de HPE (antistática, cierre de pestillos).
- **Enclosure 5 bays:** Para **sacar o meter discos** (hot-swap): suele bastar con sacar la bandeja del bay desde el frontal; no siempre hace falta abrir todo el chasis. Para revisar LEDs o conectores internos, a veces sí se abre.
- **Cisco/FortiGate:** Solo si vas a **añadir módulos** (tarjetas, SFP) o **revisar ventilación/limpieza**. Para cambiar a software soberano (OPNsense/VyOS) **no** hace falta abrirlos: usas otro hardware.

### Resumen

| Equipo | ¿Abrir para migrar a software soberano? | ¿Abrir para configurar? |
|--------|----------------------------------------|--------------------------|
| FortiGate 90D/100D | **No** (sustituyes por otro equipo con OPNsense) | No |
| Cisco switch/router | **No** (sustituyes routing por VyOS en otro equipo; Cisco puede quedar L2) | No (web/SSH/consola) |
| ProLiant | **No** para instalar Linux por USB/PXE | Solo si cambias disco/RAM/NIC |
| Enclosure 5 bays | Solo para **cambiar discos** (hot-swap por delante) | No (gestión por red si tiene IP) |

**Conclusión:** Para implementar el stack soberano (OPNsense, VyOS, Linux en ProLiant) **no es necesario abrir** FortiGate ni Cisco. Solo abres ProLiant o el enclosure cuando toque cambiar o revisar hardware (discos, RAM, etc.).

---

## 4. Qué reemplaza qué (software soberano)

| Equipo / dependencia actual | Reemplazo soberano | Dónde está |
|-----------------------------|--------------------|------------|
| FortiGate 90D/100D | OPNsense o pfSense | `sovereign-network/opnsense/` |
| Cisco IOS (routing) | VyOS o Linux + FRR | `sovereign-network/vyos/` |
| Firewall en servidor | nftables (Linux) | `sovereign-network/linux-nftables/` |
| VMware / Windows en servidores | Linux (Debian/Ubuntu/Alma) + KVM si hace falta | Script instalación más abajo |
| SendGrid | Email soberano (cola + sendmail/SMTP) | `docs/SERVICIOS-SOBERANOS.md` |
| AWS S3 | Storage soberano (archivos locales) | `docs/SERVICIOS-SOBERANOS.md` |
| OpenAI/Anthropic | Ollama (AI local) | `./scripts/instalar-ollama.sh` |
| Stripe | Pagos soberano (IGT, intención) | `docs/SERVICIOS-SOBERANOS.md` |
| Twilio | SMS soberano (cola + Telecom API) | `docs/SERVICIOS-SOBERANOS.md` |

Estrategia detallada: **`docs/SOBERANO-SOFTWARE-RED-Y-SEGURIDAD.md`**.

---

## 5. Fase 1 — Firewall (sustituir FortiGate)

### 5.1 Objetivo

Poner **OPNsense** (o pfSense) en un PC x86 o VM dedicada y replicar la segmentación actual (zonas SWIFT, banca, plataforma).

### 5.2 Pasos

1. **Descargar OPNsense**  
   - https://opnsense.org/download/  
   - Imagen ISO para x86 (AMD64). Grabar en USB o usar en VM.

2. **Instalar en el equipo elegido**  
   - Boot desde USB/ISO, seguir el asistente.  
   - Asignar interfaces: WAN (internet), LAN (red interna), OPT1/OPT2/OPT3 para SWIFT, BANCA, PLATAFORMA si usas redes separadas.

3. **Acceder a la interfaz web**  
   - Por defecto: `https://192.168.1.1` (o la IP que hayas puesto en LAN).  
   - Usuario: `root`, contraseña la que definiste en la instalación.

4. **Importar o recrear reglas**  
   - Usar como referencia: **`sovereign-network/opnsense/firewall-rules.txt`**.  
   - En OPNsense: **Firewall > Rules**.  
   - Reglas mínimas:
     - WAN: denegar por defecto; permitir solo VPN (WireGuard/OpenVPN) y lo que necesites exponer (ej. HTTPS).
     - LAN → SWIFT: permitir solo desde IPs de Node/Banking Bridge hacia puerto **8590** (Rust SWIFT).
     - No exponer **8590** a WAN.

5. **VPN (opcional)**  
   - Ejemplo WireGuard en: **`sovereign-network/opnsense/wireguard-server.conf`**.  
   - En OPNsense: **VPN > WireGuard**; generar claves y pegar AllowedIPs por cliente.

6. **Conmutar tráfico**  
   - Cuando las reglas estén listas, cambiar el cable/fibra de internet del FortiGate al equipo con OPNsense.  
   - Comprobar conectividad y que los servicios internos (8545, 3001, 8590) sigan accesibles desde LAN.

---

## 6. Fase 2 — Servidores Linux y stack IERAHKWA

### 6.1 Objetivo

Que todos los **ProLiant EC200a/EC200p** (y cualquier servidor físico) corran **Linux** y el stack propio (Node, Banking Bridge, Rust SWIFT, etc.), sin depender de Windows ni VMware.

### 6.2 Pasos

1. **Instalar sistema operativo**  
   - Debian 12, Ubuntu 22.04 LTS o AlmaLinux 9.  
   - Instalación mínima (sin escritorio); habilitar SSH.

2. **Ejecutar el script de instalación del repo**  
   Desde la raíz del proyecto (con el código clonado o copiado en el servidor):

   ```bash
   cd /ruta/al/repo
   chmod +x scripts/instalar-stack-soberano-servidor.sh
   ./scripts/instalar-stack-soberano-servidor.sh
   ```

   El script instala Node 18+, PM2 y dependencias de `RuddieSolution/node`; opcionalmente indica cómo cargar el firewall nftables.

3. **Asignar servicios por host** (según tu inventario)  
   - **Host 1:** Node (8545) + Banking Bridge (3001). Ejemplo con PM2:
     ```bash
     cd RuddieSolution/node
     pm2 start server.js --name mamey
     pm2 start banking-bridge.js --name bridge
     pm2 save && pm2 startup
     ```
   - **Host 2:** Servicio Rust SWIFT (8590). Compilar y ejecutar desde `RuddieSolution/services/rust/` (o desplegar binario).
   - **Hosts 3+:** BDET server (4001), TradeX (4002), SIIS (4003), etc., según `RuddieSolution/config/services-ports.json`.

4. **Firewall en cada servidor (opcional)**  
   - Ajustar IPs en **`sovereign-network/linux-nftables/firewall-base.nft`** y cargar:
     ```bash
     sudo nft -f sovereign-network/linux-nftables/firewall-base.nft
     ```
   - Asegurar que 8545, 3001, 8590 estén permitidos desde tu red interna y que 8590 no esté expuesto a internet.

---

## 7. Fase 3 — Routing (sustituir Cisco IOS)

### 7.1 Objetivo

Usar **VyOS** (o Linux + FRR) para routing y NAT en lugar de Cisco IOS. Los switches Cisco pueden quedar solo como L2 (VLANs).

### 7.2 Pasos

1. **Descargar VyOS**  
   - https://vyos.io/  
   - Imagen ISO o cloud para x86/VM.

2. **Instalar VyOS** en un equipo o VM (puede ser un ProLiant con dos NICs).

3. **Cargar configuración de ejemplo**  
   - Editar **`sovereign-network/vyos/static-routing.txt`** con tus IPs y gateway WAN.  
   - En VyOS (modo configure):
     - Pegar las líneas o usar `load` si exportaste un archivo.
     - `commit` y `save`.

4. **Conmutar**  
   - Poner el equipo VyOS entre internet y la LAN; quitar el Cisco de la ruta o usarlo solo como switch.

Documentación VyOS: https://docs.vyos.io/

---

## 8. Fase 4 — Almacenamiento

### 8.1 Si mantienes el enclosure 5 bays

- Comprobar estado del RAID (utilidad del fabricante o por navegador si tiene IP).  
- Si hay "ERROR REBUILD": backup, reemplazar disco fallido, reconstruir array.

### 8.2 Opción soberana (sin firmware propietario)

- Conectar discos a un servidor Linux.  
- Usar **ZFS** (OpenZFS) o **mdadm + LVM** para RAID y volúmenes.  
- Backups a otro nodo o medio bajo tu control (script: **`scripts/backup-soberano.sh`**).

---

## 9. Servicios de aplicación (email, storage, AI, SMS)

Todo esto ya está documentado en **`docs/SERVICIOS-SOBERANOS.md`**. Resumen:

| Servicio | Cómo activarlo | Reemplaza |
|----------|----------------|-----------|
| **Email** | Cola en `data/email-queue/`; procesar con `node RuddieSolution/node/scripts/procesar-cola-email.js` (cron cada 5 min) | SendGrid |
| **Storage** | Archivos en `data/storage-soberano/` | AWS S3 |
| **AI** | Ollama en `http://localhost:11434`; `./scripts/instalar-ollama.sh` y `ollama run llama2` | OpenAI, Anthropic |
| **SMS** | Cola; procesar con `node RuddieSolution/node/scripts/procesar-cola-sms.js` (Telecom API) | Twilio |
| **Pagos** | Módulo soberano (IGT, intención de pago) | Stripe |

Activar modo soberano en Node:

```bash
USE_SOBERANO=true node server.js
```

Colas unificadas:

```bash
./scripts/procesar-colas.sh
```

Estado y auditoría:

```bash
./scripts/status-soberano.sh
node scripts/auditar-soberania.js
```

---

## 10. Red y SWIFT propio (Cisco + Rust)

- **Cisco** (switches/routers): solo red y segmentación; no ejecuta lógica SWIFT.  
- **SWIFT MT/MX:** lo hace nuestro código en **RuddieSolution/services/rust/** (puerto **8590**).  
- Reglas de firewall: puerto 8590 solo desde Node (8545) y Banking Bridge (3001); nunca expuesto a internet.

Detalle: **`docs/CISCO-SWIFT-SOBERANO.md`**.

---

## 11. Checklist y referencias

### Checklist de migración

- [ ] Fase 1: OPNsense instalado; reglas WAN/LAN/SWIFT/BANCA/PLATAFORMA; VPN si aplica; tráfico conmutado desde FortiGate.
- [ ] Fase 2: Todos los ProLiant con Linux; script `instalar-stack-soberano-servidor.sh` ejecutado; Node (8545), Bridge (3001), Rust (8590) y demás servicios asignados; nftables opcional.
- [ ] Fase 3: VyOS (o Linux+FRR) en producción; rutas y NAT migrados desde Cisco; Cisco solo L2 si se mantiene.
- [ ] Fase 4: Almacenamiento revisado (ERROR REBUILD resuelto); backups y, si aplica, ZFS/mdadm en Linux.
- [ ] Servicios aplicación: `USE_SOBERANO=true`; colas email/SMS en cron; Ollama instalado; storage y pagos soberanos en uso.
- [ ] Documentación: inventario actualizado con IPs y qué corre en cada host.

### Documentos del repo

| Documento | Contenido |
|-----------|-----------|
| **docs/GUIA-COMPLETA-SOBERANA.md** | Esta guía (todo en uno) |
| **docs/SOBERANO-SOFTWARE-RED-Y-SEGURIDAD.md** | Estrategia software soberano (red y seguridad) |
| **docs/INVENTARIO-RACK-CISCO-HPE-FORTINET.md** | Inventario físico (FortiGate 90D/100D, Cisco, HPE, enclosure) |
| **docs/CISCO-SWIFT-SOBERANO.md** | Rol de Cisco y stack SWIFT propio (Rust) |
| **docs/SERVICIOS-SOBERANOS.md** | Email, storage, AI, SMS, pagos; red y seguridad (tabla resumen) |

### Carpetas y scripts

| Ruta | Contenido |
|------|-----------|
| **sovereign-network/opnsense/** | Reglas firewall, ejemplo WireGuard |
| **sovereign-network/vyos/** | Rutas estáticas, NAT (VyOS) |
| **sovereign-network/linux-nftables/** | Firewall nftables para servidor Linux |
| **scripts/instalar-stack-soberano-servidor.sh** | Node, PM2, dependencias, referencia nftables |
| **scripts/instalar-ollama.sh** | AI local (Ollama) |
| **scripts/procesar-colas.sh** | Colas email + SMS |
| **scripts/backup-soberano.sh** | Backups propios |
| **scripts/status-soberano.sh** | Estado servicios soberanos |

### Puertos principales (recordatorio)

| Puerto | Servicio |
|-------|----------|
| 8545 | Node (Mamey) — plataforma |
| 3001 | Banking Bridge |
| 8590 | Rust SWIFT (MT/MX) — solo red interna |
| 4001–4600 | BDET, TradeX, SIIS, Clearing, bancos centrales, AI Hub, Gov Portal |

---

*Sovereign Government of Ierahkwa Ne Kanienke — Guía completa de infraestructura y servicios soberanos.*
