'use strict';

// ============================================================
// Ierahkwa Sovereign Core — PostgreSQL Database Layer v1.0.0
// Connection pool, query helpers, migration runner, transactions
// ============================================================

const { Pool } = require('pg');
const fs       = require('fs');
const path     = require('path');
const config   = require('./config');

// ── Connection Pool ─────────────────────────────────────────
const pool = new Pool({
  connectionString: config.databaseUrl,
  min: config.dbPoolMin,
  max: config.dbPoolMax,
  idleTimeoutMillis: config.dbIdleTimeout,
  connectionTimeoutMillis: config.dbConnectionTimeout,
  statement_timeout: config.dbStatementTimeout,
  // Reject unauthorized SSL in production
  ssl: config.isProd ? { rejectUnauthorized: true } : false
});

// Log pool errors (do not crash — let the health check report the issue)
pool.on('error', (err) => {
  console.error(JSON.stringify({
    level: 50,
    service: 'sovereign-core',
    msg: 'Unexpected PostgreSQL pool error',
    err: { message: err.message, code: err.code, stack: err.stack },
    timestamp: new Date().toISOString()
  }));
});

pool.on('connect', (client) => {
  // Set search_path and timezone on each new connection
  client.query("SET timezone = 'UTC'").catch(() => {});
});

// ============================================================
// Public API
// ============================================================

/**
 * Execute a parameterized query
 * @param {string} text - SQL query with $1, $2 placeholders
 * @param {Array} [params] - Parameter values
 * @returns {Promise<pg.QueryResult>}
 */
async function query(text, params) {
  const start = Date.now();
  try {
    const result = await pool.query(text, params);
    const duration = Date.now() - start;

    // Log slow queries (> 500ms) at warn level
    if (duration > 500) {
      console.warn(JSON.stringify({
        level: 40,
        service: 'sovereign-core',
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
      service: 'sovereign-core',
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
 * IMPORTANT: Always release the client in a finally block.
 *
 * Usage:
 *   const client = await db.getClient();
 *   try {
 *     await client.query('BEGIN');
 *     await client.query('INSERT INTO ...');
 *     await client.query('COMMIT');
 *   } catch (err) {
 *     await client.query('ROLLBACK');
 *     throw err;
 *   } finally {
 *     client.release();
 *   }
 *
 * @returns {Promise<pg.PoolClient>}
 */
async function getClient() {
  const client = await pool.connect();
  const originalRelease = client.release.bind(client);
  let released = false;

  // Patch release to prevent double-release
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
 *
 * @param {Function} fn - async (client) => result
 * @returns {Promise<*>} Result of fn
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
 * Initialize database — run all pending migrations
 * Creates the migrations tracking table if it does not exist.
 */
async function initialize() {
  console.log(JSON.stringify({
    level: 30,
    service: 'sovereign-core',
    msg: 'Initializing database — checking migrations',
    timestamp: new Date().toISOString()
  }));

  // Verify connectivity
  const alive = await query('SELECT NOW() AS now');
  console.log(JSON.stringify({
    level: 30,
    service: 'sovereign-core',
    msg: 'Database connection verified',
    serverTime: alive.rows[0].now,
    timestamp: new Date().toISOString()
  }));

  // Create migrations tracking table
  await query(`
    CREATE TABLE IF NOT EXISTS _migrations (
      id          SERIAL PRIMARY KEY,
      filename    VARCHAR(255) NOT NULL UNIQUE,
      checksum    VARCHAR(64),
      executed_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
    )
  `);

  // Read migration files from ../migrations/ (relative to project root)
  const migrationsDir = path.resolve(__dirname, '..', 'migrations');

  if (!fs.existsSync(migrationsDir)) {
    console.log(JSON.stringify({
      level: 30,
      service: 'sovereign-core',
      msg: 'No migrations directory found — skipping',
      dir: migrationsDir,
      timestamp: new Date().toISOString()
    }));
    return;
  }

  const files = fs.readdirSync(migrationsDir)
    .filter(f => f.endsWith('.sql'))
    .sort(); // lexicographic order (e.g., 001_init.sql, 002_users.sql)

  if (files.length === 0) {
    console.log(JSON.stringify({
      level: 30,
      service: 'sovereign-core',
      msg: 'No migration files found',
      timestamp: new Date().toISOString()
    }));
    return;
  }

  // Get already-executed migrations
  const executed = await query('SELECT filename FROM _migrations ORDER BY id');
  const executedSet = new Set(executed.rows.map(r => r.filename));

  let applied = 0;

  for (const file of files) {
    if (executedSet.has(file)) continue;

    const filePath = path.join(migrationsDir, file);
    const sql = fs.readFileSync(filePath, 'utf8').trim();

    if (!sql) {
      console.warn(JSON.stringify({
        level: 40,
        service: 'sovereign-core',
        msg: `Skipping empty migration: ${file}`,
        timestamp: new Date().toISOString()
      }));
      continue;
    }

    // Compute checksum for integrity verification
    const crypto = require('crypto');
    const checksum = crypto.createHash('sha256').update(sql).digest('hex');

    console.log(JSON.stringify({
      level: 30,
      service: 'sovereign-core',
      msg: `Running migration: ${file}`,
      checksum: checksum.slice(0, 12),
      timestamp: new Date().toISOString()
    }));

    // Execute migration inside a transaction
    const client = await getClient();
    try {
      await client.query('BEGIN');
      await client.query(sql);
      await client.query(
        'INSERT INTO _migrations (filename, checksum) VALUES ($1, $2)',
        [file, checksum]
      );
      await client.query('COMMIT');
      applied++;

      console.log(JSON.stringify({
        level: 30,
        service: 'sovereign-core',
        msg: `Migration applied: ${file}`,
        timestamp: new Date().toISOString()
      }));
    } catch (err) {
      await client.query('ROLLBACK');
      console.error(JSON.stringify({
        level: 50,
        service: 'sovereign-core',
        msg: `Migration failed: ${file}`,
        err: { message: err.message, detail: err.detail, position: err.position },
        timestamp: new Date().toISOString()
      }));
      throw new Error(`Migration ${file} failed: ${err.message}`);
    } finally {
      client.release();
    }
  }

  console.log(JSON.stringify({
    level: 30,
    service: 'sovereign-core',
    msg: `Database initialization complete — ${applied} migration(s) applied, ${executedSet.size} already executed`,
    total: files.length,
    applied,
    skipped: executedSet.size,
    timestamp: new Date().toISOString()
  }));
}

/**
 * Gracefully close the pool (for shutdown)
 */
async function end() {
  await pool.end();
}

/**
 * Get pool statistics (for /metrics endpoint)
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
  pool // Exposed for advanced use cases (e.g., LISTEN/NOTIFY)
};
