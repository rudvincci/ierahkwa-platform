'use strict';

const { Router } = require('express');

module.exports = function createRoutes(blockchain, governance) {
  const router = Router();

  // ── Blocks ─────────────────────────────────────────────

  router.get('/blocks', (req, res) => {
    const limit = Math.min(parseInt(req.query.limit) || 10, 100);
    res.json({ blocks: blockchain.getLatestBlocks(limit) });
  });

  router.get('/blocks/:ref', (req, res) => {
    const ref = req.params.ref;
    const block = /^\d+$/.test(ref)
      ? blockchain.getBlockByHeight(parseInt(ref))
      : blockchain.getBlock(ref);
    if (!block) return res.status(404).json({ error: 'Block not found' });
    res.json(block);
  });

  // ── Transactions ───────────────────────────────────────

  router.get('/transactions/pending', (req, res) => {
    res.json({ transactions: blockchain.getPendingTransactions() });
  });

  router.get('/transactions/:hash', (req, res) => {
    const tx = blockchain.getTransaction(req.params.hash);
    if (!tx) return res.status(404).json({ error: 'Transaction not found' });
    res.json(tx);
  });

  router.post('/transactions', (req, res) => {
    try {
      const { from, to, amount, memo, token, gasPrice, gasLimit } = req.body;
      if (!from || !to || !amount) {
        return res.status(400).json({ error: 'Missing required fields: from, to, amount' });
      }
      const tx = blockchain.createTransaction(from, to, Number(amount), {
        memo, token, gasPrice, gasLimit, type: 'transfer'
      });
      res.status(201).json(tx);
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  // ── Wallets ────────────────────────────────────────────

  router.get('/wallets', (req, res) => {
    const page = parseInt(req.query.page) || 1;
    const limit = Math.min(parseInt(req.query.limit) || 20, 100);
    res.json(blockchain.listWallets(page, limit));
  });

  router.post('/wallets', (req, res) => {
    try {
      const { label } = req.body || {};
      const wallet = blockchain.createWallet(label);
      res.status(201).json(wallet);
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  router.get('/wallets/:address', (req, res) => {
    const wallet = blockchain.getWallet(req.params.address);
    if (!wallet) return res.status(404).json({ error: 'Wallet not found' });
    res.json(wallet);
  });

  router.get('/wallets/:address/history', (req, res) => {
    const limit = Math.min(parseInt(req.query.limit) || 50, 200);
    const history = blockchain.getWalletHistory(req.params.address, limit);
    res.json({ address: req.params.address, transactions: history, count: history.length });
  });

  // ── Tokens ─────────────────────────────────────────────

  router.get('/tokens', (req, res) => {
    res.json({ tokens: blockchain.listTokens() });
  });

  router.get('/tokens/:symbol', (req, res) => {
    const token = blockchain.getToken(req.params.symbol.toUpperCase());
    if (!token) return res.status(404).json({ error: 'Token not found' });
    res.json(token);
  });

  router.post('/tokens', (req, res) => {
    try {
      const { symbol, name, totalSupply, decimals, owner } = req.body;
      if (!symbol || !name || !totalSupply || !owner) {
        return res.status(400).json({ error: 'Missing required fields: symbol, name, totalSupply, owner' });
      }
      const token = blockchain.registerToken(
        symbol.toUpperCase(), name, Number(totalSupply), decimals || 18, owner
      );
      res.status(201).json(token);
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  // ── Validators ─────────────────────────────────────────

  router.get('/validators', (req, res) => {
    res.json({ validators: blockchain.getValidators() });
  });

  router.post('/validators', (req, res) => {
    try {
      const { address, stake } = req.body;
      if (!address || !stake) {
        return res.status(400).json({ error: 'Missing required fields: address, stake' });
      }
      const validator = blockchain.registerValidator(address, Number(stake));
      res.status(201).json(validator);
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  // ── Governance ─────────────────────────────────────────

  router.get('/governance/proposals', (req, res) => {
    const { state, page, limit } = req.query;
    res.json(governance.listProposals(state, parseInt(page) || 1, parseInt(limit) || 20));
  });

  router.get('/governance/proposals/:id', (req, res) => {
    const proposal = governance.getProposal(req.params.id);
    if (!proposal) return res.status(404).json({ error: 'Proposal not found' });
    res.json(proposal);
  });

  router.post('/governance/proposals', (req, res) => {
    try {
      const { author, title, description, category, actions } = req.body;
      if (!author || !title || !description) {
        return res.status(400).json({ error: 'Missing required fields: author, title, description' });
      }
      const proposal = governance.createProposal(author, title, description, category, actions);
      res.status(201).json(proposal);
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  router.post('/governance/proposals/:id/vote', (req, res) => {
    try {
      const { voter, choice } = req.body;
      if (!voter || !choice) {
        return res.status(400).json({ error: 'Missing required fields: voter, choice' });
      }
      const result = governance.vote(req.params.id, voter, choice);
      res.json(result);
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  router.post('/governance/proposals/:id/tally', (req, res) => {
    try {
      const result = governance.tallyProposal(req.params.id);
      res.json(result);
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  router.post('/governance/proposals/:id/execute', (req, res) => {
    try {
      const result = governance.executeProposal(req.params.id);
      res.json(result);
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  router.get('/governance/proposals/:id/votes', (req, res) => {
    const votes = governance.getVotes(req.params.id);
    res.json({ proposalId: req.params.id, votes, count: votes.length });
  });

  // ── Nodes / Network ────────────────────────────────────

  router.get('/nodes/stats', (req, res) => {
    res.json(blockchain.getNetworkStats());
  });

  // ── Mine (development/testing) ─────────────────────────

  router.post('/mine', (req, res) => {
    try {
      const { validator } = req.body || {};
      const block = blockchain.mineBlock(validator);
      res.status(201).json(block);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  return router;
};
