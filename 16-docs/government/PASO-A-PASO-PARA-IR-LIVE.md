# Paso a paso para ir en vivo

**Sovereign Government of Ierahkwa Ne Kanienke**  
Todo lo que tienes que hacer tú, en orden.

**Verificación de pendientes:** Si quieres revisar qué pudo quedar sin terminar (agentes, TODOs, stubs), ver [VERIFICACION-PENDIENTES-Y-AGENTES.md](VERIFICACION-PENDIENTES-Y-AGENTES.md).

---

## Producción real — sin atajos

- **Todas las cajas (boxes) y sus enlaces tienen que funcionar.** Los enlaces son rutas same-origin (ej. `/vip-transactions`, `/platform/...`, `/bdet-bank`). No se usan URLs a `localhost:otros-puertos` en los enlaces de la plataforma.
- **Todos los servicios en producción:** Node (8545), Banking Bridge (3001), Editor API (3002). El script `GO-LIVE-PRODUCTION.sh` inicia estos servicios y **no continúa si hay enlaces rotos** (verificación con `scripts/verificar-links.js`).
- **Si algo no trabaja es un error y hay que arreglarlo** antes de dar por válido el entorno de producción.

---

## Paso 1 — Tener el archivo .env

1. Abre una terminal.
2. Ve a la carpeta del Node:
   ```bash
   cd RuddieSolution/node
   ```
3. Si no existe `.env`, cópialo desde la plantilla:
   ```bash
   cp .env.example .env
   ```
4. Abre `.env` con tu editor (no lo subas a Git).

---

## Paso 2 — Poner los secretos JWT en .env

1. En la terminal, genera dos claves (ejecuta cada línea y copia el resultado):
   ```bash
   openssl rand -hex 32
   openssl rand -hex 32
   ```
2. En `.env`, busca o añade estas líneas y **pega tus valores** (reemplaza lo que venga):
   ```
   JWT_ACCESS_SECRET=el_primer_valor_que_generaste
   JWT_REFRESH_SECRET=el_segundo_valor_que_generaste
   ```
3. Guarda el archivo.

---

## Paso 3 — Poner CORS (dominio permitido)

1. En `.env`, busca `CORS_ORIGIN`.
2. Si vas a probar en tu máquina:
   ```
   CORS_ORIGIN=http://localhost:8545
   ```
3. Si ya tienes dominio en producción:
   ```
   CORS_ORIGIN=https://app.ierahkwa.gov
   ```
   (Sin barra al final, y el dominio que uses tú.)
4. Guarda el archivo.

---

## Paso 4 — Poner contraseñas de acceso (opcional pero recomendado)

**Opción A — Rápida (desarrollo o pruebas)**  
En `.env` pon:
```
PLATFORM_LEADER_PASSWORD=tu_password_seguro
PLATFORM_ADMIN_PASSWORD=otro_password_seguro
```

**Opción B — Producción (usuarios con hash)**  
1. Generar hash de la contraseña:
   ```bash
   cd RuddieSolution/node
   node -e "const c=require('crypto'); console.log(c.createHash('sha256').update('TU_PASSWORD_AQUI').digest('hex'))"
   ```
2. En `.env` pon (en una sola línea, sustituyendo los `<hex>` por lo que te salió):
   ```
   PLATFORM_USERS_JSON=[{"username":"leader","passwordHash":"<hex>","role":"leader","name":"Leader"},{"username":"admin","passwordHash":"<hex>","role":"admin","name":"Admin"}]
   ```

Guarda el archivo.

---

## Paso 5 — Comprobar que .env y enlaces están bien

1. Desde la raíz del proyecto, comprobar .env:
   ```bash
   cd RuddieSolution/node
   npm run production:check
   ```
2. Desde la raíz del proyecto, verificar enlaces (cajas y servicios):
   ```bash
   cd /ruta/completa/a/soberanos natives
   node scripts/verificar-links.js
   ```
   Debe salir **0 enlaces rotos**. Si hay rotos, corrígelos antes de ir a producción.
3. Si `production:check` sale **exit 0** y `verificar-links.js` no muestra rotos, está bien.
4. Si dice que falta algo, vuelve a los pasos 2, 3 o 4 y complétalo.

