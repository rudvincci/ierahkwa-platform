# Arquitectura Jarvis — HUD, Friday, Legion, Memoria Semántica, Defensa Automatizada

Especificación tipo "Iron Man" para centro de mando: HUD, control remoto móvil, hardware distribuido, memoria semántica y defensa automatizada. Todo open-source y self-hosted (TODO PROPIO).

---

## 1. El HUD (Heads-Up Display)

**Objetivo:** Pantalla transparente o vertical sin teclado/ratón — vista tipo comandante.

| Componente | Descripción | Integración |
|------------|-------------|-------------|
| **MagicMirror²** | Plataforma modular open-source para pantallas transparentes | Overlay de feeds Frigate, clima, estado del sistema |
| **Grafana Kiosk** | Vista fullscreen en monitor vertical | Logs tipo "Matrix" de eventos de seguridad |

**Flujo:** MagicMirror² muestra cámaras Frigate + clima + estado. Grafana Kiosk en monitor secundario con logs en tiempo real.

---

## 2. Protocolo "Friday" (Control Remoto Móvil)

**Objetivo:** Túnel cifrado para hablar con el sistema desde el móvil cuando no estás en casa.

| Componente | TODO PROPIO | Descripción |
|------------|-------------|-------------|
| **ntfy** (self-hosted) | ✓ | Servidor open-source de notificaciones push. Alternativa a Telegram. |
| **Telegram Bot** | ✗ API externa | Solo como referencia; usar ntfy en su lugar para TODO PROPIO |
| **WireGuard / Tailscale** | ✓ | Túnel cifrado para acceder a la red local desde fuera |

**Flujo:** 
- ntfy (self-hosted): alertas instantáneas al móvil si hay amenaza de alto nivel.
- WireGuard/Tailscale: acceso seguro a ATABEY, Frigate, Grafana desde el móvil.
- Comandos por web (ATABEY) en lugar de Telegram.

---

## 3. Despliegue "Legion" (Hardware Distribuido)

**Objetivo:** Jarvis distribuido — cámaras y sensores en múltiples puntos.

| Componente | Descripción |
|------------|-------------|
| **ESP32-CAM** | Cámaras económicas que se integran vía ESPHome al servidor central |
| **ESPHome** | Conecta ESP32/ESP8266 a Home Assistant como "ojos satélite" |
| **Pwnagotchi / Minipwn** | Monitor de seguridad Wi‑Fi para detectar intentos de jamming de cámaras |

**Integración:** ESP32-CAM → ESPHome → Home Assistant → Frigate. Ver [ARQUITECTURA-VIGILANCIA-BRAIN.md](ARQUITECTURA-VIGILANCIA-BRAIN.md).

### Códice ESP32-CAM (ESPHome YAML)

```yaml
esphome:
  name: satellite_01

esp32:
  board: esp32cam

wifi:
  ssid: "YOUR_SECURE_WIFI"
  password: "YOUR_PASSWORD"

esp32_camera:
  external_clock:
    pin: GPIO0
    frequency: 20MHz
  i2c_pins:
    sda: GPIO26
    scl: GPIO27
  data_pins: [GPIO5, GPIO18, GPIO19, GPIO21, GPIO36, GPIO39, GPIO34, GPIO35]
  vsync_pin: GPIO25
  href_pin: GPIO23
  pixel_clock_pin: GPIO22
  power_down_pin: GPIO32
  name: "Jarvis Eye 01"

api:
  encryption:
    key: "YOUR_SECRET_KEY"
```

**Hardware:** Módulos ESP32-CAM + adaptador FTDI para el primer upload.

---

## 4. Centro de Comandos de Voz ("Intent Script")

**Objetivo:** Jarvis procesa tu voz localmente con LocalAI y Home Assistant. Comandos desde móvil o micrófono de escritorio.

### Jarvis Logic (configuration.yaml de Home Assistant)

