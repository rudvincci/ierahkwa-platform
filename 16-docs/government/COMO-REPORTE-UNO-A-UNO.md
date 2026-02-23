# Cómo hacer el reporte uno a uno (todos los servidores)

Dos formas de generar el reporte consolidado en un solo documento.

---

## 1. Prender uno → reporte → apagar → siguiente (TODOS los servidores)

**Script:** `scripts/prender-uno-reporte-apagar-siguiente.js`

- **Lista de servidores:** `scripts/servicios-prender-apagar.json` (todos: Node, .NET, stubs, bases solo chequeo).
- Para **cada** servidor: libera el puerto → prende el proceso → espera health → hace el reporte → apaga el proceso → pausa 2 s → siguiente.
- Al final escribe el mismo documento único: `docs/REPORTE-SERVICIOS-UNO-A-UNO.md` y `RuddieSolution/node/data/reporte-servicios-uno-a-uno.json`.

**Una sola corrida (todos, uno por uno):**
```bash
node scripts/prender-uno-reporte-apagar-siguiente.js
```

**Repetir cada 30 minutos para todos los servidores:**
```bash
node scripts/prender-uno-reporte-apagar-siguiente.js --cada 30
```
Cada 30 min vuelve a hacer el ciclo completo (prender uno, reporte, apagar, siguiente) y actualiza el reporte. Ctrl+C para parar.

---

## 2. Solo probar (sin prender/apagar)

**Script:** `node scripts/test-servicios-uno-a-uno.js`

- Asume que los servicios ya están corriendo (por ejemplo con `./start.sh`).
- Prueba cada puerto uno a uno y escribe el mismo documento único.

```bash
./start.sh
node scripts/test-servicios-uno-a-uno.js
# Leer: docs/REPORTE-SERVICIOS-UNO-A-UNO.md
```

---

## Resumen

| Objetivo | Comando |
|----------|--------|
| Prender uno, reporte, apagar, siguiente (una vez) | `node scripts/prender-uno-reporte-apagar-siguiente.js` |
| Lo mismo, repetido cada 30 min | `node scripts/prender-uno-reporte-apagar-siguiente.js --cada 30` |
| Solo probar lo que ya está prendido | `node scripts/test-servicios-uno-a-uno.js` |

**Documento único (ambos scripts):** `docs/REPORTE-SERVICIOS-UNO-A-UNO.md`
