'use strict';

// ============================================================
// Ierahkwa Sovereign Core — Standalone Migration Runner v1.0.0
// Usage: node src/migrate.js
//        node src/migrate.js --status    (show migration status)
//        node src/migrate.js --dry-run   (preview without executing)
// ============================================================

const fs     = require('fs');
const path   = require('path');
const crypto = require('crypto');
const { Pool } = require('pg');

// ── Parse CLI flags ─────────────────────────────────────────
const args = process.argv.slice(2);
const DRY_RUN    = args.includes('--dry-run');
const STATUS_ONLY = args.includes('--status');

// ── Database connection ─────────────────────────────────────
const DATABASE_URL = process.env.DATABASE_URL || 'postgresql://localhost:5432/sovereign_core';

const pool = new Pool({
  connectionString: DATABASE_URL,
  max: 3,
  connectionTimeoutMillis: 10000,
  statement_timeout: 60000
});

const MIGRATIONS_DIR = path.resolve(__dirname, '..', 'migrations');

// ── Helpers ─────────────────────────────────────────────────

function log(msg, data = {}) {
  const line = {
    level: 30,
    service: 'sovereign-core-migrate',
    msg,
    ...data,
    timestamp: new Date().toISOString()
  };
  console.log(JSON.stringify(line));
}

function logError(msg, data = {}) {
  const line = {
    level: 50,
    service: 'sovereign-core-migrate',
    msg,
    ...data,
    timestamp: new Date().toISOString()
  };
  console.error(JSON.stringify(line));
}

function checksum(content) {
  return crypto.createHash('sha256').update(content).digest('hex');
}

// ============================================================
// Main migration logic
// ============================================================

async function run() {
  log('Migration runner started', { databaseUrl: DATABASE_URL.replace(/\/\/.*@/, '//***@'), dryRun: DRY_RUN, statusOnly: STATUS_ONLY });

  // 1. Verify database connectivity
  let client;
  try {
    client = await pool.connect();
    const result = await client.query('SELECT NOW() AS now, current_database() AS db');
    log('Database connected', { server_time: result.rows[0].now, database: result.rows[0].db });
  } catch (err) {
    logError('Cannot connect to database', { err: err.message });
    process.exit(1);
  } finally {
    if (client) client.release();
  }

  // 2. Ensure _migrations table exists
  await pool.query(`
    CREATE TABLE IF NOT EXISTS _migrations (
      id          SERIAL PRIMARY KEY,
      filename    VARCHAR(255) NOT NULL UNIQUE,
      checksum    VARCHAR(64),
      executed_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
    )
  `);

  // 3. Read executed migrations
  const executedResult = await pool.query('SELECT filename, checksum, executed_at FROM _migrations ORDER BY id');
  const executedMap = new Map(executedResult.rows.map(r => [r.filename, r]));

  // 4. Read migration files from disk
  if (!fs.existsSync(MIGRATIONS_DIR)) {
    log('No migrations directory found — nothing to do', { dir: MIGRATIONS_DIR });
    await pool.end();
    return;
  }

  const files = fs.readdirSync(MIGRATIONS_DIR)
    .filter(f => f.endsWith('.sql'))
    .sort();

  if (files.length === 0) {
    log('No migration files found', { dir: MIGRATIONS_DIR });
    await pool.end();
    return;
  }

  // 5. Status mode — show all migrations and their state
  if (STATUS_ONLY) {
    console.log('\n  Migration Status\n  ═════════════════════════════════════════════════════════');
    for (const file of files) {
      const executed = executedMap.get(file);
      const filePath = path.join(MIGRATIONS_DIR, file);
      const content = fs.readFileSync(filePath, 'utf8');
      const hash = checksum(content).slice(0, 12);

      if (executed) {
        const execDate = new Date(executed.executed_at).toISOString().split('T')[0];
        const match = executed.checksum && executed.checksum.startsWith(hash) ? 'OK' : 'CHANGED';
        console.log(`  [DONE]    ${file}  (${execDate})  checksum: ${match}`);
      } else {
        console.log(`  [PENDING] ${file}  checksum: ${hash}`);
      }
    }
    console.log(`\n  Total: ${files.length} | Executed: ${executedMap.size} | Pending: ${files.length - executedMap.size}\n`);
    await pool.end();
    return;
  }

  // 6. Run pending migrations
  const pending = files.filter(f => !executedMap.has(f));

  if (pending.length === 0) {
    log('All migrations already applied', { total: files.length });
    await pool.end();
    return;
  }

  log(`Found ${pending.length} pending migration(s)`, { pending: pending });

  let applied = 0;
  let failed = 0;

  for (const file of pending) {
    const filePath = path.join(MIGRATIONS_DIR, file);
    const sql = fs.readFileSync(filePath, 'utf8').trim();

    if (!sql) {
      log(`Skipping empty migration: ${file}`);
      continue;
    }

    const hash = checksum(sql);
    log(`${DRY_RUN ? '[DRY-RUN] Would apply' : 'Applying'}: ${file}`, { checksum: hash.slice(0, 12), size: sql.length });

    if (DRY_RUN) {
      applied++;
      continue;
    }

    // Execute in a transaction
    const txClient = await pool.connect();
    try {
      await txClient.query('BEGIN');
      await txClient.query(sql);
      await txClient.query(
        'INSERT INTO _migrations (filename, checksum) VALUES ($1, $2)',
        [file, hash]
      );
      await txClient.query('COMMIT');
      applied++;
      log(`Migration applied successfully: ${file}`);
    } catch (err) {
      await txClient.query('ROLLBACK');
      failed++;
      logError(`Migration FAILED: ${file}`, {
        err: err.message,
        detail: err.detail,
        hint: err.hint,
        position: err.position
      });
      // Stop on first failure — do not continue with later migrations
      logError('Stopping migration runner due to failure');
      break;
    } finally {
      txClient.release();
    }
  }

  // 7. Summary
  log('Migration runner completed', {
    total: files.length,
    applied,
    failed,
    skipped: executedMap.size,
    dryRun: DRY_RUN
  });

  if (failed > 0) {
    logError(`${failed} migration(s) failed — review errors above`);
  }

  await pool.end();
  process.exit(failed > 0 ? 1 : 0);
}

// ============================================================
// Execute
// ============================================================
run().catch(err => {
  logError('Migration runner crashed', { err: err.message, stack: err.stack });
  pool.end().then(() => process.exit(1)).catch(() => process.exit(1));
});
