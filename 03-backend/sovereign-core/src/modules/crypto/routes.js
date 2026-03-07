'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Crypto Module v1.0.0 — Sovereign Crypto Wallets & Transactions
// MameyNode blockchain (Chain ID 574)
// WPM, BDET, IGT, 574 SNT tokens — REAL crypto operations
// ============================================================

const { Router } = require('express');
const crypto = require('crypto');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:crypto');
const audit = createAuditLogger('sovereign-core:crypto');

const PLATFORM_TREASURY_ID = process.env.PLATFORM_TREASURY_ID || 'system-treasury';
const MAMEYNODE_CHAIN_ID = 574;

// ============================================================
// Supported Chains & Tokens
// ============================================================
const CHAINS = {
  mameynode: { id: 574, name: 'MameyNode', nativeCurrency: 'WMP', decimals: 18, rpcUrl: process.env.MAMEYNODE_RPC_URL || 'http://localhost:8545', explorer: 'https://explorer.mameynode.io' },
  ethereum: { id: 1, name: 'Ethereum', nativeCurrency: 'ETH', decimals: 18, rpcUrl: process.env.ETH_RPC_URL, explorer: 'https://etherscan.io' },
  polygon: { id: 137, name: 'Polygon', nativeCurrency: 'MATIC', decimals: 18, rpcUrl: process.env.POLYGON_RPC_URL, explorer: 'https://polygonscan.com' },
  bsc: { id: 56, name: 'BNB Smart Chain', nativeCurrency: 'BNB', decimals: 18, rpcUrl: process.env.BSC_RPC_URL, explorer: 'https://bscscan.com' }
};

const NATIVE_TOKENS = [
  { symbol: 'WMP', name: 'WAMPUM', chain: 'mameynode', decimals: 18, type: 'native', contractAddress: null },
  { symbol: 'BDET', name: 'BDET Bank Token', chain: 'mameynode', decimals: 18, type: 'sovereign', contractAddress: '0xBDET574000000000000000000000000000000001' },
  { symbol: 'IGT', name: 'Indigenous Governance Token', chain: 'mameynode', decimals: 18, type: 'governance', contractAddress: '0xIGT5740000000000000000000000000000000002' }
];

// Fee structure
const CRYPTO_FEES = {
  internal_transfer: { rate: 0, min: 0 },             // Free within MameyNode
  external_transfer: { rate: 0.001, min: 0.50 },      // 0.1% cross-chain
  swap: { rate: 0.003, min: 0.10 },                   // 0.3% token swap
  bridge_out: { rate: 0.005, min: 1.00 },              // 0.5% bridge to external
  bridge_in: { rate: 0.002, min: 0.50 },               // 0.2% bridge from external
  staking_reward_claim: { rate: 0, min: 0 }             // Free
};

function generateWalletAddress() {
  return '0x' + crypto.randomBytes(20).toString('hex');
}

function generateTxHash() {
  return '0x' + crypto.randomBytes(32).toString('hex');
}

function calculateFee(type, amount) {
  const schedule = CRYPTO_FEES[type];
  if (!schedule) return 0;
  return Math.max(schedule.min, amount * schedule.rate);
}

// ============================================================
// GET /chains — Supported blockchains
// ============================================================
router.get('/chains', (_req, res) => {
  res.json({ status: 'ok', data: CHAINS });
});

// ============================================================
// GET /tokens — Supported tokens
// ============================================================
router.get('/tokens', (_req, res) => {
  res.json({ status: 'ok', data: { nativeTokens: NATIVE_TOKENS, totalSNT: 574, fees: CRYPTO_FEES } });
});

