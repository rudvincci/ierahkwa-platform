'use strict';

// ============================================================
// Invest Routes — /v1/invest
// Reverse proxy to BDET Bank (investment module)
// Downstream: bdet-bank:3001
// ============================================================

const { Router } = require('express');
const { proxyRequest } = require('../lib/proxy');
const { createLogger } = require('../../shared/logger');

const router = Router();
const log = createLogger('invest-proxy');

const SERVICE_URL = process.env.BDET_SERVICE_URL || 'http://bdet-bank:3001';

router.all('/*', (req, res) => {
  log.info('Proxy → bdet-bank (invest)', { method: req.method, path: req.url });
  proxyRequest(req, res, SERVICE_URL, {
    pathPrefix: '/invest'
  });
});

module.exports = router;
