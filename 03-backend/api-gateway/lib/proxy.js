'use strict';

// ============================================================
// Ierahkwa API Gateway — Reverse Proxy Helper v1.0.0
// Forwards requests to downstream microservices using native
// Node.js http/https modules. Zero external dependencies.
// ============================================================

const http  = require('http');
const https = require('https');
const { AppError } = require('../../shared/error-handler');

/**
 * Default timeout for downstream requests (30 seconds)
 */
const DEFAULT_TIMEOUT = parseInt(process.env.PROXY_TIMEOUT, 10) || 30000;

/**
 * Headers that should NOT be forwarded to downstream services
 */
const HOP_BY_HOP_HEADERS = new Set([
  'connection',
  'keep-alive',
  'proxy-authenticate',
  'proxy-authorization',
  'te',
  'trailer',
  'transfer-encoding',
  'upgrade'
]);

/**
 * Build a clean set of headers to forward downstream.
 * Removes hop-by-hop headers and resets the host header.
 *
 * @param {object} incomingHeaders - req.headers from Express
 * @param {string} targetHost      - hostname of downstream service
 * @returns {object} sanitized headers
 */
function buildForwardHeaders(incomingHeaders, targetHost) {
  const forwarded = {};
  for (const [key, value] of Object.entries(incomingHeaders)) {
    if (!HOP_BY_HOP_HEADERS.has(key.toLowerCase())) {
      forwarded[key] = value;
    }
  }
  forwarded['host'] = targetHost;
  return forwarded;
}

/**
 * Proxy an incoming Express request to a downstream service.
 *
 * @param {import('express').Request}  req        - Express request
 * @param {import('express').Response} res        - Express response
 * @param {string}                     targetBase - Base URL of the downstream service (e.g. 'http://localhost:3001')
 * @param {object}                     [opts]     - Extra options
 * @param {string}                     [opts.pathPrefix]  - Prefix to prepend to the proxied path (e.g. '/v1/messages')
 * @param {object}                     [opts.extraHeaders] - Additional headers to send downstream
 * @param {number}                     [opts.timeout]      - Override request timeout in ms
 */
function proxyRequest(req, res, targetBase, opts = {}) {
  const {
    pathPrefix   = '',
    extraHeaders = {},
    timeout      = DEFAULT_TIMEOUT
  } = opts;

  // Build the downstream URL
  // req.originalUrl includes the mount path from Express, but since the router
  // is mounted at e.g. /v1/commerce, req.url only contains the path AFTER the mount.
  const downstreamPath = pathPrefix + req.url;
  const target = new URL(downstreamPath, targetBase);
  const client = target.protocol === 'https:' ? https : http;

  const headers = {
    ...buildForwardHeaders(req.headers, target.hostname + (target.port ? ':' + target.port : '')),
    ...extraHeaders
  };

  // When body has been parsed by express.json(), we need to serialize it
  let bodyBuffer = null;
  if (req.body && typeof req.body === 'object' && Object.keys(req.body).length > 0) {
    bodyBuffer = Buffer.from(JSON.stringify(req.body));
    headers['content-type']   = 'application/json';
    headers['content-length'] = bodyBuffer.length;
  }

  const options = {
    hostname: target.hostname,
    port:     target.port || (target.protocol === 'https:' ? 443 : 80),
    path:     target.pathname + target.search,
    method:   req.method,
    headers,
    timeout
  };

  const proxyReq = client.request(options, (proxyRes) => {
    // Forward status code
    res.status(proxyRes.statusCode);

    // Forward response headers (skip hop-by-hop)
    for (const [key, value] of Object.entries(proxyRes.headers)) {
      if (!HOP_BY_HOP_HEADERS.has(key.toLowerCase())) {
        res.setHeader(key, value);
      }
    }

    // Pipe the response body
    proxyRes.pipe(res);
  });

  proxyReq.on('timeout', () => {
    proxyReq.destroy();
    if (!res.headersSent) {
      const err = new AppError('SERVICE_UNAVAILABLE', `Downstream service timed out after ${timeout}ms`);
      res.status(504).json({
        type:   'https://ierahkwa.gov/errors/gateway-timeout',
        title:  'Gateway Timeout',
        status: 504,
        detail: err.message
      });
    }
  });

  proxyReq.on('error', (err) => {
    if (!res.headersSent) {
      res.status(502).json({
        type:   'https://ierahkwa.gov/errors/bad-gateway',
        title:  'Bad Gateway',
        status: 502,
        detail: `Downstream service error: ${err.message}`
      });
    }
  });

  // Send the body if present
  if (bodyBuffer) {
    proxyReq.write(bodyBuffer);
  } else if (req.readable && !req.body) {
    // Stream raw body for unparsed requests
    req.pipe(proxyReq);
    return; // pipe will call .end()
  }

  proxyReq.end();
}

/**
 * Create an Express router that proxies all methods/paths to a downstream service.
 * Convenience factory for simple 1:1 proxy routes.
 *
 * @param {string} serviceUrl   - Base URL of the downstream service
 * @param {object} [opts]       - Options forwarded to proxyRequest
 * @returns {import('express').Router}
 */
function createProxyRouter(serviceUrl, opts = {}) {
  const { Router } = require('express');
  const router = Router();

  router.all('/*', (req, res) => {
    proxyRequest(req, res, serviceUrl, opts);
  });

  return router;
}

module.exports = {
  proxyRequest,
  createProxyRouter,
  buildForwardHeaders,
  DEFAULT_TIMEOUT
};
