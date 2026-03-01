'use strict';

// ============================================================
// Atabey Routes — /v1/atabey
// Reverse proxy to Sovereign Core (AI analytics)
// Downstream: sovereign-core:3050/v1/analytics
// ============================================================

const { Router } = require('express');
const { proxyRequest } = require('../lib/proxy');
const { createLogger } = require('../../shared/logger');

const router = Router();
const log = createLogger('atabey-proxy');

const SERVICE_URL = process.env.SOVEREIGN_CORE_URL || 'http://sovereign-core:3050';

router.all('/*', (req, res) => {
  log.info('Proxy → sovereign-core (atabey analytics)', { method: req.method, path: req.url });
  proxyRequest(req, res, SERVICE_URL, {
    pathPrefix: '/v1/analytics'
  });
});

module.exports = router;
