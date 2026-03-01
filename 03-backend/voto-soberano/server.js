'use strict';

// ============================================================
// Ierahkwa Platform — Voto Soberano v1.0.0
// Sovereign voting backend for indigenous digital governance
// Immutable ballot chain with SHA-256 integrity hashing
// ============================================================

const express = require('express');
const cors = require('cors');
const crypto = require('crypto');
const jwt = require('jsonwebtoken');

// Shared modules
const { createLogger } = require('../shared/logger');
const { createAuditLogger, AUDIT_CATEGORIES, RISK_LEVELS } = require('../shared/audit');
const { AppError, errorMiddleware, asyncHandler, notFoundHandler } = require('../shared/error-handler');
const { corsConfig, applySecurityMiddleware, rateLimiters } = require('../shared/security');

// Database
const db = require('./db');

// ============================================================
// Configuration
// ============================================================
const PORT = process.env.PORT || 3006;
const JWT_SECRET = process.env.JWT_SECRET || 'voto-soberano-dev-secret-change-in-prod';
const SERVICE_NAME = 'voto-soberano';

const log = createLogger(SERVICE_NAME);
const audit = createAuditLogger(SERVICE_NAME);
const app = express();

// ============================================================
// Middleware
// ============================================================
app.use(cors(corsConfig()));
app.use(express.json());
applySecurityMiddleware(app, SERVICE_NAME);
app.use(log.requestLogger());
app.use(audit.middleware({ pathFilter: (p) => p.startsWith('/v1/') }));

// ============================================================
// Ballot Chain State
// ============================================================
let ballotChainTip = '0'.repeat(64); // genesis hash for ballot chain

// ============================================================
// Auth Middleware — verifies JWT bearer token
// ============================================================
function authenticate(req, res, next) {
  const header = req.headers.authorization;
  if (!header || !header.startsWith('Bearer ')) {
    throw new AppError('AUTH_REQUIRED', 'Bearer token is required');
  }
  try {
    const token = header.slice(7);
    const payload = jwt.verify(token, JWT_SECRET);
    req.userId = payload.sub || payload.id;
    req.userRole = payload.role || 'citizen';
    next();
  } catch (err) {
    throw new AppError('AUTH_INVALID_TOKEN', 'Token is invalid or expired');
  }
}

function requireAdmin(req, _res, next) {
  if (req.userRole !== 'admin') {
    throw new AppError('AUTH_INSUFFICIENT_ROLE', 'Admin privileges required');
  }
  next();
}

// ============================================================
// Helpers
// ============================================================
function generateId() {
  return crypto.randomUUID();
}

function hashBallot(ballot, previousHash) {
  const payload = JSON.stringify({
    id: ballot.id,
    electionId: ballot.electionId,
    voterId: ballot.voterIdHash,
    choice: ballot.choice,
    ts: ballot.createdAt,
    prev: previousHash
  });
  return crypto.createHash('sha256').update(payload).digest('hex');
}

async function computeResults(electionId) {
  const electionResult = await db.query('SELECT * FROM elections WHERE id = $1', [electionId]);
  if (electionResult.rows.length === 0) return null;
  const election = electionResult.rows[0];
  const choices = election.choices;

  const countResult = await db.query(
    'SELECT choice, COUNT(*) as votes FROM ballots WHERE election_id = $1 GROUP BY choice',
    [electionId]
  );
  const totalResult = await db.query(
    'SELECT COUNT(*) as total FROM ballots WHERE election_id = $1',
    [electionId]
  );
  const totalVotes = parseInt(totalResult.rows[0].total);

  const countMap = {};
  for (const row of countResult.rows) {
    countMap[row.choice] = parseInt(row.votes);
  }

  const results = choices.map((choice) => ({
    choice,
    votes: countMap[choice] || 0,
    percentage: totalVotes > 0 ? Math.round(((countMap[choice] || 0) / totalVotes) * 10000) / 100 : 0
  }));

  return { totalVotes, results };
}

function isElectionActive(election) {
  const now = new Date();
  return (
    election.status === 'open' &&
    new Date(election.start_date) <= now &&
    new Date(election.end_date) >= now
  );
}

/**
 * Map a DB row (snake_case) to the API response shape (camelCase)
 */
function mapElectionRow(row) {
  return {
    id: row.id,
    title: row.title,
    description: row.description,
    choices: row.choices,
    startDate: row.start_date instanceof Date ? row.start_date.toISOString() : row.start_date,
    endDate: row.end_date instanceof Date ? row.end_date.toISOString() : row.end_date,
    status: row.status,
    createdBy: row.created_by,
    createdAt: row.created_at instanceof Date ? row.created_at.toISOString() : row.created_at,
    ...(row.closed_at ? { closedAt: row.closed_at instanceof Date ? row.closed_at.toISOString() : row.closed_at } : {}),
    ...(row.closed_by ? { closedBy: row.closed_by } : {})
  };
}

