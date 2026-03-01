'use strict';

// ============================================================
// Commerce Routes — /v1/commerce
// Reverse proxy to Ierahkwa Shop service
// Downstream: ierahkwa-shop:3100
// ============================================================

const { Router } = require('express');
const { proxyRequest } = require('../lib/proxy');
const { createLogger } = require('../../shared/logger');

const router = Router();
const log = createLogger('commerce-proxy');

const SERVICE_URL = process.env.COMMERCE_SERVICE_URL || 'http://ierahkwa-shop:3100';

router.all('/*', (req, res) => {
  log.info('Proxy → ierahkwa-shop', { method: req.method, path: req.url });
  proxyRequest(req, res, SERVICE_URL);
});

module.exports = router;
