'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// ATM Module v1.0.0 — Sovereign ATM Network
// Cash deposits, withdrawals, ATM locations, nation coverage
// BDET Bank — MameyNode blockchain (Chain ID 574)
// ============================================================

const { Router } = require('express');
const crypto = require('crypto');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:atm');
const audit = createAuditLogger('sovereign-core:atm');

const PLATFORM_TREASURY_ID = process.env.PLATFORM_TREASURY_ID || 'system-treasury';

// ============================================================
// ATM Types & Capabilities
// ============================================================
const ATM_TYPES = {
  full: {
    name: 'Full Service ATM',
    capabilities: ['cash_deposit', 'cash_withdrawal', 'check_deposit', 'balance_inquiry', 'transfer', 'bill_payment', 'wpm_purchase', 'card_issuance'],
    maxDeposit: 50000,
    maxWithdrawal: 10000,
    denominations: [1, 5, 10, 20, 50, 100]
  },
  deposit: {
    name: 'Cash Deposit ATM',
    capabilities: ['cash_deposit', 'check_deposit', 'balance_inquiry', 'wpm_purchase'],
    maxDeposit: 25000,
    maxWithdrawal: 0,
    denominations: [1, 5, 10, 20, 50, 100]
  },
  standard: {
    name: 'Standard ATM',
    capabilities: ['cash_withdrawal', 'balance_inquiry', 'transfer'],
    maxDeposit: 0,
    maxWithdrawal: 5000,
    denominations: [20, 50, 100]
  },
  mini: {
    name: 'Mini ATM / Agent',
    capabilities: ['cash_deposit', 'cash_withdrawal', 'balance_inquiry', 'wpm_purchase'],
    maxDeposit: 5000,
    maxWithdrawal: 2000,
    denominations: [1, 5, 10, 20, 50, 100]
  },
  kiosk: {
    name: 'Self-Service Kiosk',
    capabilities: ['cash_deposit', 'wpm_purchase', 'bill_payment', 'balance_inquiry'],
    maxDeposit: 10000,
    maxWithdrawal: 0,
    denominations: [1, 5, 10, 20, 50, 100]
  }
};

// ============================================================
// Fee structure
// ============================================================
const FEES = {
  cash_deposit: { rate: 0.005, min: 0.50, max: 25.00 },      // 0.5%
  cash_withdrawal: { rate: 0.01, min: 1.00, max: 50.00 },     // 1.0%
  wpm_purchase: { rate: 0.002, min: 0.25, max: 10.00 },       // 0.2% (buy WAMPUM with cash)
  check_deposit: { rate: 0.003, min: 0.50, max: 15.00 },      // 0.3%
  transfer: { rate: 0.001, min: 0.25, max: 5.00 },            // 0.1%
  bill_payment: { flat: 1.50 },
  balance_inquiry: { flat: 0 },
  card_issuance: { flat: 5.00 }
};

// VIP fee multipliers (lower = cheaper)
const VIP_FEE_MULT = {
  standard: 1.0,
  silver: 0.8,
  gold: 0.5,
  platinum: 0.25,
  black: 0.10,
  sovereign: 0  // free
};

function calculateFee(operation, amount, vipTier) {
  const schedule = FEES[operation];
  if (!schedule) return 0;
  if (schedule.flat !== undefined) return schedule.flat * (VIP_FEE_MULT[vipTier] || 1);
  const raw = Math.max(schedule.min, Math.min(schedule.max, amount * schedule.rate));
  return Math.round(raw * (VIP_FEE_MULT[vipTier] || 1) * 100) / 100;
}

function generateTxId(prefix) {
  return `${prefix}-${crypto.randomBytes(12).toString('hex').toUpperCase()}`;
}

// ============================================================
// GET /types — ATM types and capabilities
// ============================================================
router.get('/types', (_req, res) => {
  res.json({ status: 'ok', data: ATM_TYPES });
});

// ============================================================
// GET /fees — Fee schedule
// ============================================================
router.get('/fees', (_req, res) => {
  res.json({ status: 'ok', data: { fees: FEES, vipMultipliers: VIP_FEE_MULT } });
});

