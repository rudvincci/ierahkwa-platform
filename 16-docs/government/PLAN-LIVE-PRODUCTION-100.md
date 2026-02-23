# Plan: Live Production 100% – Empezar por internet
## Conectar todo y tener el sistema corriendo al 100%

**Fecha:** 28 de Enero de 2026  
**Gobierno Soberano de Ierahkwa Ne Kanienke**

---

## Resumen

Tenemos entre todo: 5 racks (ProLiant, HP G4, Cisco, Fortinet, FUZE), planos del rack de inicio, código (Node 8545, Banking Bridge 3001, 60 plataformas), backup de agentes. Para ver **todo corriendo en live production 100%** empezamos por **conectar internet** y luego encender servicios y verificar.

---

## Fase 1: Conectar internet (primero)

### 1.1 Modem / WAN

| Paso | Acción |
|------|--------|
| 1 | Encender modem ISP (ej. Sagemcom 192.168.0.1). Ver que tengas internet en un PC conectado por cable o WiFi. |
| 2 | Si usas modo bridge: conectar salida del modem al **Cisco** (WAN). Si no: dejar modem en router y conectar LAN del modem al Cisco. |

### 1.2 Cisco (router principal)

| Paso | Acción |
|------|--------|
| 1 | Encender Cisco (el del rack – router/switch). |
| 2 | Conectar **WAN** del Cisco al modem (o al switch que sale del modem). |
| 3 | Conectar **LAN** del Cisco a los switches del rack (donde están los servidores). |
| 4 | Asignar IP al servidor donde corre Node (ej. 10.0.10.1 para SRV01). Ver `INFRASTRUCTURE-SETUP.md` (Cisco ISR4331, VLANs). |

### 1.3 Fortinet (firewall, si está en el rack)

| Paso | Acción |
|------|--------|
| 1 | Encender Fortinet. |
| 2 | WAN del Fortinet → Cisco (o modem). LAN → switches/servidores. |
| 3 | Reglas: permitir 80, 443, 8545, 3001 hacia la IP del servidor Node (10.0.10.1 o la que uses). |

### 1.4 Port forwarding (para que internet llegue a tu plataforma)

En el **dispositivo que hace NAT** (modem o Cisco):

| Puerto | Redirigir a | Servicio |
|--------|-------------|----------|
| 80 | 10.0.10.1 (o IP del servidor Node) | HTTP |
| 443 | 10.0.10.1 | HTTPS |
| 8545 | 10.0.10.1 | Mamey Node (API, dashboard) |
| 3001 | 10.0.10.1 (o IP del servidor Bridge) | Banking Bridge |

Así “conectamos internet” hasta los servidores donde corre la plataforma.

### 1.5 Verificar conectividad

```bash
# Desde el servidor donde corre Node (o desde un PC en la misma red)
ping -c 2 8.8.8.8
curl -sI https://google.com | head -1
```

Si hay respuesta, internet está llegando al equipo.

---

## Fase 2: Node 18+ (para que OpenAI y logger funcionen)

| Paso | Acción |
|------|--------|
| 1 | En el servidor donde corres Node: `node -v`. Si sale 14.x, instalar Node 18 LTS. |
| 2 | Con nvm: `nvm install 18 && nvm use 18`. Sin nvm: descargar desde nodejs.org e instalar. |
| 3 | Verificar: `node -v` → v18.x o v20.x. Reiniciar servicios después. |

Así dejan de fallar OpenAI (`node:stream/web`) y el logger centralizado (`||=`).

---

## Fase 3: Arrancar servicios (live production)

### Opción A: En el mismo Mac/PC donde está el repo

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives"
./start.sh
```

Eso intenta levantar Node en **8545** y Banking Bridge en **3001**.

### Opción B: En el servidor de los racks (SRV01 10.0.10.1)

1. Copiar a SRV01 (o al servidor que sea):
   - `RuddieSolution/node/` (server.js, banking-bridge.js, ai/, data/, etc.)
   - O clonar/rsync el repo.
2. En SRV01 instalar Node 18+, luego:

```bash
cd /ruta/a/node
npm install --production
node server.js &           # puerto 8545
node banking-bridge.js &   # puerto 3001
# o con PM2:
pm2 start ecosystem.config.js
```

3. Asegurar que el firewall del servidor permita 8545 y 3001 (y 80/443 si sirves web ahí).

---

## Fase 4: Verificar que todo esté al 100%

| Prueba | Comando / URL |
|--------|----------------|
| Health Node | `curl -s http://localhost:8545/health` (o http://10.0.10.1:8545/health) |
| Health Bridge | `curl -s http://localhost:3001/api/health` |
| Bankers | `curl -s http://localhost:3001/api/bankers` → deben salir 7 bankers |
| AI status | `curl -s http://localhost:8545/api/v1/ai/complete-status` (si existe el endpoint) |
| Desde internet | Si port forwarding está bien: `http://TU_IP_PUBLICA:8545/health` |

Si todo responde OK, tienes **live production al 100%** a nivel de servicios básicos.

---

## Fase 5: Mantenerlo estable (recomendado)

| Acción | Cómo |
|--------|------|
| Backup de agentes | Ejecutar `./scripts/backup-agentes.sh` periódicamente (cron o manual). |
| Persistencia | Ciudadanos/bankers/chats están en memoria; al reiniciar se pierden. Para 100% durable: añadir persistencia en archivo o BD (más adelante). |
| PM2 al arranque | En el servidor: `pm2 startup` y `pm2 save` para que Node y Bridge se levanten tras un reinicio. |

---

## Orden recomendado (resumen)

1. **Conectar internet:** modem → Cisco → Fortinet (si aplica) → switches → servidores. Port forwarding 80, 443, 8545, 3001.
2. **Node 18+** en el equipo donde corre Node.
3. **Arrancar:** `./start.sh` (local) o en SRV01 con `node server.js` + `node banking-bridge.js` (o PM2).
4. **Verificar:** health, bankers, APIs.
5. **Mantener:** backup de agentes, PM2 startup, y luego persistencia si quieres 100% durable.

Documentación de referencia: `INFRASTRUCTURE-SETUP.md`, `docs/RACKS-INVENTARIO-OFICIAL-2026-01-28.md`, `docs/PLANO-RACK-INICIO-LIVE-PRODUCTION.md`, `docs/HARDWARE-BLOCKCHAIN-GAS-CONFIRMACIONES.md`.

---

*Plan vivo. Actualizar cuando se complete cada fase.*