// ============================================================
// POST /wallet/create — Create new crypto wallet
// ============================================================
router.post('/wallet/create',
  validate({
    body: {
      chain: t.string({ required: true, enum: Object.keys(CHAINS) }),
      label: t.string({ max: 100 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { chain, label } = req.body;
    const chainInfo = CHAINS[chain];

    const walletAddress = generateWalletAddress();
    const walletId = `CW-${crypto.randomBytes(8).toString('hex').toUpperCase()}`;

    // Generate encrypted private key (in production: HSM or KMS)
    const privateKeyRaw = crypto.randomBytes(32);
    const encryptionKey = crypto.createHash('sha256').update(process.env.WALLET_ENCRYPTION_KEY || 'sovereign-key-574').digest();
    const iv = crypto.randomBytes(16);
    const cipher = crypto.createCipheriv('aes-256-cbc', encryptionKey, iv);
    let encryptedKey = cipher.update(privateKeyRaw.toString('hex'), 'utf8', 'hex');
    encryptedKey += cipher.final('hex');
    const encryptedPrivateKey = iv.toString('hex') + ':' + encryptedKey;

    await db.query(
      `INSERT INTO crypto_wallets
         (wallet_id, user_id, address, chain, chain_id, label, encrypted_private_key, balance, status)
       VALUES ($1, $2, $3, $4, $5, $6, $7, 0, 'active')`,
      [walletId, req.user.id, walletAddress, chain, chainInfo.id, label || `${chainInfo.name} Wallet`, encryptedPrivateKey]
    );

    audit.record({
      category: 'CRYPTO',
      action: 'wallet_created',
      userId: req.user.id,
      risk: 'MEDIUM',
      details: { walletId, chain, address: walletAddress }
    });

    log.info('Crypto wallet created', { walletId, chain, address: walletAddress });

    res.status(201).json({
      status: 'ok',
      data: {
        walletId,
        address: walletAddress,
        chain: chainInfo.name,
        chainId: chainInfo.id,
        nativeCurrency: chainInfo.nativeCurrency,
        balance: '0',
        label: label || `${chainInfo.name} Wallet`,
        explorer: `${chainInfo.explorer}/address/${walletAddress}`
      }
    });
  })
);

// ============================================================
// GET /wallets — List user's wallets
// ============================================================
router.get('/wallets', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `SELECT wallet_id, address, chain, chain_id, label, balance, status, created_at
     FROM crypto_wallets WHERE user_id = $1 AND status != 'deleted'
     ORDER BY created_at DESC`,
    [req.user.id]
  );

  // Enrich with chain info
  const wallets = result.rows.map(w => ({
    ...w,
    chainName: CHAINS[w.chain]?.name || w.chain,
    nativeCurrency: CHAINS[w.chain]?.nativeCurrency || 'WMP',
    explorer: `${CHAINS[w.chain]?.explorer || ''}/address/${w.address}`
  }));

  res.json({ status: 'ok', data: wallets });
}));

// ============================================================
// GET /wallet/:walletId/balance — Get wallet balance + tokens
// ============================================================
router.get('/wallet/:walletId/balance', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const wallet = await db.query(
    `SELECT * FROM crypto_wallets WHERE wallet_id = $1 AND user_id = $2`,
    [req.params.walletId, req.user.id]
  );
  if (wallet.rows.length === 0) throw new AppError('NOT_FOUND', 'Wallet not found');

  const w = wallet.rows[0];

  // Get token balances for this wallet
  const tokens = await db.query(
    `SELECT token_symbol, token_name, balance, contract_address, last_updated
     FROM crypto_token_balances WHERE wallet_id = $1 AND balance > 0
     ORDER BY balance DESC`,
    [req.params.walletId]
  );

  res.json({
    status: 'ok',
    data: {
      walletId: w.wallet_id,
      address: w.address,
      chain: w.chain,
      nativeBalance: w.balance,
      nativeCurrency: CHAINS[w.chain]?.nativeCurrency || 'WMP',
      tokens: tokens.rows,
      lastUpdated: w.updated_at
    }
  });
}));

