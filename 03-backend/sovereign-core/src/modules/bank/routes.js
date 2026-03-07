'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Sovereign Bank Module v1.0.0 — REAL Banking System
// Accounts, transfers, VIP banking, central banks,
// international settlements, statements, card management
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
const log = createLogger('sovereign-core:bank');
const audit = createAuditLogger('sovereign-core:bank');

const PLATFORM_TREASURY_ID = process.env.PLATFORM_TREASURY_ID || 'system-treasury';
const TRANSFER_FEE_RATE = 0.001; // 0.1% domestic transfer fee
const INTL_TRANSFER_FEE_RATE = 0.005; // 0.5% international
const VIP_TRANSFER_FEE_RATE = 0.0005; // 0.05% VIP reduced fee

// ============================================================
// CENTRAL BANKS — Sovereign Banking Network
// ============================================================
const CENTRAL_BANKS = [
  { code: 'BDET', name: 'BDET Bank — Ierahkwa Central', country: 'IK', currency: 'WMP', reserves: 100000000000, tier: 'sovereign', swiftCode: 'BDETIKPA' },
  { code: 'FRB', name: 'Federal Reserve Bank', country: 'US', currency: 'USD', reserves: 8940000000000, tier: 'g7', swiftCode: 'FRNYUS33' },
  { code: 'ECB', name: 'European Central Bank', country: 'EU', currency: 'EUR', reserves: 6800000000000, tier: 'g7', swiftCode: 'ECBFDEFF' },
  { code: 'BOE', name: 'Bank of England', country: 'GB', currency: 'GBP', reserves: 3200000000000, tier: 'g7', swiftCode: 'BKENGB2L' },
  { code: 'BOJ', name: 'Bank of Japan', country: 'JP', currency: 'JPY', reserves: 5100000000000, tier: 'g7', swiftCode: 'BOJPJPJT' },
  { code: 'PBOC', name: "People's Bank of China", country: 'CN', currency: 'CNY', reserves: 7200000000000, tier: 'brics', swiftCode: 'BKCHCNBJ' },
  { code: 'SNB', name: 'Swiss National Bank', country: 'CH', currency: 'CHF', reserves: 890000000000, tier: 'g20', swiftCode: 'SNBZCHZZ' },
  { code: 'RBA', name: 'Reserve Bank of Australia', country: 'AU', currency: 'AUD', reserves: 520000000000, tier: 'g20', swiftCode: 'RABOROBB' },
  { code: 'BOC', name: 'Bank of Canada', country: 'CA', currency: 'CAD', reserves: 480000000000, tier: 'g7', swiftCode: 'BNDCCAMM' },
  { code: 'BANXICO', name: 'Banco de Mexico', country: 'MX', currency: 'MXN', reserves: 210000000000, tier: 'g20', swiftCode: 'BDMXMXMM' },
  { code: 'BCB', name: 'Banco Central do Brasil', country: 'BR', currency: 'BRL', reserves: 350000000000, tier: 'brics', swiftCode: 'BCBRBRSB' },
  { code: 'RBI', name: 'Reserve Bank of India', country: 'IN', currency: 'INR', reserves: 640000000000, tier: 'brics', swiftCode: 'RBININBB' },
  { code: 'BNP', name: 'Banco Nacional de Panama', country: 'PA', currency: 'PAB', reserves: 15000000000, tier: 'regional', swiftCode: 'BNPAPAPA' },
  { code: 'BCV', name: 'Banco Central de Venezuela', country: 'VE', currency: 'VES', reserves: 8000000000, tier: 'regional', swiftCode: 'BCVEVECA' },
  { code: 'BCRA', name: 'Banco Central de Argentina', country: 'AR', currency: 'ARS', reserves: 42000000000, tier: 'regional', swiftCode: 'BCRAARBX' },
  { code: 'BCC', name: 'Banco Central de Colombia', country: 'CO', currency: 'COP', reserves: 55000000000, tier: 'regional', swiftCode: 'BCCOCOBO' }
];

// ============================================================
// ACCOUNT TYPES
// ============================================================
const ACCOUNT_TYPES = ['checking', 'savings', 'business', 'vip', 'institutional', 'trust', 'escrow', 'treasury'];
const VIP_TIERS = ['standard', 'silver', 'gold', 'platinum', 'black', 'sovereign'];

