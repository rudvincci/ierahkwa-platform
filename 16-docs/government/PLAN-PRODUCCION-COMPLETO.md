# Plan completo para ir a producción

**Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister**

Consolida: FALTANTES-PARA-PRODUCCION.md, PRODUCTION-LIVE-CHECKLIST, GO-LIVE-CHECKLIST, QUE-IMPLEMENTAMOS-PARA-SER-LOS-MEJORES-GLOBALES.md, ENV-PRODUCTION-SETUP.

---

## Resumen ejecutivo

| Estado | Descripción |
|--------|-------------|
| **Código** | Listo para producción (148+ componentes, 50+ plataformas, APIs Node/.NET) |
| **Para ir LIVE** | Config operativa: .env, HTTPS, dominio, servicios levantados |
| **Para escalar** | MameyNode (Rust), SICB, Identity, ZKP — largo plazo |

**Opciones:** A) Producción básica con lo actual (limitaciones conocidas). B) Con Mamey-io (grado gubernamental/bancario).

---

## Fase 0 — Obligatorio (sin esto no es producción)

**Estimación:** 1–2 días | **Responsable:** DevOps / Admin

| # | Tarea | Detalle | Referencia |
|---|-------|---------|------------|
| 0.1 | **Servidor** | VPS/dedicado, mínimo 2 GB RAM, SSD | $600–1,200/año |
| 0.2 | **Dominio y DNS** | Registrar dominio, A record al servidor | - |
| 0.3 | **.env Node** | Copiar `node/.env.example` → `node/.env` | `node/ENV-PRODUCTION-SETUP.md` |
| 0.4 | **JWT 32+ chars** | `openssl rand -hex 32` → JWT_ACCESS_SECRET, JWT_REFRESH_SECRET | `node/ENV-PRODUCTION-SETUP.md` |
| 0.5 | **CORS_ORIGIN** | Dominio real, ej. `https://app.ierahkwa.gov` | `.env` |
| 0.6 | **Usuarios** | PLATFORM_USERS_JSON o PLATFORM_LEADER_PASSWORD, PLATFORM_ADMIN_PASSWORD | `node/ENV-PRODUCTION-SETUP.md` |
| 0.7 | **NODE_ENV=production** | Arrancar con `npm run production` | - |
| 0.8 | **HTTPS** | Nginx o Caddy reverse proxy + Let's Encrypt | `node/ENV-PRODUCTION-SETUP.md`, `nginx/nginx.conf` |
| 0.9 | **Verificación .env** | `cd node && npm run production:check` → exit 0 | `node/scripts/check-production-env.js` |

**Checklist Fase 0:**
- [ ] Servidor y dominio
- [ ] .env con JWT, CORS, usuarios
- [ ] NODE_ENV=production
- [ ] HTTPS en proxy (Nginx/Caddy)

---

## Fase 1 — Infra y vigilancia (muy recomendable)

**Estimación:** 2–4 días | **Responsable:** DevOps

| # | Tarea | Detalle | Referencia |
|---|-------|---------|------------|
| 1.1 | **Backups automáticos** | Cron diario 2:00 con `install-cron-production.sh` | `RuddieSolution/scripts/install-cron-production.sh` |
| 1.2 | **Backup encriptado node/data** | `backup-node-data-encrypted.sh` + BACKUP_PASSPHRASE opcional | `RuddieSolution/scripts/backup-node-data-encrypted.sh` |
| 1.3 | **Rotación de logs** | logrotate para `LOG_DIR` | `RuddieSolution/docs/logrotate-ierahkwa.example` |
| 1.4 | **Health desde proxy** | Proxy hace GET a `/health`, `/ready`, `/metrics` | - |
| 1.5 | **Alertas si cae** | Cron cada 5 min: `health-alert-check.sh` + email/webhook | `RuddieSolution/scripts/health-alert-check.sh` |
| 1.6 | **Runbook 24/7** | Revisar RUNBOOK-24-7.md, PLAYBOOK-RESPUESTA-INCIDENTES, CHECKLIST-DR | `RuddieSolution/docs/` |

