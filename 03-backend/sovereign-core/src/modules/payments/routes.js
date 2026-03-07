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

// Revenue share: 70% to creator, 30% platform (Sovereign Economy Model)
const TIP_CREATOR_SHARE = 0.70;
const TIP_PLATFORM_SHARE = 0.30;
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
          'Platform revenue share (30%)',
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
// POST /card/tokenize — Tokenize a payment card (PCI-safe)
// Stores a hashed reference, NEVER raw card numbers
// ============================================================
router.post('/card/tokenize',
  validate({
    body: {
      card_last4: t.string({ required: true, min: 4, max: 4 }),
      card_brand: t.string({ required: true, enum: ['visa', 'mastercard', 'amex', 'discover', 'diners', 'unionpay'] }),
      card_exp_month: t.number({ required: true, min: 1, max: 12 }),
      card_exp_year: t.number({ required: true, min: 2026, max: 2040 }),
      cardholder_name: t.string({ required: true, min: 2, max: 100 }),
      billing_country: t.string({ max: 2 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { card_last4, card_brand, card_exp_month, card_exp_year, cardholder_name, billing_country } = req.body;

    // Generate secure card token (never store raw numbers)
    const tokenData = `${req.user.id}:${card_brand}:${card_last4}:${Date.now()}`;
    const cardToken = 'card_' + crypto.createHash('sha256').update(tokenData).digest('hex').slice(0, 32);
    const fingerprint = crypto.createHash('sha256').update(`${card_brand}:${card_last4}:${cardholder_name}`).digest('hex').slice(0, 24);

    const result = await db.query(
      `INSERT INTO payment_cards (user_id, card_token, fingerprint, brand, last4, exp_month, exp_year, cardholder_name, billing_country, status)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, 'active')
       RETURNING id, card_token, brand, last4, exp_month, exp_year, cardholder_name, status, created_at`,
      [req.user.id, cardToken, fingerprint, card_brand, card_last4, card_exp_month, card_exp_year, cardholder_name, billing_country || 'PA']
    );

    audit.record({
      category: 'CARD_TOKENIZED',
      action: 'card_tokenize',
      userId: req.user.id,
      risk: 'MEDIUM',
      details: { brand: card_brand, last4: card_last4 }
    });

    log.info('Card tokenized', { userId: req.user.id, brand: card_brand, last4: card_last4 });

    res.status(201).json({ status: 'ok', data: result.rows[0] });
  })
);

// ============================================================
// GET /cards — List user's saved payment cards
// ============================================================
router.get('/cards', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `SELECT id, card_token, brand, last4, exp_month, exp_year, cardholder_name, billing_country, status, is_default, created_at
     FROM payment_cards
     WHERE user_id = $1 AND status != 'deleted'
     ORDER BY is_default DESC, created_at DESC`,
    [req.user.id]
  );

  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// DELETE /cards/:cardToken — Remove a saved card
// ============================================================
router.delete('/cards/:cardToken', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `UPDATE payment_cards SET status = 'deleted', updated_at = NOW()
     WHERE user_id = $1 AND card_token = $2 AND status = 'active'
     RETURNING id, card_token`,
    [req.user.id, req.params.cardToken]
  );

  if (result.rows.length === 0) throw new AppError('NOT_FOUND', 'Card not found');

  res.json({ status: 'ok', message: 'Card removed' });
}));

// ============================================================
// POST /card/set-default — Set default payment card
// ============================================================
router.post('/card/set-default',
  validate({ body: { card_token: t.string({ required: true }) } }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const client = await db.getClient();
    try {
      await client.query('BEGIN');
      // Unset all defaults
      await client.query(`UPDATE payment_cards SET is_default = false WHERE user_id = $1`, [req.user.id]);
      // Set new default
      const result = await client.query(
        `UPDATE payment_cards SET is_default = true, updated_at = NOW()
         WHERE user_id = $1 AND card_token = $2 AND status = 'active'
         RETURNING id, card_token, brand, last4`,
        [req.user.id, req.body.card_token]
      );
      if (result.rows.length === 0) {
        await client.query('ROLLBACK');
        throw new AppError('NOT_FOUND', 'Card not found');
      }
      await client.query('COMMIT');
      res.json({ status: 'ok', data: result.rows[0] });
    } catch (err) {
      await client.query('ROLLBACK');
      throw err;
    } finally {
      client.release();
    }
  })
);

