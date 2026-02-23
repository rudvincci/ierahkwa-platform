require('dotenv').config();
const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const compression = require('compression');
const { createServer } = require('http');
const { WebSocketServer } = require('ws');
const routes = require('./routes');
const { logger } = require('./utils/logger');
const { rateLimiter } = require('./middleware/rateLimiter');
const { swaggerDocs } = require('./utils/swagger');

const app = express();
const server = createServer(app);
const wss = new WebSocketServer({ server, path: '/ws' });

// Middleware
app.use(helmet());
app.use(cors({ origin: process.env.CORS_ORIGINS?.split(',') || '*' }));
app.use(compression());
app.use(express.json({ limit: '10mb' }));
app.use(rateLimiter);

// Health
app.get('/health', (req, res) => res.json({ status: 'ok', version: '1.0.0', blockchain: 'MameyNode v4.2' }));
app.get('/ready', (req, res) => res.json({ ready: true, services: { db: true, redis: true, mameynode: true } }));

// API Routes
app.use('/v1', routes);

// Swagger
swaggerDocs(app);

// WebSocket
wss.on('connection', (ws) => {
  ws.on('message', (data) => {
    const msg = JSON.parse(data);
    // Broadcast to room
    wss.clients.forEach(c => { if (c !== ws && c.readyState === 1) c.send(JSON.stringify(msg)); });
  });
});

// Error handler
app.use((err, req, res, next) => {
  logger.error(err.stack);
  res.status(500).json({ error: 'Internal server error', code: 'SOBERANO_500' });
});

const PORT = process.env.PORT || 3000;
server.listen(PORT, () => {
  logger.info(`🌐 Red Soberana API running on port ${PORT}`);
  logger.info(`⛓ MameyNode: ${process.env.MAMEYNODE_RPC || 'http://localhost:8545'}`);
  logger.info(`📖 Swagger: http://localhost:${PORT}/api-docs`);
});

module.exports = { app, server };
