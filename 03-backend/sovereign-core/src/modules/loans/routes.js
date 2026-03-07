'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Loans Module v1.0.0 — Sovereign Microloans & Credit
// Community lending, credit scoring, WPM collateral
// BDET Bank — MameyNode settlement
// ============================================================

const { Router } = require('express');
const crypto = require('crypto');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:loans');
const audit = createAuditLogger('sovereign-core:loans');

const PLATFORM_TREASURY_ID = process.env.PLATFORM_TREASURY_ID || 'system-treasury';

// ============================================================
// Loan Products
// ============================================================
const LOAN_PRODUCTS = {
  micro: { name: 'Micropréstamo', minAmount: 10, maxAmount: 500, termMonths: [1, 3, 6], apr: 5.0, requiresCollateral: false, description: 'Para necesidades inmediatas' },
  personal: { name: 'Préstamo Personal', minAmount: 100, maxAmount: 5000, termMonths: [3, 6, 12], apr: 8.0, requiresCollateral: false, description: 'Para gastos personales' },
  business: { name: 'Préstamo Negocio', minAmount: 500, maxAmount: 50000, termMonths: [6, 12, 24, 36], apr: 6.0, requiresCollateral: true, collateralRatio: 1.5, description: 'Para emprendimientos y negocios' },
  community: { name: 'Préstamo Comunitario', minAmount: 1000, maxAmount: 100000, termMonths: [12, 24, 36, 60], apr: 3.0, requiresCollateral: true, collateralRatio: 1.0, description: 'Proyectos comunitarios (tasa reducida)' },
  education: { name: 'Préstamo Educativo', minAmount: 100, maxAmount: 20000, termMonths: [12, 24, 36, 48], apr: 2.0, requiresCollateral: false, description: 'Para educación y formación' },
  agriculture: { name: 'Crédito Agrícola', minAmount: 200, maxAmount: 25000, termMonths: [6, 12], apr: 4.0, requiresCollateral: false, description: 'Para siembra y cosecha' },
  housing: { name: 'Crédito Vivienda', minAmount: 5000, maxAmount: 200000, termMonths: [60, 120, 180, 240, 360], apr: 4.5, requiresCollateral: true, collateralRatio: 1.2, description: 'Para compra o mejora de vivienda' }
};

// Credit score tiers
const CREDIT_TIERS = {
  excellent: { min: 800, maxLoanMultiplier: 5.0, aprDiscount: 0.02 },
  good: { min: 700, maxLoanMultiplier: 3.0, aprDiscount: 0.01 },
  fair: { min: 600, maxLoanMultiplier: 2.0, aprDiscount: 0 },
  poor: { min: 500, maxLoanMultiplier: 1.0, aprDiscount: -0.02 },
  new: { min: 0, maxLoanMultiplier: 0.5, aprDiscount: 0 }
};

function generateId(prefix) {
  return `${prefix}-${crypto.randomBytes(8).toString('hex').toUpperCase()}`;
}

function calculateMonthlyPayment(principal, annualRate, months) {
  const monthlyRate = annualRate / 12 / 100;
  if (monthlyRate === 0) return principal / months;
  return principal * (monthlyRate * Math.pow(1 + monthlyRate, months)) / (Math.pow(1 + monthlyRate, months) - 1);
}

function calculateAmortization(principal, annualRate, months) {
  const monthly = calculateMonthlyPayment(principal, annualRate, months);
  const schedule = [];
  let balance = principal;
  const monthlyRate = annualRate / 12 / 100;

  for (let i = 1; i <= months; i++) {
    const interest = balance * monthlyRate;
    const principalPayment = monthly - interest;
    balance -= principalPayment;
    schedule.push({
      month: i,
      payment: parseFloat(monthly.toFixed(2)),
      principal: parseFloat(principalPayment.toFixed(2)),
      interest: parseFloat(interest.toFixed(2)),
      balance: parseFloat(Math.max(0, balance).toFixed(2))
    });
  }
  return { monthlyPayment: parseFloat(monthly.toFixed(2)), totalInterest: parseFloat((monthly * months - principal).toFixed(2)), schedule };
}

