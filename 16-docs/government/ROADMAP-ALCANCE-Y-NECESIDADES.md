# Roadmap: Alcance y necesidades por ámbito

Checklist accionable a partir del reporte *Por qué es mejor, hasta dónde podemos ir y qué más podemos y necesitamos*.  
Actualizar este documento según se complete cada ítem.

---

## 1. Alcance (mismo diseño, más alcance)

| Objetivo | Estado | Notas |
|----------|--------|--------|
| Más dominios bajo ATABEY (nuevas verticales) | ⬜ Pendiente | Añadir a platform-links y vista ATABEY |
| Más ciudadanos (escala de usuarios) | ⬜ Pendiente | Auth, rate limits, BD si aplica |
| Más regiones (multi-territorio) | ⬜ Pendiente | i18n, zonas, permisos por región |
| Más idiomas (i18n completo) | ⬜ Pendiente | Ampliar `/api/v1/i18n` y strings en front |

---

## 2. Integración Mamey / SICB

| Capa | Estado | Necesitamos |
|------|--------|-------------|
| Banco central (SICB) | ⬜ Pendiente | Mamey.SICB.* (Tesorería, Emisión, Overrides, Governance AI) |
| Identidad biométrica | ⬜ Pendiente | Mamey.FWID.Identities, FutureWampumID |
| ZKP (privacidad) | ⬜ Pendiente | Mamey.SICB.ZeroKnowledgeProofs |
| Tratados (validación, reportes) | ⬜ Pendiente | TreatyValidators, TreatyCompliantBudgetReports, oráculos |
| Todo bajo el mismo techo ATABEY | 🔄 Stubs listos | Rutas `/api/v1/sicb/*` y `/api/v1/mamey/*` (501 hasta integración real); ATABEY y Node listos para conectar |

---

## 3. Reconocimiento y tratados internacionales

| Objetivo | Estado | Notas |
|----------|--------|--------|
| Documentación de arquitectura soberana | ⬜ Pendiente | Reporte + PLANO ya existen; versión “oficial” si se requiere |
| Modelo con control total (datos, identidad, cumplimiento) | ✅ Diseñado | Mantener y reforzar con SICB/cumplimiento |
| Acuerdos entre naciones | ⬜ Pendiente | Depende de SICB integrado y documentación |

---

## 4. Límites y mitigación

| Límite | Mitigación | Estado |
|--------|------------|--------|
| Rendimiento | MameyNode (Rust) + servicios .NET | ⬜ Pendiente (ver FALTANTES-PARA-PRODUCCION) |
| Disponibilidad | Réplicas, balanceador, DR | ⬜ Pendiente (plan en PRODUCTION-SETUP) |
| Complejidad | Documentación y SDKs unificados | ⬜ En curso (docs); SDKs pendientes |

---

## 5. Seguridad

| Podemos hacer | Necesitamos | Estado |
|---------------|-------------|--------|
| Respuesta a incidentes (playbooks, escalado) | Playbooks definidos, UI en ATABEY | ✅ Hecho — `docs/PLAYBOOK-RESPUESTA-INCIDENTES.md`, `platform/incidentes-dr.html`, enlace en ATABEY |
| DR (recuperación ante desastres) | Plan DR escrito, sitio secundario, pruebas | ✅ Hecho — `docs/CHECKLIST-DR.md`, sección en `incidentes-dr.html` |
| Formación en seguridad | Materiales, concienciación | ✅ Hecho — `docs/CONCIENCIACION-SEGURIDAD.md` |

---

## 6. Infra / Tech

| Podemos hacer | Necesitamos | Estado |
|---------------|-------------|--------|
| Producción estable (Node + proxy) | .env completo, CORS_ORIGIN, HTTPS | ⬜ Ver PRODUCTION-SETUP |
| Backups automáticos | Cron en servidor, scripts existentes | ✅ Hecho — `scripts/install-cron-production.sh` (backup + health cron) |
| Logs y rotación | LOG_DIR, logrotate o módulo Node | ✅ Hecho — `docs/logrotate-ierahkwa.example` |
| MameyNode / componentes Mamey | Integrar cuando estén listos | ⬜ Pendiente |
| SDKs oficiales (TS, JS, Python, Go) | Desarrollar o adoptar | ⬜ Pendiente |

---

## 7. Negocio / Dominios

