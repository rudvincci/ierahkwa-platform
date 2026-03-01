# Sovereign Core -- Whitepaper Tecnico

**Ierahkwa Ne Kanienke -- Backend Universal Multi-Tenant**
**Version 1.0.0**

---

## 1. Problema

El ecosistema Ierahkwa Ne Kanienke comprende 441+ plataformas HTML distribuidas en 18 NEXUS mega-portales que sirven a mas de 1 billion de personas en 35+ paises y 574 naciones tribales. Cada plataforma necesita funcionalidad de backend: autenticacion, almacenamiento de contenido, transacciones financieras, mensajeria, votaciones y analiticas.

Crear 441 servicios backend independientes presenta problemas insuperables:

- **Mantenimiento**: Actualizar una correccion de seguridad requeriria parchearlo en 441 codebases.
- **Costos**: 441 instancias de base de datos, 441 procesos de servidor, 441 pipelines de CI/CD.
- **Inconsistencia**: Cada servicio divergiria en su implementacion de autenticacion, manejo de errores y esquema de datos.
- **Identidad fragmentada**: Los usuarios necesitarian 441 cuentas separadas en vez de una identidad soberana unificada.
- **Auditoria imposible**: Rastrear actividad a traves de servicios aislados requiere infraestructura de observabilidad que excede los recursos disponibles.

## 2. Solucion

**Sovereign Core** es un backend universal multi-tenant que sirve todas las 441+ plataformas desde una unica instancia Express.js con una base de datos PostgreSQL compartida. Cada plataforma se identifica por un slug (e.g., `correo-soberano`, `radio-ik`, `consejo-tribal`) y todo el contenido, transacciones y actividad se scoped automaticamente a la plataforma correspondiente.

### Principios de Diseno

1. **Una identidad, todas las plataformas**: Un usuario se registra una vez y accede a cualquier plataforma con el mismo JWT.
2. **Contenido scoped por plataforma**: La tabla `content` usa columnas `platform` + `type` + JSONB `body` para almacenar cualquier estructura de datos sin migraciones por plataforma.
3. **Zero-dependency frontends**: Las 441 plataformas HTML son archivos estaticos que solo necesitan `fetch()` hacia la API REST.
4. **Audit trail inmutable**: Toda operacion de escritura genera un registro hash-chained para trazabilidad forense.
5. **Economia soberana integrada**: Transferencias WMP/IGT/BDET nativas con tips a creadores (92/8 split).

## 3. Arquitectura

### 3.1 Stack Tecnologico

| Capa | Tecnologia | Justificacion |
|---|---|---|
| Runtime | Node.js >= 20 | Event loop nativo, amplio ecosistema |
| Framework | Express.js 4.18 | Estabilidad probada, middleware composable |
| Base de datos | PostgreSQL 14+ | JSONB nativo, transacciones ACID, indices GIN |
| Tiempo real | WebSocket (ws 8.x) | Baja latencia para chat, sin polling |
| Autenticacion | JWT HS256 (crypto nativo) | Stateless, zero-dependency, timing-safe |
| Hashing | PBKDF2 SHA-512, 310k iter | OWASP 2023, crypto nativo de Node.js |
| Upload | Multer (disco) | Streaming eficiente, validacion MIME |
| Seguridad | Helmet + custom OWASP | CSP, HSTS, X-Frame, sanitizacion |

### 3.2 Modulos

Sovereign Core se organiza en 8 modulos funcionales independientes, cada uno con su propio archivo `routes.js`:

```
src/modules/
├── auth/        Registro, login, JWT lifecycle
├── users/       Perfiles, busqueda, datos publicos
├── payments/    Transacciones WMP/IGT/BDET, tips, balance
├── messaging/   Mensajes internos con threads
├── voting/      Elecciones, boletas con SHA-256 hash
├── storage/     Upload de archivos, metadatos, descarga
├── analytics/   Metricas por plataforma, actividad
└── content/     CRUD generico scoped por plataforma (JSONB)
```

Cada modulo:
- Recibe `db`, `log`, `audit` y `config` via `app.locals`
- Define sus propias validaciones con un sistema de schemas
- Produce errores RFC 7807 (Problem Details)
- Genera registros de auditoria automaticamente para operaciones de escritura

### 3.3 Middleware Stack

El orden del middleware es critico para seguridad y funcionalidad:

