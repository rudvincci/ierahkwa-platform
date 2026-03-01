'use strict';

// ============================================================================
// VIGILANCIA SOBERANA — PostgreSQL Database Layer
// Persistent storage for SIEM events, alert rules, and compliance data
// Ierahkwa Ne Kanienke / MameyNode Platform
// ============================================================================

const { Pool } = require('pg');
const { createLogger } = require('../shared/logger');

const logger = createLogger('vigilancia-soberana-db');

const DATABASE_URL = process.env.DATABASE_URL || 'postgresql://localhost:5432/vigilancia_soberana';

const pool = new Pool({
  connectionString: DATABASE_URL,
  max: 20,
  idleTimeoutMillis: 30000,
  connectionTimeoutMillis: 5000
});

pool.on('error', (err) => {
  logger.error('Unexpected pool error', { err });
});

// ============================================================================
// CORE QUERY HELPERS
// ============================================================================

/**
 * Execute a parameterized query
 * @param {string} text - SQL query text
 * @param {Array} params - Query parameters
 * @returns {Promise<import('pg').QueryResult>}
 */
async function query(text, params) {
  const start = Date.now();
  const result = await pool.query(text, params);
  const duration = Date.now() - start;
  logger.debug('query executed', { text: text.substring(0, 80), duration, rows: result.rowCount });
  return result;
}

/**
 * Get a client from the pool for manual transaction control
 * @returns {Promise<import('pg').PoolClient>}
 */
async function getClient() {
  return pool.connect();
}

/**
 * Execute a function inside a transaction
 * @param {Function} fn - async function(client) => result
 * @returns {Promise<*>}
 */
async function transaction(fn) {
  const client = await pool.connect();
  try {
    await client.query('BEGIN');
    const result = await fn(client);
    await client.query('COMMIT');
    return result;
  } catch (err) {
    await client.query('ROLLBACK');
    throw err;
  } finally {
    client.release();
  }
}

// ============================================================================
// SCHEMA INITIALIZATION
// ============================================================================

/**
 * Create all required tables if they do not exist.
 * Matches the data structures from the original in-memory stores:
 *   - events (CircularEventBuffer)
 *   - alert_rules (Map of rules)
 *   - triggered_alerts (array of fired alerts)
 *   - compliance_frameworks + compliance_controls (static frameworks)
 *   - counters (totalIngested tracker)
 */
async function initialize() {
  logger.info('Initializing database schema');

  await query(`
    CREATE TABLE IF NOT EXISTS events (
      id UUID PRIMARY KEY,
      source TEXT NOT NULL,
      severity TEXT NOT NULL CHECK (severity IN ('critical','high','medium','low','info')),
      message TEXT NOT NULL,
      category TEXT NOT NULL DEFAULT 'general',
      metadata JSONB NOT NULL DEFAULT '{}',
      ip TEXT,
      user_agent TEXT,
      timestamp TIMESTAMPTZ NOT NULL DEFAULT NOW()
    )
  `);

  await query(`
    CREATE INDEX IF NOT EXISTS idx_events_severity ON events (severity)
  `);
  await query(`
    CREATE INDEX IF NOT EXISTS idx_events_source ON events (source)
  `);
  await query(`
    CREATE INDEX IF NOT EXISTS idx_events_category ON events (category)
  `);
  await query(`
    CREATE INDEX IF NOT EXISTS idx_events_timestamp ON events (timestamp DESC)
  `);

  await query(`
    CREATE TABLE IF NOT EXISTS alert_rules (
      id UUID PRIMARY KEY,
      name TEXT NOT NULL,
      condition TEXT NOT NULL CHECK (condition IN ('severity_equals','source_contains','message_contains','category_equals')),
      value TEXT NOT NULL,
      threshold INT NOT NULL DEFAULT 1,
      action TEXT NOT NULL DEFAULT 'log',
      alert_severity TEXT NOT NULL DEFAULT 'high',
      enabled BOOLEAN NOT NULL DEFAULT TRUE,
      trigger_count INT NOT NULL DEFAULT 0,
      last_triggered TIMESTAMPTZ,
      created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
    )
  `);

  await query(`
    CREATE TABLE IF NOT EXISTS triggered_alerts (
      id UUID PRIMARY KEY,
      rule_id UUID NOT NULL REFERENCES alert_rules(id) ON DELETE CASCADE,
      rule_name TEXT NOT NULL,
      severity TEXT NOT NULL,
      message TEXT NOT NULL,
      event_id UUID,
      event_source TEXT,
      event_severity TEXT,
      action TEXT,
      triggered_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
    )
  `);

  await query(`
    CREATE INDEX IF NOT EXISTS idx_triggered_alerts_triggered_at ON triggered_alerts (triggered_at DESC)
  `);

  await query(`
    CREATE TABLE IF NOT EXISTS compliance_frameworks (
      key TEXT PRIMARY KEY,
      name TEXT NOT NULL
    )
  `);

  await query(`
    CREATE TABLE IF NOT EXISTS compliance_controls (
      id SERIAL PRIMARY KEY,
      framework_key TEXT NOT NULL REFERENCES compliance_frameworks(key) ON DELETE CASCADE,
      control_id TEXT NOT NULL,
      name TEXT NOT NULL,
      status TEXT NOT NULL DEFAULT 'pass',
      score INT NOT NULL DEFAULT 0,
      UNIQUE (framework_key, control_id)
    )
  `);

  await query(`
    CREATE TABLE IF NOT EXISTS counters (
      key TEXT PRIMARY KEY,
      value BIGINT NOT NULL DEFAULT 0
    )
  `);

  // Seed the totalIngested counter if missing
  await query(`
    INSERT INTO counters (key, value) VALUES ('totalIngested', 0)
    ON CONFLICT (key) DO NOTHING
  `);

  // Seed compliance frameworks
  await seedComplianceData();

  logger.info('Database schema initialized');
}

