# Atabey — 3 Niveles para Conectar con el Mundo Exterior de Forma Segura

Acceso remoto, monitoreo y comunicación con Atabey sin exponer puertos del router. TODO PROPIO preferido.

---

## 1. El Túnel Cifrado (Acceso Remoto Seguro)

**Objetivo:** Acceder a tus sistemas de forma remota sin exponer puertos del router (riesgoso).

| Opción | TODO PROPIO | Descripción |
|--------|-------------|-------------|
| **Tailscale** | ✓ | Mesh VPN sobre WireGuard. Fácil de configurar. |
| **WireGuard** | ✓ | VPN ligera, código abierto, alto rendimiento |

### Implementación

1. Instala el cliente en tu **servidor** (Home Assistant, Frigate, etc.).
2. Instala el cliente en los **dispositivos remotos** (móvil, portátil).
3. Tras la configuración, accede a tus sistemas mediante una **IP privada segura**, como si estuvieras en la red local.

**Beneficio:** No abres puertos en el router; la conexión sale de forma outbound y es cifrada.

### WireGuard con Docker (linuxserver/wireguard)

```bash
docker run -d \
  --name=wireguard_atabey \
  --cap-add=NET_ADMIN \
  --cap-add=SYS_MODULE \
  -e PUID=1000 \
  -e PGID=1000 \
  -e TZ=Etc/UTC \
  -e SERVERURL=tu_ip_publica_o_dominio \
  -e PEERS=takoda_phone,takoda_laptop \
  -p 51820:51820/udp \
  -v ./wireguard-config:/config \
  --restart unless-stopped \
  linuxserver/wireguard
```

**Variables:**
- `SERVERURL`: Tu IP pública o dominio (para que los clientes se conecten).
- `PEERS`: Lista de peers separados por coma (genera configs en `wireguard-config/peer_*`).
- `51820/udp`: Puerto estándar de WireGuard — **abrir en el router** para este contenedor.

**Después de iniciar:** Los archivos `.conf` y QR para cada peer están en `./wireguard-config/`. Escanea el QR en el móvil o importa el `.conf` en el cliente WireGuard.

---

## 2. Monitoreo Básico del Sistema

**Objetivo:** Conocer el estado de tus sistemas y recibir alertas.

| Herramienta | Descripción |
|-------------|-------------|
| **Home Assistant** | Centraliza estado de servidores, dispositivos conectados y configuración |
| **Notificaciones** | Alertas sobre eventos: estado de red, rendimiento, intrusiones, ALPR |

### Configuración

- **Dashboard:** Home Assistant como centro de mando (estado de servicios, cámaras, sensores).
- **Alertas:** `notify.mobile_app_takoda_phone` o ntfy (self-hosted) para eventos críticos.
- **Integración:** Frigate, Grafana, sensores ESPHome → Home Assistant → Notificaciones.

---

## 3. Comunicación a través de Bots Seguros

**Objetivo:** Enviar comandos y recibir información de Atabey desde el móvil.

| Opción | TODO PROPIO | Descripción |
|--------|-------------|-------------|
| **ntfy** (self-hosted) | ✓ | Push notifications y suscripciones. Comandos vía web o app. |
| **Telegram Bot API** | ✗ API externa | Alternativa externa; solo como referencia |
| **Signal API** | ⚠️ | Signal no ofrece bot API oficial; opciones no oficiales |

### Comandos (a través de Home Assistant)

| Comando | Acción |
|---------|--------|
| `/estado` | Información básica del estado de los sistemas |
| `/reiniciar` | Reiniciar un servicio específico |

**Implementación TODO PROPIO:**

- Usar **ntfy** (self-hosted) para push y notificaciones.
- Acceder a **ATABEY** (plataforma web) vía Tailscale/WireGuard y ejecutar acciones desde el dashboard.
- Integrar **Assist** de Home Assistant o comandos por voz locales (Whisper + LocalAI).

**Alternativa con Telegram (si se acepta API externa):** Integración de Home Assistant con Telegram Bot; el bot recibe `/estado` y `/reiniciar` y ejecuta scripts de Home Assistant.

---

## Resumen por Nivel

| Nivel | Componente | TODO PROPIO |
|-------|------------|-------------|
| 1. Túnel | Tailscale / WireGuard | ✓ |
| 2. Monitoreo | Home Assistant + ntfy | ✓ |
| 3. Bots | ntfy + ATABEY web | ✓ |
| 3. Bots (alternativa) | Telegram Bot | ✗ API externa |

---

## Referencias

| Archivo | Descripción |
|---------|-------------|
| [ARQUITECTURA-JARVIS-HUD.md](ARQUITECTURA-JARVIS-HUD.md) | Friday Protocol, ntfy, WireGuard |
| [ATABEY-IDENTIDAD-HOME-ASSISTANT.md](ATABEY-IDENTIDAD-HOME-ASSISTANT.md) | Identidad Atabey, Takoda |
| [JARVIS-DEPLOYMENT-GUIDE.md](../DEPLOY-SERVERS/JARVIS-DEPLOYMENT-GUIDE.md) | VPN, acceso seguro |
| [ATABEY-HOGAR-DIGITAL-PRIVACIDAD.md](ATABEY-HOGAR-DIGITAL-PRIVACIDAD.md) | 5 módulos: Red, VPN, NAS, HA, Grafana |
| [ATABEY-FASE-EXPANSION-DIGITAL.md](ATABEY-FASE-EXPANSION-DIGITAL.md) | HoneyPot, OSINT, Kali, CompreFace, Gluetun |
| [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) | Sin APIs externas |
