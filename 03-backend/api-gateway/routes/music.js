'use strict';

// ============================================================
// Music Routes — /v1/music
// Reverse proxy to Sovereign Core (music content)
// Downstream: sovereign-core:3050/v1/content?type=music
// ============================================================

const { Router } = require('express');
const { proxyRequest } = require('../lib/proxy');
const { createLogger } = require('../../shared/logger');

const router = Router();
const log = createLogger('music-proxy');

const SERVICE_URL = process.env.SOVEREIGN_CORE_URL || 'http://sovereign-core:3050';

router.all('/*', (req, res) => {
  log.info('Proxy → sovereign-core (music)', { method: req.method, path: req.url });
  proxyRequest(req, res, SERVICE_URL, {
    pathPrefix: '/v1/content',
    extraHeaders: { 'x-content-type': 'music' }
  });
});

module.exports = router;
