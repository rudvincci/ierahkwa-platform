'use strict';

// ============================================================
// Ierahkwa API Gateway v3.6.0 — MameyNode v4.2
// Production-ready Express gateway with:
//   - JWT authentication (HS256 / RS256)
//   - Tier-based access control (member / resident / citizen)
//   - RFC 7807 error responses
//   - Structured JSON logging (Pino-compatible)
//   - Immutable audit trail on financial & governance routes
//   - Circuit breaker resilience for downstream services
//   - Input validation & body sanitization
//   - Graceful shutdown (SIGTERM / SIGINT)
// ============================================================

const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const compression = require('compression');
const { RateLimiterMemory } = require('rate-limiter-flexible');

// ── Shared Modules ──────────────────────────────────────────
const { createLogger }                              = require('../shared/logger');
const { errorMiddleware, notFoundHandler, AppError } = require('../shared/error-handler');
const { createAuditLogger }                          = require('../shared/audit');
const { createCircuitBreaker }                       = require('../shared/resilience');
const { jwtAuth }                                    = require('./middleware/auth');
const { globalSanitizer }                            = require('./middleware/validate');

// ── Initialize ──────────────────────────────────────────────
const log   = createLogger('api-gateway');
const audit = createAuditLogger('api-gateway', {
  alertOnCritical: true,
  hashChain: true
});

const app  = express();
const PORT = process.env.PORT || 3000;
const ENV  = process.env.NODE_ENV || 'development';

// ── Circuit Breakers for downstream services ────────────────
const breakers = {
  bdet:     createCircuitBreaker('bdet-service',     { threshold: 5, resetTimeout: 30000, onStateChange: (n, o, s) => log.warn(`Circuit ${n}: ${o} → ${s}`) }),
  chain:    createCircuitBreaker('chain-service',     { threshold: 5, resetTimeout: 30000, onStateChange: (n, o, s) => log.warn(`Circuit ${n}: ${o} → ${s}`) }),
  invest:   createCircuitBreaker('invest-service',    { threshold: 5, resetTimeout: 30000, onStateChange: (n, o, s) => log.warn(`Circuit ${n}: ${o} → ${s}`) }),
  commerce: createCircuitBreaker('commerce-service',  { threshold: 5, resetTimeout: 30000, onStateChange: (n, o, s) => log.warn(`Circuit ${n}: ${o} → ${s}`) })
};

// Expose breakers to route handlers via app.locals
app.locals.breakers = breakers;
app.locals.audit    = audit;
app.locals.log      = log;

// ============================================================
// MIDDLEWARE STACK (order matters)
// ============================================================

// 1. Security headers
app.use(helmet({
  contentSecurityPolicy: ENV === 'production' ? undefined : false,
  crossOriginEmbedderPolicy: false
}));

// 2. CORS
const corsOptions = {
  origin: process.env.CORS_ORIGIN || '*',
  methods: ['GET', 'POST', 'PUT', 'PATCH', 'DELETE', 'OPTIONS'],
  allowedHeaders: ['Content-Type', 'Authorization', 'X-Request-Id', 'X-Tenant-Id'],
  exposedHeaders: ['X-Request-Id', 'X-RateLimit-Remaining'],
  credentials: true,
  maxAge: 86400
};
app.use(cors(corsOptions));

// 3. Compression
app.use(compression());

// 4. Body parsing with limits
app.use(express.json({ limit: '10mb' }));
app.use(express.urlencoded({ extended: true, limit: '2mb' }));

// 5. Request ID + structured request logging
app.use(log.requestLogger());

// 6. Global body sanitization (prototype pollution defense)
app.use(globalSanitizer);

// 7. Rate limiting — memory-based, 200 req/60s per IP
const rateLimiter = new RateLimiterMemory({
  points: parseInt(process.env.RATE_LIMIT_POINTS, 10) || 200,
  duration: parseInt(process.env.RATE_LIMIT_DURATION, 10) || 60
});

app.use(async (req, res, next) => {
  try {
    const rateLimitRes = await rateLimiter.consume(req.ip);
    res.set('X-RateLimit-Remaining', rateLimitRes.remainingPoints);
    next();
  } catch (rateLimitRes) {
    res.set('Retry-After', Math.ceil(rateLimitRes.msBeforeNext / 1000));
    next(new AppError('RATE_LIMITED', `Rate limit exceeded — retry in ${Math.ceil(rateLimitRes.msBeforeNext / 1000)}s`));
  }
});

// 8. JWT Authentication (skips public routes automatically)
app.use(jwtAuth);

// 9. Audit middleware — log all write operations automatically
app.use(audit.middleware({
  auditReads: false,
  pathFilter: (path) => path.startsWith('/v1/')
}));

// ============================================================
// HEALTH & READINESS PROBES
// ============================================================

app.get('/health', (req, res) => {
  res.json({
    status: 'ok',
    version: '3.6.0',
    node: 'MameyNode v4.2',
    uptime: Math.floor(process.uptime()),
    timestamp: new Date().toISOString()
  });
});

