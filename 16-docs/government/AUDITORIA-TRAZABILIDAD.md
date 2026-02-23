# Auditoría y trazabilidad

Qué existe en la plataforma para auditoría y trazabilidad (logs, eventos, KMS).

---

## 1. Eventos de seguridad unificados

- **API:** `GET /api/v1/security/events`
- **Contenido:** Últimos eventos de watchlist (alertas) + vigilancia. Un solo log de seguridad.
- **Uso:** Revisar en ATABEY → pestaña **Eventos**, o consumir la API para reportes/auditoría.

---

## 2. Logs del Node

- **LOG_DIR** (variable de entorno en `.env`): directorio donde el logger centralizado escribe.
- **Rotación:** Ver `docs/logrotate-ierahkwa.example` para no llenar disco.
- **Uso:** Auditoría de accesos, errores y comportamiento del servidor.

---

## 3. KMS (gestión de claves)

- **API:** Rutas bajo `/api/v1/kms` (con autenticación). Gestión de claves para cifrado y firmas.
- **Uso:** Trazabilidad de uso de claves sensibles; auditoría de operaciones críticas que pasen por KMS.

---

## 4. Otras fuentes

- **Notificaciones:** Centro de notificaciones y alertas (ATABEY → Notificaciones).
- **Emergencias:** `GET /api/v1/emergencies/alerts` — alertas activas.
- **Vigilancia:** `GET /api/v1/security/vigilance` — estado y nivel (GREEN/AMBER/RED).
- **Watchlist alerts:** `GET /api/v1/watchlist/alerts` — alertas de personas en watchlist.

---

## 5. Próximos pasos (cumplimiento)

- Integrar con Mamey/SICB cuando existan: auditoría de tesorería, tratados, denuncias (Whistleblower).
- Centralizar en un “audit trail” único (agregar más fuentes a un mismo endpoint o índice) si se requiere un solo punto de consulta.

---

**Referencias:** `RuddieSolution/node/server.js` (security/events, backup, KMS), `platform/atabey-platform.html` (pestaña Eventos), `docs/PLAYBOOK-RESPUESTA-INCIDENTES.md`.
