# Concienciación en seguridad

Sovereign Government of Ierahkwa Ne Kanienke · Principios básicos para todo el equipo.

---

## 1. Todo propio — nada de tercera compañía

- No usar servicios externos (Google, AWS, Stripe, WhatsApp, etc.) para datos o operación crítica.
- Infraestructura, código y protocolos propios. Ver `PRINCIPIO-TODO-PROPIO.md`.

---

## 2. Roles y accesos

- **Front office:** solo lo que ve el ciudadano. No acceder a Admin ni ATABEY con usuario de prueba de ciudadano.
- **Back office (Admin):** gestión interna. No compartir credenciales de admin; cada trabajador con su usuario.
- **Leader (ATABEY):** acceso total. Credenciales y JWT en backend (.env); no en el HTML. Ver `docs/ONBOARDING-POR-ROL.md`.

---

## 3. Incidentes y DR

- Saber dónde está el **playbook** y el **checklist DR**: ATABEY → Plataformas seguridad → Incidentes y DR, o `docs/PLAYBOOK-RESPUESTA-INCIDENTES.md` y `docs/CHECKLIST-DR.md`.
- Si algo cae: revisar Eventos, Notificaciones, Comando Conjunto; si el health falla, seguir playbook y no borrar evidencias.

---

## 4. Backups y contraseñas

- Backups automáticos (cron) deben estar activos en producción. Ver `scripts/install-cron-production.sh`.
- Contraseñas y secretos solo en .env o gestor seguro; nunca en el código ni en el front.

---

## 5. Resumen

- Todo propio. Roles claros. Playbook y DR conocidos. Backups y secretos bajo control.

**Referencias:** `PRINCIPIO-TODO-PROPIO.md`, `docs/ONBOARDING-POR-ROL.md`, `docs/PLAYBOOK-RESPUESTA-INCIDENTES.md`, `docs/CHECKLIST-DR.md`, `docs/RUNBOOK-24-7.md`, `platform/incidentes-dr.html`.
