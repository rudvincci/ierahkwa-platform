# Diagnóstico: Agentes / AI “andan pendejos” — 28 Ene 2026

## Resumen

Se revisó el sistema de **agentes** (bankers del chat, AI Orchestrator, AI Banker, OpenAI). Hay **dos causas principales** por las que todo puede verse “pendejo” o limitado.

---

## 1. Node 14 en lugar de Node 18+

**Situación:** El servidor está corriendo con **Node v14.19.3**. El proyecto exige **Node >= 18** (`package.json` → `"engines": { "node": ">=18.0.0" }`).

**Consecuencias:**

| Componente | Efecto |
|------------|--------|
| **OpenAI** | No carga. Error: `No such built-in module: node:stream/web`. El SDK de OpenAI usa ese módulo (Node 18+). |
| **AI Code Generator / Chat con IA** | Sin OpenAI, las respuestas usan **plantillas genéricas** (`generateCodeFromTemplate`), no GPT. Por eso las respuestas parecen “pendejas” o repetitivas. |
| **Logger centralizado** | No carga. Error: `Unexpected token '||='`. El operador `||=` existe desde Node 15; alguna dependencia (p. ej. winston) lo usa y Node 14 no lo entiende. |

**Solución:** Usar **Node 18 o superior** en el entorno donde corre el servidor.

```bash
# Si usas nvm:
nvm install 18
nvm use 18

# Verificar:
node -v   # debe ser v18.x o v20.x
```

Luego reiniciar el servidor Node. Con Node 18+ deberían cargar OpenAI y el logger; las respuestas de IA dejarán de ser solo plantillas.

---

## 2. Solo 4 de 7 bankers en línea (ya corregido)

**Situación:** En `banking-bridge.js`, al iniciar solo los primeros 4 bankers quedaban con estado `ONLINE`; los otros 3 quedaban `AWAY`. Eso reducía la disponibilidad para chat y video.

**Cambio aplicado:** Los **7 bankers** quedan ahora en `ONLINE` al iniciar. Hay más agentes disponibles y menos cola.

---

## Verificaciones rápidas

1. **Versión de Node**  
   `node -v` → debe ser 18.x o superior.

2. **Bankers disponibles**  
   `GET /api/bankers` → deberías ver 7 bankers, todos con `status: "ONLINE"` (tras reiniciar el bridge).

3. **OpenAI**  
   Tras pasar a Node 18+ y reiniciar, en logs ya no debería aparecer `OpenAI not available` por el error de `node:stream/web`.

4. **Logger**  
   Con Node 18+, al iniciar debería aparecer `✓ Centralized Logging loaded` en lugar de `⚠️ Centralized Logging not loaded`.

---

## Archivos tocados

- `RuddieSolution/node/banking-bridge.js`: inicialización de bankers para que los 7 queden `ONLINE`.

Si después de usar Node 18+ y reiniciar algo sigue fallando (p. ej. un tipo concreto de agente o endpoint), indica cuál y lo revisamos.
