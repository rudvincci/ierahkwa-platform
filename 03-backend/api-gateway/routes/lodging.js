'use strict';

// ============================================================
// Lodging Routes — /v1/lodging
// Reverse proxy to Reservas service
// Downstream: reservas:3005
// ============================================================

const { Router } = require('express');
const { proxyRequest } = require('../lib/proxy');
const { createLogger } = require('../../shared/logger');

const router = Router();
const log = createLogger('lodging-proxy');

const SERVICE_URL = process.env.RESERVAS_SERVICE_URL || 'http://reservas:3005';

router.all('/*', (req, res) => {
  log.info('Proxy → reservas', { method: req.method, path: req.url });
  proxyRequest(req, res, SERVICE_URL);
});

module.exports = router;
