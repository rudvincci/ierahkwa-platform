# Despliegue en la nube – IERAHKWA

Guía para desplegar y **probar en servidores live en la nube** (Railway, Render, Fly.io, DigitalOcean, Docker en VPS).

---

## Resumen de alternativas

| Plataforma     | Archivos / acciones                    | Notas                       |
|----------------|----------------------------------------|-----------------------------|
| **Docker (VPS/nube)** | `deploy/docker-compose.cloud.yml`     | Postgres, Redis, .NET, Node, Bridge, Platform |
| **Railway**    | `deploy/railway.json`, `deploy/railway.banking-bridge.json` | Node, Banking Bridge por servicio |
| **Render**     | `deploy/render.yaml` (copiar a `render.yaml` en raíz si aplica) | Node, Bridge, Platform; Banking .NET aparte |
| **Fly.io**     | `deploy/fly.node.toml`, `fly.banking-bridge.toml`, `fly.platform.toml` | Un `fly.toml` por app          |
| **DigitalOcean** | `deploy/digitalocean.md`              | App Platform, Droplet + Docker o PM2 |

---

## 1. Docker en VPS / cualquier nube

Desde la **raíz del repo**:

```bash
# Crear .env con POSTGRES_PASSWORD, REDIS_PASSWORD, JWT_SECRET
docker compose -f deploy/docker-compose.cloud.yml --project-directory . up -d
```

El compose en `deploy/` usa `context: ..` para construir desde la raíz. Servicios:

- `postgres`, `redis`
- `banking-api` (.NET)
- `node-mamey` (8545)
- `banking-bridge` (3001)
- `platform` (8080)

Probar en vivo:

```bash
NODE_URL=http://TU_IP:8545 \
BRIDGE_URL=http://TU_IP:3001 \
BANKING_URL=http://TU_IP:5000 \
PLATFORM_URL=http://TU_IP:8080 \
./scripts/test-live.sh
```

---

## 2. Railway

- Crea un proyecto y, por cada servicio, conecta el repo.
- **Node Mamey:** en la raíz, usa `deploy/railway.json` (o configura Nixpacks/start a mano: `node server.js`, root `node`, health `/health`).
- **Banking Bridge:** usa `deploy/railway.banking-bridge.json` o: root `node`, start `BRIDGE_PORT=3001 node banking-bridge.js`, health `/api/health`.
- Define `BANKING_API_URL` si el Banking .NET está en otro servicio (Railway u otra nube).

Probar:

```bash
NODE_URL=https://tu-node.railway.app \
BRIDGE_URL=https://tu-bridge.railway.app \
./scripts/test-live.sh
```

(Si Banking .NET y Platform están en Railway, añade `BANKING_URL` y `PLATFORM_URL`.)

---

## 3. Render

- Copia `deploy/render.yaml` a `render.yaml` en la raíz (o indica la ruta al blueprint en el dashboard).
- Ajusta `fromService` para `BANKING_API_URL` o pon `BANKING_API_URL` a mano si el Banking .NET está en otro sitio.
- Banking .NET: despliega por separado (Docker o .NET en Render) y pone la URL en `BANKING_API_URL`.

Ejemplo de prueba:

```bash
NODE_URL=https://ierahkwa-node.onrender.com \
BRIDGE_URL=https://ierahkwa-banking-bridge.onrender.com \
PLATFORM_URL=https://ierahkwa-platform.onrender.com \
BANKING_URL=https://tu-banking-api.onrender.com \
./scripts/test-live.sh
```

---

## 4. Fly.io

Desde la raíz:

```bash
# Node
fly launch -c deploy/fly.node.toml
fly deploy -c deploy/fly.node.toml

# Banking Bridge (definir BANKING_API_URL con fly secrets)
fly secrets set BANKING_API_URL=https://tu-banking-api.fly.dev
fly launch -c deploy/fly.banking-bridge.toml
fly deploy -c deploy/fly.banking-bridge.toml

# Platform
fly launch -c deploy/fly.platform.toml
fly deploy -c deploy/fly.platform.toml
```

Probar:

```bash
NODE_URL=https://ierahkwa-node.fly.dev \
BRIDGE_URL=https://ierahkwa-banking-bridge.fly.dev \
PLATFORM_URL=https://ierahkwa-platform.fly.dev \
BANKING_URL=https://tu-banking-api.fly.dev \
./scripts/test-live.sh
```

---

## 5. DigitalOcean

Ver `deploy/digitalocean.md`:

- **App Platform:** servicios Node (Node, Bridge, Platform) con `node server.js` o `node banking-bridge.js`, health `/health` o `/api/health`.
- **Droplet + Docker:** igual que “Docker en VPS” con `deploy/docker-compose.cloud.yml`.
- **Droplet + PM2:** `install-private-server.sh` y `start-all.sh` (ver `INSTALACION-SERVIDOR-PRIVADO.md`).

---

## 6. Probar todas las alternativas en vivo

Para cada despliegue (servidor propio, Railway, Render, Fly, DO):

1. Desplegar el stack (Docker) o cada servicio (PaaS).
2. Anotar las URLs (Node, Bridge, Banking .NET, Platform).
3. Ejecutar:

```bash
NODE_URL=<url> BRIDGE_URL=<url> BANKING_URL=<url> PLATFORM_URL=<url> \
./scripts/test-live.sh
```

Con eso se comprueban `/health`, `/api/health`, `/api/status`, etc. en **entorno live en la nube** y en servidor privado.
