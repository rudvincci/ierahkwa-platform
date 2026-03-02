'use strict';

const crypto = require('crypto');

// ============================================================
// MameyNode — In-Memory Sovereign Blockchain Engine
// Supports: blocks, transactions, wallets, token balances
// ============================================================

const GENESIS_TIMESTAMP = '2024-01-01T00:00:00.000Z';
const BLOCK_INTERVAL_MS = 15000; // 15-second blocks
const MAX_TX_PER_BLOCK = 100;
const NATIVE_TOKEN = 'WMP'; // WAMPUM

class Blockchain {
  constructor() {
    this.chain = [];
    this.pendingTx = [];
    this.wallets = new Map();     // address → { balance, nonce, created, label }
    this.txIndex = new Map();     // txHash → { tx, blockHash, blockHeight }
    this.tokenRegistry = new Map(); // symbol → { name, symbol, totalSupply, decimals, owner, holders }
    this.validators = new Map();  // address → { stake, blocks, uptime, lastSeen }
    this.listeners = [];

    this._initGenesis();
    this._registerNativeToken();
  }

  // ── Genesis ──────────────────────────────────────────────

  _initGenesis() {
    const genesis = {
      height: 0,
      hash: this._hash('IERAHKWA_GENESIS_BLOCK_2024'),
      previousHash: '0'.repeat(64),
      timestamp: GENESIS_TIMESTAMP,
      transactions: [],
      txCount: 0,
      merkleRoot: '0'.repeat(64),
      validator: 'genesis',
      nonce: 0,
      difficulty: 1,
      size: 285,
      gasUsed: 0,
      gasLimit: 8000000
    };
    this.chain.push(genesis);
  }

  _registerNativeToken() {
    this.tokenRegistry.set(NATIVE_TOKEN, {
      name: 'WAMPUM',
      symbol: NATIVE_TOKEN,
      totalSupply: 1000000000, // 1 billion
      decimals: 18,
      owner: 'sovereign-treasury',
      holders: new Map(),
      created: GENESIS_TIMESTAMP,
      type: 'native'
    });

    // Treasury wallet with initial supply
    this.wallets.set('sovereign-treasury', {
      balance: 1000000000,
      nonce: 0,
      created: GENESIS_TIMESTAMP,
      label: 'Sovereign Treasury'
    });
    this.tokenRegistry.get(NATIVE_TOKEN).holders.set('sovereign-treasury', 1000000000);
  }

  // ── Hashing ──────────────────────────────────────────────

  _hash(data) {
    return crypto.createHash('sha256').update(String(data)).digest('hex');
  }

  _computeMerkleRoot(transactions) {
    if (transactions.length === 0) return '0'.repeat(64);
    let hashes = transactions.map(tx => tx.hash);
    while (hashes.length > 1) {
      const next = [];
      for (let i = 0; i < hashes.length; i += 2) {
        const left = hashes[i];
        const right = hashes[i + 1] || left;
        next.push(this._hash(left + right));
      }
      hashes = next;
    }
    return hashes[0];
  }

  // ── Wallet Operations ────────────────────────────────────

  createWallet(label) {
    const privateKey = crypto.randomBytes(32).toString('hex');
    const address = '0x' + this._hash(privateKey).slice(0, 40);

    this.wallets.set(address, {
      balance: 0,
      nonce: 0,
      created: new Date().toISOString(),
      label: label || null
    });

    return { address, privateKey, label };
  }

  getWallet(address) {
    const wallet = this.wallets.get(address);
    if (!wallet) return null;
    return {
      address,
      balance: wallet.balance,
      nonce: wallet.nonce,
      created: wallet.created,
      label: wallet.label
    };
  }