// ============================================================
// Routes — Health (public)
// ============================================================
app.get('/health', asyncHandler(async (_req, res) => {
  const electionCount = await db.query('SELECT COUNT(*) as cnt FROM elections');
  const ballotCount = await db.query('SELECT COUNT(*) as cnt FROM ballots');
  const dbStats = db.stats();

  res.json({
    service: SERVICE_NAME,
    status: 'operational',
    version: '1.0.0',
    elections: parseInt(electionCount.rows[0].cnt),
    totalBallots: parseInt(ballotCount.rows[0].cnt),
    db: dbStats,
    uptime: process.uptime(),
    timestamp: new Date().toISOString()
  });
}));

// ============================================================
// Routes — Elections
// ============================================================

// POST /v1/elections — Create a new election
app.post('/v1/elections', authenticate, requireAdmin, asyncHandler(async (req, res) => {
  const { title, description, choices, start_date, end_date } = req.body;

  if (!title || !choices || !Array.isArray(choices) || choices.length < 2) {
    throw new AppError('VALIDATION_FAILED', 'title and choices (array with >= 2 options) are required');
  }
  if (!start_date || !end_date) {
    throw new AppError('MISSING_FIELD', 'start_date and end_date are required');
  }
  if (new Date(end_date) <= new Date(start_date)) {
    throw new AppError('VALIDATION_FAILED', 'end_date must be after start_date');
  }

  const id = generateId();
  const startDate = new Date(start_date).toISOString();
  const endDate = new Date(end_date).toISOString();
  const createdAt = new Date().toISOString();

  await db.query(
    `INSERT INTO elections (id, title, description, choices, start_date, end_date, status, created_by, created_at)
     VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9)`,
    [id, title, description || '', JSON.stringify(choices), startDate, endDate, 'open', req.userId, createdAt]
  );

  const election = {
    id,
    title,
    description: description || '',
    choices,
    startDate,
    endDate,
    status: 'open',
    createdBy: req.userId,
    createdAt
  };

  audit.record({
    category: AUDIT_CATEGORIES.PROPOSAL_CREATED,
    action: 'election_created',
    risk: RISK_LEVELS.HIGH,
    req,
    resource: { type: 'election', id: election.id, name: title },
    details: { choices, startDate: election.startDate, endDate: election.endDate }
  });

  log.info('Election created', { electionId: election.id, title });
  res.status(201).json(election);
}));

// GET /v1/elections/active — Currently active elections
app.get('/v1/elections/active', authenticate, asyncHandler(async (_req, res) => {
  const result = await db.query('SELECT * FROM elections WHERE status = $1', ['open']);
  const active = result.rows.filter(isElectionActive).map(mapElectionRow);
  res.json({ elections: active, count: active.length });
}));

// GET /v1/elections — List all elections
app.get('/v1/elections', authenticate, asyncHandler(async (_req, res) => {
  const result = await db.query('SELECT * FROM elections ORDER BY created_at DESC');
  const all = result.rows.map(mapElectionRow);
  res.json({ elections: all, count: all.length });
}));

