'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Platform Identification Middleware v1.0.0
// Extracts and validates the target platform context
// ============================================================

const { AppError } = require('../../../shared/error-handler');
const { createLogger } = require('../../../shared/logger');

const log = createLogger('sovereign-core:platform-middleware');

// Allowed platform identifiers (slugified names)
// These correspond to the 18 NEXUS mega-portals and their sub-platforms
const VALID_PLATFORM_PATTERN = /^[a-z0-9][a-z0-9\-]{1,63}$/;

/**
 * Express middleware: identify the target platform from request context
 *
 * Resolution order:
 *   1. URL parameter :platform (e.g., /v1/platforms/:platform/content)
 *   2. X-Platform request header
 *   3. Query parameter ?platform=...
 *
 * The resolved platform slug is attached to req.platform
 */
function platformMiddleware(req, res, next) {
  // 1. From URL params (highest priority)
  let platform = req.params.platform;

  // 2. From custom header
  if (!platform) {
    platform = req.headers['x-platform'];
  }

  // 3. From query string
  if (!platform) {
    platform = req.query.platform;
  }

  // Normalize
  if (platform) {
    platform = platform.toLowerCase().trim();
  }

  // Validate format if present
  if (platform && !VALID_PLATFORM_PATTERN.test(platform)) {
    return next(new AppError('INVALID_INPUT', `Invalid platform identifier: "${platform}". Must be 2-64 lowercase alphanumeric characters and hyphens.`));
  }

  // Attach to request
  req.platform = platform || null;

  // Log for traceability
  if (platform) {
    log.debug('Platform context resolved', { platform, source: req.params.platform ? 'url' : req.headers['x-platform'] ? 'header' : 'query' });
  }

  next();
}

/**
 * Middleware that requires a valid platform to be present
 * Use on routes where platform is mandatory
 */
function requirePlatform(req, res, next) {
  if (!req.platform) {
    return next(new AppError('MISSING_FIELD', 'Platform identifier is required. Provide via :platform URL param, X-Platform header, or ?platform query parameter.'));
  }
  next();
}

// ============================================================
// Exports
// ============================================================
module.exports = {
  platformMiddleware,
  requirePlatform,
  VALID_PLATFORM_PATTERN
};
