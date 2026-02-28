'use strict';

// ============================================================
// Ierahkwa API Gateway — JWT Authentication Middleware v1.0.0
// RS256 JWT verification with FWID claim extraction
// Supports tier-based access control (citizen, resident, member)
// ============================================================

const crypto = require('crypto');
const { AppError } = require('../../shared/error-handler');
const { createLogger } = require('../../shared/logger');

const log = createLogger('auth-middleware');

/**
 * Routes that bypass JWT authentication
 */
const PUBLIC_ROUTES = [
  { method: 'POST', path: '/v1/auth/login' },
  { method: 'POST', path: '/v1/auth/register' },
  { method: 'GET',  path: '/health' },
  { method: 'GET',  path: '/ready' },
  { method: 'GET',  path: '/metrics' }
];

/**
 * Tier hierarchy — higher index = more access
 */
const TIER_LEVELS = { member: 0, resident: 1, citizen: 2, admin: 3 };

/**
 * Decode base64url string
 */
function base64urlDecode(str) {
  const padded = str.replace(/-/g, '+').replace(/_/g, '/');
  return Buffer.from(padded, 'base64');
}

/**
 * Parse JWT token without external dependencies
 * Supports HS256 (symmetric) and RS256 (asymmetric) verification
 */
function parseJWT(token) {
  const parts = token.split('.');
  if (parts.length !== 3) {
    throw new AppError('AUTH_INVALID_TOKEN', 'Malformed token — expected 3 segments');
  }

  try {
    const header = JSON.parse(base64urlDecode(parts[0]).toString('utf8'));
    const payload = JSON.parse(base64urlDecode(parts[1]).toString('utf8'));
    return { header, payload, signature: parts[2], signedPart: `${parts[0]}.${parts[1]}` };
  } catch {
    throw new AppError('AUTH_INVALID_TOKEN', 'Token contains invalid JSON');
  }
}

/**
 * Verify JWT signature using HS256 (HMAC-SHA256)
 * Used when JWT_SECRET env var is set
 */
function verifyHS256(signedPart, signature, secret) {
  const expected = crypto
    .createHmac('sha256', secret)
    .update(signedPart)
    .digest('base64url');
  return crypto.timingSafeEqual(Buffer.from(expected), Buffer.from(signature));
}

/**
 * Verify JWT signature using RS256 (RSA-SHA256)
 * Used when JWT_PUBLIC_KEY env var is set
 */
function verifyRS256(signedPart, signature, publicKey) {
  const verifier = crypto.createVerify('RSA-SHA256');
  verifier.update(signedPart);
  return verifier.verify(publicKey, base64urlDecode(signature));
}

/**
 * Verify JWT token — supports both HS256 and RS256
 */
function verifyToken(token) {
  const { header, payload, signature, signedPart } = parseJWT(token);

  // Check expiration
  if (payload.exp && Date.now() >= payload.exp * 1000) {
    throw new AppError('AUTH_INVALID_TOKEN', 'Token has expired');
  }

  // Check not-before
  if (payload.nbf && Date.now() < payload.nbf * 1000) {
    throw new AppError('AUTH_INVALID_TOKEN', 'Token not yet valid');
  }

  // Check issuer if configured
  const expectedIssuer = process.env.JWT_ISSUER || 'ierahkwa';
  if (payload.iss && payload.iss !== expectedIssuer) {
    throw new AppError('AUTH_INVALID_TOKEN', 'Invalid token issuer');
  }

  // Verify signature based on algorithm
  const alg = header.alg || 'HS256';

  if (alg === 'RS256') {
    const publicKey = process.env.JWT_PUBLIC_KEY;
    if (!publicKey) {
      log.warn('RS256 token received but JWT_PUBLIC_KEY not configured');
      throw new AppError('INTERNAL', 'Server authentication configuration error');
    }
    if (!verifyRS256(signedPart, signature, publicKey)) {
      throw new AppError('AUTH_INVALID_TOKEN', 'Invalid token signature');
    }
  } else if (alg === 'HS256') {
    const secret = process.env.JWT_SECRET;
    if (!secret) {
      log.warn('HS256 token received but JWT_SECRET not configured');
      throw new AppError('INTERNAL', 'Server authentication configuration error');
    }
    if (!verifyHS256(signedPart, signature, secret)) {
      throw new AppError('AUTH_INVALID_TOKEN', 'Invalid token signature');
    }
  } else {
    throw new AppError('AUTH_INVALID_TOKEN', `Unsupported algorithm: ${alg}`);
  }

  return payload;
}

