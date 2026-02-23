# Subir todo a la nube — Paso a paso

**Sovereign Government of Ierahkwa Ne Kanienke**  
Guía para subir la plataforma a la nube (VPS, Fly.io, Railway, Render, DigitalOcean).

---

## Opción 1 — VPS (recomendado: un servidor donde tú controlas todo)

**Qué necesitas:** Un VPS ya contratado (Hetzner, OVH, DigitalOcean, Vultr, etc.) con **Ubuntu 22.04**, **IP pública** y acceso **SSH**.

### Paso 1 — Conectarte por SSH

```bash
ssh usuario@IP_DE_TU_SERVIDOR
```

Ejemplo: `ssh root@95.216.123.45`

### Paso 2 — Instalar Node.js en el servidor

En el servidor (por SSH):

```bash
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt-get install -y nodejs
node -v   # debe salir v20.x
```

### Paso 3 — Subir el proyecto al servidor

**Desde tu Mac (en tu casa),** en una terminal:

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives"

# Reemplaza usuario e IP por los de tu servidor
rsync -avz --exclude 'node_modules' --exclude '.git' --exclude 'logs' \
  ./ usuario@IP_DE_TU_SERVIDOR:/opt/ierahkwa/
```

O con **git** (si el repo está en GitHub/GitLab):

En el **servidor**:

```bash
sudo mkdir -p /opt/ierahkwa
sudo chown $USER:$USER /opt/ierahkwa
cd /opt/ierahkwa
git clone https://github.com/TU-USUARIO/TU-REPO.git .
# o: git clone <url-de-tu-repo> .
```

### Paso 4 — Crear .env en el servidor

En el **servidor**:

```bash
cd /opt/ierahkwa/RuddieSolution/node
cp .env.example .env
nano .env
```

Pon al menos:

- `NODE_ENV=production`
- `JWT_ACCESS_SECRET=` (genera con `openssl rand -hex 32`)
- `JWT_REFRESH_SECRET=` (otro `openssl rand -hex 32`)
- `CORS_ORIGIN=https://tudominio.com` (o `http://IP_DE_TU_SERVIDOR:8545` para probar)

Guarda (Ctrl+O, Enter, Ctrl+X).

### Paso 5 — Instalar dependencias y arrancar

En el **servidor**:

```bash
cd /opt/ierahkwa/RuddieSolution/node
npm ci --production
NODE_ENV=production PORT=8545 nohup node server.js > /opt/ierahkwa/logs/node.log 2>&1 &
```

Banking Bridge (opcional, en otra terminal o con PM2):

```bash
cd /opt/ierahkwa/RuddieSolution/node
NODE_ENV=production BRIDGE_PORT=3001 nohup node banking-bridge.js > /opt/ierahkwa/logs/bridge.log 2>&1 &
```

### Paso 6 — Abrir puertos en el firewall del servidor

En el servidor:

```bash
sudo ufw allow 8545
sudo ufw allow 3001
sudo ufw allow 80
sudo ufw allow 443
sudo ufw enable
```

### Paso 7 — Probar

En tu navegador:

- `http://IP_DE_TU_SERVIDOR:8545/health`
- `http://IP_DE_TU_SERVIDOR:8545/platform`

Si ves la plataforma, **ya está todo en la nube**.

---

## Opción 2 — Usar el script de deploy al servidor (desde tu Mac)

Si tienes **SSH** al servidor y el servidor ya tiene Node + PM2:

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives/RuddieSolution"

SERVER_HOST=IP_DE_TU_SERVIDOR SERVER_USER=tu_usuario ./scripts/deploy-to-physical.sh
```

La **primera vez** en ese servidor haz antes:

```bash
SERVER_HOST=IP_DE_TU_SERVIDOR SERVER_USER=tu_usuario ./scripts/prepare-physical-server.sh
```

(Los scripts están en `RuddieSolution/scripts/`; el path del repo en el servidor puede variar, revisa el script si usas otra ruta.)

---

## Opción 3 — Fly.io (PaaS, sin gestionar servidor)

### Paso 1 — Instalar Fly CLI

En tu Mac:

```bash
curl -L https://fly.io/install.sh | sh
```

Cierra y abre la terminal o ejecuta: `export FLYCTL_INSTALL="/Users/tu_usuario/.fly"` y `export PATH="$FLYCTL_INSTALL/bin:$PATH"`.

### Paso 2 — Autenticarte

```bash
fly auth login
```

### Paso 3 — Ir al proyecto y crear la app (solo primera vez)

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives/RuddieSolution"
fly launch -c deploy/fly.node.toml
```

