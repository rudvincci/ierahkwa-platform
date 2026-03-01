'use strict';

// ============================================================
// Social Routes — /v1/social
// Reverse proxy to Red Social service
// Downstream: red-social:3003
// ============================================================

const { Router } = require('express');
const { proxyRequest } = require('../lib/proxy');
const { createLogger } = require('../../shared/logger');

const router = Router();
const log = createLogger('social-proxy');

const SERVICE_URL = process.env.SOCIAL_SERVICE_URL || 'http://red-social:3003';

router.all('/*', (req, res) => {
  log.info('Proxy → red-social', { method: req.method, path: req.url });
  proxyRequest(req, res, SERVICE_URL);
});

module.exports = router;
