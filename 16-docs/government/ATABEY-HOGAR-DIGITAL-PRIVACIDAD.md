# Atabey — Hogar Digital con Enfoque en Privacidad y Control

5 módulos para construir un sistema robusto de gestión del hogar digital. Todo open-source y local (TODO PROPIO).

---

## 1. Gestión de Red con Privacidad

**Objetivo:** Controlar el tráfico de red en el hogar para bloquear rastreadores y anuncios no deseados.

| Herramienta | Descripción |
|-------------|-------------|
| **Pi-hole** | Bloqueador de ads y rastreadores a nivel DNS. Todos los dispositivos de la red se benefician. |
| **AdGuard Home** | Alternativa similar; interfaz web moderna, bloqueo DNS y DoH. |

**Implementación:** Actúan como DNS primario del router; todas las peticiones DNS pasan por ellos y se filtran dominios de anuncios y trackers.

**TODO PROPIO:** ✓ Ambos son self-hosted, sin APIs externas para el bloqueo.

---

## 2. Acceso Remoto Seguro

**Objetivo:** Acceder al servidor de forma segura cuando no estás en casa.

| Herramienta | Descripción |
|-------------|-------------|
| **WireGuard** | VPN ligera, código abierto. Túnel seguro a la red doméstica. |
| **OpenVPN** | VPN clásica, bien documentada, amplia compatibilidad. |

**Implementación:** Ver [ATABEY-CONEXION-EXTERIOR-SEGURA.md](ATABEY-CONEXION-EXTERIOR-SEGURA.md) y el comando Docker `wireguard_atabey`.

**TODO PROPIO:** ✓ Ambos son self-hosted.

---

## 3. Almacenamiento y Gestión de Archivos

**Objetivo:** Centralizar archivos y medios para acceso fácil y seguro.

| Herramienta | Descripción |
|-------------|-------------|
| **TrueNAS Scale** | NAS con ZFS, RAID, snapshots, plugins. Escalable. |
| **OpenMediaVault** | NAS ligero, basado en Debian, interfaz web. |

**Características:** Almacenamiento redundante (RAID), compartir archivos de forma segura, backups, integración con Frigate (grabaciones).

**TODO PROPIO:** ✓ Self-hosted, datos locales.

---

## 4. Automatización del Hogar con Control Local

**Objetivo:** Controlar dispositivos inteligentes sin depender de la nube.

| Herramienta | Descripción |
|-------------|-------------|
| **Home Assistant** | Plataforma open-source para integrar y controlar dispositivos de forma local. |

**Integración:** Zigbee2MQTT, ESPHome, Frigate, Atabey, Takoda, Intents, TTS. Ver [ATABEY-IDENTIDAD-HOME-ASSISTANT.md](ATABEY-IDENTIDAD-HOME-ASSISTANT.md).

**TODO PROPIO:** ✓ Todo local; sin Alexa, Google Home ni servicios en la nube.

---

## 5. Monitoreo del Sistema

**Objetivo:** Conocer el estado y rendimiento del servidor.

| Herramienta | Descripción |
|-------------|-------------|
| **Prometheus** | Recolección de métricas (CPU, memoria, disco, servicios). |
| **Grafana** | Dashboards y visualización de métricas. |

**Métricas típicas:** Uso de CPU, memoria RAM, espacio en disco, estado de Frigate, Home Assistant, cámaras, red.

**TODO PROPIO:** ✓ Self-hosted. Ya incluido en [ARQUITECTURA-VIGILANCIA-BRAIN.md](ARQUITECTURA-VIGILANCIA-BRAIN.md) (Grafana puerto 3001).

---

## Orden Sugerido de Implementación

| Prioridad | Módulo | Razón |
|-----------|--------|-------|
| 1 | **Gestión de Red (Pi-hole/AdGuard)** | Base de privacidad para toda la red |
| 2 | **Acceso Remoto (WireGuard)** | Permite administrar desde fuera con seguridad |
| 3 | **Almacenamiento (TrueNAS/OMV)** | Centraliza datos y grabaciones de Frigate |
| 4 | **Automatización (Home Assistant)** | Ya documentado; integra todo |
| 5 | **Monitoreo (Grafana/Prometheus)** | Visibilidad del estado del sistema |

---

## Resumen

| Módulo | Herramientas | TODO PROPIO |
|--------|--------------|-------------|
| Red + Privacidad | Pi-hole, AdGuard Home | ✓ |
| Acceso Remoto | WireGuard, OpenVPN | ✓ |
| Almacenamiento | TrueNAS Scale, OpenMediaVault | ✓ |
| Automatización | Home Assistant | ✓ |
| Monitoreo | Grafana, Prometheus | ✓ |

---

## Referencias

| Archivo | Descripción |
|---------|-------------|
| [ATABEY-CONEXION-EXTERIOR-SEGURA.md](ATABEY-CONEXION-EXTERIOR-SEGURA.md) | WireGuard Docker, túnel cifrado |
| [ATABEY-IDENTIDAD-HOME-ASSISTANT.md](ATABEY-IDENTIDAD-HOME-ASSISTANT.md) | Home Assistant, Takoda |
| [ARQUITECTURA-VIGILANCIA-BRAIN.md](ARQUITECTURA-VIGILANCIA-BRAIN.md) | Grafana, Frigate |
| [JARVIS-DEPLOYMENT-GUIDE.md](../DEPLOY-SERVERS/JARVIS-DEPLOYMENT-GUIDE.md) | Docker, despliegue |
| [ATABEY-FASE-EXPANSION-DIGITAL.md](ATABEY-FASE-EXPANSION-DIGITAL.md) | HoneyPot, OSINT, Kali, CompreFace, Gluetun |
| [ATABEY-INTEGRIDAD-FISICA-Y-PERIMETRO.md](ATABEY-INTEGRIDAD-FISICA-Y-PERIMETRO.md) | USB, cifrado, escaneo inalámbrico, GPS, eventos |
| [ATABEY-SISTEMA-ARCHIVOS-GHOST.md](ATABEY-SISTEMA-ARCHIVOS-GHOST.md) | LUKS, Kill Switch, restic, UPS, Organizr |
| [SOBERANIA-GOBIERNO-5-MODULOS.md](SOBERANIA-GOBIERNO-5-MODULOS.md) | Nextcloud, Keycloak, Matrix, Decidim, Vocdoni |