**Checklist Fase 1:**
- [ ] Cron backup diario
- [ ] Rotación logs
- [ ] Alertas de health
- [ ] Runbook revisado

---

## Fase 2 — Servicios por plataforma

**Estimación:** 1–3 días según servicios usados

| Servicio | Puerto | Qué hacer | Variable Node |
|----------|--------|-----------|---------------|
| **Node (Mamey)** | 8545 | Siempre. `npm run production` | - |
| **Platform API** | 3000 | .env, deploy-production.sh o PM2 | PLATFORM_API_URL |
| **Banking Bridge** | 3001 | Levantar proceso; .env bridge | BANKING_BRIDGE_URL |
| **Editor API** | 3002 | Levantar; auth y .env | EDITOR_API_URL |
| **NET10 DeFi** | 5071 | Levantar .NET API | NET10_API_URL |
| **TradeX** | 5054 | Opcional | TRADEX_URL |
| **Forex Server** | 5200 | Si usas forex | FOREX_SERVER_URL |
| **Rust SWIFT** | 8590 | Opcional (Live Connect) | RUST_SERVICE_URL |
| **Go Queue** | 8591 | Opcional | GO_SERVICE_URL |
| **Python ML** | 8592 | Opcional | PYTHON_SERVICE_URL |

**Scripts de arranque:**
- `./start.sh` — arranca todo
- `./services/start-all.sh` — Rust, Go, Python
- `./go-live.sh` — go-live completo

---

## Fase 3 — Seguridad adicional

**Estimación:** 1–2 semanas

| # | Tarea | Detalle |
|---|-------|---------|
| 3.1 | **Hardening Node** | O considerar MameyNode (Rust) a futuro |
| 3.2 | **WAF/DDoS** | Nginx mod_security o Cloudflare |
| 3.3 | **HSM / custodia llaves** | O TreasuryKeyCustodies (Mamey-io) |
| 3.4 | **Audit logs** | Ya existe AuditTrail; revisar cobertura |
| 3.5 | **KYC/AML** | Proceso manual o Mamey.FWID.Identities |
| 3.6 | **WhistleblowerReports** | Canal de denuncias (ya existe whistleblower) |

---

## Fase 4 — Ya implementado en código ✅

No requiere implementación; solo verificar en producción:

| Ítem | Estado |
|------|--------|
| Estado producción en dashboard | `GET /api/v1/production/status` |
| AI Hub health público | `GET /api/ai-hub/health`, `platform/ai-hub-status.html` |
| Developer portal | `platform/developer-portal.html` |
| Panel compliance BDET | `node/public/bdet-compliance.html` |
| Panel RWA | `node/public/rwa-panel.html` |
| Formularios BDET (cuenta, préstamo, depósito) | `node/public/bdet-accounts.html` |
| Reporte mensual BDET | `GET /api/v1/bdet/reports/monthly`, `GET /api/v1/bdet-server/api/reports/monthly` |
| SDK JS/TS | `RuddieSolution/sdk/ierahkwa-sdk.js`, `.d.ts` |
| PWA dashboard | `platform/manifest.json`, `platform/sw.js` |
| Backup encriptado script | `RuddieSolution/scripts/backup-node-data-encrypted.sh` |
| Card "Por qué invertir" en tokens | 107 tokens |
| CHANGELOG | `CHANGELOG.md` |
| Rate limits, auditoría | middleware en server.js |

---

## Fase 5 — Opcional / post go-live

| # | Tarea | Prioridad | Referencia |
|---|-------|-----------|------------|
| 5.1 | Notificaciones ATABEY (precios, préstamos, KYC, fraude) | Media | QUE-IMPLEMENTAMOS |
| 5.2 | Auditoría licencias con alertas vencimiento | Media | `GET /api/v1/licenses/audit` ya existe |
| 5.3 | SDKs Python, Go | Baja | FALTANTES-PARA-PRODUCCION |
| 5.4 | Identidad biométrica (FutureWampumID) | Largo plazo | FALTANTES |
| 5.5 | Rotación JWT sin downtime | Baja | `docs/ROTACION-SECRETS-JWT.md` |