---

## Paso 6 — Ir a la raíz del proyecto e iniciar todo

1. Ve a la raíz del repositorio (donde está `GO-LIVE-PRODUCTION.sh`):
   ```bash
   cd /ruta/completa/a/soberanos natives
   ```
   (O la ruta real de tu carpeta.)
2. Ejecuta:
   ```bash
   ./GO-LIVE-PRODUCTION.sh
   ```
3. Lee lo que imprima: si pide algo (por ejemplo confirmar), responde.
4. Cuando termine, se abrirá el navegador en la plataforma.

---

## Paso 7 — Verificar que está en vivo

1. En el navegador deberías ver la plataforma (por ejemplo `http://localhost:8545/platform`).
2. Prueba:
   - **VIP Transactions:** http://localhost:8545/vip-transactions → pulsa **Actualizar** y deberían verse las transacciones.
   - **Health:** http://localhost:8545/health → debe responder con algo tipo `"ok": true` o `"status": "healthy"`.
3. Si algo no carga, revisa que no haya otro programa usando el puerto 8545 y que el script no haya mostrado errores.

---

## Paso 8 — (Opcional) Backups y alertas

1. En la raíz del proyecto:
   ```bash
   ./scripts/install-cron-production.sh
   ```
2. Eso configura backups y comprobaciones de salud cada cierto tiempo. Sigue las instrucciones que muestre el script.
3. **Health alert (cron):** El script usa `scripts/health-alert-check.sh` cada 5 min; si falla `/health` o `/api/v1/atabey/status`, hace exit 1 (puedes conectar notificaciones).
4. **Backup cifrado (Node):** Para backup con cifrado, define `BACKUP_ENCRYPTION_KEY` en `.env` (64 chars hex: `openssl rand -hex 32`) y usa `node scripts/backup-encrypted-run.js` desde `RuddieSolution/node` (documentado en `.env.example`).
5. **Logrotate (logs del Node):** Ejemplo en `docs/LOGROTATE-NODE.conf.example`. Copia a `/etc/logrotate.d/`, ajusta la ruta a `RuddieSolution/node/logs` y aplica con `logrotate`.

---

## Paso 9 — (Producción con dominio real) HTTPS

