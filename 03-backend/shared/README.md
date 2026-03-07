# Shared Utilities (@ierahkwa/shared)

> Modulos compartidos de seguridad, logging, validacion, resiliencia, auditoria y accesibilidad para todos los servicios backend de Ierahkwa.

## Descripcion

El paquete `@ierahkwa/shared` es la biblioteca central de utilidades que todos los microservicios backend de Ierahkwa consumen. Proporciona siete modulos fundamentales con cero dependencias externas (salvo las del propio Node.js), garantizando consistencia, seguridad y observabilidad a traves de toda la plataforma soberana.

Los modulos cubren las necesidades criticas de una plataforma financiera y de gobernanza: seguridad OWASP Top 10:2025 (CORS, rate limiting, sanitizacion, headers, tenant isolation), logging estructurado compatible con Pino, errores estandarizados RFC 7807, validacion de schemas con tipos especificos para WAMPUM (wallet addresses, montos con 8 decimales), patrones de resiliencia (circuit breaker, retry con backoff, timeout, bulkhead), auditoria inmutable con cadena de hashes SHA-256, y middleware de accesibilidad WCAG 2.2 AA.

Cada modulo se puede importar de forma independiente, lo que permite a los servicios usar solo lo que necesitan. La funcion `applySecurityMiddleware()` aplica toda la pila de seguridad en una sola linea.

## Modulos

### 1. security.js — Seguridad OWASP

Middleware de seguridad que cubre OWASP Top 10:2025, API Security y mas.

| Exportacion | Tipo | Descripcion |
|-------------|------|-------------|
| `corsConfig()` | Function | Configuracion CORS whitelist-based (A01) |
| `rateLimiterMiddleware(options)` | Function | Rate limiter en memoria con headers (API4) |
| `rateLimiters` | Object | Presets: auth (10/15min), api (100/15min), public (300/15min), upload (20/1h) |
| `sanitizeInput` | Middleware | Sanitiza XSS, SQL injection, null bytes, prototype pollution (A03) |
| `securityHeaders` | Middleware | Headers CSP, HSTS, X-Frame-Options, Referrer-Policy (A05) |
| `requestId` | Middleware | UUID unico por request, header X-Request-Id (A09) |
| `securityLogger(serviceName)` | Function | Logger de seguridad: authSuccess, authFailure, accessDenied, suspiciousActivity, dataAccess |
| `errorHandler(serviceName)` | Function | Error handler que nunca filtra detalles en produccion (A05) |
| `jwtUtils` | Object | generateSecret, isSecretStrong, generateRefreshToken, validateConfig (A07) |
| `fileUploadSecurity` | Object | Validacion por magic bytes, sanitizacion de filenames, limites (A08) |
| `tenantIsolation` | Middleware | Aislamiento multi-tenant por X-Tenant-Id (A01) |
| `applySecurityMiddleware(app, name)` | Function | Aplica toda la pila de seguridad en una linea |

### 2. logger.js — Logging Estructurado

Logger JSON estructurado compatible con Pino, con redaccion de campos sensibles.

| Exportacion | Tipo | Descripcion |
|-------------|------|-------------|
| `createLogger(serviceName, options)` | Function | Crea logger con niveles trace/debug/info/warn/error/fatal |
| `LOG_LEVELS` | Object | Mapa de niveles numericos (trace:10 ... fatal:60) |

Caracteristicas del logger:
- Redaccion automatica de campos sensibles (password, token, secret, creditCard, ssn)
- `requestLogger()` middleware con X-Request-Id y duracion
- `child(bindings)` para loggers con contexto adicional
- `startTimer(operation)` para medir duracion de operaciones
- Pretty mode en desarrollo, JSON puro en produccion
- Output a stdout (info) y stderr (warn+)

### 3. error-handler.js — Errores RFC 7807

Errores estandarizados siguiendo RFC 7807 Problem Details para APIs HTTP.

| Exportacion | Tipo | Descripcion |
|-------------|------|-------------|
| `ERROR_CODES` | Object | 25 codigos de error predefinidos en 7 categorias |
| `AppError` | Class | Clase de error con status, code, title, detail, toJSON() |
| `errorMiddleware(serviceName, logger)` | Function | Middleware de error RFC 7807 |
| `asyncHandler(fn)` | Function | Wrapper para rutas async |
| `notFoundHandler` | Middleware | Handler 404 estandarizado |

Categorias de error: Auth (1xxx), Validation (2xxx), Resources (3xxx), Rate Limiting (4xxx), Server (5xxx), BDET/Financial (6xxx), Tenant (7xxx).

### 4. validator.js — Validacion de Schemas

Validacion de esquemas zero-dependency con tipos especificos para la economia soberana.

| Exportacion | Tipo | Descripcion |
|-------------|------|-------------|
| `validate(schema)` | Function | Middleware de validacion (body, query, params) |
| `sanitizeBody(allowedFields)` | Function | Strip de campos no permitidos + coercion |
| `schemas` | Object | Schemas preconfigurados (pagination, bdetTransfer, userRegistration, idParam) |
| `t` | Object | Tipos: string, number, boolean, array, uuid, date, walletAddress, wampumAmount |

Tipos especificos soberanos:
- `t.walletAddress()` — Valida `0x` + 40 hex chars
- `t.wampumAmount()` — Valida montos WAMPUM (positivos, max 8 decimales)

### 5. resilience.js — Patrones de Resiliencia

Circuit Breaker, Retry con Backoff, Timeout y Bulkhead sin dependencias externas.

