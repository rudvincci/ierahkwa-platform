'use strict';

const express = require('express');
const cors = require('cors');
const http = require('http');
const { WebSocketServer } = require('ws');
const { Store } = require('./lib/store');

const app = express();
const server = http.createServer(app);
const wss = new WebSocketServer({ server, path: '/ws' });

// Store instance
const store = new Store();

// Middleware
try {
  const { corsConfig } = require('../shared/security');
  app.use(cors(corsConfig()));
} catch (_) {
  app.use(cors());
}
app.use(express.json());

// Routes — each factory receives the store
app.use('/v1/providers', require('./routes/providers')(store));
app.use('/v1/services', require('./routes/services')(store));
app.use('/v1/bookings', require('./routes/bookings')(store));
app.use('/v1/reviews', require('./routes/reviews')(store));
app.use('/v1/categories', require('./routes/categories')());
app.use('/v1/availability', require('./routes/availability')(store));
app.use('/v1/locations', require('./routes/locations')(store));

// Health
app.get('/health', (req, res) => {
  const stats = store.getStats();
  res.json({
    service: 'SoberanoServicios',
    version: '1.0.0',
    status: 'operational',
    categories: 31,
    providerPercent: '92%',
    taxRate: '0%',
    languages: 43,
    ...stats
  });
});

// Stats
app.get('/v1/stats', (req, res) => res.json(store.getStats()));

// WebSocket — real-time booking updates, provider location
require('./services/realtime')(wss);

// Error handler
app.use((err, req, res, _next) => {
  console.error(err.stack || err.message);
  res.status(500).json({ error: 'Internal server error' });
});

const PORT = process.env.SERVICIOS_PORT || 4010;
server.listen(PORT, () => {
  console.log(`SoberanoServicios on :${PORT} — 92% to providers — 0% taxes`);
  console.log(`WebSocket: ws://localhost:${PORT}/ws`);
  console.log(`Providers: ${store.providers.size}, Categories: 31`);
});

module.exports = { app, server, store };
