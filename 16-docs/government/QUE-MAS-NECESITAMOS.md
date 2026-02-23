# Qué más necesitamos — Estado actual y pendientes

**Sovereign Government of Ierahkwa Ne Kanienke**  
Resumen: qué ya está y qué falta por prioridad.  
Actualizado: 2026-02-02

---

## ✅ Ya está hecho (reciente + esta ola)

| Área | Implementado |
|------|----------------|
| **Monitoreo por plataforma** | `admin-monitoring.html?platform=casino|bdet|treasury|financial-center` + API `GET /api/v1/admin/monitoring?platform=...` |
| **Renta por plataforma** | Dashboard unificado en `/platform`; cada plataforma con su sección de renta. API `GET /api/v1/platform/rent?platform=...` |
| **Estado producción en dashboard** | Sección en `platform/index.html` que consume `GET /api/v1/production/status` (datos OK, endpoints) |
| **Auditoría de licencias** | `license-audit.html` + API `GET /api/v1/licenses/audit` (vigentes, por vencer, alertas) |
| **Developer portal** | `developer-portal.html` con catálogo de APIs soberanas |
| **Formularios BDET** | Crear cuenta y depósito en `bdet-accounts.html` (APIs reales) |
| **Panel compliance BDET** | Centro de aprobaciones en `bdet-bank.html` (pendientes, aprobar/rechazar) |
| **CORS y JWT en producción** | `.env.example` con CORS/JWT; `docs/GO-LIVE-CHECKLIST.md` |
| **Backup automático** | Script `node/scripts/backup-node-data.js` → `node/backups/` |
| **Notificaciones ATABEY** | APIs preferencias/notificaciones en ai-hub; página `atabey-notifications.html` |
| **404 amigable** | `platform/404.html` servido por el Node |
| **Smoke test** | `scripts/smoke-test.sh [BASE_URL]` para health/ready/production/bdet |
| **Verificación de puertos** | En `start.sh` (CHECK_PORTS=1) |
| **Caso de inversión en whitepapers** | Script `scripts/add-caso-inversion-whitepapers.js`; 109 whitepapers actualizados |
| **Card "Por qué invertir"** | En pre-launch (ej. IGT-DELIVERY-AIR, IGT-DELIVERY-LAND) |
| **API tokens en dashboard/cumplimiento** | Dashboard y License Authority usan `GET /api/v1/sovereignty/tokens` |
| **Backup programado** | `node/scripts/backup-cron.sh`; doc en GO-LIVE-CHECKLIST (cron) |
| **Reporte mensual BDET** | `GET /api/v1/bdet/reports/monthly` |
| **Health público AI Hub** | `GET /api/ai-hub/health` (atabey + recentErrors) |
| **Registro RWA** | `GET /api/v1/rwa/parcels`, `node/data/rwa-parcels.json` |
| **Rotación secrets** | `docs/ROTACION-SECRETS-JWT.md` |
| **Panel Logit** | `platform/logit.html`, ruta `/logit` |
| **PWA dashboard** | `platform/manifest.json`, `sw.js`, registro en index.html |
| **Changelog / API version** | `CHANGELOG.md` en raíz; nota sobre `/api/v2` |
| **Liquidación por token delivery** | En `GET /api/v1/logistics/status`: `deliveryTokensByMode` (IGT-DELIVERY-SEA/AIR/LAND) |

---

## Pendientes — Qué más (siguiente ola)

| # | Qué | Dónde / Cómo | Impacto |
|---|-----|--------------|---------|
| 1 | **Aplicar CORS/JWT en producción** | En el servidor real: `.env` con `CORS_ORIGIN`, `NODE_ENV=production`, JWT secrets (ver GO-LIVE-CHECKLIST). | Seguridad real al desplegar. |
| 2 | **Card "Por qué invertir" en todos los tokens** | Replicar la card de 108/109 en el resto de pre-launch (script o plantilla en cada `tokens/NN-IGT-XXX/index.html`). | Pre-launch uniforme. |
| 3 | **Formulario préstamo BDET** | En `bdet-accounts.html`: formulario "Solicitar préstamo" que llame a API BDET (si existe endpoint de préstamo). | Uso directo sin Postman. |
| 4 | **RWA: alta de parcelas** | Formulario o API `POST /api/v1/rwa/parcels` para registrar parcelas; opcional integración con BDET/colateral. | Registro de tierras operativo. |
| 5 | **Idiomas EN/FR/MOH en whitepapers** | Script que genere o actualice `whitepaper-en.md`, `-fr.md`, `-moh.md` desde `whitepaper-es.md` (plantilla o traducción). | Coherencia multiidioma. |
| 6 | **Persistir reporte mensual BDET** | Guardar resultado de reporte en `node/data/bdet-reports/YYYY-MM.json` y opción `GET /api/v1/bdet/reports/monthly?year=&month=`. | Histórico para auditoría. |
| 7 | **Iconos PWA** | Añadir `platform/assets/icon-192.png` e `icon-512.png` (manifest ya los referencia). | PWA instalable correctamente. |
| 8 | **Liquidación real con IGT-DELIVERY-*** | En sovereign-logistics: al cobrar tarifa/recompensa, opción de acreditar en IGT-DELIVERY-SEA/AIR/LAND según `transportMode` (requiere servicio de tokens para esos símbolos). | Liquidación por modo en producción. |

---

## Resumen ejecutivo

- **Hecho:** Todo lo listado en "Ya está hecho" (monitoreo, renta, auditoría, developer portal, formularios BDET, backup, notificaciones ATABEY, caso de inversión en whitepapers, card en 108/109, API tokens en dashboard/cumplimiento, reporte mensual BDET, health AI Hub, RWA API, rotación secrets, panel Logit, PWA, changelog, deliveryTokensByMode en status).
- **Siguiente:** Aplicar CORS/JWT en prod, extender card "Por qué invertir" a todos los tokens, formulario préstamo BDET, RWA alta de parcelas, whitepapers multiidioma, persistir reportes BDET, iconos PWA, liquidación real con tokens delivery.

---

## Referencias

| Tema | Documento |
|------|-----------|
| Plataformas y departamentos | `docs/ESTADO-PLATAFORMAS-DEPARTAMENTOS-SERVICIOS.md` |
| Lista priorizada anterior | `docs/QUE-MAS-IMPLEMENTAMOS.md`, `docs/QUE-MAS-IMPLEMENTARIA-RECOMENDADO.md` |
| Go-live | `docs/GO-LIVE-CHECKLIST.md` |
| BHBK y departamentos | `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md` |
| Plan inversión por token | `docs/PLAN-INVERSION-ATRACTIVO-POR-TOKEN.md` |
