'use strict';

const crypto = require('crypto');

// ============================================================
// OWASP Ierahkwa Platform — Shared Security Middleware
// Covers: OWASP Top 10:2025, API Security, CI/CD, LLM, Mobile
// ============================================================

/**
 * 1. CORS Configuration (OWASP A01 - Broken Access Control)
 * Whitelist-based CORS — never use origin: '*' in production
 */
function corsConfig() {
  const allowedOrigins = (process.env.CORS_ORIGINS || 'http://localhost:3000').split(',');
  return {
    origin: function (origin, callback) {
      // Allow requests with no origin (mobile apps, curl, server-to-server)
      if (!origin) return callback(null, true);
      if (allowedOrigins.indexOf(origin) !== -1) {
        return callback(null, true);
      }
      return callback(new Error('CORS: Origin not allowed'));
    },
    credentials: true,
    methods: ['GET', 'POST', 'PUT', 'DELETE', 'PATCH'],
    allowedHeaders: ['Content-Type', 'Authorization', 'X-Requested-With', 'X-Tenant-Id'],
    exposedHeaders: ['X-Total-Count', 'X-Request-Id'],
    maxAge: 600 // 10 minutes
  };
}

/**
 * 2. Rate Limiter Factory (OWASP API4 - Unrestricted Resource Consumption)
 * Creates rate limiters for different endpoint types
 */
function rateLimiterMiddleware(options = {}) {
  const {
    windowMs = 15 * 60 * 1000, // 15 minutes
    max = 100,                  // limit per window
    message = 'Too many requests, please try again later',
    keyGenerator = (req) => req.ip || req.connection.remoteAddress
  } = options;

  const store = new Map();

  // Clean expired entries every minute
  setInterval(() => {
    const now = Date.now();
    for (const [key, data] of store.entries()) {
      if (now - data.startTime > windowMs) store.delete(key);
    }
  }, 60000).unref();

  return (req, res, next) => {
    const key = keyGenerator(req);
    const now = Date.now();
    let record = store.get(key);

    if (!record || now - record.startTime > windowMs) {
      record = { count: 0, startTime: now };
      store.set(key, record);
    }

    record.count++;

    const remaining = Math.max(0, max - record.count);
    const resetTime = new Date(record.startTime + windowMs);

    res.setHeader('X-RateLimit-Limit', max);
    res.setHeader('X-RateLimit-Remaining', remaining);
    res.setHeader('X-RateLimit-Reset', resetTime.toISOString());

    if (record.count > max) {
      res.setHeader('Retry-After', Math.ceil((record.startTime + windowMs - now) / 1000));
      return res.status(429).json({ error: message, retryAfter: res.getHeader('Retry-After') });
    }

    next();
  };
}

// Preset rate limiters
const rateLimiters = {
  // Strict: for login/auth endpoints
  auth: () => rateLimiterMiddleware({ windowMs: 15 * 60 * 1000, max: 10, message: 'Too many login attempts. Try again in 15 minutes.' }),
  // Standard: for API endpoints
  api: () => rateLimiterMiddleware({ windowMs: 15 * 60 * 1000, max: 100 }),
  // Relaxed: for public/read endpoints
  public: () => rateLimiterMiddleware({ windowMs: 15 * 60 * 1000, max: 300 }),
  // Upload: for file upload endpoints
  upload: () => rateLimiterMiddleware({ windowMs: 60 * 60 * 1000, max: 20, message: 'Upload limit reached. Try again in 1 hour.' })
};

/**
 * 3. Input Sanitizer (OWASP A03 - Injection)
 * Sanitizes common injection patterns from request data
 */
function sanitizeInput(req, res, next) {
  const sanitize = (obj) => {
    if (typeof obj === 'string') {
      // Remove null bytes
      obj = obj.replace(/\0/g, '');
      // Basic XSS prevention — strip script tags
      obj = obj.replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, '');
      // Remove common SQL injection patterns (belt and suspenders with parameterized queries)
      obj = obj.replace(/(\b(SELECT|INSERT|UPDATE|DELETE|DROP|UNION|ALTER|CREATE|EXEC)\b)/gi, '');
      return obj.trim();
    }
    if (Array.isArray(obj)) return obj.map(sanitize);
    if (obj && typeof obj === 'object') {
      const clean = {};
      for (const [key, value] of Object.entries(obj)) {
        // Prevent prototype pollution
        if (key === '__proto__' || key === 'constructor' || key === 'prototype') continue;
        clean[sanitize(key)] = sanitize(value);
      }
      return clean;
    }
    return obj;
  };

  if (req.body) req.body = sanitize(req.body);
  if (req.query) req.query = sanitize(req.query);
  if (req.params) req.params = sanitize(req.params);
  next();
}