```yaml
intent_script:
  JarvisStatus:
    speech:
      text: "System check complete. All devices are online."
    action:
      - service: notify.mobile_app_your_phone
        data:
          title: "System Status"
          message: "All connected devices are active."

  ToggleLivingRoomLights:
    speech:
      text: "Understood. Adjusting living room lights."
    action:
      - service: light.toggle
        target:
          entity_id: light.living_room
```

### Flujo Voz → Acción (tipo película)

| Paso | Componente | Descripción |
|------|------------|-------------|
| **Capturar** | Home Assistant Assist (Android/iOS) | Captura de voz en el dispositivo |
| **Procesar** | Whisper Add-on (servidor) | Speech-to-text local |
| **Ejecutar** | Intent Script | Comando "Jarvis, Toggle Living Room Lights" → acciona hardware físico |

**TODO PROPIO:** Whisper (open-source), LocalAI, Home Assistant — todo local, sin envío a la nube.

---

## 5. Memoria Semántica (Expansión del "Cerebro")

**Objetivo:** Que el sistema recuerde personas y patrones habituales.

| Componente | Descripción |
|------------|-------------|
| **MemGPT** | Framework open-source para memoria a largo plazo en LLMs. LocalAI recuerda patrones (ej. camión de reparto los martes) |
| **OpenFace** | Reconocimiento facial en tiempo real. Al acercarse alguien a la puerta: "Bienvenido, [Nombre]" por altavoces |

**Integración:** Face Recognition Propio ya existe en la plataforma. MemGPT se puede integrar con LocalAI/Ollama para contexto persistente.

---

## 6. Defensa Automatizada (Respuesta Accionable)

**Objetivo:** Respuesta automática ante intrusiones.

| Componente | Descripción |
|------------|-------------|
| **Sirenas Z-Wave/Zigbee** | Si se detecta intruso y no estás en casa: sirena 120 dB + luces en rojo estroboscópico |
| **Frigate → TTS** | Eventos de Frigate disparan mensaje de voz por altavoces: "Zona restringida. Se ha notificado a la policía." |
| **Piper / Festival** | Motores TTS locales (sin nube) |

**Flujo:** Frigate detecta persona → webhook a plataforma → regla de automatización → sirena + luces + TTS.

---

## Pro-Tip: Portainer

**Portainer** proporciona un dashboard visual para gestionar todos los contenedores (Frigate, go2rtc, Grafana, ntfy, Home Assistant, etc.) de forma centralizada.

---

## Mapa de integración con la plataforma

| Jarvis | Plataforma Ierahkwa |
|--------|---------------------|
| Frigate feeds | War Room, vigilancia-brain.html |
| Grafana | Puerto 3001, embed en War Room |
| ntfy (self-hosted) | Notificaciones push sin APIs externas |
| Face Recognition | Face Recognition Propio, watchlist |
| Frigate webhooks | `/api/v1/vms/alerts` |
| Control remoto | WireGuard + ATABEY desde móvil |

---

## Archivos de referencia

| Archivo | Descripción |
|---------|-------------|
| [JARVIS-DEPLOYMENT-GUIDE.md](../DEPLOY-SERVERS/JARVIS-DEPLOYMENT-GUIDE.md) | Host, Docker, Home Assistant, Frigate, Vaultwarden |
| [ATABEY-IDENTIDAD-HOME-ASSISTANT.md](ATABEY-IDENTIDAD-HOME-ASSISTANT.md) | Identidad Atabey, Protocolo Takoda, Saludo TTS, Dashboard |
| [ATABEY-ALPR-DRONES.md](ATABEY-ALPR-DRONES.md) | ALPR perimetral, drones de patrulla autónoma |
| [ARQUITECTURA-VIGILANCIA-BRAIN.md](ARQUITECTURA-VIGILANCIA-BRAIN.md) | Frigate, go2rtc, Grafana |
| [SISTEMA-GESTION-VIDEO-VMS.md](SISTEMA-GESTION-VIDEO-VMS.md) | Los 5 pilares del VMS |
| [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) | Sin dependencias de 3ra compañía |
