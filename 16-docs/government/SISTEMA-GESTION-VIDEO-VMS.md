# Sistema de Gestión de Video Avanzado (VMS)

Especificación que mapea los 5 pilares al stack soberano (TODO PROPIO). Frigate NVR, go2rtc, ATABEY, Backup Department.

---

## 1. Sistema de Gestión de Video Avanzado (VMS)

**Requisito:** Integrar diversas fuentes de video, análisis inteligente y herramientas de búsqueda eficientes.

| Componente | Implementación | Archivo/API |
|------------|----------------|-------------|
| **Centralizar gestión** | Frigate NVR + go2rtc | Puerto 5000 (Frigate), 1984 (go2rtc) |
| **Visualizar tiempo real** | WebRTC/RTSP vía go2rtc | `data/webcams-registry.json` |
| **Grabar y archivar** | Frigate `record` + `clips` | `frigate/config/frigate.yml` |
| **Buscar imágenes** | Frigate API `/api/events` | Eventos por tipo (person, car) y fecha |
| **UI integrada** | War Room + ATABEY Vigilancia | `vigilancia-brain.html`, `atabey-platform.html` |

**Acción:** Ver [ARQUITECTURA-VIGILANCIA-BRAIN.md](ARQUITECTURA-VIGILANCIA-BRAIN.md) y `DEPLOY-SERVERS/vigilancia-brain/`.

---

## 2. Análisis de Video Inteligente

**Requisito:** Detección automática de movimiento, objetos sospechosos, conteo de personas/vehículos, patrones de comportamiento inusuales.

| Función | Implementación | Configuración |
|---------|----------------|---------------|
| **Movimiento en zonas restringidas** | Frigate `motion` + `zones` | `frigate.yml` → `detect.zones` |
| **Objetos sospechosos** | Frigate `objects.track` | `person`, `car`, `dog`, etc. |
| **Conteo personas/vehículos** | Frigate `object_filters` + eventos | API `/api/events` |
| **Patrones inusuales** | Reglas en Frigate + MQTT (opcional) | `detect.required_zones` |
| **Alertas automáticas** | Frigate webhook → ATABEY | `/api/v1/vms/alerts` |

**Acción:** Configurar `frigate/config/frigate.yml` con zonas y objetos a detectar.

---

## 3. Control de Acceso Integrado

**Requisito:** Validar credenciales, registrar entradas/salidas, restringir áreas sensibles.

| Función | Implementación | Archivo/API |
|---------|----------------|-------------|
| **Validar credenciales** | API propia + Face Recognition | `/api/v1/face/match`, `/api/v1/vms/access/validate` |
| **Registrar entradas/salidas** | Log local JSON | `data/vms-access-log.json` |
| **Restringir áreas sensibles** | Reglas en plataforma | Integración con watchlist |
| **Cruce con watchlist** | Evento al detectar persona | `/api/v1/watchlist/event` |

**Acción:** Usar Face Recognition Propio y watchlist para cruzar detecciones con personas de interés.

---

## 4. Automatización de Alarmas y Notificaciones

**Requisito:** Notificaciones instantáneas, alarmas sonoras/visuales, bloqueo de puertas en emergencia.

| Función | Implementación | Archivo/API |
|---------|----------------|-------------|
| **Notificaciones al personal** | Notificaciones propios (sin email/SMS externo) | `/api/v1/notifications/send` |
| **Alarmas sonoras/visuales** | UI War Room + alertas en ATABEY | Indicadores en `vigilancia-brain.html` |
| **Bloqueo puertas (emergencia)** | Integración con hardware local | Documentar en `VMS-CONTROL-ACCESO.md` |
| **Reglas de automatización** | Reglas en JSON | `data/vms-automation-rules.json` |

**Nota TODO PROPIO:** Sin SendGrid, Twilio ni APIs externas. Usar canal interno de notificaciones y Web Push propio si se implementa.

---

## 5. Almacenamiento Seguro y Respaldo

**Requisito:** Grabaciones seguras, copias de seguridad periódicas, protección frente a pérdida o acceso no autorizado.

| Función | Implementación | Archivo/API |
|---------|----------------|-------------|
| **Grabaciones seguras** | Frigate → volumen local encriptado | `frigate/storage` |
| **Copias de seguridad** | Backup Department (ya existente) | `/api/v1/backup/create`, `backup-department.html` |
| **Protección acceso** | WireGuard/Tailscale + Vaultwarden | Ver ARQUITECTURA-VIGILANCIA-BRAIN |
| **Integridad evidencia** | Checksums y retención configurable | Frigate `record.retain` |

**Acción:** Incluir `frigate/storage` y `data/vms/` en el backup automático del Backup Department.

---

## Webhook Frigate → ATABEY

Configurar en `frigate/config/frigate.yml`:

```yaml
# Webhook para enviar eventos a la plataforma
webhook:
  enabled: true
  url: http://TU_SERVIDOR:8545/api/v1/vms/alerts
  # Opcional: Frigate envía eventos de detección (person, car, etc.)
```

---

## Resumen de APIs propias

| Endpoint | Descripción |
|----------|-------------|
| `GET /api/v1/vms/status` | Estado Frigate, go2rtc, Grafana |
| `GET /api/v1/vms/events` | Eventos recientes (proxy a Frigate si disponible) |
| `POST /api/v1/vms/access/log` | Registrar entrada/salida |
| `GET /api/v1/vms/access/log` | Historial de accesos |
| `POST /api/v1/vms/alerts` | Recibir webhook de Frigate (alertas) |
| `GET /api/v1/vms/storage` | Estado de almacenamiento y backup |

---

## Referencias

- [ARQUITECTURA-VIGILANCIA-BRAIN.md](ARQUITECTURA-VIGILANCIA-BRAIN.md)
- [FUENTES-OFICIALES-JUSTICIA-GLOBAL.md](FUENTES-OFICIALES-JUSTICIA-GLOBAL.md)
- [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md)
