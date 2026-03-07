'use strict';

/**
 * WiFi Bridge Module — sovereign-core ↔ wifi-soberano
 *
 * Provides integration between the universal backend (port 3050)
 * and the WiFi captive portal service (port 3095).
 *
 * Features:
 *   - Session verification for WiFi users
 *   - Payment forwarding for WAMPUM transactions
 *   - User provisioning (create sovereign-core account from WiFi signup)
 *   - Analytics aggregation (WiFi data → sovereign analytics)
 *   - Fleet status proxy (Starlink kit health)
 *   - Vigilancia alerts forwarding
 */

const { Router } = require('express');
const http = require('http');

const router = Router();

// WiFi Soberano service URL (internal network)
const WIFI_SERVICE = process.env.WIFI_SERVICE_URL || 'http://wifi-soberano:3095';
const WIFI_TIMEOUT = parseInt(process.env.WIFI_TIMEOUT_MS) || 5000;

// ── Helper: Proxy request to wifi-soberano ─────────────────────
function proxyToWifi(path, method, body) {
  return new Promise((resolve, reject) => {
    const url = new URL(path, WIFI_SERVICE);
    const options = {
      hostname: url.hostname,
      port: url.port,
      path: url.pathname + url.search,
      method: method || 'GET',
      timeout: WIFI_TIMEOUT,
      headers: {
        'Content-Type': 'application/json',
        'X-Source': 'sovereign-core',
        'X-Bridge': 'wifi-bridge'
      }
    };

    const req = http.request(options, (res) => {
      let data = '';
      res.on('data', chunk => { data += chunk; });
      res.on('end', () => {
        try {
          resolve({ status: res.statusCode, data: JSON.parse(data) });
        } catch {
          resolve({ status: res.statusCode, data: data });
        }
      });
    });

    req.on('error', (err) => {
      reject(new Error(`WiFi bridge connection failed: ${err.message}`));
    });

    req.on('timeout', () => {
      req.destroy();
      reject(new Error('WiFi bridge request timeout'));
    });

    if (body) {
      req.write(JSON.stringify(body));
    }
    req.end();
  });
}

// ── GET /v1/wifi/status ────────────────────────────────────────
// Check WiFi service health and connectivity
router.get('/status', async (req, res) => {
  try {
    const result = await proxyToWifi('/health', 'GET');
    res.json({
      status: 'ok',
      bridge: 'sovereign-core ↔ wifi-soberano',
      wifi_service: result.data,
      connected: result.status === 200,
      timestamp: new Date().toISOString()
    });
  } catch (err) {
    res.status(503).json({
      status: 'error',
      bridge: 'sovereign-core ↔ wifi-soberano',
      connected: false,
      error: err.message,
      timestamp: new Date().toISOString()
    });
  }
});

// ── GET /v1/wifi/plans ─────────────────────────────────────────
// Get available WiFi plans (proxy from wifi-soberano)
router.get('/plans', async (req, res) => {
  try {
    const result = await proxyToWifi('/api/v1/wifi/plans', 'GET');
    res.status(result.status).json(result.data);
  } catch (err) {
    res.status(502).json({
      status: 'error',
      detail: 'Cannot reach WiFi service',
      error: err.message
    });
  }
});

// ── GET /v1/wifi/dashboard ─────────────────────────────────────
// Aggregated WiFi dashboard data for sovereign-core admin
router.get('/dashboard', async (req, res) => {
  try {
    const [dashboard, fleet] = await Promise.allSettled([
      proxyToWifi('/api/v1/wifi/admin/dashboard', 'GET'),
      proxyToWifi('/api/v1/wifi/admin/fleet', 'GET')
    ]);

    res.json({
      status: 'ok',
      wifi_dashboard: dashboard.status === 'fulfilled' ? dashboard.value.data : null,
      fleet: fleet.status === 'fulfilled' ? fleet.value.data : null,
      source: 'sovereign-core-wifi-bridge',
      timestamp: new Date().toISOString()
    });
  } catch (err) {
    res.status(502).json({
      status: 'error',
      detail: 'WiFi dashboard unavailable',
      error: err.message
    });
  }
});

