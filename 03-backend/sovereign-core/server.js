'use strict';

// ============================================================
// Ierahkwa Sovereign Core v1.0.0 — Universal Backend
// Production-ready Express server for 441+ HTML platforms
//   - PostgreSQL persistence with migration system
//   - JWT authentication (HS256)
//   - RFC 7807 error responses
//   - Structured JSON logging (Pino-compatible)
//   - Immutable audit trail (hash-chained)
//   - OWASP security middleware stack
//   - WebSocket server for real-time chat (/ws/chat)
//   - Graceful shutdown (SIGTERM / SIGINT)
// ============================================================

const http    = require('http');
const express = require('express');
const cors    = require('cors');
const helmet  = require('helmet');
const compression = require('compression');
const { WebSocketServer } = require('ws');

// ── Shared Modules ──────────────────────────────────────────
const { createLogger }                              = require('../shared/logger');
const { errorMiddleware, notFoundHandler, AppError } = require('../shared/error-handler');
const { createAuditLogger }                          = require('../shared/audit');
const {
  requestId: requestIdMiddleware,
  securityHeaders,
  sanitizeInput,
  rateLimiterMiddleware
} = require('../shared/security');

// ── Local Modules ───────────────────────────────────────────
const config = require('./src/config');
const db     = require('./src/db');

// ── Initialize ──────────────────────────────────────────────
const log   = createLogger('sovereign-core');
const audit = createAuditLogger('sovereign-core', {
  alertOnCritical: true,
  hashChain: true
});

const app  = express();
const PORT = config.port;
const ENV  = config.env;

// Expose shared instances to route handlers
app.locals.log    = log;
app.locals.audit  = audit;
app.locals.db     = db;
app.locals.config = config;

// ============================================================
// MIDDLEWARE STACK (order matters)
// ============================================================

// 1. Security headers (helmet)
app.use(helmet({
  contentSecurityPolicy: ENV === 'production' ? undefined : false,
  crossOriginEmbedderPolicy: false
}));

