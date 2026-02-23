# Plano completo — Toda la plataforma y ATABEY

Referencia única: **dónde está cada cosa** y cómo se relaciona con **ATABEY** (centro de mando arriba de todo).

---

## 🌺 ATABEY — Arriba de todo

| Qué es | Dónde está |
|--------|-------------|
| **ATABEY** | `/platform/atabey-platform.html` — Una sola pantalla con todas las pestañas: Vista Global, AI, Fortress, Quantum, Telecom, Vigilancia, Chat·Video, Familia, Notificaciones, Backup, Cumplimiento, Node, etc. |
| **Acceso** | Solo rol **leader** / **superadmin**. Admin y ciudadano son redirigidos a admin.html o user-dashboard.html. |
| **Login líder** | `login.html` → usuario leader/superadmin → redirige a `atabey-platform.html`. |

**Desde ATABEY se llega a todo** (por pestañas e iframes). El resto de la plataforma son pantallas que también se pueden abrir desde el index/launchpad.

---

## Cuadrícula de botones (GOV, BANK, CHAT, VIDEO, etc.)

Origen de los botones: **`RuddieSolution/platform/data/platform-links.json`** (sección `version-badges`) y **`RuddieSolution/platform/config.json`** (`headerNav`, `primaryPlatforms`).  
Rutas relativas a la raíz del sitio (ej. Node en 8545 sirve `/platform/` y otras rutas).

| Botón | URL / Destino | Archivo / Ruta |
|-------|----------------|----------------|
| **GOV** | `/platform/government-portal.html` | RuddieSolution/platform/government-portal.html |
| **103 DEPTS** | `/departments` | departments (servicio/redirección) |
| **SOVEREIGNTY** | `/platform/sovereignty-education.html` | platform/sovereignty-education.html |
| **RUDDIE** | `/leader-control` o `/platform/leader-control.html` | platform/leader-control.html |
| **BANK** | `/bdet-bank` | bdet-bank (BDET Bank) |
| **SIIS** | `/platform/siis-settlement.html` | platform/siis-settlement.html |
| **4 BANKS** | `/central-banks` | central-banks |
| **RENT** | Servicios de renta | `/platform/servicios-renta.html` — platform/servicios-renta.html |
| **DEBTS** | `/platform/debt-collection.html` | platform/debt-collection.html |
| **FUTUREHEAD** | `/platform/futurehead-group.html` | platform/futurehead-group.html |
| **FORTRESS** | `/platform/security-fortress.html` | platform/security-fortress.html |
| **BACKUP** | `/platform/backup-department.html` | platform/backup-department.html |
| **SERVICES** | `/platform/services-platform.html` | platform/services-platform.html |
| **GAMING** | `/platform/gaming-platform.html` | platform/gaming-platform.html |
| **CHAT** | `/platform/secure-chat.html` | platform/secure-chat.html |
| **VIDEO** | `/platform/video-call.html` | platform/video-call.html |
| **ALERTS** | `/platform/notifications.html` | platform/notifications.html |
| **AMERICAS** | `/platform/americas-communication-platform.html` | platform/americas-communication-platform.html |
| **AI** | `/platform/ai-platform.html` | platform/ai-platform.html |
| **QUANTUM** | `/platform/quantum-platform.html` | platform/quantum-platform.html |
| **ATABEY** | `/platform/atabey-platform.html` | platform/atabey-platform.html |
| **APP STUDIO** | `/platform/app-studio.html` | platform/app-studio.html |
| **AI 24/7** | `/platform/support-ai.html` | platform/support-ai.html |
| **MEGA** | `/mega-dashboard.html` | RuddieSolution/node/public/mega-dashboard.html o ruta equivalente |
| **ADMIN** | `/platform/admin.html` | platform/admin.html |
| **HEALTH** | `/platform/health-dashboard.html` | platform/health-dashboard.html |
| **MONITOR** | `/platform/monitor.html` | platform/monitor.html |
| **TENANTS** | `/platform/tenant-dashboard.html` | platform/tenant-dashboard.html |
| **CONFIG** | `/platform/settings.html` | platform/settings.html |
| **LAUNCHPAD** | `/platform/citizen-launchpad.html` | platform/citizen-launchpad.html |
| **FAMILIA** | `/IERAHKWA-PLATFORM-DEPLOY/family-system/family-portal.html` | IERAHKWA-PLATFORM-DEPLOY/family-system/family-portal.html |

