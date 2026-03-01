# Sovereign Core -- Blueprint Tecnico

**Ierahkwa Ne Kanienke -- Planos de Arquitectura**
**Version 1.0.0**

---

## 1. Diagrama de Arquitectura del Sistema

```
                         ┌─────────────────────────────────┐
                         │     441+ Plataformas HTML        │
                         │  (02-plataformas-html/*/*.html)  │
                         └────────────┬────────────────────┘
                                      │ fetch() / WebSocket
                                      ▼
                         ┌─────────────────────────────────┐
                         │         Load Balancer            │
                         │    (nginx / sovereign-proxy)     │
                         └─────────┬───────────┬───────────┘
                                   │           │
                          ┌────────▼──┐  ┌─────▼───────┐
                          │ Instance  │  │  Instance    │
                          │  :3050    │  │   :3051      │
                          │           │  │              │
                          │ ┌───────┐ │  │  ┌───────┐  │
                          │ │Express│ │  │  │Express│  │
                          │ │  App  │ │  │  │  App  │  │
                          │ └───┬───┘ │  │  └───┬───┘  │
                          │     │     │  │      │      │
                          │ ┌───▼───┐ │  │  ┌───▼───┐  │
                          │ │  WS   │ │  │  │  WS   │  │
                          │ │Server │ │  │  │Server │  │
                          │ └───────┘ │  │  └───────┘  │
                          └─────┬─────┘  └──────┬──────┘
                                │               │
                                └───────┬───────┘
                                        │
                                ┌───────▼───────┐
                                │  PostgreSQL    │
                                │  sovereign_    │
                                │  core DB       │
                                │                │
                                │  8 tablas      │
                                │  JSONB flex    │
                                │  UUID PKs      │
                                └───────┬───────┘
                                        │
                                ┌───────▼───────┐
                                │  Disco Local   │
                                │  data/uploads/ │
                                │  (archivos)    │
                                └───────────────┘
```

## 2. Diagrama de Esquema de Base de Datos

```
┌──────────────────────────────────────────────────────────────────┐
│                        PostgreSQL Schema                         │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌─────────────────────┐          ┌───────────────────────────┐  │
│  │       users         │          │         content           │  │
│  ├─────────────────────┤          ├───────────────────────────┤  │
│  │ id          UUID PK │◄────┐    │ id          UUID PK       │  │
│  │ email       TEXT UQ │     │    │ platform    TEXT    [idx]  │  │
│  │ password_hash TEXT  │     ├────│ author_id   UUID FK→users │  │
│  │ display_name TEXT   │     │    │ type        TEXT    [idx]  │  │
│  │ nation      TEXT    │     │    │ title       TEXT           │  │
│  │ language    TEXT     │     │    │ body        JSONB          │  │
│  │ role        TEXT    │     │    │ metadata    JSONB          │  │
│  │ tier        TEXT    │     │    │ status      TEXT    [idx]  │  │
│  │ bio         TEXT    │     │    │ view_count  INTEGER        │  │
│  │ avatar_url  TEXT    │     │    │ created_at  TIMESTAMPTZ    │  │
│  │ website     TEXT    │     │    │ updated_at  TIMESTAMPTZ    │  │
│  │ social_links JSONB  │     │    └───────────────────────────┘  │
│  │ status      TEXT    │     │                                   │
│  │ failed_logins INT   │     │    ┌───────────────────────────┐  │
│  │ last_login_at TSTZ  │     │    │      transactions         │  │
│  │ created_at  TSTZ    │     │    ├───────────────────────────┤  │
│  │ updated_at  TSTZ    │     │    │ id          UUID PK       │  │
│  └─────────────────────┘     ├────│ from_user   UUID FK→users │  │
│                              ├────│ to_user     UUID FK→users │  │
│                              │    │ amount      DECIMAL(18,8) │  │
│  ┌─────────────────────┐     │    │ currency    TEXT           │  │
│  │      messages       │     │    │ type        TEXT           │  │
│  ├─────────────────────┤     │    │ tx_hash     TEXT           │  │
│  │ id          UUID PK │     │    │ memo        TEXT           │  │
│  │ from_user   UUID FK─┤─────┤    │ status      TEXT           │  │
│  │ to_user     UUID FK─┤─────┤    │ metadata    JSONB          │  │
│  │ thread_id   UUID    │     │    │ created_at  TIMESTAMPTZ    │  │
│  │ subject     TEXT    │     │    └───────────────────────────┘  │
│  │ body        TEXT    │     │                                   │
│  │ platform    TEXT    │     │    ┌───────────────────────────┐  │
│  │ is_read     BOOL   │     │    │       elections           │  │
│  │ metadata    JSONB   │     │    ├───────────────────────────┤  │
│  │ created_at  TSTZ   │     │    │ id            UUID PK     │  │
│  └─────────────────────┘     │    │ title         TEXT        │  │
│                              │    │ description   TEXT        │  │
│                              │    │ choices       JSONB       │  │
│  ┌─────────────────────┐     │    │ start_date    TIMESTAMPTZ │  │
│  │       files         │     │    │ end_date      TIMESTAMPTZ │  │
│  ├─────────────────────┤     ├────│ created_by    UUID FK     │  │
│  │ id          UUID PK │     │    │ election_type TEXT        │  │
│  │ owner_id    UUID FK─┤─────┘    │ status        TEXT        │  │
│  │ original_name TEXT  │          │ blockchain_tx TEXT        │  │
│  │ stored_name  TEXT   │          │ created_at    TIMESTAMPTZ │  │
│  │ mime_type    TEXT   │          └──────────┬────────────────┘  │
│  │ size_bytes   BIGINT │                     │                   │
│  │ file_hash    TEXT   │          ┌──────────▼────────────────┐  │
│  │ file_path    TEXT   │          │        ballots            │  │
│  │ platform     TEXT   │          ├───────────────────────────┤  │
│  │ description  TEXT   │          │ id          UUID PK       │  │
│  │ status       TEXT   │          │ election_id UUID FK───────┤  │
│  │ created_at   TSTZ   │          │ voter_id    UUID FK→users │  │
│  └─────────────────────┘          │ choice      TEXT/JSONB    │  │
│                                   │ ballot_hash TEXT (SHA256) │  │
│  ┌─────────────────────┐          │ blockchain_tx TEXT        │  │
│  │    _migrations      │          │ created_at  TIMESTAMPTZ   │  │
│  ├─────────────────────┤          │ UQ(election_id, voter_id) │  │
│  │ id       SERIAL PK  │          └───────────────────────────┘  │
│  │ filename VARCHAR UQ │                                         │
│  │ checksum VARCHAR    │                                         │
│  │ executed_at TSTZ    │                                         │
│  └─────────────────────┘                                         │
└──────────────────────────────────────────────────────────────────┘
```

