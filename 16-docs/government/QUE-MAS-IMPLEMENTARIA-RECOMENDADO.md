# Qué más implementaría — Recomendado

**Gobierno Soberano de Ierahkwa Ne Kanienke**  
Recomendaciones priorizadas, alineadas con: 109 tokens, negocios separados, backend compartido, production 100%, principio todo propio.

---

## Resumen rápido

| Prioridad | Enfoque | Ejemplos |
|-----------|---------|----------|
| **Alta** | Tokens + producción | Caso de inversión por token, API lista tokens, 404 y smoke test |
| **Media** | Dashboard + seguridad | Estado por plataforma en main dashboard, CORS/JWT en prod, backup state |
| **Baja** | IA + DX | Notificaciones Atabey, formularios BDET reales, PWA, i18n |

---

## 1. Tokens y negocios (109 tokens)

| # | Qué | Dónde / Cómo | Impacto |
|---|-----|--------------|---------|
| 1 | **Caso de inversión en cada whitepaper** | La plantilla está en `docs/PLAN-INVERSION-ATRACTIVO-POR-TOKEN.md`. Opción: script que añada la sección "Caso de inversión" (esqueleto) en cada `whitepaper-es.md` a partir de `token.json` (valor, demanda, cómo gana valor, roadmap, riesgos). | Inversionistas ven por qué invertir en cada token. |
| 2 | **Card "Por qué invertir" en cada pre-launch** | En cada `tokens/NN-IGT-XXX/index.html`: bloque con 3–5 bullets (respaldo BDET, uso real, staking, liquidez TradeX) + CTA a whitepaper. Plantilla en el plan de inversión. | Pre-launch más convincente sin mezclar negocios. |
| 3 | **API lista de tokens (pre-launch + whitepaper)** | `GET /api/v1/sovereignty/tokens`: lista con `symbol`, `name`, `category`, `preLaunchUrl`, `whitepaperUrl`, `hasWhitepaper`. **Hecho:** en `server.js`; consumir desde dashboard y cumplimiento. | Un solo backend; cada negocio sigue con su página. ✅ |
| 4 | **Idiomas EN/FR/MOH por token (opcional)** | Script que genere `whitepaper-en.md`, `whitepaper-fr.md`, `whitepaper-moh.md` desde `whitepaper-es.md` (traducción manual o plantilla igual por idioma). | Coherencia con los 41 tokens que ya tienen 4 idiomas. |

---

## 2. Main dashboard y producción

| # | Qué | Dónde / Cómo | Impacto |
|---|-----|--------------|---------|
| 5 | **Estado por plataforma en el main dashboard** | Sección "Estado por plataforma" que consuma `GET /api/v1/production/status` y muestre: datos OK, endpoints OK, enlace a cada servicio/negocio (sin mezclar contenido; solo enlaces y estado). | Un solo lugar donde "se mezcla todo"; profesional. |
| 6 | **404 amigable** | `platform/404.html` con enlaces a index, BDET, TradeX, tokens, departamentos. **Hecho:** `RuddieSolution/platform/404.html`; Node ya sirve 404 con esta página. | Mejor experiencia y recuperación. ✅ |
| 7 | **Smoke test antes de producción** | Script que haga curl a `/health`, `/ready`, `/api/v1/production/ready`, `/api/v1/bdet/bank-registry`. **Hecho:** `./scripts/smoke-test.sh [BASE_URL]` — exit 0 solo si todos 200. | Confianza al desplegar. ✅ |
| 8 | **Verificar puertos antes de arrancar** | En `start.sh`: comprobar 8545 y 3001; mensaje claro si hay conflicto. **Hecho:** bloque 0b en `start.sh`; si `CHECK_PORTS=1` y puerto en uso, exit 1. | Evita fallos silenciosos. ✅ |

---

## 3. Seguridad y resiliencia

| # | Qué | Dónde / Cómo | Impacto |
|---|-----|--------------|---------|
| 9 | **CORS y JWT en producción** | `.env`: `CORS_ORIGIN=https://tudominio.gov`, `NODE_ENV=production`, JWT secrets ≥32 caracteres. Documentar en GO-LIVE-CHECKLIST. | Seguridad real en producción. |
| 10 | **Backup automático de state y data crítico** | Script (cron o post-start) que guarde `node/data/` (o subset: bank-registry, bdet-bank, bridge-persistence) y, si aplica, clave/state KMS en backup encriptado. Ver ALGO-MAS-PARA-IMPLEMENTAR. | Recuperación ante fallos. |
| 11 | **Rotación de secrets** | Doc corto: cómo rotar `JWT_ACCESS_SECRET` / `JWT_REFRESH_SECRET` sin downtime (doble clave, ventana, reinicio). | Operación segura a largo plazo. |

