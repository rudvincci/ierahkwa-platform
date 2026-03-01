'use strict';

// ============================================================================
// CONFERENCIA SOBERANA — PostgreSQL Database Module
// Persistent storage for conference rooms and participants
// Ierahkwa Ne Kanienke / MameyNode Platform
// ============================================================================

const { Pool } = require('pg');
const { createLogger } = require('../shared/logger');

const logger = createLogger('conferencia-soberana-db');

// ============================================================================
// CONNECTION POOL
// ============================================================================

const pool = new Pool({
  connectionString: process.env.DATABASE_URL || 'postgresql://localhost:5432/conferencia_soberana',
  max: parseInt(process.env.DB_POOL_MAX || '10', 10),
  idleTimeoutMillis: 30000,
  connectionTimeoutMillis: 5000
});

pool.on('error', (err) => {
  logger.error('Unexpected pool error', { err });
});

// ============================================================================
// QUERY HELPERS
// ============================================================================

/**
 * Execute a parameterized query against the pool
 * @param {string} text - SQL query text with $1, $2 placeholders
 * @param {Array} params - Query parameter values
 * @returns {import('pg').QueryResult}
 */
async function query(text, params) {
  const start = Date.now();
  const result = await pool.query(text, params);
  const duration = Date.now() - start;
  logger.debug('query executed', { text: text.substring(0, 80), duration, rows: result.rowCount });
  return result;
}

/**
 * Get a dedicated client from the pool (for transactions)
 * Caller MUST release the client when done.
 * @returns {import('pg').PoolClient}
 */
async function getClient() {
  const client = await pool.connect();
  return client;
}

/**
 * Execute a function inside a database transaction
 * Automatically commits on success or rolls back on error.
 * @param {Function} fn - async function(client) => result
 * @returns {*} result of fn
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
 * Matches the in-memory data structures from the original service.
 */
async function initialize() {
  logger.info('Initializing database schema');

  await query(`
    CREATE TABLE IF NOT EXISTS rooms (
      id            VARCHAR(12) PRIMARY KEY,
      name          VARCHAR(255) NOT NULL,
      host          VARCHAR(255) NOT NULL DEFAULT 'anonymous',
      max_participants INTEGER NOT NULL DEFAULT 50,
      status        VARCHAR(20)  NOT NULL DEFAULT 'waiting',
      is_locked     BOOLEAN      NOT NULL DEFAULT FALSE,
      recording     BOOLEAN      NOT NULL DEFAULT FALSE,
      encryption    VARCHAR(64)  NOT NULL DEFAULT 'E2EE-AES-256-GCM',
      created_at    TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
      updated_at    TIMESTAMPTZ  NOT NULL DEFAULT NOW()
    )
  `);

  await query(`
    CREATE TABLE IF NOT EXISTS participants (
      id            VARCHAR(64) NOT NULL,
      room_id       VARCHAR(12) NOT NULL REFERENCES rooms(id) ON DELETE CASCADE,
      display_name  VARCHAR(255) NOT NULL DEFAULT 'Participante',
      audio         BOOLEAN     NOT NULL DEFAULT TRUE,
      video         BOOLEAN     NOT NULL DEFAULT TRUE,
      joined_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
      PRIMARY KEY (id, room_id)
    )
  `);

  // Index for listing rooms ordered by updated_at
  await query(`
    CREATE INDEX IF NOT EXISTS idx_rooms_updated_at ON rooms (updated_at DESC)
  `);

  // Index for filtering rooms by status
  await query(`
    CREATE INDEX IF NOT EXISTS idx_rooms_status ON rooms (status)
  `);

  // Index for looking up participants by room
  await query(`
    CREATE INDEX IF NOT EXISTS idx_participants_room_id ON participants (room_id)
  `);

  logger.info('Database schema initialized successfully');
}

// ============================================================================
// ROOM CRUD HELPERS
// ============================================================================

/**
 * Insert a new room and return the full room object (with empty participants array).
 */
async function createRoom({ id, name, host, maxParticipants, encryption, createdAt, updatedAt }) {
  const res = await query(
    `INSERT INTO rooms (id, name, host, max_participants, encryption, created_at, updated_at)
     VALUES ($1, $2, $3, $4, $5, $6, $7)
     RETURNING *`,
    [id, name, host, maxParticipants, encryption, createdAt, updatedAt]
  );
  return rowToRoom(res.rows[0], []);
}

/**
 * Get a room by ID including its participants.
 * Returns null if not found.
 */
async function getRoom(id) {
  const roomRes = await query('SELECT * FROM rooms WHERE id = $1', [id]);
  if (roomRes.rows.length === 0) return null;

  const partRes = await query(
    'SELECT * FROM participants WHERE room_id = $1 ORDER BY joined_at',
    [id]
  );

  return rowToRoom(roomRes.rows[0], partRes.rows);
}

/**
 * List rooms with optional status filter, ordered by updated_at DESC.
 * Returns { rooms, total }.
 */