// ============================================================
// POST /intent — Create a payment intent (card charge flow)
// ============================================================
router.post('/intent',
  validate({
    body: {
      amount: t.number({ required: true, min: 0.01 }),
      currency: t.string({ enum: ['WMP', 'IGT', 'BDET', 'USD', 'EUR', 'PAB'] }),
      card_token: t.string({}),
      description: t.string({ max: 500 }),
      metadata: t.object({})
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { amount, description, metadata } = req.body;
    const currency = req.body.currency || 'USD';
    const cardToken = req.body.card_token || null;

    // Generate unique intent ID
    const intentId = 'pi_' + crypto.createHash('sha256')
      .update(`${req.user.id}:${amount}:${currency}:${Date.now()}:${Math.random()}`)
      .digest('hex').slice(0, 32);

    // If card_token provided, verify it belongs to user
    if (cardToken) {
      const cardCheck = await db.query(
        `SELECT id, brand, last4 FROM payment_cards WHERE user_id = $1 AND card_token = $2 AND status = 'active'`,
        [req.user.id, cardToken]
      );
      if (cardCheck.rows.length === 0) throw new AppError('NOT_FOUND', 'Payment card not found');
    }

    const result = await db.query(
      `INSERT INTO payment_intents (intent_id, user_id, amount, currency, card_token, description, metadata, status)
       VALUES ($1, $2, $3, $4, $5, $6, $7::jsonb, 'requires_confirmation')
       RETURNING intent_id, amount, currency, status, description, created_at`,
      [intentId, req.user.id, amount, currency, cardToken, description || null, JSON.stringify(metadata || {})]
    );

    log.info('Payment intent created', { intentId, userId: req.user.id, amount, currency });

    res.status(201).json({ status: 'ok', data: result.rows[0] });
  })
);

// ============================================================
// POST /intent/:intentId/confirm — Confirm and execute payment
// ============================================================
router.post('/intent/:intentId/confirm', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const { intentId } = req.params;

  const client = await db.getClient();
  try {
    await client.query('BEGIN');

    // Lock the intent row
    const intentResult = await client.query(
      `SELECT * FROM payment_intents WHERE intent_id = $1 AND user_id = $2 FOR UPDATE`,
      [intentId, req.user.id]
    );

    if (intentResult.rows.length === 0) {
      await client.query('ROLLBACK');
      throw new AppError('NOT_FOUND', 'Payment intent not found');
    }

    const intent = intentResult.rows[0];
    if (intent.status !== 'requires_confirmation') {
      await client.query('ROLLBACK');
      throw new AppError('INVALID_STATE', `Intent is ${intent.status}, cannot confirm`);
    }

    // Process based on currency — if crypto (WMP/IGT/BDET), deduct from balance
    // If fiat (USD/EUR/PAB), process card charge
    const isCrypto = ['WMP', 'IGT', 'BDET'].includes(intent.currency);

    if (isCrypto) {
      // Verify balance
      const balResult = await client.query(
        `SELECT COALESCE(SUM(CASE WHEN to_user = $1 THEN amount ELSE 0 END), 0) -
                COALESCE(SUM(CASE WHEN from_user = $1 THEN amount ELSE 0 END), 0) AS balance
         FROM transactions WHERE (from_user = $1 OR to_user = $1) AND currency = $2 AND status = 'completed'`,
        [req.user.id, intent.currency]
      );
      if (parseFloat(balResult.rows[0].balance) < parseFloat(intent.amount)) {
        await client.query('ROLLBACK');
        throw new AppError('INSUFFICIENT_FUNDS', `Insufficient ${intent.currency} balance`);
      }
    }

    // Create the transaction
    const timestamp = new Date().toISOString();
    const txHash = generateTxHash(req.user.id, PLATFORM_TREASURY_ID, intent.amount, intent.currency, timestamp);

    // Split: 70% goes to destination (or stays as payment), 30% platform
    const destinationAmount = Math.floor(parseFloat(intent.amount) * TIP_CREATOR_SHARE * 100000000) / 100000000;
    const platformFee = Math.floor(parseFloat(intent.amount) * TIP_PLATFORM_SHARE * 100000000) / 100000000;

    // Main transaction
    await client.query(
      `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
       VALUES ($1, $2, $3, $4, 'card_payment', $5, $6, 'completed', $7::jsonb, $8)`,
      [req.user.id, PLATFORM_TREASURY_ID, intent.amount, intent.currency,
       intent.description || 'Card payment', txHash,
       JSON.stringify({ intentId, cardToken: intent.card_token, destinationAmount, platformFee }),
       timestamp]
    );

    // Update intent status
    await client.query(
      `UPDATE payment_intents SET status = 'succeeded', confirmed_at = NOW(), tx_hash = $1 WHERE intent_id = $2`,
      [txHash, intentId]
    );

    await client.query('COMMIT');

    audit.transaction(req, { intentId, amount: intent.amount, currency: intent.currency, txHash, type: 'card_payment' });
    log.info('Payment intent confirmed', { intentId, txHash });

    res.json({
      status: 'ok',
      data: {
        intentId,
        txHash,
        amount: intent.amount,
        currency: intent.currency,
        breakdown: { total: parseFloat(intent.amount), destinationAmount, platformFee, platformFeePercent: TIP_PLATFORM_SHARE * 100 },
        status: 'succeeded'
      }
    });
  } catch (err) {
    await client.query('ROLLBACK');
    if (err instanceof AppError) throw err;
    log.error('Payment intent confirmation failed', { err, intentId });
    throw new AppError('TRANSACTION_FAILED', 'Payment could not be completed');
  } finally {
    client.release();
  }
}));

