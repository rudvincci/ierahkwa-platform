# Qué más implementamos — Lista priorizada

**Sovereign Government of Ierahkwa Ne Kanienke**  
Lista única de próximas implementaciones, alineada con: Blockchage/Logit/deliveries, 109+ tokens, BHBK, principio todo propio.

---

## ✅ Reciente (ya hecho)

- Blockchage, mall digital, Logit (docs + tokens 104–109).
- Tres tokens por modo: IGT-DELIVERY-SEA (bote), IGT-DELIVERY-AIR (avión), IGT-DELIVERY-LAND (carros).
- IGT-DELIVERY-LOC, IGT-DELIVERY-INT (local/internacional).
- Integración exchange/trading (banking-bridge, TradeX, ierahkwa-shop).
- Whitepapers EN/ES + páginas (index.html) para 104–109.
- Token Registry: categoría Platform & Logistics (104–109).

---

## Alta prioridad (rápido impacto)

| # | Qué implementar | Dónde / Cómo | Impacto |
|---|------------------|--------------|---------|
| 1 | **Estado por plataforma en main dashboard** | Sección que consuma `GET /api/v1/production/status`: datos OK, endpoints OK, enlace a cada servicio (BDET, TradeX, Logit, tokens). Solo enlaces y estado, sin mezclar contenido. | Un solo lugar donde ver todo; profesional. |
| 2 | **Caso de inversión en whitepapers** | Añadir sección "Caso de inversión" (plantilla `docs/PLAN-INVERSION-ATRACTIVO-POR-TOKEN.md`) en whitepaper-es.md de tokens clave (104–109 y otros). Valor, demanda, cómo gana valor, riesgos. | Inversionistas ven por qué invertir. |
| 3 | **API lista de tokens (pre-launch + whitepaper)** | Confirmar/consumir `GET /api/v1/sovereignty/tokens`: symbol, name, preLaunchUrl, whitepaperUrl, hasWhitepaper. Usar en dashboard y cumplimiento. | Un backend; cada negocio con su página. |
| 4 | **CORS y JWT en producción** | `.env`: `CORS_ORIGIN`, `NODE_ENV=production`, JWT secrets ≥32 caracteres. Documentar en GO-LIVE-CHECKLIST. | Seguridad real en producción. |
| 5 | **Backup automático de state y data crítico** | Script (cron o post-start) que guarde `node/data/` (bank-registry, logistics-manifests, bridge-persistence) en backup encriptado. Ver ALGO-MAS-PARA-IMPLEMENTAR. | Recuperación ante fallos. |

---

## Media prioridad (dashboard, BDET, logística)

| # | Qué implementar | Dónde / Cómo | Impacto |
|---|------------------|--------------|---------|
| 6 | **Formularios reales en BDET Bank** | En BDET: "Crear cuenta", "Solicitar préstamo", "Depósito" que llamen a APIs existentes y muestren resultado. | Uso directo sin Postman. |
| 7 | **Panel de compliance en BDET** | Vista: transacciones en revisión (AML), KYC pendientes, alertas de fraude; acciones aprobar/rechazar vía backend. | Cumplimiento y auditoría. |
| 8 | **Notificaciones en ATABEY** | Alertas (precio, préstamo, KYC, fraude). Preferencias en atabey; envío por canal propio (sin terceros). | Uso diario del líder. |
| 9 | **Developer portal / catálogo de APIs** | Página `/platform/developer-portal.html`: listar APIs soberanas (sovereignty, production, bdet, tokens, logistics, casino) con método, ruta y descripción. Solo nuestras APIs. | Integraciones propias y partners. |
| 10 | **Registro RWA (tierras/activos)** | Módulo o API para registro de parcelas/colateral; integración con BDET y préstamos agrícolas. Alineado con BHBK Activos Reales. | Banco central indígena completo. |

---

## Baja prioridad (mejoras UX y operación)

| # | Qué implementar | Dónde / Cómo | Impacto |
|---|------------------|--------------|---------|
| 11 | **PWA del main dashboard** | Manifest + service worker para `platform/index.html`: offline básico, "Añadir a pantalla de inicio". Todo propio. | Acceso desde móvil. |
| 12 | **Reporte mensual automático BDET** | Consolidar transacciones, préstamos, cuentas; guardar en reports; `GET /api/bdet/reports/monthly`. | Operación y gobierno. |
| 13 | **Health público del AI Hub** | Ruta de solo lectura: estado ATABEY, BDET AI, últimos errores. `GET /api/ai-hub/health`. | Visibilidad sin login. |
| 14 | **Rotación de secrets** | Doc corto: cómo rotar JWT_ACCESS_SECRET / JWT_REFRESH_SECRET sin downtime. | Operación segura a largo plazo. |
| 15 | **Card "Por qué invertir" en pre-launch** | En cada token (ej. 104–109): bloque 3–5 bullets + CTA a whitepaper en index.html. | Pre-launch más convincente. |

---

## Logística y Blockchage (refuerzo)

| # | Qué implementar | Dónde / Cómo | Impacto |
|---|------------------|--------------|---------|
| 16 | **Liquidación por token de delivery** | En sovereign-logistics: al cobrar tarifa o recompensa, opción de usar IGT-DELIVERY-SEA/AIR/LAND según transportMode del envío (además de LOGi). | Coherencia con los 3 tokens por modo. |
| 17 | **Panel Logit en platform** | Página `platform/logit.html` o sección en logistics.html: puntos Logit por región, estado, cash pickup, seguros. Consumir `/api/v1/logistics/status` y transport-modes. | Visibilidad operativa de la franquicia. |
| 18 | **Blockchage: enlace desde mail/postal** | En IGT-PSI e IGT-MAIL (páginas o docs): enlace a Blockchage (104) como correo 100% digital. | Un solo relato: correo = Blockchage. |

---

## Orden sugerido (si priorizas poco)

1. **Estado por plataforma en main dashboard** (1–2 días).
2. **Caso de inversión en whitepapers** (script o manual para 104–109 y principales).
3. **CORS/JWT en producción** (medio día + doc).
4. **Backup automático** (script + cron).
5. **Formularios reales BDET** o **Developer portal** (elegir uno).

---

## Referencias

| Tema | Documento |
|------|-----------|
| Recomendado (detalle) | `docs/QUE-MAS-IMPLEMENTARIA-RECOMENDADO.md` |
| Plan general | `QUE-IMPLEMENTAR.md`, `ALGO-MAS-PARA-IMPLEMENTAR.md` |
| BHBK / RWA | `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md` |
| Blockchage / Logit | `docs/BLOCKCHAGE-LOGIT-MALL-DIGITAL.md` |
| Tokens pre-launch | `docs/PLAN-NEGOCIOS-TOKENS-PRE-LAUNCH-WHITEPAPER.md` |
| Inversión por token | `docs/PLAN-INVERSION-ATRACTIVO-POR-TOKEN.md` |

---

*Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister.*