// ============================================================================
// COMPLIANCE DATA SEEDING
// ============================================================================

async function seedComplianceData() {
  const frameworks = {
    'OWASP-TOP-10': {
      name: 'OWASP Top 10:2025',
      controls: [
        { id: 'A01', name: 'Broken Access Control', status: 'pass', score: 92 },
        { id: 'A02', name: 'Cryptographic Failures', status: 'pass', score: 95 },
        { id: 'A03', name: 'Injection', status: 'pass', score: 98 },
        { id: 'A04', name: 'Insecure Design', status: 'pass', score: 88 },
        { id: 'A05', name: 'Security Misconfiguration', status: 'pass', score: 90 },
        { id: 'A06', name: 'Vulnerable Components', status: 'warning', score: 78 },
        { id: 'A07', name: 'Authentication Failures', status: 'pass', score: 94 },
        { id: 'A08', name: 'Software Integrity Failures', status: 'pass', score: 91 },
        { id: 'A09', name: 'Logging & Monitoring Failures', status: 'pass', score: 96 },
        { id: 'A10', name: 'Server-Side Request Forgery', status: 'pass', score: 93 }
      ]
    },
    'PCI-DSS': {
      name: 'PCI DSS v4.0',
      controls: [
        { id: 'R1', name: 'Network Security Controls', status: 'pass', score: 90 },
        { id: 'R2', name: 'Secure Configurations', status: 'pass', score: 88 },
        { id: 'R3', name: 'Protect Stored Account Data', status: 'pass', score: 95 },
        { id: 'R4', name: 'Encrypt Transmission', status: 'pass', score: 97 },
        { id: 'R5', name: 'Malware Protection', status: 'pass', score: 85 },
        { id: 'R6', name: 'Secure Systems & Software', status: 'warning', score: 80 },
        { id: 'R7', name: 'Restrict Access', status: 'pass', score: 92 },
        { id: 'R8', name: 'Identify Users & Auth', status: 'pass', score: 94 },
        { id: 'R9', name: 'Physical Access', status: 'pass', score: 88 },
        { id: 'R10', name: 'Log & Monitor', status: 'pass', score: 96 },
        { id: 'R11', name: 'Test Security', status: 'pass', score: 87 },
        { id: 'R12', name: 'Security Policies', status: 'pass', score: 91 }
      ]
    },
    'HIPAA': {
      name: 'HIPAA Security Rule',
      controls: [
        { id: 'AD1', name: 'Security Management Process', status: 'pass', score: 92 },
        { id: 'AD2', name: 'Assigned Security Responsibility', status: 'pass', score: 95 },
        { id: 'AD3', name: 'Workforce Security', status: 'pass', score: 88 },
        { id: 'AD4', name: 'Information Access Management', status: 'pass', score: 90 },
        { id: 'AD5', name: 'Security Awareness Training', status: 'warning', score: 76 },
        { id: 'AD6', name: 'Security Incident Procedures', status: 'pass', score: 94 },
        { id: 'PH1', name: 'Facility Access Controls', status: 'pass', score: 87 },
        { id: 'PH2', name: 'Workstation Use', status: 'pass', score: 89 },
        { id: 'PH3', name: 'Device & Media Controls', status: 'pass', score: 85 },
        { id: 'TE1', name: 'Access Control', status: 'pass', score: 93 },
        { id: 'TE2', name: 'Audit Controls', status: 'pass', score: 96 },
        { id: 'TE3', name: 'Integrity', status: 'pass', score: 91 },
        { id: 'TE4', name: 'Transmission Security', status: 'pass', score: 97 }
      ]
    }
  };

  for (const [key, fw] of Object.entries(frameworks)) {
    await query(
      `INSERT INTO compliance_frameworks (key, name) VALUES ($1, $2)
       ON CONFLICT (key) DO UPDATE SET name = EXCLUDED.name`,
      [key, fw.name]
    );

    for (const ctrl of fw.controls) {
      await query(
        `INSERT INTO compliance_controls (framework_key, control_id, name, status, score)
         VALUES ($1, $2, $3, $4, $5)
         ON CONFLICT (framework_key, control_id)
         DO UPDATE SET name = EXCLUDED.name, status = EXCLUDED.status, score = EXCLUDED.score`,
        [key, ctrl.id, ctrl.name, ctrl.status, ctrl.score]
      );
    }
  }
}

// ============================================================================
// SHUTDOWN
// ============================================================================

/**
 * Gracefully close the connection pool
 */
async function end() {
  logger.info('Closing database pool');
  await pool.end();
}

// ============================================================================
// POOL STATS
// ============================================================================

/**
 * Return current pool statistics
 */
function stats() {
  return {
    totalCount: pool.totalCount,
    idleCount: pool.idleCount,
    waitingCount: pool.waitingCount
  };
}

// ============================================================================
// EXPORTS
// ============================================================================

module.exports = {
  query,
  getClient,
  transaction,
  initialize,
  end,
  stats,
  pool
};
