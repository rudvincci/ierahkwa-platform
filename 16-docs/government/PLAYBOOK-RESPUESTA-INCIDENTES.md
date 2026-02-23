# Playbook: Respuesta a incidentes

Sovereign Government of Ierahkwa Ne Kanienke · ATABEY

---

## 1. Detección

- **Eventos:** Revisar pestaña **Eventos** en ATABEY (log unificado watchlist + vigilancia).
- **Alertas:** Centro de **Notificaciones** y **Emergencias** (`/api/v1/emergencies/alerts`).
- **Estado conjunto:** **Vista Global** y **Comando Conjunto** en ATABEY (Fortress + AI + Quantum).
- **Health:** Si el script de alerta (`scripts/health-alert-check.sh`) falla, el Node o un servicio no responde.

---

## 2. Clasificación rápida

| Tipo | Acción inmediata |
|------|-------------------|
| **Watchlist / Face** | Ver pestaña Face · Watchlist y Eventos; activar protocolo si persona de interés. |
| **Intrusión / IDS** | Revisar `/api/v1/defense/ids/alerts` y Security Fortress; aislar si es necesario. |
| **Node caído** | Comprobar proceso Node (8545); reiniciar con `start.sh` o systemd; revisar logs. |
| **Backup fallido** | Ir a Backup Department; comprobar último backup; relanzar manual si hace falta. |
| **Emergencia ciudadano** | Usar `/api/v1/emergencies/alert` y Safety Link si aplica; movilizar según protocolo. |

---

## 3. Contención

- **Red:** Ghost Mode ya reduce superficie; si hace falta, aislar segmentos (firewall).
- **Servicios:** Detener solo el servicio afectado si es posible; no bajar todo el Node salvo emergencia.
- **Datos:** No borrar evidencias; guardar logs y estado de ATABEY antes de cambios.

---

## 4. Erradicación y recuperación

- Corregir causa (config, parche, restauración desde backup si aplica).
- Si hay restauración: usar **Backup Department** → restaurar desde último backup válido.
- Tras recuperación: comprobar `/health` y `/api/v1/atabey/status`.

---

## 5. Cierre

- Anotar qué pasó, qué se hizo y qué se va a mejorar (playbook, DR, formación).
- Actualizar este playbook si hace falta.

---

**Referencias:** ATABEY (atabey-platform.html), Compliance Center, Security Fortress, `docs/CHECKLIST-DR.md`, `scripts/health-alert-check.sh`.
