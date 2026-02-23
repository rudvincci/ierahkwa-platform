# Estado real: Plataforma para producción

**Fecha:** 23 enero 2026  
**Alcance:** Toda la plataforma IERAHKWA (Node, platform/, BDET, .NET, etc.)

---

## Resumen

```
╔══════════════════════════════════════════════════════════════════════════════╗
║  PRODUCCIÓN REAL (HTTPS, 24/7, monitoreo, backups DB, etc.)  →  NO LISTO    ║
║  FUNCIONA EN LOCAL (Node 8545, platform, BDET, links, rutas) →  SÍ          ║
╚══════════════════════════════════════════════════════════════════════════════╝
```

Los documentos que dicen "100% LIVE" o "PRODUCTION READY" se refieren a **un solo componente** (p. ej. NET10, `banking-bridge.js`), no a la plataforma completa. Este archivo aclara el estado real de **toda** la plataforma.

---

## ✅ Lo que SÍ está listo (local / desarrollo)

| Componente | Estado | Nota |
|------------|--------|------|
| **Node server (8545)** | ✅ | `node/server.js` sirve platform/, rutas cortas, APIs, BDET, Live Connect |
| **Platform HTML (50+)** | ✅ | `platform/*.html` accesibles vía `/platform/` o rutas tipo `/bdet-bank`, `/documents` |
| **Rutas cortas** | ✅ | `/bdet-bank`, `/leader-control`, `/central-banks`, `/departments`, `/documents`, `/gaming`, `/casino`, `/lotto`, `/raffle`, `/smartschool`, etc. |
| **BDET / banking** | ✅ | Páginas y APIs básicas; `banking-bridge.js` (si se usa) con sintaxis y carga OK |
| **NET10 (.NET)** | ✅ | Por sí mismo: build OK, health, 62 servicios; ver `NET10/PRODUCTION-READY.md` |
| **Backup de archivos** | ✅ | `backup-system/`, scripts de backup |

---

## ❌ Lo que NO está listo para producción real

### Crítico

| Falta | Dónde verlo |
|-------|-------------|
| **HTTPS / TLS** | Todo corre HTTP. En producción hace falta TLS (nginx/Cloudflare) y WSS para Live Connect. |
| **JWT / secrets en prod** | `.env` con `JWT_ACCESS_SECRET`, `JWT_REFRESH_SECRET`; no usar defaults. Ver `GO-LIVE-CHECKLIST.md`. |
| **PM2 (o similar) para todo** | Solo algunos servicios; `node/server.js` y dependencias deben estar en PM2/systemd con restart. Ver `docs/CHECKLIST-24-7-PRODUCCION.md`. |
| **Backup de bases de datos** | Hay backup de archivos; no hay scripts ni política para PostgreSQL/Redis u otras DB. |
| **Monitoreo y alertas** | Sin Prometheus/Grafana, UptimeRobot, PagerDuty, etc. |

### Infraestructura y seguridad (FALTANTES-PARA-PRODUCCION.md)

- MameyNode (Rust), MameyFramework, Identity biométrica, ZKP, SICB Treasury, SDKs oficiales: **no están en este repo**; son requisitos para un grado “gubernamental” completo.
- Sin ellos se puede hacer un **live limitado** con lo que hay (Node, platform, BDET, .NET que ya corren).

---

## Rutas del Node (server.js) que sí existen

Para que **botones y enlaces** de `platform/index.html` funcionen, el Node debe estar en 8545. Ejemplos de rutas que ya están definidas:

| Ruta | Archivo |
|------|---------|
| `/` | `node/public/index.html` |
| `/platform` | `platform/index.html` |
| `/platform/*` | static `platform/` |
| `/bdet-bank`, `/bdet` | `platform/bdet-bank.html` |
| `/leader-control` | `platform/leader-control.html` |
| `/central-banks`, `/4-banks` | `platform/central-banks.html` |
| `/departments`, `/103-departments`, `/depts` | `platform/departments.html` |
| `/documents` | `platform/documents.html` |
| `/project-hub` | `platform/project-hub.html` |
| `/spike-office` | `platform/spike-office.html` |
| `/social-media` | `platform/social-media.html` |
| `/gaming` | `platform/gaming-platform.html` (Casino, Lotto, Raffle) |
| `/casino`, `/lotto`, `/raffle` | `platform/casino.html`, `lotto.html`, `raffle.html` |
| `/smartschool` | `platform/smartschool.html` |
| `/vip-transactions`, `/vip` | `platform/vip-transactions.html` |
| `/forex`, `/wallet` | `platform/forex.html`, `wallet.html` |
| `/commerce-business-dashboard.html` | raíz `commerce-business-dashboard.html` |
| `/mega-dashboard`, `/live-connect` | `node/public/` |

---

## Cómo dejar de decir “100% production” por error

- **PRODUCTION-100-STATUS.md**: solo `banking-bridge.js`. En el texto se aclara que el “live 100%” depende de levantar servicios y entornos.
- **PRODUCTION-READY.md** (en NET10): solo el subsistema **NET10**; no la plataforma entera.
- **FALTANTES-PARA-PRODUCCION.md** y **GO-LIVE-CHECKLIST.md**: sí hablan de la plataforma; usarlos como referencia de qué falta.
- **docs/CHECKLIST-24-7-PRODUCCION.md**: checklist detallado 24/7 (PM2, health, logs, SSL, backup DB, etc.).

---

## Mínimo para un “pre-live” controlado

1. `cd node && npm install` y crear `node/.env` desde `node/.env.example` con JWT secrets y, si aplica, `PORT`, URLs de Rust/Go/Python.
2. Levantar Node: `node node/server.js` (o vía `go-live.sh` si se usan Rust/Go/Python).
3. Abrir `http://localhost:8545/platform` y comprobar que los botones (BANK, Documents, Gaming, etc.) llevan a las rutas indicadas arriba.
4. Para acercarse a producción real: seguir **GO-LIVE-CHECKLIST.md** y **docs/CHECKLIST-24-7-PRODUCCION.md** (HTTPS, PM2, backups DB, monitoreo).

---

*Documento de estado. No reemplaza a FALTANTES-PARA-PRODUCCION.md ni a GO-LIVE-CHECKLIST.md.*
