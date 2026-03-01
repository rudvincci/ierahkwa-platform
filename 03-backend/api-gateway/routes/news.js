'use strict';

// ============================================================
// News Routes — /v1/news
// Reverse proxy to Sovereign Core (news content)
// Downstream: sovereign-core:3050/v1/content?type=news
// ============================================================

const { Router } = require('express');
const { proxyRequest } = require('../lib/proxy');
const { createLogger } = require('../../shared/logger');

const router = Router();
const log = createLogger('news-proxy');

const SERVICE_URL = process.env.SOVEREIGN_CORE_URL || 'http://sovereign-core:3050';

router.all('/*', (req, res) => {
  log.info('Proxy → sovereign-core (news)', { method: req.method, path: req.url });
  proxyRequest(req, res, SERVICE_URL, {
    pathPrefix: '/v1/content',
    extraHeaders: { 'x-content-type': 'news' }
  });
});

module.exports = router;