// ============================================================
// GET /products — Loan products
// ============================================================
router.get('/products', (_req, res) => {
  const products = Object.entries(LOAN_PRODUCTS).map(([id, product]) => ({
    productId: id, ...product,
    monthlyPaymentExample: calculateMonthlyPayment(product.minAmount * 10, product.apr, product.termMonths[0])
  }));
  res.json({ status: 'ok', data: { products, creditTiers: CREDIT_TIERS, currency: 'WMP' } });
});

// ============================================================
// GET /credit-score — Get user's credit score
// ============================================================
router.get('/credit-score', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  // Calculate credit score based on platform activity
  let score = 600; // base
  let factors = [];

  try {
    // Account age
    const user = await db.query(`SELECT created_at FROM users WHERE id = $1`, [req.user.id]);
    if (user.rows.length > 0) {
      const ageMonths = (Date.now() - new Date(user.rows[0].created_at).getTime()) / (1000 * 60 * 60 * 24 * 30);
      if (ageMonths > 12) { score += 50; factors.push('Account age > 1 year (+50)'); }
      else if (ageMonths > 6) { score += 25; factors.push('Account age > 6 months (+25)'); }
    }

    // Transaction history
    const txCount = await db.query(
      `SELECT COUNT(*) AS count FROM transactions WHERE (from_user = $1 OR to_user = $1) AND status = 'completed'`,
      [req.user.id]
    );
    const txNum = parseInt(txCount.rows[0]?.count || 0);
    if (txNum > 100) { score += 80; factors.push('100+ transactions (+80)'); }
    else if (txNum > 50) { score += 50; factors.push('50+ transactions (+50)'); }
    else if (txNum > 10) { score += 20; factors.push('10+ transactions (+20)'); }

    // Bank account balance
    const balance = await db.query(
      `SELECT SUM(balance) AS total FROM bank_accounts WHERE user_id = $1 AND status = 'active'`,
      [req.user.id]
    );
    const totalBal = parseFloat(balance.rows[0]?.total || 0);
    if (totalBal > 10000) { score += 70; factors.push('Balance > 10,000 WMP (+70)'); }
    else if (totalBal > 1000) { score += 40; factors.push('Balance > 1,000 WMP (+40)'); }
    else if (totalBal > 100) { score += 15; factors.push('Balance > 100 WMP (+15)'); }

    // Previous loans (payment history)
    const loanHistory = await db.query(
      `SELECT COUNT(*) AS total,
              COUNT(CASE WHEN status = 'repaid' THEN 1 END) AS repaid,
              COUNT(CASE WHEN status = 'defaulted' THEN 1 END) AS defaulted
       FROM loans WHERE borrower_id = $1`,
      [req.user.id]
    );
    const lh = loanHistory.rows[0];
    if (parseInt(lh?.repaid || 0) > 0) { score += 100; factors.push(`${lh.repaid} loans repaid (+100)`); }
    if (parseInt(lh?.defaulted || 0) > 0) { score -= 200; factors.push(`${lh.defaulted} defaults (-200)`); }

    // VIP tier bonus
    const vip = await db.query(
      `SELECT vip_tier FROM bank_accounts WHERE user_id = $1 AND vip_tier != 'standard' LIMIT 1`,
      [req.user.id]
    );
    if (vip.rows.length > 0) {
      const tierBonus = { silver: 20, gold: 40, platinum: 60, black: 80, sovereign: 100 };
      const bonus = tierBonus[vip.rows[0].vip_tier] || 0;
      if (bonus > 0) { score += bonus; factors.push(`VIP ${vip.rows[0].vip_tier} (+${bonus})`); }
    }
  } catch { /* tables may not exist */ }

  score = Math.min(900, Math.max(300, score));
  const tier = Object.entries(CREDIT_TIERS).find(([, t]) => score >= t.min)?.[0] || 'new';

  res.json({
    status: 'ok',
    data: {
      score,
      tier,
      tierInfo: CREDIT_TIERS[tier],
      factors,
      maxBorrowingPower: parseFloat((score * (CREDIT_TIERS[tier]?.maxLoanMultiplier || 1)).toFixed(2)),
      currency: 'WMP',
      lastCalculated: new Date().toISOString()
    }
  });
}));