---

## 4. IA, Atabey y BDET

| # | Qué | Dónde / Cómo | Impacto |
|---|-----|--------------|---------|
| 12 | **Notificaciones en ATABEY** | Alertas cuando: precio, préstamo, KYC, fraude. Preferencias en `atabey/preferences.json`; envío por canal propio (sin terceros). Ver QUE-MAS-HACEMOS-ROADMAP. | Uso diario del líder. |
| 13 | **Formularios reales en BDET Bank** | En BDET: "Crear cuenta", "Solicitar préstamo", "Depósito" que llamen a APIs existentes y muestren resultado. | Uso directo sin depender de Postman. |
| 14 | **Reporte mensual automático BDET** | `generateMonthlyReport()`: consolidar transacciones, préstamos, cuentas; guardar en `reports.json`; `GET /api/bdet/reports/monthly`. | Operación y gobierno. |
| 15 | **Health público del AI Hub** | Página o ruta de solo lectura (sin login): estado ATABEY, BDET AI, World Intel, últimos errores. `GET /api/ai-hub/health`. | Visibilidad sin entrar al dashboard. |

---

## 5. Integración y DX

| # | Qué | Dónde / Cómo | Impacto |
|---|-----|--------------|---------|
| 16 | **Developer portal / catálogo de APIs** | Página (ej. `/platform/developer-portal.html`) que liste APIs soberanas (sovereignty, production, bdet, tokens, casino) con método, ruta y descripción breve. Sin claves de terceros; solo nuestras APIs. | Integraciones propias y partners. |
| 17 | **PWA del main dashboard** | Manifest + service worker para `platform/index.html`: offline básico, "Añadir a pantalla de inicio". Todo propio. | Acceso desde móvil. |
| 18 | **Changelog y versión de API** | `CHANGELOG.md` en raíz; cuando se rompa compatibilidad, prefijo `/api/v2` y doc de migración. | Mantenimiento y partners. |

---

## 6. BHBK y RWA (medio plazo)

| # | Qué | Dónde / Cómo | Impacto |
|---|-----|--------------|---------|
| 19 | **Registro de tierras/activos (RWA)** | Alineado con BHBK Activos Reales: módulo o API para registro de parcelas/colateral; integración con BDET y préstamos agrícolas. Ver ARQUITECTURA-BHBK-DEPARTAMENTOS. | Banco central indígena completo. |
| 20 | **Panel de compliance en BDET** | Vista: transacciones en revisión (AML), KYC pendientes, alertas de fraude; acciones aprobar/rechazar que llamen al backend. Ver QUE-MAS-HACEMOS-ROADMAP. | Cumplimiento y auditoría. |

---

## Orden sugerido (si priorizas poco)

1. **API lista de tokens** (1–2 días) — un endpoint, dashboard y cumplimiento.
2. **Smoke test + verificar puertos en start** (medio día) — producción más segura.
3. **404 amigable** (medio día) — ya está en roadmap.
4. **Caso de inversión en whitepapers** — script que añada la sección en cada token.
5. **Estado por plataforma en main dashboard** — consumir `/api/v1/production/status` y enlaces.

---

## Referencias

| Tema | Documento |
|------|------------|
| Plan tokens y whitepaper | `docs/PLAN-NEGOCIOS-TOKENS-PRE-LAUNCH-WHITEPAPER.md` |
| Atractivo inversión por token | `docs/PLAN-INVERSION-ATRACTIVO-POR-TOKEN.md` |
| Negocios separados, dashboard único | `docs/PRINCIPIO-NEGOCIOS-SEPARADOS-DASHBOARD-UNICO.md` |
| Producción 100% | `docs/PLATAFORMA-100-PRODUCTION.md`, `docs/ASEGURAR-100-PRODUCTION-CADA-PLATAFORMA.md` |
| Qué implementar (existente) | `QUE-IMPLEMENTAR.md`, `ALGO-MAS-PARA-IMPLEMENTAR.md` |
| Roadmap AI / Atabey | `docs/QUE-MAS-HACEMOS-ROADMAP.md` |
| Roadmap uno a uno | `ROADMAP-IMPLEMENTACION-UNO-A-UNO.md` |
| Arquitectura BHBK | `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md` |

---

*Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister.*