// GET /v1/elections/:id — Election detail with vote counts
app.get('/v1/elections/:id', authenticate, asyncHandler(async (req, res) => {
  const electionResult = await db.query('SELECT * FROM elections WHERE id = $1', [req.params.id]);
  if (electionResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Election not found');

  const election = mapElectionRow(electionResult.rows[0]);
  const result = await computeResults(election.id);
  res.json({ ...election, ...result });
}));

// POST /v1/elections/:id/vote — Cast a vote
app.post('/v1/elections/:id/vote', authenticate, rateLimiters.auth(), asyncHandler(async (req, res) => {
  const electionResult = await db.query('SELECT * FROM elections WHERE id = $1', [req.params.id]);
  if (electionResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Election not found');
  const election = electionResult.rows[0];

  if (!isElectionActive(election)) {
    throw new AppError('CONFLICT', 'Election is not currently active');
  }

  const { choice } = req.body;
  if (!choice || !election.choices.includes(choice)) {
    throw new AppError('VALIDATION_FAILED', `Invalid choice. Must be one of: ${election.choices.join(', ')}`);
  }

  // Enforce one vote per user per election via DB unique constraint check
  const voterIdHash = crypto.createHash('sha256').update(req.userId).digest('hex');
  const existing = await db.query(
    'SELECT 1 FROM ballots WHERE election_id = $1 AND voter_id_hash = $2',
    [election.id, voterIdHash]
  );
  if (existing.rows.length > 0) {
    throw new AppError('ALREADY_EXISTS', 'You have already voted in this election');
  }

  // Build immutable ballot with hash chain
  const ballot = {
    id: generateId(),
    electionId: election.id,
    voterIdHash, // store hash, not raw userId — voter privacy
    choice,
    previousHash: ballotChainTip,
    createdAt: new Date().toISOString()
  };
  ballot.receiptHash = hashBallot(ballot, ballotChainTip);
  ballotChainTip = ballot.receiptHash;

  // Persist ballot to PostgreSQL
  await db.query(
    `INSERT INTO ballots (id, election_id, voter_id_hash, choice, previous_hash, receipt_hash, created_at)
     VALUES ($1, $2, $3, $4, $5, $6, $7)`,
    [ballot.id, ballot.electionId, ballot.voterIdHash, ballot.choice, ballot.previousHash, ballot.receiptHash, ballot.createdAt]
  );

  // Audit trail
  audit.voteCast(req, election.id, { choice, receiptHash: ballot.receiptHash });

  log.info('Vote cast', { electionId: election.id, receiptHash: ballot.receiptHash });

  res.status(201).json({
    message: 'Vote recorded successfully',
    receiptHash: ballot.receiptHash,
    electionId: election.id,
    timestamp: ballot.createdAt
  });
}));

// GET /v1/elections/:id/results — Aggregated results
app.get('/v1/elections/:id/results', authenticate, asyncHandler(async (req, res) => {
  const electionResult = await db.query('SELECT * FROM elections WHERE id = $1', [req.params.id]);
  if (electionResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Election not found');
  const election = electionResult.rows[0];

  const result = await computeResults(election.id);
  res.json({
    electionId: election.id,
    title: election.title,
    status: election.status,
    ...result
  });
}));

// POST /v1/elections/:id/close — Close an election (admin only)
app.post('/v1/elections/:id/close', authenticate, requireAdmin, asyncHandler(async (req, res) => {
  const electionResult = await db.query('SELECT * FROM elections WHERE id = $1', [req.params.id]);
  if (electionResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Election not found');
  const election = electionResult.rows[0];

  if (election.status === 'closed') {
    throw new AppError('CONFLICT', 'Election is already closed');
  }

  const closedAt = new Date().toISOString();
  await db.query(
    'UPDATE elections SET status = $1, closed_at = $2, closed_by = $3 WHERE id = $4',
    ['closed', closedAt, req.userId, election.id]
  );

  const result = await computeResults(election.id);

  audit.adminAction(req, 'election_closed', {
    electionId: election.id,
    title: election.title,
    totalVotes: result.totalVotes
  });

  log.info('Election closed', { electionId: election.id, totalVotes: result.totalVotes });

  res.json({
    message: 'Election closed',
    electionId: election.id,
    closedAt,
    ...result
  });
}));

// ============================================================
// Error handling
// ============================================================
app.use(notFoundHandler);
app.use(errorMiddleware(SERVICE_NAME, log));

// ============================================================
// Server start & graceful shutdown
// ============================================================
let server;

async function startServer() {
  await db.initialize();

  // Load ballot chain tip from last ballot in the database
  const lastBallot = await db.query('SELECT receipt_hash FROM ballots ORDER BY created_at DESC LIMIT 1');
  if (lastBallot.rows.length > 0) {
    ballotChainTip = lastBallot.rows[0].receipt_hash;
    log.info('Ballot chain tip loaded from database', { chainTip: ballotChainTip.slice(0, 12) + '...' });
  }

  server = app.listen(PORT, () => {
    log.info(`${SERVICE_NAME} listening`, { port: PORT });
    audit.record({
      category: AUDIT_CATEGORIES.SYSTEM_STARTUP,
      action: 'service_started',
      risk: RISK_LEVELS.LOW,
      details: { port: PORT, nodeVersion: process.version, storage: 'postgresql' }
    });
  });
}

function gracefulShutdown(signal) {
  log.info(`${signal} received — shutting down`, { signal });
  audit.record({
    category: AUDIT_CATEGORIES.SYSTEM_SHUTDOWN,
    action: 'service_stopped',
    risk: RISK_LEVELS.LOW,
    details: { signal, uptime: process.uptime() }
  });

  if (server) {
    server.close(async () => {
      log.info('Server closed');
      await db.end();
      log.info('Database pool closed');
      process.exit(0);
    });
  } else {
    db.end().then(() => process.exit(0)).catch(() => process.exit(1));
  }

  // Force exit after 10 seconds if graceful shutdown stalls
  setTimeout(() => {
    log.error('Forced shutdown — timeout exceeded');
    process.exit(1);
  }, 10000).unref();
}

process.on('SIGTERM', () => gracefulShutdown('SIGTERM'));
process.on('SIGINT', () => gracefulShutdown('SIGINT'));

startServer().catch((err) => {
  log.error('Failed to start server', { error: err.message });
  process.exit(1);
});

module.exports = app;
