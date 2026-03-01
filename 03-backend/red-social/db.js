'use strict';

const { Pool } = require('pg');

const pool = new Pool({
  connectionString: process.env.DATABASE_URL || 'postgresql://localhost:5432/red_social',
  max: 20,
  idleTimeoutMillis: 30000,
  connectionTimeoutMillis: 5000,
});

pool.on('error', (err) => {
  console.error('[db] Unexpected pool error:', err.message);
});

/**
 * Execute a parameterized query.
 * @param {string} text  SQL statement with $1, $2... placeholders
 * @param {any[]}  params  Bind values
 * @returns {Promise<import('pg').QueryResult>}
 */
function query(text, params) {
  return pool.query(text, params);
}

/**
 * Acquire a dedicated client from the pool.
 * Caller MUST call client.release() when done.
 */
function getClient() {
  return pool.connect();
}

/**
 * Run a callback inside a BEGIN / COMMIT transaction.
 * Automatically rolls back on error.
 * @param {(client: import('pg').PoolClient) => Promise<any>} fn
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

/**
 * Create all required tables and indexes.
 * Safe to call repeatedly (IF NOT EXISTS).
 */
async function initialize() {
  await pool.query(`
    -- Posts
    CREATE TABLE IF NOT EXISTS posts (
      id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
      author_id TEXT NOT NULL,
      content TEXT,
      media JSONB DEFAULT '[]',
      type TEXT DEFAULT 'post',
      language TEXT DEFAULT 'es',
      visibility TEXT DEFAULT 'public',
      monetized BOOLEAN DEFAULT FALSE,
      likes INTEGER DEFAULT 0,
      comments INTEGER DEFAULT 0,
      shares INTEGER DEFAULT 0,
      views INTEGER DEFAULT 0,
      tips DECIMAL(18,8) DEFAULT 0,
      hashtags TEXT[] DEFAULT '{}',
      mentions TEXT[] DEFAULT '{}',
      earnings DECIMAL(18,8) DEFAULT 0,
      ad_content BOOLEAN DEFAULT FALSE,
      repost_of UUID,
      created_at TIMESTAMPTZ DEFAULT NOW(),
      updated_at TIMESTAMPTZ DEFAULT NOW()
    );
    CREATE INDEX IF NOT EXISTS idx_posts_author ON posts(author_id);
    CREATE INDEX IF NOT EXISTS idx_posts_created ON posts(created_at DESC);

    -- Conversations
    CREATE TABLE IF NOT EXISTS conversations (
      id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
      type TEXT DEFAULT 'direct',
      name TEXT,
      participants TEXT[] NOT NULL,
      created_by TEXT NOT NULL,
      last_message JSONB,
      encryption TEXT DEFAULT 'aes-256-gcm',
      created_at TIMESTAMPTZ DEFAULT NOW()
    );
    CREATE INDEX IF NOT EXISTS idx_conv_participants ON conversations USING GIN(participants);

    -- Messages
    CREATE TABLE IF NOT EXISTS messages (
      id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
      conversation_id UUID NOT NULL REFERENCES conversations(id),
      sender_id TEXT NOT NULL,
      text TEXT,
      media JSONB,
      reply_to UUID,
      type TEXT DEFAULT 'text',
      encryption TEXT DEFAULT 'aes-256-gcm',
      delivered BOOLEAN DEFAULT FALSE,
      read BOOLEAN DEFAULT FALSE,
      created_at TIMESTAMPTZ DEFAULT NOW()
    );
    CREATE INDEX IF NOT EXISTS idx_msg_conversation ON messages(conversation_id);

    -- Trading orders
    CREATE TABLE IF NOT EXISTS trading_orders (
      id TEXT PRIMARY KEY,
      user_id TEXT NOT NULL,
      pair TEXT NOT NULL,
      side TEXT NOT NULL,
      type TEXT DEFAULT 'limit',
      price DECIMAL(18,8),
      amount DECIMAL(18,8) NOT NULL,
      filled DECIMAL(18,8) DEFAULT 0,
      remaining DECIMAL(18,8),
      status TEXT DEFAULT 'open',
      executed_price DECIMAL(18,8),
      fee DECIMAL(18,8),
      fiscal JSONB DEFAULT '{}',
      created_at TIMESTAMPTZ DEFAULT NOW()
    );
    CREATE INDEX IF NOT EXISTS idx_orders_pair ON trading_orders(pair);
    CREATE INDEX IF NOT EXISTS idx_orders_status ON trading_orders(status);

    -- Trades history
    CREATE TABLE IF NOT EXISTS trades (
      id SERIAL PRIMARY KEY,
      pair TEXT NOT NULL,
      price DECIMAL(18,8) NOT NULL,
      amount DECIMAL(18,8) NOT NULL,
      side TEXT NOT NULL,
      ts BIGINT NOT NULL,
      created_at TIMESTAMPTZ DEFAULT NOW()
    );
    CREATE INDEX IF NOT EXISTS idx_trades_pair ON trades(pair);
  `);
}

/**
 * Close all pool connections (for graceful shutdown).
 */
function end() {
  return pool.end();
}

/**
 * Return pool statistics for health checks.
 */
function stats() {
  return {
    totalCount: pool.totalCount,
    idleCount: pool.idleCount,
    waitingCount: pool.waitingCount,
  };
}

module.exports = { query, getClient, transaction, initialize, end, stats };