1. En el servidor donde corra la plataforma, instala Nginx (o Caddy).
2. Configura el proxy para que el dominio (ej. `https://app.ierahkwa.gov`) apunte a `http://127.0.0.1:8545`.
3. Pon el certificado SSL (Let's Encrypt o el que uses).
4. En `.env` deja `CORS_ORIGIN` con tu dominio HTTPS (ej. `https://app.ierahkwa.gov`).

Detalle técnico: `RuddieSolution/node/ENV-PRODUCTION-SETUP.md` y `RuddieSolution/nginx/nginx.conf`.

---

## Resumen rápido

| Paso | Qué haces tú |
|------|------------------|
| 1 | Crear/copiar `.env` en `RuddieSolution/node` |
| 2 | Poner `JWT_ACCESS_SECRET` y `JWT_REFRESH_SECRET` en `.env` |
| 3 | Poner `CORS_ORIGIN` en `.env` |
| 4 | Poner contraseñas (leader/admin) o `PLATFORM_USERS_JSON` en `.env` |
| 5 | Ejecutar `npm run production:check` en `RuddieSolution/node` |
| 6 | Desde la raíz, ejecutar `./GO-LIVE-PRODUCTION.sh` |
| 7 | Abrir navegador y probar VIP Transactions y /health |
| 8 | (Opcional) Ejecutar `./scripts/install-cron-production.sh` |
| 9 | (Con dominio) Configurar HTTPS y proxy en el servidor |

Si en el paso 5 todo está OK y en el paso 6 el script termina sin errores, la plataforma queda **en vivo** y tú ya hiciste tu parte.

---

## Roadmap de despliegue soberano (timeline)

| Cuándo | Qué | Nota |
|--------|-----|------|
| **AHORA** | Bare metal dedicado (Hetzner/OVH) — servidor físico real, no compartido | Esta semana |
| **MES 1–2** | Montar Rack 1 con internet fija + FortiGate + ProLiant | Cuando tengas ISP |
| **MES 3** | DNS propio (bind9) + correo propio (Postfix) | Después del rack |
| **MES 6** | ASN + bloque IP + BGP | Cuando tengas tráfico |
| **Futuro** | Satélite propio para internet independiente | Ya lo tienes planeado |

Referencia de equipos y compras: `docs/DONDE-IR-Y-QUE-COMPRAR-PARA-LIVE.md`, `docs/CONEXION-COMPLETA-EQUIPOS.md`.

---

## Listo para que alguien conecte todo

Todo está documentado para que otra persona pueda conectar la infraestructura física sin duplicar trabajo:

1. **Conexión física (racks, cables, FortiGate, ProLiant)**  
   → `docs/CONEXION-COMPLETA-EQUIPOS.md` — topología, tabla de cables, qué va en cada rack, orden de conexión.

2. **Configuración mínima FortiGate y Cisco (para que pase internet)**  
   → `DEPLOY-SERVERS/CONFIG-MINIMA-FORTIGATE-CISCO-INTERNET.md`, `DEPLOY-SERVERS/INSTRUCCIONES-SIMPLES-FORTIGATE.md`.

3. **Inventario de equipos (qué hay en cada rack)**  
   → `DEPLOY-SERVERS/RACK-SERVICES-INVENTORY.md`, `DEPLOY-SERVERS/INVENTARIO-COMPLETO-EQUIPOS.md`.

4. **Software en vivo (esta guía)**  
   → Pasos 1–9 arriba; al final ejecutar `./GO-LIVE-PRODUCTION.sh` desde la raíz del proyecto.

Quien vaya a conectar: seguir en orden **CONEXION-COMPLETA-EQUIPOS** → config FortiGate/Cisco → luego arrancar servicios con `GO-LIVE-PRODUCTION.sh`.

5. **Servicios soberanos (ya en el repo)**  
   DNS, PKI, correo, VPN Manager, IDS/IPS, FIM, reverse proxy: `RuddieSolution/node/services/sovereign-*.js`. Estado: `GET /api/v1/sovereign-services/status`. Hardening del servidor: `./scripts/sovereign-hardening.sh` (ejecutar como root cuando el equipo esté montado).

---

## No duplicar arranques

Los scripts evitan lanzar dos veces el mismo arranque:

| Script | Comportamiento |
|--------|----------------|
| **start.sh** | Si los puertos 8545 (Node) o 3001 (Bridge) ya están en uso → no inicia. Para forzar: `FORCE_START=1 ./start.sh` (puede duplicar procesos). |
| **GO-LIVE-PRODUCTION.sh** | Lock `.go-live.lock`: si ya hay una ejecución en curso (mismo PID vivo) → sale con "No duplicar". |
| **RuddieSolution/servers/start-all-platforms.sh** | Lock `.platform-start.lock`: si otro start-all-platforms está corriendo → sale sin duplicar. |
| **RuddieSolution/servers/start-banking-hierarchy.sh** | Lock `.banking-hierarchy-start.lock`: misma lógica. |

Los archivos `.lock` se borran al terminar el script (o al recibir INT/TERM). Están en `.gitignore` y no se versionan.

**Logs (producción):** La salida de los servidores de plataforma (BDET, TradeX, SIIS, bancos centrales, etc.) y de la jerarquía bancaria va a `logs/platform-servers.log` y `logs/banking-hierarchy.log`, así la terminal no se llena de líneas `GET /health`.

---

## Verificación final (todo asegurado)

Antes de dar por listo el entorno:

1. **Ejecutar:** `./scripts/production-ready-check.sh` — debe mostrar OK en JWT, .env, puertos, plataformas financieras y script de backup.
2. **Ejecutar:** `./GO-LIVE-PRODUCTION.sh` — levanta Node 8545, Banking Bridge 3001, Editor API 3002 y verifica APIs (finance-all-in-one, seven-generations).
3. **Backup automático (cron):** `./RuddieSolution/scripts/backup-data-production.sh` — recomendado cada 6h para ai-hub, bdet-bank y datos críticos.
4. **APIs sensibles auditadas:** `/api/ai-hub`, `/api/v1/bdet`, `/api/v1/bdet-server` — rate-limit y audit en middleware.
