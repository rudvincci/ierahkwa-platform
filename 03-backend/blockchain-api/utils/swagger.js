'use strict';

function swaggerDocs(app) {
  const spec = {
    openapi: '3.0.3',
    info: {
      title: 'Red Soberana Blockchain API',
      version: '1.0.0',
      description: 'MameyNode v4.2 — Sovereign blockchain for Ierahkwa Ne Kanienke'
    },
    servers: [{ url: '/v1', description: 'API v1' }],
    paths: {
      '/blocks': { get: { summary: 'List recent blocks', tags: ['Blocks'] } },
      '/blocks/{ref}': { get: { summary: 'Get block by hash or height', tags: ['Blocks'] } },
      '/transactions': { post: { summary: 'Submit transaction', tags: ['Transactions'] } },
      '/transactions/pending': { get: { summary: 'List pending transactions', tags: ['Transactions'] } },
      '/transactions/{hash}': { get: { summary: 'Get transaction by hash', tags: ['Transactions'] } },
      '/wallets': {
        get: { summary: 'List wallets', tags: ['Wallets'] },
        post: { summary: 'Create wallet', tags: ['Wallets'] }
      },
      '/wallets/{address}': { get: { summary: 'Get wallet by address', tags: ['Wallets'] } },
      '/wallets/{address}/history': { get: { summary: 'Get wallet transaction history', tags: ['Wallets'] } },
      '/tokens': {
        get: { summary: 'List tokens', tags: ['Tokens'] },
        post: { summary: 'Register new token', tags: ['Tokens'] }
      },
      '/tokens/{symbol}': { get: { summary: 'Get token info', tags: ['Tokens'] } },
      '/validators': {
        get: { summary: 'List validators', tags: ['Validators'] },
        post: { summary: 'Register validator', tags: ['Validators'] }
      },
      '/governance/proposals': {
        get: { summary: 'List governance proposals', tags: ['Governance'] },
        post: { summary: 'Create proposal', tags: ['Governance'] }
      },
      '/governance/proposals/{id}': { get: { summary: 'Get proposal details', tags: ['Governance'] } },
      '/governance/proposals/{id}/vote': { post: { summary: 'Vote on proposal', tags: ['Governance'] } },
      '/governance/proposals/{id}/tally': { post: { summary: 'Tally proposal votes', tags: ['Governance'] } },
      '/governance/proposals/{id}/execute': { post: { summary: 'Execute passed proposal', tags: ['Governance'] } },
      '/nodes/stats': { get: { summary: 'Network statistics', tags: ['Network'] } },
      '/mine': { post: { summary: 'Mine a block (dev only)', tags: ['Mining'] } }
    }
  };

  app.get('/api-docs', (req, res) => res.json(spec));
}

module.exports = { swaggerDocs };
