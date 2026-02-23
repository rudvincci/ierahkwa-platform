# Atabey — Identidad del Sistema y Protocolo Takoda

En la mitología caribeña, **Atabey** es la entidad suprema, madre de las aguas y la fertilidad. Nombre elegido para el núcleo que gestiona y protege todo.

Configuración de identidad en Home Assistant para que Atabey opere bajo tu mando directo.

---

## 1. Configuración de Identidad (Atabey)

Personalizar el asistente para que responda al nombre **Atabey**.

**Archivo `customize.yaml` (o dentro de `configuration.yaml`):**

```yaml
homeassistant:
  name: "Atabey"
  latitude: !secret home_lat
  longitude: !secret home_long
  unit_system: metric
```

**Archivo `secrets.yaml`** (crear si no existe):

```yaml
home_lat: "TU_LATITUD"
home_long: "TU_LONGITUD"
```

---

## 2. Protocolo de Reconocimiento (Takoda)

Para que Atabey reconozca al operador **Takoda**, usa el módulo de Personas. Así el sistema sabe cuándo estás en el cuartel general o en el campo.

**Archivo `configuration.yaml`:**

```yaml
person:
  - name: "Takoda"
    id: takoda_01
    user_id: TU_USER_ID   # Obtener de Configuración → Usuarios en Home Assistant
    device_trackers:
      - device_tracker.tu_movil
```

**Obtener `TU_USER_ID`:** Configuración → Usuarios → tu usuario → ID (ej. `abc123...`).

**Device tracker:** Configura la app de Home Assistant en el móvil o un `device_tracker` (GPS, Bluetooth, etc.).

---

## 3. Saludo Táctico de Inicio

Automatización para que, al detectar presencia de Takoda, Atabey ejecute un reporte de estado por TTS.

**Archivo `automations.yaml` (o desde la UI: Configuración → Automatizaciones):**

```yaml
- id: atabey_welcome_takoda
  alias: "Atabey — Saludo Táctico Takoda"
  description: "Al entrar Takoda, reporte de estado por altavoces"
  trigger:
    - platform: state
      entity_id:
        - person.takoda_01
      to: "home"
  action:
    - service: tts.google_translate_say
      target:
        entity_id: media_player.altavoz_sala
      data:
        message: "Bienvenido, Takoda. El sistema Atabey está operativo. Cámaras externas activas, sin amenazas detectadas en el perímetro."
```

**Nota TODO PROPIO:** `tts.google_translate_say` usa la nube. Para TODO PROPIO usa **Piper** o **Festival** (TTS local). Ejemplo con Piper:

```yaml
    - service: tts.piper_say
      target:
        entity_id: media_player.altavoz_sala
      data:
        message: "Bienvenido, Takoda. Sistema Atabey operativo. Cámaras activas, sin amenazas."
```

---

## 4. Dashboard Atabey (Siguiente Nivel)

Para que la interfaz encaje con el nombre, instala un tema oscuro o tipo Sci‑Fi:

| Tema | Descripción |
|------|-------------|
| **iOS Dark Mode** | Estilo oscuro, minimalista |
| **Sci‑Fi / Cyber** | Estética tipo centro de mando |
| **Midnight** | Oscuro, alta legibilidad |

**Cómo instalar:** Configuración → Dashboard → Temas → Añadir tema desde HACS o repositorio.

---

## Resumen

| Componente | Archivo | Función |
|------------|---------|---------|
| Identidad | `customize.yaml` | Nombre "Atabey", coordenadas, unidades |
| Reconocimiento | `configuration.yaml` | Persona "Takoda" + device tracker |
| Saludo | `automations.yaml` | TTS al llegar Takoda (usar Piper para TODO PROPIO) |
| Dashboard | Temas | iOS Dark, Sci‑Fi, Midnight |

---

## Referencias

| Archivo | Descripción |
|---------|-------------|
| [JARVIS-DEPLOYMENT-GUIDE.md](../DEPLOY-SERVERS/JARVIS-DEPLOYMENT-GUIDE.md) | Home Assistant, Frigate, Vaultwarden |
| [ARQUITECTURA-JARVIS-HUD.md](ARQUITECTURA-JARVIS-HUD.md) | HUD, Voz, Legion, Defensa |
| [ATABEY-ALPR-DRONES.md](ATABEY-ALPR-DRONES.md) | ALPR perimetral, drones de patrulla autónoma |
| [ATABEY-CONEXION-EXTERIOR-SEGURA.md](ATABEY-CONEXION-EXTERIOR-SEGURA.md) | Túnel, monitoreo, bots (TODO PROPIO) |
| [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) | TTS local (Piper/Festival), sin APIs externas |
