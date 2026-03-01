# Sovereign Core -- Backend Universal

**Ierahkwa Ne Kanienke -- Sovereign Digital Nation**

Backend universal que sirve las 441+ plataformas HTML del ecosistema Ierahkwa desde un unico servicio Express.js multi-tenant. Cada plataforma accede a autenticacion, contenido, pagos, mensajeria, votaciones, almacenamiento y analiticas a traves de una API REST unificada con contexto de plataforma dinamico.

## Arquitectura

```
sovereign-core v1.0.0
├── Express.js 4.18 (HTTP + REST API)
├── PostgreSQL (persistencia, JSONB flexible)
├── WebSocket /ws/chat (mensajeria en tiempo real)
├── JWT HS256 (autenticacion stateless)
├── 8 modulos funcionales
├── Hash-chained audit trail
└── MameyNode v4.2 (nodo soberano)
```

## 8 Modulos

| Modulo | Prefijo | Descripcion |
|---|---|---|
| **Auth** | `/v1/auth` | Registro, login, logout, refresh, perfil actual |
| **Users** | `/v1/users` | Perfiles, busqueda, datos publicos |
| **Payments** | `/v1/payments` | Transferencias WMP/IGT/BDET, tips a creadores, balance, historial |
| **Messaging** | `/v1/messages` | Mensajes internos con hilos, bandeja de entrada |
| **Voting** | `/v1/votes` | Elecciones, boletas con hash SHA-256, resultados |
| **Storage** | `/v1/storage` | Subida de archivos via multer, metadatos en DB |
| **Analytics** | `/v1/analytics` | Metricas por plataforma, actividad de usuarios |
| **Content** | `/v1/:platform/*` | CRUD generico de contenido con JSONB, scoped por plataforma |

## Inicio Rapido

### Prerrequisitos

- Node.js >= 20.0.0
- PostgreSQL >= 14
- Variables de entorno configuradas (ver abajo)

### Instalacion

```bash
# Clonar e instalar dependencias
cd 03-backend/sovereign-core
npm install

# Crear la base de datos
createdb sovereign_core

# Ejecutar migraciones (6 archivos SQL)
npm run migrate

# Iniciar servidor (puerto 3050 por defecto)
npm start

# Modo desarrollo con auto-reload
npm run dev

# Ejecutar tests
npm test
```

El servidor inicia en `http://localhost:3050`. Las migraciones se ejecutan automaticamente al arrancar si hay pendientes.

## Referencia de API

### Endpoints de Sistema

| Metodo | Ruta | Auth | Descripcion |
|---|---|---|---|
| `GET` | `/health` | No | Estado del servicio, version, uptime |
| `GET` | `/ready` | No | Readiness probe (verifica conexion a DB) |
| `GET` | `/metrics` | No | Metricas de memoria, PID, uptime |

### Auth (`/v1/auth`)

| Metodo | Ruta | Auth | Descripcion |
|---|---|---|---|
| `POST` | `/v1/auth/register` | No | Crear cuenta (email, password, display_name) |
| `POST` | `/v1/auth/login` | No | Autenticar con email y password |
| `POST` | `/v1/auth/logout` | Si | Logout (stateless, descartar token en cliente) |
| `POST` | `/v1/auth/refresh` | Si | Renovar JWT token |
| `GET` | `/v1/auth/me` | Si | Obtener usuario autenticado |

### Users (`/v1/users`)

| Metodo | Ruta | Auth | Descripcion |
|---|---|---|---|
| `GET` | `/v1/users/profile` | Si | Perfil completo del usuario actual + stats |
| `PUT` | `/v1/users/profile` | Si | Actualizar perfil (display_name, bio, avatar, etc.) |
| `GET` | `/v1/users/:id/public` | No | Perfil publico de cualquier usuario |

### Payments (`/v1/payments`)

| Metodo | Ruta | Auth | Descripcion |
|---|---|---|---|
| `POST` | `/v1/payments/send` | Si | Transferir fondos (WMP, IGT, BDET) |
| `GET` | `/v1/payments/balance` | Si | Balance por moneda del usuario |
| `GET` | `/v1/payments/history` | Si | Historial de transacciones (paginado, filtros) |
| `POST` | `/v1/payments/tip` | Si | Tip a creador (92% creador, 8% plataforma) |

### Messages (`/v1/messages`)

| Metodo | Ruta | Auth | Descripcion |
|---|---|---|---|
| `POST` | `/v1/messages/send` | Si | Enviar mensaje con subject y body |
| `GET` | `/v1/messages/inbox` | Si | Bandeja de entrada (paginada, filtro unread) |
| `GET` | `/v1/messages/thread/:threadId` | Si | Mensajes de un hilo (auto-marca como leidos) |
| `PUT` | `/v1/messages/:id/read` | Si | Marcar mensaje como leido |

### Votes (`/v1/votes`)

| Metodo | Ruta | Auth | Descripcion |
|---|---|---|---|
| `POST` | `/v1/votes/create-election` | Admin | Crear eleccion (simple_majority, ranked_choice, etc.) |
| `POST` | `/v1/votes/cast` | Si | Emitir voto (SHA-256 ballot hash) |
| `GET` | `/v1/votes/results/:electionId` | No | Resultados con porcentajes y ganador |
| `GET` | `/v1/votes/active` | No | Elecciones activas (filtro por plataforma) |

### Storage (`/v1/storage`)

