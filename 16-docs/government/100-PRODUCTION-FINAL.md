# 100% Production — Checklist Final

**Sovereign Government of Ierahkwa Ne Kanienke · Office of the Prime Minister**

---

## Pre-live (desde raíz del repo)

```bash
# 1. Verificación 100%
./scripts/verificar-100.sh

# 2. Go live
./GO-LIVE-PRODUCTION.sh
```

GO-LIVE auto-añade a `.env` si faltan:
- SOVEREIGN_MASTER_KEY
- STORAGE_ENCRYPT_KEY
- INTERNAL_SERVICE_TOKEN
- API_ORIGIN / PLATFORM_DOMAIN (desde CORS_ORIGIN si está definido)

---

## Post-live (recomendado)

### Cron (backups + health)

```bash
# Con localhost (desarrollo)
./scripts/install-cron-production.sh

# Con dominio real (producción)
BASE_URL=https://app.ierahkwa.gov ./scripts/install-cron-production.sh
```

- Health/ATABEY: cada 5 min
- Backup completo: diario 2:00
- Recurring run-due: diario 3:00

### Logrotate

```bash
# Copiar ejemplo y ajustar rutas
cp docs/logrotate-ierahkwa.example /etc/logrotate.d/ierahkwa
# Editar /etc/logrotate.d/ierahkwa con rutas reales del proyecto
```

### SSL (servidor con dominio)

```bash
sudo DOMAIN=app.ierahkwa.gov EMAIL=admin@ierahkwa.gov ./scripts/setup-ssl-certbot-nginx.sh
```

---

## Scripts útiles

| Script | Uso |
|-------|-----|
| `./scripts/verificar-100.sh` | Checklist links, env, config |
| `./scripts/verificar-links.js` | Validar 178+ enlaces |
| `./scripts/check-production-env.sh` | Variables .env críticas |
| `./scripts/sync-config-header-nav.js` | Tras editar config.json |
| `./scripts/install-cron-production.sh` | Backups + health |
| `./scripts/health-alert-check.sh [URL]` | Health manual (exit 1 = fallo) |

---

© 2026 Ierahkwa Futurehead — Office of the Prime Minister