## 3. Grafo de Dependencias de Modulos

```
                    ┌──────────────────────────┐
                    │      server.js           │
                    │   (Express App + WSS)    │
                    └─────┬──────┬─────────────┘
                          │      │
               ┌──────────┘      └──────────────────────┐
               ▼                                         ▼
    ┌──────────────────┐                     ┌─────────────────────┐
    │    src/config.js  │                     │ ../shared/ (monorepo)│
    │                  │                     ├─────────────────────┤
    │  - env vars      │                     │ logger.js           │
    │  - fail-fast     │                     │ error-handler.js    │
    │  - JWT config    │                     │ audit.js            │
    │  - DB config     │                     │ security.js         │
    │  - Feature flags │                     │ validator.js        │
    └────────┬─────────┘                     └──────────┬──────────┘
             │                                          │
             ▼                                          │
    ┌──────────────────┐                                │
    │    src/db.js     │                                │
    │                  │                                │
    │  - Pool (pg)     │                                │
    │  - query()       │                                │
    │  - transaction() │                                │
    │  - initialize()  │                                │
    │  - migrations    │                                │
    └────────┬─────────┘                                │
             │                                          │
             ▼                                          ▼
    ┌─────────────────────────────────────────────────────────────┐
    │                    src/middleware/                           │
    │  ┌──────────────────────┐  ┌──────────────────────────┐    │
    │  │  auth.js             │  │  platform.js             │    │
    │  │                      │  │                          │    │
    │  │  - authMiddleware    │  │  - platformMiddleware    │    │
    │  │  - generateToken     │  │  - requirePlatform       │    │
    │  │  - verifyToken       │  │  - VALID_PLATFORM_PATTERN│    │
    │  │  - requireRole       │  │                          │    │
    │  └──────────┬───────────┘  └─────────────┬────────────┘    │
    └─────────────┼────────────────────────────┼─────────────────┘
                  │                            │
                  ▼                            ▼
    ┌─────────────────────────────────────────────────────────────┐
    │                    src/modules/                              │
    │                                                             │
    │  ┌──────────┐  ┌──────────┐  ┌───────────┐  ┌───────────┐ │
    │  │  auth/   │  │  users/  │  │ payments/ │  │ messaging/│ │
    │  │ routes.js│  │ routes.js│  │ routes.js │  │ routes.js │ │
    │  └──────────┘  └──────────┘  └───────────┘  └───────────┘ │
    │                                                             │
    │  ┌──────────┐  ┌──────────┐  ┌───────────┐  ┌───────────┐ │
    │  │ voting/  │  │ storage/ │  │ analytics/│  │  content/ │ │
    │  │ routes.js│  │ routes.js│  │ routes.js │  │ routes.js │ │
    │  └──────────┘  └──────────┘  └───────────┘  └───────────┘ │
    └─────────────────────────────────────────────────────────────┘
```