---

## Dónde se define la cuadrícula

| Origen | Archivo | Uso |
|--------|---------|-----|
| **Lista de enlaces (badges)** | `RuddieSolution/platform/data/platform-links.json` | Entradas con `section: "version-badges"` — muchos de los botones del launchpad/index. |
| **Config principal** | `RuddieSolution/platform/config.json` | `headerNav`, `primaryPlatforms`, `unifiedPlatforms`, `services` — navegación, dashboards y “abrir plataforma”. |
| **Página que muestra los botones** | `RuddieSolution/platform/index.html` | Dashboard/launchpad que puede usar platform-registry.js + platform-buttons.js o enlaces directos. |

Para **añadir o cambiar** un botón: editar `platform-links.json` (version-badges) o `config.json` (headerNav / primaryPlatforms) según qué vista use ese botón.

---

## Son para toda la plataforma

Los botones (GOV, 103 DEPTS, SOVEREIGNTY, RUDDIE, BANK, SIIS, 4 BANKS, RENT, DEBTS, FUTUREHEAD, FORTRESS, BACKUP, SERVICES, GAMING, CHAT, VIDEO, ALERTS, AMERICAS, AI, QUANTUM, ATABEY, APP STUDIO, AI 24/7, MEGA, ADMIN, HEALTH, MONITOR, TENANTS, CONFIG, LAUNCHPAD, etc.) **no están pegados en una sola página**: salen de **una sola fuente** y se usan en **toda la plataforma**.

| Dónde se muestran | Cómo |
|-------------------|------|
| **Index / Launchpad** | `index.html` → `<div id="headerNavContainer">` se rellena con JS usando `platform-registry.js` + `platform-buttons.js`. La lista viene de `config.json` (headerNav) o de `platform-links.json` (section headerNav / version-badges). |
| **Admin** | `admin.html` → panel "LINKS Y BOTONES" carga/edita `platform-links.json` (version-badges, headerNav, dashboard, quickActions, services). Lo que guardes afecta lo que ve toda la plataforma si el index/registry usa ese JSON. |
| **Otras páginas** | Cualquier página que use `unified-header.js` o `renderPlatformButtons(..., { style: 'header' })` muestra los mismos enlaces. |

**Fuente de verdad:** barra del header → `config.json` → `headerNav` y/o `platform-links.json` (section `headerNav`). Cuadrícula de badges → `platform-links.json` (section `version-badges`). Un solo cambio en esos archivos actualiza los botones en **toda** la plataforma.

---

## Rutas base

- **Plataforma web:** normalmente servida en `http://localhost:8545/platform/` (Node sirve estáticos desde `RuddieSolution/platform/` o similar).
- **BDET / Bank:** `/bdet-bank`, `/central-banks` — pueden ser otra ruta o servidor según configuración.
- **Leader control:** `/leader-control` o `/platform/leader-control.html`.
- **Family portal:** `/IERAHKWA-PLATFORM-DEPLOY/family-system/family-portal.html` (raíz del repo o ruta configurada en el servidor).

---

## Resumen: “abrir todo” y ATABEY

- **Una sola entrada para el líder:** **ATABEY** (`atabey-platform.html`). Desde ahí se accede por pestañas a: Vista Global, AI, Fortress, Quantum, Telecom, Vigilancia, Chat·Video, Familia, Notificaciones, Backup, Cumplimiento, Node, Servicio Inteligencia, etc.
- **Resto de la plataforma:** los mismos servicios aparecen como botones en el index/launchpad (GOV, BANK, CHAT, VIDEO, AMERICAS, etc.); cada botón tiene su URL en la tabla de arriba y en `platform-links.json` / `config.json`.
- **Para tener “todo” en un solo plano:** este documento es el plano completo; los archivos de configuración son la fuente de verdad para qué se muestra y a dónde apunta cada botón.

**Referencias:**  
- Lista maestra de plataformas: `docs/LISTA-MAESTRA-PLATAFORMAS.md`  
- Producción: `RuddieSolution/platform/PRODUCTION-SETUP.md`  
- Próximos pasos: `docs/PROXIMOS-PASOS-PRODUCCION.md`
