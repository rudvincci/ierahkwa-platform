# 100% Production — Checklist definitivo plataforma

**IERAHKWA Futurehead — BDET Bank back · 1 Settlement + 4 Bancos Centrales · Regional / Nacional / Comercial**

Checklist único para dejar la plataforma **100% lista para producción**. Todo se administra desde **Ierahkwa Futurehead BDET Bank (back)**. Los nodos (1 Settlement + 4 centrales) tienen Regional, Nacional y Comercial separados por nodo y están **todos conectados entre sí**.

---

## ✅ 0. Un comando: preparar todo para READY

```bash
./scripts/prepare-ready.sh
```

Comprueba `.env`, archivos de datos (incl. `bank-registry.json`) y, si el Node está arriba, la verificación 100% Production Live. Si todo pasa → **READY**. Ver `docs/PREPARAR-READY.md`.

---

## ✅ 1. Pre-vuelo (antes de arrancar)

| # | Acción | Comprobación |
|---|--------|----------------|
| 1 | Crear `.env` desde ejemplo | `cp RuddieSolution/node/.env.example RuddieSolution/node/.env` |
| 2 | Rellenar secrets en `.env` | `NODE_ENV=production`, `JWT_ACCESS_SECRET`, `JWT_REFRESH_SECRET` (≥32 caracteres) |
| 3 | Archivos de datos en `node/data/` | Ver lista en sección 2; incluye `bank-registry.json` para BDET back |
| 4 | Puertos libres | 8545 (o 8500+8545–8548 si usas 4 regiones); 3001 (Banking Bridge); 3002 (Editor API) |

---

## ✅ 2. Archivos de datos requeridos (production)

En `RuddieSolution/node/data/`:

| Archivo | Uso |
|---------|-----|
| `resumen-soberano.json` | Sovereignty / resumen |
| `ciberseguridad-101.json` | Ciberseguridad 101 |
| `security-tools-recommended.json` | Herramientas seguridad |
| `formacion-datacamp.json` | Formación DataCamp |
| `ecosistema-futurehead.json` | Ecosistema Futurehead |
| `whitepaper-futurehead.json` | Whitepaper |
| `plan-implementacion-futurehead.json` | Plan implementación |
| `beneficios-empleados.json` | Beneficios empleados |
| `ofertas-corporativas.json` | Ofertas corporativas |
| **`bank-registry.json`** | **BDET back — registro de bancos (central, regional, nacional, comercial)** |
| `casino/promo-codes.json` | Casino promociones |
| `casino/plataforma-mundial.json` | Casino plataforma mundial |
| `casino/apuestas-deportivas-algoritmos.json` | Casino apuestas deportivas |

---

## ✅ 3. Arranque

**Opción A — Un solo nodo (desarrollo/MVP):**
```bash
./start.sh
```
Verificar Node en puerto configurado (por defecto 8545) y Banking Bridge (3001).

**Opción B — 1 Settlement + 4 Bancos Centrales (producción alta disponibilidad):**
```bash
cd RuddieSolution/node && pm2 start ecosystem.4regions.config.js
```
- 1 nodo International Settlement (8500)  
- 4 nodos bancos centrales: Águila (8545), Quetzal (8546), Cóndor (8547), Caribe (8548)  
- Banking Bridge (3001) y Editor API (3002)

Ver: `docs/CUATRO-NODOS-REGIONES.md`, `node/ecosystem.4regions.config.js`.

---

## ✅ 4. Endpoints de producción

| Endpoint | Uso |
|----------|-----|
| `GET /health` | Salud (incluye `role`, `region`, `port` si aplica) |
| `GET /ready` | Readiness (K8s/PM2) |
| `GET /live` | Liveness |
| `GET /api/v1/production/status` | Estado 100%: archivos de datos y endpoints críticos |
| `GET /api/v1/production/ready` | **200** solo si todos los datos presentes; **503** si falta alguno |
| `GET /api/v1/bdet/bank-registry` | Registro de bancos (BDET back) |
| `GET /api/v1/bdet/bank-status` | Estado del banco: registro OK + Banking Bridge alcanzable (`ok: true`) |

