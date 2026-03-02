'use strict';

const { Blockchain, NATIVE_TOKEN } = require('../lib/blockchain');

describe('Blockchain Engine', () => {
  let chain;

  beforeEach(() => {
    chain = new Blockchain();
  });

  // ── Genesis ──────────────────────────────────────────

  describe('genesis', () => {
    test('starts with genesis block', () => {
      expect(chain.chain.length).toBe(1);
      expect(chain.chain[0].height).toBe(0);
      expect(chain.chain[0].previousHash).toBe('0'.repeat(64));
    });

    test('has sovereign-treasury wallet with initial supply', () => {
      const treasury = chain.getWallet('sovereign-treasury');
      expect(treasury).not.toBeNull();
      expect(treasury.balance).toBe(1000000000);
    });

    test('has native WMP token registered', () => {
      const token = chain.getToken(NATIVE_TOKEN);
      expect(token).not.toBeNull();
      expect(token.symbol).toBe('WMP');
      expect(token.totalSupply).toBe(1000000000);
    });
  });

  // ── Wallets ──────────────────────────────────────────

  describe('wallets', () => {
    test('createWallet returns address and privateKey', () => {
      const wallet = chain.createWallet('Test Wallet');
      expect(wallet.address).toMatch(/^0x[a-f0-9]{40}$/);
      expect(wallet.privateKey).toHaveLength(64);
      expect(wallet.label).toBe('Test Wallet');
    });

    test('getWallet returns null for unknown address', () => {
      expect(chain.getWallet('0xnonexistent')).toBeNull();
    });

    test('listWallets returns paginated results', () => {
      chain.createWallet('W1');
      chain.createWallet('W2');
      const result = chain.listWallets(1, 10);
      expect(result.total).toBeGreaterThanOrEqual(3); // treasury + 2
      expect(result.wallets.length).toBeGreaterThanOrEqual(3);
    });
  });

  // ── Transactions ─────────────────────────────────────

  describe('transactions', () => {
    let sender, receiver;

    beforeEach(() => {
      sender = chain.createWallet('Sender');
      receiver = chain.createWallet('Receiver');
      // Fund sender from treasury
      chain.createTransaction('sovereign-treasury', sender.address, 10000);
      chain.mineBlock();
    });

    test('createTransaction adds to pending pool', () => {
      chain.createTransaction(sender.address, receiver.address, 100);
      expect(chain.getPendingTransactions().length).toBe(1);
    });

    test('rejects transfer with insufficient balance', () => {
      expect(() => {
        chain.createTransaction(sender.address, receiver.address, 999999999);
      }).toThrow('Insufficient balance');
    });

    test('rejects transfer to unknown wallet', () => {
      expect(() => {
        chain.createTransaction(sender.address, '0xunknown', 100);
      }).toThrow('Recipient wallet not found');
    });

    test('rejects non-positive amounts', () => {
      expect(() => {
        chain.createTransaction(sender.address, receiver.address, 0);
      }).toThrow('Amount must be positive');
    });

    test('getTransaction returns pending tx', () => {
      const tx = chain.createTransaction(sender.address, receiver.address, 50);
      const found = chain.getTransaction(tx.hash);
      expect(found).not.toBeNull();
      expect(found.status).toBe('pending');
      expect(found.confirmations).toBe(0);
    });

    test('getTransaction returns confirmed tx after mining', () => {
      const tx = chain.createTransaction(sender.address, receiver.address, 50);
      chain.mineBlock();
      const found = chain.getTransaction(tx.hash);
      expect(found.status).toBe('confirmed');
      expect(found.confirmations).toBeGreaterThanOrEqual(0);
    });

    test('getTransaction returns null for unknown hash', () => {
      expect(chain.getTransaction('0xnotreal')).toBeNull();
    });

    test('getWalletHistory returns transactions for address', () => {
      chain.createTransaction(sender.address, receiver.address, 50);
      chain.mineBlock();
      const history = chain.getWalletHistory(sender.address);
      expect(history.length).toBeGreaterThan(0);
    });
  });

  // ── Mining ───────────────────────────────────────────

  describe('mining', () => {
    test('mineBlock creates new block linked to previous', () => {
      const prevHash = chain.chain[chain.chain.length - 1].hash;
      chain.createWallet('miner-wallet');
      const block = chain.mineBlock('miner-1');
      expect(block.height).toBe(1);
      expect(block.previousHash).toBe(prevHash);
    });

    test('mineBlock processes pending transactions', () => {
      const w1 = chain.createWallet('A');
      const w2 = chain.createWallet('B');
      chain.createTransaction('sovereign-treasury', w1.address, 5000);
      chain.mineBlock();

      chain.createTransaction(w1.address, w2.address, 100);
      expect(chain.pendingTx.length).toBe(1);

      const block = chain.mineBlock();
      expect(block.txCount).toBe(1);
      expect(chain.pendingTx.length).toBe(0);
    });

    test('mining updates wallet balances', () => {
      const w1 = chain.createWallet('Sender');
      const w2 = chain.createWallet('Receiver');
      chain.createTransaction('sovereign-treasury', w1.address, 1000);
      chain.mineBlock();

      chain.createTransaction(w1.address, w2.address, 300);
      chain.mineBlock();

      expect(chain.getWallet(w1.address).balance).toBe(700);
      expect(chain.getWallet(w2.address).balance).toBe(300);
    });

    test('getLatestBlocks returns blocks in reverse order', () => {
      chain.mineBlock();
      chain.mineBlock();
      const blocks = chain.getLatestBlocks(3);
      expect(blocks[0].height).toBeGreaterThan(blocks[1].height);
    });

    test('getBlock by height works', () => {
      const block = chain.getBlock(0);
      expect(block.height).toBe(0);
    });
  });

  // ── Tokens ───────────────────────────────────────────

  describe('tokens', () => {
    test('registerToken creates new token', () => {
      const w = chain.createWallet('Token Owner');
      chain.createTransaction('sovereign-treasury', w.address, 50000);
      chain.mineBlock();

      const token = chain.registerToken('EAGLE', 'Eagle Token', 1000000, 18, w.address);
      expect(token.symbol).toBe('EAGLE');
      expect(token.type).toBe('IGT');
    });

    test('rejects duplicate token symbol', () => {
      const w = chain.createWallet('Owner');
      chain.createTransaction('sovereign-treasury', w.address, 50000);
      chain.mineBlock();

      chain.registerToken('DUP', 'First', 1000, 18, w.address);
      expect(() => chain.registerToken('DUP', 'Second', 2000, 18, w.address)).toThrow('already exists');
    });

    test('listTokens includes native token', () => {
      const tokens = chain.listTokens();
      expect(tokens.some(t => t.symbol === 'WMP')).toBe(true);
    });
  });

  // ── Validators ───────────────────────────────────────

  describe('validators', () => {
    test('registerValidator deducts stake from balance', () => {
      const w = chain.createWallet('Validator');
      chain.createTransaction('sovereign-treasury', w.address, 50000);
      chain.mineBlock();

      chain.registerValidator(w.address, 10000);
      expect(chain.getWallet(w.address).balance).toBe(40000);
    });

    test('getValidators returns sorted by stake', () => {
      const w1 = chain.createWallet('V1');
      const w2 = chain.createWallet('V2');
      chain.createTransaction('sovereign-treasury', w1.address, 50000);
      chain.createTransaction('sovereign-treasury', w2.address, 80000);
      chain.mineBlock();

      chain.registerValidator(w1.address, 10000);
      chain.registerValidator(w2.address, 30000);

      const vals = chain.getValidators();
      expect(vals[0].stake).toBeGreaterThan(vals[1].stake);
    });

    test('rejects insufficient stake', () => {
      const w = chain.createWallet('Poor');
      expect(() => chain.registerValidator(w.address, 10000)).toThrow('Insufficient stake');
    });
  });

  // ── Network Stats ────────────────────────────────────

  describe('network stats', () => {
    test('getNetworkStats returns comprehensive data', () => {
      const stats = chain.getNetworkStats();
      expect(stats.chainId).toBe('mameynode-mainnet');
      expect(stats.version).toBe('4.2.0');
      expect(stats.totalBlocks).toBeGreaterThanOrEqual(1);
      expect(stats.nativeToken).toBe('WMP');
    });
  });

  // ── Reset ────────────────────────────────────────────

  describe('reset', () => {
    test('reset restores to initial state', () => {
      chain.createWallet('temp');
      chain.mineBlock();
      chain.reset();
      expect(chain.chain.length).toBe(1);
      expect(chain.wallets.size).toBe(1); // treasury only
    });
  });
});
