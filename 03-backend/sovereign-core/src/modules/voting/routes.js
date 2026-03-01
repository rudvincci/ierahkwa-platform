'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Voting & Elections Routes v1.0.0
// Sovereign digital democracy — elections, ballots, results
// SHA-256 ballot hashing for audit transparency
// ============================================================

const { Router } = require('express');
const crypto = require('crypto');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');
const { requireRole } = require('../../middleware/auth');

const router = Router();
const log = createLogger('sovereign-core:voting');
const audit = createAuditLogger('sovereign-core:voting');

// ============================================================
// POST /create-election — Create a new election (admin only)
// ============================================================
router.post('/create-election',
  requireRole('admin'),
  validate({
    body: {
      title: t.string({ required: true, min: 3, max: 500 }),
      description: t.string({ max: 10000 }),
      choices: t.array({ required: true, min: 2 }),
      start_date: t.date({ required: true }),
      end_date: t.date({ required: true }),
      platform: t.string({ max: 64 }),
      election_type: t.string({ enum: ['simple_majority', 'ranked_choice', 'approval', 'consensus'] })
    }
  }),
  asyncHandler(async (req, res) => {
    const { title, description, choices, start_date, end_date, platform, election_type } = req.body;

    // Validate dates
    const startDate = new Date(start_date);
    const endDate = new Date(end_date);

    if (endDate <= startDate) {
      throw new AppError('INVALID_INPUT', 'End date must be after start date');
    }

    // Validate choices array
    if (!Array.isArray(choices) || choices.length < 2) {
      throw new AppError('INVALID_INPUT', 'Election must have at least 2 choices');
    }

    // Ensure choices are strings
    const sanitizedChoices = choices.map((c, i) => {
      if (typeof c === 'string') return c.trim();
      if (typeof c === 'object' && c.label) return c.label.trim();
      throw new AppError('INVALID_INPUT', `Choice at index ${i} must be a string or object with a label`);
    });

    // Determine initial status
    const now = new Date();
    let status = 'pending';
    if (now >= startDate && now <= endDate) status = 'active';
    else if (now > endDate) status = 'closed';

    const result = await db.query(
      `INSERT INTO elections (title, description, choices, start_date, end_date, platform, election_type, status, created_by, created_at, updated_at)
       VALUES ($1, $2, $3::jsonb, $4, $5, $6, $7, $8, $9, NOW(), NOW())
       RETURNING id, title, description, choices, start_date, end_date, platform, election_type, status, created_by, created_at`,
      [
        title.trim(),
        description ? description.trim() : null,
        JSON.stringify(sanitizedChoices),
        startDate.toISOString(),
        endDate.toISOString(),
        platform || null,
        election_type || 'simple_majority',
        status,
        req.user.id
      ]
    );

    const election = result.rows[0];

    audit.adminAction(req, 'election_created', { electionId: election.id, title, choiceCount: sanitizedChoices.length });
    log.info('Election created', { electionId: election.id, title, status });

    res.status(201).json({
      status: 'ok',
      data: election
    });
  })
);