1. **Helmet** -- Headers de seguridad (CSP, HSTS, X-Content-Type-Options)
2. **CORS** -- Origenes permitidos con credenciales
3. **Compression** -- gzip/brotli para respuestas
4. **Body Parsing** -- JSON (10MB limit), URL-encoded (2MB limit)
5. **Request ID** -- UUID unico por request para trazabilidad
6. **Request Logger** -- Logging estructurado JSON (Pino-compatible)
7. **Security Headers** -- Hardening adicional OWASP
8. **Input Sanitization** -- Defensa contra prototype pollution, XSS, injection
9. **Rate Limiting** -- 200 req/15min por IP (configurable)
10. **Audit Middleware** -- Registro automatico de operaciones de escritura en `/v1/`

### 3.4 Identificacion de Plataforma

La plataforma se resuelve por orden de prioridad:

1. Parametro URL `:platform` (e.g., `/v1/correo-soberano/items`)
2. Header `X-Platform`
3. Query parameter `?platform=correo-soberano`

Patron valido: `^[a-z0-9][a-z0-9\-]{1,63}$`

## 4. Seguridad

### 4.1 Autenticacion

- **Registro**: Email + password (PBKDF2 SHA-512, 310,000 iteraciones, salt de 32 bytes)
- **Login**: Retorna JWT HS256 con claims `sub`, `email`, `role`, `display_name`, `nation`
- **Verificacion**: Timing-safe comparison de firma HMAC-SHA256
- **Expiracion**: 24h por defecto, refresh de 7 dias
- **Roles**: `user`, `admin` con middleware `requireRole()`
- **Modo desarrollo**: Tokens mock `ik_<userId>_<role>` cuando `JWT_SECRET` no esta configurado

### 4.2 Protecciones OWASP

| Proteccion | Implementacion |
|---|---|
| Injection (SQL) | Queries parametrizados ($1, $2, etc.) |
| XSS | Sanitizacion de input, Helmet CSP |
| CSRF | CORS estricto con credenciales |
| Broken Auth | Rate limit en auth (10/15min), bloqueo de cuenta |
| Security Misconfiguration | Helmet defaults, fail-fast en produccion |
| Sensitive Data Exposure | PBKDF2 hashing, JWT con expiracion |
| Prototype Pollution | Sanitizacion de req.body/query/params |

### 4.3 Rate Limiting

- **Global**: 200 requests / 15 minutos por IP
- **Auth**: 10 intentos / 15 minutos por IP
- Headers de respuesta: `X-RateLimit-Remaining`

### 4.4 Audit Trail

Cada operacion de escritura genera un registro de auditoria con:
- Categoria (AUTH_LOGIN, DATA_CREATE, DATA_MODIFY, SYSTEM_STARTUP, etc.)
- Nivel de riesgo (LOW, MEDIUM, HIGH, CRITICAL)
- Hash-chain para integridad (cada registro contiene el hash del anterior)
- Request ID, IP, user agent, timestamp
- Alertas automaticas en eventos CRITICAL

## 5. Modelo de Datos

### 5.1 PostgreSQL con JSONB

La clave de la flexibilidad multi-tenant es el uso de JSONB en PostgreSQL:

- **`content.body`**: JSONB que almacena cualquier estructura de datos. Una plataforma de blog guarda `{ "text": "...", "tags": [...] }`, una galeria guarda `{ "images": [...], "category": "..." }`, un marketplace guarda `{ "price": 100, "stock": 50 }`. Sin migraciones por plataforma.
- **`content.metadata`**: JSONB auxiliar para datos adicionales indexables.
- **`transactions.metadata`**: JSONB para datos contextuales de transacciones (tip details, content_id, etc.).

### 5.2 Esquema de Tablas

| Tabla | Columnas Clave | Proposito |
|---|---|---|
| `users` | id (UUID), email, password_hash, display_name, nation, tier, role | Identidad unica cross-platform |
| `content` | id, platform, type, author_id, title, body (JSONB), metadata (JSONB), status | Contenido generico scoped |
| `transactions` | id, from_user, to_user, amount (DECIMAL 18,8), currency, type, tx_hash | Economia soberana |
| `messages` | id, from_user, to_user, thread_id, subject, body, is_read | Mensajeria con hilos |
| `elections` | id, title, choices (JSONB), start_date, end_date, election_type, status | Democracia digital |
| `ballots` | id, election_id, voter_id, choice, ballot_hash (SHA-256), UNIQUE(election,voter) | Votos verificables |
| `files` | id, owner_id, original_name, mime_type, size_bytes, file_hash (SHA-256), path | Almacenamiento con integridad |
| `_migrations` | id, filename, checksum (SHA-256), executed_at | Control de migraciones |

