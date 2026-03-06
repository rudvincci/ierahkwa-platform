'use strict';

/**
 * WiFi Soberano — Test Suite
 * Internet Satelital Soberano — Ierahkwa Ne Kanienke
 *
 * Run: npm test
 */

const { describe, it, before, after } = require('node:test');
const assert = require('node:assert/strict');
const http = require('node:http');

const BASE_URL = process.env.TEST_URL || 'http://localhost:3095';

// Helper: HTTP request
function request(path, options = {}) {
  return new Promise((resolve, reject) => {
    const url = new URL(path, BASE_URL);
    const opts = {
      hostname: url.hostname,
      port: url.port,
      path: url.pathname + url.search,
      method: options.method || 'GET',
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
    };

    const req = http.request(opts, (res) => {
      let body = '';
      res.on('data', (chunk) => (body += chunk));
      res.on('end', () => {
        try {
          resolve({ status: res.statusCode, headers: res.headers, body: JSON.parse(body) });
        } catch {
          resolve({ status: res.statusCode, headers: res.headers, body });
        }
      });
    });

    req.on('error', reject);
    if (options.body) req.write(JSON.stringify(options.body));
    req.end();
  });
}

// ============================================================
// Health Check
// ============================================================
describe('Health Check', () => {
  it('GET /health should return 200', async () => {
    const res = await request('/health');
    assert.equal(res.status, 200);
    assert.equal(res.body.status, 'ok');
    assert.equal(res.body.service, 'wifi-soberano');
  });
});

// ============================================================
// Portal API
// ============================================================
describe('Portal API', () => {
  it('GET /api/v1/wifi/portal should return portal data', async () => {
    const res = await request('/api/v1/wifi/portal');
    assert.equal(res.status, 200);
    assert.ok(res.body.network, 'Should have network info');
    assert.ok(Array.isArray(res.body.plans), 'Should have plans array');
  });

  it('GET /api/v1/wifi/plans should return plans list', async () => {
    const res = await request('/api/v1/wifi/plans');
    assert.equal(res.status, 200);
    assert.ok(Array.isArray(res.body.plans), 'Should return plans array');
  });
});

// ============================================================
// Session API
// ============================================================
describe('Session API', () => {
  it('GET /api/v1/wifi/session/status should return session info', async () => {
    const res = await request('/api/v1/wifi/session/status');
    // Should return 200 even with no session (returns inactive status)
    assert.ok([200, 404].includes(res.status));
  });

  it('POST /api/v1/wifi/connect without plan_id should return 400', async () => {
    const res = await request('/api/v1/wifi/connect', {
      method: 'POST',
      body: {},
    });
    assert.equal(res.status, 400);
  });
});

// ============================================================
// Payment API
// ============================================================
describe('Payment API', () => {
  it('POST /api/v1/wifi/payment/create without plan_id should return 400', async () => {
    const res = await request('/api/v1/wifi/payment/create', {
      method: 'POST',
      body: {},
    });
    assert.equal(res.status, 400);
  });

  it('POST /api/v1/wifi/payment/verify without payment_id should return 400', async () => {
    const res = await request('/api/v1/wifi/payment/verify', {
      method: 'POST',
      body: {},
    });
    assert.equal(res.status, 400);
  });
});

// ============================================================
// Admin API
// ============================================================
describe('Admin API', () => {
  it('GET /api/v1/wifi/admin/dashboard should return metrics', async () => {
    const res = await request('/api/v1/wifi/admin/dashboard');
    assert.equal(res.status, 200);
    assert.ok('active_users' in res.body, 'Should have active_users');
    assert.ok('revenue_today_wampum' in res.body, 'Should have revenue');
    assert.ok('fleet_online' in res.body, 'Should have fleet status');
  });

  it('GET /api/v1/wifi/admin/sessions should return sessions list', async () => {
    const res = await request('/api/v1/wifi/admin/sessions');
    assert.equal(res.status, 200);
    assert.ok(Array.isArray(res.body.sessions), 'Should return sessions array');
  });

  it('GET /api/v1/wifi/admin/revenue should return revenue data', async () => {
    const res = await request('/api/v1/wifi/admin/revenue');
    assert.equal(res.status, 200);
    assert.ok(Array.isArray(res.body.daily), 'Should have daily revenue');
    assert.ok(Array.isArray(res.body.weekly), 'Should have weekly revenue');
    assert.ok(Array.isArray(res.body.by_plan), 'Should have by_plan revenue');
  });

  it('POST /api/v1/wifi/admin/hotspot without name should return 400', async () => {
    const res = await request('/api/v1/wifi/admin/hotspot', {
      method: 'POST',
      body: {},
    });
    assert.equal(res.status, 400);
  });

  it('POST /api/v1/wifi/admin/plan without required fields should return 400', async () => {
    const res = await request('/api/v1/wifi/admin/plan', {
      method: 'POST',
      body: { name: 'Test' },
    });
    assert.equal(res.status, 400);
  });

  it('POST /api/v1/wifi/admin/vip without name should return 400', async () => {
    const res = await request('/api/v1/wifi/admin/vip', {
      method: 'POST',
      body: {},
    });
    assert.equal(res.status, 400);
  });
});

// ============================================================
// Fleet API
// ============================================================
describe('Fleet API', () => {
  it('GET /api/v1/wifi/admin/fleet should return fleet data', async () => {
    const res = await request('/api/v1/wifi/admin/fleet');
    assert.equal(res.status, 200);
    assert.ok(Array.isArray(res.body.fleet), 'Should return fleet array');
    assert.ok(res.body.summary, 'Should have summary');
  });

  it('POST /api/v1/wifi/admin/fleet without utid should return 400', async () => {
    const res = await request('/api/v1/wifi/admin/fleet', {
      method: 'POST',
      body: { model: 'Test' },
    });
    assert.equal(res.status, 400);
  });
});

// ============================================================
// Analytics API
// ============================================================
describe('Analytics API', () => {
  it('GET /api/v1/wifi/admin/analytics should return analytics', async () => {
    const res = await request('/api/v1/wifi/admin/analytics');
    assert.equal(res.status, 200);
    assert.ok('total_sessions' in res.body, 'Should have total_sessions');
  });

  it('GET /api/v1/wifi/admin/analytics?period=week should filter by week', async () => {
    const res = await request('/api/v1/wifi/admin/analytics?period=week');
    assert.equal(res.status, 200);
    assert.equal(res.body.period, 'week');
  });

  it('GET /api/v1/wifi/admin/vigilancia should return alerts', async () => {
    const res = await request('/api/v1/wifi/admin/vigilancia');
    assert.equal(res.status, 200);
    assert.ok(Array.isArray(res.body.alerts), 'Should return alerts array');
  });
});

// ============================================================
// Security Headers
// ============================================================
describe('Security', () => {
  it('Should have security headers', async () => {
    const res = await request('/health');
    assert.ok(res.headers['x-content-type-options'], 'Should have X-Content-Type-Options');
    assert.ok(res.headers['x-frame-options'] || res.headers['content-security-policy'], 'Should have frame protection');
  });

  it('Should handle CORS', async () => {
    const res = await request('/health', {
      headers: { Origin: 'http://localhost:3000' },
    });
    assert.equal(res.status, 200);
  });
});

console.log('\n🛰️ WiFi Soberano — Test Suite');
console.log('================================');
console.log(`Target: ${BASE_URL}\n`);
