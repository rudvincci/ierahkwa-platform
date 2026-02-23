# Dónde ver todo — Resumen único

**Sovereign Government of Ierahkwa Ne Kanienke**  
Un solo documento para ver toda la plataforma, lo nuevo y las rutas.

---

## 1. Punto de entrada principal

| Dónde | URL (base: http://localhost:8545) |
|-------|-----------------------------------|
| **Platform (hub principal)** | `/platform` o `/platform/index.html` |
| **Dashboard Node (Mamey)** | `/` o `/index.html` |
| **Hub — Todo lo nuevo** | `/platform#hubTodoNuevo` |

Desde el Node hay un botón **⭐ Todo lo nuevo** que lleva a `/platform#hubTodoNuevo`. En el header de la plataforma aparece el badge **⭐ TODO NUEVO** con el mismo destino.

---

## 2. Lo nuevo unificado (Hub)

En **`/platform#hubTodoNuevo`** tienes acceso a:

| Nombre | Ruta | Qué es |
|--------|------|--------|
| Monitoreo por plataforma | `/admin-monitoring.html` | Estado por Casino, BDET, Treasury, SFC |
| ATABEY Notificaciones | `/atabey-notifications.html` | Preferencias y alertas |
| Auditoría licencias | `/license-audit.html` | Vigentes, por vencer, alertas |
| Developer portal | `/platform/developer-portal.html` | Catálogo APIs soberanas |
| Panel Logit | `/logit` | Estado SLS/CDE por región |
| Logística SLS/CDE | `/platform/logistics.html` | Manifiestos, órdenes, tracking |
| BDET Cuentas | `/bdet-accounts.html` | Crear cuenta, depósito |
| BDET Compliance | `/bdet-bank` | Centro de aprobaciones |
| Licenses + Tokens | `/license-authority.html` | Emisión licencias, API tokens |
| Renta por plataforma | `#commercialServicesSection` (en `/platform`) | Servicios de renta por plataforma |

---

## 3. Plataformas por categoría

| Categoría | Ejemplos de rutas |
|-----------|-------------------|
| **Banco** | `/bdet-bank`, `/bdet-accounts.html`, `/treasury-dashboard.html`, `/financial-center.html`, `/wallet`, `/forex`, `/vip-transactions` |
| **Gobierno** | `/platform/government-portal.html`, `/departments`, `/platform/leader-control.html`, `/platform/security-fortress.html` |
| **Logística** | `/logit`, `/logistics`, `/platform/logistics.html` |
| **Casino / Gaming** | `/casino`, `/platform/gaming-platform.html`, `/lotto`, `/raffle` |
| **Tokens** | `/tokens` (registro), `/platform/blockchain-platform.html` |
| **ATABEY / IA** | `/platform/atabey-platform.html`, `/atabey-notifications.html` |
| **Licencias** | `/license-authority.html`, `/license-audit.html` |
| **Monitoreo** | `/admin-monitoring.html?platform=casino|bdet|treasury|financial-center` |

---

## 4. APIs útiles (ver todo por API)

| API | Uso |
|-----|-----|
| `GET /api/v1/sovereignty/tokens` | Lista tokens (pre-launch, whitepaper) |
| `GET /api/v1/admin/monitoring?platform=` | Monitoreo por plataforma |
| `GET /api/v1/platform/rent?platform=` | Renta por plataforma |
| `GET /api/v1/production/status` | Estado producción (datos + endpoints) |
| `GET /api/v1/licenses/audit` | Auditoría licencias |
| `GET /api/v1/bdet/reports/monthly` | Reporte mensual BDET |
| `GET /api/ai-hub/health` | Health AI Hub (ATABEY, errores) |
| `GET /api/v1/rwa/parcels` | Registro RWA (parcelas) |
| `GET /api/v1/logistics/status` | Estado logística (Logit, regiones) |

---

## 5. Documentación clave

| Documento | Contenido |
|-----------|-----------|
| `docs/ESTADO-PLATAFORMAS-DEPARTAMENTOS-SERVICIOS.md` | Plataformas, departamentos, productos/servicios |
| `docs/QUE-MAS-NECESITAMOS.md` | Qué está hecho y qué falta |
| `docs/GO-LIVE-CHECKLIST.md` | Checklist producción (CORS, JWT, backup) |
| `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md` | BHBK, departamentos, servicios (ES) |
| `docs/BHBK-ARCHITECTURE-INDIGENOUS-CENTRAL-BANK.md` | BHBK Architecture — Indigenous Central Bank (EN) |
| `docs/ROTACION-SECRETS-JWT.md` | Rotación JWT sin downtime |
| `CHANGELOG.md` | Cambios recientes y versión API |

---

## 6. Scripts y datos

| Qué | Dónde |
|-----|--------|
| Backup datos críticos | `RuddieSolution/node/scripts/backup-node-data.js` |
| Backup por cron | `RuddieSolution/node/scripts/backup-cron.sh` |
| Caso de inversión en whitepapers | `scripts/add-caso-inversion-whitepapers.js` |
| Datos RWA | `RuddieSolution/node/data/rwa-parcels.json` |
| Config badges/nav plataforma | `RuddieSolution/platform/data/platform-links.json` |

---

## Resumen

- **Ver todo desde la UI:** entra a **`/platform`** y baja hasta la sección **Hub — Todo lo nuevo**, o usa el badge **⭐ TODO NUEVO** del header.
- **Ver todo desde el Node:** en **`/`** usa el botón **⭐ Todo lo nuevo**.
- **Ver todo en docs:** este archivo (`docs/DONDE-VER-TODO.md`) + `QUE-MAS-NECESITAMOS.md` y `ESTADO-PLATAFORMAS-DEPARTAMENTOS-SERVICIOS.md`.