/**
 * 4. Security Headers (OWASP A05 - Security Misconfiguration)
 * Adds security headers when helmet is not available
 */
function securityHeaders(req, res, next) {
  res.setHeader('X-Content-Type-Options', 'nosniff');
  res.setHeader('X-Frame-Options', 'DENY');
  res.setHeader('X-XSS-Protection', '0'); // Disabled per OWASP recommendation (use CSP instead)
  res.setHeader('Strict-Transport-Security', 'max-age=31536000; includeSubDomains');
  res.setHeader('Cache-Control', 'no-store, no-cache, must-revalidate, private');
  res.setHeader('Pragma', 'no-cache');
  res.setHeader('X-Permitted-Cross-Domain-Policies', 'none');
  res.setHeader('Referrer-Policy', 'strict-origin-when-cross-origin');
  res.setHeader('Content-Security-Policy', "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; connect-src 'self'; frame-ancestors 'none'");
  res.removeHeader('X-Powered-By');
  next();
}

/**
 * 5. Request ID Middleware (OWASP A09 - Logging Failures)
 * Assigns unique ID to every request for tracing
 */
function requestId(req, res, next) {
  req.id = req.headers['x-request-id'] || crypto.randomUUID();
  res.setHeader('X-Request-Id', req.id);
  next();
}

/**
 * 6. Security Logger (OWASP A09 - Logging & Monitoring Failures)
 * Logs security-relevant events in structured format
 */
function securityLogger(serviceName) {
  return {
    authSuccess: (req, userId) => {
      console.log(JSON.stringify({
        event: 'AUTH_SUCCESS',
        service: serviceName,
        userId,
        ip: req.ip || req.connection.remoteAddress,
        userAgent: req.headers['user-agent'],
        requestId: req.id,
        timestamp: new Date().toISOString()
      }));
    },
    authFailure: (req, reason) => {
      console.warn(JSON.stringify({
        event: 'AUTH_FAILURE',
        service: serviceName,
        reason,
        ip: req.ip || req.connection.remoteAddress,
        userAgent: req.headers['user-agent'],
        requestId: req.id,
        path: req.path,
        timestamp: new Date().toISOString()
      }));
    },
    accessDenied: (req, resource) => {
      console.warn(JSON.stringify({
        event: 'ACCESS_DENIED',
        service: serviceName,
        resource,
        ip: req.ip || req.connection.remoteAddress,
        requestId: req.id,
        timestamp: new Date().toISOString()
      }));
    },
    suspiciousActivity: (req, details) => {
      console.error(JSON.stringify({
        event: 'SUSPICIOUS_ACTIVITY',
        service: serviceName,
        details,
        ip: req.ip || req.connection.remoteAddress,
        userAgent: req.headers['user-agent'],
        requestId: req.id,
        path: req.path,
        method: req.method,
        timestamp: new Date().toISOString()
      }));
    },
    dataAccess: (req, resource, action) => {
      console.log(JSON.stringify({
        event: 'DATA_ACCESS',
        service: serviceName,
        resource,
        action,
        ip: req.ip || req.connection.remoteAddress,
        requestId: req.id,
        timestamp: new Date().toISOString()
      }));
    }
  };
}

/**
 * 7. Error Handler (OWASP A05 - Security Misconfiguration)
 * Never leak stack traces or internal details in production
 */
function errorHandler(serviceName) {
  return (err, req, res, _next) => {
    const requestId = req.id || 'unknown';

    // Log full error internally
    console.error(JSON.stringify({
      event: 'ERROR',
      service: serviceName,
      message: err.message,
      stack: err.stack,
      requestId,
      path: req.path,
      method: req.method,
      timestamp: new Date().toISOString()
    }));

    // Never expose internal errors to client
    const statusCode = err.statusCode || err.status || 500;
    const response = {
      error: statusCode >= 500 ? 'Internal server error' : err.message,
      requestId,
      timestamp: new Date().toISOString()
    };

    // Only add stack trace in development
    if (process.env.NODE_ENV === 'development') {
      response.stack = err.stack;
      response.detail = err.message;
    }

    res.status(statusCode).json(response);
  };
}

/**
 * 8. JWT Utilities (OWASP A07 - Authentication Failures)
 * Secure JWT token generation and validation helpers
 */
