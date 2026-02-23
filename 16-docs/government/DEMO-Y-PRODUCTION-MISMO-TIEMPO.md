# Demo y Production al mismo tiempo

**Sovereign Government of Ierahkwa Ne Kanienke**  
Tener dos entornos en el mismo equipo: **Production** (live real) y **Demo** (para mostrar sin tocar producción).

---

## Opción A — Misma carpeta (Production + Demo en paralelo)

En la **misma** carpeta del proyecto:

1. **Prender Production (todo):**  
   `./start.sh`  
   → Node 8545, Bridge 3001, Platform 8080, servidores 4001–4600, jerarquía 6000–6400.

2. **Prender Demo (solo Node, Bridge y Platform frontend en otros puertos):**  
   `./start-demo.sh`  
   → Node **9545**, Bridge **3003**, Platform **9080**.  
   Los servidores 4001–4600 y 6000–6400 se **comparten** con Production (un solo juego).

3. **URLs:**
   - **Production:** http://localhost:8545/platform/ | Bridge http://localhost:3001
   - **Demo:** http://localhost:9545/platform/ | Bridge http://localhost:3003 | Frontend http://localhost:9080

4. **Parar solo Demo:**  
   `./stop-demo.sh`  
   (Production sigue corriendo.)

5. **Parar todo:**  
   `./stop-all.sh`

---

## Opción B — Duplicación para guardar (dos carpetas)

Tener **dos carpetas**: una para Production y otra para Demo (por ejemplo para backups o para ejecutar en otra máquina).

1. **Crear la copia:**  
   `./scripts/crear-duplicacion-demo.sh [ruta_destino]`  
   Por defecto crea `../soberanos-natives-demo`.

2. **En la copia:**  
   - Instalar dependencias si hace falta: `cd RuddieSolution/node && npm install`
   - Configurar `.env` si quieres (puedes copiar desde `.env.demo.example`).
   - Arrancar **solo** Demo en esa carpeta: `./start-demo.sh`  
     → En esa carpeta Demo usa 9545, 3003, 9080.

3. **Mismo tiempo en un solo equipo:**  
   - En la **carpeta original**: `./start.sh` (Production completo).  
   - En la **copia**: `./start-demo.sh` (Demo).  
   Así tienes Production y Demo a la vez; la copia comparte los mismos servidores 4001/6000 si están levantados en la carpeta original.

---

## Auditoría: prender todo y verificar LIVE 100% Production

Para **prender todos los servidores** y comprobar que todo está en producción correctamente:

```bash
./scripts/auditoria-prende-todo-y-verifica-live.sh
```

- Ejecuta `./start.sh`, espera, luego pre-live-check, verificación de servicios críticos y todo-funcionando-production.
- Si todo pasa, imprime **LIVE 100% PRODUCTION** y sale con 0.
- Si no quieres que arranque nada (solo verificar):  
  `AUDITORIA_SKIP_START=1 ./scripts/auditoria-prende-todo-y-verifica-live.sh`

---

## Resumen de scripts

| Script | Qué hace |
|--------|----------|
| `./start.sh` | Production completo (8545, 3001, 8080, 4001…, 6000…) |
| `./start-demo.sh` | Solo Demo: Node 9545, Bridge 3003, Platform 9080 (comparte el resto) |
| `./stop-demo.sh` | Para solo los procesos Demo (9545, 3003, 9080) |
| `./stop-all.sh` | Para todo (Production + Demo si estaban en la misma carpeta) |
| `./scripts/auditoria-prende-todo-y-verifica-live.sh` | Prende todo y verifica LIVE 100% production |
| `./scripts/crear-duplicacion-demo.sh [destino]` | Copia el proyecto a otra carpeta para usar como Demo |

Con esto puedes tener **demos y production al mismo tiempo** y **guardar una duplicación** para el demo.
