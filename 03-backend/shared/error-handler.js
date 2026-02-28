'use strict';

// ============================================================
// Ierahkwa Platform — Standardized Error Handler v1.0.0
// RFC 7807 Problem Details for HTTP APIs
// https://tools.ietf.org/html/rfc7807
// ============================================================

const ERROR_CODES = {
  // Authentication & Authorization (1xxx)
  AUTH_REQUIRED:          { status: 401, code: 'ERR_AUTH_REQUIRED',          title: 'Authentication required' },
  AUTH_INVALID_TOKEN:     { status: 401, code: 'ERR_AUTH_INVALID_TOKEN',     title: 'Invalid or expired token' },
  AUTH_INSUFFICIENT_ROLE: { status: 403, code: 'ERR_AUTH_INSUFFICIENT_ROLE', title: 'Insufficient permissions' },
  AUTH_ACCOUNT_LOCKED:    { status: 403, code: 'ERR_AUTH_ACCOUNT_LOCKED',    title: 'Account is locked' },
  AUTH_INVALID_CREDS:     { status: 401, code: 'ERR_AUTH_INVALID_CREDS',     title: 'Invalid credentials' },

  // Validation (2xxx)
  VALIDATION_FAILED:      { status: 400, code: 'ERR_VALIDATION_FAILED',      title: 'Validation failed' },
  INVALID_INPUT:          { status: 400, code: 'ERR_INVALID_INPUT',          title: 'Invalid input data' },
  MISSING_FIELD:          { status: 400, code: 'ERR_MISSING_FIELD',          title: 'Required field missing' },
  INVALID_FORMAT:         { status: 400, code: 'ERR_INVALID_FORMAT',         title: 'Invalid data format' },

  // Resources (3xxx)
  NOT_FOUND:              { status: 404, code: 'ERR_NOT_FOUND',              title: 'Resource not found' },
  ALREADY_EXISTS:         { status: 409, code: 'ERR_ALREADY_EXISTS',         title: 'Resource already exists' },
  CONFLICT:               { status: 409, code: 'ERR_CONFLICT',               title: 'Resource conflict' },
  GONE:                   { status: 410, code: 'ERR_GONE',                   title: 'Resource no longer available' },

  // Rate Limiting (4xxx)
  RATE_LIMITED:            { status: 429, code: 'ERR_RATE_LIMITED',           title: 'Too many requests' },
  QUOTA_EXCEEDED:          { status: 429, code: 'ERR_QUOTA_EXCEEDED',        title: 'Usage quota exceeded' },

  // Server (5xxx)
  INTERNAL:               { status: 500, code: 'ERR_INTERNAL',               title: 'Internal server error' },
  SERVICE_UNAVAILABLE:    { status: 503, code: 'ERR_SERVICE_UNAVAILABLE',    title: 'Service temporarily unavailable' },
  DOWNSTREAM_FAILURE:     { status: 502, code: 'ERR_DOWNSTREAM_FAILURE',     title: 'Downstream service failure' },
  TIMEOUT:                { status: 504, code: 'ERR_TIMEOUT',                title: 'Operation timed out' },

  // BDET / Financial (6xxx)
  INSUFFICIENT_FUNDS:     { status: 422, code: 'ERR_INSUFFICIENT_FUNDS',     title: 'Insufficient WAMPUM balance' },
  TRANSACTION_FAILED:     { status: 422, code: 'ERR_TRANSACTION_FAILED',     title: 'Transaction failed' },
  INVALID_WALLET:         { status: 400, code: 'ERR_INVALID_WALLET',         title: 'Invalid wallet address' },
  BLOCKCHAIN_ERROR:       { status: 502, code: 'ERR_BLOCKCHAIN_ERROR',       title: 'Blockchain operation failed' },

  // Tenant (7xxx)
  TENANT_NOT_FOUND:       { status: 404, code: 'ERR_TENANT_NOT_FOUND',       title: 'Tenant not found' },
  TENANT_SUSPENDED:       { status: 403, code: 'ERR_TENANT_SUSPENDED',       title: 'Tenant account suspended' }
};

/**
 * Application Error — extends Error with RFC 7807 fields
 */