const jwtUtils = {
  // Generate cryptographically secure secret (for initial setup)
  generateSecret: () => crypto.randomBytes(64).toString('hex'),

  // Validate JWT secret strength
  isSecretStrong: (secret) => {
    if (!secret || secret.length < 32) return false;
    // Reject common weak secrets
    const weakPatterns = ['change', 'secret', 'password', 'default', 'test', '1234'];
    return !weakPatterns.some(pattern => secret.toLowerCase().includes(pattern));
  },

  // Generate secure refresh token
  generateRefreshToken: () => crypto.randomBytes(48).toString('hex'),

  // Warn if JWT secret is weak
  validateConfig: () => {
    const secret = process.env.JWT_SECRET;
    if (!secret) {
      console.error('[SECURITY] JWT_SECRET environment variable is not set!');
      return false;
    }
    if (secret.length < 32) {
      console.error('[SECURITY] JWT_SECRET is too short (minimum 32 characters)');
      return false;
    }
    return true;
  }
};

/**
 * 9. File Upload Security (OWASP A08 - Software Integrity Failures)
 * Validates file uploads by magic bytes, not just extension
 */
const fileUploadSecurity = {
  // Magic bytes for common file types
  MAGIC_BYTES: {
    'image/jpeg': [Buffer.from([0xFF, 0xD8, 0xFF])],
    'image/png': [Buffer.from([0x89, 0x50, 0x4E, 0x47])],
    'image/gif': [Buffer.from([0x47, 0x49, 0x46, 0x38])],
    'image/webp': [Buffer.from([0x52, 0x49, 0x46, 0x46])],
    'application/pdf': [Buffer.from([0x25, 0x50, 0x44, 0x46])]
  },

  validateMagicBytes: (buffer, declaredMimeType) => {
    const expectedMagicBytes = fileUploadSecurity.MAGIC_BYTES[declaredMimeType];
    if (!expectedMagicBytes) return false;

    return expectedMagicBytes.some(magic => {
      if (buffer.length < magic.length) return false;
      return magic.every((byte, i) => buffer[i] === byte);
    });
  },

  sanitizeFilename: (filename) => {
    // Remove path traversal attempts
    let safe = filename.replace(/[/\\]/g, '');
    // Remove null bytes
    safe = safe.replace(/\0/g, '');
    // Remove leading dots (hidden files)
    safe = safe.replace(/^\.+/, '');
    // Only allow alphanumeric, dashes, underscores, dots
    safe = safe.replace(/[^a-zA-Z0-9._-]/g, '_');
    // Limit length
    if (safe.length > 255) safe = safe.substring(0, 255);
    return safe || 'unnamed_file';
  },

  // Max file sizes per type (in bytes)
  MAX_SIZES: {
    image: 10 * 1024 * 1024,    // 10MB
    document: 50 * 1024 * 1024,  // 50MB
    video: 500 * 1024 * 1024     // 500MB
  }
};

/**
 * 10. Tenant Isolation (OWASP A01 - Broken Access Control)
 * Ensures multi-tenant data isolation
 */
function tenantIsolation(req, res, next) {
  const tenantId = req.headers['x-tenant-id'] || req.user?.tenantId;
  if (!tenantId) {
    return res.status(400).json({ error: 'Tenant identification required' });
  }
  // Validate tenant ID format (UUID)
  const uuidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
  if (!uuidRegex.test(tenantId) && !/^\d+$/.test(tenantId)) {
    return res.status(400).json({ error: 'Invalid tenant identifier' });
  }
  req.tenantId = tenantId;
  next();
}

/**
 * 11. Apply All Security Middleware (convenience function)
 * Usage: const { applySecurityMiddleware } = require('../shared/security');
 *        applySecurityMiddleware(app, 'service-name');
 */
function applySecurityMiddleware(app, serviceName, options = {}) {
  const cors = options.cors !== false;

  // 1. Request ID for tracing
  app.use(requestId);

  // 2. Security Headers
  app.use(securityHeaders);

  // 3. Input Sanitization
  app.use(sanitizeInput);

  // 4. Rate Limiting (global)
  app.use(rateLimiters.api());

  // 5. Disable X-Powered-By
  app.disable('x-powered-by');

  // 6. Trust proxy (for correct IP behind reverse proxy)
  app.set('trust proxy', 1);

  // Return logger for the service
  return securityLogger(serviceName);
}

// ============================================================
// Exports
// ============================================================
module.exports = {
  corsConfig,
  rateLimiterMiddleware,
  rateLimiters,
  sanitizeInput,
  securityHeaders,
  requestId,
  securityLogger,
  errorHandler,
  jwtUtils,
  fileUploadSecurity,
  tenantIsolation,
  applySecurityMiddleware
};
