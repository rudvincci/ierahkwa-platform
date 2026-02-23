# Instalar todo en los servidores físicos
## Plataforma, seguridad, VPN, banco, ATABEY – que trabaje en los racks

**Objetivo:** Que todo (plataforma, seguridad, VPN, banco, AI, ATABEY) trabaje en los servidores físicos (ProLiant, HP G4 en los racks).

---

## 1. Qué va en cada servidor físico

```
  SERVIDOR FÍSICO 1 (ej. 10.0.10.1 – SRV01)
  ─────────────────────────────────────────────
  • server.js (puerto 8545)
    → Plataforma completa
    → ATABEY + AI Hub + AI Orchestrator + AI Banker
    → Defensa / seguridad   → /api/v1/defense   /security
    → VPN                   → /api/v1/vpn   /api/v1/vpn/multihop
    → Government Banking    → /api/v1/bdet
    → Check Deposit         → /api/v1/bank/checks
    → Todo el resto (módulos, dashboards)

  SERVIDOR FÍSICO 2 (ej. 10.0.10.2 – o el mismo si quieres todo en uno)
  ─────────────────────────────────────────────
  • banking-bridge.js (puerto 3001)
    → Banco (cuentas, préstamos, chat, bankers, API bancaria)
```

**Opción simple:** Un solo servidor físico corre los dos procesos (server.js + banking-bridge.js). Así todo trabaja en el servidor físico.

---

## 2. Qué copiar a los servidores físicos

Desde tu Mac (o desde donde tengas el repo):

| Qué copiar | A dónde en el servidor |
|------------|------------------------|
| **RuddieSolution/node/** (todo menos node_modules) | /opt/ierahkwa/node/ (o la ruta que elijas) |
| **RuddieSolution/platform/** (HTML, dashboards) | /opt/ierahkwa/platform/ |
| **tokens/** (si lo usas) | /opt/ierahkwa/tokens/ |

En el servidor físico, después de copiar:

```bash
cd /opt/ierahkwa/node
npm install --production
```

Así tienes dependencias y todo el código (plataforma, seguridad, VPN, banco) en el servidor físico.

---

## 3. Requisitos en el servidor físico

| Requisito | Cómo |
|-----------|------|
| **Node 18+** | `node -v`. Si es 14, instalar Node 18 LTS (nvm o nodejs.org). |
| **Puertos 8545 y 3001 libres** | No tener otro proceso usando 8545 ni 3001. |
| **Red** | Servidor con IP fija (ej. 10.0.10.1) y conectado al Cisco (internet). |

---

## 4. Cómo arrancar todo en el servidor físico

### Opción A: PM2 (recomendado para que siga tras reinicio)

En el servidor físico:

```bash
cd /opt/ierahkwa/node
pm2 start ecosystem.config.js
pm2 save
pm2 startup   # para que arranque al encender el servidor
```

Eso levanta server.js (8545) y banking-bridge.js (3001). Todo (plataforma, seguridad, VPN, banco, ATABEY) trabaja en el servidor físico.

### Opción B: Sin PM2 (dos procesos en background)

```bash
cd /opt/ierahkwa/node
nohup node server.js > /var/log/ierahkwa/node.log 2>&1 &
nohup node banking-bridge.js > /var/log/ierahkwa/bridge.log 2>&1 &
```

### Opción C: systemd (servicios del sistema)

Crear dos servicios (ej. `ierahkwa-node.service` e `ierahkwa-bridge.service`) que ejecuten `node server.js` y `node banking-bridge.js` desde /opt/ierahkwa/node. Así el sistema operativo los mantiene corriendo.

---

## 5. Verificar que todo trabaja en el servidor físico

Desde tu Mac (o desde otro PC en la red), sustituye 10.0.10.1 por la IP de tu servidor:

| Qué | URL |
|-----|-----|
| Health | http://10.0.10.1:8545/health |
| ATABEY | http://10.0.10.1:8545/atabey |
| AI Hub | http://10.0.10.1:8545/ai-hub |
| Seguridad (Defense) | http://10.0.10.1:8545/api/v1/defense/status (o el endpoint que exponga el módulo) |
| VPN | http://10.0.10.1:8545/api/v1/vpn/status (o el endpoint que exponga el módulo) |
| Banco BDET | http://10.0.10.1:8545/api/v1/bdet/... (o el endpoint que exponga government-banking) |
| Banking Bridge | http://10.0.10.1:3001/api/health |
| Bankers | http://10.0.10.1:3001/api/bankers |

Si esas URLs responden, **todo está trabajando en los servidores físicos**.

---

## 6. Resumen en 3 pasos

1. **Copiar** a cada servidor físico: `node/` (sin node_modules) + `platform/`. En el servidor: `cd /opt/ierahkwa/node && npm install --production`.
2. **Arrancar** en el servidor: `pm2 start ecosystem.config.js` (o nohup o systemd).
3. **Comprobar** desde tu Mac: http://IP_DEL_SERVIDOR:8545/health y http://IP_DEL_SERVIDOR:8545/atabey (y el resto de URLs de arriba).

Con eso la plataforma, seguridad, VPN, banco y ATABEY **deben trabajar en los servidores físicos**.