async function listRooms({ status, limit, offset }) {
  let countText = 'SELECT COUNT(*)::int AS total FROM rooms';
  let listText = 'SELECT * FROM rooms';
  const params = [];
  let paramIdx = 1;

  if (status) {
    const where = ` WHERE status = $${paramIdx++}`;
    countText += where;
    listText += where;
    params.push(status);
  }

  listText += ' ORDER BY updated_at DESC';
  listText += ` LIMIT $${paramIdx++} OFFSET $${paramIdx++}`;
  const listParams = [...params, limit, offset];

  const [countRes, listRes] = await Promise.all([
    query(countText, params),
    query(listText, listParams)
  ]);

  // Fetch participants for each room in the current page
  const roomIds = listRes.rows.map(r => r.id);
  let participantsByRoom = {};

  if (roomIds.length > 0) {
    const placeholders = roomIds.map((_, i) => `$${i + 1}`).join(',');
    const partRes = await query(
      `SELECT * FROM participants WHERE room_id IN (${placeholders}) ORDER BY joined_at`,
      roomIds
    );
    for (const p of partRes.rows) {
      if (!participantsByRoom[p.room_id]) participantsByRoom[p.room_id] = [];
      participantsByRoom[p.room_id].push(p);
    }
  }

  const rooms = listRes.rows.map(r => rowToRoom(r, participantsByRoom[r.id] || []));
  return { rooms, total: countRes.rows[0].total };
}

/**
 * Add a participant to a room. Returns the participant object.
 */
async function addParticipant(roomId, { id, displayName, joinedAt, audio, video }) {
  await query(
    `INSERT INTO participants (id, room_id, display_name, audio, video, joined_at)
     VALUES ($1, $2, $3, $4, $5, $6)
     ON CONFLICT (id, room_id) DO NOTHING`,
    [id, roomId, displayName, audio, video, joinedAt]
  );

  await query(
    `UPDATE rooms SET status = 'active', updated_at = NOW() WHERE id = $1`,
    [roomId]
  );

  return { id, displayName, joinedAt, audio, video };
}

/**
 * Remove a participant from a room.
 * If no participants remain, set room status to 'waiting'.
 */
async function removeParticipant(roomId, userId) {
  await transaction(async (client) => {
    await client.query(
      'DELETE FROM participants WHERE id = $1 AND room_id = $2',
      [userId, roomId]
    );

    const countRes = await client.query(
      'SELECT COUNT(*)::int AS cnt FROM participants WHERE room_id = $1',
      [roomId]
    );

    const newStatus = countRes.rows[0].cnt === 0 ? 'waiting' : 'active';
    await client.query(
      'UPDATE rooms SET status = $1, updated_at = NOW() WHERE id = $2',
      [newStatus, roomId]
    );
  });
}

/**
 * Get participant count for a room.
 */
async function getParticipantCount(roomId) {
  const res = await query(
    'SELECT COUNT(*)::int AS cnt FROM participants WHERE room_id = $1',
    [roomId]
  );
  return res.rows[0].cnt;
}

/**
 * Get total participant count across all rooms.
 */
async function getTotalParticipants() {
  const res = await query('SELECT COUNT(*)::int AS cnt FROM participants');
  return res.rows[0].cnt;
}

/**
 * Get active room count.
 */
async function getRoomCount() {
  const res = await query('SELECT COUNT(*)::int AS cnt FROM rooms');
  return res.rows[0].cnt;
}

/**
 * Delete rooms that have no participants and are older than 24 hours (cleanup).
 */
async function cleanupEmptyRooms() {
  const res = await query(`
    DELETE FROM rooms
    WHERE id IN (
      SELECT r.id FROM rooms r
      LEFT JOIN participants p ON p.room_id = r.id
      WHERE p.id IS NULL
        AND r.updated_at < NOW() - INTERVAL '24 hours'
    )
  `);
  if (res.rowCount > 0) {
    logger.info('Cleaned up empty rooms', { count: res.rowCount });
  }
  return res.rowCount;
}

// ============================================================================
// ROW MAPPER
// ============================================================================

/**
 * Convert a database row + participant rows to the room JSON shape
 * expected by the REST API.
 */
function rowToRoom(row, participantRows) {
  return {
    id: row.id,
    name: row.name,
    host: row.host,
    participants: (participantRows || []).map(p => ({
      id: p.id,
      displayName: p.display_name,
      joinedAt: new Date(p.joined_at).toISOString(),
      audio: p.audio,
      video: p.video
    })),
    maxParticipants: row.max_participants,
    status: row.status,
    isLocked: row.is_locked,
    recording: row.recording,
    encryption: row.encryption,
    createdAt: new Date(row.created_at).toISOString(),
    updatedAt: new Date(row.updated_at).toISOString()
  };
}

// ============================================================================
// LIFECYCLE
// ============================================================================

/**
 * Gracefully close the pool.
 */
async function end() {
  logger.info('Closing database connection pool');
  await pool.end();
}

/**
 * Return pool statistics for health checks.
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
  createRoom,
  getRoom,
  listRooms,
  addParticipant,
  removeParticipant,
  getParticipantCount,
  getTotalParticipants,
  getRoomCount,
  cleanupEmptyRooms,
  pool
};
