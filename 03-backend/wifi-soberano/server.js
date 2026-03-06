'use strict';

const express = require('express');
const helmet = require('helmet');
const cors = require('cors');
const compression = require('compression');
const rateLimit = require('express-rate-limit');
const { Pool } = require('pg');
const { createClient } = require('redis');
const { WebSocketServer } = require('ws');
const http = require('http');
const pino = require('pino');

// ─── Config ───
const PORT = process.env.PORT || 3095;
const logger = pino({ level: process.env.LOG_LEVEL || 'info' });

// ─── Database ───
const pool = new Pool({
  connectionString: process.env.DATABASE_URL || 'postgresql://ierahkwa:ierahkwa@localhost:5432/wifi_soberano',
  max: 20,
  idleTimeoutMillis: 30000,
  connectionTimeoutMillis: 5000,
  ssl: process.env.NODE_ENV === 'production' ? { rejectUnauthorized: false } : false
});

// ─── Redis ───
const redis = createClient({ url: process.env.REDIS_URL || 'redis://localhost:6379' });
redis.on('error', (err) => logger.error({ err }, 'Redis error'));
redis.connect().catch((err) => logger.warn({ err }, 'Redis not available — using in-memory fallback'));

// ─── Express App ───
const app = express();
const server = http.createServer(app);

// Security middleware
app.use(helmet({
  contentSecurityPolicy: process.env.NODE_ENV === 'production' ? undefined : false,
  crossOriginEmbedderPolicy: false
}));
app.use(cors({ origin: process.env.CORS_ORIGINS?.split(',') || '*' }));
app.use(compression());
app.use(express.json({ limit: '1mb' }));
app.use(express.urlencoded({ extended: true }));

// Rate limiting
const limiter = rateLimit({
  windowMs: 15 * 60 * 1000,
  max: 200,
  standardHeaders: true,
  legacyHeaders: false,
  message: { error: 'Demasiadas solicitudes. Intente más tarde.' }
});
app.use('/api/', limiter);

// Captive portal rate limit (more permissive)
const portalLimiter = rateLimit({
  windowMs: 60 * 1000,
  max: 30,
  message: { error: 'Espere un momento antes de intentar de nuevo.' }
});

// ─── Health Check ───
app.get('/health', (req, res) => {
  res.json({
    status: 'ok',
    service: 'wifi-soberano',
    version: '1.0.0',
    uptime: process.uptime(),
    timestamp: new Date().toISOString()
  });
});

// ─── Routes ───
const authRoutes = require('./routes/auth');
const planRoutes = require('./routes/plans');
const sessionRoutes = require('./routes/sessions');
const paymentRoutes = require('./routes/payments');
const analyticsRoutes = require('./routes/analytics');
const fleetRoutes = require('./routes/fleet');
const adminRoutes = require('./routes/admin');

// Public routes (captive portal)
app.use('/api/v1/wifi', portalLimiter, authRoutes(pool, redis, logger));
app.use('/api/v1/wifi', portalLimiter, planRoutes(pool, logger));
app.use('/api/v1/wifi', sessionRoutes(pool, redis, logger));
app.use('/api/v1/wifi', paymentRoutes(pool, redis, logger));

// Admin routes (authenticated)
app.use('/api/v1/wifi/admin', adminRoutes(pool, redis, logger));
app.use('/api/v1/wifi/admin', analyticsRoutes(pool, logger));
app.use('/api/v1/wifi/admin', fleetRoutes(pool, logger));

// ─── WebSocket (Real-time Dashboard) ───
const wss = new WebSocketServer({ server, path: '/ws/wifi' });
wss.on('connection', (ws) => {
  logger.info('WebSocket client connected — dashboard');
  ws.on('close', () => logger.info('WebSocket client disconnected'));
});

// Broadcast to all admin WebSocket clients
function broadcast(data) {
  wss.clients.forEach((client) => {
    if (client.readyState === 1) client.send(JSON.stringify(data));
  });
}

// ─── Vigilancia Soberana — IP Tracking + VIP Protection ───
app.use((req, res, next) => {
  const ip = req.headers['x-forwarded-for']?.split(',')[0] || req.socket.remoteAddress;
  const ua = req.headers['user-agent'] || 'unknown';
  const path = req.originalUrl;
  const method = req.method;
  const timestamp = new Date().toISOString();

  // Log every request for forensic analysis
  const logEntry = { timestamp, ip, method, path, ua, referer: req.headers.referer || '' };

  // Store in Redis for real-time dashboard
  redis.lPush('wifi:vigilancia:log', JSON.stringify(logEntry)).catch(() => {});
  redis.lTrim('wifi:vigilancia:log', 0, 9999).catch(() => {});

  // Broadcast to admin dashboard
  broadcast({ type: 'vigilancia', data: logEntry });

  // VIP keyword detection in search queries
  if (req.query.q || req.body?.query) {
    const searchQuery = (req.query.q || req.body?.query || '').toLowerCase();
    checkVIPAlert(searchQuery, ip, ua, timestamp);
  }

  next();
});

// VIP Protection — Atabey AI
async function checkVIPAlert(query, ip, ua, timestamp) {
  try {
    const vipResult = await pool.query('SELECT name FROM vip_protected WHERE LOWER(name) = ANY(string_to_array($1, \' \'))', [query]);
    if (vipResult.rows.length > 0) {
      const alert = {
        type: 'vip_alert',
        level: 'critical',
        timestamp,
        ip,
        ua,
        query,
        matched: vipResult.rows.map(r => r.name),
        action: 'atabey_monitoring_activated'
      };
      logger.warn(alert, 'VIP ALERT — Atabey AI activated');
      broadcast({ type: 'vip_alert', data: alert });

      // Store alert
      await pool.query(
        'INSERT INTO vigilancia_alerts (ip, user_agent, query, matched_vip, level, created_at) VALUES ($1, $2, $3, $4, $5, NOW())',
        [ip, ua, query, JSON.stringify(alert.matched), 'critical']
      ).catch(() => {});
    }
  } catch (err) {
    logger.error({ err }, 'VIP check error');
  }
}

// ─── Error Handler ───
app.use((err, req, res, _next) => {
  logger.error({ err }, 'Unhandled error');
  res.status(500).json({ error: 'Error interno del servidor' });
});

// ─── Start ───
server.listen(PORT, '0.0.0.0', () => {
  logger.info(`WiFi Soberano server running on port ${PORT}`);
  logger.info(`WebSocket dashboard: ws://localhost:${PORT}/ws/wifi`);
  logger.info('Vigilancia Soberana: ACTIVE');
  logger.info('Atabey AI VIP Protection: ACTIVE');
});

// Graceful shutdown
process.on('SIGTERM', async () => {
  logger.info('SIGTERM received — shutting down');
  server.close();
  await pool.end();
  await redis.quit().catch(() => {});
  process.exit(0);
});
process.on('SIGINT', async () => {
  logger.info('SIGINT received — shutting down');
  server.close();
  await pool.end();
  await redis.quit().catch(() => {});
  process.exit(0);
});

module.exports = { app, pool, redis, broadcast };
