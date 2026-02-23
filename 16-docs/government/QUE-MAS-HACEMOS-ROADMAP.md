# 🌺 ¿Qué más hacemos? — Roadmap y opciones

**Plataforma AI Ierahkwa** — Próximos pasos posibles, ordenados por impacto y esfuerzo.

---

## 🟢 Rápido (1–2 días)

| # | Idea | Descripción | Dónde |
|---|------|-------------|--------|
| 1 | **Notificaciones en ATABEY** | Alertas cuando: alerta de precio se dispara, préstamo aprobado/rechazado, KYC listo, fraude bloqueado. Guardar preferencias en `atabey/preferences.json` y enviar (email o push). | `atabey-system.js`, dashboard |
| 2 | **Formularios en BDET Bank** | En el panel AI Banker BDET: formularios reales “Crear cuenta”, “Solicitar préstamo”, “Depósito” que llamen a `/api/ai-hub/bdet/...` y muestren resultado. | `platform/bdet-bank.html` |
| 3 | **Reporte mensual automático** | En AI Banker BDET: `generateMonthlyReport()` que consolide transacciones, préstamos, cuentas nuevas; guardar en `reports.json` y endpoint `GET /bdet/reports/monthly`. | `ai-banker-bdet.js`, index |
| 4 | **Idioma Kanien’kéha en ATABEY** | Añadir comandos/respuestas en Kanien’kéha (ej. “Taíno ti”, “Guaitiao”) y opción en preferencias: `language: 'es' | 'kane'`. | `atabey-system.js`, preferences |
| 5 | **Health público del AI Hub** | Página `/platform/ai-hub-status.html` (o ruta que elijas) que muestre: ATABEY activa, BDET activo, World Intel, últimos errores. Solo lectura, sin login. | Nuevo HTML + `GET /api/ai-hub/health` |

---

## 🟡 Medio (3–7 días)

| # | Idea | Descripción | Dónde |
|---|------|-------------|--------|
| 6 | **WebSocket para ATABEY** | Actualizaciones en tiempo real en el dashboard: nuevo mensaje, cambio de estado de producción, alerta de trading. | `server.js`, `atabey-dashboard.html`, `atabey-system.js` |
| 7 | **Noticias reales en World Intelligence** | Integrar una API de noticias (NewsAPI, GNews, etc.) por categoría (crypto, economía, gobierno) y guardar en `world-intelligence/news.json`; exponer `GET /api/ai-hub/news`. | `world-intelligence.js` |
| 8 | **Dashboard de Learning Engine** | Página o sección: gráficos de errores por servicio, evolución de mejoras aplicadas, top sugerencias pendientes. | `platform/ai-hub-dashboard.html` o nuevo HTML |
| 9 | **Backup automático de datos AI** | Script o job que copie `node/data/ai-hub/` y `node/data/bdet-bank/` a carpeta/zip con fecha; opcionalmente llamar desde `start.sh` o cron. | Script en `scripts/` o integrado en server |
| 10 | **Logs y auditoría de ATABEY** | Registrar cada comando maestro y cada tarea asignada en `atabey/audit-log.json` (quién, qué, cuándo) y endpoint `GET /api/ai-hub/atabey/audit`. | `atabey-master-controller.js`, index |
| 11 | **Integración AI Trader con World Intelligence** | Que el AI Trader consuma señales de `world-intelligence` (trading-signals, market-analysis) para decisiones o reportes. | `ai-trader.js`, world-intelligence |
| 12 | **Export/Import de conocimiento** | Exportar `ai-learnings.json` + `improvements-log.json` a un JSON/zip; importar en otra instancia para clonar “aprendizaje”. | `learning-engine.js`, rutas en index |

---

## 🔴 Más grande (1–2 semanas)

| # | Idea | Descripción | Dónde |
|---|------|-------------|--------|
| 13 | **LLM opcional para ATABEY** | Conectar ATABEY a un LLM (OpenAI, Anthropic, o modelo local) para respuestas más naturales y sugerencias de mejoras de código; mantener fallback sin LLM. | `atabey-system.js`, env (API key), preferencias |
| 14 | **Auto-aplicar mejoras (con aprobación)** | En Learning Engine: cola de mejoras “aprobadas” que se apliquen automáticamente (patch de archivos o PR); registro de resultado en `improvements-log.json`. | `learning-engine.js`, jobs, seguridad |
| 15 | **App móvil o PWA** | PWA del dashboard ATABEY (o BDET Bank) para ver estado, briefing y alertas desde el móvil; service worker y manifest. | `platform/`, manifest, service-worker |
| 16 | **Multi-tenant / múltiples “familias”** | Varias familias (no solo una) con sus miembros y permisos; ATABEY resuelve familia por usuario o token. | `atabey-system.js`, family-members, auth |
| 17 | **Tests E2E del AI Hub** | Tests (Playwright o Cypress) que: levanten servidor, llamen a `/api/ai-hub/atabey`, `/bdet/status`, y comprueben respuesta. | `tests/` o `e2e/` |
| 18 | **Panel de compliance BDET** | Vista en BDET Bank: transacciones en revisión (AML), KYC pendientes, alertas de fraude; acciones “aprobar / rechazar” que llamen al AI Banker. | `platform/bdet-bank.html`, `ai-banker-bdet.js` |

---

## 📌 Prioridad sugerida (si eliges poco)

1. **Formularios en BDET Bank** (crear cuenta, préstamo, depósito) — uso directo.
2. **Notificaciones en ATABEY** — alertas de precio, préstamos, KYC.
3. **Reporte mensual BDET** — operación y gobierno.
4. **Health público del AI Hub** — visibilidad sin entrar al dashboard.
5. **Logs/auditoría de ATABEY** — trazabilidad y cumplimiento.

---

## ✅ Ya hecho (referencia)

- AI Hub (registry, collector, learning).
- ATABEY asistente + control maestra + dashboard.
- World Intelligence (mercados, señales, alertas).
- AI Banker BDET (cuentas, transacciones, préstamos, tarjetas, KYC, reporte diario).
- Integración AI Banker dentro de BDET Bank (menú, panel, estado en tiempo real).
- Sistema familiar (Family First).
- Documentación: `AI-HUB-DOCUMENTATION.md`, `PLATAFORMA-AI-RESUMEN-COMPLETO.md`.

---

Cuando decidas “qué más hacemos”, podemos bajar cualquiera de estas ideas a tareas concretas (archivos, endpoints, textos de UI) y hacerlo paso a paso.
