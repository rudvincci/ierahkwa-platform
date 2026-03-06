'use strict';

/**
 * Session Expiry Worker
 * Runs every 60 seconds to expire WiFi sessions and clean up Redis.
 * Can be run as standalone process or imported as module.
 *
 * Usage:
 *   node workers/session-expiry.js
 *   or: require('./workers/session-expiry')(pool, redis, logger)
 */

const { Pool } = require('pg');

async function expireSessions(pool, logger) {
  try {
    // 1. Expire sessions in PostgreSQL
    const result = await pool.query(`
      UPDATE wifi_sessions
      SET status = 'expired'
      WHERE status = 'active' AND expires_at < NOW()
      RETURNING id, ip_address, plan_id, hotspot_id, data_used_mb
    `);

    if (result.rowCount > 0) {
      logger.info({ count: result.rowCount }, 'Sessions expired');

      // 2. Log analytics for expired sessions
      for (const session of result.rows) {
        await pool.query(`
          INSERT INTO vigilancia_alerts (alert_type, severity, ip_address, details)
          VALUES ('session_expired', 'info', $1, $2)
        `, [session.ip_address, JSON.stringify({
          session_id: session.id,
          data_used_mb: session.data_used_mb,
          action: 'auto_expired'
        })]);
      }
    }

    // 3. Clean old vigilancia connections (older than 90 days)
    const cleanup = await pool.query(`
      DELETE FROM vigilancia_connections
      WHERE timestamp < NOW() - INTERVAL '90 days'
    `);
    if (cleanup.rowCount > 0) {
      logger.info({ count: cleanup.rowCount }, 'Old connections cleaned');
    }

    return result.rowCount;
  } catch (err) {
    logger.error({ err }, 'Session expiry worker error');
    return 0;
  }
}

// Standalone mode
if (require.main === module) {
  const pino = require('pino');
  const logger = pino({ name: 'session-expiry-worker' });

  const pool = new Pool({
    connectionString: process.env.DATABASE_URL || 'postgresql://soberano:soberano@localhost:5432/soberana',
    max: 3,
  });

  const INTERVAL = parseInt(process.env.EXPIRY_INTERVAL_MS) || 60000; // 1 minute

  logger.info({ interval: INTERVAL }, 'Session expiry worker started');

  async function run() {
    const expired = await expireSessions(pool, logger);
    if (expired > 0) {
      logger.info({ expired }, 'Cycle complete');
    }
  }

  run();
  setInterval(run, INTERVAL);

  process.on('SIGTERM', async () => {
    logger.info('Worker shutting down');
    await pool.end();
    process.exit(0);
  });
}

module.exports = expireSessions;
