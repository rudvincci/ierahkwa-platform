# Atabey — Integridad Física, Perímetro y Arquitectura Avanzada

Sistema de monitoreo de integridad física, detección inalámbrica, cartografía local y automatización basada en eventos. TODO PROPIO.

---

## 1. Sistema de Monitoreo de Integridad Física

**Objetivo:** Asegurar que los componentes de hardware permanezcan inalterados y protegidos.

| Componente | Descripción |
|------------|-------------|
| **Monitoreo de puertos USB** | Herramientas que alertan o reaccionan si se conectan dispositivos USB no autorizados (usbguard, udev rules, auditd). |
| **Gestión de cifrado** | Cifrado robusto para datos almacenados (LUKS, VeraCrypt), gestión segura de acceso y claves (Vaultwarden). |

**Implementación:** usbguard en Linux para política de dispositivos USB permitidos/bloqueados; LUKS para discos; Vaultwarden ya documentado para secretos.

**TODO PROPIO:** ✓ usbguard, LUKS, Vaultwarden son open-source y self-hosted.

---

## 2. Detección de Dispositivos Inalámbricos

**Objetivo:** Identificar redes Wi-Fi y dispositivos Bluetooth en un área definida.

| Componente | Descripción |
|------------|-------------|
| **Escaneo de redes** | Software para escanear y reportar Wi-Fi y Bluetooth cercanos (iwlist, nmap, hcitool, Kismet). |
| **Generación de alertas** | Alertas si se detectan dispositivos desconocidos dentro del perímetro definido. |

**Integración:** Scripts que ejecuten escaneos periódicos; Home Assistant o la plataforma reciben resultados vía MQTT/API; reglas de automatización emiten alertas.

**TODO PROPIO:** ✓ Herramientas locales (nmap, aircrack-ng, Kismet).

---

## 3. Sistema de Cartografía y Seguimiento Local

**Objetivo:** Visualizar información espacial y seguimiento sin depender de servicios en línea.

| Componente | Descripción |
|------------|-------------|
| **Plataforma de seguimiento GPS** | Software local (Traccar, OpenGTS, ownTracks) para monitorear ubicación de activos (vehículos, dispositivos) en mapa privado. |
| **Mapas desconectados** | Datos de mapas de fuentes abiertas (OpenStreetMap) para acceso offline. |

**Implementación:** Traccar/OpenGTS en Docker; tiles OSM descargados o servidor de tiles local (tile-server) para uso offline.

**TODO PROPIO:** ✓ Traccar, OpenStreetMap, tile-server self-hosted.

---

## 4. Automatización Basada en Eventos

**Objetivo:** Respuesta automática ante situaciones detectadas por el sistema.

| Protocolo | Descripción |
|-----------|-------------|
| **Monitoreo rutinario** | Acciones automáticas periódicas (escaneo, backup, reporte de estado). |
| **Alertas por detección** | Respuestas ante eventos: activar cámaras, enviar notificaciones (ntfy, mobile_app). |
| **Respuesta a intrusión** | Bloquear accesos, iniciar grabación, emitir advertencias (TTS, sirena) si sensores confirman intrusión. |

**Integración:** Frigate + Home Assistant + MQTT; reglas en `automations.yaml`; scripts de bloqueo (firewall, usbguard).

**TODO PROPIO:** ✓ Todo local.

---

## Resumen: Arquitectura Avanzada

| Componente | Descripción |
|------------|-------------|
| **Hardware** | Servidores físicos para alojar aplicaciones (Ubuntu/Debian, Docker). |
| **Visión** | Cámaras con análisis (Frigate, ESP32-CAM, CompreFace). |
| **Voz** | Procesamiento de voz para interacción (Whisper, Piper, Home Assistant Assist). |
| **Inteligencia** | Fuentes de información y detección de señales (RSS-Bridge, HoneyPot, escaneo inalámbrico). |
| **Seguridad** | Seguridad de red (Pi-hole, CrowdSec, WireGuard) y protección física de datos (LUKS, usbguard). |

---

## Mapa de Integración

| Módulo | Herramientas | Doc |
|--------|--------------|-----|
| Integridad física | usbguard, LUKS, Vaultwarden | Este doc |
| Detección inalámbrica | nmap, Kismet, scripts | Este doc |
| Cartografía local | Traccar, OpenStreetMap offline | Este doc |
| Automatización | Home Assistant, Frigate, MQTT | ATABEY-ALPR-DRONES, ARQUITECTURA-VIGILANCIA-BRAIN |

---

## Referencias

| Archivo | Descripción |
|---------|-------------|
| [ATABEY-FASE-EXPANSION-DIGITAL.md](ATABEY-FASE-EXPANSION-DIGITAL.md) | HoneyPot, OSINT, Kali, CompreFace |
| [ATABEY-ALPR-DRONES.md](ATABEY-ALPR-DRONES.md) | ALPR, drones, automatización |
| [ATABEY-HOGAR-DIGITAL-PRIVACIDAD.md](ATABEY-HOGAR-DIGITAL-PRIVACIDAD.md) | Red, VPN, NAS, monitoreo |
| [ARQUITECTURA-VIGILANCIA-BRAIN.md](ARQUITECTURA-VIGILANCIA-BRAIN.md) | Frigate, go2rtc, VMS |
| [ATABEY-SISTEMA-ARCHIVOS-GHOST.md](ATABEY-SISTEMA-ARCHIVOS-GHOST.md) | LUKS, Kill Switch, restic, UPS |
| [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) | Sin APIs externas |
