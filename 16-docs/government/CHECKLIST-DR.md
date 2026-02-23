# Checklist: Recuperación ante desastres (DR)

Sovereign Government of Ierahkwa Ne Kanienke · ATABEY

---

## Antes (preparación)

- [ ] Backups automáticos programados (cron) y verificados. Ver `scripts/backup-*.sh`, `PRODUCTION-SETUP.md`.
- [ ] Documentación de arquitectura y despliegue actualizada (PLANO, PRODUCTION-SETUP, DEPLOY-SERVERS).
- [ ] Copia de `.env` y claves (KMS, JWT) en lugar seguro y recuperable.
- [ ] Sitio o servidor secundario definido (opcional; para réplica o failover).
- [ ] Contactos de escalado y turnos 24/7 definidos (si aplica).

---

## Durante (incidente)

- [ ] Clasificar: ¿solo Node? ¿datos? ¿red? Ver `docs/PLAYBOOK-RESPUESTA-INCIDENTES.md`.
- [ ] Contener: no borrar evidencias; guardar logs y estado (p. ej. `/api/v1/atabey/status`).
- [ ] Si hay pérdida de datos o corrupción: decidir punto de restauración (último backup válido).
- [ ] Restaurar desde Backup Department o script de restore; verificar integridad.
- [ ] Comprobar `/health` y `/api/v1/atabey/status` tras restaurar.

---

## Después (post-incidente)

- [ ] Documentar causa, acciones y tiempo de recuperación.
- [ ] Actualizar playbook y este checklist si hace falta.
- [ ] Revisar y probar backups y restores de forma periódica (p. ej. trimestral).

---

**Referencias:** `RuddieSolution/platform/backup-department.html`, `scripts/backup-*.sh`, `docs/PLAYBOOK-RESPUESTA-INCIDENTES.md`, `PRODUCTION-SETUP.md`.
