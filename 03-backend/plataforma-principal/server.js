'use strict';

const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const http = require('http');

const app = express();
const server = http.createServer(app);

// Middleware
try {
  const { corsConfig, requestId } = require('../shared/security');
  app.use(helmet());
  app.use(cors(corsConfig()));
  app.use(requestId);
} catch (_) {
  app.use(helmet());
  app.use(cors());
}
app.use(express.json({ limit: '10mb' }));

// ── Service Registry ─────────────────────────────────

const SERVICES = {
  // Banking (BDET)
  '/v1/wallet':        { target: 'http://bdet-bank:4000',           name: 'bdet-bank' },
  '/v1/payments':      { target: 'http://bdet-bank:4000',           name: 'bdet-bank' },
  '/v1/exchange':      { target: 'http://bdet-bank:4000',           name: 'bdet-bank' },
  '/v1/trading':       { target: 'http://bdet-bank:4000',           name: 'bdet-bank' },
  '/v1/remittance':    { target: 'http://bdet-bank:4000',           name: 'bdet-bank' },
  '/v1/escrow':        { target: 'http://bdet-bank:4000',           name: 'bdet-bank' },
  '/v1/loans':         { target: 'http://bdet-bank:4000',           name: 'bdet-bank' },
  '/v1/insurance':     { target: 'http://bdet-bank:4000',           name: 'bdet-bank' },
  '/v1/staking':       { target: 'http://bdet-bank:4000',           name: 'bdet-bank' },
  '/v1/treasury':      { target: 'http://bdet-bank:4000',           name: 'bdet-bank' },
  '/v1/fiscal':        { target: 'http://bdet-bank:4000',           name: 'bdet-bank' },
  // Social
  '/v1/feed':          { target: 'http://social-media:4001',        name: 'social-media' },
  '/v1/posts':         { target: 'http://social-media:4001',        name: 'social-media' },
  '/v1/stories':       { target: 'http://social-media:4001',        name: 'social-media' },
  '/v1/comments':      { target: 'http://social-media:4001',        name: 'social-media' },
  '/v1/likes':         { target: 'http://social-media:4001',        name: 'social-media' },
  '/v1/follow':        { target: 'http://social-media:4001',        name: 'social-media' },
  '/v1/profiles':      { target: 'http://social-media:4001',        name: 'social-media' },
  '/v1/groups':        { target: 'http://social-media:4001',        name: 'social-media' },
  '/v1/chat':          { target: 'http://social-media:4001',        name: 'social-media' },
  '/v1/notifications': { target: 'http://social-media:4001',        name: 'social-media' },
  '/v1/live':          { target: 'http://social-media:4001',        name: 'social-media' },
  // Specialized services
  '/v1/doctor':        { target: 'http://soberano-doctor:4002',     name: 'soberano-doctor' },
  '/v1/education':     { target: 'http://pupitresoberano:4003',     name: 'pupitresoberano' },
  '/v1/rides':         { target: 'http://soberano-uber:4004',       name: 'soberano-uber' },
  '/v1/food':          { target: 'http://soberano-eats:4005',       name: 'soberano-eats' },
  '/v1/vote':          { target: 'http://voto-soberano:4006',       name: 'voto-soberano' },
  '/v1/disputes':      { target: 'http://justicia-soberano:4007',   name: 'justicia-soberano' },
  '/v1/census':        { target: 'http://censo-soberano:4008',      name: 'censo-soberano' },
  '/v1/identity':      { target: 'http://soberano-id:4009',         name: 'soberano-id' },
  '/v1/services':      { target: 'http://soberano-servicios:4010',  name: 'soberano-servicios' },
  '/v1/bookings':      { target: 'http://soberano-servicios:4010',  name: 'soberano-servicios' },
  '/v1/mail':          { target: 'http://correo-soberano:4011',     name: 'correo-soberano' },
  '/v1/search':        { target: 'http://busqueda-soberana:4012',   name: 'busqueda-soberana' },
  '/v1/maps':          { target: 'http://mapa-soberano:4013',       name: 'mapa-soberano' },
  '/v1/cloud':         { target: 'http://nube-soberana:4014',       name: 'nube-soberana' },
  '/v1/farm':          { target: 'http://soberano-farm:4015',       name: 'soberano-farm' },
  '/v1/radio':         { target: 'http://radio-soberana:4016',      name: 'radio-soberana' },
  '/v1/cooperatives':  { target: 'http://cooperativa-soberana:4017', name: 'cooperativa-soberana' },
  '/v1/tourism':       { target: 'http://turismo-soberano:4018',    name: 'turismo-soberano' },
  '/v1/freelance':     { target: 'http://soberano-freelance:4019',  name: 'soberano-freelance' },
  '/v1/pos':           { target: 'http://soberano-pos:4020',        name: 'soberano-pos' },
  // Blockchain & ML
  '/v1/blocks':        { target: 'http://blockchain-api:3000',      name: 'blockchain-api' },
  '/v1/tokens':        { target: 'http://blockchain-api:3000',      name: 'blockchain-api' },
  '/v1/validators':    { target: 'http://blockchain-api:3000',      name: 'blockchain-api' },
  '/v1/governance':    { target: 'http://blockchain-api:3000',      name: 'blockchain-api' },
  '/v1/ml':            { target: 'http://ierahkwa-ml:3092',         name: 'ierahkwa-ml' },
  '/v1/anomaly':       { target: 'http://ierahkwa-ml:3092',         name: 'ierahkwa-ml' },
  '/v1/trust':         { target: 'http://ierahkwa-ml:3092',         name: 'ierahkwa-ml' },
  '/v1/vigilancia':    { target: 'http://vigilancia-soberana:3091',  name: 'vigilancia-soberana' },
};

