# Todo lo implementado hoy para el Node — Ciberseguridad 101 · Verticales · Clone

Resumen de lo pasado hoy al **Node** (backend + datos + API + UI).

---

## 1. Datos en el Node

| Archivo | Contenido |
|---------|-----------|
| **RuddieSolution/node/data/ciberseguridad-101.json** | Ciberseguridad 101 completo: `meta`, **`porPlataforma`** (orden por plataforma/departamento), `enlacesSentinelOne`, `pilares`, `categorias`, `documentos`, `checklist`, `verticales`, `clone`, `marketplace`, `identity`, `articulos`. **porPlataforma** agrupa los bloques en: Seguridad (Fortress · Guardian), Gobierno, Finanzas, Operaciones · Recuperación, Integraciones, Referencia. |

---

## 2. API en el Node

### Soberanía (server.js)

| Método y ruta | Descripción |
|---------------|-------------|
| **GET /api/v1/sovereignty/estado-final** | Estado final del sistema (módulos, declaración, validación, tactical, oneLove, systemStatus). Lee `data/estado-final-sistema.json`. |
| **GET /api/v1/sovereignty/ciberseguridad-101** | JSON completo de Ciberseguridad 101 (enlaces, pilares, categorías, documentos, checklist, verticales, clone). Cache ~120 s. |

### Ciberseguridad 101 (routes/ciberseguridad-api.js)

| Método y ruta | Descripción |
|---------------|-------------|
| **GET /api/v1/ciberseguridad/mapa** | Pilares See/Protect/Resolve + categorías 101↔stack. Para dashboards. Cache ~120 s. |
| **GET /api/v1/ciberseguridad/checklist** | Checklist estático + documentos (desde JSON). Cache 60 s. |
| **GET /api/v1/ciberseguridad/checklist-status** | Checklist con estado en vivo: pings a Baserow (:8080), Nextcloud (:8081), Nginx PM (:81); opcional `?host=127.0.0.1`. Lee `data/ciberseguridad-last-run.json` para backup/secretos/cloudData/smb. Sin cache. |
| **GET /api/v1/ciberseguridad/101** | Mismo payload completo que `/api/v1/sovereignty/ciberseguridad-101`. |
| **POST /api/v1/ciberseguridad/last-run** | Body: `{ "id": "backup"|"secretos"|"cloudData"|"smb", "lastRun": "ISO8601" }`. Actualiza última ejecución de scripts para el checklist-status. |
| **GET /api/v1/ciberseguridad/health** | Salud del módulo (existencia de data file). |

Ubicación en código: **RuddieSolution/node/server.js** (montaje de `routes/ciberseguridad-api.js` en `/api/v1/ciberseguridad`).

---

## 3. Verticales (Federal, Finance, Manufacturing)

- **Federal Government:** FedRAMP, NIST, on‑prem/air‑gap → mismo stack (Wazuh, backups, cifrado, estado-final-sistema.json).
- **Finance:** GLBA, PCI DSS, GDPR, recuperación → mismo stack (Vault, backups, secret scanning).
- **Manufacturing:** Uptime, “Stop threats. Keep producing.”, recuperación → mismo stack + **código clone**: CLONE-SETUP.md, `clone-repo.sh`, `clone-from-backup.sh`.

Todo esto está en **node/data/ciberseguridad-101.json** bajo `verticales` y `clone`.

---

## 4. Clone (recuperación) en el Node

En el JSON del node:

- **clone.doc:** docs/CLONE-SETUP.md  
- **clone.scripts:** clone-repo.sh, clone-from-backup.sh  
- **clone.flujo:** clonar repo → copiar backups → restaurar (clone-from-backup.sh) → docker-compose up -d  

La API **/api/v1/sovereignty/ciberseguridad-101** expone este bloque para la UI o otros consumidores.

---

## 5. Documentos de referencia (fuera del Node)

- **docs/CIBERSEGURIDAD-101.md** — Checklist único, enlaces SentinelOne, mapa 101↔stack.  
- **docs/SENTINELONE-VERTICALES-FEDERAL-FINANCE.md** — Federal, Finance, Manufacturing; sin duplicar checklist; clone = recuperación.  
- **docs/CLONE-SETUP.md** — Flujo clone (repo + backup).  
- **docs/SMB-CYBERSECURITY.md**, **SEGURIDAD-GOBIERNO-ESTATAL-Y-LOCAL.md**, **CLOUD-DATA-SECURITY.md** — Referencian checklist único en CIBERSEGURIDAD-101.

---

## 6. Cómo consumir desde la plataforma (UI)

- **Atabey Command Center** (`/platform/atabey-platform.html`): pestaña **Ciberseguridad 101** — puede usar `GET /api/v1/sovereignty/ciberseguridad-101` o `GET /api/v1/ciberseguridad/101` para el payload completo; `GET /api/v1/ciberseguridad/mapa` para pilares y categorías; `GET /api/v1/ciberseguridad/checklist-status` para el checklist con estado en vivo (censo/bóveda/gateway con pings; backup/secretos/cloudData/smb si se actualiza vía `POST /api/v1/ciberseguridad/last-run` desde cron o scripts).
- Cualquier cliente puede usar las rutas anteriores (JSON) para integrar mapa, checklist estático o checklist con estado.

---

*Resumen de lo implementado hoy para el Node — Ciberseguridad 101, verticales SentinelOne (Federal, Finance, Manufacturing) y código clone (recuperación).*
