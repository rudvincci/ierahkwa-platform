# 🌺 Cómo poner a todos a trabajar en conjunto — Quantum, Atabey, Mamey

**Objetivo:** Que Quantum, ATABEY, Mamey Node, Fortress, Telecom y todos los sistemas operen **juntos**, no separados.

---

## 1. Entrada única: **ATABEY**

| URL | Qué hace |
|-----|----------|
| `/platform/atabey-platform.html` | **Centro de mando único.** Una sola pantalla con pestañas: Vista Global, AI, Fortress, Quantum, Telecom, Vigilancia, Chat·Video, etc. |

**Desde ATABEY se llega a todo.** Es el “Jarvis” de la plataforma — todo integrado.

---

## 2. Comando Conjunto: Fortress + AI + Quantum

| URL | Qué hace |
|-----|----------|
| `/platform/comando-conjunto-fortress-ai-quantum.html` | Estado conjunto de **Security Fortress + AI (ATABEY) + Quantum**. Una sola vista que muestra si los tres están operativos. |

Cada 15 segundos llama a `GET /api/v1/security/conjunto` y muestra:
- Fortress (Ghost Mode, Platform)
- AI · ATABEY (workers, tareas)
- Quantum (Kyber, Dilithium, post-cuántica)

---

## 3. API que agrega todo: `/api/v1/atabey/status`

Un solo endpoint que junta el estado de:

| Sistema | Cómo se obtiene |
|---------|-----------------|
| Fortress | `/api/v1/security/conjunto` → fortress |
| Quantum | `/api/v1/security/conjunto` → quantum |
| AI (ATABEY) | `/api/v1/security/conjunto` → ai |
| Telecom | `/api/v1/telecom/status` |
| BDET | `/api/ai-hub/bdet/status` |
| Vigilancia | `/api/v1/security/vigilance` |
| Face | `/api/v1/face/status` |
| Watchlist | `/api/v1/watchlist` |
| Emergencias | `/api/v1/emergencies/alerts` |
| Backup | `/api/v1/backup/stats` |
| Nodos | `/api/v1/security/nodes` |

**Uso:** Health checks, scripts de alerta, dashboards.  
Ejemplo cron: `*/5 * * * * curl -s https://app.ierahkwa.gov/api/v1/atabey/status | jq .overall`

---

## 4. Cómo prender todo junto

### Opción A: PM2 (recomendado)

```bash
cd RuddieSolution/node
pm2 start ecosystem.config.js
```

Eso levanta Node (8545), Banking Bridge (3001), Editor API (3002).  
En el **Node 8545** viven ATABEY, Quantum, Fortress, AI Hub, BDET, etc.

### Opción B: Script start-all

```bash
./start-all.sh
# o
./start.sh
```

---

## 5. Mamey Node — El núcleo

Todo pasa por **Mamey Node (puerto 8545)**:

- ATABEY (AI Hub)
- Quantum (cripto post-cuántica)
- Security Fortress (APIs)
- BDET, SIIS, Clearing
- 70+ plataformas y APIs

Si el Node 8545 está arriba, **todos trabajan en conjunto** porque las APIs que usa ATABEY y Comando Conjunto están en el mismo servidor.

---

## 6. Flujo resumido

```
┌─────────────────────────────────────────────────────────────────┐
│  TÚ (Líder)                                                     │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│  ATABEY Platform  OR  Comando Conjunto                          │
│  /platform/atabey-platform.html  |  /platform/comando-conjunto-  │
│  fortress-ai-quantum.html                                        │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│  /api/v1/atabey/status  o  /api/v1/security/conjunto            │
│  (una sola llamada → estado de todo)                            │
└─────────────────────────────────────────────────────────────────┘
                              │
         ┌────────────────────┼────────────────────┐
         ▼                    ▼                    ▼
┌─────────────┐      ┌─────────────┐      ┌─────────────┐
│  Fortress   │      │  Quantum    │      │  AI/ATABEY   │
│  Ghost, WAF │      │  Kyber, etc │      │  Workers    │
└─────────────┘      └─────────────┘      └─────────────┘
         │                    │                    │
         └────────────────────┼────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│  Mamey Node :8545 — Un solo servidor, todo integrado             │
└─────────────────────────────────────────────────────────────────┘
```

---

## 7. Enlaces directos

| Para ver… | Ir a |
|-----------|------|
| Todo junto (centro de mando) | [ATABEY Platform](/platform/atabey-platform.html) |
| Estado Fortress + AI + Quantum | [Comando Conjunto](/platform/comando-conjunto-fortress-ai-quantum.html) |
| Solo Quantum | [Quantum Platform](/platform/quantum-platform.html) |
| Solo Security Fortress | [Security Fortress](/platform/security-fortress.html) |

---

## 8. Checklist: ¿Están trabajando en conjunto?

1. **Node 8545 arriba:** `curl http://localhost:8545/health`
2. **ATABEY status OK:** `curl http://localhost:8545/api/v1/atabey/status` → `overall: "SECURE"` o `"PARTIAL"`
3. **Conjunto OK:** `curl http://localhost:8545/api/v1/security/conjunto` → fortress, ai, quantum con `ok: true`

Si los tres puntos pasan, **Quantum, Atabey, Mamey y el resto trabajan en conjunto**.
