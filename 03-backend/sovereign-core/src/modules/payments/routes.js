'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// BDET Payment Routes v1.0.0
// Sovereign digital economy transactions (WAMPUM / IGT)
// Send, receive, tip creators, balance, history
// ============================================================

const { Router } = require('express');
const crypto = require('crypto');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:payments');
const audit = createAuditLogger('sovereign-core:payments');

// Default currency for the sovereign economy
const DEFAULT_CURRENCY = 'WMP'; // WAMPUM

// Creator tip split: 92% to creator, 8% platform fee
const TIP_CREATOR_SHARE = 0.92;
const TIP_PLATFORM_SHARE = 0.08;
const PLATFORM_TREASURY_ID = process.env.PLATFORM_TREASURY_ID || 'system-treasury';

// ============================================================
// Helper: Generate transaction hash for audit trail
// ============================================================
function generateTxHash(fromUser, toUser, amount, currency, timestamp) {
  return crypto
    .createHash('sha256')
    .update(`${fromUser}:${toUser}:${amount}:${currency}:${timestamp}`)
    .digest('hex');
}

// ============================================================
// POST /send — Transfer funds between users
// ============================================================
router.post('/send',
  validate({
    body: {
      to_user: t.uuid({ required: true }),
      amount: t.number({ required: true, min: 0.00000001 }),
      currency: t.string({ enum: ['WMP', 'IGT', 'BDET'] }),
      type: t.string({ enum: ['transfer', 'payment', 'escrow', 'refund'] }),
      memo: t.string({ max: 500 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required');
    }

    const fromUser = req.user.id;
    const { to_user, amount, memo } = req.body;
    const currency = req.body.currency || DEFAULT_CURRENCY;
    const type = req.body.type || 'transfer';

    // Cannot send to self
    if (fromUser === to_user) {
      throw new AppError('INVALID_INPUT', 'Cannot send funds to yourself');
    }

    // Verify recipient exists
    const recipientCheck = await db.query(
      `SELECT id, status FROM users WHERE id = $1`,
      [to_user]
    );

    if (recipientCheck.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'Recipient user not found');
    }

    if (recipientCheck.rows[0].status !== 'active') {
      throw new AppError('INVALID_INPUT', 'Recipient account is not active');
    }

    // Calculate sender's balance
    const balanceResult = await db.query(
      `SELECT
         COALESCE(SUM(CASE WHEN to_user = $1 THEN amount ELSE 0 END), 0) -
         COALESCE(SUM(CASE WHEN from_user = $1 THEN amount ELSE 0 END), 0) AS balance
       FROM transactions
       WHERE (from_user = $1 OR to_user = $1)
         AND currency = $2
         AND status = 'completed'`,
      [fromUser, currency]
    );

    const balance = parseFloat(balanceResult.rows[0].balance);

    if (balance < amount) {
      throw new AppError('INSUFFICIENT_FUNDS', `Insufficient ${currency} balance. Available: ${balance.toFixed(8)}, requested: ${amount}`);
    }

    // Generate transaction hash
    const timestamp = new Date().toISOString();
    const txHash = generateTxHash(fromUser, to_user, amount, currency, timestamp);

    // Insert transaction (use DB transaction for atomicity)
    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      const result = await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at)
         VALUES ($1, $2, $3, $4, $5, $6, $7, 'completed', $8)
         RETURNING id, from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at`,
        [fromUser, to_user, amount, currency, type, memo || null, txHash, timestamp]
      );

      await client.query('COMMIT');

      const tx = result.rows[0];

      audit.transaction(req, {
        from: fromUser,
        to: to_user,
        amount,
        currency,
        type,
        txHash
      });

      log.info('Transaction completed', { txId: tx.id, from: fromUser, to: to_user, amount, currency });

      res.status(201).json({
        status: 'ok',
        data: tx
      });
    } catch (err) {
      await client.query('ROLLBACK');
      log.error('Transaction failed', { err, from: fromUser, to: to_user, amount });
      throw new AppError('TRANSACTION_FAILED', 'Transaction could not be completed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// GET /history — Transaction history for current user
// ============================================================
router.get('/history',
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required');
    }

    const userId = req.user.id;
    const page = Math.max(1, parseInt(req.query.page, 10) || 1);
    const limit = Math.min(100, Math.max(1, parseInt(req.query.limit, 10) || 20));
    const offset = (page - 1) * limit;
    const currency = req.query.currency || null;
    const type = req.query.type || null;
    const direction = req.query.direction || null; // 'sent' | 'received'

    // Build conditions
    const conditions = [`(t.from_user = $1 OR t.to_user = $1)`];
    const params = [userId];
    let paramIdx = 2;

    if (currency) {
      conditions.push(`t.currency = $${paramIdx}`);
      params.push(currency);
      paramIdx++;
    }

    if (type) {
      conditions.push(`t.type = $${paramIdx}`);
      params.push(type);
      paramIdx++;
    }

    if (direction === 'sent') {
      conditions[0] = `t.from_user = $1`;
    } else if (direction === 'received') {
      conditions[0] = `t.to_user = $1`;
    }

    const whereClause = conditions.join(' AND ');

    // Count total
    const countResult = await db.query(
      `SELECT COUNT(*) AS total FROM transactions t WHERE ${whereClause}`,
      params
    );
    const total = parseInt(countResult.rows[0].total, 10);

    // Fetch transactions
    const dataResult = await db.query(
      `SELECT t.id, t.from_user, t.to_user, t.amount, t.currency, t.type, t.memo, t.tx_hash, t.status, t.created_at,
              uf.display_name AS from_name,
              ut.display_name AS to_name,
              CASE
                WHEN t.from_user = $1 THEN 'sent'
                ELSE 'received'
              END AS direction
       FROM transactions t
       LEFT JOIN users uf ON uf.id = t.from_user
       LEFT JOIN users ut ON ut.id = t.to_user
       WHERE ${whereClause}
       ORDER BY t.created_at DESC
       LIMIT $${paramIdx} OFFSET $${paramIdx + 1}`,
      [...params, limit, offset]
    );

    res.json({
      status: 'ok',
      data: dataResult.rows,
      pagination: {
        page,
        limit,
        total,
        totalPages: Math.ceil(total / limit),
        hasNext: page < Math.ceil(total / limit),
        hasPrev: page > 1
      }
    });
  })
);

// ============================================================
// GET /balance — Calculate balance from transactions
// ============================================================
router.get('/balance', asyncHandler(async (req, res) => {
  if (!req.user) {
    throw new AppError('AUTH_REQUIRED', 'Authentication required');
  }

  const userId = req.user.id;

  // Calculate balances per currency
  const result = await db.query(
    `SELECT
       currency,
       COALESCE(SUM(CASE WHEN to_user = $1 THEN amount ELSE 0 END), 0) AS total_received,
       COALESCE(SUM(CASE WHEN from_user = $1 THEN amount ELSE 0 END), 0) AS total_sent,
       COALESCE(SUM(CASE WHEN to_user = $1 THEN amount ELSE 0 END), 0) -
       COALESCE(SUM(CASE WHEN from_user = $1 THEN amount ELSE 0 END), 0) AS balance
     FROM transactions
     WHERE (from_user = $1 OR to_user = $1)
       AND status = 'completed'
     GROUP BY currency
     ORDER BY currency`,
    [userId]
  );

  // Build balances map
  const balances = {};
  for (const row of result.rows) {
    balances[row.currency] = {
      balance: parseFloat(row.balance),
      totalReceived: parseFloat(row.total_received),
      totalSent: parseFloat(row.total_sent)
    };
  }

  // Ensure default currency always appears
  if (!balances[DEFAULT_CURRENCY]) {
    balances[DEFAULT_CURRENCY] = { balance: 0, totalReceived: 0, totalSent: 0 };
  }

  res.json({
    status: 'ok',
    data: {
      userId,
      balances,
      defaultCurrency: DEFAULT_CURRENCY
    }
  });
}));

// ============================================================
// POST /tip — Creator tip (92% to creator, 8% platform)
// ============================================================
router.post('/tip',
  validate({
    body: {
      creator_id: t.uuid({ required: true }),
      amount: t.number({ required: true, min: 0.01 }),
      currency: t.string({ enum: ['WMP', 'IGT', 'BDET'] }),
      content_id: t.uuid({}),
      memo: t.string({ max: 500 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required');
    }

    const fromUser = req.user.id;
    const { creator_id, amount, content_id, memo } = req.body;
    const currency = req.body.currency || DEFAULT_CURRENCY;

    // Cannot tip yourself
    if (fromUser === creator_id) {
      throw new AppError('INVALID_INPUT', 'Cannot tip yourself');
    }

    // Verify creator exists and is active
    const creatorCheck = await db.query(
      `SELECT id, display_name, status FROM users WHERE id = $1`,
      [creator_id]
    );

    if (creatorCheck.rows.length === 0 || creatorCheck.rows[0].status !== 'active') {
      throw new AppError('NOT_FOUND', 'Creator not found or inactive');
    }

    // Check balance
    const balanceResult = await db.query(
      `SELECT
         COALESCE(SUM(CASE WHEN to_user = $1 THEN amount ELSE 0 END), 0) -
         COALESCE(SUM(CASE WHEN from_user = $1 THEN amount ELSE 0 END), 0) AS balance
       FROM transactions
       WHERE (from_user = $1 OR to_user = $1)
         AND currency = $2
         AND status = 'completed'`,
      [fromUser, currency]
    );

    const balance = parseFloat(balanceResult.rows[0].balance);
    if (balance < amount) {
      throw new AppError('INSUFFICIENT_FUNDS', `Insufficient ${currency} balance for tip. Available: ${balance.toFixed(8)}`);
    }

    // Calculate split
    const creatorAmount = Math.floor(amount * TIP_CREATOR_SHARE * 100000000) / 100000000;
    const platformFee = Math.floor(amount * TIP_PLATFORM_SHARE * 100000000) / 100000000;
    const timestamp = new Date().toISOString();

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // 1. Creator receives their share
      const creatorTxHash = generateTxHash(fromUser, creator_id, creatorAmount, currency, timestamp);
      const creatorTx = await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
         VALUES ($1, $2, $3, $4, 'tip', $5, $6, 'completed', $7::jsonb, $8)
         RETURNING id, from_user, to_user, amount, currency, type, tx_hash, status, created_at`,
        [
          fromUser, creator_id, creatorAmount, currency,
          memo || `Tip for ${creatorCheck.rows[0].display_name}`,
          creatorTxHash,
          JSON.stringify({ tip: true, content_id: content_id || null, originalAmount: amount, creatorShare: TIP_CREATOR_SHARE }),
          timestamp
        ]
      );

      // 2. Platform treasury receives fee
      const platformTxHash = generateTxHash(fromUser, PLATFORM_TREASURY_ID, platformFee, currency, timestamp + ':fee');
      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
         VALUES ($1, $2, $3, $4, 'platform_fee', $5, $6, 'completed', $7::jsonb, $8)`,
        [
          fromUser, PLATFORM_TREASURY_ID, platformFee, currency,
          'Platform tip fee (8%)',
          platformTxHash,
          JSON.stringify({ tip: true, parentTxId: creatorTx.rows[0].id, feeRate: TIP_PLATFORM_SHARE }),
          timestamp
        ]
      );

      await client.query('COMMIT');

      audit.transaction(req, {
        from: fromUser,
        to: creator_id,
        amount,
        currency,
        type: 'tip',
        txHash: creatorTxHash,
        creatorAmount,
        platformFee
      });

      log.info('Tip sent', { from: fromUser, creator: creator_id, amount, creatorAmount, platformFee });

      res.status(201).json({
        status: 'ok',
        data: {
          transaction: creatorTx.rows[0],
          breakdown: {
            totalAmount: amount,
            creatorReceives: creatorAmount,
            platformFee: platformFee,
            creatorSharePercent: TIP_CREATOR_SHARE * 100,
            platformFeePercent: TIP_PLATFORM_SHARE * 100
          }
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      log.error('Tip transaction failed', { err, from: fromUser, creator: creator_id, amount });
      throw new AppError('TRANSACTION_FAILED', 'Tip transaction could not be completed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// Exports
// ============================================================
module.exports = router;
