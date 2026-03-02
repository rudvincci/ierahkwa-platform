'use strict';

// ============================================================================
// IERAHKWA ML ENGINE — Sovereign Anomaly Detection & Trust Scoring API
//
// Endpoints:
//   GET  /health                     - Health check
//   POST /api/trust/calculate        - Calculate trust score for a user
//   POST /api/trust/event            - Report positive/negative trust event
//   GET  /api/trust/profile/:userId  - Get user trust profile
//   GET  /api/trust/leaderboard      - Top trusted users
//   GET  /api/trust/stats            - Trust engine stats
//   POST /api/anomaly/detect         - Run ensemble anomaly detection
//   POST /api/anomaly/event          - Analyze a security event
//   GET  /api/anomaly/log            - Recent anomaly detections
//   GET  /api/anomaly/stats          - Anomaly detector stats
// ============================================================================

const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const compression = require('compression');

const { TrustEngine } = require('./lib/trust-engine');
const { AnomalyDetector } = require('./lib/anomaly-detector');

const PORT = parseInt(process.env.PORT || '3097', 10);
const HOST = process.env.HOST || '0.0.0.0';

const app = express();

app.use(helmet());
app.use(cors({ origin: process.env.CORS_ORIGIN || '*' }));
app.use(compression());
app.use(express.json({ limit: '1mb' }));

const trust = new TrustEngine({
  decayRate: parseFloat(process.env.TRUST_DECAY_RATE || '0.02'),
  boostRate: parseFloat(process.env.TRUST_BOOST_RATE || '0.5'),
});

const anomaly = new AnomalyDetector({
  windowSize: parseInt(process.env.ANOMALY_WINDOW || '100', 10),
  zThreshold: parseFloat(process.env.ANOMALY_Z_THRESHOLD || '2.5'),
  emaAlpha: parseFloat(process.env.ANOMALY_EMA_ALPHA || '0.1'),
  iqrMultiplier: parseFloat(process.env.ANOMALY_IQR_MULT || '1.5'),
});

const startTime = Date.now();

// ── Health ────────────────────────────────────────────────────────────

app.get('/health', (_req, res) => {
  res.json({
    status: 'healthy',
    service: 'ierahkwa-ml',
    version: '1.0.0',
    uptime_s: Math.round((Date.now() - startTime) / 1000),
    modules: { trust: 'active', anomaly: 'active' },
  });
});

// ── Trust Engine Routes ──────────────────────────────────────────────

app.post('/api/trust/calculate', (req, res) => {
  const { userId, signals } = req.body;
  if (!userId) return res.status(400).json({ error: 'userId required' });
  const result = trust.calculateTrust(userId, signals || {});
  res.json(result);
});

app.post('/api/trust/event', (req, res) => {
  const { userId, type, reason, magnitude } = req.body;
  if (!userId || !type) return res.status(400).json({ error: 'userId and type required' });
  if (!['positive', 'negative'].includes(type)) {
    return res.status(400).json({ error: 'type must be positive or negative' });
  }
  const result = trust.reportEvent(userId, type, reason || '', magnitude || 1);
  res.json(result);
});

app.get('/api/trust/profile/:userId', (req, res) => {
  const profile = trust.getProfile(req.params.userId);
  if (!profile) return res.status(404).json({ error: 'Profile not found' });
  res.json(profile);
});

app.get('/api/trust/leaderboard', (req, res) => {
  const limit = parseInt(req.query.limit || '20', 10);
  res.json(trust.getLeaderboard(limit));
});

app.get('/api/trust/stats', (_req, res) => {
  res.json(trust.getStats());
});

// ── Anomaly Detector Routes ──────────────────────────────────────────

app.post('/api/anomaly/detect', (req, res) => {
  const { metricKey, value } = req.body;
  if (!metricKey || value === undefined) {
    return res.status(400).json({ error: 'metricKey and value required' });
  }
  const result = anomaly.detect(metricKey, parseFloat(value));
  res.json(result);
});

app.post('/api/anomaly/event', (req, res) => {
  const event = req.body;
  if (!event || typeof event !== 'object') {
    return res.status(400).json({ error: 'Event object required' });
  }
  const result = anomaly.analyzeEvent(event);
  res.json(result);
});

app.get('/api/anomaly/log', (req, res) => {
  const limit = parseInt(req.query.limit || '50', 10);
  res.json(anomaly.getDetectionLog(limit));
});

app.get('/api/anomaly/stats', (_req, res) => {
  res.json(anomaly.getStats());
});

// ── Error handler ────────────────────────────────────────────────────

app.use((err, _req, res, _next) => {
  console.error(`[ierahkwa-ml] Error: ${err.message}`);
  res.status(500).json({ error: 'Internal server error' });
});

// ── Start ────────────────────────────────────────────────────────────

app.listen(PORT, HOST, () => {
  console.log(`[ierahkwa-ml] Sovereign ML Engine running on ${HOST}:${PORT}`);
  console.log(`[ierahkwa-ml] Modules: TrustEngine + AnomalyDetector`);
});

module.exports = app;