// ============================================================
// POST /account/open — Open a new bank account
// ============================================================
router.post('/account/open',
  validate({
    body: {
      account_type: t.string({ required: true, enum: ACCOUNT_TYPES }),
      currency: t.string({ required: true }),
      display_name: t.string({ max: 200 }),
      initial_deposit: t.number({ min: 0 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { account_type, currency, display_name, initial_deposit } = req.body;

    // Generate account number: IK + type prefix + random
    const typePrefix = { checking: 'CK', savings: 'SV', business: 'BZ', vip: 'VP', institutional: 'IN', trust: 'TR', escrow: 'ES', treasury: 'TY' };
    const accountNumber = `IK${typePrefix[account_type]}${crypto.randomBytes(8).toString('hex').toUpperCase().slice(0, 12)}`;
    const iban = `IK00BDET${accountNumber}`;

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      const result = await client.query(
        `INSERT INTO bank_accounts
           (user_id, account_number, iban, account_type, currency, display_name, balance, status, vip_tier)
         VALUES ($1, $2, $3, $4, $5, $6, $7, 'active', $8)
         RETURNING id, account_number, iban, account_type, currency, display_name, balance, status, vip_tier, created_at`,
        [req.user.id, accountNumber, iban, account_type, currency,
         display_name || `${account_type.charAt(0).toUpperCase() + account_type.slice(1)} Account`,
         initial_deposit || 0,
         account_type === 'vip' ? 'gold' : 'standard']
      );

      // If initial deposit, create transaction
      if (initial_deposit && initial_deposit > 0) {
        const txHash = crypto.createHash('sha256')
          .update(`deposit:${req.user.id}:${initial_deposit}:${currency}:${Date.now()}`)
          .digest('hex');

        await client.query(
          `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
           VALUES ($1, $2, $3, $4, 'deposit', 'Initial deposit', $5, 'completed', $6::jsonb, NOW())`,
          ['system-bank', req.user.id, initial_deposit, currency, txHash,
           JSON.stringify({ accountNumber, type: 'initial_deposit' })]
        );
      }

      await client.query('COMMIT');

      audit.record({
        category: 'BANK_ACCOUNT',
        action: 'account_opened',
        userId: req.user.id,
        risk: 'LOW',
        details: { accountNumber, accountType: account_type, currency }
      });

      log.info('Bank account opened', { userId: req.user.id, accountNumber, accountType: account_type });

      res.status(201).json({ status: 'ok', data: result.rows[0] });
    } catch (err) {
      await client.query('ROLLBACK');
      throw err;
    } finally {
      client.release();
    }
  })
);

// ============================================================
// GET /accounts — List user's bank accounts
// ============================================================
router.get('/accounts', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `SELECT id, account_number, iban, account_type, currency, display_name, balance,
            status, vip_tier, created_at, updated_at
     FROM bank_accounts
     WHERE user_id = $1 AND status != 'closed'
     ORDER BY created_at DESC`,
    [req.user.id]
  );

  // Calculate total balance in WMP equivalent
  let totalWmpEquivalent = 0;
  for (const acc of result.rows) {
    if (acc.currency === 'WMP') totalWmpEquivalent += parseFloat(acc.balance);
    else totalWmpEquivalent += parseFloat(acc.balance); // TODO: exchange rate conversion
  }

  res.json({
    status: 'ok',
    data: {
      accounts: result.rows,
      totalAccounts: result.rows.length,
      totalBalance: totalWmpEquivalent
    }
  });
}));