class AppError extends Error {
  constructor(errorCode, detail, extras = {}) {
    const errorDef = typeof errorCode === 'string' ? ERROR_CODES[errorCode] : errorCode;

    if (!errorDef) {
      throw new Error(`Unknown error code: ${errorCode}`);
    }

    super(detail || errorDef.title);

    this.name = 'AppError';
    this.status = errorDef.status;
    this.code = errorDef.code;
    this.title = errorDef.title;
    this.detail = detail || errorDef.title;
    this.instance = extras.instance;
    this.errors = extras.errors; // validation errors array
    this.meta = extras.meta;     // additional metadata

    // Capture stack trace
    Error.captureStackTrace(this, this.constructor);
  }

  /**
   * Convert to RFC 7807 Problem Details JSON
   */
  toJSON() {
    const problem = {
      type: `https://api.ierahkwa.bo/errors/${this.code}`,
      title: this.title,
      status: this.status,
      detail: this.detail,
      instance: this.instance
    };

    if (this.errors && this.errors.length > 0) {
      problem.errors = this.errors;
    }

    if (this.meta) {
      problem.meta = this.meta;
    }

    return problem;
  }
}

/**
 * Express error handler middleware — RFC 7807 compliant
 * @param {string} serviceName - Microservice name for logging
 * @param {object} [logger] - Logger instance (optional, falls back to console)
 */
function errorMiddleware(serviceName, logger) {
  const log = logger || console;

  return (err, req, res, _next) => {
    const requestId = req.id || req.headers['x-request-id'] || 'unknown';

    // Determine error details
    let status, responseBody;

    if (err instanceof AppError) {
      status = err.status;
      responseBody = {
        ...err.toJSON(),
        instance: `${req.method} ${req.originalUrl || req.url}`,
        requestId,
        timestamp: new Date().toISOString()
      };

      // Log at appropriate level
      if (status >= 500) {
        (log.error || log.warn).call(log, `[${serviceName}] Server error`, { err, requestId, path: req.path });
      } else {
        (log.warn || log.info).call(log, `[${serviceName}] Client error`, { code: err.code, detail: err.detail, requestId });
      }
    } else if (err.type === 'entity.parse.failed') {
      // JSON parse error
      status = 400;
      responseBody = {
        type: `https://api.ierahkwa.bo/errors/ERR_INVALID_JSON`,
        title: 'Invalid JSON',
        status: 400,
        detail: 'Request body contains invalid JSON',
        instance: `${req.method} ${req.originalUrl || req.url}`,
        requestId,
        timestamp: new Date().toISOString()
      };
    } else {
      // Unknown error — never leak details in production
      status = err.statusCode || err.status || 500;
      const isProduction = process.env.NODE_ENV === 'production';

      responseBody = {
        type: `https://api.ierahkwa.bo/errors/ERR_INTERNAL`,
        title: 'Internal server error',
        status,
        detail: isProduction ? 'An unexpected error occurred' : err.message,
        instance: `${req.method} ${req.originalUrl || req.url}`,
        requestId,
        timestamp: new Date().toISOString()
      };

      if (!isProduction) {
        responseBody.stack = err.stack;
      }

      // Always log unknown errors at error level
      (log.error || console.error).call(log, `[${serviceName}] Unhandled error`, {
        err: { message: err.message, stack: err.stack, code: err.code },
        requestId,
        path: req.path,
        method: req.method
      });
    }

    // Set RFC 7807 content type
    res.status(status)
       .set('Content-Type', 'application/problem+json')
       .json(responseBody);
  };
}

/**
 * Async route handler wrapper — catches async errors
 * Usage: router.get('/path', asyncHandler(async (req, res) => { ... }))
 */
function asyncHandler(fn) {
  return (req, res, next) => {
    Promise.resolve(fn(req, res, next)).catch(next);
  };
}

/**
 * Not Found handler — 404 for unmatched routes
 */
function notFoundHandler(req, res) {
  res.status(404)
     .set('Content-Type', 'application/problem+json')
     .json({
       type: 'https://api.ierahkwa.bo/errors/ERR_NOT_FOUND',
       title: 'Not found',
       status: 404,
       detail: `Route ${req.method} ${req.originalUrl || req.url} not found`,
       instance: `${req.method} ${req.originalUrl || req.url}`,
       requestId: req.id || 'unknown',
       timestamp: new Date().toISOString()
     });
}

// ============================================================
// Exports
// ============================================================
module.exports = {
  ERROR_CODES,
  AppError,
  errorMiddleware,
  asyncHandler,
  notFoundHandler
};