// ============================================================
// POST /apply — Apply for a loan
// ============================================================
router.post('/apply',
  validate({
    body: {
      product: t.string({ required: true, enum: Object.keys(LOAN_PRODUCTS) }),
      amount: t.number({ required: true, min: 1 }),
      term_months: t.number({ required: true, min: 1 }),
      purpose: t.string({ required: true, max: 1000 }),
      collateral_wallet_id: t.string({}),
      guarantor_id: t.string({})
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { product, amount, term_months, purpose, collateral_wallet_id, guarantor_id } = req.body;
    const productInfo = LOAN_PRODUCTS[product];

    if (amount < productInfo.minAmount || amount > productInfo.maxAmount) {
      throw new AppError('INVALID_INPUT', `Amount must be between ${productInfo.minAmount} and ${productInfo.maxAmount} WMP`);
    }

    if (!productInfo.termMonths.includes(term_months)) {
      throw new AppError('INVALID_INPUT', `Term must be one of: ${productInfo.termMonths.join(', ')} months`);
    }

    // Check credit score
    let creditScore = 600;
    try {
      const txCount = await db.query(
        `SELECT COUNT(*) AS c FROM transactions WHERE (from_user = $1 OR to_user = $1) AND status = 'completed'`,
        [req.user.id]
      );
      creditScore += Math.min(200, parseInt(txCount.rows[0]?.c || 0) * 2);
      const bal = await db.query(`SELECT SUM(balance) AS t FROM bank_accounts WHERE user_id = $1 AND status = 'active'`, [req.user.id]);
      if (parseFloat(bal.rows[0]?.t || 0) > 1000) creditScore += 50;
    } catch { /* ok */ }

    const tier = Object.entries(CREDIT_TIERS).find(([, t]) => creditScore >= t.min)?.[0] || 'new';
    const aprDiscount = CREDIT_TIERS[tier]?.aprDiscount || 0;
    const effectiveAPR = Math.max(0.5, productInfo.apr + aprDiscount);

    const amortization = calculateAmortization(amount, effectiveAPR, term_months);
    const loanId = generateId('LOAN');

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Check collateral if required
      let collateralAmount = 0;
      if (productInfo.requiresCollateral) {
        const ratio = productInfo.collateralRatio || 1.5;
        collateralAmount = amount * ratio;

        if (collateral_wallet_id) {
          const wallet = await client.query(
            `SELECT balance FROM crypto_wallets WHERE wallet_id = $1 AND user_id = $2 AND status = 'active'`,
            [collateral_wallet_id, req.user.id]
          );
          if (!wallet.rows.length || parseFloat(wallet.rows[0].balance) < collateralAmount) {
            throw new AppError('INSUFFICIENT_COLLATERAL', `Need ${collateralAmount} WMP in collateral (${ratio}x ratio)`);
          }
          // Lock collateral
          await client.query(
            `UPDATE crypto_wallets SET balance = balance - $1, updated_at = NOW() WHERE wallet_id = $2`,
            [collateralAmount, collateral_wallet_id]
          );
        } else {
          // Check bank balance as collateral
          const bal = await client.query(
            `SELECT account_number, balance FROM bank_accounts WHERE user_id = $1 AND currency = 'WMP' AND status = 'active' AND balance >= $2 ORDER BY balance DESC LIMIT 1 FOR UPDATE`,
            [req.user.id, collateralAmount]
          );
          if (!bal.rows.length) {
            throw new AppError('INSUFFICIENT_COLLATERAL', `Need ${collateralAmount} WMP in collateral (${ratio}x ratio)`);
          }
          await client.query(
            `UPDATE bank_accounts SET balance = balance - $1, updated_at = NOW() WHERE account_number = $2`,
            [collateralAmount, bal.rows[0].account_number]
          );
        }
      }

      // Create loan
      await client.query(
        `INSERT INTO loans
           (loan_id, borrower_id, product, amount, currency, apr, term_months,
            monthly_payment, total_interest, total_repayment,
            purpose, collateral_amount, collateral_wallet_id, guarantor_id,
            credit_score, credit_tier, status, disbursed_at, due_date)
         VALUES ($1, $2, $3, $4, 'WMP', $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15, 'active', NOW(), $16)`,
        [loanId, req.user.id, product, amount, effectiveAPR, term_months,
         amortization.monthlyPayment, amortization.totalInterest,
         amount + amortization.totalInterest,
         purpose, collateralAmount, collateral_wallet_id || null, guarantor_id || null,
         creditScore, tier, new Date(Date.now() + term_months * 30 * 24 * 60 * 60 * 1000)]
      );

      // Disburse loan amount to user's bank account
      const userAccount = await client.query(
        `SELECT account_number FROM bank_accounts WHERE user_id = $1 AND currency = 'WMP' AND status = 'active' ORDER BY balance DESC LIMIT 1`,
        [req.user.id]
      );
      if (userAccount.rows.length > 0) {
        await client.query(
          `UPDATE bank_accounts SET balance = balance + $1, updated_at = NOW() WHERE account_number = $2`,
          [amount, userAccount.rows[0].account_number]
        );
      }

      // Record disbursement transaction
      const txHash = crypto.createHash('sha256').update(`loan:${loanId}:${amount}:${Date.now()}`).digest('hex');
      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at)
         VALUES ($1, $2, $3, 'WMP', 'loan_disbursement', $4, $5, 'completed', NOW())`,
        ['system-lending', req.user.id, amount, `Loan disbursement: ${productInfo.name} (${loanId})`, txHash]
      );

      await client.query('COMMIT');

      audit.record({
        category: 'LOANS',
        action: 'loan_disbursed',
        userId: req.user.id,
        risk: amount > 10000 ? 'HIGH' : 'MEDIUM',
        details: { loanId, product, amount, apr: effectiveAPR, termMonths: term_months }
      });

      res.status(201).json({
        status: 'ok',
        data: {
          loanId,
          product: productInfo.name,
          amount,
          apr: effectiveAPR,
          termMonths: term_months,
          monthlyPayment: amortization.monthlyPayment,
          totalInterest: amortization.totalInterest,
          totalRepayment: amount + amortization.totalInterest,
          collateral: collateralAmount > 0 ? { amount: collateralAmount, currency: 'WMP' } : null,
          creditScore,
          creditTier: tier,
          status: 'active',
          firstPaymentDue: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
          amortizationSchedule: amortization.schedule.slice(0, 6), // first 6 months preview
          currency: 'WMP',
          blockchain: { network: 'MameyNode', chainId: 574, bank: 'BDET Bank' }
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      throw new AppError('LOAN_FAILED', 'Loan application failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// POST /repay — Make a loan payment
// ============================================================
router.post('/repay',
  validate({
    body: {
      loan_id: t.string({ required: true }),
      amount: t.number({ required: true, min: 0.01 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { loan_id, amount } = req.body;

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      const loan = await client.query(
        `SELECT * FROM loans WHERE loan_id = $1 AND borrower_id = $2 AND status = 'active' FOR UPDATE`,
        [loan_id, req.user.id]
      );
      if (loan.rows.length === 0) throw new AppError('NOT_FOUND', 'Active loan not found');
      const l = loan.rows[0];

      const remaining = parseFloat(l.total_repayment) - parseFloat(l.amount_repaid || 0);
      const paymentAmount = Math.min(amount, remaining);

      // Debit user
      const userAccount = await client.query(
        `SELECT account_number, balance FROM bank_accounts
         WHERE user_id = $1 AND currency = 'WMP' AND status = 'active' AND balance >= $2
         ORDER BY balance DESC LIMIT 1 FOR UPDATE`,
        [req.user.id, paymentAmount]
      );
      if (!userAccount.rows.length) {
        throw new AppError('INSUFFICIENT_FUNDS', `Need ${paymentAmount} WMP to make payment`);
      }

      await client.query(
        `UPDATE bank_accounts SET balance = balance - $1, updated_at = NOW() WHERE account_number = $2`,
        [paymentAmount, userAccount.rows[0].account_number]
      );

      // Update loan
      const newRepaid = parseFloat(l.amount_repaid || 0) + paymentAmount;
      const isFullyRepaid = newRepaid >= parseFloat(l.total_repayment);

      await client.query(
        `UPDATE loans SET amount_repaid = $1, payments_made = payments_made + 1,
                status = $2, last_payment_at = NOW(), updated_at = NOW()
         WHERE loan_id = $3`,
        [newRepaid, isFullyRepaid ? 'repaid' : 'active', loan_id]
      );

      // Release collateral if fully repaid
      if (isFullyRepaid && parseFloat(l.collateral_amount) > 0) {
        if (l.collateral_wallet_id) {
          await client.query(
            `UPDATE crypto_wallets SET balance = balance + $1, updated_at = NOW() WHERE wallet_id = $2`,
            [parseFloat(l.collateral_amount), l.collateral_wallet_id]
          );
        } else {
          await client.query(
            `UPDATE bank_accounts SET balance = balance + $1, updated_at = NOW()
             WHERE user_id = $2 AND currency = 'WMP' AND status = 'active' ORDER BY balance DESC LIMIT 1`,
            [parseFloat(l.collateral_amount), req.user.id]
          );
        }
      }

      // Record payment transaction
      const txHash = crypto.createHash('sha256').update(`repay:${loan_id}:${paymentAmount}:${Date.now()}`).digest('hex');
      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at)
         VALUES ($1, 'system-lending', $2, 'WMP', 'loan_repayment', $3, $4, 'completed', NOW())`,
        [req.user.id, paymentAmount, `Loan payment: ${loan_id}${isFullyRepaid ? ' (FULLY REPAID)' : ''}`, txHash]
      );

      await client.query('COMMIT');

      res.json({
        status: 'ok',
        data: {
          loanId: loan_id,
          paymentAmount,
          totalRepaid: newRepaid,
          remaining: Math.max(0, parseFloat(l.total_repayment) - newRepaid),
          fullyRepaid: isFullyRepaid,
          collateralReleased: isFullyRepaid && parseFloat(l.collateral_amount) > 0,
          status: isFullyRepaid ? 'repaid' : 'active',
          txHash
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      throw new AppError('PAYMENT_FAILED', 'Loan payment failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// GET /my-loans — User's loans
// ============================================================
router.get('/my-loans', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `SELECT loan_id, product, amount, apr, term_months, monthly_payment,
            total_interest, total_repayment, amount_repaid, payments_made,
            collateral_amount, credit_score, credit_tier,
            status, disbursed_at, due_date, last_payment_at, created_at
     FROM loans WHERE borrower_id = $1 ORDER BY created_at DESC`,
    [req.user.id]
  );

  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// GET /simulate — Simulate a loan
// ============================================================
router.get('/simulate', (req, res) => {
  const { product, amount, term_months } = req.query;
  if (!product || !amount || !term_months) {
    throw new AppError('INVALID_INPUT', 'Provide product, amount, term_months');
  }

  const productInfo = LOAN_PRODUCTS[product];
  if (!productInfo) throw new AppError('INVALID_INPUT', 'Product not found');

  const amt = parseFloat(amount);
  const months = parseInt(term_months);
  const amortization = calculateAmortization(amt, productInfo.apr, months);

  res.json({
    status: 'ok',
    data: {
      product: productInfo.name,
      amount: amt,
      apr: productInfo.apr,
      termMonths: months,
      monthlyPayment: amortization.monthlyPayment,
      totalInterest: amortization.totalInterest,
      totalRepayment: amt + amortization.totalInterest,
      collateralRequired: productInfo.requiresCollateral ? amt * (productInfo.collateralRatio || 1.5) : 0,
      amortizationSchedule: amortization.schedule,
      currency: 'WMP'
    }
  });
});

module.exports = router;
