'use strict';

require('dotenv').config();
const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const compression = require('compression');
const { createServer } = require('http');
const { WebSocketServer } = require('ws');

const createRoutes = require('./routes');
const { logger } = require('./utils/logger');
const { rateLimiter } = require('./middleware/rateLimiter');
const { swaggerDocs } = require('./utils/swagger');
const { Blockchain, BLOCK_INTERVAL_MS } = require('./lib/blockchain');
const { Governance } = require('./lib/governance');

// ── Initialize Engines ──────────────────────────────────

const blockchain = new Blockchain();
const governance = new Governance(blockchain);

// ── Express App ─────────────────────────────────────────

const app = express();
const server = createServer(app);
const wss = new WebSocketServer({ server, path: '/ws' });

// Middleware
app.use(helmet());
try {
  const corsOptions = require('../shared/security').corsConfig();
  app.use(cors(corsOptions));
} catch (_) {
  app.use(cors());
}
app.use(compression());
app.use(express.json({ limit: '10mb' }));
app.use(rateLimiter);

// Health
app.get('/health', (req, res) => res.json({
  status: 'ok',
  version: '1.0.0',
  blockchain: 'MameyNode v4.2',
  chainHeight: blockchain.chain.length,
  wallets: blockchain.wallets.size,
  pendingTx: blockchain.pendingTx.length
}));

app.get('/ready', (req, res) => res.json({
  ready: true,
  services: {
    blockchain: blockchain.chain.length > 0,
    governance: true,
    websocket: wss.clients !== undefined
  }
}));

// API Routes
app.use('/v1', createRoutes(blockchain, governance));

// Swagger
swaggerDocs(app);

// ── WebSocket ───────────────────────────────────────────

function broadcast(type, data) {
  const msg = JSON.stringify({ type, data, timestamp: new Date().toISOString() });
  wss.clients.forEach(client => {
    if (client.readyState === 1) client.send(msg);
  });
}

wss.on('connection', (ws) => {
  logger.info('WebSocket client connected');

  // Send current chain state on connect
  ws.send(JSON.stringify({
    type: 'sync',
    data: blockchain.getNetworkStats()
  }));

  ws.on('message', (raw) => {
    try {
      const msg = JSON.parse(raw);
      if (msg.type === 'subscribe') {
        ws._subscriptions = msg.channels || ['blocks', 'transactions'];
      }
    } catch (_) { /* ignore malformed */ }
  });

  ws.on('close', () => logger.info('WebSocket client disconnected'));
});

// Broadcast new blocks and transactions
blockchain.on('new_block', (block) => broadcast('new_block', block));
blockchain.on('pending_tx', (tx) => broadcast('pending_tx', { hash: tx.hash, from: tx.from, to: tx.to, amount: tx.amount }));

// ── Auto-mine (development) ─────────────────────────────

if (process.env.NODE_ENV !== 'production') {
  setInterval(() => {
    if (blockchain.pendingTx.length > 0) {
      const block = blockchain.mineBlock('auto-miner');
      logger.info(`Auto-mined block #${block.height} with ${block.txCount} tx`);
    }
  }, BLOCK_INTERVAL_MS).unref();
}

// ── Error Handler ───────────────────────────────────────

app.use((err, req, res, _next) => {
  logger.error(err.stack || err.message);
  res.status(500).json({ error: 'Internal server error', code: 'SOBERANO_500' });
});

// ── Start ───────────────────────────────────────────────

const PORT = process.env.PORT || 3000;
server.listen(PORT, () => {
  logger.info(`Red Soberana Blockchain API running on port ${PORT}`);
  logger.info(`MameyNode RPC: ${process.env.MAMEYNODE_RPC || 'http://localhost:8545'}`);
  logger.info(`Swagger: http://localhost:${PORT}/api-docs`);
  logger.info(`WebSocket: ws://localhost:${PORT}/ws`);
  logger.info(`Chain height: ${blockchain.chain.length}, Wallets: ${blockchain.wallets.size}`);
});

module.exports = { app, server, blockchain, governance };
