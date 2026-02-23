# Roadmap – Implementación uno a uno

**Objetivo:** Completar todo lo implementable en el repo, bloque por bloque.

**Criterio:** Cada ítem es autocontenido y se puede marcar como hecho.

---

## FASE 1 – Conectar UI con backend (APIs existentes)

| # | Item | Archivos | Estado |
|---|------|----------|--------|
| 1 | DAO Governance: UI consume `/api/v1/dao/proposals` y muestra datos reales | dao-governance.html, server.js | ✅ |
| 2 | DAO: persistencia en JSON (proposals, votes) | server.js, data/dao-proposals.json | ✅ |
| 3 | DAO: colores alineados con tema soberano (gold #FFD700, cyan, etc.) | dao-governance.html | ✅ |
| 4 | DAO: crear propuesta desde UI (POST) | dao-governance.html, server.js | ✅ |
| 5 | DAO: votar desde UI (POST /api/v1/dao/votes) | dao-governance.html, server.js | ✅ |

---

## FASE 2 – Persistencia Banking / Central Banks

| # | Item | Archivos | Estado |
|---|------|----------|--------|
| 6 | Banking Bridge: guardar `CENTRAL_BANK_CONNECTIONS` en JSON | banking-bridge.js, data/ | ✅ |
| 7 | Banking Bridge: cargar datos al iniciar | banking-bridge.js | ✅ |
| 8 | Correspondent relationships: persistencia | banking-bridge.js, data/ | ⬜ |

---

## FASE 3 – Seguridad y health

| # | Item | Archivos | Estado |
|---|------|----------|--------|
| 9 | Health unificado: `/health/all` lista Node, Bridge, Editor, .NET | server.js | ✅ |
| 10 | Dashboard status: mostrar estado de cada servicio | monitor.html o health-dashboard | ⬜ |
| 11 | Auth middleware: proteger `/api/v1/dao/*` con token básico | server.js, middleware | ✅ |
| 12 | Rate limit ya aplicado (verificar que funciona) | rate-limit.js | ✅ |

---

## FASE 4 – Integración y consistencia

| # | Item | Archivos | Estado |
|---|------|----------|--------|
| 13 | Nav/links: todas las plataformas accesibles desde index | index.html, config.json | ✅ |
| 14 | 404 amigable con links a plataformas principales | 404.html | ⬜ |
| 15 | Variables de entorno documentadas (.env.example completo) | node/.env.example | ✅ |
| 16 | Script de arranque: verificar puertos antes de levantar | start.sh | ⬜ |

---

## FASE 5 – Documentación y QA

| # | Item | Archivos | Estado |
|---|------|----------|--------|
| 17 | README actualizado con estado real (qué funciona, qué falta) | README.md | ⬜ |
| 18 | Lista de endpoints y servicios con puertos | docs/ENDPOINTS-SERVICIOS.md | ⬜ |
| 19 | Smoke test: curl a /health, /api/central-bank/list, etc. | scripts/smoke-test.sh | ⬜ |

---

## Orden de ejecución recomendado

1. **#1** – DAO UI consume API (rápido, sin persistencia aún)  
2. **#2** – DAO persistencia JSON  
3. **#3** – DAO colores soberanos  
4. **#4–5** – DAO crear/votar  
5. **#6–8** – Banking persistencia  
6. **#9–10** – Health unificado  
7. **#11** – Auth básico  
8. Resto según prioridad

---

**© 2026 Sovereign Government of Ierahkwa Ne Kanienke**