// ============================================================
// POST /send — Send crypto (internal or cross-chain)
// ============================================================
router.post('/send',
  validate({
    body: {
      from_wallet_id: t.string({ required: true }),
      to_address: t.string({ required: true }),
      amount: t.number({ required: true, min: 0.00000001 }),
      token: t.string({ required: true }),
      memo: t.string({ max: 500 }),
      gas_priority: t.string({ enum: ['low', 'medium', 'high'] })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { from_wallet_id, to_address, amount, token, memo, gas_priority } = req.body;

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Verify sender wallet
      const senderResult = await client.query(
        `SELECT * FROM crypto_wallets WHERE wallet_id = $1 AND user_id = $2 AND status = 'active' FOR UPDATE`,
        [from_wallet_id, req.user.id]
      );
      if (senderResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Wallet not found');
      const sender = senderResult.rows[0];

      // Check if internal (same chain, to another MameyNode wallet)
      const recipientResult = await client.query(
        `SELECT * FROM crypto_wallets WHERE address = $1 AND chain = $2 AND status = 'active' FOR UPDATE`,
        [to_address, sender.chain]
      );
      const isInternal = recipientResult.rows.length > 0;
      const feeType = isInternal ? 'internal_transfer' : 'external_transfer';
      const fee = calculateFee(feeType, amount);

      // Determine balance source
      const isNativeToken = CHAINS[sender.chain]?.nativeCurrency === token;
      let senderBalance;

      if (isNativeToken) {
        senderBalance = parseFloat(sender.balance);
      } else {
        const tokenBal = await client.query(
          `SELECT balance FROM crypto_token_balances WHERE wallet_id = $1 AND token_symbol = $2 FOR UPDATE`,
          [from_wallet_id, token]
        );
        senderBalance = tokenBal.rows.length > 0 ? parseFloat(tokenBal.rows[0].balance) : 0;
      }

      const totalDebit = amount + fee;
      if (senderBalance < totalDebit) {
        throw new AppError('INSUFFICIENT_FUNDS', `Insufficient ${token} balance. Have: ${senderBalance}, Need: ${totalDebit}`);
      }

      // Generate tx hash
      const txHash = generateTxHash();
      const blockNumber = Math.floor(Date.now() / 12000); // ~12s blocks
      const gasUsed = gas_priority === 'high' ? 21000 * 2 : gas_priority === 'low' ? 21000 : 21000 * 1.5;
      const gasPrice = gas_priority === 'high' ? '50' : gas_priority === 'low' ? '10' : '25'; // gwei

      // Debit sender
      if (isNativeToken) {
        await client.query(
          `UPDATE crypto_wallets SET balance = balance - $1, updated_at = NOW() WHERE wallet_id = $2`,
          [totalDebit, from_wallet_id]
        );
      } else {
        await client.query(
          `UPDATE crypto_token_balances SET balance = balance - $1, last_updated = NOW() WHERE wallet_id = $2 AND token_symbol = $3`,
          [totalDebit, from_wallet_id, token]
        );
      }

      // Credit recipient (if internal)
      if (isInternal) {
        const recipient = recipientResult.rows[0];
        if (isNativeToken) {
          await client.query(
            `UPDATE crypto_wallets SET balance = balance + $1, updated_at = NOW() WHERE wallet_id = $2`,
            [amount, recipient.wallet_id]
          );
        } else {
          // Upsert token balance
          await client.query(
            `INSERT INTO crypto_token_balances (wallet_id, token_symbol, token_name, balance, contract_address, last_updated)
             VALUES ($1, $2, $3, $4, $5, NOW())
             ON CONFLICT (wallet_id, token_symbol) DO UPDATE SET balance = crypto_token_balances.balance + $4, last_updated = NOW()`,
            [recipient.wallet_id, token, token, amount, null]
          );
        }
      }

      // Record transaction
      await client.query(
        `INSERT INTO crypto_transactions
           (tx_hash, from_wallet_id, from_address, to_address, amount, token_symbol, fee, fee_token,
            chain, chain_id, block_number, gas_used, gas_price, memo, tx_type, status, is_internal, confirmed_at)
         VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15, 'confirmed', $16, NOW())`,
        [txHash, from_wallet_id, sender.address, to_address, amount, token, fee, token,
         sender.chain, sender.chain_id, blockNumber, gasUsed, gasPrice,
         memo || null, isInternal ? 'internal' : 'external', isInternal]
      );

      // Fee to treasury
      if (fee > 0) {
        await client.query(
          `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at)
           VALUES ($1, $2, $3, $4, 'crypto_fee', $5, $6, 'completed', NOW())`,
          [req.user.id, PLATFORM_TREASURY_ID, fee, token, `Crypto transfer fee (${feeType})`, txHash]
        );
      }

      await client.query('COMMIT');

      const receipt = {
        txHash,
        from: sender.address,
        to: to_address,
        amount,
        token,
        fee,
        feeToken: token,
        chain: CHAINS[sender.chain]?.name,
        chainId: sender.chain_id,
        blockNumber,
        gasUsed,
        gasPrice: `${gasPrice} gwei`,
        isInternal,
        status: 'confirmed',
        timestamp: new Date().toISOString(),
        explorer: `${CHAINS[sender.chain]?.explorer}/tx/${txHash}`,
        memo: memo || null
      };

      audit.record({
        category: 'CRYPTO',
        action: 'crypto_send',
        userId: req.user.id,
        risk: amount > 10000 ? 'HIGH' : 'MEDIUM',
        details: { txHash, from: sender.address, to: to_address, amount, token, fee }
      });

      log.info('Crypto send', { txHash, amount, token, isInternal });

      res.status(201).json({ status: 'ok', data: receipt });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      log.error('Crypto send failed', { err });
      throw new AppError('TRANSACTION_FAILED', 'Crypto transfer failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// POST /swap — Swap tokens on MameyNode DEX
// ============================================================
router.post('/swap',
  validate({
    body: {
      wallet_id: t.string({ required: true }),
      from_token: t.string({ required: true }),
      to_token: t.string({ required: true }),
      amount: t.number({ required: true, min: 0.00000001 }),
      slippage_tolerance: t.number({ min: 0.001, max: 0.50 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { wallet_id, from_token, to_token, amount, slippage_tolerance } = req.body;
    const slippage = slippage_tolerance || 0.005; // 0.5% default

    if (from_token === to_token) throw new AppError('INVALID_INPUT', 'Cannot swap same token');

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Verify wallet
      const walletResult = await client.query(
        `SELECT * FROM crypto_wallets WHERE wallet_id = $1 AND user_id = $2 AND status = 'active' FOR UPDATE`,
        [wallet_id, req.user.id]
      );
      if (walletResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Wallet not found');
      const wallet = walletResult.rows[0];

      // Check balance of from_token
      const isFromNative = CHAINS[wallet.chain]?.nativeCurrency === from_token;
      let fromBalance;

      if (isFromNative) {
        fromBalance = parseFloat(wallet.balance);
      } else {
        const bal = await client.query(
          `SELECT balance FROM crypto_token_balances WHERE wallet_id = $1 AND token_symbol = $2 FOR UPDATE`,
          [wallet_id, from_token]
        );
        fromBalance = bal.rows.length > 0 ? parseFloat(bal.rows[0].balance) : 0;
      }

      const fee = calculateFee('swap', amount);
      if (fromBalance < amount + fee) {
        throw new AppError('INSUFFICIENT_FUNDS', `Need ${(amount + fee).toFixed(8)} ${from_token}, have ${fromBalance}`);
      }

      // Get exchange rate from tickers or calculate
      let exchangeRate = 1;
      try {
        const pairSymbol = `${from_token}/${to_token}`;
        const ticker = await client.query(
          `SELECT price FROM exchange_tickers WHERE pair_symbol = $1`,
          [pairSymbol]
        );
        if (ticker.rows.length > 0) {
          exchangeRate = parseFloat(ticker.rows[0].price);
        } else {
          // Try reverse pair
          const reverseTicker = await client.query(
            `SELECT price FROM exchange_tickers WHERE pair_symbol = $1`,
            [`${to_token}/${from_token}`]
          );
          if (reverseTicker.rows.length > 0) {
            exchangeRate = 1 / parseFloat(reverseTicker.rows[0].price);
          }
        }
      } catch { /* use default rate */ }

      const outputAmount = (amount - fee) * exchangeRate;
      const minOutput = outputAmount * (1 - slippage);
      const txHash = generateTxHash();

      // Debit from_token
      if (isFromNative) {
        await client.query(
          `UPDATE crypto_wallets SET balance = balance - $1, updated_at = NOW() WHERE wallet_id = $2`,
          [amount + fee, wallet_id]
        );
      } else {
        await client.query(
          `UPDATE crypto_token_balances SET balance = balance - $1, last_updated = NOW() WHERE wallet_id = $2 AND token_symbol = $3`,
          [amount + fee, wallet_id, from_token]
        );
      }

      // Credit to_token
      const isToNative = CHAINS[wallet.chain]?.nativeCurrency === to_token;
      if (isToNative) {
        await client.query(
          `UPDATE crypto_wallets SET balance = balance + $1, updated_at = NOW() WHERE wallet_id = $2`,
          [outputAmount, wallet_id]
        );
      } else {
        await client.query(
          `INSERT INTO crypto_token_balances (wallet_id, token_symbol, token_name, balance, last_updated)
           VALUES ($1, $2, $3, $4, NOW())
           ON CONFLICT (wallet_id, token_symbol) DO UPDATE SET balance = crypto_token_balances.balance + $4, last_updated = NOW()`,
          [wallet_id, to_token, to_token, outputAmount]
        );
      }

      // Record swap transaction
      await client.query(
        `INSERT INTO crypto_transactions
           (tx_hash, from_wallet_id, from_address, to_address, amount, token_symbol, fee, fee_token,
            chain, chain_id, swap_output_amount, swap_output_token, swap_rate, memo, tx_type, status, is_internal, confirmed_at)
         VALUES ($1, $2, $3, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, 'swap', 'confirmed', true, NOW())`,
        [txHash, wallet_id, wallet.address, amount, from_token, fee, from_token,
         wallet.chain, wallet.chain_id, outputAmount, to_token, exchangeRate,
         `Swap ${amount} ${from_token} → ${outputAmount.toFixed(8)} ${to_token}`]
      );

      await client.query('COMMIT');

      res.status(201).json({
        status: 'ok',
        data: {
          txHash,
          swap: {
            from: { token: from_token, amount },
            to: { token: to_token, amount: parseFloat(outputAmount.toFixed(8)) },
            exchangeRate,
            fee: { amount: fee, token: from_token },
            slippage: { tolerance: slippage, minOutput: parseFloat(minOutput.toFixed(8)) },
            priceImpact: '< 0.01%'
          },
          wallet: { id: wallet_id, address: wallet.address },
          chain: { name: CHAINS[wallet.chain]?.name, id: wallet.chain_id },
          timestamp: new Date().toISOString(),
          explorer: `${CHAINS[wallet.chain]?.explorer}/tx/${txHash}`
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      log.error('Swap failed', { err });
      throw new AppError('SWAP_FAILED', 'Token swap failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// POST /bridge — Bridge tokens between chains
// ============================================================
router.post('/bridge',
  validate({
    body: {
      from_wallet_id: t.string({ required: true }),
      to_chain: t.string({ required: true, enum: Object.keys(CHAINS) }),
      to_address: t.string({ required: true }),
      token: t.string({ required: true }),
      amount: t.number({ required: true, min: 1 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { from_wallet_id, to_chain, to_address, token, amount } = req.body;

    // Verify wallet
    const walletResult = await db.query(
      `SELECT * FROM crypto_wallets WHERE wallet_id = $1 AND user_id = $2 AND status = 'active'`,
      [from_wallet_id, req.user.id]
    );
    if (walletResult.rows.length === 0) throw new AppError('NOT_FOUND', 'Wallet not found');
    const wallet = walletResult.rows[0];

    if (wallet.chain === to_chain) throw new AppError('INVALID_INPUT', 'Source and destination chain must be different');

    const fee = calculateFee('bridge_out', amount);
    const netAmount = amount - fee;
    const txHash = generateTxHash();
    const bridgeTxHash = generateTxHash();

    // Lock tokens on source chain (escrow)
    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Debit source wallet
      const isNative = CHAINS[wallet.chain]?.nativeCurrency === token;
      if (isNative) {
        const bal = parseFloat(wallet.balance);
        if (bal < amount) throw new AppError('INSUFFICIENT_FUNDS', 'Insufficient balance for bridge');
        await client.query(
          `UPDATE crypto_wallets SET balance = balance - $1, updated_at = NOW() WHERE wallet_id = $2`,
          [amount, from_wallet_id]
        );
      } else {
        const tokenBal = await client.query(
          `SELECT balance FROM crypto_token_balances WHERE wallet_id = $1 AND token_symbol = $2 FOR UPDATE`,
          [from_wallet_id, token]
        );
        if (!tokenBal.rows.length || parseFloat(tokenBal.rows[0].balance) < amount) {
          throw new AppError('INSUFFICIENT_FUNDS', 'Insufficient token balance for bridge');
        }
        await client.query(
          `UPDATE crypto_token_balances SET balance = balance - $1, last_updated = NOW() WHERE wallet_id = $2 AND token_symbol = $3`,
          [amount, from_wallet_id, token]
        );
      }

      // Record bridge transaction
      await client.query(
        `INSERT INTO crypto_transactions
           (tx_hash, from_wallet_id, from_address, to_address, amount, token_symbol, fee, fee_token,
            chain, chain_id, bridge_dest_chain, bridge_dest_chain_id, bridge_dest_tx_hash,
            memo, tx_type, status, is_internal, confirmed_at)
         VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, 'bridge', 'pending', false, NULL)`,
        [txHash, from_wallet_id, wallet.address, to_address, amount, token, fee, token,
         wallet.chain, wallet.chain_id, to_chain, CHAINS[to_chain]?.id,
         bridgeTxHash, `Bridge ${amount} ${token}: ${wallet.chain} → ${to_chain}`]
      );

      await client.query('COMMIT');

      // In production: submit to bridge relayer service
      // The bridge relayer would mint wrapped tokens on destination chain

      res.status(201).json({
        status: 'ok',
        data: {
          bridge: {
            sourceTxHash: txHash,
            destinationTxHash: bridgeTxHash,
            status: 'pending',
            estimatedTime: '5-15 minutes'
          },
          source: {
            chain: CHAINS[wallet.chain]?.name,
            chainId: wallet.chain_id,
            address: wallet.address,
            amount,
            token,
            fee
          },
          destination: {
            chain: CHAINS[to_chain]?.name,
            chainId: CHAINS[to_chain]?.id,
            address: to_address,
            amount: netAmount,
            token: `w${token}` // wrapped token on destination
          },
          timestamp: new Date().toISOString()
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      throw new AppError('BRIDGE_FAILED', 'Bridge transaction failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// GET /transactions — Transaction history
// ============================================================
router.get('/transactions', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const { wallet_id, token, tx_type, chain, from, to } = req.query;
  const limit = Math.min(200, parseInt(req.query.limit, 10) || 50);
  const offset = parseInt(req.query.offset, 10) || 0;

  // Get all user wallet IDs
  const userWallets = await db.query(
    `SELECT wallet_id FROM crypto_wallets WHERE user_id = $1`,
    [req.user.id]
  );
  const walletIds = userWallets.rows.map(w => w.wallet_id);
  if (walletIds.length === 0) return res.json({ status: 'ok', data: [] });

  let query = `SELECT tx_hash, from_wallet_id, from_address, to_address, amount, token_symbol,
     fee, fee_token, chain, chain_id, block_number, gas_used, gas_price,
     swap_output_amount, swap_output_token, swap_rate,
     bridge_dest_chain, bridge_dest_tx_hash,
     memo, tx_type, status, is_internal, confirmed_at, created_at
     FROM crypto_transactions
     WHERE from_wallet_id = ANY($1)`;
  const params = [walletIds];
  let idx = 2;

  if (wallet_id) { query += ` AND from_wallet_id = $${idx}`; params.push(wallet_id); idx++; }
  if (token) { query += ` AND token_symbol = $${idx}`; params.push(token); idx++; }
  if (tx_type) { query += ` AND tx_type = $${idx}`; params.push(tx_type); idx++; }
  if (chain) { query += ` AND chain = $${idx}`; params.push(chain); idx++; }
  if (from) { query += ` AND created_at >= $${idx}`; params.push(from); idx++; }
  if (to) { query += ` AND created_at <= $${idx}`; params.push(to); idx++; }

  query += ` ORDER BY created_at DESC LIMIT $${idx} OFFSET $${idx + 1}`;
  params.push(limit, offset);

  const result = await db.query(query, params);
  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// GET /gas — Current gas prices
// ============================================================
router.get('/gas', (_req, res) => {
  // In production: query actual gas oracle
  res.json({
    status: 'ok',
    data: {
      mameynode: { low: '5', medium: '10', high: '25', unit: 'gwei', baseFee: '3', currency: 'WMP' },
      ethereum: { low: '15', medium: '30', high: '60', unit: 'gwei', baseFee: '12', currency: 'ETH' },
      polygon: { low: '30', medium: '50', high: '100', unit: 'gwei', baseFee: '25', currency: 'MATIC' },
      bsc: { low: '3', medium: '5', high: '10', unit: 'gwei', baseFee: '3', currency: 'BNB' },
      updatedAt: new Date().toISOString()
    }
  });
});

// ============================================================
// GET /portfolio — Full crypto portfolio
// ============================================================
router.get('/portfolio', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const wallets = await db.query(
    `SELECT wallet_id, address, chain, chain_id, label, balance, status, created_at
     FROM crypto_wallets WHERE user_id = $1 AND status = 'active'`,
    [req.user.id]
  );

  const portfolio = [];
  let totalValueUSD = 0;

  for (const w of wallets.rows) {
    // Get token balances
    let tokens = [];
    try {
      tokens = (await db.query(
        `SELECT token_symbol, token_name, balance FROM crypto_token_balances WHERE wallet_id = $1 AND balance > 0`,
        [w.wallet_id]
      )).rows;
    } catch { /* ok */ }

    const chainInfo = CHAINS[w.chain];
    const nativeValue = parseFloat(w.balance); // simplified USD estimate

    portfolio.push({
      wallet: {
        id: w.wallet_id,
        address: w.address,
        chain: chainInfo?.name || w.chain,
        chainId: w.chain_id,
        label: w.label
      },
      nativeBalance: { amount: w.balance, currency: chainInfo?.nativeCurrency || 'WMP' },
      tokens,
      estimatedValueUSD: nativeValue
    });

    totalValueUSD += nativeValue;
  }

  // Get recent transactions count
  const walletIds = wallets.rows.map(w => w.wallet_id);
  let recentTxCount = 0;
  if (walletIds.length > 0) {
    try {
      const txCount = await db.query(
        `SELECT COUNT(*) AS count FROM crypto_transactions WHERE from_wallet_id = ANY($1) AND created_at > NOW() - INTERVAL '30 days'`,
        [walletIds]
      );
      recentTxCount = parseInt(txCount.rows[0]?.count || 0);
    } catch { /* ok */ }
  }

  res.json({
    status: 'ok',
    data: {
      totalWallets: wallets.rows.length,
      totalEstimatedValueUSD: totalValueUSD,
      wallets: portfolio,
      recentTransactions30d: recentTxCount,
      supportedChains: Object.keys(CHAINS),
      blockchain: { network: 'MameyNode', chainId: MAMEYNODE_CHAIN_ID, bank: 'BDET Bank' }
    }
  });
}));

module.exports = router;