app.get('/ready', (req, res) => {
  const circuitStates = Object.entries(breakers).map(([name, b]) => ({
    name,
    state: b.getState()
  }));

  const allClosed = circuitStates.every(c => c.state !== 'OPEN');

  res.status(allClosed ? 200 : 503).json({
    ready: allClosed,
    platforms: 194,
    engines: 42,
    circuits: circuitStates
  });
});

app.get('/metrics', (req, res) => {
  const circuitStats = Object.entries(breakers).map(([, b]) => b.getStats());
  res.json({
    uptime: Math.floor(process.uptime()),
    memory: process.memoryUsage(),
    circuits: circuitStats,
    pid: process.pid,
    version: '3.6.0'
  });
});

// ============================================================
// API ROUTES — v1
// ============================================================

// Auth (public routes handled by jwtAuth skip logic)
app.use('/v1/auth',     require('./routes/auth'));

// Financial (audited + circuit-protected)
app.use('/v1/bdet',     require('./routes/bdet'));
app.use('/v1/invest',   require('./routes/invest'));
app.use('/v1/commerce', require('./routes/commerce'));
app.use('/v1/chain',    require('./routes/chain'));

// Communication
app.use('/v1/mail',     require('./routes/mail'));
app.use('/v1/social',   require('./routes/social'));
app.use('/v1/voice',    require('./routes/voice'));

// Content
app.use('/v1/search',   require('./routes/search'));
app.use('/v1/video',    require('./routes/video'));
app.use('/v1/music',    require('./routes/music'));
app.use('/v1/docs',     require('./routes/docs'));
app.use('/v1/news',     require('./routes/news'));
app.use('/v1/wiki',     require('./routes/wiki'));
app.use('/v1/edu',      require('./routes/edu'));

// Services
app.use('/v1/lodging',  require('./routes/lodging'));
app.use('/v1/artisan',  require('./routes/artisan'));
app.use('/v1/jobs',     require('./routes/jobs'));
app.use('/v1/renta',    require('./routes/renta'));
app.use('/v1/map',      require('./routes/map'));

// Governance & AI
app.use('/v1/atabey',   require('./routes/atabey'));
app.use('/v1/ai',       require('./routes/ai'));

// ============================================================
// ERROR HANDLING
// ============================================================

// 404 — unmatched routes
app.use(notFoundHandler);

// Global error handler — RFC 7807 Problem Details
app.use(errorMiddleware('api-gateway', log));

// ============================================================
// SERVER + GRACEFUL SHUTDOWN
// ============================================================

let server;

function startServer() {
  server = app.listen(PORT, () => {
    log.info(`Ierahkwa API Gateway v3.6.0 started`, {
      port: PORT,
      env: ENV,
      platforms: 194,
      routes: 22,
      node: 'MameyNode v4.2',
      pid: process.pid
    });

    audit.record({
      category: audit.CATEGORIES.SYSTEM_STARTUP,
      action: 'gateway_started',
      risk: audit.RISK.LOW,
      details: { port: PORT, env: ENV, version: '3.6.0' }
    });
  });

  server.keepAliveTimeout = 65000;
  server.headersTimeout = 66000;

  return server;
}

/**
 * Graceful shutdown — drain connections before exit
 */
function shutdown(signal) {
  log.info(`${signal} received — starting graceful shutdown`, { signal });

  audit.record({
    category: audit.CATEGORIES.SYSTEM_SHUTDOWN,
    action: 'gateway_shutdown',
    risk: audit.RISK.LOW,
    details: { signal, uptime: Math.floor(process.uptime()) }
  });

  if (!server) {
    process.exit(0);
    return;
  }

  // Stop accepting new connections
  server.close((err) => {
    if (err) {
      log.error('Error during shutdown', { err });
      process.exit(1);
    }

    log.info('All connections drained — exiting', { uptime: Math.floor(process.uptime()) });
    process.exit(0);
  });

  // Force exit after 30s if connections don't drain
  const forceTimeout = setTimeout(() => {
    log.warn('Forced shutdown — connections did not drain in 30s');
    process.exit(1);
  }, 30000);

  // Don't keep process alive just for this timer
  if (forceTimeout.unref) forceTimeout.unref();
}

// Handle termination signals
process.on('SIGTERM', () => shutdown('SIGTERM'));
process.on('SIGINT',  () => shutdown('SIGINT'));

// Handle uncaught errors
process.on('uncaughtException', (err) => {
  log.fatal('Uncaught exception', { err: { message: err.message, stack: err.stack } });
  shutdown('uncaughtException');
});

process.on('unhandledRejection', (reason) => {
  log.error('Unhandled rejection', { reason: reason instanceof Error ? { message: reason.message, stack: reason.stack } : reason });
});

// Start server if this is the main module
if (require.main === module) {
  startServer();
}

// ============================================================
// Exports (for testing)
// ============================================================
module.exports = { app, startServer, shutdown };
