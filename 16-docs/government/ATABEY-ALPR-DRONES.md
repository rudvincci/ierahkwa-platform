# Atabey — ALPR Perimetral y Drones de Patrulla Autónoma

Sistema de seguridad perimetral (lectura de placas) y patrulla autónoma con drones. Integración con Frigate, Home Assistant y MQTT. TODO PROPIO preferido.

---

## 1. Seguridad Perimetral — ALPR (Atabey)

**Objetivo:** Registrar cada vehículo que entra en la propiedad.

### Componentes

| Componente | TODO PROPIO | Descripción |
|------------|-------------|-------------|
| **OpenALPR** (self-hosted) | ✓ | Software open-source para lectura de placas. Corre localmente. |
| **Plate Recognizer** | ✗ API externa | Cloud API; usar solo como referencia; preferir OpenALPR para TODO PROPIO |
| **Frigate** | ✓ | Procesa los streams de cámaras; alimenta ALPR |
| **Home Assistant** | ✓ | Orquesta alertas y automatizaciones |

### Pasos de implementación

1. **Setup:** Ejecutar el software ALPR localmente en el servidor físico, alimentándolo con los streams que Frigate ya procesa.
2. **Base de datos:** Mantener una base local de vehículos autorizados (familia, repartos, servicios).
3. **Automatización:** Si ALPR detecta una placa que no está en la lista autorizada → alerta + sirena.

### Automatización — Vehículo no autorizado

**Archivo `automations.yaml` (Home Assistant):**

```yaml
- id: atabey_alpr_unauthorized
  alias: "Atabey Alert - Unauthorized Vehicle Detected"
  description: "Dispara cuando ALPR detecta placa no autorizada"
  trigger:
    platform: mqtt
    topic: "atabey/alpr/unauthorized_detection"
  action:
    - service: notify.mobile_app_takoda_phone
      data:
        title: "🚨 SECURITY BREACH"
        message: "Unauthorized vehicle detected at perimeter camera 04. License Plate: {{ trigger.payload_json.plate }}"
    - service: script.activate_exterior_siren
```

**MQTT payload esperado:** `{ "plate": "ABC123", "camera": "perimeter_04", "timestamp": "..." }`

**Nota TODO PROPIO:** Usar OpenALPR o similar self-hosted. Evitar Plate Recognizer (cloud API) para cumplir el principio TODO PROPIO.

---

## 2. Drones de Patrulla Autónoma — "Flying Agent" Códice

**Objetivo:** Patrulla automática que se activa ante intrusiones detectadas por Frigate.

### Software y hardware

| Componente | Descripción |
|------------|-------------|
| **ArduPilot** | Firmware open-source para drones |
| **Raspberry Pi / CubePilot** | Computadora de compañía conectada al dron |
| **Mission Planner** | Software open-source en laptop para rutas (waypoints) y geofencing |

### Flujo de integración con Atabey

1. **Frigate** detecta intrusión en una zona configurada.
2. **Home Assistant** recibe el evento (webhook o MQTT).
3. **Home Assistant** publica comando MQTT: `Atabey/drone_01/command/mission_start`.
4. **ArduPilot** (o script en la Raspberry Pi) recibe el comando e inicia la misión de patrulla.
5. El dron vuela la ruta predefinida (waypoints en Mission Planner).
6. El **stream de video** del dron llega al dashboard principal de Atabey.

### Protocolo del dron

| Mensaje MQTT | Descripción |
|--------------|-------------|
| `Atabey/drone_01/command/mission_start` | Iniciar ruta de patrulla |
| `Atabey/drone_01/command/return_home` | Regresar a base |
| `Atabey/drone_01/status` | Estado del dron (armado, en vuelo, aterrizado) |

### Plan de implementación

1. **Mission Planner:** Definir waypoints de patrulla y geofencing en laptop.
2. **ArduPilot:** Flashear el hardware del dron con ArduPilot.
3. **Companion computer:** Raspberry Pi conectada a ArduPilot, suscrita a MQTT.
4. **Script:** En la Pi, escuchar `Atabey/drone_01/command/mission_start` y enviar comando a ArduPilot.
5. **Stream de video:** Configurar streaming RTSP o similar desde el dron → Frigate o go2rtc → Dashboard Atabey.

---

## Resumen

| Sistema | Integración | TODO PROPIO |
|---------|-------------|-------------|
| **ALPR** | Frigate → OpenALPR → MQTT → Home Assistant | OpenALPR local ✓ |
| **Drones** | Frigate → MQTT → ArduPilot → Patrulla | ArduPilot, Mission Planner ✓ |

---

## Referencias

| Archivo | Descripción |
|---------|-------------|
| [ATABEY-IDENTIDAD-HOME-ASSISTANT.md](ATABEY-IDENTIDAD-HOME-ASSISTANT.md) | Identidad Atabey, Takoda |
| [ATABEY-CONEXION-EXTERIOR-SEGURA.md](ATABEY-CONEXION-EXTERIOR-SEGURA.md) | Túnel, monitoreo, bots |
| [ARQUITECTURA-JARVIS-HUD.md](ARQUITECTURA-JARVIS-HUD.md) | HUD, Legion, Defensa |
| [SISTEMA-GESTION-VIDEO-VMS.md](SISTEMA-GESTION-VIDEO-VMS.md) | VMS, Frigate, webhooks |
| [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) | Sin APIs externas |
