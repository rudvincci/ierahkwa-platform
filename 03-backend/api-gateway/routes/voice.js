'use strict';

// ============================================================
// Voice Routes — /v1/voice
// Reverse proxy to Voz Soberana service
// Downstream: voz-soberana:3002
// ============================================================

const { Router } = require('express');
const { proxyRequest } = require('../lib/proxy');
const { createLogger } = require('../../shared/logger');

const router = Router();
const log = createLogger('voice-proxy');

const SERVICE_URL = process.env.VOICE_SERVICE_URL || 'http://voz-soberana:3002';

router.all('/*', (req, res) => {
  log.info('Proxy → voz-soberana', { method: req.method, path: req.url });
  proxyRequest(req, res, SERVICE_URL);
});

module.exports = router;
