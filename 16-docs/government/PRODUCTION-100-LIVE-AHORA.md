# Production 100% Live — Listo para ir en vivo

**Sovereign Government of Ierahkwa Ne Kanienke**  
Un solo runbook para poner la plataforma **100% en producción en vivo**.

---

## 1. Un comando para ir live (recomendado)

Desde la raíz del proyecto:

```bash
./GO-LIVE-PRODUCTION.sh
```

Este script:

- Define **NODE_ENV=production**
- Ejecuta verificaciones previas (pre-live, production-env, .env)
- Asegura **.env** con SOVEREIGN_MASTER_KEY, STORAGE_ENCRYPT_KEY, CORS si faltan
- Limpia procesos anteriores en 8545, 3001, 3002, 5000, 8080
- Inicia con **PM2** (si está instalado) o en background: Node (8545), Banking Bridge (3001), Editor API (3002)
- Verifica health de cada servicio
- Muestra todas las URLs de producción y abre el navegador

**Requisito:** Tener `RuddieSolution/node/.env` (puede crearse desde `.env.example`). JWT y CORS son obligatorios para producción real; el script puede añadir claves básicas si faltan.

**Plataformas que quedan en marcha:** Node (8545) — API, BDET, VIP, CryptoHost, Central Banks, SIIS, Clearing, Global Service, Government Portal, etc.; Banking Bridge (3001); Editor API (3002). En VIP Transactions, las transacciones se cargan desde el servidor al abrir la página o al pulsar **Actualizar**.

---

## 2. Arranque mínimo (solo Node en 8545)

Si solo quieres el servidor principal en modo producción:

```bash
./start-production-100.sh
```

O manualmente:

```bash
cd RuddieSolution/node
export NODE_ENV=production
npm run production
# o: node server.js
```

Crea `logs/` si no existe y escribe en `logs/node-server.log`.

---

## 3. Antes de producción real (checklist obligatorio)

| Paso | Acción | Referencia |
|------|--------|------------|
| 1 | Copiar `RuddieSolution/node/.env.example` → `RuddieSolution/node/.env` | `RuddieSolution/node/ENV-PRODUCTION-SETUP.md` |
| 2 | Generar JWT: `openssl rand -hex 32` (x2). Poner en `.env`: `JWT_ACCESS_SECRET`, `JWT_REFRESH_SECRET` | Mismo doc |
| 3 | Definir `CORS_ORIGIN=https://tu-dominio.gov` (sin barra final) | Mismo doc |
| 4 | Definir usuarios: `PLATFORM_USERS_JSON` o `PLATFORM_LEADER_PASSWORD` / `PLATFORM_ADMIN_PASSWORD` | Mismo doc |
| 5 | Poner **NODE_ENV=production** al arrancar (ya lo hace `GO-LIVE-PRODUCTION.sh` y `npm run production`) | `RuddieSolution/node/package.json` |
| 6 | HTTPS: Nginx o Caddy como reverse proxy delante del Node (puerto 8545) | `RuddieSolution/node/ENV-PRODUCTION-SETUP.md`, `RuddieSolution/nginx/nginx.conf` |
| 7 | Verificación .env: `cd RuddieSolution/node && npm run production:check` | Exit 0 = OK |

---

## 4. Verificar que está 100% live

```bash
# Health del Node
curl -s http://localhost:8545/health

# API VIP (transacciones)
curl -s http://localhost:8545/api/v1/vip/transactions | head -c 300

# Webhook DCB/DAES (debe existir la ruta)
curl -s -o /dev/null -w "%{http_code}" -X POST http://localhost:8545/api/v1/webhooks/dcb/cash-transfer -H "Content-Type: application/json" -d '{}'
# Esperado: 400 (falta payload) — la ruta responde
```

**URLs principales en vivo:**

| Qué | URL |
|-----|-----|
| Plataforma | http://localhost:8545/platform |
| VIP Transactions | http://localhost:8545/vip-transactions |
| BDET Bank | http://localhost:8545/platform/bdet-bank.html |
| Health Dashboard | http://localhost:8545/platform/health-dashboard.html |
| Leader Control | http://localhost:8545/leader-control |
| Security Fortress | http://localhost:8545/platform/security-fortress.html |

Con dominio y HTTPS, reemplazar `http://localhost:8545` por `https://tu-dominio`.

---

## 5. Backups y monitoreo (recomendado)

```bash
# Instalar cron: backups diarios + health cada 5 min
./scripts/install-cron-production.sh
```

Documentación operación:

- **Runbook 24/7:** `RuddieSolution/docs/RUNBOOK-24-7.md`
- **Playbook incidentes:** `RuddieSolution/docs/PLAYBOOK-RESPUESTA-INCIDENTES.md`
- **Checklist DR:** `RuddieSolution/docs/CHECKLIST-DR.md`

---

## 6. Resumen

| Objetivo | Comando / Acción |
|----------|-------------------|
| **Ir 100% live (todo el sistema)** | `./GO-LIVE-PRODUCTION.sh` |
| **Solo Node en producción** | `./start-production-100.sh` o `cd RuddieSolution/node && npm run production` |
| **Comprobar .env** | `cd RuddieSolution/node && npm run production:check` |
| **Checklist completo pre-live** | `GO-LIVE-CHECKLIST.md`, `RuddieSolution/node/PRODUCTION-LIVE-CHECKLIST.md` |
| **Configuración .env y HTTPS** | `RuddieSolution/node/ENV-PRODUCTION-SETUP.md` |

Con **.env** configurado (JWT, CORS, usuarios), **NODE_ENV=production** y, en producción real, **HTTPS** delante, la plataforma queda **100% lista para live**.