| Metodo | Ruta | Auth | Descripcion |
|---|---|---|---|
| `POST` | `/v1/storage/upload` | Si | Subir archivo (multipart, max 50MB) |
| `GET` | `/v1/storage/:fileId` | No | Descargar archivo o metadatos (?meta=true) |
| `DELETE` | `/v1/storage/:fileId` | Si | Soft-delete (owner o admin) |

### Analytics (`/v1/analytics`)

| Metodo | Ruta | Auth | Descripcion |
|---|---|---|---|
| `GET` | `/v1/analytics/:platform/summary` | No | Resumen completo (contenido, usuarios, transacciones) |
| `GET` | `/v1/analytics/:platform/users` | No | Desglose de actividad por usuario |

### Content (`/v1/:platform/*`)

| Metodo | Ruta | Auth | Descripcion |
|---|---|---|---|
| `GET` | `/v1/:platform/` | No | Listar contenido (paginado, filtro por type) |
| `POST` | `/v1/:platform/` | Si | Crear contenido (type, title, body JSONB) |
| `GET` | `/v1/:platform/categories` | No | Tipos de contenido distintos |
| `GET` | `/v1/:platform/stats` | No | Estadisticas de contenido |
| `GET` | `/v1/:platform/:id` | No | Obtener item de contenido |
| `PUT` | `/v1/:platform/:id` | Si | Actualizar contenido (autor o admin) |
| `DELETE` | `/v1/:platform/:id` | Si | Soft-delete contenido (autor o admin) |

### WebSocket

| Protocolo | Ruta | Descripcion |
|---|---|---|
| `ws://` | `/ws/chat?platform=X&userId=Y` | Chat en tiempo real scoped por plataforma |

Tipos de mensaje WebSocket: `chat` (broadcast a plataforma), `dm` (mensaje directo).

## Variables de Entorno

| Variable | Default | Descripcion |
|---|---|---|
| `NODE_ENV` | `development` | Entorno (development, test, production) |
| `PORT` | `3050` | Puerto del servidor HTTP |
| `HOST` | `0.0.0.0` | Host de binding |
| `DATABASE_URL` | `postgresql://localhost:5432/sovereign_core` | Connection string PostgreSQL |
| `JWT_SECRET` | (auto-gen en dev) | Secreto HMAC-SHA256 para JWT (min 32 chars en prod) |
| `JWT_EXPIRES_IN` | `24h` | Tiempo de expiracion del token |
| `JWT_REFRESH_EXPIRES_IN` | `7d` | Expiracion de refresh token |
| `BCRYPT_ROUNDS` | `12` | Rondas de hashing (PBKDF2 310k iter) |
| `CORS_ORIGINS` | `http://localhost:3000,...` | Origenes CORS permitidos (comma-separated) |
| `RATE_LIMIT_MAX` | `200` | Max requests por ventana (global) |
| `RATE_LIMIT_WINDOW_MS` | `900000` | Ventana de rate limit (15 min) |
| `AUTH_RATE_LIMIT_MAX` | `10` | Max intentos de auth por ventana |
| `UPLOAD_DIR` | `./data/uploads` | Directorio de archivos subidos |
| `MAX_FILE_SIZE` | `52428800` | Tamano max de archivo (50 MB) |
| `SMTP_HOST` | `localhost` | Host SMTP para emails |
| `SMTP_PORT` | `587` | Puerto SMTP |
| `SMTP_USER` | (vacio) | Usuario SMTP |
| `SMTP_PASS` | (vacio) | Password SMTP |
| `EMAIL_FROM` | `noreply@ierahkwa.gov` | Remitente de emails |
| `LOG_LEVEL` | `debug` (dev) / `info` (prod) | Nivel de logging |
| `SOVEREIGN_NODE_ID` | `mameynode-core-001` | ID del nodo soberano |
| `BLOCKCHAIN_RPC` | `http://localhost:8545` | RPC del blockchain MameyNode |
| `NATION_CODE` | `IK` | Codigo de la nacion |
| `ENABLE_WEBSOCKET` | `true` | Habilitar WebSocket |
| `ENABLE_ANALYTICS` | `true` | Habilitar modulo analytics |
| `ENABLE_AUDIT_LOG` | `true` | Habilitar audit trail |
| `ENABLE_EMAIL_NOTIFICATIONS` | `false` | Habilitar notificaciones por email |
| `MAINTENANCE_MODE` | `false` | Modo mantenimiento |
| `DB_POOL_MIN` | `2` | Conexiones minimas del pool |
| `DB_POOL_MAX` | `20` | Conexiones maximas del pool |
| `DB_IDLE_TIMEOUT` | `30000` | Timeout de conexion idle (ms) |
| `DB_CONNECTION_TIMEOUT` | `5000` | Timeout de conexion (ms) |
| `DB_STATEMENT_TIMEOUT` | `30000` | Timeout de sentencia SQL (ms) |

**Requeridas en produccion**: `DATABASE_URL`, `JWT_SECRET`, `CORS_ORIGINS`.

## Seguridad

- JWT HS256 con timing-safe comparison
- PBKDF2 con 310,000 iteraciones (OWASP 2023)
- Helmet (OWASP security headers)
- Rate limiting global (200 req/15 min) y por auth
- Input sanitization (prototype pollution, XSS, injection)
- RFC 7807 error responses
- Hash-chained immutable audit trail
- Soft-delete en todas las entidades (datos nunca se pierden)
- SSL obligatorio en produccion

## Licencia

Propiedad del Gobierno Soberano de Ierahkwa Ne Kanienke. No licenciado para uso externo.