// ── Proxy Setup ──────────────────────────────────────

try {
  const { createProxyMiddleware } = require('http-proxy-middleware');
  for (const [path, svc] of Object.entries(SERVICES)) {
    app.use(path, createProxyMiddleware({
      target: svc.target,
      changeOrigin: true,
      timeout: 30000,
      proxyTimeout: 30000,
      onError: (err, req, res) => {
        res.status(503).json({
          error: 'Service unavailable',
          service: svc.name,
          path: req.originalUrl
        });
      }
    }));
  }
} catch (_) {
  // http-proxy-middleware not installed — register stub routes
  for (const [path, svc] of Object.entries(SERVICES)) {
    app.use(path, (req, res) => {
      res.status(503).json({
        error: 'Service unavailable (proxy not configured)',
        service: svc.name,
        target: svc.target,
        hint: 'Install http-proxy-middleware or run services directly'
      });
    });
  }
}

// ── Service catalog ──────────────────────────────────

app.get('/v1/services-catalog', (req, res) => {
  const catalog = {};
  for (const [path, svc] of Object.entries(SERVICES)) {
    if (!catalog[svc.name]) catalog[svc.name] = { target: svc.target, routes: [] };
    catalog[svc.name].routes.push(path);
  }
  res.json({
    gateway: 'Red Soberana API Gateway',
    totalRoutes: Object.keys(SERVICES).length,
    services: Object.keys(catalog).length,
    catalog
  });
});

// ── Health ────────────────────────────────────────────

app.get('/health', async (req, res) => {
  // Deduplicate targets for health checks
  const targets = new Map();
  for (const [path, svc] of Object.entries(SERVICES)) {
    if (!targets.has(svc.name)) targets.set(svc.name, svc.target);
  }

  const checks = {};
  const timeout = 3000; // 3 second timeout per check

  await Promise.all([...targets.entries()].map(async ([name, target]) => {
    try {
      const controller = new AbortController();
      const timer = setTimeout(() => controller.abort(), timeout);
      await fetch(target + '/health', { signal: controller.signal });
      clearTimeout(timer);
      checks[name] = 'healthy';
    } catch (_) {
      checks[name] = 'down';
    }
  }));

  const healthy = Object.values(checks).filter(v => v === 'healthy').length;
  const total = targets.size;

  res.json({
    gateway: 'Red Soberana API Gateway',
    version: '1.0.0',
    totalRoutes: Object.keys(SERVICES).length,
    totalServices: total,
    healthyServices: healthy,
    status: healthy > total / 2 ? 'operational' : healthy > 0 ? 'degraded' : 'down',
    checks,
    taxRate: '0%'
  });
});

// ── Error Handler ────────────────────────────────────

app.use((err, req, res, _next) => {
  console.error(err.stack || err.message);
  res.status(500).json({ error: 'Gateway error', requestId: req.id });
});

// ── Start ────────────────────────────────────────────

const PORT = process.env.GATEWAY_PORT || 3000;
server.listen(PORT, () => {
  const uniqueServices = new Set(Object.values(SERVICES).map(s => s.name)).size;
  console.log(`Red Soberana API Gateway on :${PORT}`);
  console.log(`${Object.keys(SERVICES).length} routes → ${uniqueServices} microservices`);
});

module.exports = { app, server, SERVICES };