### 5.3 Indices

Cada tabla tiene indices optimizados para los patrones de consulta mas frecuentes:
- `content`: indices en `platform`, `author_id`, `status`, `(platform, type)`, `created_at DESC`
- `transactions`: indices en `from_user`, `to_user`, `platform`, `created_at DESC`
- `messages`: indices en `to_user`, `from_user`, `thread_id`, `(to_user, read)`
- `ballots`: indice en `election_id`, constraint UNIQUE `(election_id, voter_id)`

## 6. Escalabilidad

### 6.1 Escalado Horizontal

Sovereign Core esta disenado para escalar horizontalmente:

1. **Stateless**: Sin sesiones server-side. JWT contiene todos los claims necesarios.
2. **Pool de conexiones**: PostgreSQL pool configurable (2-20 conexiones por instancia).
3. **Multiples instancias**: Un load balancer distribuye trafico entre N instancias de Sovereign Core, todas conectadas al mismo PostgreSQL.
4. **WebSocket sticky sessions**: Para chat en tiempo real, el load balancer necesita sticky sessions por `platform:userId`.

### 6.2 PostgreSQL como Cuello de Botella

Para el volumen actual (441 plataformas, ~1B+ usuarios potenciales):

- **Read replicas**: Las consultas de lectura (analytics, content listing, public profiles) pueden dirigirse a replicas.
- **Particionamiento**: La tabla `content` puede particionarse por `platform` si el volumen lo requiere.
- **Indices GIN en JSONB**: Para busquedas dentro de `body` y `metadata`, PostgreSQL soporta indices GIN nativos.
- **Connection pooling**: PgBouncer o Odyssey frente a PostgreSQL para manejar miles de conexiones.

### 6.3 Limites Actuales

| Metrica | Limite |
|---|---|
| Body JSON request | 10 MB |
| URL-encoded request | 2 MB |
| Archivo upload | 50 MB |
| Mensaje WebSocket | 64 KB |
| Rate limit global | 200 req / 15 min / IP |
| Rate limit auth | 10 req / 15 min / IP |
| Pool DB minimo | 2 conexiones |
| Pool DB maximo | 20 conexiones |
| Heartbeat WebSocket | 30 segundos |

## 7. Principios de Diseno de API

### 7.1 Convenciones REST

- **Versionado**: Prefijo `/v1/` en todas las rutas
- **Formato**: JSON en request y response
- **Paginacion**: Query params `page` y `limit`, response incluye `pagination` object
- **Errores**: RFC 7807 Problem Details (`{ type, title, detail, status, instance }`)
- **IDs**: UUID v4 para todas las entidades
- **Timestamps**: ISO 8601 con timezone UTC
- **Soft deletes**: Columna `status = 'deleted'` en vez de DELETE fisico

### 7.2 Formato de Respuesta

Todas las respuestas exitosas siguen el formato:

```json
{
  "status": "ok",
  "data": { ... },
  "pagination": {
    "page": 1,
    "limit": 20,
    "total": 150,
    "totalPages": 8,
    "hasNext": true,
    "hasPrev": false
  }
}
```

### 7.3 Autenticacion en Requests

```
Authorization: Bearer <jwt_token>
X-Platform: correo-soberano
X-Request-Id: <auto-generated>
Content-Type: application/json
```

## 8. Conclusion

Sovereign Core resuelve el problema de servir 441+ plataformas desde un unico backend mantenible, seguro y escalable. El uso de PostgreSQL JSONB elimina la necesidad de migraciones por plataforma, mientras que la identidad JWT unificada permite a los ciudadanos de Ierahkwa moverse libremente entre todas las plataformas con una sola cuenta.

La arquitectura modular permite agregar nuevas funcionalidades sin afectar los modulos existentes, y el audit trail hash-chained garantiza trazabilidad completa para un gobierno digital transparente.

---

*Gobierno Soberano de Ierahkwa Ne Kanienke -- 2026*