| Exportacion | Tipo | Descripcion |
|-------------|------|-------------|
| `createCircuitBreaker(name, options)` | Function | Circuit breaker CLOSED/OPEN/HALF_OPEN |
| `CircuitBreakerError` | Class | Error 503 cuando el circuito esta abierto |
| `retry(fn, options)` | Function | Retry con backoff exponencial + jitter |
| `withTimeout(fn, ms)` | Function | Timeout wrapper que falla si excede el limite |
| `TimeoutError` | Class | Error 504 por timeout |
| `createBulkhead(name, options)` | Function | Limita operaciones concurrentes (pool + cola) |
| `createResilientClient(name, options)` | Function | Cliente HTTP resiliente (breaker + retry + timeout) |

### 6. audit.js — Auditoria Inmutable

Trail de auditoria append-only con cadena de hashes SHA-256 para deteccion de manipulacion.

| Exportacion | Tipo | Descripcion |
|-------------|------|-------------|
| `createAuditLogger(serviceName, options)` | Function | Logger de auditoria con hash-chain |
| `AUDIT_CATEGORIES` | Object | 21 categorias (Transaction, Auth, Data, Admin, Governance, System) |
| `RISK_LEVELS` | Object | LOW, MEDIUM, HIGH, CRITICAL |

Metodos de conveniencia: `transaction()`, `loginSuccess()`, `loginFailure()`, `dataAccess()`, `dataModify()`, `adminAction()`, `permissionChange()`, `voteCast()`.

Incluye `middleware()` para auto-auditoria de todas las operaciones write, y `verifyChain()` para verificar integridad de la cadena de hashes.

### 7. accessibility.js — Accesibilidad WCAG 2.2

Middleware de accesibilidad conforme a WCAG 2.2 AA y el GAAD Pledge.

| Exportacion | Tipo | Descripcion |
|-------------|------|-------------|
| `accessibilityHeaders` | Middleware | Content-Language, Vary, Cache-Control |
| `accessibleErrorHandler` | Middleware | Errores con campos para screen readers |
| `htmlAccessibilityCheck(html)` | Function | Auditoria de HTML (10 reglas WCAG) |
| `createA11yAuditRoute(app)` | Function | Ruta GET /api/accessibility/status |
| `respectReducedMotion` | Middleware | Detecta prefers-reduced-motion |
| `respectColorScheme` | Middleware | Detecta prefers-color-scheme |
| `applyAccessibilityMiddleware(app)` | Function | Aplica todos los middleware a11y |

## Uso

```javascript
// Aplicar toda la seguridad en una linea
const { applySecurityMiddleware } = require('../shared/security');
const logger = applySecurityMiddleware(app, 'mi-servicio');

// Logger estructurado
const { createLogger } = require('../shared/logger');
const log = createLogger('mi-servicio');
log.info('Servidor iniciado', { port: 3000 });

// Validacion de rutas
const { validate, t } = require('../shared/validator');
router.post('/transfer', validate({
  body: {
    from:   t.walletAddress({ required: true }),
    to:     t.walletAddress({ required: true }),
    amount: t.wampumAmount({ required: true, max: 1000000 })
  }
}), handler);

// Circuit Breaker
const { createCircuitBreaker } = require('../shared/resilience');
const breaker = createCircuitBreaker('bdet-api', { threshold: 5 });
const result = await breaker.execute(() => fetch('/api/balance'));

// Auditoria inmutable
const { createAuditLogger } = require('../shared/audit');
const audit = createAuditLogger('mi-servicio', { hashChain: true });
audit.transaction(req, { from, to, amount: 5000, currency: 'WMP' });

// Accesibilidad
const { applyAccessibilityMiddleware } = require('../shared/accessibility');
applyAccessibilityMiddleware(app);
```

## Instalacion

```bash
# Los servicios importan directamente via path relativo:
const { createLogger } = require('../shared/logger');
const { corsConfig } = require('../shared/security');
```

No requiere instalacion separada. Se consume como dependencia local por todos los servicios en `03-backend/`.

## Variables de Entorno

| Variable | Modulo | Descripcion | Default |
|----------|--------|-------------|---------|
| NODE_ENV | Todos | Entorno de ejecucion | development |
| LOG_LEVEL | logger | Nivel minimo (trace/debug/info/warn/error/fatal) | debug (dev), info (prod) |
| CORS_ORIGINS | security | Origenes permitidos (coma-separados) | http://localhost:3000 |
| JWT_SECRET | security | Clave JWT HS256 (min 32 chars) | — |

## Arquitectura

```
@ierahkwa/shared
  ├── security.js ──────── OWASP Top 10:2025 (11 exportaciones)
  ├── logger.js ────────── Logging estructurado Pino-compatible
  ├── error-handler.js ─── RFC 7807 Problem Details (25 codigos)
  ├── validator.js ─────── Schema validation (8 tipos, 4 schemas)
  ├── resilience.js ────── Circuit Breaker + Retry + Timeout + Bulkhead
  ├── audit.js ─────────── Hash-chain audit trail (21 categorias)
  └── accessibility.js ── WCAG 2.2 AA + GAAD Pledge

Consumidores:
  api-gateway          → security, logger, error-handler, audit, resilience, validator
  blockchain-api       → security
  conferencia-soberana → security
  empresa-soberana     → security
  (todos los servicios backend de Ierahkwa)
```

## Parte de

**Ierahkwa Ne Kanienke** — Plataforma Soberana Digital
