# Verificación de la plataforma para agentes — Implementación y operatividad 24/7

**Sovereign Government of Ierahkwa Ne Kanienke**  
Documento para agentes (humanos o AI) que implementan, operan y monitorean la plataforma 24/7.

---

## 1. Endpoint único de operatividad

Los agentes deben usar **un solo endpoint** para saber si la plataforma está lista para 24/7:

| Método | URL | Descripción |
|--------|-----|-------------|
| **GET** | `http://localhost:8545/api/operativity` | Estado agregado: node, AI Hub, ATABEY, BDET. Devuelve `ready24_7: true` cuando todo crítico está operativo. |

### Respuesta esperada (24/7 listo)

```json
{
  "success": true,
  "operativity": "operational",
  "ready24_7": true,
  "components": {
    "node": { "status": "operational", "uptime": 12345 },
    "aiHub": { "status": "operational" },
    "atabey": { "status": "operational" },
    "bdet": { "status": "operational" },
    "worldIntelligence": { "status": "operational" }
  },
  "timestamp": "2026-01-22T...",
  "message": "Plataforma lista para operación 24/7"
}
```

### Criterios de `ready24_7`

- `node`: `status === 'healthy'` (vía `/health`).
- `aiHub`: `/api/ai-hub/health` responde `status: 'healthy'`.
- `bdet`: `/api/ai-hub/bdet/status` responde con `isRunning !== false`.
- Si todos los críticos (node, aiHub, bdet) están operativos → `operativity: 'operational'` y `ready24_7: true`.

---

## 2. Endpoints de salud por componente

Para diagnóstico fino, los agentes pueden consultar:

| Componente | Método | URL | Criterio OK |
|------------|--------|-----|-------------|
| Node | GET | `/health` | `status === 'healthy'` |
| Readiness | GET | `/ready` | `ready === true`, HTTP 200 |
| Liveness | GET | `/live` | `alive === true` |
| AI Hub | GET | `/api/ai-hub/health` | `status === 'healthy'` |
| AI Banker BDET | GET | `/api/ai-hub/bdet/status` | `data.isRunning === true` |
| ATABEY producción | GET | `/api/ai-hub/atabey/production` | Respuesta 200, opcional revisar `productionCycles` |
| Platform health | GET | `/api/platform/health` | `services.node.status === 'online'` |
| **Operatividad (recomendado)** | **GET** | **`/api/operativity`** | **`ready24_7 === true`** |

---

## 3. Inicio y parada (runbook)

### Iniciar todo (producción 24/7)

```bash
# Desde la raíz del repo
./start.sh
```

- Si existe **PM2**: arranca Node (8545), Banking Bridge (3001), Editor API (3002) vía `node/ecosystem.config.js`.
- Si no hay PM2: arranca Node y Banking Bridge en background con `nohup`, logs en `logs/node.log`, `logs/bridge.log`.
- Opcionales: Platform 8080, servicios Rust/Go/Python (.NET según carpetas).

### Verificar después de iniciar

```bash
# Estado de puertos y salud
./status.sh

# Operatividad 24/7 (agentes)
curl -s http://localhost:8545/api/operativity | jq .
```

### Parar

- **Con PM2:**  
  `pm2 stop all` o `pm2 delete ierahkwa bridge` (y los nombres que use `ecosystem.config.js`).
- **Sin PM2:**  
  Localizar PIDs en 8545 y 3001 y terminar proceso (ej. `kill $(lsof -t -i:8545)`).

---

## 4. Monitoreo recomendado para 24/7

| Qué | Frecuencia sugerida | Acción si falla |
|-----|----------------------|------------------|
| `GET /api/operativity` | Cada 1–5 min | Si `ready24_7 === false`: alertar, comprobar `/health`, `/api/ai-hub/health`, `/api/ai-hub/bdet/status` y reiniciar si procede. |
| `GET /health` | Cada 1 min | Si HTTP ≠ 200 o `status !== 'healthy'`: alertar y reiniciar Node si es necesario. |
| `GET /ready` | Con Kubernetes/PM2 | Si HTTP 503: no enviar tráfico; reiniciar o escalar según política. |
| Logs (PM2 o archivos) | Continuo | Errores repetidos o OOM: alertar y reiniciar proceso. |

