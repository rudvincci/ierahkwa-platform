'use strict';

// ============================================================
// Map Routes — /v1/map
// Reverse proxy to Sovereign Core (map content)
// Downstream: sovereign-core:3050/v1/content?type=map
// ============================================================

const { Router } = require('express');
const { proxyRequest } = require('../lib/proxy');
const { createLogger } = require('../../shared/logger');

const router = Router();
const log = createLogger('map-proxy');

const SERVICE_URL = process.env.SOVEREIGN_CORE_URL || 'http://sovereign-core:3050';

router.all('/*', (req, res) => {
  log.info('Proxy → sovereign-core (map)', { method: req.method, path: req.url });
  proxyRequest(req, res, SERVICE_URL, {
    pathPrefix: '/v1/content',
    extraHeaders: { 'x-content-type': 'map' }
  });
});

module.exports = router;