**Dependencias entre modulos**:
- Todos los modulos dependen de `db.js`, `config.js`, y las librerias shared (`logger`, `error-handler`, `audit`, `validator`)
- `auth/routes.js` depende de `middleware/auth.js` (generateToken)
- `voting/routes.js` depende de `middleware/auth.js` (requireRole)
- `content/routes.js` depende de `middleware/platform.js` (requirePlatform)
- Los modulos NO dependen entre si (desacoplados)

## 4. Diagrama de Flujo de Request

### 4.1 Request HTTP Autenticado

```
Cliente (HTML Platform)
    │
    │  POST /v1/payments/send
    │  Authorization: Bearer <jwt>
    │  X-Platform: marketplace-ik
    │  Content-Type: application/json
    │  {"to_user": "uuid", "amount": 50, "currency": "WMP"}
    │
    ▼
┌─────────────────────────────────────────────────────┐
│ 1. Helmet          → Security headers               │
│ 2. CORS            → Validate origin                 │
│ 3. Compression     → gzip response                   │
│ 4. Body Parser     → Parse JSON (max 10MB)           │
│ 5. Request ID      → Generate X-Request-Id UUID      │
│ 6. Request Logger  → Log method, path, IP            │
│ 7. Security Headers→ OWASP hardening                 │
│ 8. Sanitize Input  → Strip __proto__, prototype      │
│ 9. Rate Limiter    → Check 200/15min limit           │
│10. Audit Middleware → Prepare audit context           │
│11. Auth Middleware  → Verify JWT, attach req.user     │
│12. Validation      → Validate body schema            │
│13. Route Handler   → Business logic                  │
│    │                                                 │
│    ├─ db.getClient() → BEGIN transaction             │
│    ├─ Check balance                                  │
│    ├─ INSERT transaction                             │
│    ├─ COMMIT                                         │
│    ├─ audit.transaction()                            │
│    └─ res.status(201).json(...)                      │
│                                                      │
│14. Error Handler   → RFC 7807 if error               │
└─────────────────────────────────────────────────────┘
    │
    ▼
  201 Created
  {"status": "ok", "data": { "id": "uuid", "tx_hash": "sha256", ... }}
```

### 4.2 Flujo WebSocket

```
Cliente (HTML Platform)
    │
    │  ws://host:3050/ws/chat?platform=radio-ik&userId=abc-123
    │
    ▼
┌───────────────────────────────────────┐
│ WSS.on('connection')                  │
│   ├─ Parse platform + userId          │
│   ├─ clientId = "radio-ik:abc-123"   │
│   ├─ wsClients.set(clientId, ws)     │
│   ├─ Send { type: "connected" }       │
│   │                                   │
│   ├─ ws.on('message')                 │
│   │   ├─ Parse JSON                   │
│   │   ├─ type: "chat" → broadcast     │
│   │   │   to all clients where        │
│   │   │   client.platform === platform │
│   │   └─ type: "dm" → send to         │
│   │       wsClients.get(target)        │
│   │                                   │
│   ├─ ws.on('pong') → isAlive = true   │
│   └─ ws.on('close') → remove client   │
│                                       │
│ Heartbeat (30s interval)              │
│   └─ ping all, terminate dead sockets │
└───────────────────────────────────────┘
```

## 5. Arquitectura de Deployment

