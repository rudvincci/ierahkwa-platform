'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Staking Module v1.0.0 — Sovereign Proof of Stake
// Stake WPM/IGT/BDET for rewards + governance
// MameyNode blockchain (Chain ID 574)
// ============================================================

const { Router } = require('express');
const crypto = require('crypto');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:staking');
const audit = createAuditLogger('sovereign-core:staking');

// ============================================================
// Staking Pools
// ============================================================
const STAKING_POOLS = {
  'wpm-sovereign': {
    name: 'WAMPUM Sovereign Pool',
    token: 'WMP',
    apy: 12.0,
    minStake: 100,
    lockPeriodDays: 30,
    maxCapacity: 10000000,
    rewardToken: 'WMP',
    compounding: true,
    description: 'Stake WPM to secure MameyNode + earn rewards'
  },
  'wpm-flex': {
    name: 'WAMPUM Flexible',
    token: 'WMP',
    apy: 5.0,
    minStake: 10,
    lockPeriodDays: 0,
    maxCapacity: 50000000,
    rewardToken: 'WMP',
    compounding: false,
    description: 'Flexible staking — withdraw anytime, lower rewards'
  },
  'igt-governance': {
    name: 'IGT Governance Staking',
    token: 'IGT',
    apy: 15.0,
    minStake: 50,
    lockPeriodDays: 90,
    maxCapacity: 5000000,
    rewardToken: 'IGT',
    compounding: true,
    votingPower: true,
    description: 'Stake IGT for governance voting power + higher rewards'
  },
  'bdet-bank': {
    name: 'BDET Bank Reserve',
    token: 'BDET',
    apy: 8.0,
    minStake: 1,
    lockPeriodDays: 60,
    maxCapacity: 1000000,
    rewardToken: 'BDET',
    compounding: true,
    description: 'Stake BDET to back bank reserves + earn interest'
  },
  'lp-wpm-usd': {
    name: 'WPM/USD Liquidity Pool',
    token: 'WMP',
    pairToken: 'USD',
    apy: 25.0,
    minStake: 500,
    lockPeriodDays: 14,
    maxCapacity: 20000000,
    rewardToken: 'WMP',
    compounding: true,
    isLiquidityPool: true,
    description: 'Provide liquidity for WPM/USD trading pair'
  },
  'validator-node': {
    name: 'Validator Node Stake',
    token: 'WMP',
    apy: 18.0,
    minStake: 10000,
    lockPeriodDays: 180,
    maxCapacity: 100000000,
    rewardToken: 'WMP',
    compounding: true,
    validatorRequired: true,
    description: 'Run a MameyNode validator — highest rewards'
  }
};

function generateId(prefix) {
  return `${prefix}-${crypto.randomBytes(8).toString('hex').toUpperCase()}`;
}

// ============================================================
// GET /pools — All staking pools
// ============================================================
router.get('/pools', asyncHandler(async (_req, res) => {
  // Get current staked amounts per pool
  let poolStats = {};
  try {
    const stats = await db.query(
      `SELECT pool_id, COUNT(*) AS stakers, SUM(amount) AS total_staked
       FROM staking_positions WHERE status = 'active' GROUP BY pool_id`
    );
    stats.rows.forEach(r => { poolStats[r.pool_id] = r; });
  } catch { /* ok */ }

  const pools = Object.entries(STAKING_POOLS).map(([id, pool]) => ({
    poolId: id,
    ...pool,
    currentStakers: parseInt(poolStats[id]?.stakers || 0),
    totalStaked: parseFloat(poolStats[id]?.total_staked || 0),
    utilization: poolStats[id] ? (parseFloat(poolStats[id].total_staked) / pool.maxCapacity * 100).toFixed(2) + '%' : '0%'
  }));

  res.json({ status: 'ok', data: { pools, totalPools: pools.length } });
}));