| Podemos hacer | Necesitamos | Estado |
|---------------|-------------|--------|
| Más verticales bajo ATABEY | Lista maestra (platform-links), vistas | ✅ Hecho — `docs/LISTA-MAESTRA-PLATAFORMAS.md` + platform-links.json |
| Monetización (renta, licencias) | Métricas, cobros, dashboards | ✅ Hecho — Vista Ingresos en ATABEY; cobros recurrentes: `/api/v1/recurring` (subscriptions, run-due), cron opcional en `install-cron-production.sh` |
| i18n completo | Lista de idiomas, strings por pantalla | ✅ Hecho — `api/v1/i18n` ampliado (en, es, moh, tai) con keys: search_placeholder, favorites, theme, login, atabey, backup, security, income, whistleblower, compliance, recurring, kyc, etc. |

---

## 8. Cumplimiento / Legal

| Podemos hacer | Necesitamos | Estado |
|---------------|-------------|--------|
| KYC/AML centralizado | Reforzar CitizenCRM + KYC actual | ✅ Hecho — API `/api/v1/kyc` (status, submit, pending, approve, reject); almacenamiento file-based en `node/data/kyc-records.json` |
| ZKP para privacidad | Mamey.SICB.ZeroKnowledgeProofs | 🔄 Placeholder — módulo `zkp-privacy.js`, `/api/v1/zkp/status`; ver `docs/ZKP-PRIVACY.md` |
| Validación de tratados | TreatyValidators, reportes SICB | ⬜ Pendiente — stubs en `/api/v1/mamey/treaties/*` |
| Canal de denuncias (Whistleblower) | Mamey.SICB.WhistleblowerReports o módulo propio | ✅ Hecho — módulo propio `whistleblower.js`, `/api/v1/whistleblower/report`, `/reports`; UI `platform/whistleblower.html`; datos en `node/data/whistleblower-reports.json` |
| Auditoría y trazabilidad | Logs centralizados, auditoría en KMS | ✅ Hecho — `docs/AUDITORIA-TRAZABILIDAD.md` (eventos, logs, KMS, fuentes) |

---

## 9. AI / Datos

| Podemos hacer | Necesitamos | Estado |
|---------------|-------------|--------|
| AI soberana (Ollama/local) en todo | Mantener ai-soberano, AI Platform, Support AI | ✅ En marcha |
| Briefing unificado para el líder | Más fuentes en `/api/v1/atabey/status` y AI Hub | 🔄 En curso — backup, vigilancia, emergencias en API y Vista Global ATABEY |
| “Todo propio” en datos | Revisar cada nuevo módulo (sin enviar a terceros) | ⬜ Continuo |
| AI para tesorería/riesgo | TreasuryGovernanceAIAdvisors cuando exista | ⬜ Pendiente |

---

## 10. Gobernanza

| Podemos hacer | Necesitamos | Estado |
|---------------|-------------|--------|
| Roles claros (Front / Back / Leader) | Ya definidos; ampliar si hace falta | ✅ Hecho |
| Alertas si algo cae | Health/atabey/status en proxy, script de alerta | ✅ Hecho — `scripts/health-alert-check.sh` (cron: exit 1 = alertar) |
| Operación 24/7 | Monitoreo + turnos o automatización | ✅ Hecho — `docs/RUNBOOK-24-7.md` (qué revisar, qué hacer si falla, checklist) |

---

## 11. Personas

| Podemos hacer | Necesitamos | Estado |
|---------------|-------------|--------|
| Documentación | Reporte, PLANO, PRODUCTION-SETUP, FALTANTES | ✅ Hecho |
| Concienciación en seguridad | Guías, materiales, sección en ATABEY | ⬜ Pendiente |
| Onboarding por rol | Checklist Front/Back/Leader | ✅ Hecho — `docs/ONBOARDING-POR-ROL.md` |

---

## 12. Internacional

| Podemos hacer | Necesitamos | Estado |
|---------------|-------------|--------|
| Tratados y SICB | SICB integrado, documentación de arquitectura | ⬜ Pendiente |
| Posición soberana auditable | Documentación + cumplimiento (SICB/tratados) | ⬜ En curso |

---

## Leyenda de estados

- ⬜ Pendiente  
- 🔄 En curso  
- ✅ Hecho / diseñado  

**Referencias:**  
- Visión y detalle: `docs/REPORTE-POR-QUE-ES-MEJOR-Y-HASTA-DONDE.md`  
- Producción: `RuddieSolution/platform/PRODUCTION-SETUP.md`  
- Faltantes técnicos: `FALTANTES-PARA-PRODUCCION.md`  
- Estructura: `RuddieSolution/platform/PLANO-ATABEY-ARRIBA-DE-TODO.md`
