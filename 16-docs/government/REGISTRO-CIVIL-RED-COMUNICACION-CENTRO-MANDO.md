# Registro Civil · Red de Comunicación Segura · Centro de Mando de Seguridad

Sovereign Government of Ierahkwa Ne Kanienke. Implementación de 3 pilares: identidad soberana, blindaje diplomático y protección territorial. TODO PROPIO.

---

## 1. Registro Civil y Base de Datos (Soberanía de Identidad)

Para organizar la población sin depender de sistemas externos: gestión de identidades y registros con **Baserow** y **Keycloak**.

### Función
- **Censo digital inmutable.** Cada ciudadano recibe un ID único que lo conecta con los servicios del gobierno y el nodo bancario.

### Acción
- Base de datos cifrada en servidores propios.
- Solo el gabinete autorizado tiene las llaves.
- Gestión desde actas de nacimiento hasta registros de tierras.

| Componente | Herramienta | Uso |
|------------|-------------|-----|
| Registros | Baserow | Censos, actas, tierras — control total, alternativa a Airtable |
| Identidad | Keycloak | SSO, ID único por ciudadano, acceso a gobierno y nodo bancario |
| Almacenamiento | Base cifrada (LUKS/VeraCrypt) | Datos en reposo cifrados; llaves en gabinete |

**TODO PROPIO:** ✓ Baserow y Keycloak self-hosted. Sin Airtable, Auth0 ni Google.

---

## 2. Red de Comunicación Segura (Blindaje Diplomático)

Para los líderes, el correo electrónico común es un riesgo. **Matrix-Synapse** con cliente **Element**.

### Seguridad
- **Cifrado extremo a extremo (E2EE)** — nadie puede interceptar, ni siquiera nosotros.
- Mensajes en tiempo real, federación opcional, control total.

### Video
- **Jitsi Meet** alojado localmente.
- Reuniones de gabinete sin pasar por servidores extranjeros.
- Videollamadas cifradas, sin Zoom ni Teams.

> **Mensaje para el equipo:** *"Takoda, la comunicación es el primer escudo del gobierno. Si el mensaje está seguro, el plan está seguro."*

| Componente | Herramienta | Uso |
|------------|-------------|-----|
| Mensajería | Matrix-Synapse + Element | E2EE, diplomacia, gabinete |
| Videoconferencias | Jitsi Meet | Reuniones locales, sin proveedores externos |

**TODO PROPIO:** ✓ Matrix, Element, Jitsi self-hosted. Sin WhatsApp, Telegram ni Zoom.

---

## 3. Centro de Mando de Seguridad (Protección Territorial)

Vinculación del sistema Atabey con cámaras y drones: **Frigate AI** y **QGroundControl**.

### Visión
- Detección automática de intrusos en fronteras y zonas críticas.
- Integración con sensores perimetrales y zonas definidas.

### Drones
- Centro de mando lanza **patrullas automáticas** desde servidores.
- Activación cuando sensores perimetrales detectan anomalías.
- ArduPilot + Mission Planner para rutas; MQTT/Home Assistant para automatización.

### Mapa Táctico
- Dashboard en **tiempo real**.
- Ubicación de agentes de seguridad.
- Reportes de incidentes y alertas.

| Componente | Herramienta | Uso |
|------------|-------------|-----|
| Cámaras + AI | Frigate | Detección de intrusos, objetos, zonas |
| Drones | QGroundControl, ArduPilot | Patrullas automáticas, rutas |
| Mapa | Dashboard propio / OpenStreetMap | Agentes, incidentes, zonas críticas |
| Automatización | Home Assistant, MQTT | Sensores → alertas → lanzamiento de patrullas |

**TODO PROPIO:** ✓ Frigate, QGroundControl, Home Assistant self-hosted. Sin servicios en la nube.

---

## Resumen por Pilar

| Pilar | Herramientas | Objetivo |
|-------|--------------|----------|
| 1. Registro Civil | Baserow, Keycloak, DB cifrada | Censo inmutable, ID único, actas y tierras |
| 2. Red Segura | Matrix/Element, Jitsi Meet | Blindaje diplomático E2EE |
| 3. Centro de Mando | Frigate, QGroundControl, dashboard | Protección territorial, drones, mapa táctico |

---

## Referencias

| Archivo | Descripción |
|---------|-------------|
| [SOBERANIA-GOBIERNO-5-MODULOS.md](SOBERANIA-GOBIERNO-5-MODULOS.md) | Nextcloud, Baserow, Keycloak, Matrix, Jitsi, OSM, Decidim |
| [ATABEY-ALPR-DRONES.md](ATABEY-ALPR-DRONES.md) | Drones ArduPilot, ALPR, MQTT |
| [ARQUITECTURA-VIGILANCIA-BRAIN.md](ARQUITECTURA-VIGILANCIA-BRAIN.md) | Frigate, go2rtc, Home Assistant |
| [ATABEY-INTEGRIDAD-FISICA-Y-PERIMETRO.md](ATABEY-INTEGRIDAD-FISICA-Y-PERIMETRO.md) | Sensores perimetrales, Traccar |
| [JARVIS-DEPLOYMENT-GUIDE.md](../DEPLOY-SERVERS/JARVIS-DEPLOYMENT-GUIDE.md) | Docker, despliegue |
| [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) | Sin APIs externas |