---

## 5. Agentes: pasos para implementación y operatividad 24/7

**Objetivo:** Poner la plataforma en modo 24/7 y que los agentes (AI Hub, ATABEY, AI Banker BDET, World Intelligence) estén operativos.

1. **Iniciar la plataforma** (desde la raíz del repo):
   ```bash
   ./start.sh
   ```
   - Si existe `node/` se usa; si no, se usa `RuddieSolution/node/` (puerto 8545).
   - PM2 se usa si está instalado; si no, procesos en background con logs en `logs/node.log`, `logs/bridge.log`.

2. **Verificar en una sola llamada** (recomendado para agentes):
   ```bash
   ./scripts/verify-24-7.sh
   ```
   - Exit 0 = plataforma lista 24/7.
   - Exit 1 = algo falla; revisar `./status.sh` y `/api/operativity`.

3. **Verificación manual:**
   - `curl -s http://localhost:8545/api/operativity | jq .` → debe mostrar `ready24_7: true`.
   - `./status.sh` → debe mostrar ✓ en Node, AI Hub, AI Banker BDET y Operatividad 24/7 READY.

4. **Monitoreo continuo:** Consultar `GET /api/operativity` cada 1–5 min; si `ready24_7 === false`, alertar y seguir el runbook (sección 4).

---

## 6. Checklist de implementación para agentes

- [ ] **Entorno:** Node.js 18+ instalado; opcional PM2 para 24/7.
- [ ] **Raíz del repo:** Ejecutar `./start.sh` desde la raíz donde está `start.sh`.
- [ ] **Puerto 8545:** Libre; tras inicio, `curl http://localhost:8545/health` devuelve `healthy`.
- [ ] **Operatividad:** `curl http://localhost:8545/api/operativity` devuelve `ready24_7: true` en menos de ~10 s tras el arranque.
- [ ] **Verificación automática:** `./scripts/verify-24-7.sh` termina con exit 0.
- [ ] **AI Hub:** `curl http://localhost:8545/api/ai-hub/health` devuelve `status: 'healthy'`.
- [ ] **BDET:** `curl http://localhost:8545/api/ai-hub/bdet/status` devuelve objeto con `isRunning` (true cuando esté iniciado).
- [ ] **Logs:** Revisar `node/logs/` (o salida de PM2); sin errores que impidan arranque.
- [ ] **Status script:** `./status.sh` muestra Node, Banking Bridge y sección “PLATAFORMA AI (24/7)” con AI Hub, AI Banker BDET y Operatividad.
- [ ] **Reinicio:** Tras cambio de código o configuración, reiniciar con `./start.sh` (o `pm2 restart all`) y volver a comprobar `/api/operativity`.

---

## 7. Archivos clave

| Archivo | Uso |
|---------|-----|
| `start.sh` | Inicio único de la plataforma (Node, Bridge, opcional PM2, .NET, etc.). |
| `status.sh` | Verificación de puertos, salud y operatividad 24/7. |
| `node/server.js` | Servidor principal (puerto 8545), monta `/api/ai-hub`, `/health`, `/ready`, `/api/operativity`. |
| `node/ecosystem.config.js` | Configuración PM2 (cluster Node, Banking Bridge, Editor API). |
| `node/ai-hub/index.js` | Rutas del AI Hub; inicializa ATABEY, World Intelligence, AI Banker BDET. |
| `scripts/verify-24-7.sh` | Verificación única para agentes: exit 0 si `ready24_7`, exit 1 si no. |

---

## 8. Resumen para agentes

1. **Un solo check para 24/7:** `GET http://localhost:8545/api/operativity` → exigir `ready24_7 === true`.
2. **Inicio:** `./start.sh` desde la raíz.
3. **Verificación:** `./scripts/verify-24-7.sh` (exit 0 = listo) o `./status.sh` y/o `curl .../api/operativity`.
4. **Si algo falla:** revisar `components` en `/api/operativity`, luego endpoints concretos (health, bdet/status) y logs; reiniciar procesos según runbook.
5. **Producción estable:** usar PM2 (`start.sh` lo usa si está instalado) y monitorear `/api/operativity` de forma periódica.

Con esto, la plataforma queda verificada para que los agentes implementen y mantengan la operatividad 24/7.