---

## Fase 6 — Largo plazo (Mamey-io / escalar)

Si se requiere grado gubernamental/bancario o escalar:

| Componente | Descripción | Repo |
|------------|-------------|------|
| MameyNode | Blockchain Rust (~100+ TPS) | Mamey-io/MameyNode |
| MameyFramework | Base .NET | Mamey-io/MameyFramework |
| MameyLockSlot | Bloqueo distribuido | Mamey-io/MameyLockSlot |
| Mamey.Government.Identity | Auth centralizada | Mamey-io/Mamey.Government.Identity |
| Mamey.FWID.Identities | Identidad biométrica | Mamey-io/Mamey.FWID.Identities |
| Mamey.SICB.* | Tesorería soberana, ZKP, tratados | Mamey-io/Mamey.SICB.* |
| MameyNode.TypeScript/JavaScript | SDKs oficiales | Mamey-io/MameyNode.* |

**Coste estimado:** Sin Mamey-io ~$91,500 año 1 | Con Mamey-io ~$41,500 año 1 (integración).

---

## Checklist único pre go-live

```
OBLIGATORIO
[ ] Servidor + dominio + DNS
[ ] node/.env con JWT 32+, CORS_ORIGIN, usuarios
[ ] npm run production:check → OK
[ ] NODE_ENV=production al arrancar
[ ] HTTPS (Nginx/Caddy + Let's Encrypt)
[ ] Proxy enruta a Node 8545

RECOMENDABLE
[ ] Cron backup diario (install-cron-production.sh)
[ ] Backup encriptado node/data
[ ] Rotación logs
[ ] Alertas health (health-alert-check.sh)
[ ] Runbook y playbook revisados

POR SERVICIO (solo si usas)
[ ] Platform API (3000) — .env, deploy
[ ] Banking Bridge (3001) — levantar, BANKING_BRIDGE_URL
[ ] NET10 (5071) — .NET API
[ ] Forex (5200), Rust (8590), Go (8591), Python (8592)
```

---

## Orden de ejecución sugerido

1. **Día 1:** Fase 0 completa (servidor, dominio, .env, HTTPS)
2. **Día 2:** Verificación (`production:check`), arrancar Node, probar health/metrics
3. **Día 3:** Fase 1 (backups, logs, alertas)
4. **Día 4+:** Levantar servicios adicionales (Platform, Banking, NET10) según necesidad
5. **Post go-live:** Fase 5 opcional, Fase 6 largo plazo

---

## Script GO-PRODUCTION.sh

Ejecutar para verificación pre go-live:

```bash
./GO-PRODUCTION.sh [BASE_URL]
```

Ejecuta: `production:check` (Node), health checks, imprime checklist y próximos pasos. `BASE_URL` default: `http://localhost:8545`.

---

## Referencias rápidas

| Documento | Contenido |
|-----------|-----------|
| `node/ENV-PRODUCTION-SETUP.md` | Paso a paso .env, JWT, HTTPS |
| `node/PRODUCTION-LIVE-CHECKLIST.md` | Checklist por plataforma |
| `GO-LIVE-CHECKLIST.md` | Live Connect, JWT, HTTPS |
| `FALTANTES-PARA-PRODUCCION.md` | Mamey-io, SICB, ZKP, SDKs |
| `docs/QUE-IMPLEMENTAMOS-PARA-SER-LOS-MEJORES-GLOBALES.md` | Roadmap mejores globales |
| `RuddieSolution/docs/RUNBOOK-24-7.md` | Operación 24/7 |
| `RuddieSolution/nginx/nginx.conf` | Config Nginx reverse proxy (Docker) |
| `RuddieSolution/nginx/nginx-single-server.conf` | Config Nginx servidor único (127.0.0.1) |

---

**© 2026 Sovereign Government of Ierahkwa Ne Kanienke**
