# ✅ IERAHKWA — PRODUCTION READY | 100% LIVE

**Fecha:** 2026-01-25  
**Sistema:** Ierahkwa Futurehead Mamey Node + Platform

---

## 1. ESTADO DEL SISTEMA

| Componente | Estado | Notas |
|------------|--------|-------|
| **Health** | `/health` | `{ok:true, service:"MameyNode"}` |
| **Ready** | `/ready` | `{ready:true}` — K8s/PM2 readiness |
| **Live** | `/live` | `{live:true}` — K8s/PM2 liveness |
| **Platform** | 77 rutas | Todas los HTML en `platform/` existen |
| **APIs** | `/api/*` | VIP, BDET, Financial Hierarchy, Platform Global |
| **Duplicados** | 0 | Rutas y links sin duplicados |

---

## 2. CÓMO LEVANTAR EN PRODUCCIÓN

### Opción A — PM2 (recomendado 24/7)

```bash
cd RuddieSolution/node
cp .env.example .env
# Editar .env: NODE_ENV=production, JWT_ACCESS_SECRET, JWT_REFRESH_SECRET
npm install --production
mkdir -p logs
pm2 start ecosystem.config.js
pm2 save && pm2 startup
```

### Opción B — Docker

```bash
cd RuddieSolution/node
docker build -t ierahkwa-node .
docker run -d -p 8545:8545 --name ierahkwa ierahkwa-node
# Health: docker inspect --format='{{.State.Health.Status}}' ierahkwa
```

### Opción C — Node directo

```bash
cd RuddieSolution/node
NODE_ENV=production PORT=8545 node server.js
```

---

## 3. VARIABLES DE PRODUCCIÓN (.env)

| Variable | Obligatorio | Uso |
|----------|-------------|-----|
| `NODE_ENV` | Sí | `production` |
| `PORT` | No | Default 8545 |
| `JWT_ACCESS_SECRET` | Sí | Min 32 caracteres |
| `JWT_REFRESH_SECRET` | Sí | Min 32 caracteres |
| `BANKING_BRIDGE_URL` | Si usas proxy | ej. `http://localhost:3001` |
| `EDITOR_API_URL` | Si usas editor | ej. `http://localhost:3002` |
| `PLATFORM_API_URL` | Si usas casino | ej. `http://localhost:5112` |
| `KMS_MASTER_KEY` | Recomendado | Para KMS |

---

## 4. ENDPOINTS PRINCIPALES

| Ruta | Descripción |
|------|-------------|
| `/` | Dashboard |
| `/leader-control` | Control Center |
| `/app-ai-studio` | App & AI Studio |
| `/bdet-bank` | BDET Bank |
| `/wallet` | Wallet |
| `/mamey-futures` | Mamey Futures |
| `/api/platform-global` | Rutas y servicios |
| `/api/v1/vip/modules` | Módulos VIP |
| `/health` | Health check |
| `/ready` | Readiness probe |
| `/live` | Liveness probe |

---

## 5. VERIFICACIÓN RÁPIDA

```bash
# Con el servidor corriendo:
curl -s http://localhost:8545/health
curl -s http://localhost:8545/ready
curl -s http://localhost:8545/live

# O ejecutar (desde RuddieSolution):
./scripts/verify-production.sh
# Con otro host: ./scripts/verify-production.sh https://tudominio.com
```

---

## 6. PROXIES (opcionales)

- **Banking Bridge:** 3001 → `BANKING_BRIDGE_URL`
- **Editor API:** 3002 → `EDITOR_API_URL`
- **Platform Casino:** 5112 → `PLATFORM_API_URL`

Si no corres esos servicios, las rutas que los usan devolverán 503 hasta que estén arriba.

---

## 7. SEGURIDAD EN PRODUCCIÓN

- [ ] `NODE_ENV=production`
- [ ] JWT secrets de 32+ caracteres
- [ ] `KMS_MASTER_KEY` definido si usas KMS
- [ ] Rate limiting activo (ya en server)
- [ ] Helmet/CORS (ya en server)
- [ ] HTTPS vía nginx/traefik delante del Node

---

**Sistema listo para producción. 100% live para usar.** 🚀