---

## ✅ 5. Verificación 100% Production Live

Desde la raíz del repo:

```bash
cd RuddieSolution/node && node scripts/verificar-production-live.js
```

Con servidor en otro host/puerto:

```bash
BASE_URL=https://api.tudominio.com node RuddieSolution/node/scripts/verificar-production-live.js
```

- **Exit 0:** todos los archivos de datos existen (incl. `bank-registry.json`) y todos los endpoints responden OK → **100% listo para producción**.
- **Exit 1:** hay fallos → corregir antes de ir a producción.

Usar en CI/CD o antes de cada despliegue.

---

## ✅ 6. Arquitectura producción (recordatorio)

| Elemento | Descripción |
|----------|-------------|
| **Admin** | Todo desde **Ierahkwa Futurehead BDET Bank (back)** |
| **1 nodo** | International Settlement (SIIS) — puerto 8500 |
| **4 nodos** | Bancos centrales: Águila, Quetzal, Cóndor, Caribe (8545–8548) |
| **Por nodo** | Regional, Nacional y Comercial **separados** (registros y lógica distintos) |
| **Entre nodos** | **Todos conectados entre sí** (Settlement ↔ 4 centrales; centrales ↔ entre ellos) |
| **Alta disponibilidad** | Si cae un nodo, los demás siguen; balanceador puede usar `/health` (role/region) |

---

## ✅ 7. Seguridad (ya en código / revisar en producción)

| Item | Estado |
|------|--------|
| Body limit (500kb) | ✅ `server.js` |
| Security headers (X-Content-Type-Options, Referrer-Policy, etc.) | ✅ `server.js` |
| Rate limiting (estándar, financial, ML, etc.) | ✅ middleware `rate-limit` |
| Helmet CSP | ✅ `server.js` |
| HTTPS/SSL | ⚠️ Configurar en producción (nginx/reverse proxy, Let's Encrypt) |
| Secrets en `.env` (no en código) | ✅ Requerido en pre-vuelo |

---

## ✅ 8. Checklist rápido go/no-go

- [ ] `.env` creado y rellenado (JWT_ACCESS_SECRET, JWT_REFRESH_SECRET ≥32 chars)
- [ ] Todos los archivos de datos en `node/data/` (incl. `bank-registry.json`)
- [ ] `./start.sh` o `pm2 start ecosystem.4regions.config.js` — servicios responden
- [ ] `cd RuddieSolution/node && node scripts/verificar-production-live.js` → **exit 0**
- [ ] BDET Bank (`platform/bdet-bank.html`) carga y puede leer registro de bancos
- [ ] (Opcional) HTTPS y dominio en producción; rate limit en login platform-auth estricto

Si todos los ítems están OK → **plataforma 100% para production**.

---

## Script único (ambas verificaciones)

Desde la raíz del repo: `./scripts/asegurar-100-production.sh` — exit 0 solo si pasan (1) links/rutas/data y (2) datos + endpoints. Ver `docs/ASEGURAR-100-PRODUCTION-CADA-PLATAFORMA.md`.

---

## Documentación relacionada

| Tema | Documento |
|------|-----------|
| Asegurar 100% en cada plataforma | `docs/ASEGURAR-100-PRODUCTION-CADA-PLATAFORMA.md` |
| Producción resumen | `docs/PRODUCTION-LISTO.md` |
| 100% Production Live (datos + endpoints) | `docs/100-PRODUCTION-LIVE.md` |
| 1 Settlement + 4 nodos, R/N/C, nodos conectados | `docs/CUATRO-NODOS-REGIONES.md` |
| Arquitectura BHBK, BDET back | `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md` |
| Bancos unificado, main admin | `docs/BANCOS-UNIFICADO-VS-MULTIPLE.md` |
| Checklist 24/7 (PM2, monitoring, etc.) | `docs/CHECKLIST-24-7-PRODUCCION.md` |

---

*Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister.*
