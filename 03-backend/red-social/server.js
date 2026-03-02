'use strict';

const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const http = require('http');
const { WebSocketServer } = require('ws');
const db = require('./db');

const app = express();
const server = http.createServer(app);
const wss = new WebSocketServer({ server, path: '/ws' });

// Middleware
try {
  const { corsConfig } = require('../shared/security');
  app.use(helmet());
  app.use(cors(corsConfig()));
} catch (_) {
  app.use(helmet());
  app.use(cors());
}
app.use(express.json());

// ── Social Routes (use existing modules) ─────────────

app.use('/v1/posts', require('./posts'));
app.use('/v1/trading', require('./trading'));
app.use('/v1/chat', require('./chat'));

// ── Health ───────────────────────────────────────────

app.get('/health', (req, res) => {
  let dbStats;
  try { dbStats = db.stats(); } catch (_) { dbStats = { connected: false }; }
  res.json({
    service: 'Red Social Soberana',
    version: '1.0.0',
    status: 'operational',
    features: ['posts', 'chat', 'trading', 'tips', 'repost'],
    currency: 'Wampum (WMP)',
    taxRate: '0%',
    db: dbStats
  });
});

// ── WebSocket — live social updates ──────────────────

wss.on('connection', (ws) => {
  ws._userId = null;
  ws._rooms = new Set();

  ws.on('message', (raw) => {
    try {
      const msg = JSON.parse(raw);
      if (msg.type === 'auth') ws._userId = msg.userId;
      if (msg.type === 'subscribe') ws._rooms.add(msg.channel);
      if (msg.type === 'chat') {
        // Relay chat message to conversation subscribers
        const channel = `conv:${msg.conversationId}`;
        wss.clients.forEach(c => {
          if (c !== ws && c.readyState === 1 && c._rooms.has(channel)) {
            c.send(JSON.stringify({ type: 'chat_message', data: msg }));
          }
        });
      }
    } catch (_) { /* ignore malformed */ }
  });
});

// ── Error Handler ────────────────────────────────────

app.use((err, req, res, _next) => {
  console.error(err.stack || err.message);
  res.status(500).json({ error: 'Internal server error' });
});

// ── Start ────────────────────────────────────────────

const PORT = process.env.RED_SOCIAL_PORT || 4003;

(async () => {
  try {
    await db.initialize();
    console.log('PostgreSQL initialized');
  } catch (e) {
    console.warn('PostgreSQL not available, running in degraded mode:', e.message);
  }
  server.listen(PORT, () => {
    console.log(`Red Social Soberana on :${PORT} — posts, chat, trading — 0% taxes`);
    console.log(`WebSocket: ws://localhost:${PORT}/ws`);
  });
})();

// Graceful shutdown
async function shutdown() {
  server.close();
  try { await db.end(); } catch (_) { /* ignore */ }
  process.exit(0);
}
process.on('SIGTERM', shutdown);
process.on('SIGINT', shutdown);

module.exports = { app, server, wss };
