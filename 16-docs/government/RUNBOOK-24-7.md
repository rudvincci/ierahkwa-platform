# Runbook: Operación 24/7

Sovereign Government of Ierahkwa Ne Kanienke · Monitoreo y respuesta continua.

---

## 1. Qué revisar de forma continua

| Revisión | Frecuencia | Dónde / Cómo |
|----------|------------|--------------|
| Health del Node | Cada 5 min (cron) | `scripts/health-alert-check.sh` — exit 1 = alertar |
| Estado ATABEY | Cada 5 min (mismo script) | GET `/api/v1/atabey/status` |
| Backups | Diario 2:00 (cron) | `scripts/install-cron-production.sh`; revisar `logs/backup-cron.log` |
| Eventos / alertas | Según necesidad | ATABEY → Eventos, Notificaciones, Emergencias |
| Logs del Node | Diario o al incidente | `LOG_DIR` (ver .env); rotación con `docs/logrotate-ierahkwa.example` |

---

## 2. Si algo falla

1. **Health o ATABEY caído**  
   Ver `docs/PLAYBOOK-RESPUESTA-INCIDENTES.md`. Comprobar proceso Node (puerto 8545), reiniciar si hace falta, revisar logs.

2. **Backup fallido**  
   Revisar `logs/backup-cron.log`; ejecutar manualmente `scripts/backup-platforms.sh` o el script configurado en cron.

3. **Alerta de seguridad (watchlist, IDS)**  
   ATABEY → Face · Watchlist, Eventos; seguir playbook según tipo de incidente.

4. **Emergencia ciudadano**  
   Emergencias, Safety Link; protocolo interno.

---

## 3. Contactos y escalado (ejemplo)

- Definir responsable 24/7 o turnos (ej. semanal).
- Definir escalado: nivel 1 (operador) → nivel 2 (admin/tech) → nivel 3 (líder).
- Mantener lista de contactos actualizada (fuera de este repo por seguridad).

---

## 4. Checklist diario (opcional)

- [ ] Revisar que health-check.log no muestre fallos consecutivos.
- [ ] Revisar que backup-cron.log tenga entrada del último día.
- [ ] Revisar Notificaciones / Eventos en ATABEY si hay alertas.

---

**Referencias:** `scripts/health-alert-check.sh`, `scripts/install-cron-production.sh`, `docs/PLAYBOOK-RESPUESTA-INCIDENTES.md`, `docs/CHECKLIST-DR.md`, `platform/incidentes-dr.html`.