Sigue el asistente (nombre de la app, región). **No** despliega aún si pregunta.

### Paso 4 — Poner variables en Fly

En [fly.io/apps](https://fly.io/apps) → tu app → **Secrets** (o Variables):

- `NODE_ENV` = `production`
- `JWT_ACCESS_SECRET` = (generado con `openssl rand -hex 32`)
- `JWT_REFRESH_SECRET` = (otro `openssl rand -hex 32`)
- `CORS_ORIGIN` = `https://tu-app.fly.dev` (o tu dominio)
- `PORT` = `8545`

### Paso 5 — Desplegar

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives/RuddieSolution"
./scripts/deploy-to-cloud.sh fly
```

O directamente:

```bash
fly deploy -c deploy/fly.node.toml
```

### Paso 6 — Probar

Abre la URL que te da Fly (ej. `https://tu-app.fly.dev/health` y `https://tu-app.fly.dev/platform`).

---

## Opción 4 — Docker en un VPS (stack completo)

Si en tu VPS quieres levantar **todo** (Node, Bridge, Postgres, Redis, etc.) con Docker:

En el **servidor** (después de instalar Docker):

```bash
cd /opt/ierahkwa
# Si subiste el repo con git clone o rsync desde la raíz "soberanos natives",
# la ruta puede ser: /opt/ierahkwa/RuddieSolution
cd RuddieSolution
cp node/.env.example node/.env
# Editar node/.env con NODE_ENV, JWT, CORS_ORIGIN

# En la raíz del repo (donde está deploy/docker-compose.cloud.yml)
docker compose -f deploy/docker-compose.cloud.yml --project-directory . up -d --build
```

Abre puertos 8545, 3001, 80, 443 y prueba `http://IP:8545/health`.

---

## Resumen rápido

| Dónde subir | Cómo |
|-------------|------|
| **VPS (tu servidor)** | SSH → instalar Node → subir código (rsync o git clone) → .env → `node server.js` (o PM2) → abrir puertos |
| **Script automático** | `SERVER_HOST=IP SERVER_USER=usuario ./scripts/deploy-to-physical.sh` (y antes `prepare-physical-server.sh`) |
| **Fly.io** | `fly auth login` → `fly launch -c deploy/fly.node.toml` → variables en dashboard → `./scripts/deploy-to-cloud.sh fly` |
| **Docker en VPS** | En el servidor: `docker compose -f deploy/docker-compose.cloud.yml up -d` (desde RuddieSolution) |

---

## Después de subir: HTTPS (dominio)

1. En tu registrador de dominios (Namecheap, Cloudflare, etc.) crea un registro **A** apuntando a la **IP del servidor**.
2. En el servidor instala **Nginx** y **Certbot** (Let's Encrypt):
   ```bash
   sudo apt install -y nginx certbot python3-certbot-nginx
   sudo certbot --nginx -d tudominio.com -d app.tudominio.com
   ```
3. Configura Nginx para que el dominio proxy a `http://127.0.0.1:8545`.
4. En `.env` pon `CORS_ORIGIN=https://app.tudominio.com`.

Referencia: `RuddieSolution/node/ENV-PRODUCTION-SETUP.md`, `DEPLOY-SERVERS/HTTPS-REVERSE-PROXY-EXAMPLE.md`.

---

*Más detalle: `DEPLOY-CLOUD.md`, `RuddieSolution/docs/DEPLOY-NUBE-Y-FISICO.md`, `docs/QUE-NECESITO-Y-QUE-COMPRAR-DONDE.md`.*

*Sovereign Government of Ierahkwa Ne Kanienke · Office of the Prime Minister · One Love, One Life.*
