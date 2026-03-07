# Red Social Soberana

> Red social descentralizada con posts, chat en tiempo real, trading de WMP y propinas -- 0% impuestos.

## Descripcion

Red Social Soberana es el servicio backend de la red social del ecosistema Ierahkwa Ne Kanienke. Proporciona funcionalidad completa de publicaciones (posts), un sistema de chat con encriptacion post-cuantica, y un motor de trading integrado para el token Wampum (WMP).

El servicio soporta creacion de posts con multimedia, hashtags y menciones automaticas, sistema de propinas (tips) para creadores donde el 92% va al creador, reposts/shares, y feeds por usuario. El modulo de trading incluye ordenes de mercado y limitadas, libro de ordenes (orderbook), historial de trades, y datos de velas (candles) para graficos financieros.

Las comunicaciones en tiempo real se manejan via WebSocket, permitiendo suscripcion a canales de conversacion y relay de mensajes de chat entre participantes. La persistencia de datos se realiza en PostgreSQL con esquema auto-inicializable que crea tablas para posts, conversaciones, mensajes, ordenes de trading e historial de trades.

## Stack Tecnico

- **Runtime**: Node.js 22
- **Framework**: Express 4.x
- **Base de Datos**: PostgreSQL 16 (driver `pg` 8.x)
- **WebSocket**: ws 8.x
- **Seguridad**: Helmet 7.x, express-rate-limit 7.x
- **Puerto**: 4003 (configurable via `RED_SOCIAL_PORT`)

## API Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /health | Health check con estado de DB y features |
| POST | /v1/posts | Crear nuevo post (requiere auth) |
| GET | /v1/posts/:id | Obtener post por ID (incrementa vistas) |
| PUT | /v1/posts/:id | Editar post propio (requiere auth) |
| DELETE | /v1/posts/:id | Eliminar post propio (requiere auth) |
| POST | /v1/posts/:id/tip | Enviar propina en WMP al creador (requiere auth) |
| POST | /v1/posts/:id/share | Compartir/repostear un post (requiere auth) |
| GET | /v1/posts/user/:userId | Posts publicos de un usuario (paginados) |
| POST | /v1/chat/conversation | Crear conversacion (requiere auth) |
| GET | /v1/chat/conversations | Listar conversaciones del usuario (requiere auth) |
| POST | /v1/chat/:convId/message | Enviar mensaje en conversacion (requiere auth) |
| GET | /v1/chat/:convId/messages | Obtener mensajes de conversacion (requiere auth) |
| POST | /v1/chat/:convId/send-wmp | Enviar WMP dentro de un chat (requiere auth) |
| POST | /v1/trading/order | Crear orden de compra/venta (market o limit) |
| DELETE | /v1/trading/order/:id | Cancelar orden abierta |
| GET | /v1/trading/orderbook/:pair | Libro de ordenes (bids/asks) para un par |
| GET | /v1/trading/trades/:pair | Historial de trades recientes para un par |
| GET | /v1/trading/candles/:pair | Datos de velas (OHLCV) para graficos |
| WS | /ws | WebSocket para actualizaciones en tiempo real |

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| RED_SOCIAL_PORT | Puerto del servicio | 4003 |
| DATABASE_URL | Cadena de conexion PostgreSQL | postgresql://localhost:5432/red_social |

## Instalacion

```bash
npm install
npm start
```

## Docker

```bash
docker build -t red-social .
docker run -p 3003:4003 red-social
```

## Arquitectura

El servicio se estructura en modulos separados:

- **server.js** -- Servidor principal Express + WebSocket, orquesta rutas y realtime
- **db.js** -- Capa de datos PostgreSQL con pool de conexiones, transacciones, e inicializacion de esquema
- **posts.js** -- Router de publicaciones: CRUD, tips (92% al creador), shares/reposts
- **chat.js** -- Router de chat: conversaciones, mensajes con encriptacion AES-256-GCM, transferencias WMP in-chat
- **trading.js** -- Router de trading: ordenes market/limit, orderbook, trades, candles sinteticas

```
Clientes HTTP/WS --> [Red Social :4003]
                          |
                     +---------+----------+---------+
                     |         |          |         |
                   Posts     Chat     Trading     WS
                     |         |          |         |
                     +----+----+----+-----+         |
                          |              |          |
                     PostgreSQL     Fiscal Utils   Realtime
```

## Parte de

**Ierahkwa Ne Kanienke** -- Plataforma Soberana Digital
