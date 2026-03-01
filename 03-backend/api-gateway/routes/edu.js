'use strict';

// ============================================================
// Education Routes — /v1/edu
// Reverse proxy to Smart School service
// Downstream: smart-school:3500
// ============================================================

const { Router } = require('express');
const { proxyRequest } = require('../lib/proxy');
const { createLogger } = require('../../shared/logger');

const router = Router();
const log = createLogger('edu-proxy');

const SERVICE_URL = process.env.EDU_SERVICE_URL || 'http://smart-school:3500';

router.all('/*', (req, res) => {
  log.info('Proxy → smart-school', { method: req.method, path: req.url });
  proxyRequest(req, res, SERVICE_URL);
});

module.exports = router;