// 2. CORS
const corsOptions = {
  origin: config.corsOrigins,
  methods: ['GET', 'POST', 'PUT', 'PATCH', 'DELETE', 'OPTIONS'],
  allowedHeaders: ['Content-Type', 'Authorization', 'X-Request-Id', 'X-Tenant-Id', 'X-Platform-Id'],
  exposedHeaders: ['X-Request-Id', 'X-RateLimit-Remaining', 'X-Total-Count'],
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
app.use(requestIdMiddleware);
app.use(log.requestLogger());

// 6. Security headers (additional — OWASP hardening beyond helmet)
app.use(securityHeaders);

// 7. Input sanitization (prototype pollution, XSS, injection defense)
app.use(sanitizeInput);

// 8. Rate limiting — 200 req/15min per IP (global)
const globalRateLimiter = rateLimiterMiddleware({
  windowMs: 15 * 60 * 1000,
  max: parseInt(process.env.RATE_LIMIT_MAX, 10) || 200,
  message: 'Rate limit exceeded — retry later'
});
app.use(globalRateLimiter);

// 9. Audit middleware — log all write operations automatically
app.use(audit.middleware({
  auditReads: false,
  pathFilter: (path) => path.startsWith('/v1/')
}));

// ============================================================
// HEALTH & READINESS PROBES
// ============================================================

app.get('/health', (_req, res) => {
  res.json({
    status: 'ok',
    service: 'sovereign-core',
    version: '1.0.0',
    node: 'MameyNode v4.2',
    uptime: Math.floor(process.uptime()),
    timestamp: new Date().toISOString()
  });
});

app.get('/ready', async (_req, res) => {
  let dbReady = false;
  try {
    const result = await db.query('SELECT 1 AS alive');
    dbReady = result.rows.length > 0;
  } catch {
    dbReady = false;
  }

  const status = dbReady ? 200 : 503;
  res.status(status).json({
    ready: dbReady,
    service: 'sovereign-core',
    platforms: 441,
    modules: ['auth', 'users', 'payments', 'messages', 'votes', 'storage', 'analytics', 'content'],
    database: dbReady ? 'connected' : 'unavailable',
    timestamp: new Date().toISOString()
  });
});

app.get('/metrics', (_req, res) => {
  const mem = process.memoryUsage();
  res.json({
    service: 'sovereign-core',
    version: '1.0.0',
    uptime: Math.floor(process.uptime()),
    memory: {
      rss: Math.round(mem.rss / 1024 / 1024) + 'MB',
      heapUsed: Math.round(mem.heapUsed / 1024 / 1024) + 'MB',
      heapTotal: Math.round(mem.heapTotal / 1024 / 1024) + 'MB',
      external: Math.round(mem.external / 1024 / 1024) + 'MB'
    },
    pid: process.pid,
    platforms: 441,
    timestamp: new Date().toISOString()
  });
});

// ============================================================
// API ROUTES — v1
// ============================================================

// Auth — login, register, logout, refresh, profile
app.use('/v1/auth',       require('./src/modules/auth/routes'));

// Users — CRUD, roles, tiers
app.use('/v1/users',      require('./src/modules/users/routes'));

// Payments — BDET transactions, wallets, invoices
app.use('/v1/payments',   require('./src/modules/payments/routes'));

// Messages — sovereign messaging, notifications
app.use('/v1/messages',   require('./src/modules/messaging/routes'));

// Votes — proposals, ballots, results (governance)
app.use('/v1/votes',      require('./src/modules/voting/routes'));

// Storage — file upload, media library
app.use('/v1/storage',    require('./src/modules/storage/routes'));

// Analytics — platform metrics, dashboards
app.use('/v1/analytics',  require('./src/modules/analytics/routes'));

// WiFi Bridge — sovereign-core ↔ wifi-soberano integration
app.use('/v1/wifi',       require('./src/modules/wifi-bridge/routes'));

// Content — dynamic per-platform items (/:platform/items)
app.use('/v1',            require('./src/modules/content/routes'));

// ============================================================
// ERROR HANDLING
// ============================================================

// 404 — unmatched routes
app.use(notFoundHandler);

// Global error handler — RFC 7807 Problem Details
app.use(errorMiddleware('sovereign-core', log));

// ============================================================
// HTTP SERVER + WEBSOCKET
// ============================================================

const httpServer = http.createServer(app);

// ── WebSocket Server — /ws/chat ─────────────────────────────
const wss = new WebSocketServer({
  server: httpServer,
  path: '/ws/chat',
  maxPayload: 64 * 1024 // 64 KB per message
});

// Connected clients indexed by platform + userId
const wsClients = new Map();

wss.on('connection', (ws, req) => {
  const url = new URL(req.url, `http://${req.headers.host}`);
  const platform = url.searchParams.get('platform') || 'global';
  const userId   = url.searchParams.get('userId')   || `anon-${Date.now()}`;
  const clientId = `${platform}:${userId}`;

  ws.isAlive = true;
  ws.platform = platform;
  ws.userId = userId;
  ws.clientId = clientId;
  wsClients.set(clientId, ws);

  log.info('WebSocket connected', { clientId, platform, userId, totalClients: wsClients.size });

  // Send welcome
  ws.send(JSON.stringify({
    type: 'connected',
    clientId,
    platform,
    timestamp: new Date().toISOString(),
    message: 'Sovereign Core — real-time channel active'
  }));

  ws.on('pong', () => { ws.isAlive = true; });

  ws.on('message', (data) => {
    let msg;
    try {
      msg = JSON.parse(data.toString());
    } catch {
      ws.send(JSON.stringify({ type: 'error', detail: 'Invalid JSON' }));
      return;
    }

    log.debug('WebSocket message', { clientId, type: msg.type });

    // Broadcast to same platform
    if (msg.type === 'chat') {
      const outgoing = JSON.stringify({
        type: 'chat',
        from: userId,
        platform,
        body: (msg.body || '').slice(0, 2000), // limit message size
        timestamp: new Date().toISOString()
      });

      for (const [, client] of wsClients) {
        if (client.readyState === 1 && client.platform === platform) {
          client.send(outgoing);
        }
      }
    }

    // Direct message
    if (msg.type === 'dm' && msg.to) {
      const target = wsClients.get(`${platform}:${msg.to}`);
      if (target && target.readyState === 1) {
        target.send(JSON.stringify({
          type: 'dm',
          from: userId,
          body: (msg.body || '').slice(0, 2000),
          timestamp: new Date().toISOString()
        }));
      }
    }
  });

  ws.on('close', () => {
    wsClients.delete(clientId);
    log.info('WebSocket disconnected', { clientId, totalClients: wsClients.size });
  });

  ws.on('error', (err) => {
    log.error('WebSocket error', { clientId, err: { message: err.message } });
    wsClients.delete(clientId);
  });
});

// Heartbeat — ping every 30s, terminate dead sockets
const heartbeatInterval = setInterval(() => {
  for (const [id, ws] of wsClients) {
    if (!ws.isAlive) {
      log.debug('WebSocket heartbeat timeout', { clientId: id });
      wsClients.delete(id);
      ws.terminate();
      continue;
    }
    ws.isAlive = false;
    ws.ping();
  }
}, 30000);

wss.on('close', () => {
  clearInterval(heartbeatInterval);
});

// ============================================================
// SERVER START + GRACEFUL SHUTDOWN
// ============================================================

let server;

async function startServer() {
  // Initialize database (run migrations)
  try {
    await db.initialize();
    log.info('Database initialized successfully');
  } catch (err) {
    log.error('Database initialization failed', { err: { message: err.message, stack: err.stack } });
    if (ENV === 'production') {
      log.fatal('Cannot start in production without database — exiting');
      process.exit(1);
    }
    log.warn('Continuing without database in development mode');
  }

  server = httpServer.listen(PORT, () => {
    log.info('Ierahkwa Sovereign Core v1.0.0 started', {
      port: PORT,
      env: ENV,
      platforms: 441,
      modules: 8,
      websocket: '/ws/chat',
      node: 'MameyNode v4.2',
      pid: process.pid
    });

    audit.record({
      category: audit.CATEGORIES.SYSTEM_STARTUP,
      action: 'sovereign_core_started',
      risk: audit.RISK.LOW,
      details: { port: PORT, env: ENV, version: '1.0.0', platforms: 441 }
    });
  });

  server.keepAliveTimeout = 65000;
  server.headersTimeout = 66000;

  return server;
}

/**
 * Graceful shutdown — drain connections, close DB pool, then exit
 */
function shutdown(signal) {
  log.info(`${signal} received — starting graceful shutdown`, { signal });

  audit.record({
    category: audit.CATEGORIES.SYSTEM_SHUTDOWN,
    action: 'sovereign_core_shutdown',
    risk: audit.RISK.LOW,
    details: { signal, uptime: Math.floor(process.uptime()) }
  });

  // Close WebSocket server
  wss.close(() => {
    log.info('WebSocket server closed');
  });

  // Close all WebSocket clients
  for (const [, ws] of wsClients) {
    ws.close(1001, 'Server shutting down');
  }
  wsClients.clear();

  if (!server) {
    db.end().then(() => process.exit(0)).catch(() => process.exit(0));
    return;
  }

  // Stop accepting new HTTP connections
  server.close(async (err) => {
    if (err) {
      log.error('Error during HTTP server shutdown', { err: { message: err.message } });
    }

    // Close database pool
    try {
      await db.end();
      log.info('Database pool closed');
    } catch (dbErr) {
      log.error('Error closing database pool', { err: { message: dbErr.message } });
    }

    log.info('All connections drained — exiting', { uptime: Math.floor(process.uptime()) });
    process.exit(err ? 1 : 0);
  });

  // Force exit after 30s if connections don't drain
  const forceTimeout = setTimeout(() => {
    log.warn('Forced shutdown — connections did not drain in 30s');
    process.exit(1);
  }, 30000);

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
  log.error('Unhandled rejection', {
    reason: reason instanceof Error
      ? { message: reason.message, stack: reason.stack }
      : reason
  });
});

// Start server if this is the main module
if (require.main === module) {
  startServer();
}

// ============================================================
// Exports (for testing)
// ============================================================
module.exports = { app, httpServer, wss, startServer, shutdown };