// ── POST /v1/wifi/provision-user ───────────────────────────────
// Create a sovereign-core user account from WiFi captive portal signup
router.post('/provision-user', async (req, res) => {
  const { email, display_name, mac_address, hotspot_id } = req.body;
  const { db, log } = req.app.locals;

  if (!email) {
    return res.status(400).json({
      status: 'error',
      detail: 'Email required for account provisioning'
    });
  }

  try {
    // Check if user already exists
    const existing = await db.query(
      'SELECT id, email FROM users WHERE email = $1',
      [email]
    );

    if (existing.rows.length > 0) {
      return res.json({
        status: 'ok',
        action: 'existing',
        user_id: existing.rows[0].id,
        message: 'User already has sovereign account'
      });
    }

    // Create new sovereign user from WiFi signup
    const tempPassword = require('crypto').randomBytes(16).toString('hex');
    const bcrypt = require('bcryptjs');
    const hash = await bcrypt.hash(tempPassword, 12);

    const result = await db.query(`
      INSERT INTO users (email, password_hash, display_name, role, metadata)
      VALUES ($1, $2, $3, 'user', $4)
      RETURNING id, email, display_name
    `, [
      email,
      hash,
      display_name || email.split('@')[0],
      JSON.stringify({
        source: 'wifi-captive-portal',
        mac_address: mac_address || null,
        hotspot_id: hotspot_id || null,
        provisioned_at: new Date().toISOString()
      })
    ]);

    log.info('WiFi user provisioned', {
      user_id: result.rows[0].id,
      email,
      hotspot_id
    });

    res.status(201).json({
      status: 'ok',
      action: 'created',
      user: result.rows[0],
      message: 'Sovereign account created from WiFi portal'
    });
  } catch (err) {
    log.error('WiFi user provisioning failed', { error: err.message });
    res.status(500).json({
      status: 'error',
      detail: 'User provisioning failed',
      error: err.message
    });
  }
});

// ── GET /v1/wifi/sessions ──────────────────────────────────────
// Get active WiFi sessions (admin)
router.get('/sessions', async (req, res) => {
  try {
    const result = await proxyToWifi('/api/v1/wifi/admin/sessions', 'GET');
    res.status(result.status).json(result.data);
  } catch (err) {
    res.status(502).json({
      status: 'error',
      detail: 'Cannot retrieve WiFi sessions'
    });
  }
});

// ── GET /v1/wifi/revenue ───────────────────────────────────────
// WiFi revenue aggregated for sovereign-core analytics
router.get('/revenue', async (req, res) => {
  try {
    const result = await proxyToWifi('/api/v1/wifi/admin/revenue', 'GET');
    res.status(result.status).json({
      ...result.data,
      source: 'wifi-soberano',
      bridge: 'sovereign-core'
    });
  } catch (err) {
    res.status(502).json({
      status: 'error',
      detail: 'Revenue data unavailable'
    });
  }
});

// ── GET /v1/wifi/fleet ─────────────────────────────────────────
// Starlink fleet status
router.get('/fleet', async (req, res) => {
  try {
    const result = await proxyToWifi('/api/v1/wifi/admin/fleet', 'GET');
    res.status(result.status).json(result.data);
  } catch (err) {
    res.status(502).json({
      status: 'error',
      detail: 'Fleet data unavailable'
    });
  }
});

// ── GET /v1/wifi/vigilancia ────────────────────────────────────
// Vigilancia alerts from WiFi network
router.get('/vigilancia', async (req, res) => {
  try {
    const result = await proxyToWifi('/api/v1/wifi/admin/vigilancia', 'GET');
    res.status(result.status).json(result.data);
  } catch (err) {
    res.status(502).json({
      status: 'error',
      detail: 'Vigilancia data unavailable'
    });
  }
});

// ── POST /v1/wifi/alert ────────────────────────────────────────
// Forward alert from WiFi network to sovereign-core
router.post('/alert', async (req, res) => {
  const { log } = req.app.locals;
  const { alert_type, severity, ip_address, details } = req.body;

  log.warn('WiFi alert received', {
    alert_type,
    severity,
    ip_address,
    source: 'wifi-bridge'
  });

  // Could forward to Atabey AI, notification system, etc.
  res.json({
    status: 'ok',
    received: true,
    alert_type,
    severity,
    timestamp: new Date().toISOString()
  });
});

module.exports = router;
