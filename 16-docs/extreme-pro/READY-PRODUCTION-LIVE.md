# ¿Ready para production / live?

**Fecha:** 2026-01-23

---

## Resumen: **casi listo — faltan env, nginx/SSL y CORS**

| Área | Estado | Notas |
|------|--------|-------|
| **Servidor Node** | ✅ | /health, /ready, /live, rate limit, helmet, compression, logging, metrics |
| **Config (config/platform, ROUTES)** | ✅ | platform-global, financial-hierarchy, routes |
| **Docker (deploy/Dockerfile.node)** | ✅ | Incluye config/ y platform/; `docker-compose.cloud` y APP_ROOT actualizados |
| **nginx + SSL** | ⚠️ | nginx apunta a `node-app`, `platform`, `banking-bridge`; certs LetsEncrypt por generar |
| **Variables de entorno** | ⚠️ | JWT, DB, Redis en `.env.example`; en producción `.env` con valores reales |
| **CORS** | ⚠️ | `app.use(cors())` abierto; en producción restringir orígenes |
| **Tests** | ⚠️ | Unit: ✅ 22/22. API: fallan (AbortController, servidor en 8545) |

---

## 1. Listo para producción

- **/health** → `{ ok: true, service: 'MameyNode' }`
- **Rate limiting** en `/api` (si existe `middleware/rate-limit`)
- **Helmet** (CSP desactivado), **compression**, **express.json**
- **Logging** centralizado (si existe `logging/centralized-logger`)
- **Prometheus /metrics** (si existe `middleware/metrics`)
- **platform-global**: ROUTES, REDIRECTS, SERVICES; sin duplicados
- **APIs**: financial-hierarchy, global, maletas, platform-global
- **Deploy configs**: `deploy/fly.node.toml`, `deploy/docker-compose.cloud.yml`, `deploy/render.yaml`

---

## 2. Crítico antes de live

### 2.1 Node en Docker: `config` y `platform`

El `server.js` usa:

- `require('../config/platform-global')` → necesita `config/` como hermano de `node/`
- `path.join(ROOT, 'tokens')`, `express.static(platform)`, etc. → necesita `platform/` y opcionalmente `tokens/`

Con `context: ./node` el build **no** incluye `config` ni `platform` → el proceso **cae al arrancar**.

**Qué hacer:**

- Usar un **Dockerfile que construya desde la raíz del repo** (donde están `config/` y `platform/`) y copie `node/`, `config/` y `platform/` al imagen.  
  Hay un `deploy/Dockerfile.node` de ejemplo que hace eso.
- O en `docker-compose`/K8s montar `config` y `platform` en las rutas que `server.js` espera (p. ej. `../config` y `../platform` respecto al `WORKDIR` del contenedor).

### 2.2 `APP_ROOT` para `ROOT`

`ROOT` se usa en `path.join(ROOT, 'tokens')`, `AssetTracker`, `TradeX`, etc. En contenedor, `path.join(__dirname, '..', '..')` puede no coincidir con la raíz del proyecto.

**Cambio en `server.js`:**

```js
const ROOT = process.env.APP_ROOT || path.join(__dirname, '..', '..');
```

En Docker (si todo va bajo `/workspace`): `APP_ROOT=/workspace`.

### 2.3 `.env` en producción

- Copiar `node/.env.example` → `node/.env` (o inyectar env en el host/ordenador de ejecución).
- Valores obligatorios para producción:
  - `NODE_ENV=production`
  - `JWT_ACCESS_SECRET`, `JWT_REFRESH_SECRET` (o `JWT_SECRET`) con secretos fuertes (≥32 caracteres).
  - Si usas DB: `PG_*`, `MONGO_URI`, `REDIS_*`, etc.
- No commitear `.env` (debe estar en `.gitignore`).

---

## 3. Recomendado antes de live

### 3.1 CORS

- Sustituir `app.use(cors())` por algo del estilo:

  ```js
  app.use(cors({
    origin: process.env.CORS_ORIGIN || /\.ierahkwa\.(gov|io)$/,
    credentials: true
  }));
  ```

- O una lista fija de orígenes en producción.

### 3.2 nginx y SSL

- **Hosts en nginx**: `node_backend` → `node:8545` (el servicio se llama `node` en `docker-compose`, no `node-app`). Ajustar `platform_backend` y `banking_backend` a los nombres reales de los servicios.
- **Certificados**: nginx usa `/etc/letsencrypt/live/ierahkwa.gov/`. En producción:
  - Certbot/ACME para obtener los certs en ese path, o
  - Montar certs en `/etc/nginx/ssl` y cambiar las directivas `ssl_certificate`/`ssl_certificate_key` a ese path (o al que uses).

