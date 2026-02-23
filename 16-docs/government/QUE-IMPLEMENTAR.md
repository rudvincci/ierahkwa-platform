# Qué más implementar — Plan priorizado IERAHKWA

**Fecha:** 2026-01  
**Referencias:** FALTANTES-PARA-PRODUCCION.md, ATABEY-QUE-FALTA.md, PLATAFORMAS-SUGERIDAS.md, REPORTE-ULTIMO-2026-01-28.md

---

## ✅ Ya implementado (esta sesión)

| Qué | Dónde |
|-----|-------|
| Persistencia bankers/ciudadanos/chats | `node/services/bridge-persistence.js` + banking-bridge load/scheduleSave |
| Backup automático agentes (cron diario) | `./scripts/install-backup-agentes-cron.sh` (03:00) |
| Identidad soberana | `platform/sovereign-identity.html` |
| Portal ciudadano único | `platform/citizen-portal.html` |
| Enlaces en plataforma | `platform/data/platform-links.json` (IDENTITY, CITIZEN) |

**Comandos:** `./scripts/install-backup-agentes-cron.sh` para activar backup diario. Python ML: `./scripts/install-python-ml.sh`.

---

## Rápido (1–3 días) — Quick wins

| # | Qué | Dónde / Cómo | Impacto |
|---|-----|--------------|---------|
| 1 | **Node 18+** | Actualizar entorno (nvm/Homebrew) para OpenAI y logger | IA y logs funcionan bien |
| 2 | **Persistencia bankers/ciudadanos** | ✅ **Hecho.** `node/services/bridge-persistence.js` + load/save en banking-bridge | No se pierde estado al reiniciar |
| 3 | **Python ML (8592)** | `./scripts/install-python-ml.sh` o `pip install -r services/python/requirements.txt` | Servicio fraud/risk activo |
| 4 | **Backup automático** | ✅ **Hecho.** `./scripts/install-backup-agentes-cron.sh` — cron diario 03:00 | Recuperación ante fallos |
| 5 | **Health unificado** | ~~Ya existe~~ `GET /api/health/all` en `node/services/health-monitor.js` (8545, 3001, 8590, 8591, 8592, 3000, 5071, etc.) | Dashboard: `/platform/health-dashboard.html` |

---

## Corto plazo (1–2 semanas)

| # | Qué | Dónde / Cómo | Impacto |
|---|-----|--------------|---------|
| 6 | **Atabey backend real** | ai-hub-server.js o rutas en server.js: chat con IA, workers, briefing | Atabey deja de ser solo UI |
| 7 | **Identidad soberana** | ✅ **Hecho.** `platform/sovereign-identity.html` + enlace en platform-links.json | Refuerza soberanía y KYC |
| 8 | **Portal ciudadano único** | ✅ **Hecho.** `platform/citizen-portal.html` + enlace en platform-links.json | Una sola puerta para el ciudadano |
| 9 | **API Gateway / Developer Portal** | Catálogo de APIs, docs, claves para socios (página + rutas en 8545) | Integraciones externas |
| 10 | **Calendario/Agenda en Atabey** | Eventos, reuniones, recordatorios del Ministro (Leader Control) | Uso diario del líder |

---

## Medio plazo (1–2 meses)

| # | Qué | Referencia | Impacto |
|---|-----|------------|---------|
| 11 | **MameyNode en Rust** | FALTANTES: nodo blockchain producción | Escala y velocidad |
| 12 | **Identity + ZKP** | Mamey.Government.Identity, Mamey.SICB.ZeroKnowledgeProofs | Privacidad y cumplimiento |
| 13 | **Tesorería SICB** | TreasuryDisbursements, TreasuryIssuances, TreasuryGovernanceAI | Banco central soberano |
| 14 | **SDKs oficiales** | MameyNode TypeScript/JavaScript/Python/Go | Integración unificada |
| 15 | **Cumplimiento tratados** | TreatyValidators, TreatyCompliantBudgetReports | Auditoría y tratados |

---

## Largo plazo (roadmap)

| # | Qué | Referencia |
|---|-----|------------|
| 16 | **SOC (centro de operaciones seguridad)** | PLATAFORMAS-SUGERIDAS: monitoreo, alertas, respuesta incidentes |
| 17 | **BC/DR (continuidad y recuperación)** | Backups, RTO/RPO, sitios secundarios |
| 18 | **Registro tierras y bienes** | Land & Assets Registry, soberanía territorial |
| 19 | **Atabey: voz y multi-idioma** | ATABEY-QUE-FALTA: voz, taíno, mohawk, español, inglés |
| 20 | **Maestro (orquestación agentes AI)** | FALTANTES: sagas, notificaciones, workflows |

---

## Dónde está cada cosa

- **Faltantes detallados:** `FALTANTES-PARA-PRODUCCION.md`
- **Atabey por implementar:** `platform/ATABEY-QUE-FALTA.md`
- **Nuevas plataformas sugeridas:** `platform/PLATAFORMAS-SUGERIDAS.md`
- **Pendientes último reporte:** `REPORTE-ULTIMO-2026-01-28.md` (backup agentes ya hecho; Node 18 y persistencia pendientes)

---

## Siguiente paso recomendado

1. **Rápido:** Implementar **persistencia de bankers/ciudadanos** en el bridge (guardar/cargar JSON) y **health unificado** (`/api/health/all`).  
2. **Luego:** **Identidad soberana** o **Portal ciudadano único** (elegir uno primero).  
3. **En paralelo:** Subir a **Node 18+** y dejar **Python ML** instalado y estable.

Si dices cuál quieres hacer primero (por ejemplo: “persistencia”, “health all”, “identidad soberana” o “portal ciudadano”), se puede bajar a tareas concretas y código en el repo.
