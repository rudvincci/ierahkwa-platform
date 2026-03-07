# Conferencia Soberana

> Servidor de videoconferencia soberana con senalizacion WebRTC, cifrado E2E AES-256-GCM y persistencia en PostgreSQL.

## Descripcion

Conferencia Soberana es el servicio de videoconferencia de la plataforma Ierahkwa. Proporciona una API REST para gestion de salas (crear, listar, unirse, salir) y un servidor de senalizacion WebRTC via Socket.IO para la negociacion de conexiones peer-to-peer entre participantes. Las conferencias soportan hasta 50 participantes por sala con cifrado extremo-a-extremo AES-256-GCM.

El servicio gestiona salas de conferencia persistentes en PostgreSQL, incluyendo metadata de participantes, estados de bloqueo y limites de capacidad. La senalizacion WebRTC maneja el intercambio de ofertas SDP, respuestas e ICE candidates entre peers. Ademas soporta funcionalidades como chat en sala, compartir pantalla y toggle de audio/video, todo transmitido en tiempo real via Socket.IO.

Incluye configuracion de servidores STUN/TURN para traversal de NAT, limpieza automatica de salas vacias cada 30 minutos, graceful shutdown con cierre de pool de base de datos, y middleware de seguridad compartido de Ierahkwa con rate limiting, sanitizacion y request tracing.

## Stack Tecnico

- **Runtime**: Node.js 18+
- **Framework**: Express 4.x
- **WebSocket**: Socket.IO 4.x
- **Base de Datos**: PostgreSQL (pg 8.x)
- **Cifrado**: E2EE AES-256-GCM
- **Seguridad**: Helmet, CORS, middleware compartido Ierahkwa
- **NAT Traversal**: STUN (Google) + TURN configurable
- **Puerto**: 3090

## API Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /health | Health check con salas activas y participantes |
| POST | /api/rooms | Crear sala de conferencia |
| GET | /api/rooms | Listar salas activas (filtro por estado, paginado) |
| GET | /api/rooms/:id | Obtener informacion de sala |
| POST | /api/rooms/:id/join | Unirse a una sala (retorna ICE servers) |
| POST | /api/rooms/:id/leave | Salir de una sala |
| GET | /api/ice-servers | Configuracion STUN/TURN |

### Socket.IO Events

| Evento | Direccion | Descripcion |
|--------|-----------|-------------|
| room:join | Client → Server | Unirse a sala de senalizacion |
| room:peers | Server → Client | Lista de peers existentes en sala |
| peer:joined | Server → Client | Nuevo peer se unio |
| peer:left | Server → Client | Peer abandono la sala |
| signal:offer | Bidireccional | Relay de oferta SDP |
| signal:answer | Bidireccional | Relay de respuesta SDP |
| signal:ice-candidate | Bidireccional | Relay de ICE candidate |
| media:toggle | Client → Server | Toggle audio/video |
| peer:media-toggle | Server → Client | Notificacion de toggle de peer |
| chat:message | Bidireccional | Mensaje de chat en sala |
| screen:start | Client → Server | Inicio de compartir pantalla |
| screen:stop | Client → Server | Fin de compartir pantalla |

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| PORT | Puerto del servicio | 3090 |
| CORS_ORIGINS | Origenes permitidos (separados por coma) | http://localhost:3000 |
| DATABASE_URL | URL de conexion PostgreSQL | postgresql://user:pass@localhost/conferencia |
| TURN_SERVER_URL | URL del servidor TURN | turn:turn.soberano.sovereign:3478 |
| TURN_USERNAME | Usuario TURN | soberano |
| TURN_CREDENTIAL | Credencial TURN | sovereign-credential |

## Instalacion

```bash
npm install
npm start
```

### Requisitos previos

- PostgreSQL 16 ejecutandose con la base de datos creada
- El servicio inicializa las tablas automaticamente al arrancar

## Docker

```bash
docker build -t conferencia-soberana .
docker run -p 3090:3090 \
  -e DATABASE_URL=postgresql://user:pass@db:5432/conferencia \
  -e CORS_ORIGINS=https://ierahkwa.gov \
  conferencia-soberana
```

## Arquitectura

```
Clientes REST ──→ Express API ──→ PostgreSQL (salas, participantes)
                                    ├── Crear/listar salas
                                    ├── Join/leave con validacion
                                    └── Limpieza automatica (30 min)

Clientes WebRTC ──→ Socket.IO ──→ Senalizacion P2P
                                    ├── SDP Offer/Answer relay
                                    ├── ICE Candidate relay
                                    ├── Chat en sala
                                    └── Screen sharing events

NAT Traversal:
  STUN (Google) → Descubrimiento de IP publica
  TURN (configurable) → Relay cuando P2P no es posible
```

Las conexiones de video/audio son peer-to-peer con cifrado E2EE. El servidor solo actua como relay de senalizacion y nunca ve ni almacena el contenido multimedia.

## Parte de

**Ierahkwa Ne Kanienke** — Plataforma Soberana Digital
