# Soberano Social — Servicio de Redes Sociales

> Plataforma social completa con 10 modulos: feed, posts, stories, comentarios, likes, follows, grupos, chat, live streaming y notificaciones.

## Descripcion

Soberano Social es el microservicio de redes sociales de la plataforma Ierahkwa Ne Kanienke. Provee una suite completa de funcionalidades sociales implementadas a traves de un motor social in-memory (`SocialEngine`) que gestiona perfiles, publicaciones, stories, comentarios, likes, sistema de seguidores, grupos, notificaciones y transmisiones en vivo.

El servicio soporta un sistema de propinas (tips) para creadores de contenido, reposts/shares, busqueda de perfiles, feed personalizado y feed de exploracion. Las transmisiones en vivo (live streaming) permiten a los usuarios crear y gestionar streams con audiencia en tiempo real.

La comunicacion en tiempo real se implementa via WebSocket con soporte para autenticacion por canal y suscripcion a multiples canales de eventos. El servicio opera como microservicio independiente registrado en el API Gateway principal en el puerto 4001.

## Stack Tecnico

- **Runtime**: Node.js 22
- **Framework**: Express 4.x
- **WebSocket**: ws 8.x
- **Motor Social**: SocialEngine (in-memory, lib/social-engine.js)
- **Seguridad**: Helmet 7.x, express-rate-limit 7.x
- **Puerto**: 4001 (configurable via `SOCIAL_PORT`)

## API Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /health | Health check con estadisticas y lista de features |
| **Perfiles** | | |
| POST | /v1/profiles | Crear perfil de usuario |
| GET | /v1/profiles/search | Buscar perfiles (param: q, limit) |
| GET | /v1/profiles/:userId | Obtener perfil por userId |
| PUT | /v1/profiles/:userId | Actualizar perfil |
| **Feed** | | |
| GET | /v1/feed | Feed personalizado del usuario (header: X-User-Id) |
| GET | /v1/feed/explore | Feed de exploracion (contenido publico) |
| **Posts** | | |
| POST | /v1/posts | Crear post |
| GET | /v1/posts/:id | Obtener post por ID |
| PUT | /v1/posts/:id | Editar post (solo autor) |
| DELETE | /v1/posts/:id | Eliminar post (solo autor) |
| GET | /v1/posts/user/:userId | Posts de un usuario |
| POST | /v1/posts/:id/tip | Enviar propina al creador del post |
| POST | /v1/posts/:id/share | Compartir/repostear un post |
| **Stories** | | |
| POST | /v1/stories | Crear story (contenido efimero) |
| GET | /v1/stories | Obtener stories de contactos (header: X-User-Id) |
| **Comentarios** | | |
| POST | /v1/comments | Crear comentario en un post |
| GET | /v1/comments/:postId | Obtener comentarios de un post |
| DELETE | /v1/comments/:id | Eliminar comentario (solo autor) |
| **Likes** | | |
| POST | /v1/likes/:postId | Dar like a un post |
| DELETE | /v1/likes/:postId | Quitar like |
| **Seguidores** | | |
| POST | /v1/follow/:targetId | Seguir a un usuario |
| DELETE | /v1/follow/:targetId | Dejar de seguir |
| GET | /v1/follow/:userId/followers | Lista de seguidores |
| GET | /v1/follow/:userId/following | Lista de seguidos |
| **Grupos** | | |
| POST | /v1/groups | Crear grupo |
| GET | /v1/groups | Listar grupos |
| POST | /v1/groups/:id/join | Unirse a un grupo |
| POST | /v1/groups/:id/leave | Salir de un grupo |
| **Notificaciones** | | |
| GET | /v1/notifications | Obtener notificaciones (header: X-User-Id) |
| POST | /v1/notifications/read | Marcar notificaciones como leidas |
| **Live Streaming** | | |
| POST | /v1/live | Iniciar transmision en vivo |
| GET | /v1/live | Listar transmisiones activas |
| POST | /v1/live/:id/end | Finalizar transmision |
| **WebSocket** | | |
| WS | /ws | Eventos sociales en tiempo real |

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| SOCIAL_PORT | Puerto del servicio | 4001 |

## Instalacion

```bash
npm install
npm start
```

## Docker

```bash
docker build -t social-media .
docker run -p 4001:4001 social-media
```

## Arquitectura

El servicio esta disenado con un motor social central (`SocialEngine`) que encapsula toda la logica de negocio en memoria, mientras que las rutas Express actuan como capa HTTP delgada que delega al motor.

- **server.js** -- Servidor Express + WebSocket con 10 modulos de rutas
- **lib/social-engine.js** -- Motor social con toda la logica de negocio (perfiles, posts, stories, comments, likes, follows, groups, notifications, live streams)

```
Clientes HTTP/WS --> [Soberano Social :4001]
                          |
          +---+---+---+---+---+---+---+---+---+---+
          |   |   |   |   |   |   |   |   |   |   |
        Feed Post Story Cmnt Like Follow Grp Notif Live WS
          |   |   |   |   |   |   |   |   |   |
          +---+---+---+---+---+---+---+---+---+
                          |
                    [SocialEngine]
                    (in-memory store)
```

**Autenticacion**: El userId se obtiene del header `X-User-Id` para la mayoria de los endpoints. En produccion, esto seria reemplazado por JWT validado en el API Gateway.

## Parte de

**Ierahkwa Ne Kanienke** -- Plataforma Soberana Digital