// ============================================================
// POST /stake — Stake tokens
// ============================================================
router.post('/stake',
  validate({
    body: {
      pool_id: t.string({ required: true, enum: Object.keys(STAKING_POOLS) }),
      amount: t.number({ required: true, min: 0.01 }),
      auto_compound: t.string({ enum: ['yes', 'no'] })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { pool_id, amount, auto_compound } = req.body;
    const pool = STAKING_POOLS[pool_id];

    if (amount < pool.minStake) {
      throw new AppError('INVALID_INPUT', `Minimum stake is ${pool.minStake} ${pool.token}`);
    }

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Check pool capacity
      let currentStaked = 0;
      try {
        const poolTotal = await client.query(
          `SELECT COALESCE(SUM(amount), 0) AS total FROM staking_positions WHERE pool_id = $1 AND status = 'active'`,
          [pool_id]
        );
        currentStaked = parseFloat(poolTotal.rows[0].total);
      } catch { /* ok */ }

      if (currentStaked + amount > pool.maxCapacity) {
        throw new AppError('POOL_FULL', `Pool capacity reached. Available: ${(pool.maxCapacity - currentStaked).toFixed(2)} ${pool.token}`);
      }

      // Debit user balance
      const userAccount = await client.query(
        `SELECT account_number, balance FROM bank_accounts
         WHERE user_id = $1 AND currency = $2 AND status = 'active' AND balance >= $3
         ORDER BY balance DESC LIMIT 1 FOR UPDATE`,
        [req.user.id, pool.token, amount]
      );

      if (!userAccount.rows.length) {
        // Try crypto wallet
        const wallet = await client.query(
          `SELECT wallet_id, balance FROM crypto_wallets
           WHERE user_id = $1 AND chain = 'mameynode' AND status = 'active' AND balance >= $2
           ORDER BY balance DESC LIMIT 1 FOR UPDATE`,
          [req.user.id, amount]
        );
        if (!wallet.rows.length) {
          throw new AppError('INSUFFICIENT_FUNDS', `Need ${amount} ${pool.token} to stake`);
        }
        await client.query(
          `UPDATE crypto_wallets SET balance = balance - $1, updated_at = NOW() WHERE wallet_id = $2`,
          [amount, wallet.rows[0].wallet_id]
        );
      } else {
        await client.query(
          `UPDATE bank_accounts SET balance = balance - $1, updated_at = NOW() WHERE account_number = $2`,
          [amount, userAccount.rows[0].account_number]
        );
      }

      const stakeId = generateId('STK');
      const lockUntil = pool.lockPeriodDays > 0
        ? new Date(Date.now() + pool.lockPeriodDays * 24 * 60 * 60 * 1000)
        : null;

      await client.query(
        `INSERT INTO staking_positions
           (stake_id, user_id, pool_id, token, amount, apy, lock_period_days,
            lock_until, auto_compound, rewards_earned, status)
         VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, 0, 'active')`,
        [stakeId, req.user.id, pool_id, pool.token, amount,
         pool.apy, pool.lockPeriodDays, lockUntil,
         auto_compound !== 'no']
      );

      // Record transaction
      const txHash = crypto.createHash('sha256').update(`stake:${stakeId}:${amount}:${Date.now()}`).digest('hex');
      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at)
         VALUES ($1, 'staking-pool', $2, $3, 'staking_deposit', $4, $5, 'completed', NOW())`,
        [req.user.id, amount, pool.token, `Staked in ${pool.name}`, txHash]
      );

      await client.query('COMMIT');

      // Calculate projected rewards
      const dailyReward = (amount * pool.apy / 100) / 365;
      const monthlyReward = dailyReward * 30;
      const yearlyReward = amount * pool.apy / 100;

      audit.record({
        category: 'STAKING',
        action: 'tokens_staked',
        userId: req.user.id,
        risk: 'MEDIUM',
        details: { stakeId, poolId: pool_id, amount, token: pool.token }
      });

      res.status(201).json({
        status: 'ok',
        data: {
          stakeId,
          pool: pool.name,
          amount,
          token: pool.token,
          apy: pool.apy,
          lockPeriod: pool.lockPeriodDays > 0 ? `${pool.lockPeriodDays} days` : 'Flexible',
          lockUntil: lockUntil?.toISOString() || null,
          autoCompound: auto_compound !== 'no',
          projectedRewards: {
            daily: parseFloat(dailyReward.toFixed(8)),
            monthly: parseFloat(monthlyReward.toFixed(4)),
            yearly: parseFloat(yearlyReward.toFixed(2)),
            token: pool.rewardToken
          },
          votingPower: pool.votingPower ? amount : null,
          txHash,
          blockchain: { network: 'MameyNode', chainId: 574 }
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      throw new AppError('STAKING_FAILED', 'Staking failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// POST /unstake — Unstake tokens
// ============================================================
router.post('/unstake',
  validate({
    body: {
      stake_id: t.string({ required: true })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      const position = await client.query(
        `SELECT * FROM staking_positions WHERE stake_id = $1 AND user_id = $2 AND status = 'active' FOR UPDATE`,
        [req.body.stake_id, req.user.id]
      );
      if (position.rows.length === 0) throw new AppError('NOT_FOUND', 'Staking position not found');
      const pos = position.rows[0];

      // Check lock period
      if (pos.lock_until && new Date(pos.lock_until) > new Date()) {
        const remaining = Math.ceil((new Date(pos.lock_until) - new Date()) / (1000 * 60 * 60 * 24));
        throw new AppError('LOCKED', `Tokens locked for ${remaining} more days. Unlock date: ${pos.lock_until}`);
      }

      // Calculate accrued rewards
      const stakeDays = (Date.now() - new Date(pos.created_at).getTime()) / (1000 * 60 * 60 * 24);
      const dailyRate = parseFloat(pos.apy) / 100 / 365;
      const accruedRewards = parseFloat(pos.amount) * dailyRate * stakeDays;
      const totalReturn = parseFloat(pos.amount) + accruedRewards + parseFloat(pos.rewards_earned || 0);

      // Return staked amount + rewards to user
      const userAccount = await client.query(
        `SELECT account_number FROM bank_accounts
         WHERE user_id = $1 AND currency = $2 AND status = 'active' ORDER BY balance DESC LIMIT 1`,
        [req.user.id, pos.token]
      );

      if (userAccount.rows.length > 0) {
        await client.query(
          `UPDATE bank_accounts SET balance = balance + $1, updated_at = NOW() WHERE account_number = $2`,
          [totalReturn, userAccount.rows[0].account_number]
        );
      } else {
        // Try crypto wallet
        await client.query(
          `UPDATE crypto_wallets SET balance = balance + $1, updated_at = NOW()
           WHERE user_id = $2 AND chain = 'mameynode' AND status = 'active'
           ORDER BY balance DESC LIMIT 1`,
          [totalReturn, req.user.id]
        );
      }

      // Update position
      await client.query(
        `UPDATE staking_positions SET status = 'unstaked', rewards_earned = rewards_earned + $1, unstaked_at = NOW()
         WHERE stake_id = $2`,
        [accruedRewards, pos.stake_id]
      );

      const txHash = crypto.createHash('sha256').update(`unstake:${pos.stake_id}:${totalReturn}:${Date.now()}`).digest('hex');
      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at)
         VALUES ('staking-pool', $1, $2, $3, 'staking_withdrawal', $4, $5, 'completed', NOW())`,
        [req.user.id, totalReturn, pos.token, `Unstaked from ${pos.pool_id} (${stakeDays.toFixed(0)} days)`, txHash]
      );

      await client.query('COMMIT');

      res.json({
        status: 'ok',
        data: {
          stakeId: pos.stake_id,
          originalStake: parseFloat(pos.amount),
          rewardsEarned: parseFloat(accruedRewards.toFixed(8)),
          previousRewards: parseFloat(pos.rewards_earned || 0),
          totalReturned: parseFloat(totalReturn.toFixed(8)),
          stakeDuration: `${Math.floor(stakeDays)} days`,
          effectiveAPY: parseFloat((accruedRewards / parseFloat(pos.amount) * 365 / stakeDays * 100).toFixed(2)),
          token: pos.token,
          txHash
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      throw new AppError('UNSTAKE_FAILED', 'Unstaking failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// GET /positions — User's staking positions
// ============================================================
router.get('/positions', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `SELECT stake_id, pool_id, token, amount, apy, lock_period_days, lock_until,
            auto_compound, rewards_earned, status, created_at, unstaked_at
     FROM staking_positions WHERE user_id = $1 ORDER BY created_at DESC`,
    [req.user.id]
  );

  // Calculate current accrued rewards for active positions
  const positions = result.rows.map(pos => {
    if (pos.status === 'active') {
      const stakeDays = (Date.now() - new Date(pos.created_at).getTime()) / (1000 * 60 * 60 * 24);
      const dailyRate = parseFloat(pos.apy) / 100 / 365;
      const accrued = parseFloat(pos.amount) * dailyRate * stakeDays;
      return { ...pos, accruedRewards: parseFloat(accrued.toFixed(8)), stakeDays: Math.floor(stakeDays) };
    }
    return pos;
  });

  const totalStaked = positions.filter(p => p.status === 'active').reduce((s, p) => s + parseFloat(p.amount), 0);
  const totalRewards = positions.reduce((s, p) => s + parseFloat(p.rewards_earned || 0) + parseFloat(p.accruedRewards || 0), 0);

  res.json({
    status: 'ok',
    data: {
      positions,
      summary: {
        totalStaked,
        totalRewards: parseFloat(totalRewards.toFixed(8)),
        activePositions: positions.filter(p => p.status === 'active').length,
        totalPositions: positions.length
      }
    }
  });
}));

// ============================================================
// GET /rewards — Rewards history
// ============================================================
router.get('/rewards', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `SELECT t.amount, t.currency, t.memo, t.tx_hash, t.created_at
     FROM transactions t
     WHERE t.to_user = $1 AND t.type IN ('staking_reward', 'staking_withdrawal')
     ORDER BY t.created_at DESC LIMIT 50`,
    [req.user.id]
  );

  res.json({ status: 'ok', data: result.rows });
}));

module.exports = router;
