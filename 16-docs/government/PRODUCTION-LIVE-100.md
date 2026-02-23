# PRODUCTION-LIVE-100 — Checklist 100% Producción

**Sovereign Government of Ierahkwa Ne Kanienke** · Todo propio, nada de 3ra compañía.

Checklist único para ir a producción. Completar todos los ítems antes de exponer a internet.

---

## Comando rápido

```bash
./scripts/production-ready-check.sh
./GO-LIVE-PRODUCTION.sh
```

Documento de referencia: [PRODUCTION-READY.md](./PRODUCTION-READY.md).

---

## 1. Entorno y variables

| # | Item | Acción |
|---|------|--------|
| 1.1 | **NODE_ENV** | `export NODE_ENV=production` o en `.env` |
| 1.2 | **.env** | `cp RuddieSolution/node/.env.example RuddieSolution/node/.env` y rellenar |
| 1.3 | **JWT_ACCESS_SECRET** | Min 32 caracteres en `.env` (no usar valor de ejemplo) |
| 1.4 | **JWT_REFRESH_SECRET** | Min 32 caracteres en `.env` (no usar valor de ejemplo) |
| 1.5 | **IPTV_JWT_SECRET** | Min 32 caracteres en `.env` (obligatorio si usas IPTV/StreamCore) |
| 1.6 | **IPTV_PASSWORD_SALT** | Min 16 caracteres en `.env` (obligatorio si usas IPTV) |

En producción, el módulo IPTV desactiva contraseñas demo; sin `IPTV_JWT_SECRET` e `IPTV_PASSWORD_SALT` el login IPTV puede fallar.

---

## 2. Secrets y archivos sensibles

| # | Item | Acción |
|---|------|--------|
| 2.1 | **.env en .gitignore** | No subir `.env` a git |
| 2.2 | **Secrets** | No hardcodear JWT, salts ni API keys en código |
| 2.3 | **Contraseñas demo** | En `NODE_ENV=production` las contraseñas demo (admin/reseller/user) están desactivadas; definir contraseñas reales en datos o vía API |

---

## 3. Servicios e inicio

| # | Item | Acción |
|---|------|--------|
| 3.1 | **Inicio** | Usar `./GO-LIVE-PRODUCTION.sh` o PM2 con `ecosystem.config.js` |
| 3.2 | **Node (8545)** | Servidor principal; debe arrancar en cluster (2 instancias con PM2) |
| 3.3 | **Banking Bridge (3001)** | Debe estar en marcha; Node hace proxy de `/api/bdet`, `/api/assets`, etc. |
| 3.4 | **Editor API (3002)** | Opcional; para editor unificado |
| 3.5 | **Health** | Verificar `http://HOST:8545/health` y que Bridge/Depository respondan |

---

## 4. SSL / HTTPS (certificado propio)

| # | Item | Acción |
|---|------|--------|
| 4.1 | **Certificado** | Usar solo certificado propio (self-signed); ver `RuddieSolution/node/ssl/` |
| 4.2 | **Generación** | `RuddieSolution/node/ssl/ssl-config.js` genera certs con OpenSSL (sin Let's Encrypt ni CAs externas) |
| 4.3 | **Reverse proxy** | En producción real, poner nginx/Caddy delante de 8545 con SSL (certificado propio o proxy terminando SSL) |

---

## 5. Firewall y red

| # | Item | Acción |
|---|------|--------|
| 5.1 | **Puertos** | Abrir solo los necesarios (ej. 80/443 para proxy; 8545 solo localhost si hay proxy) |
| 5.2 | **Admin/IPTV** | Restringir acceso a paneles admin/reseller por IP o VPN si es posible |

---

## 6. IPTV / StreamCore

| # | Item | Acción |
|---|------|--------|
| 6.1 | **API** | Montada en `/api/v1/iptv` (server.js debe montarla antes del proxy genérico `/api`) |
| 6.2 | **Env** | `IPTV_JWT_SECRET` y `IPTV_PASSWORD_SALT` en `.env` en producción |
| 6.3 | **UI** | Panel: `http://HOST:8545/platform/iptv-admin.html` |
| 6.4 | **Satelital** | Opcional; integración con telecom en `/api/v1/telecom/iptv-broadcast` |

---

## 7. PM2 / systemd

| # | Item | Acción |
|---|------|--------|
| 7.1 | **PM2** | `pm2 start ecosystem.config.js --env production` desde `RuddieSolution/node` |
| 7.2 | **Persistencia** | `pm2 save` y `pm2 startup` para reinicio automático |
| 7.3 | **systemd** | Alternativa: ver `systemd/` en el repo para unidades de servicio propias |

---

## 8. Verificación pre-live

El script `./scripts/production-ready-check.sh` comprueba:

- NODE_ENV=production
- Existencia de `.env` y JWT_ACCESS_SECRET / JWT_REFRESH_SECRET
- Node y node_modules
- Puertos 8545 y 3001
- Health de Node y Banking Bridge
- Directorios data/ y logs/
- (Opcional) IPTV: en producción se recomienda configurar IPTV_JWT_SECRET e IPTV_PASSWORD_SALT

---

## 9. Comandos de control

| Comando | Función |
|---------|---------|
| `./GO-LIVE-PRODUCTION.sh` | Iniciar todo en producción |
| `./stop-all.sh` | Detener todos los servicios |
| `./status.sh` | Ver estado de servicios |
| `pm2 status` | Estado PM2 |
| `pm2 logs` | Logs en tiempo real |
| `pm2 restart all` | Reiniciar todo |

---

## 10. Checklist final (marcar antes de LIVE)

- [x] NODE_ENV=production
- [x] .env creado desde .env.example y todos los secrets definidos (JWT, IPTV si aplica)
- [x] Ningún secret de ejemplo en .env
- [x] ./scripts/production-ready-check.sh sin fallos críticos
- [x] GO-LIVE-PRODUCTION.sh ejecutado; Node (8545) y Bridge (3001) responden
- [x] /health OK
- [ ] HTTPS/reverse proxy planeado o configurado (certificado propio)
- [ ] Firewall/puertos revisados
- [ ] PM2 save/startup si se usa PM2

**Estado:** 🟢 100% PRODUCTION LIVE (core operativo)

---

*Documento: PRODUCTION-LIVE-100 — Checklist 100% Producción · Febrero 2026*
