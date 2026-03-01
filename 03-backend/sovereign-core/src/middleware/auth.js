'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// JWT Authentication Middleware v1.0.0
// Stateless token verification with HMAC-SHA256
// ============================================================

const crypto = require('crypto');
const { AppError } = require('../../../shared/error-handler');
const { createLogger } = require('../../../shared/logger');

const log = createLogger('sovereign-core:auth-middleware');

// Routes that bypass authentication entirely
const PUBLIC_ROUTES = [
  { method: null, path: '/v1/auth/login' },
  { method: null, path: '/v1/auth/register' },
  { method: 'GET', path: '/health' },
  { method: 'GET', path: '/ready' },
  { method: 'GET', path: '/metrics' }
];

/**
 * Encode a JSON payload to URL-safe Base64
 */
function base64url(obj) {
  return Buffer.from(JSON.stringify(obj))
    .toString('base64')
    .replace(/=/g, '')
    .replace(/\+/g, '-')
    .replace(/\//g, '_');
}

/**
 * Decode a URL-safe Base64 string to a JSON object
 */
function base64urlDecode(str) {
  // Restore standard base64
  let base64 = str.replace(/-/g, '+').replace(/_/g, '/');
  const pad = base64.length % 4;
  if (pad) base64 += '='.repeat(4 - pad);
  return JSON.parse(Buffer.from(base64, 'base64').toString('utf8'));
}

/**
 * Generate a JWT token using HMAC-SHA256
 * @param {object} payload - Token payload (user data)
 * @param {string} secret - HMAC secret key
 * @param {number} [expiresInSec=86400] - Expiration in seconds (default 24h)
 * @returns {string} Signed JWT string
 */
function generateToken(payload, secret, expiresInSec = 86400) {
  const header = { alg: 'HS256', typ: 'JWT' };

  const now = Math.floor(Date.now() / 1000);
  const tokenPayload = {
    ...payload,
    iat: now,
    exp: now + expiresInSec,
    iss: 'ierahkwa-sovereign-core'
  };

  const segments = [base64url(header), base64url(tokenPayload)];
  const signingInput = segments.join('.');
  const signature = crypto
    .createHmac('sha256', secret)
    .update(signingInput)
    .digest('base64')
    .replace(/=/g, '')
    .replace(/\+/g, '-')
    .replace(/\//g, '_');

  return `${signingInput}.${signature}`;
}

/**
 * Verify a JWT token using HMAC-SHA256
 * @param {string} token - JWT string
 * @param {string} secret - HMAC secret key
 * @returns {object} Decoded payload
 * @throws {AppError} If token is invalid or expired
 */
function verifyToken(token, secret) {
  const parts = token.split('.');
  if (parts.length !== 3) {
    throw new AppError('AUTH_INVALID_TOKEN', 'Malformed token: expected 3 segments');
  }

  const [headerB64, payloadB64, signatureB64] = parts;

  // Verify signature
  const signingInput = `${headerB64}.${payloadB64}`;
  const expectedSig = crypto
    .createHmac('sha256', secret)
    .update(signingInput)
    .digest('base64')
    .replace(/=/g, '')
    .replace(/\+/g, '-')
    .replace(/\//g, '_');

  // Timing-safe comparison
  const sigBuffer = Buffer.from(signatureB64);
  const expectedBuffer = Buffer.from(expectedSig);

  if (sigBuffer.length !== expectedBuffer.length || !crypto.timingSafeEqual(sigBuffer, expectedBuffer)) {
    throw new AppError('AUTH_INVALID_TOKEN', 'Invalid token signature');
  }

  // Decode payload
  let payload;
  try {
    payload = base64urlDecode(payloadB64);
  } catch {
    throw new AppError('AUTH_INVALID_TOKEN', 'Malformed token payload');
  }

  // Check expiration
  const now = Math.floor(Date.now() / 1000);
  if (payload.exp && payload.exp < now) {
    throw new AppError('AUTH_INVALID_TOKEN', 'Token has expired');
  }

  return payload;
}

/**
 * Check if a route is public (no auth required)
 */
function isPublicRoute(method, path) {
  return PUBLIC_ROUTES.some(route => {
    const methodMatch = route.method === null || route.method === method;
    const pathMatch = path === route.path || path.startsWith(route.path + '/') || path.startsWith(route.path + '?');
    return methodMatch && pathMatch;
  });
}

/**
 * Express middleware: authenticate requests via Bearer JWT
 *
 * In development mode without JWT_SECRET set, accepts mock tokens
 * prefixed with "ik_" for local testing convenience.
 */
function authMiddleware(req, res, next) {
  // Skip public routes
  if (isPublicRoute(req.method, req.path)) {
    return next();
  }

  const authHeader = req.headers.authorization;

  if (!authHeader) {
    return next(new AppError('AUTH_REQUIRED', 'Authorization header is required'));
  }

  if (!authHeader.startsWith('Bearer ')) {
    return next(new AppError('AUTH_INVALID_TOKEN', 'Authorization header must use Bearer scheme'));
  }

  const token = authHeader.slice(7).trim();

  if (!token) {
    return next(new AppError('AUTH_REQUIRED', 'Bearer token is empty'));
  }

  const secret = process.env.JWT_SECRET;

  // DEV MODE: accept mock tokens when JWT_SECRET is not configured
  if (!secret && process.env.NODE_ENV !== 'production') {
    if (token.startsWith('ik_')) {
      log.warn('Dev mode: accepting mock ik_ token (no JWT_SECRET set)');
      // Parse mock token format: ik_<userId>_<role>
      const parts = token.split('_');
      req.user = {
        id: parts[1] || 'dev-user-001',
        email: 'dev@ierahkwa.local',
        role: parts[2] || 'user',
        display_name: 'Dev User',
        nation: 'development',
        iat: Math.floor(Date.now() / 1000),
        iss: 'ierahkwa-dev-mock'
      };
      return next();
    }

    return next(new AppError('AUTH_INVALID_TOKEN', 'JWT_SECRET not configured and token is not a valid dev mock (ik_) token'));
  }

  if (!secret) {
    log.error('JWT_SECRET environment variable is not set in production');
    return next(new AppError('INTERNAL', 'Authentication configuration error'));
  }

  try {
    const decoded = verifyToken(token, secret);

    // Attach user to request
    req.user = {
      id: decoded.sub || decoded.id,
      email: decoded.email,
      role: decoded.role || 'user',
      display_name: decoded.display_name,
      nation: decoded.nation,
      iat: decoded.iat,
      exp: decoded.exp,
      iss: decoded.iss
    };

    next();
  } catch (err) {
    if (err instanceof AppError) {
      return next(err);
    }
    log.error('Token verification failed', { err });
    return next(new AppError('AUTH_INVALID_TOKEN', 'Token verification failed'));
  }
}

/**
 * Role-checking middleware factory
 * Usage: router.post('/admin-only', requireRole('admin'), handler)
 */
function requireRole(...roles) {
  return (req, res, next) => {
    if (!req.user) {
      return next(new AppError('AUTH_REQUIRED', 'Authentication required'));
    }
    if (!roles.includes(req.user.role)) {
      return next(new AppError('AUTH_INSUFFICIENT_ROLE', `Role ${req.user.role} is not authorized. Required: ${roles.join(', ')}`));
    }
    next();
  };
}

// ============================================================
// Exports
// ============================================================
module.exports = {
  authMiddleware,
  generateToken,
  verifyToken,
  requireRole,
  PUBLIC_ROUTES
};