// ============================================================
// POST /register — Register a new ATM location
// ============================================================
router.post('/register',
  validate({
    body: {
      atm_type: t.string({ required: true, enum: Object.keys(ATM_TYPES) }),
      name: t.string({ required: true, max: 200 }),
      address: t.string({ required: true, max: 500 }),
      city: t.string({ required: true, max: 100 }),
      country: t.string({ required: true, max: 3 }),
      latitude: t.number({ min: -90, max: 90 }),
      longitude: t.number({ min: -180, max: 180 }),
      nation_id: t.string({ max: 10 }),
      territory: t.string({ max: 200 }),
      operating_hours: t.string({ max: 100 }),
      currencies_accepted: t.string({ max: 200 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { atm_type, name, address, city, country, latitude, longitude, nation_id, territory, operating_hours, currencies_accepted } = req.body;

    const atmId = generateTxId('ATM');
    const typeInfo = ATM_TYPES[atm_type];

    const result = await db.query(
      `INSERT INTO atm_locations
         (atm_id, atm_type, name, address, city, country, latitude, longitude,
          nation_id, territory, operating_hours, currencies_accepted,
          capabilities, max_deposit, max_withdrawal, denominations,
          owner_id, status, cash_level)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15, $16, $17, 'active', 100)
       RETURNING atm_id, atm_type, name, address, city, country, latitude, longitude, status, created_at`,
      [atmId, atm_type, name, address, city, country,
       latitude || null, longitude || null,
       nation_id || null, territory || null,
       operating_hours || '24/7',
       currencies_accepted || 'USD,PAB,WMP',
       JSON.stringify(typeInfo.capabilities),
       typeInfo.maxDeposit, typeInfo.maxWithdrawal,
       JSON.stringify(typeInfo.denominations),
       req.user.id]
    );

    audit.record({
      category: 'ATM',
      action: 'atm_registered',
      userId: req.user.id,
      risk: 'LOW',
      details: { atmId, type: atm_type, city, country }
    });

    log.info('ATM registered', { atmId, type: atm_type, city, country });

    res.status(201).json({ status: 'ok', data: result.rows[0] });
  })
);

// ============================================================
// GET /locations — Find ATMs (with geo search)
// ============================================================
router.get('/locations', asyncHandler(async (req, res) => {
  const { city, country, nation_id, atm_type, lat, lng, radius } = req.query;
  const limit = Math.min(200, Math.max(10, parseInt(req.query.limit, 10) || 50));
  const offset = parseInt(req.query.offset, 10) || 0;

  let query = `SELECT atm_id, atm_type, name, address, city, country, latitude, longitude,
     nation_id, territory, operating_hours, currencies_accepted, capabilities,
     max_deposit, max_withdrawal, denominations, status, cash_level, created_at`;

  // If coordinates provided, add distance calculation
  const params = [];
  let idx = 1;
  let conditions = [`status = 'active'`];

  if (lat && lng) {
    // Haversine distance in km
    query += `,
      (6371 * acos(
        cos(radians($${idx})) * cos(radians(latitude)) *
        cos(radians(longitude) - radians($${idx + 1})) +
        sin(radians($${idx})) * sin(radians(latitude))
      )) AS distance_km`;
    params.push(parseFloat(lat), parseFloat(lng));
    idx += 2;

    if (radius) {
      conditions.push(`(6371 * acos(
        cos(radians($${idx - 2})) * cos(radians(latitude)) *
        cos(radians(longitude) - radians($${idx - 1})) +
        sin(radians($${idx - 2})) * sin(radians(latitude))
      )) <= $${idx}`);
      params.push(parseFloat(radius));
      idx++;
    }
  }

  query += ` FROM atm_locations`;

  if (city) { conditions.push(`LOWER(city) = LOWER($${idx})`); params.push(city); idx++; }
  if (country) { conditions.push(`country = $${idx}`); params.push(country.toUpperCase()); idx++; }
  if (nation_id) { conditions.push(`nation_id = $${idx}`); params.push(nation_id); idx++; }
  if (atm_type) { conditions.push(`atm_type = $${idx}`); params.push(atm_type); idx++; }

  query += ` WHERE ${conditions.join(' AND ')}`;

  // Order by distance if geo search, else by name
  if (lat && lng) {
    query += ` ORDER BY distance_km ASC`;
  } else {
    query += ` ORDER BY country, city, name`;
  }

  query += ` LIMIT $${idx} OFFSET $${idx + 1}`;
  params.push(limit, offset);

  const result = await db.query(query, params);

  // Count total
  const countResult = await db.query(
    `SELECT COUNT(*) AS total FROM atm_locations WHERE ${conditions.join(' AND ')}`,
    params.slice(0, idx - 1 - (limit ? 1 : 0))
  );

  res.json({
    status: 'ok',
    data: {
      atms: result.rows,
      total: parseInt(countResult.rows[0]?.total || result.rows.length),
      filters: { city, country, nation_id, atm_type, lat, lng, radius }
    }
  });
}));

// ============================================================
// GET /locations/:atmId — Single ATM details
// ============================================================
router.get('/locations/:atmId', asyncHandler(async (req, res) => {
  const result = await db.query(
    `SELECT * FROM atm_locations WHERE atm_id = $1`,
    [req.params.atmId]
  );
  if (result.rows.length === 0) throw new AppError('NOT_FOUND', 'ATM not found');

  // Get recent transactions for this ATM
  let recentTx = [];
  try {
    recentTx = (await db.query(
      `SELECT tx_id, operation, amount, currency, fee, status, created_at
       FROM atm_transactions WHERE atm_id = $1
       ORDER BY created_at DESC LIMIT 20`,
      [req.params.atmId]
    )).rows;
  } catch { /* ok */ }

  // Get daily stats
  let dailyStats = null;
  try {
    dailyStats = (await db.query(
      `SELECT
         COUNT(*) AS total_tx,
         SUM(CASE WHEN operation = 'cash_deposit' THEN amount ELSE 0 END) AS total_deposits,
         SUM(CASE WHEN operation = 'cash_withdrawal' THEN amount ELSE 0 END) AS total_withdrawals,
         SUM(fee) AS total_fees
       FROM atm_transactions
       WHERE atm_id = $1 AND created_at > NOW() - INTERVAL '24 hours' AND status = 'completed'`,
      [req.params.atmId]
    )).rows[0];
  } catch { /* ok */ }

  res.json({
    status: 'ok',
    data: {
      atm: result.rows[0],
      recentTransactions: recentTx,
      dailyStats,
      typeInfo: ATM_TYPES[result.rows[0].atm_type]
    }
  });
}));

// ============================================================
// POST /deposit — Cash deposit at ATM
// ============================================================
router.post('/deposit',
  validate({
    body: {
      atm_id: t.string({ required: true }),
      account_number: t.string({ required: true }),
      amount: t.number({ required: true, min: 1 }),
      currency: t.string({ required: true }),
      denominations: t.object({}),
      convert_to_wpm: t.string({ enum: ['yes', 'no'] })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { atm_id, account_number, amount, currency, denominations } = req.body;
    const convertToWpm = req.body.convert_to_wpm === 'yes';

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Verify ATM exists and supports deposits
      const atmResult = await client.query(
        `SELECT * FROM atm_locations WHERE atm_id = $1 AND status = 'active'`,
        [atm_id]
      );
      if (atmResult.rows.length === 0) throw new AppError('NOT_FOUND', 'ATM not found or offline');

      const atm = atmResult.rows[0];
      const capabilities = typeof atm.capabilities === 'string' ? JSON.parse(atm.capabilities) : atm.capabilities;

      if (!capabilities.includes('cash_deposit')) {
        throw new AppError('INVALID_INPUT', 'This ATM does not accept cash deposits');
      }

      if (amount > parseFloat(atm.max_deposit)) {
        throw new AppError('INVALID_INPUT', `Maximum deposit at this ATM is ${atm.max_deposit} ${currency}`);
      }

      // Verify account
      const accResult = await client.query(
        `SELECT * FROM bank_accounts WHERE account_number = $1 AND user_id = $2 AND status = 'active' FOR UPDATE`,
        [account_number, req.user.id]
      );
      if (accResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Bank account not found');
      const account = accResult.rows[0];

      // Calculate fee
      const operation = convertToWpm ? 'wpm_purchase' : 'cash_deposit';
      const fee = calculateFee(operation, amount, account.vip_tier);
      const netAmount = amount - fee;

      // Exchange rate if converting to WMP
      let wmpAmount = netAmount;
      let exchangeRate = 1;
      if (convertToWpm && currency !== 'WMP') {
        // Simplified rates — production would query oracle
        const rates = { USD: 1, PAB: 1, EUR: 1.09, GBP: 1.27, MXN: 0.058, COP: 0.00024, BRL: 0.20 };
        exchangeRate = rates[currency] || 1;
        wmpAmount = netAmount * exchangeRate;
      }

      const txId = generateTxId('DEP');
      const txHash = crypto.createHash('sha256')
        .update(`atm-deposit:${atm_id}:${account_number}:${amount}:${Date.now()}`)
        .digest('hex');

      // Credit account
      const creditCurrency = convertToWpm ? 'WMP' : currency;
      const creditAmount = convertToWpm ? wmpAmount : netAmount;

      await client.query(
        `UPDATE bank_accounts SET balance = balance + $1, updated_at = NOW() WHERE account_number = $2`,
        [creditAmount, account_number]
      );

      // Record ATM transaction
      await client.query(
        `INSERT INTO atm_transactions
           (tx_id, atm_id, user_id, account_number, operation, amount, currency,
            fee, net_amount, converted_amount, converted_currency, exchange_rate,
            denominations, tx_hash, status)
         VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13::jsonb, $14, 'completed')`,
        [txId, atm_id, req.user.id, account_number, operation, amount, currency,
         fee, netAmount,
         convertToWpm ? wmpAmount : null,
         convertToWpm ? 'WMP' : null,
         convertToWpm ? exchangeRate : null,
         JSON.stringify(denominations || {}),
         txHash]
      );

      // Ledger transaction
      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
         VALUES ($1, $2, $3, $4, 'atm_deposit', $5, $6, 'completed', $7::jsonb, NOW())`,
        ['system-atm', req.user.id, creditAmount, creditCurrency,
         `Cash deposit at ATM ${atm.name}${convertToWpm ? ' (→ WPM)' : ''}`,
         txHash,
         JSON.stringify({ txId, atmId: atm_id, atmName: atm.name, originalCurrency: currency, originalAmount: amount, fee, convertedToWpm: convertToWpm })]
      );

      // Fee to treasury
      if (fee > 0) {
        const feeHash = crypto.createHash('sha256')
          .update(`atm-fee:${txId}:${fee}:${Date.now()}`)
          .digest('hex');
        await client.query(
          `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at)
           VALUES ($1, $2, $3, $4, 'atm_fee', $5, $6, 'completed', NOW())`,
          [req.user.id, PLATFORM_TREASURY_ID, fee, currency,
           `ATM deposit fee (${operation})`, feeHash]
        );
      }

      // Update ATM cash level
      await client.query(
        `UPDATE atm_locations SET cash_level = LEAST(100, cash_level + $1), last_transaction_at = NOW() WHERE atm_id = $2`,
        [Math.min(10, Math.round(amount / 100)), atm_id]
      );

      await client.query('COMMIT');

      // Generate receipt
      const receipt = {
        receiptNumber: txId,
        timestamp: new Date().toISOString(),
        atm: { id: atm.atm_id, name: atm.name, address: atm.address, city: atm.city },
        operation: operation === 'wpm_purchase' ? 'COMPRA WAMPUM (CASH → WPM)' : 'DEPOSITO EN EFECTIVO',
        cashInserted: { amount, currency, denominations: denominations || {} },
        fee: { amount: fee, currency, rate: FEES[operation].rate ? `${FEES[operation].rate * 100}%` : `$${FEES[operation].flat}` },
        credited: { amount: creditAmount, currency: creditCurrency, account: account_number },
        exchangeRate: convertToWpm && currency !== 'WMP' ? { from: currency, to: 'WMP', rate: exchangeRate } : null,
        txHash,
        blockchain: { network: 'MameyNode', chainId: 574, bank: 'BDET Bank' },
        newBalance: parseFloat(account.balance) + creditAmount,
        message: '¡Gracias por usar la Red ATM Soberana!'
      };

      audit.transaction(req, { txId, atmId: atm_id, operation, amount, fee, creditAmount, currency: creditCurrency });
      log.info('ATM deposit', { txId, atmId: atm_id, amount, currency, fee, creditAmount, creditCurrency });

      res.status(201).json({ status: 'ok', data: { transaction: { txId, txHash, status: 'completed' }, receipt } });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      log.error('ATM deposit failed', { err });
      throw new AppError('TRANSACTION_FAILED', 'Deposit could not be completed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// POST /withdraw — Cash withdrawal at ATM
// ============================================================
router.post('/withdraw',
  validate({
    body: {
      atm_id: t.string({ required: true }),
      account_number: t.string({ required: true }),
      amount: t.number({ required: true, min: 1 }),
      currency: t.string({ required: true }),
      source_currency: t.string({})
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { atm_id, account_number, amount, currency } = req.body;
    const sourceCurrency = req.body.source_currency || currency;

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Verify ATM
      const atmResult = await client.query(
        `SELECT * FROM atm_locations WHERE atm_id = $1 AND status = 'active'`,
        [atm_id]
      );
      if (atmResult.rows.length === 0) throw new AppError('NOT_FOUND', 'ATM not found or offline');

      const atm = atmResult.rows[0];
      const capabilities = typeof atm.capabilities === 'string' ? JSON.parse(atm.capabilities) : atm.capabilities;

      if (!capabilities.includes('cash_withdrawal')) {
        throw new AppError('INVALID_INPUT', 'This ATM does not support cash withdrawals');
      }

      if (amount > parseFloat(atm.max_withdrawal)) {
        throw new AppError('INVALID_INPUT', `Maximum withdrawal at this ATM is ${atm.max_withdrawal} ${currency}`);
      }

      if (parseInt(atm.cash_level) < 10) {
        throw new AppError('ATM_LOW_CASH', 'This ATM has insufficient cash. Please try another ATM.');
      }

      // Verify account
      const accResult = await client.query(
        `SELECT * FROM bank_accounts WHERE account_number = $1 AND user_id = $2 AND status = 'active' FOR UPDATE`,
        [account_number, req.user.id]
      );
      if (accResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Bank account not found');
      const account = accResult.rows[0];

      // Calculate fee
      const fee = calculateFee('cash_withdrawal', amount, account.vip_tier);

      // Convert from WMP if needed
      let debitAmount = amount + fee;
      let exchangeRate = 1;
      if (sourceCurrency === 'WMP' && currency !== 'WMP') {
        const rates = { USD: 1, PAB: 1, EUR: 0.92, GBP: 0.79, MXN: 17.15 };
        exchangeRate = 1 / (rates[currency] || 1);
        debitAmount = (amount + fee) * exchangeRate;
      }

      // Check balance
      if (parseFloat(account.balance) < debitAmount) {
        throw new AppError('INSUFFICIENT_FUNDS', `Insufficient balance. Available: ${account.balance} ${account.currency}, Required: ${debitAmount.toFixed(2)}`);
      }

      const txId = generateTxId('WDR');
      const txHash = crypto.createHash('sha256')
        .update(`atm-withdraw:${atm_id}:${account_number}:${amount}:${Date.now()}`)
        .digest('hex');

      // Debit account
      await client.query(
        `UPDATE bank_accounts SET balance = balance - $1, updated_at = NOW() WHERE account_number = $2`,
        [debitAmount, account_number]
      );

      // Record ATM transaction
      await client.query(
        `INSERT INTO atm_transactions
           (tx_id, atm_id, user_id, account_number, operation, amount, currency,
            fee, net_amount, converted_amount, converted_currency, exchange_rate,
            tx_hash, status)
         VALUES ($1, $2, $3, $4, 'cash_withdrawal', $5, $6, $7, $8, $9, $10, $11, $12, 'completed')`,
        [txId, atm_id, req.user.id, account_number, amount, currency,
         fee, amount,
         sourceCurrency !== currency ? debitAmount : null,
         sourceCurrency !== currency ? sourceCurrency : null,
         sourceCurrency !== currency ? exchangeRate : null,
         txHash]
      );

      // Ledger transaction
      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
         VALUES ($1, $2, $3, $4, 'atm_withdrawal', $5, $6, 'completed', $7::jsonb, NOW())`,
        [req.user.id, 'system-atm', debitAmount, account.currency,
         `Cash withdrawal at ATM ${atm.name}`,
         txHash,
         JSON.stringify({ txId, atmId: atm_id, cashAmount: amount, cashCurrency: currency, fee })]
      );

      // Update ATM cash level
      await client.query(
        `UPDATE atm_locations SET cash_level = GREATEST(0, cash_level - $1), last_transaction_at = NOW() WHERE atm_id = $2`,
        [Math.min(15, Math.round(amount / 50)), atm_id]
      );

      await client.query('COMMIT');

      const receipt = {
        receiptNumber: txId,
        timestamp: new Date().toISOString(),
        atm: { id: atm.atm_id, name: atm.name, address: atm.address, city: atm.city },
        operation: 'RETIRO EN EFECTIVO',
        cashDispensed: { amount, currency },
        fee: { amount: fee, currency },
        debited: { amount: debitAmount, currency: account.currency, account: account_number },
        exchangeRate: sourceCurrency !== currency ? { from: account.currency, to: currency, rate: exchangeRate } : null,
        txHash,
        blockchain: { network: 'MameyNode', chainId: 574, bank: 'BDET Bank' },
        newBalance: parseFloat(account.balance) - debitAmount,
        message: '¡Retiro exitoso! Red ATM Soberana'
      };

      audit.transaction(req, { txId, atmId: atm_id, operation: 'cash_withdrawal', amount, fee, currency });
      log.info('ATM withdrawal', { txId, atmId: atm_id, amount, currency, fee });

      res.status(201).json({ status: 'ok', data: { transaction: { txId, txHash, status: 'completed' }, receipt } });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      log.error('ATM withdrawal failed', { err });
      throw new AppError('TRANSACTION_FAILED', 'Withdrawal could not be completed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// POST /buy-wpm — Buy WAMPUM with cash at ATM
// ============================================================
router.post('/buy-wpm',
  validate({
    body: {
      atm_id: t.string({ required: true }),
      cash_amount: t.number({ required: true, min: 1 }),
      cash_currency: t.string({ required: true }),
      account_number: t.string({})
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { atm_id, cash_amount, cash_currency, account_number } = req.body;

    // This is essentially a deposit + conversion
    // Delegate to deposit with convert_to_wpm flag
    const depositReq = {
      ...req,
      body: {
        atm_id,
        account_number: account_number || null,
        amount: cash_amount,
        currency: cash_currency,
        convert_to_wpm: 'yes'
      }
    };

    // If no account specified, find or create a WMP account
    if (!account_number) {
      let wmpAcc = await db.query(
        `SELECT account_number FROM bank_accounts WHERE user_id = $1 AND currency = 'WMP' AND status = 'active' LIMIT 1`,
        [req.user.id]
      );

      if (wmpAcc.rows.length === 0) {
        // Auto-create WMP account
        const newAccNum = `IKWP${crypto.randomBytes(8).toString('hex').toUpperCase().slice(0, 12)}`;
        await db.query(
          `INSERT INTO bank_accounts (user_id, account_number, iban, account_type, currency, display_name, balance, status, vip_tier)
           VALUES ($1, $2, $3, 'checking', 'WMP', 'WAMPUM Account', 0, 'active', 'standard')`,
          [req.user.id, newAccNum, `IK00BDET${newAccNum}`]
        );
        depositReq.body.account_number = newAccNum;
      } else {
        depositReq.body.account_number = wmpAcc.rows[0].account_number;
      }
    }

    // Exchange rate
    const rates = { USD: 1, PAB: 1, EUR: 1.09, GBP: 1.27, MXN: 0.058, COP: 0.00024, BRL: 0.20 };
    const rate = rates[cash_currency] || 1;
    const fee = calculateFee('wpm_purchase', cash_amount, 'standard');
    const wpmAmount = (cash_amount - fee) * rate;

    res.json({
      status: 'ok',
      data: {
        preview: {
          cashInserted: { amount: cash_amount, currency: cash_currency },
          fee: { amount: fee, currency: cash_currency },
          wampumReceived: { amount: parseFloat(wpmAmount.toFixed(8)), currency: 'WMP' },
          exchangeRate: { from: cash_currency, to: 'WMP', rate },
          account: depositReq.body.account_number,
          instructions: 'Use POST /v1/atm/deposit with convert_to_wpm=yes to complete'
        },
        blockchain: { network: 'MameyNode', chainId: 574, bank: 'BDET Bank', nativeCurrency: 'WAMPUM (WMP)' }
      }
    });
  })
);

// ============================================================
// GET /transactions — ATM transaction history
// ============================================================
router.get('/transactions', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const { atm_id, operation, from, to } = req.query;
  const limit = Math.min(200, Math.max(10, parseInt(req.query.limit, 10) || 50));
  const offset = parseInt(req.query.offset, 10) || 0;

  let query = `SELECT t.tx_id, t.atm_id, t.operation, t.amount, t.currency, t.fee,
     t.net_amount, t.converted_amount, t.converted_currency, t.exchange_rate,
     t.tx_hash, t.status, t.created_at,
     a.name AS atm_name, a.city AS atm_city, a.country AS atm_country
     FROM atm_transactions t
     LEFT JOIN atm_locations a ON a.atm_id = t.atm_id
     WHERE t.user_id = $1`;
  const params = [req.user.id];
  let idx = 2;

  if (atm_id) { query += ` AND t.atm_id = $${idx}`; params.push(atm_id); idx++; }
  if (operation) { query += ` AND t.operation = $${idx}`; params.push(operation); idx++; }
  if (from) { query += ` AND t.created_at >= $${idx}`; params.push(from); idx++; }
  if (to) { query += ` AND t.created_at <= $${idx}`; params.push(to); idx++; }

  query += ` ORDER BY t.created_at DESC LIMIT $${idx} OFFSET $${idx + 1}`;
  params.push(limit, offset);

  const result = await db.query(query, params);
  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// GET /network — Sovereign ATM network overview
// ============================================================
router.get('/network', asyncHandler(async (_req, res) => {
  let stats = { totalAtms: 0, byType: [], byCountry: [], byNation: [] };

  try {
    const [totalResult, typeResult, countryResult, nationResult] = await Promise.all([
      db.query(`SELECT COUNT(*) AS total, SUM(CASE WHEN status = 'active' THEN 1 ELSE 0 END) AS active FROM atm_locations`),
      db.query(`SELECT atm_type, COUNT(*) AS count FROM atm_locations WHERE status = 'active' GROUP BY atm_type ORDER BY count DESC`),
      db.query(`SELECT country, COUNT(*) AS count FROM atm_locations WHERE status = 'active' GROUP BY country ORDER BY count DESC`),
      db.query(`SELECT nation_id, COUNT(*) AS count FROM atm_locations WHERE status = 'active' AND nation_id IS NOT NULL GROUP BY nation_id ORDER BY count DESC LIMIT 50`)
    ]);

    stats = {
      totalAtms: parseInt(totalResult.rows[0]?.total || 0),
      activeAtms: parseInt(totalResult.rows[0]?.active || 0),
      byType: typeResult.rows,
      byCountry: countryResult.rows,
      byNation: nationResult.rows
    };
  } catch { /* tables may not exist yet */ }

  res.json({
    status: 'ok',
    data: {
      network: 'BDET Bank Sovereign ATM Network',
      stats,
      atmTypes: ATM_TYPES,
      fees: FEES,
      supportedCurrencies: ['USD', 'PAB', 'EUR', 'GBP', 'MXN', 'COP', 'BRL', 'ARS', 'WMP'],
      features: [
        'Cash deposits in local currency',
        'Buy WAMPUM (WMP) with cash',
        'Cash withdrawals from WMP balance',
        'Cross-currency conversion at ATM',
        'Bill payments',
        'Balance inquiries',
        'VIP fee discounts (up to 100% off)',
        'Nation-specific ATMs for 574 tribes',
        'Real-time blockchain settlement on MameyNode'
      ],
      blockchain: { network: 'MameyNode', chainId: 574, bank: 'BDET Bank', nativeCurrency: 'WAMPUM (WMP)' }
    }
  });
}));

// ============================================================
// PATCH /locations/:atmId/status — Update ATM status
// ============================================================
router.patch('/locations/:atmId/status',
  validate({
    body: {
      status: t.string({ required: true, enum: ['active', 'maintenance', 'offline', 'decommissioned'] }),
      cash_level: t.number({ min: 0, max: 100 }),
      note: t.string({ max: 500 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { status, cash_level, note } = req.body;

    const result = await db.query(
      `UPDATE atm_locations SET status = $1, cash_level = COALESCE($2, cash_level), updated_at = NOW()
       WHERE atm_id = $3 AND owner_id = $4
       RETURNING atm_id, status, cash_level`,
      [status, cash_level || null, req.params.atmId, req.user.id]
    );

    if (result.rows.length === 0) throw new AppError('NOT_FOUND', 'ATM not found or not owned by you');

    audit.record({
      category: 'ATM',
      action: 'atm_status_update',
      userId: req.user.id,
      risk: 'LOW',
      details: { atmId: req.params.atmId, status, note }
    });

    res.json({ status: 'ok', data: result.rows[0] });
  })
);

// ============================================================
// Exports
// ============================================================
module.exports = router;
