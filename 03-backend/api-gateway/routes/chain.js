'use strict';

// ============================================================
// Chain Routes — /v1/chain
// Reverse proxy to MameyNode blockchain RPC
// Downstream: mameynode:8545
// ============================================================

const { Router } = require('express');
const { proxyRequest } = require('../lib/proxy');
const { createLogger } = require('../../shared/logger');

const router = Router();
const log = createLogger('chain-proxy');

const SERVICE_URL = process.env.CHAIN_SERVICE_URL || 'http://mameynode:8545';

router.all('/*', (req, res) => {
  log.info('Proxy → mameynode (blockchain)', { method: req.method, path: req.url });
  proxyRequest(req, res, SERVICE_URL);
});

module.exports = router;
