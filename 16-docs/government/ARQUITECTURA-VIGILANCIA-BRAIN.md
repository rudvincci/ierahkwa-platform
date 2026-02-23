# Arquitectura Vigilancia Brain — VMS + AI + Prevención

El "cerebro" de vigilancia: agregar cámaras, sensores, detección AI y datos de prevención. Todo open-source y self-hosted (principio TODO PROPIO).

---

## 1. Core Platform (El "Brain")

Para agregar cientos de APIs de cámaras y sensores, usar un VMS (Video Management System) o Hub de Automatización:

| Herramienta | Descripción |
|-------------|-------------|
| **Home Assistant** | Mejor opción open-source. Integra Amcrest, Hikvision, Reolink, Nest y mobile tracking. |
| **Frigate NVR** | NVR open-source con AI y detección de objetos. Distingue personas, coches, mascotas en tiempo real. Solo alerta ante amenazas reales. |

---

## 2. Integración de cámaras (API)

Para "mirar" cámaras externas, usar protocolos que unifiquen marcas:

| Herramienta | Descripción |
|-------------|-------------|
| **ONVIF Device Manager** | Descubrir direcciones internas de cámaras en la red. |
| **WebRTC / RTSP** | Protocolos para streaming con latencia cero. |
| **go2rtc** | Gestionar múltiples streams externos simultáneamente. |

---

## 3. Inteligencia y datos de prevención

Para cumplir la parte de "prevención":

| Fuente | Uso |
|--------|-----|
| **OpenData Soft** | Conectores para mapas de delitos o feeds de emergencia desde portales de ciudades USA/Canadá. |
| **Shodan API** | Auditorías de seguridad: ver si las propias cámaras están expuestas en internet. |

**Nota TODO PROPIO:** OpenData Soft y Shodan son APIs externas. Usar solo como referencia o para auditorías puntuales; no integrar como flujo crítico.

---

## 4. Infraestructura segura (cifrada)

| Herramienta | Descripción |
|-------------|-------------|
| **Tailscale / WireGuard** | Red overlay cifrada. Ver cámaras desde cualquier lugar como si estuvieras on-site, sin abrir puertos. |
| **Vaultwarden** | Almacenar API keys y contraseñas de cámaras de forma segura. Open-source, compatible con Bitwarden. |

---

## 5. Plan de acción recomendado

| Paso | Acción |
|------|--------|
| **Hardware** | Servidor dedicado (Mini PC o Rack) con Linux (Ubuntu/Debian). |
| **Contenedores** | Docker para Frigate y Home Assistant. |
| **Visualización** | Grafana para dashboard "War Room" con feeds de cámaras y estadísticas de delitos (FBI/StatCan). |

---

## Integración con ATABEY

- **ATABEY → Vigilancia** usa `data/webcams-registry.json` para las cámaras.
- **War Room Brain** se conecta vía:
  - Frigate API para streams y alertas.
  - go2rtc para WebRTC/RTSP.
  - Grafana para dashboards embebidos (opcional).

---

## Archivos de implementación

| Archivo | Descripción |
|---------|-------------|
| `DEPLOY-SERVERS/vigilancia-brain/docker-compose.yml` | Frigate, go2rtc, Vaultwarden, Grafana. |
| `DEPLOY-SERVERS/vigilancia-brain/setup-brain.sh` | Script de instalación en servidor Linux. |
| `RuddieSolution/platform/vigilancia-brain.html` | Página War Room que enlaza a Frigate/Grafana. |

---

## Referencias

- [FUENTES-OFICIALES-JUSTICIA-GLOBAL.md](FUENTES-OFICIALES-JUSTICIA-GLOBAL.md) — Estadísticas FBI, StatCan, UNODC.
- [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) — Sin dependencias de 3ra compañía.
