# Poner servidores a trabajar y conectar la AI plataforma
## Pasos concretos – servidores + AI conectados

**Objetivo:** Servidores corriendo + plataforma AI conectada y funcionando.

---

## 1. Cómo está conectado (dibujo)

```
  ┌─────────────────────────────────────────────────────────────────┐
  │  SERVIDOR 1 (puerto 8545)  →  server.js                         │
  │  ─────────────────────────────────────────────────────────────  │
  │  • AI Hub        →  /api/ai-hub     (ATABEY, workers, registry) │
  │  • AI Orchestrator → /api/v1/ai/orchestrator                     │
  │  • AI Banker     →  /api/v1/ai/banker/*                          │
  │  • AI Code       →  /api/ai/code, /api/ai/chat                   │
  │  • Dashboards    →  /ai-hub   /atabey                            │
  └───────────────────────────┬─────────────────────────────────────┘
                               │  proxy (cuando aplica)
                               ▼
  ┌─────────────────────────────────────────────────────────────────┐
  │  SERVIDOR 2 (puerto 3001)  →  banking-bridge.js                 │
  │  ─────────────────────────────────────────────────────────────  │
  │  • Bankers (chat) → /api/bankers, /api/chat                      │
  │  • Banco, cuentas, préstamos, etc.                               │
  └─────────────────────────────────────────────────────────────────┘
```

**En tu Mac (todo en la misma máquina):** Los dos “servidores” son dos procesos: `server.js` (8545) y `banking-bridge.js` (3001). La AI está dentro de server.js (8545).

---

## 2. Pasos para poner los servidores a trabajar

### Opción A: Un solo comando (recomendado)

En Terminal, desde la carpeta del proyecto:

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives"
./start.sh
```

Eso levanta:
- **Node (8545)** → server.js → incluye AI Hub, AI Orchestrator, AI Banker, AI Code
- **Banking Bridge (3001)** → banking-bridge.js → banco, chat, bankers

### Opción B: Manual (dos terminales)

**Terminal 1 – Servidor principal (con la AI):**
```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives/RuddieSolution/node"
node server.js
```
Dejar corriendo. Debe salir: `✓ AI Hub loaded`, `✓ AI Banker started`, `✓ AI Orchestrator`, etc.

**Terminal 2 – Banking Bridge:**
```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives/RuddieSolution/node"
node banking-bridge.js
```
Dejar corriendo.

### Opción C: Con PM2 (para que siga tras cerrar Terminal)

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives/RuddieSolution/node"
pm2 start ecosystem.config.js
```

---

## 3. Cómo verificar que la AI plataforma está conectada

Abrir en el navegador (o con `curl` en Terminal):

| Qué | URL | Qué debe pasar |
|-----|-----|----------------|
| Health del servidor | http://localhost:8545/health | JSON con `ok: true` |
| AI Hub | http://localhost:8545/api/ai-hub/status | Respuesta del AI Hub |
| AI Orchestrator | http://localhost:8545/api/v1/ai/orchestrator | Estado del orquestador |
| AI Banker | http://localhost:8545/api/v1/ai/banker/status | Estado del AI Banker (si existe el endpoint) |
| Dashboard AI Hub | http://localhost:8545/ai-hub | Página del dashboard AI |
| Dashboard ATABEY | http://localhost:8545/atabey | Página del dashboard ATABEY |
| Banking Bridge | http://localhost:3001/api/health | Health del bridge |
| Bankers (chat) | http://localhost:3001/api/bankers | Lista de 7 bankers |

Si esas URLs responden, **los servidores están a trabajar y la AI plataforma está conectada**.

---

## 4. Si algo no arranca

| Problema | Qué hacer |
|----------|-----------|
| `node: command not found` | Instalar Node (mejor Node 18+). |
| Puerto 8545 en uso | Cerrar el proceso que lo usa o cambiar `PORT` en `.env`. |
| Puerto 3001 en uso | Cerrar el proceso que lo usa o cambiar `BRIDGE_PORT` en `.env`. |
| AI Hub no carga | Revisar que existan `RuddieSolution/node/ai-hub/` y `RuddieSolution/node/data/ai-hub/`. |
| OpenAI / logger fallan | Proyecto pide Node 18+; con Node 14 fallan. Actualizar Node. |

---

## 5. Resumen en 3 pasos

1. **Arrancar:** `./start.sh` (o manual o PM2).
2. **Comprobar:** http://localhost:8545/health y http://localhost:8545/ai-hub (y opcionalmente /atabey, /api/ai-hub/status).
3. **Usar:** Dashboards en /ai-hub y /atabey; APIs en /api/ai-hub, /api/v1/ai/orchestrator, /api/v1/ai/banker; banco y chat en http://localhost:3001.

Cuando eso funcione en tu Mac, el siguiente paso es hacer lo mismo en los servidores de los racks (copiar el repo, Node 18+, mismo `./start.sh` o PM2).