  getWalletHistory(address, limit = 50) {
    const txs = [];
    for (const [hash, entry] of this.txIndex) {
      if (entry.tx.from === address || entry.tx.to === address) {
        txs.push({ ...entry.tx, blockHash: entry.blockHash, blockHeight: entry.blockHeight });
      }
    }
    txs.sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp));
    return txs.slice(0, limit);
  }

  listWallets(page = 1, limit = 20) {
    const all = [];
    for (const [address, data] of this.wallets) {
      all.push({ address, ...data });
    }
    all.sort((a, b) => b.balance - a.balance);
    const start = (page - 1) * limit;
    return {
      wallets: all.slice(start, start + limit),
      total: all.length,
      page,
      pages: Math.ceil(all.length / limit)
    };
  }

  // ── Transaction Operations ───────────────────────────────

  createTransaction(from, to, amount, data = {}) {
    const fromWallet = this.wallets.get(from);
    if (!fromWallet) throw new Error(`Sender wallet not found: ${from}`);
    if (!this.wallets.has(to)) throw new Error(`Recipient wallet not found: ${to}`);
    if (amount <= 0) throw new Error('Amount must be positive');
    if (fromWallet.balance < amount) throw new Error(`Insufficient balance: has ${fromWallet.balance}, needs ${amount}`);

    fromWallet.nonce++;

    const tx = {
      hash: this._hash(`${from}${to}${amount}${fromWallet.nonce}${Date.now()}`),
      from,
      to,
      amount,
      token: data.token || NATIVE_TOKEN,
      nonce: fromWallet.nonce,
      gasPrice: data.gasPrice || 1,
      gasLimit: data.gasLimit || 21000,
      timestamp: new Date().toISOString(),
      status: 'pending',
      type: data.type || 'transfer',
      memo: data.memo || null
    };

    this.pendingTx.push(tx);
    this._emit('pending_tx', tx);
    return tx;
  }

  getPendingTransactions() {
    return [...this.pendingTx];
  }

  getTransaction(hash) {
    // Check pending
    const pending = this.pendingTx.find(tx => tx.hash === hash);
    if (pending) return { ...pending, confirmations: 0 };

    // Check indexed
    const indexed = this.txIndex.get(hash);
    if (!indexed) return null;

    return {
      ...indexed.tx,
      blockHash: indexed.blockHash,
      blockHeight: indexed.blockHeight,
      confirmations: this.chain.length - 1 - indexed.blockHeight
    };
  }

  // ── Block Operations ─────────────────────────────────────

  mineBlock(validatorAddress) {
    const validator = validatorAddress || 'default-validator';
    const prev = this.chain[this.chain.length - 1];
    const txBatch = this.pendingTx.splice(0, MAX_TX_PER_BLOCK);

    // Process transactions
    let gasUsed = 0;
    const processed = [];

    for (const tx of txBatch) {
      const from = this.wallets.get(tx.from);
      const to = this.wallets.get(tx.to);

      if (from && to && from.balance >= tx.amount) {
        from.balance -= tx.amount;
        to.balance += tx.amount;
        tx.status = 'confirmed';
        gasUsed += tx.gasLimit;

        // Update token holders
        const token = this.tokenRegistry.get(tx.token);
        if (token) {
          const fromBal = (token.holders.get(tx.from) || 0) - tx.amount;
          const toBal = (token.holders.get(tx.to) || 0) + tx.amount;
          if (fromBal <= 0) token.holders.delete(tx.from); else token.holders.set(tx.from, fromBal);
          token.holders.set(tx.to, toBal);
        }

        processed.push(tx);
      } else {
        tx.status = 'failed';
        processed.push(tx);
      }
    }

    const merkleRoot = this._computeMerkleRoot(processed);
    const blockData = `${prev.hash}${merkleRoot}${Date.now()}${validator}`;
    let nonce = 0;
    let hash;
    do {
      hash = this._hash(blockData + nonce);
      nonce++;
    } while (!hash.startsWith('0'.repeat(prev.difficulty)));

    const block = {
      height: this.chain.length,
      hash,
      previousHash: prev.hash,
      timestamp: new Date().toISOString(),
      transactions: processed.map(tx => tx.hash),
      txCount: processed.length,
      merkleRoot,
      validator,
      nonce,
      difficulty: prev.difficulty,
      size: JSON.stringify(processed).length,
      gasUsed,
      gasLimit: 8000000
    };

    this.chain.push(block);

    // Index transactions
    for (const tx of processed) {
      this.txIndex.set(tx.hash, { tx, blockHash: block.hash, blockHeight: block.height });
    }

    // Reward validator
    if (this.validators.has(validator)) {
      const v = this.validators.get(validator);
      v.blocks++;
      v.lastSeen = block.timestamp;
    }

    this._emit('new_block', block);
    return block;
  }

  getBlock(hashOrHeight) {
    if (typeof hashOrHeight === 'number') {
      return this.chain[hashOrHeight] || null;
    }
    return this.chain.find(b => b.hash === hashOrHeight) || null;
  }

  getLatestBlocks(limit = 10) {
    return this.chain.slice(-limit).reverse();
  }

  getBlockByHeight(height) {
    return this.chain[height] || null;
  }

  // ── Token Registry ───────────────────────────────────────

  registerToken(symbol, name, totalSupply, decimals, ownerAddress) {
    if (this.tokenRegistry.has(symbol)) throw new Error(`Token ${symbol} already exists`);
    if (!this.wallets.has(ownerAddress)) throw new Error('Owner wallet not found');

    const token = {
      name,
      symbol,
      totalSupply,
      decimals: decimals || 18,
      owner: ownerAddress,
      holders: new Map([[ownerAddress, totalSupply]]),
      created: new Date().toISOString(),
      type: 'IGT' // Indigenous Governance Token
    };

    this.tokenRegistry.set(symbol, token);

    // Credit owner
    const wallet = this.wallets.get(ownerAddress);
    wallet.balance += totalSupply;

    return { symbol, name, totalSupply, owner: ownerAddress, type: 'IGT' };
  }

  getToken(symbol) {
    const token = this.tokenRegistry.get(symbol);
    if (!token) return null;
    return {
      name: token.name,
      symbol: token.symbol,
      totalSupply: token.totalSupply,
      decimals: token.decimals,
      owner: token.owner,
      holders: token.holders.size,
      created: token.created,
      type: token.type
    };
  }

  listTokens() {
    const tokens = [];
    for (const [symbol, token] of this.tokenRegistry) {
      tokens.push({
        symbol,
        name: token.name,
        totalSupply: token.totalSupply,
        holders: token.holders.size,
        type: token.type
      });
    }
    return tokens;
  }

  // ── Validators ───────────────────────────────────────────

  registerValidator(address, stake) {
    if (!this.wallets.has(address)) throw new Error('Wallet not found');
    const wallet = this.wallets.get(address);
    if (wallet.balance < stake) throw new Error('Insufficient stake');

    wallet.balance -= stake;

    this.validators.set(address, {
      address,
      stake,
      blocks: 0,
      uptime: 100,
      lastSeen: new Date().toISOString(),
      registered: new Date().toISOString(),
      status: 'active'
    });

    return this.validators.get(address);
  }

  getValidators() {
    const vals = [];
    for (const [addr, data] of this.validators) {
      vals.push({ address: addr, ...data });
    }
    return vals.sort((a, b) => b.stake - a.stake);
  }

  // ── Network Stats ────────────────────────────────────────

  getNetworkStats() {
    const totalTx = this.txIndex.size + this.pendingTx.length;
    const totalBlocks = this.chain.length;
    const latestBlock = this.chain[this.chain.length - 1];

    let totalStake = 0;
    for (const v of this.validators.values()) totalStake += v.stake;

    return {
      chainId: 'mameynode-mainnet',
      version: '4.2.0',
      totalBlocks,
      totalTransactions: totalTx,
      pendingTransactions: this.pendingTx.length,
      totalWallets: this.wallets.size,
      totalTokens: this.tokenRegistry.size,
      totalValidators: this.validators.size,
      totalStake,
      latestBlock: {
        height: latestBlock.height,
        hash: latestBlock.hash,
        timestamp: latestBlock.timestamp
      },
      gasPrice: 1,
      blockTime: BLOCK_INTERVAL_MS / 1000,
      nativeToken: NATIVE_TOKEN
    };
  }

  // ── Events ───────────────────────────────────────────────

  on(event, callback) {
    this.listeners.push({ event, callback });
  }

  _emit(event, data) {
    for (const l of this.listeners) {
      if (l.event === event) {
        try { l.callback(data); } catch (_) { /* ignore */ }
      }
    }
  }

  // ── Export for testing ───────────────────────────────────

  reset() {
    this.chain = [];
    this.pendingTx = [];
    this.wallets.clear();
    this.txIndex.clear();
    this.tokenRegistry.clear();
    this.validators.clear();
    this.listeners = [];
    this._initGenesis();
    this._registerNativeToken();
  }
}

module.exports = { Blockchain, NATIVE_TOKEN, BLOCK_INTERVAL_MS };