/**
 * Check if request matches a public route (no auth required)
 */
function isPublicRoute(req) {
  return PUBLIC_ROUTES.some(route =>
    req.method === route.method && req.path === route.path
  );
}

/**
 * Extract Bearer token from Authorization header
 */
function extractToken(req) {
  const header = req.headers.authorization;
  if (!header) return null;

  const [scheme, token] = header.split(' ');
  if (scheme !== 'Bearer' || !token) return null;

  return token;
}

// ============================================================
// Middleware: JWT Authentication
// ============================================================

/**
 * JWT authentication middleware
 * - Skips public routes (login, register, health)
 * - Extracts Bearer token from Authorization header
 * - Verifies signature (HS256 or RS256)
 * - Attaches decoded user to req.user
 *
 * In development mode (JWT_SECRET not set), accepts mock tokens
 * with prefix 'ik_' for local development
 */
function jwtAuth(req, res, next) {
  // Skip public routes
  if (isPublicRoute(req)) return next();

  const token = extractToken(req);

  if (!token) {
    return next(new AppError('AUTH_REQUIRED', 'Bearer token required in Authorization header'));
  }

  // Development mode: accept mock tokens (prefix ik_)
  const isDev = process.env.NODE_ENV !== 'production';
  const hasSigningKey = process.env.JWT_SECRET || process.env.JWT_PUBLIC_KEY;

  if (isDev && !hasSigningKey && token.startsWith('ik_')) {
    // Parse dev token: ik_{timestamp}_{fwid}
    const parts = token.split('_');
    req.user = {
      id: parts[2] || 'dev-user',
      fwid: parts[2] || 'FWID-DEV-001',
      tier: 'citizen',
      roles: ['user'],
      nation: 'sovereign',
      iss: 'ierahkwa-dev'
    };
    req.tokenType = 'dev';
    return next();
  }

  try {
    const payload = verifyToken(token);

    // Attach user to request
    req.user = {
      id: payload.sub || payload.fwid,
      fwid: payload.fwid || payload.sub,
      tier: payload.tier || 'member',
      roles: payload.roles || ['user'],
      nation: payload.nation || 'sovereign',
      tenantId: payload.tid,
      iss: payload.iss
    };
    req.tokenType = 'jwt';

    next();
  } catch (err) {
    if (err.name === 'AppError') return next(err);
    log.error('Token verification failed', { err });
    next(new AppError('AUTH_INVALID_TOKEN', 'Token verification failed'));
  }
}

// ============================================================
// Middleware: Tier-Based Access Control
// ============================================================

/**
 * Require minimum tier level for route access
 * Usage: router.get('/premium', requireTier('resident'), handler)
 *
 * @param {string} minTier - Minimum tier: 'member' | 'resident' | 'citizen' | 'admin'
 */
function requireTier(minTier) {
  return (req, res, next) => {
    if (!req.user) {
      return next(new AppError('AUTH_REQUIRED', 'Authentication required'));
    }

    const userLevel = TIER_LEVELS[req.user.tier] ?? 0;
    const requiredLevel = TIER_LEVELS[minTier] ?? 0;

    if (userLevel < requiredLevel) {
      log.warn('Insufficient tier', { user: req.user.fwid, userTier: req.user.tier, required: minTier });
      return next(new AppError('AUTH_INSUFFICIENT_ROLE',
        `Tier '${minTier}' or higher required — current tier: '${req.user.tier}'`
      ));
    }

    next();
  };
}

/**
 * Require specific role(s) for route access
 * Usage: router.delete('/user/:id', requireRole('admin'), handler)
 *
 * @param {...string} roles - One or more roles required (OR logic)
 */
function requireRole(...roles) {
  return (req, res, next) => {
    if (!req.user) {
      return next(new AppError('AUTH_REQUIRED', 'Authentication required'));
    }

    const hasRole = roles.some(role => req.user.roles.includes(role));
    if (!hasRole) {
      log.warn('Insufficient role', { user: req.user.fwid, userRoles: req.user.roles, required: roles });
      return next(new AppError('AUTH_INSUFFICIENT_ROLE',
        `Role '${roles.join("' or '")}' required`
      ));
    }

    next();
  };
}

// ============================================================
// Exports
// ============================================================
module.exports = {
  jwtAuth,
  requireTier,
  requireRole,
  extractToken,
  verifyToken,
  PUBLIC_ROUTES,
  TIER_LEVELS
};