// ============================================================
// POST /intent/:intentId/cancel — Cancel a payment intent
// ============================================================
router.post('/intent/:intentId/cancel', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `UPDATE payment_intents SET status = 'canceled', updated_at = NOW()
     WHERE intent_id = $1 AND user_id = $2 AND status = 'requires_confirmation'
     RETURNING intent_id, status`,
    [req.params.intentId, req.user.id]
  );

  if (result.rows.length === 0) throw new AppError('NOT_FOUND', 'Intent not found or already processed');

  res.json({ status: 'ok', data: result.rows[0] });
}));

// ============================================================
// GET /intents — List payment intents for current user
// ============================================================
router.get('/intents', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const status = req.query.status || null;
  const limit = Math.min(100, Math.max(1, parseInt(req.query.limit, 10) || 20));
  const offset = parseInt(req.query.offset, 10) || 0;

  let query = `SELECT intent_id, amount, currency, status, description, card_token, tx_hash, created_at, confirmed_at
     FROM payment_intents WHERE user_id = $1`;
  const params = [req.user.id];

  if (status) {
    query += ` AND status = $2`;
    params.push(status);
  }

  query += ` ORDER BY created_at DESC LIMIT $${params.length + 1} OFFSET $${params.length + 2}`;
  params.push(limit, offset);

  const result = await db.query(query, params);
  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// POST /refund — Refund a completed payment
// ============================================================
router.post('/refund',
  validate({
    body: {
      tx_hash: t.string({ required: true }),
      amount: t.number({ min: 0.01 }),
      reason: t.string({ max: 500 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { tx_hash, reason } = req.body;

    // Find original transaction
    const original = await db.query(
      `SELECT * FROM transactions WHERE tx_hash = $1 AND status = 'completed'`,
      [tx_hash]
    );
    if (original.rows.length === 0) throw new AppError('NOT_FOUND', 'Original transaction not found');

    const origTx = original.rows[0];
    const refundAmount = req.body.amount || parseFloat(origTx.amount);

    if (refundAmount > parseFloat(origTx.amount)) {
      throw new AppError('INVALID_INPUT', 'Refund amount exceeds original transaction');
    }

    const timestamp = new Date().toISOString();
    const refundHash = generateTxHash(origTx.to_user, origTx.from_user, refundAmount, origTx.currency, timestamp + ':refund');

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
         VALUES ($1, $2, $3, $4, 'refund', $5, $6, 'completed', $7::jsonb, $8)`,
        [origTx.to_user, origTx.from_user, refundAmount, origTx.currency,
         reason || 'Refund', refundHash,
         JSON.stringify({ originalTxHash: tx_hash, refundType: refundAmount < parseFloat(origTx.amount) ? 'partial' : 'full' }),
         timestamp]
      );

      // Mark original as refunded if full refund
      if (refundAmount >= parseFloat(origTx.amount)) {
        await client.query(
          `UPDATE transactions SET status = 'refunded' WHERE tx_hash = $1`,
          [tx_hash]
        );
      }

      await client.query('COMMIT');

      audit.transaction(req, { originalTxHash: tx_hash, refundHash, refundAmount, currency: origTx.currency });
      log.info('Refund processed', { originalTxHash: tx_hash, refundHash, refundAmount });

      res.status(201).json({
        status: 'ok',
        data: { refundHash, amount: refundAmount, currency: origTx.currency, originalTxHash: tx_hash }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      throw new AppError('TRANSACTION_FAILED', 'Refund could not be completed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// Exports
// ============================================================
module.exports = router;
