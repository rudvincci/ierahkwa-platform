# GO-LIVE Checklist — Producción Ierahkwa Node

**Sovereign Government of Ierahkwa Ne Kanienke**  
Antes de exponer el Node en producción, completar esta lista.

---

## 1. Variables de entorno (.env)

| Variable | Descripción | Producción |
|---------|-------------|------------|
| `NODE_ENV` | `production` activa modo seguro (desactiva contraseñas demo, refuerza headers). | `NODE_ENV=production` |
| `CORS_ORIGIN` | Orígenes permitidos para CORS (separados por coma). Sin definir = CORS abierto. | `CORS_ORIGIN=https://app.ierahkwa.gov,https://admin.ierahkwa.gov` |
| `JWT_ACCESS_SECRET` | Secreto para access tokens (mín. 32 caracteres). Generar: `openssl rand -hex 32` | Obligatorio, único por entorno |
| `JWT_REFRESH_SECRET` | Secreto para refresh tokens (mín. 32 caracteres). | Obligatorio, distinto de ACCESS |
| `PORT` | Puerto del Node (por defecto 8545). | Definir si no es 8545 |

**Copiar plantilla:** `cp node/.env.example node/.env` y rellenar valores. No subir `.env` a repositorio.

---

## 2. Seguridad

- [ ] `.env` con `NODE_ENV=production`
- [ ] `CORS_ORIGIN` restringido a dominios propios (no `*` en producción)
- [ ] `JWT_ACCESS_SECRET` y `JWT_REFRESH_SECRET` ≥ 32 caracteres, aleatorios
- [ ] HTTPS en front y en el Node (reverse proxy con TLS o Node con certificados). Ver **`docs/CERTIFICADOS-SSL-TLS.md`** (rutas, certbot, nginx).
- [ ] Rotación de secrets documentada: ver `docs/QUE-MAS-IMPLEMENTAMOS.md` (rotación JWT)

---

## 3. Datos y endpoints críticos

- [ ] Ejecutar `node scripts/verificar-production-live.js` y corregir hasta que pase
- [ ] `GET /api/v1/production/status` devuelve `productionReady: true`
- [ ] Archivos en `node/data/` presentes (resumen-soberano, ciberseguridad-101, bank-registry, etc.)

---

## 4. Backup

- [ ] Script de backup automático configurado: `node scripts/backup-node-data.js` (cron o post-start)
- [ ] Ejemplo cron (diario 02:00): `0 2 * * * cd /ruta/a/RuddieSolution/node && node scripts/backup-node-data.js`
- [ ] O ejecutar después del arranque: `node/scripts/backup-cron.sh`
- [ ] Destino de backups seguro y accesible para restauración

---

## 5. Monitoreo

- [ ] Dashboard de monitoreo accesible: `/admin-monitoring.html?platform=...`
- [ ] Health check: `GET /api/health/all` o `GET /health` para load balancer

---

## Referencias

- Plantilla env: `RuddieSolution/node/.env.example`
- Producción live: `docs/100-PRODUCTION-LIVE.md`
- Verificación: `node/scripts/verificar-production-live.js`

---

*Office of the Prime Minister — Ierahkwa Ne Kanienke.*