```
┌─────────────────────────────────────────────────────────────────┐
│                    Servidor de Produccion                        │
│                                                                 │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  Docker / Proceso systemd                                 │  │
│  │                                                           │  │
│  │  ┌─────────────────────────────────────────────────┐      │  │
│  │  │  sovereign-core (Node.js >= 20)                 │      │  │
│  │  │                                                 │      │  │
│  │  │  PORT=3050  NODE_ENV=production                 │      │  │
│  │  │  DATABASE_URL=postgresql://...                  │      │  │
│  │  │  JWT_SECRET=<64+ chars>                         │      │  │
│  │  │  CORS_ORIGINS=https://ierahkwa.gov              │      │  │
│  │  │                                                 │      │  │
│  │  │  HTTP Server (:3050)                            │      │  │
│  │  │    ├── REST API /v1/*                           │      │  │
│  │  │    ├── Health /health, /ready, /metrics         │      │  │
│  │  │    └── WebSocket /ws/chat                       │      │  │
│  │  │                                                 │      │  │
│  │  │  Graceful shutdown: SIGTERM/SIGINT              │      │  │
│  │  │    ├── Close WSS + drain HTTP connections       │      │  │
│  │  │    ├── Close DB pool                            │      │  │
│  │  │    └── Force exit after 30s timeout             │      │  │
│  │  └─────────────────────────────────────────────────┘      │  │
│  └───────────────────────────────────────────────────────────┘  │
│                              │                                  │
│                              ▼                                  │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  PostgreSQL 14+                                           │  │
│  │  Base: sovereign_core                                     │  │
│  │  Pool: 2-20 conexiones                                    │  │
│  │  SSL: rejectUnauthorized=true (produccion)                │  │
│  │  Timezone: UTC                                            │  │
│  │  Migrations: auto-run on startup                          │  │
│  └───────────────────────────────────────────────────────────┘  │
│                                                                 │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  Filesystem                                               │  │
│  │  data/uploads/   (archivos subidos, SHA-256 hash)         │  │
│  │  migrations/     (6 archivos SQL versionados)             │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### Probes de Salud

| Probe | Ruta | Uso |
|---|---|---|
| Liveness | `GET /health` | Verificar que el proceso responde |
| Readiness | `GET /ready` | Verificar conectividad a DB + modulos cargados |
| Metrics | `GET /metrics` | Memoria, PID, uptime para monitoreo |

### Shutdown Graceful

1. Recibir SIGTERM o SIGINT
2. Registrar evento de audit (SYSTEM_SHUTDOWN)
3. Cerrar WebSocket server + desconectar todos los clientes
4. Dejar de aceptar nuevas conexiones HTTP
5. Drenar conexiones existentes
6. Cerrar pool de PostgreSQL
7. Exit con codigo 0 (o 1 si hay error)
8. Force exit despues de 30 segundos si no drena

## 6. Estructura de Archivos

```
sovereign-core/
├── server.js                      # Entry point: Express app + WSS + shutdown
├── package.json                   # Dependencias y scripts (start, dev, test, migrate)
├── Dockerfile                     # Container image
│
├── src/
│   ├── config.js                  # Configuracion centralizada (env vars, fail-fast)
│   ├── db.js                      # PostgreSQL pool, query(), transaction(), migrations
│   ├── migrate.js                 # Script standalone de migraciones
│   │
│   ├── middleware/
│   │   ├── auth.js                # JWT auth middleware, generateToken, verifyToken, requireRole
│   │   └── platform.js            # Platform resolution middleware (URL/header/query)
│   │
│   └── modules/
│       ├── auth/
│       │   └── routes.js          # POST register, login, logout, refresh; GET me
│       ├── users/
│       │   └── routes.js          # GET/PUT profile; GET :id/public
│       ├── payments/
│       │   └── routes.js          # POST send, tip; GET balance, history
│       ├── messaging/
│       │   └── routes.js          # POST send; GET inbox, thread/:id; PUT :id/read
│       ├── voting/
│       │   └── routes.js          # POST create-election, cast; GET results/:id, active
│       ├── storage/
│       │   └── routes.js          # POST upload; GET :id; DELETE :id
│       ├── analytics/
│       │   └── routes.js          # GET :platform/summary, :platform/users
│       └── content/
│           └── routes.js          # GET /, categories, stats, :id; POST /; PUT :id; DELETE :id
│
├── migrations/
│   ├── 001_users.sql              # Tabla users + indices
│   ├── 002_content.sql            # Tabla content + indices (JSONB body)
│   ├── 003_transactions.sql       # Tabla transactions + indices
│   ├── 004_messages.sql           # Tabla messages + indices
│   ├── 005_elections.sql          # Tablas elections + ballots + indices
│   └── 006_files.sql              # Tabla files + indices
│
├── data/
│   └── uploads/                   # Directorio de archivos subidos
│
├── __tests__/                     # Tests con Jest
│
├── README.md                      # Documentacion general y referencia de API
├── WHITEPAPER.md                  # Whitepaper tecnico completo
└── BLUEPRINT.md                   # Este archivo: planos de arquitectura
```

### Convencion de Archivos

- Cada modulo contiene un unico `routes.js` que exporta un Express Router
- Las migraciones siguen el patron `NNN_nombre.sql` y se ejecutan en orden lexicografico
- Los archivos shared (`logger`, `error-handler`, `audit`, `security`, `validator`) viven en `../shared/` del monorepo
- El `Dockerfile` produce una imagen lista para produccion

---

*Gobierno Soberano de Ierahkwa Ne Kanienke -- 2026*