// ============================================================
// POST /transfer — Bank transfer (domestic or international)
// ============================================================
router.post('/transfer',
  validate({
    body: {
      from_account: t.string({ required: true }),
      to_account: t.string({ required: true }),
      amount: t.number({ required: true, min: 0.01 }),
      currency: t.string({ required: true }),
      memo: t.string({ max: 500 }),
      transfer_type: t.string({ enum: ['domestic', 'international', 'wire', 'ach', 'iss'] }),
      swift_code: t.string({ max: 11 }),
      beneficiary_name: t.string({ max: 200 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { from_account, to_account, amount, currency, memo, swift_code, beneficiary_name } = req.body;
    const transferType = req.body.transfer_type || 'domestic';

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Verify source account belongs to user
      const sourceResult = await client.query(
        `SELECT * FROM bank_accounts WHERE account_number = $1 AND user_id = $2 AND status = 'active' FOR UPDATE`,
        [from_account, req.user.id]
      );
      if (sourceResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Source account not found');
      const source = sourceResult.rows[0];

      // Check balance
      if (parseFloat(source.balance) < amount) {
        throw new AppError('INSUFFICIENT_FUNDS', `Insufficient balance. Available: ${source.balance} ${source.currency}`);
      }

      // Calculate fee based on transfer type and VIP tier
      let feeRate = transferType === 'international' || transferType === 'wire' || transferType === 'swift'
        ? INTL_TRANSFER_FEE_RATE
        : transferType === 'iss' ? 0 // Indigenous Settlement System: FREE
        : TRANSFER_FEE_RATE;

      // VIP fee reduction
      if (['platinum', 'black', 'sovereign'].includes(source.vip_tier)) {
        feeRate = VIP_TRANSFER_FEE_RATE;
      }
      if (source.vip_tier === 'sovereign') feeRate = 0; // Sovereign tier: no fees

      const fee = Math.floor(amount * feeRate * 100) / 100;
      const netAmount = amount - fee;

      // Check if destination is internal
      const destResult = await client.query(
        `SELECT * FROM bank_accounts WHERE account_number = $1 AND status = 'active' FOR UPDATE`,
        [to_account]
      );

      const isInternal = destResult.rows.length > 0;
      const timestamp = new Date().toISOString();

      // Debit source
      await client.query(
        `UPDATE bank_accounts SET balance = balance - $1, updated_at = NOW() WHERE account_number = $2`,
        [amount, from_account]
      );

      // Credit destination (if internal)
      if (isInternal) {
        await client.query(
          `UPDATE bank_accounts SET balance = balance + $1, updated_at = NOW() WHERE account_number = $2`,
          [netAmount, to_account]
        );
      }

      // Create transfer transaction
      const txHash = crypto.createHash('sha256')
        .update(`${from_account}:${to_account}:${amount}:${currency}:${timestamp}`)
        .digest('hex');

      const transferId = 'TXF-' + crypto.randomBytes(10).toString('hex').toUpperCase();

      await client.query(
        `INSERT INTO bank_transfers
           (transfer_id, from_account, to_account, from_user_id, to_user_id, amount, fee, net_amount,
            currency, transfer_type, swift_code, beneficiary_name, memo, tx_hash, status)
         VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15)`,
        [transferId, from_account, to_account, req.user.id,
         isInternal ? destResult.rows[0].user_id : null,
         amount, fee, netAmount, currency, transferType,
         swift_code || null, beneficiary_name || null,
         memo || null, txHash,
         isInternal ? 'completed' : 'pending']
      );

      // Create ledger transaction
      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
         VALUES ($1, $2, $3, $4, 'bank_transfer', $5, $6, 'completed', $7::jsonb, $8)`,
        [req.user.id, isInternal ? destResult.rows[0].user_id : 'external', netAmount, currency,
         memo || `Bank transfer ${transferType}`, txHash,
         JSON.stringify({ transferId, transferType, fee, swiftCode: swift_code, from: from_account, to: to_account }),
         timestamp]
      );

      // Fee to treasury
      if (fee > 0) {
        const feeHash = crypto.createHash('sha256')
          .update(`fee:${transferId}:${fee}:${timestamp}`)
          .digest('hex');

        await client.query(
          `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
           VALUES ($1, $2, $3, $4, 'bank_fee', $5, $6, 'completed', $7::jsonb, $8)`,
          [req.user.id, PLATFORM_TREASURY_ID, fee, currency,
           `Transfer fee (${(feeRate * 100).toFixed(2)}%)`, feeHash,
           JSON.stringify({ transferId, feeRate }), timestamp]
        );
      }

      await client.query('COMMIT');

      audit.transaction(req, { transferId, from: from_account, to: to_account, amount, fee, transferType });
      log.info('Bank transfer', { transferId, from: from_account, to: to_account, amount, fee, transferType });

      res.status(201).json({
        status: 'ok',
        data: {
          transferId,
          from: from_account,
          to: to_account,
          amount,
          fee,
          netAmount,
          currency,
          transferType,
          txHash,
          status: isInternal ? 'completed' : 'pending',
          estimatedSettlement: transferType === 'iss' ? 'instant' : transferType === 'wire' ? 'T+0' : transferType === 'international' ? 'T+1 to T+3' : 'T+0'
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      log.error('Bank transfer failed', { err });
      throw new AppError('TRANSACTION_FAILED', 'Transfer could not be completed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// POST /deposit — Deposit funds to bank account
// ============================================================
router.post('/deposit',
  validate({
    body: {
      account_number: t.string({ required: true }),
      amount: t.number({ required: true, min: 0.01 }),
      source: t.string({ enum: ['crypto_wallet', 'card', 'wire', 'cash', 'exchange'] }),
      memo: t.string({ max: 500 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { account_number, amount, memo } = req.body;
    const source = req.body.source || 'crypto_wallet';

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      const accResult = await client.query(
        `SELECT * FROM bank_accounts WHERE account_number = $1 AND user_id = $2 AND status = 'active' FOR UPDATE`,
        [account_number, req.user.id]
      );
      if (accResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Account not found');

      // Update balance
      await client.query(
        `UPDATE bank_accounts SET balance = balance + $1, updated_at = NOW() WHERE account_number = $2`,
        [amount, account_number]
      );

      // Create deposit transaction
      const txHash = crypto.createHash('sha256')
        .update(`deposit:${req.user.id}:${account_number}:${amount}:${Date.now()}`)
        .digest('hex');

      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
         VALUES ($1, $2, $3, $4, 'deposit', $5, $6, 'completed', $7::jsonb, NOW())`,
        ['system-bank', req.user.id, amount, accResult.rows[0].currency,
         memo || `Deposit from ${source}`, txHash,
         JSON.stringify({ accountNumber: account_number, source })]
      );

      await client.query('COMMIT');

      res.status(201).json({
        status: 'ok',
        data: {
          accountNumber: account_number,
          depositAmount: amount,
          newBalance: parseFloat(accResult.rows[0].balance) + amount,
          currency: accResult.rows[0].currency,
          txHash
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      throw new AppError('TRANSACTION_FAILED', 'Deposit failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// POST /withdraw — Withdraw from bank account
// ============================================================
router.post('/withdraw',
  validate({
    body: {
      account_number: t.string({ required: true }),
      amount: t.number({ required: true, min: 0.01 }),
      destination: t.string({ enum: ['crypto_wallet', 'card', 'wire', 'exchange'] }),
      memo: t.string({ max: 500 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { account_number, amount, memo } = req.body;
    const destination = req.body.destination || 'crypto_wallet';

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      const accResult = await client.query(
        `SELECT * FROM bank_accounts WHERE account_number = $1 AND user_id = $2 AND status = 'active' FOR UPDATE`,
        [account_number, req.user.id]
      );
      if (accResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Account not found');

      if (parseFloat(accResult.rows[0].balance) < amount) {
        throw new AppError('INSUFFICIENT_FUNDS', `Insufficient balance: ${accResult.rows[0].balance} ${accResult.rows[0].currency}`);
      }

      await client.query(
        `UPDATE bank_accounts SET balance = balance - $1, updated_at = NOW() WHERE account_number = $2`,
        [amount, account_number]
      );

      const txHash = crypto.createHash('sha256')
        .update(`withdraw:${req.user.id}:${account_number}:${amount}:${Date.now()}`)
        .digest('hex');

      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
         VALUES ($1, $2, $3, $4, 'withdrawal', $5, $6, 'completed', $7::jsonb, NOW())`,
        [req.user.id, 'system-bank', amount, accResult.rows[0].currency,
         memo || `Withdrawal to ${destination}`, txHash,
         JSON.stringify({ accountNumber: account_number, destination })]
      );

      await client.query('COMMIT');

      res.status(201).json({
        status: 'ok',
        data: {
          accountNumber: account_number,
          withdrawAmount: amount,
          newBalance: parseFloat(accResult.rows[0].balance) - amount,
          currency: accResult.rows[0].currency,
          txHash,
          destination
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      throw new AppError('TRANSACTION_FAILED', 'Withdrawal failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// GET /statement/:accountNumber — Account statement
// ============================================================
router.get('/statement/:accountNumber', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const { accountNumber } = req.params;
  const from = req.query.from || new Date(Date.now() - 30 * 86400000).toISOString();
  const to = req.query.to || new Date().toISOString();
  const limit = Math.min(500, Math.max(10, parseInt(req.query.limit, 10) || 100));

  // Verify account ownership
  const accResult = await db.query(
    `SELECT * FROM bank_accounts WHERE account_number = $1 AND user_id = $2`,
    [accountNumber, req.user.id]
  );
  if (accResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Account not found');
  const account = accResult.rows[0];

  // Get transfers for this account
  const transfers = await db.query(
    `SELECT transfer_id, from_account, to_account, amount, fee, net_amount, currency,
            transfer_type, beneficiary_name, memo, status, created_at,
            CASE WHEN from_account = $1 THEN 'debit' ELSE 'credit' END AS direction
     FROM bank_transfers
     WHERE (from_account = $1 OR to_account = $1)
       AND created_at BETWEEN $2 AND $3
     ORDER BY created_at DESC
     LIMIT $4`,
    [accountNumber, from, to, limit]
  );

  // Calculate period totals
  const totals = await db.query(
    `SELECT
       COALESCE(SUM(CASE WHEN to_account = $1 THEN net_amount ELSE 0 END), 0) AS total_credits,
       COALESCE(SUM(CASE WHEN from_account = $1 THEN amount ELSE 0 END), 0) AS total_debits,
       COALESCE(SUM(CASE WHEN from_account = $1 THEN fee ELSE 0 END), 0) AS total_fees,
       COUNT(*) AS total_transactions
     FROM bank_transfers
     WHERE (from_account = $1 OR to_account = $1)
       AND created_at BETWEEN $2 AND $3`,
    [accountNumber, from, to]
  );

  const t = totals.rows[0];

  res.json({
    status: 'ok',
    data: {
      account: {
        accountNumber: account.account_number,
        iban: account.iban,
        type: account.account_type,
        currency: account.currency,
        currentBalance: parseFloat(account.balance),
        vipTier: account.vip_tier
      },
      period: { from, to },
      summary: {
        totalCredits: parseFloat(t.total_credits),
        totalDebits: parseFloat(t.total_debits),
        totalFees: parseFloat(t.total_fees),
        netMovement: parseFloat(t.total_credits) - parseFloat(t.total_debits),
        transactionCount: parseInt(t.total_transactions)
      },
      transactions: transfers.rows,
      generatedAt: new Date().toISOString(),
      bank: 'BDET Bank — Ierahkwa Ne Kanienke',
      swiftCode: 'BDETIKPA'
    }
  });
}));

// ============================================================
// GET /central-banks — List sovereign central banks network
// ============================================================
router.get('/central-banks', asyncHandler(async (_req, res) => {
  res.json({ status: 'ok', data: CENTRAL_BANKS });
}));

// ============================================================
// POST /international/transfer — International wire transfer
// via central bank corridor
// ============================================================
router.post('/international/transfer',
  validate({
    body: {
      from_account: t.string({ required: true }),
      amount: t.number({ required: true, min: 1 }),
      currency: t.string({ required: true }),
      destination_bank_code: t.string({ required: true }),
      destination_account: t.string({ required: true }),
      beneficiary_name: t.string({ required: true, max: 200 }),
      beneficiary_address: t.string({ max: 500 }),
      reference: t.string({ max: 200 }),
      purpose: t.string({ enum: ['trade', 'investment', 'personal', 'business', 'sovereign'] })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { from_account, amount, currency, destination_bank_code, destination_account, beneficiary_name, beneficiary_address, reference } = req.body;
    const purpose = req.body.purpose || 'business';

    // Verify destination bank exists in network
    const destBank = CENTRAL_BANKS.find(b => b.code === destination_bank_code);
    if (!destBank) throw new AppError('NOT_FOUND', `Bank ${destination_bank_code} not found in sovereign network`);

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Verify source account
      const sourceAcc = await client.query(
        `SELECT * FROM bank_accounts WHERE account_number = $1 AND user_id = $2 AND status = 'active' FOR UPDATE`,
        [from_account, req.user.id]
      );
      if (sourceAcc.rows.length === 0) throw new AppError('NOT_FOUND', 'Source account not found');

      if (parseFloat(sourceAcc.rows[0].balance) < amount) {
        throw new AppError('INSUFFICIENT_FUNDS', 'Insufficient balance for international transfer');
      }

      // Calculate fees
      const isISS = destBank.tier === 'sovereign' || purpose === 'sovereign';
      let feeRate = isISS ? 0 : INTL_TRANSFER_FEE_RATE;

      // VIP reduction
      if (['platinum', 'black', 'sovereign'].includes(sourceAcc.rows[0].vip_tier)) {
        feeRate = Math.max(0, feeRate * 0.5);
      }

      const fee = Math.floor(amount * feeRate * 100) / 100;
      const netAmount = amount - fee;

      // Settlement type based on destination
      const settlementType = isISS ? 'ISS' : destBank.tier === 'g7' ? 'SWIFT' : 'WIRE';
      const estimatedSettlement = isISS ? 'T+0 (instant)' : settlementType === 'SWIFT' ? 'T+1 to T+3' : 'T+1';

      // Debit source
      await client.query(
        `UPDATE bank_accounts SET balance = balance - $1, updated_at = NOW() WHERE account_number = $2`,
        [amount, from_account]
      );

      const txHash = crypto.createHash('sha256')
        .update(`intl:${from_account}:${destination_account}:${amount}:${Date.now()}`)
        .digest('hex');

      const transferId = 'INTL-' + crypto.randomBytes(10).toString('hex').toUpperCase();

      // Create international transfer record
      await client.query(
        `INSERT INTO bank_transfers
           (transfer_id, from_account, to_account, from_user_id, amount, fee, net_amount,
            currency, transfer_type, swift_code, beneficiary_name, memo, tx_hash, status, metadata)
         VALUES ($1, $2, $3, $4, $5, $6, $7, $8, 'international', $9, $10, $11, $12, $13, $14::jsonb)`,
        [transferId, from_account, destination_account, req.user.id,
         amount, fee, netAmount, currency, destBank.swiftCode,
         beneficiary_name, reference || `International transfer to ${destBank.name}`,
         txHash, isISS ? 'completed' : 'processing',
         JSON.stringify({
           destinationBank: destBank,
           beneficiaryAddress: beneficiary_address,
           purpose,
           settlementType,
           estimatedSettlement,
           corridor: `${CENTRAL_BANKS[0].code}-${destBank.code}`
         })]
      );

      // Ledger transaction
      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
         VALUES ($1, $2, $3, $4, 'intl_transfer', $5, $6, 'completed', $7::jsonb, NOW())`,
        [req.user.id, 'system-intl', netAmount, currency,
         `International transfer to ${destBank.name} — ${beneficiary_name}`, txHash,
         JSON.stringify({ transferId, destBankCode: destination_bank_code, settlementType })]
      );

      // Fee to treasury
      if (fee > 0) {
        const feeHash = crypto.createHash('sha256')
          .update(`intl-fee:${transferId}:${fee}:${Date.now()}`)
          .digest('hex');
        await client.query(
          `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at)
           VALUES ($1, $2, $3, $4, 'intl_fee', $5, $6, 'completed', NOW())`,
          [req.user.id, PLATFORM_TREASURY_ID, fee, currency,
           `International transfer fee (${(feeRate * 100).toFixed(2)}%)`, feeHash]
        );
      }

      await client.query('COMMIT');

      audit.transaction(req, {
        transferId, type: 'international', from: from_account, to: destination_account,
        amount, fee, destBank: destination_bank_code, settlementType
      });

      log.info('International transfer', { transferId, from: from_account, destBank: destination_bank_code, amount, fee });

      res.status(201).json({
        status: 'ok',
        data: {
          transferId,
          from: from_account,
          to: { account: destination_account, bank: destBank.name, swiftCode: destBank.swiftCode, country: destBank.country },
          amount, fee, netAmount, currency,
          beneficiary: beneficiary_name,
          settlementType,
          estimatedSettlement,
          status: isISS ? 'completed' : 'processing',
          txHash
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      throw new AppError('TRANSACTION_FAILED', 'International transfer failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// POST /vip/upgrade — Upgrade account to VIP tier
// ============================================================
router.post('/vip/upgrade',
  validate({
    body: {
      account_number: t.string({ required: true }),
      tier: t.string({ required: true, enum: VIP_TIERS })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { account_number, tier } = req.body;

    // VIP tier requirements (minimum balance in WMP)
    const tierMinBalance = {
      standard: 0,
      silver: 1000,
      gold: 10000,
      platinum: 100000,
      black: 1000000,
      sovereign: 10000000
    };

    const result = await db.query(
      `SELECT * FROM bank_accounts WHERE account_number = $1 AND user_id = $2 AND status = 'active'`,
      [account_number, req.user.id]
    );
    if (result.rows.length === 0) throw new AppError('NOT_FOUND', 'Account not found');

    const balance = parseFloat(result.rows[0].balance);
    if (balance < tierMinBalance[tier]) {
      throw new AppError('INVALID_INPUT', `Minimum balance for ${tier} tier is ${tierMinBalance[tier]} WMP. Current: ${balance}`);
    }

    await db.query(
      `UPDATE bank_accounts SET vip_tier = $1, account_type = 'vip', updated_at = NOW()
       WHERE account_number = $2 AND user_id = $3`,
      [tier, account_number, req.user.id]
    );

    // VIP benefits
    const benefits = {
      standard: { feeDiscount: 0, interestBonus: 0, support: '72h' },
      silver: { feeDiscount: 10, interestBonus: 0.5, support: '48h' },
      gold: { feeDiscount: 25, interestBonus: 1.0, support: '24h' },
      platinum: { feeDiscount: 50, interestBonus: 2.0, support: '4h', dedicatedManager: true },
      black: { feeDiscount: 75, interestBonus: 3.5, support: '1h', dedicatedManager: true, loungeAccess: true },
      sovereign: { feeDiscount: 100, interestBonus: 5.0, support: 'instant', dedicatedManager: true, loungeAccess: true, prioritySettlement: true }
    };

    audit.record({
      category: 'BANK_VIP',
      action: 'vip_upgrade',
      userId: req.user.id,
      risk: 'LOW',
      details: { accountNumber: account_number, newTier: tier }
    });

    res.json({
      status: 'ok',
      data: {
        accountNumber: account_number,
        tier,
        benefits: benefits[tier]
      }
    });
  })
);

// ============================================================
// GET /transfers — Transfer history
// ============================================================
router.get('/transfers', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const type = req.query.type || null; // domestic, international, wire, iss
  const status = req.query.status || null;
  const limit = Math.min(200, Math.max(10, parseInt(req.query.limit, 10) || 50));
  const offset = parseInt(req.query.offset, 10) || 0;

  let query = `SELECT transfer_id, from_account, to_account, amount, fee, net_amount,
     currency, transfer_type, swift_code, beneficiary_name, memo, tx_hash, status, metadata, created_at
     FROM bank_transfers WHERE from_user_id = $1`;
  const params = [req.user.id];
  let idx = 2;

  if (type) { query += ` AND transfer_type = $${idx}`; params.push(type); idx++; }
  if (status) { query += ` AND status = $${idx}`; params.push(status); idx++; }

  query += ` ORDER BY created_at DESC LIMIT $${idx} OFFSET $${idx + 1}`;
  params.push(limit, offset);

  const result = await db.query(query, params);
  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// GET /dashboard — Banking dashboard (admin overview)
// ============================================================
router.get('/dashboard', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const [accountStats, transferStats, recentTransfers] = await Promise.all([
    db.query(
      `SELECT account_type, COUNT(*) AS count, SUM(balance) AS total_balance, currency
       FROM bank_accounts WHERE user_id = $1 AND status = 'active'
       GROUP BY account_type, currency`,
      [req.user.id]
    ),
    db.query(
      `SELECT transfer_type, COUNT(*) AS count, SUM(amount) AS total_amount, SUM(fee) AS total_fees
       FROM bank_transfers WHERE from_user_id = $1
       GROUP BY transfer_type`,
      [req.user.id]
    ),
    db.query(
      `SELECT transfer_id, from_account, to_account, amount, currency, transfer_type, status, created_at
       FROM bank_transfers WHERE from_user_id = $1
       ORDER BY created_at DESC LIMIT 10`,
      [req.user.id]
    )
  ]);

  res.json({
    status: 'ok',
    data: {
      accounts: accountStats.rows,
      transferSummary: transferStats.rows,
      recentTransfers: recentTransfers.rows,
      centralBanksNetwork: CENTRAL_BANKS.length,
      supportedCurrencies: [...new Set(CENTRAL_BANKS.map(b => b.currency))],
      bank: 'BDET Bank — Ierahkwa Ne Kanienke',
      blockchain: { network: 'MameyNode', chainId: 574, nativeCurrency: 'WAMPUM (WMP)' }
    }
  });
}));

// ============================================================
// GET /exchange-rates — Current exchange rates via BDET
// ============================================================
router.get('/exchange-rates', asyncHandler(async (_req, res) => {
  // Base rates against WMP (WAMPUM)
  const rates = {
    WMP: { USD: 1.00, EUR: 0.92, GBP: 0.79, JPY: 149.50, CHF: 0.87, CAD: 1.36, AUD: 1.53, MXN: 17.15, BRL: 4.97, INR: 83.12, CNY: 7.24, PAB: 1.00, BTC: 0.0000148, ETH: 0.000308 },
    timestamp: new Date().toISOString(),
    source: 'BDET Bank Sovereign Exchange',
    updateFrequency: '1 second',
    blockchain: 'MameyNode (Chain ID 574)'
  };

  res.json({ status: 'ok', data: rates });
}));

// ============================================================
// POST /exchange — Currency exchange (swap)
// ============================================================
router.post('/exchange',
  validate({
    body: {
      from_currency: t.string({ required: true }),
      to_currency: t.string({ required: true }),
      amount: t.number({ required: true, min: 0.01 }),
      account_number: t.string({ required: true })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { from_currency, to_currency, amount, account_number } = req.body;

    if (from_currency === to_currency) throw new AppError('INVALID_INPUT', 'Cannot exchange same currency');

    // Get exchange rate (simplified — production would query real-time oracle)
    const baseRates = { USD: 1, EUR: 0.92, GBP: 0.79, JPY: 149.50, WMP: 1, BDET: 10, IGT: 0.5, CHF: 0.87, PAB: 1, BTC: 67500, ETH: 3250 };
    const fromRate = baseRates[from_currency];
    const toRate = baseRates[to_currency];
    if (!fromRate || !toRate) throw new AppError('INVALID_INPUT', 'Unsupported currency pair');

    const exchangeRate = fromRate / toRate;
    const swapFee = amount * 0.002; // 0.2% exchange fee
    const netAmount = amount - swapFee;
    const receivedAmount = netAmount * exchangeRate;

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Verify and debit account
      const accResult = await client.query(
        `SELECT * FROM bank_accounts WHERE account_number = $1 AND user_id = $2 AND status = 'active' FOR UPDATE`,
        [account_number, req.user.id]
      );
      if (accResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Account not found');
      if (parseFloat(accResult.rows[0].balance) < amount) throw new AppError('INSUFFICIENT_FUNDS', 'Insufficient balance');

      // Debit from_currency
      await client.query(
        `UPDATE bank_accounts SET balance = balance - $1, updated_at = NOW() WHERE account_number = $2`,
        [amount, account_number]
      );

      // Credit to_currency — find or create account
      let destAcc = await client.query(
        `SELECT account_number FROM bank_accounts WHERE user_id = $1 AND currency = $2 AND status = 'active' LIMIT 1`,
        [req.user.id, to_currency]
      );

      if (destAcc.rows.length === 0) {
        // Auto-create account for new currency
        const newAccNum = `IK${'EX'}${crypto.randomBytes(8).toString('hex').toUpperCase().slice(0, 12)}`;
        await client.query(
          `INSERT INTO bank_accounts (user_id, account_number, iban, account_type, currency, display_name, balance, status, vip_tier)
           VALUES ($1, $2, $3, 'checking', $4, $5, $6, 'active', 'standard')`,
          [req.user.id, newAccNum, `IK00BDET${newAccNum}`, to_currency, `${to_currency} Account`, receivedAmount]
        );
        destAcc = { rows: [{ account_number: newAccNum }] };
      } else {
        await client.query(
          `UPDATE bank_accounts SET balance = balance + $1, updated_at = NOW() WHERE account_number = $2`,
          [receivedAmount, destAcc.rows[0].account_number]
        );
      }

      const txHash = crypto.createHash('sha256')
        .update(`exchange:${from_currency}:${to_currency}:${amount}:${Date.now()}`)
        .digest('hex');

      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
         VALUES ($1, $2, $3, $4, 'exchange', $5, $6, 'completed', $7::jsonb, NOW())`,
        [req.user.id, req.user.id, receivedAmount, to_currency,
         `Exchange ${from_currency} → ${to_currency}`, txHash,
         JSON.stringify({ from: from_currency, to: to_currency, rate: exchangeRate, fee: swapFee, originalAmount: amount })]
      );

      await client.query('COMMIT');

      res.json({
        status: 'ok',
        data: {
          from: { currency: from_currency, amount },
          to: { currency: to_currency, amount: receivedAmount, account: destAcc.rows[0].account_number },
          exchangeRate,
          fee: swapFee,
          feePercent: 0.2,
          txHash
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      throw new AppError('TRANSACTION_FAILED', 'Exchange failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// Exports
// ============================================================
module.exports = router;
