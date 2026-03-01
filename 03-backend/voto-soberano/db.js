'use strict';

// ============================================================
// Ierahkwa Platform — Voto Soberano Database Layer v1.0.0
// PostgreSQL connection pool, query helpers, transactions
// Simplified from sovereign-core/src/db.js pattern
// ============================================================

const { Pool } = require('pg');

// ── Configuration ───────────────────────────────────────────
const DATABASE_URL = process.env.DATABASE_URL || 'postgresql://localhost:5432/voto_soberano';
const IS_PROD = process.env.NODE_ENV === 'production';

// ── Connection Pool ─────────────────────────────────────────
const pool = new Pool({
  connectionString: DATABASE_URL,
  min: parseInt(process.env.DB_POOL_MIN, 10) || 2,
  max: parseInt(process.env.DB_POOL_MAX, 10) || 10,
  idleTimeoutMillis: 30000,
  connectionTimeoutMillis: 5000,
  statement_timeout: 30000,
  ssl: IS_PROD ? { rejectUnauthorized: true } : false
});

pool.on('error', (err) => {
  console.error(JSON.stringify({
    level: 50,
    service: 'voto-soberano',
    msg: 'Unexpected PostgreSQL pool error',
    err: { message: err.message, code: err.code },
    timestamp: new Date().toISOString()
  }));
});

pool.on('connect', (client) => {
  client.query("SET timezone = 'UTC'").catch(() => {});
});

// ============================================================
// Public API
// ============================================================

/**
 * Execute a parameterized query
 * @param {string} text - SQL with $1, $2 placeholders
 * @param {Array} [params] - Parameter values
 * @returns {Promise<pg.QueryResult>}
 */
async function query(text, params) {
  const start = Date.now();
  try {
    const result = await pool.query(text, params);
    const duration = Date.now() - start;

    if (duration > 500) {
      console.warn(JSON.stringify({
        level: 40,
        service: 'voto-soberano',
        msg: 'Slow query detected',
        query: text.slice(0, 200),
        duration,
        rows: result.rowCount,
        timestamp: new Date().toISOString()
      }));
    }

    return result;
  } catch (err) {
    console.error(JSON.stringify({
      level: 50,
      service: 'voto-soberano',
      msg: 'Query execution error',
      query: text.slice(0, 200),
      err: { message: err.message, code: err.code, detail: err.detail, constraint: err.constraint },
      duration: Date.now() - start,
      timestamp: new Date().toISOString()
    }));
    throw err;
  }
}

/**
 * Get a dedicated client from the pool (for transactions)
 * Always release the client in a finally block.
 * @returns {Promise<pg.PoolClient>}
 */
async function getClient() {
  const client = await pool.connect();
  const originalRelease = client.release.bind(client);
  let released = false;

  client.release = () => {
    if (released) return;
    released = true;
    return originalRelease();
  };

  return client;
}

/**
 * Execute a callback inside a transaction
 * Automatically handles BEGIN / COMMIT / ROLLBACK
 * @param {Function} fn - async (client) => result
 * @returns {Promise<*>}
 */
async function transaction(fn) {
  const client = await getClient();
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

/**
 * Initialize database — create elections and ballots tables
 */
async function initialize() {
  console.log(JSON.stringify({
    level: 30,
    service: 'voto-soberano',
    msg: 'Initializing database — creating tables if needed',
    timestamp: new Date().toISOString()
  }));

  // Verify connectivity
  const alive = await query('SELECT NOW() AS now');
  console.log(JSON.stringify({
    level: 30,
    service: 'voto-soberano',
    msg: 'Database connection verified',
    serverTime: alive.rows[0].now,
    timestamp: new Date().toISOString()
  }));

  // Create elections table
  await query(`
    CREATE TABLE IF NOT EXISTS elections (
      id UUID PRIMARY KEY,
      title TEXT NOT NULL,
      description TEXT DEFAULT '',
      choices JSONB NOT NULL,
      start_date TIMESTAMPTZ NOT NULL,
      end_date TIMESTAMPTZ NOT NULL,
      status TEXT DEFAULT 'open',
      created_by TEXT NOT NULL,
      created_at TIMESTAMPTZ DEFAULT NOW(),
      closed_at TIMESTAMPTZ,
      closed_by TEXT
    )
  `);

  // Create ballots table
  await query(`
    CREATE TABLE IF NOT EXISTS ballots (
      id UUID PRIMARY KEY,
      election_id UUID NOT NULL REFERENCES elections(id),
      voter_id_hash TEXT NOT NULL,
      choice TEXT NOT NULL,
      previous_hash TEXT NOT NULL,
      receipt_hash TEXT NOT NULL,
      created_at TIMESTAMPTZ DEFAULT NOW(),
      UNIQUE(election_id, voter_id_hash)
    )
  `);

  await query('CREATE INDEX IF NOT EXISTS idx_ballots_election ON ballots(election_id)');

  console.log(JSON.stringify({
    level: 30,
    service: 'voto-soberano',
    msg: 'Database initialization complete — tables ready',
    timestamp: new Date().toISOString()
  }));
}

/**
 * Gracefully close the pool
 */
async function end() {
  await pool.end();
}

/**
 * Get pool statistics (for health/metrics)
 */
function stats() {
  return {
    totalCount: pool.totalCount,
    idleCount: pool.idleCount,
    waitingCount: pool.waitingCount
  };
}

// ============================================================
// Exports
// ============================================================
module.exports = {
  query,
  getClient,
  transaction,
  initialize,
  end,
  stats,
  pool
};