### 3.3 Endpoints `/ready` y `/live`

**Hecho:** En `server.js` se añadieron `app.get('/ready', ...)` y `app.get('/live', ...)` que responden `{ ready: true }` / `{ live: true }`.

### 3.4 `docker-compose` vs nginx

- `docker-compose.yml` define: `node`, `bridge`, `editor`, `dotnet`, `gateway`, `ml`, `crypto`, `tradex`, `redis`, `mongo`, `postgres`, `rabbitmq`, `prometheus`, `grafana`, `nginx`.
- nginx asume: `node-app`, `platform`, `banking-bridge`, `grafana`, `kibana`. Alinear nombres de servicios y puertos entre `docker-compose` y `nginx.conf` (o usar un `nginx.conf` que coincida con tu `docker-compose`).

### 3.5 Dockerfiles referenciados

- `Dockerfile.bridge` en `./node` (bridge)
- `Dockerfile.editor` en `./platform` (editor)
- `Dockerfile.tradex` en `./servers`

Comprobar que existan y que el `docker-compose` (o el `docker-compose.cloud`) los use con el `context` correcto.

---

## 4. Cómo ir a live (mínimo)

### Opción A: sin Docker (VPS / PM2)

1. Clonar el repo (Software2026 o RuddieSolution según dónde vivan `config` y `platform`).
2. En la raíz del proyecto (donde están `node/`, `config/`, `platform/`):
   - `cd node && npm ci --only=production`
   - Crear y rellenar `node/.env` desde `node/.env.example`.
3. Poner `NODE_ENV=production` y `PORT=8545` (o el puerto que uses).
4. Arrancar: `node server.js` o con PM2:  
   `pm2 start server.js -n mamey --node-args="--max-old-space-size=512"`.
5. Delante, nginx o Caddy con HTTPS y `proxy_pass` a `http://127.0.0.1:8545`.

### Opción B: Docker (solo Node + config + platform)

1. Usar un Dockerfile que incluya `node/`, `config/` y `platform/` (p. ej. `deploy/Dockerfile.node` con `context = .` en la raíz del repo).
2. En el `docker run` o en `docker-compose`:
   - `APP_ROOT=/workspace` (o la ruta donde hayas copiado `config`/`platform`/`tokens`).
   - Las mismas variables de `node/.env.example` (o un `.env` montado).
3. Exponer `8545` y poner nginx u otro reverso con HTTPS delante.

### Opción C: Fly.io

```bash
# desde la raíz del repo (donde está config/ y platform/)
fly launch -c deploy/fly.node.toml
fly secrets set JWT_ACCESS_SECRET="..." JWT_REFRESH_SECRET="..."
fly deploy -c deploy/fly.node.toml
```

- El `fly.node.toml` debe apuntar a un Dockerfile que copie `config/` y `platform/` (o a una imagen que ya los traiga). Si el `Dockerfile` actual solo construye `node/`, hay que sustituirlo o usar otro `Dockerfile` como en la opción B.

---

## 5. Checklist rápida pre-live

- [ ] `config/` y `platform/` disponibles en el entorno donde corre Node (Docker o filesystem).
- [ ] `APP_ROOT` definido en contenedor si usas Docker; o `ROOT` correcto sin Docker.
- [ ] `.env` (o variables inyectadas) con `NODE_ENV=production`, JWT, y DB/Redis si los usas.
- [ ] JWT secrets con al menos 32 caracteres y no por defecto.
- [ ] CORS acotado a tus dominios en producción.
- [ ] nginx (o similar) con HTTPS y nombres de upstream alineados con tus servicios.
- [ ] Certificados SSL (LetsEncrypt o propios) generados y montados donde nginx los espera.
- [ ] `/health` (y opcionalmente `/ready`, `/live`) respondiendo detrás del proxy.
- [ ] Tests unitarios en verde; tests de API con el servidor arriba en un entorno de staging.

---

## 6. Conclusión

- **Para producción en vivo**: hace falta corregir la **imagen Docker del Node** (incluir `config` y `platform`) y la definición de **ROOT/APP_ROOT**, y después cerrar **env, CORS, nginx y SSL** según este documento.
- **Para pruebas en local**: con `config` y `platform` en el repo y `node server.js` en la raíz correcta, puedes considerar que estás “ready” para un entorno de **staging** o **pre-live**, siempre que no dependas de los estáticos que usan `ROOT` fuera de `RuddieSolution` (AssetTracker, TradeX, etc.). Esos en un deploy mínimo pueden no existir y devolver 404 hasta que montes esos proyectos.

---

*Actualizado a partir de `server.js`, `node/Dockerfile`, `docker-compose.yml`, `nginx/nginx.conf`, `deploy/*` y `node/.env.example`.*
