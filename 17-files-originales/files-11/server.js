const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const http = require('http');
const { WebSocketServer } = require('ws');
const rateLimit = require('express-rate-limit');
const logger = require('./utils/logger');

const app = express();
const server = http.createServer(app);
const wss = new WebSocketServer({ server, path: '/ws' });

app.use(helmet());
app.use(cors());
app.use(express.json());
app.use(rateLimit({ windowMs: 60000, max: 200 }));

// ===== 11 ENGINES =====
app.use('/v1/wallet', require('./routes/wallet'));
app.use('/v1/payments', require('./routes/payments'));
app.use('/v1/exchange', require('./routes/exchange'));
app.use('/v1/trading', require('./routes/trading'));
app.use('/v1/remittance', require('./routes/remittance'));
app.use('/v1/escrow', require('./routes/escrow'));
app.use('/v1/loans', require('./routes/loans'));
app.use('/v1/insurance', require('./routes/insurance'));
app.use('/v1/staking', require('./routes/staking'));
app.use('/v1/treasury', require('./routes/treasury'));
app.use('/v1/fiscal', require('./routes/fiscal'));

app.get('/health', (req, res) => res.json({
  bank: 'BDET — Blockchain Digital Exchange Trading Bank',
  version: '4.2.0', engines: 11, status: 'operational',
  currency: 'Wampum (WMP)', supply: '720,000,000',
  blockchain: 'MameyNode v4.2', consensus: 'Proof of Sovereignty',
  taxRate: '0% — Constitutional Article VII',
}));

// WebSocket — live market data + transaction feed
require('./engines/market-feed')(wss);

const PORT = process.env.BDET_PORT || 4000;
server.listen(PORT, () => logger.info(`🏦 BDET Bank live on :${PORT} — 11 engines — 0% taxes`));
module.exports = { app, server, wss };