// ============================================================
// POST /cast — Cast a ballot in an election
// ============================================================
router.post('/cast',
  validate({
    body: {
      election_id: t.uuid({ required: true }),
      choice: t.string({ required: true, min: 1 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required');
    }

    const userId = req.user.id;
    const { election_id, choice } = req.body;

    // Fetch election
    const electionResult = await db.query(
      `SELECT id, title, choices, start_date, end_date, status, election_type
       FROM elections
       WHERE id = $1`,
      [election_id]
    );

    if (electionResult.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'Election not found');
    }

    const election = electionResult.rows[0];

    // Validate election is active
    const now = new Date();
    if (election.status !== 'active' || now < new Date(election.start_date) || now > new Date(election.end_date)) {
      throw new AppError('CONFLICT', 'This election is not currently open for voting');
    }

    // Validate choice is one of the allowed choices
    const validChoices = typeof election.choices === 'string' ? JSON.parse(election.choices) : election.choices;
    if (!validChoices.includes(choice)) {
      throw new AppError('INVALID_INPUT', `Invalid choice. Available options: ${validChoices.join(', ')}`);
    }

    // Check if user has already voted (one vote per user per election)
    const existingVote = await db.query(
      `SELECT id FROM ballots WHERE election_id = $1 AND voter_id = $2`,
      [election_id, userId]
    );

    if (existingVote.rows.length > 0) {
      throw new AppError('ALREADY_EXISTS', 'You have already cast a ballot in this election');
    }

    // Generate SHA-256 ballot hash for audit transparency
    const ballotData = `${election_id}:${userId}:${choice}:${now.toISOString()}`;
    const ballotHash = crypto.createHash('sha256').update(ballotData).digest('hex');

    const result = await db.query(
      `INSERT INTO ballots (election_id, voter_id, choice, ballot_hash, created_at)
       VALUES ($1, $2, $3, $4, NOW())
       RETURNING id, election_id, choice, ballot_hash, created_at`,
      [election_id, userId, choice, ballotHash]
    );

    const ballot = result.rows[0];

    // Do NOT include voter_id in the response for ballot secrecy
    audit.voteCast(req, election_id, choice);
    log.info('Ballot cast', { electionId: election_id, ballotHash });

    res.status(201).json({
      status: 'ok',
      data: {
        id: ballot.id,
        election_id: ballot.election_id,
        choice: ballot.choice,
        ballot_hash: ballot.ballot_hash,
        cast_at: ballot.created_at,
        message: 'Your vote has been recorded. Keep your ballot hash for verification.'
      }
    });
  })
);

// ============================================================
// GET /results/:electionId — Election results
// ============================================================
router.get('/results/:electionId',
  validate({ params: { electionId: t.uuid({ required: true }) } }),
  asyncHandler(async (req, res) => {
    const { electionId } = req.params;

    // Fetch election
    const electionResult = await db.query(
      `SELECT id, title, description, choices, start_date, end_date, status, election_type, created_by, created_at
       FROM elections
       WHERE id = $1`,
      [electionId]
    );

    if (electionResult.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'Election not found');
    }

    const election = electionResult.rows[0];

    // Count ballots grouped by choice
    const resultsData = await db.query(
      `SELECT choice, COUNT(*) AS votes
       FROM ballots
       WHERE election_id = $1
       GROUP BY choice
       ORDER BY votes DESC`,
      [electionId]
    );

    // Total votes
    const totalResult = await db.query(
      `SELECT COUNT(*) AS total FROM ballots WHERE election_id = $1`,
      [electionId]
    );
    const totalVotes = parseInt(totalResult.rows[0].total, 10);

    // Build results with percentages
    const results = resultsData.rows.map(row => ({
      choice: row.choice,
      votes: parseInt(row.votes, 10),
      percentage: totalVotes > 0 ? Math.round((parseInt(row.votes, 10) / totalVotes) * 10000) / 100 : 0
    }));

    // Include choices with zero votes
    const validChoices = typeof election.choices === 'string' ? JSON.parse(election.choices) : election.choices;
    const votedChoices = new Set(results.map(r => r.choice));
    for (const c of validChoices) {
      if (!votedChoices.has(c)) {
        results.push({ choice: c, votes: 0, percentage: 0 });
      }
    }

    // Determine winner (for simple majority)
    const winner = results.length > 0 && results[0].votes > 0 ? results[0].choice : null;

    res.json({
      status: 'ok',
      data: {
        election: {
          id: election.id,
          title: election.title,
          description: election.description,
          electionType: election.election_type,
          status: election.status,
          startDate: election.start_date,
          endDate: election.end_date
        },
        results,
        totalVotes,
        winner: election.status === 'closed' ? winner : null,
        isFinalized: election.status === 'closed'
      }
    });
  })
);

// ============================================================
// GET /active — Currently active elections
// ============================================================
router.get('/active', asyncHandler(async (req, res) => {
  const platform = req.query.platform || null;

  const conditions = [`status = 'active'`, `NOW() BETWEEN start_date AND end_date`];
  const params = [];
  let paramIdx = 1;

  if (platform) {
    conditions.push(`platform = $${paramIdx}`);
    params.push(platform);
    paramIdx++;
  }

  const whereClause = conditions.join(' AND ');

  const result = await db.query(
    `SELECT e.id, e.title, e.description, e.choices, e.start_date, e.end_date, e.platform, e.election_type, e.status,
            e.created_by, u.display_name AS created_by_name,
            (SELECT COUNT(*) FROM ballots WHERE election_id = e.id) AS total_votes
     FROM elections e
     LEFT JOIN users u ON u.id = e.created_by
     WHERE ${whereClause}
     ORDER BY e.end_date ASC`,
    params
  );

  // If user is authenticated, mark which elections they've already voted in
  if (req.user) {
    const votedResult = await db.query(
      `SELECT DISTINCT election_id FROM ballots WHERE voter_id = $1`,
      [req.user.id]
    );
    const votedSet = new Set(votedResult.rows.map(r => r.election_id));

    for (const election of result.rows) {
      election.has_voted = votedSet.has(election.id);
    }
  }

  res.json({
    status: 'ok',
    data: result.rows,
    total: result.rows.length
  });
}));

// ============================================================
// Exports
// ============================================================
module.exports = router;
