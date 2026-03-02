'use strict';

// ============================================================================
// IERAHKWA ML ENGINE — Sovereign AI/ML Service
// Anomaly Detection · Trust Scoring · Pattern Analysis · Swarm Monitoring
//
// Endpoints:
//   GET  /health                      - Health check
//   POST /api/agent/sync              - Unified frontend agent sync
//   POST /api/trust/calculate         - Calculate trust score
//   POST /api/trust/event             - Report trust event
//   GET  /api/trust/profile/:userId   - User trust profile
//   GET  /api/trust/leaderboard       - Top trusted users
//   GET  /api/trust/stats             - Trust engine stats
//   POST /api/anomaly/detect          - Ensemble anomaly detection
//   POST /api/anomaly/event           - Analyze security event
//   GET  /api/anomaly/log             - Recent detections
//   GET  /api/anomaly/stats           - Anomaly detector stats
//   POST /api/pattern/analyze         - Behavioral pattern check
//   GET  /api/pattern/fingerprint/:id - User fingerprint
//   POST /api/swarm/heartbeat         - Node heartbeat
//   GET  /api/swarm/status            - Network health
//   GET  /api/swarm/consensus         - Consensus check
//   GET  /api/dashboard               - Full ML dashboard
// ============================================================================

const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const compression = require('compression');
const { v4: uuidv4 } = require('uuid');

const { TrustEngine } = require('./lib/trust-engine');
const { AnomalyDetector } = require('./lib/anomaly-detector');
const { PatternAnalyzer } = require('./lib/pattern-analyzer');
const { SwarmMonitor } = require('./lib/swarm-monitor');

const PORT = parseInt(process.env.PORT || '3092', 10);
const HOST = process.env.HOST || '0.0.0.0';

const app = express();

app.use(helmet());
app.use(cors({ origin: process.env.CORS_ORIGIN || '*' }));
app.use(compression());
app.use(express.json({ limit: '2mb' }));

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

const pattern = new PatternAnalyzer({
  maxUsers: 5000,
  historyDepth: 200,
  similarityThreshold: 0.6,
});

const swarm = new SwarmMonitor({
  heartbeatTimeout: 120000,
  fragmentationThreshold: 0.51,
  maxNodes: 10000,
});

const startTime = Date.now();

// ── Health ────────────────────────────────────────────────────────────

app.get('/health', (_req, res) => {
  res.json({
    status: 'healthy',
    service: 'ierahkwa-ml',
    version: '1.0.0',
    uptime_s: Math.round((Date.now() - startTime) / 1000),
    modules: { trust: 'active', anomaly: 'active', pattern: 'active', swarm: 'active' },
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

// ── Pattern Analysis Routes ───────────────────────────────────────────

app.post('/api/pattern/analyze', (req, res) => {
  const { userId, action } = req.body;
  if (!userId || !action) return res.status(400).json({ error: 'userId and action required' });
  const result = pattern.analyze(userId, action);
  res.json(result);
});

app.get('/api/pattern/fingerprint/:userId', (req, res) => {
  const fp = pattern.getFingerprint(req.params.userId);
  if (!fp) return res.status(404).json({ error: 'Fingerprint not found (needs >= 20 actions)' });
  res.json(fp);
});

app.get('/api/pattern/stats', (_req, res) => {
  res.json(pattern.getStats());
});

// ── Swarm Monitor Routes ─────────────────────────────────────────────

app.post('/api/swarm/heartbeat', (req, res) => {
  const { id, type, lat, lng, region, metrics } = req.body;
  if (!id) return res.status(400).json({ error: 'id required' });
  const result = swarm.heartbeat({ id, type, lat, lng, region, metrics });
  res.json(result);
});

app.get('/api/swarm/status', (_req, res) => {
  res.json(swarm.sweep());
});

app.get('/api/swarm/consensus', (_req, res) => {
  res.json(swarm.checkConsensus());
});

app.get('/api/swarm/nodes', (_req, res) => {
  res.json(swarm.getNodesByType());
});

app.get('/api/swarm/alerts', (req, res) => {
  const limit = parseInt(req.query.limit || '50', 10);
  res.json(swarm.getAlerts(limit));
});

// ── Unified Agent Sync (frontend ierahkwa-agents.js → backend) ──────

app.post('/api/agent/sync', (req, res) => {
  const { userId, agentId, events, trustSignals, actions, nodeHeartbeat } = req.body;
  if (!userId) return res.status(400).json({ error: 'userId required' });

  const results = {};

  // Anomaly detection on events
  if (Array.isArray(events) && events.length > 0) {
    results.anomalies = events.map(evt => {
      evt.id = evt.id || uuidv4();
      evt.source = evt.source || agentId || 'frontend-agent';
      evt.timestamp = evt.timestamp || new Date().toISOString();
      return anomaly.analyzeEvent(evt);
    });
    results.anomalyCount = results.anomalies.filter(r => r.isAnomalous).length;
  }

  // Trust scoring
  if (trustSignals) {
    results.trust = trust.calculateTrust(userId, trustSignals);
  }

  // Pattern analysis
  if (Array.isArray(actions) && actions.length > 0) {
    results.patterns = actions.map(act => pattern.analyze(userId, act));
    results.deviationCount = results.patterns.filter(r => r.isDeviation).length;
  }

  // Node heartbeat
  if (nodeHeartbeat && nodeHeartbeat.id) {
    results.node = swarm.heartbeat(nodeHeartbeat);
  }

  // Cross-engine: anomalies/deviations penalize trust
  if ((results.anomalyCount || 0) > 0) {
    trust.reportEvent(userId, 'negative', 'anomalous_behavior', results.anomalyCount);
  }
  if ((results.deviationCount || 0) > 0) {
    trust.reportEvent(userId, 'negative', 'pattern_deviation', results.deviationCount);
  }

  res.json({ success: true, userId, syncedAt: new Date().toISOString(), results });
});

// ── Dashboard ────────────────────────────────────────────────────────

app.get('/api/dashboard', (_req, res) => {
  res.json({
    success: true,
    dashboard: {
      anomalyDetector: { ...anomaly.getStats(), recentDetections: anomaly.getDetectionLog(10) },
      trustEngine: trust.getStats(),
      patternAnalyzer: pattern.getStats(),
      swarmMonitor: { ...swarm.getStats(), networkHealth: swarm.sweep(), recentAlerts: swarm.getAlerts(10) },
      timestamp: new Date().toISOString()
    }
  });
});

// ── Error handler ────────────────────────────────────────────────────

app.use((err, _req, res, _next) => {
  console.error(`[ierahkwa-ml] Error: ${err.message}`);
  res.status(500).json({ error: 'Internal server error' });
});

// ── Start ────────────────────────────────────────────────────────────

app.listen(PORT, HOST, () => {
  console.log(`[ierahkwa-ml] Sovereign ML Engine running on ${HOST}:${PORT}`);
  console.log(`[ierahkwa-ml] Modules: TrustEngine + AnomalyDetector + PatternAnalyzer + SwarmMonitor`);
  console.log(`[ierahkwa-ml] Agent sync: POST /api/agent/sync`);
  console.log(`[ierahkwa-ml] Dashboard:  GET  /api/dashboard`);
});

// Periodic swarm sweep every 30 seconds
setInterval(() => swarm.sweep(), 30000);

module.exports = app;
