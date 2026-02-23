# DigitalOcean – IERAHKWA

## Opción A: App Platform (PaaS)

1. **Crear App** → **Repository** (GitHub/GitLab).
2. **Service: Node.js**
   - **Run Command:** `node server.js` (Node Mamey) o `BRIDGE_PORT=3001 node banking-bridge.js` (Bridge).
   - **Root:** `node`
   - **Build:** `npm install`
   - **HTTP Port:** 8545 (Node) o 3001 (Bridge).
3. **Variables:**
   - `NODE_ENV=production`
   - `BANKING_API_URL` = URL del Banking .NET si se despliega aparte.
4. **Health Check:** `/health` (Node) o `/api/health` (Bridge).

Para **Platform estático**: service Node con Root `platform`, Run `node server.js`, Puerto 8080, Health `/health`.

---

## Opción B: Droplet + Docker Compose

En un Droplet (Ubuntu):

```bash
# Instalar Docker y Docker Compose
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER
# Re-login

git clone <tu-repo> ierahkwa && cd ierahkwa
cp .env.example .env   # editar POSTGRES_PASSWORD, JWT_SECRET, etc.
docker compose -f deploy/docker-compose.cloud.yml up -d
./scripts/test-live.sh
```

Puertos a abrir en el Firewall / creador del Droplet: **5432, 6379, 5000, 8545, 3001, 8080** (o solo los públicos: 5000, 8545, 3001, 8080 si Postgres/Redis son solo internos).

---

## Opción C: Droplet + PM2 (sin Docker)

```bash
./install-private-server.sh
./start-all.sh
# o con systemd:
sudo bash systemd/install-services.sh
```

Ver `INSTALACION-SERVIDOR-PRIVADO.md`.
