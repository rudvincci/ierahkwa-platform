'use strict';

// ============================================================
// Ierahkwa Platform — Structured Logger v1.0.0
// JSON-structured logging with Pino-compatible output
// Supports: request tracing, performance metrics, child loggers
// ============================================================

const crypto = require('crypto');
const os = require('os');

const LOG_LEVELS = { trace: 10, debug: 20, info: 30, warn: 40, error: 50, fatal: 60 };
const LEVEL_NAMES = Object.fromEntries(Object.entries(LOG_LEVELS).map(([k, v]) => [v, k]));

const ENV = process.env.NODE_ENV || 'development';
const MIN_LEVEL = LOG_LEVELS[process.env.LOG_LEVEL || (ENV === 'production' ? 'info' : 'debug')] || LOG_LEVELS.info;
const HOSTNAME = os.hostname();

/**
 * Creates a structured logger for a service
 * @param {string} serviceName - Name of the microservice
 * @param {object} [options] - Logger configuration
 * @returns {Logger} Structured logger instance
 */
function createLogger(serviceName, options = {}) {
  const {
    level = process.env.LOG_LEVEL || (ENV === 'production' ? 'info' : 'debug'),
    pretty = ENV === 'development',
    redact = ['password', 'token', 'secret', 'authorization', 'cookie', 'creditCard', 'ssn'],
    context = {}
  } = options;

  const minLevel = LOG_LEVELS[level] || LOG_LEVELS.info;

  // Redact sensitive fields from objects
  function redactObj(obj, depth = 0) {
    if (depth > 5 || !obj || typeof obj !== 'object') return obj;
    if (Array.isArray(obj)) return obj.map(item => redactObj(item, depth + 1));

    const clean = {};
    for (const [key, value] of Object.entries(obj)) {
      if (redact.some(r => key.toLowerCase().includes(r.toLowerCase()))) {
        clean[key] = '[REDACTED]';
      } else if (typeof value === 'object' && value !== null) {
        clean[key] = redactObj(value, depth + 1);
      } else {
        clean[key] = value;
      }
    }
    return clean;
  }

  // Format log entry
  function formatEntry(level, msg, data = {}) {
    const entry = {
      level,
      levelName: LEVEL_NAMES[level],
      time: new Date().toISOString(),
      pid: process.pid,
      hostname: HOSTNAME,
      service: serviceName,
      ...context,
      msg,
      ...redactObj(data)
    };

    if (data.err || data.error) {
      const err = data.err || data.error;
      if (err instanceof Error) {
        entry.err = {
          type: err.constructor.name,
          message: err.message,
          stack: ENV !== 'production' ? err.stack : undefined,
          code: err.code
        };
      }
    }

    return entry;
  }

  // Write log entry
  function write(level, msg, data) {
    if (level < minLevel) return;

    const entry = formatEntry(level, msg, data);

    if (pretty) {
      const color = level >= 50 ? '\x1b[31m' : level >= 40 ? '\x1b[33m' : level >= 30 ? '\x1b[36m' : '\x1b[90m';
      const reset = '\x1b[0m';
      const ts = entry.time.split('T')[1].replace('Z', '');
      const stream = level >= 40 ? process.stderr : process.stdout;
      stream.write(`${color}[${ts}] ${entry.levelName.toUpperCase().padEnd(5)} ${reset}${serviceName} — ${msg}${data && Object.keys(data).length ? ' ' + JSON.stringify(redactObj(data)) : ''}\n`);
    } else {
      const stream = level >= 40 ? process.stderr : process.stdout;
      stream.write(JSON.stringify(entry) + '\n');
    }
  }

  const logger = {
    trace: (msg, data) => write(LOG_LEVELS.trace, msg, data),
    debug: (msg, data) => write(LOG_LEVELS.debug, msg, data),
    info:  (msg, data) => write(LOG_LEVELS.info, msg, data),
    warn:  (msg, data) => write(LOG_LEVELS.warn, msg, data),
    error: (msg, data) => write(LOG_LEVELS.error, msg, data),
    fatal: (msg, data) => write(LOG_LEVELS.fatal, msg, data),

    /**
     * Create child logger with additional context
     * @param {object} bindings - Additional context fields
     * @returns {Logger} Child logger with merged context
     */
    child: (bindings) => createLogger(serviceName, {
      ...options,
      context: { ...context, ...bindings }
    }),

    /**
     * Express request logging middleware
     * Logs incoming requests and response time
     */
    requestLogger: () => {
      return (req, res, next) => {
        const start = process.hrtime.bigint();
        const requestId = req.id || req.headers['x-request-id'] || crypto.randomUUID();

        // Attach request ID
        req.id = requestId;
        req.log = logger.child({ requestId });
        res.setHeader('X-Request-Id', requestId);

        // Log request
        req.log.info('request received', {
          method: req.method,
          url: req.originalUrl || req.url,
          ip: req.ip || req.connection?.remoteAddress,
          userAgent: req.headers['user-agent'],
          contentLength: req.headers['content-length']
        });

        // Log response on finish
        const onFinish = () => {
          const duration = Number(process.hrtime.bigint() - start) / 1e6; // ms
          const level = res.statusCode >= 500 ? 'error' : res.statusCode >= 400 ? 'warn' : 'info';

          req.log[level]('request completed', {
            method: req.method,
            url: req.originalUrl || req.url,
            statusCode: res.statusCode,
            duration: Math.round(duration * 100) / 100,
            contentLength: res.getHeader('content-length')
          });

          res.removeListener('finish', onFinish);
        };

        res.on('finish', onFinish);
        next();
      };
    },

    /**
     * Performance timer for measuring operation duration
     * @param {string} operation - Name of the operation
     * @returns {Function} Call to end timer and log duration
     */
    startTimer: (operation) => {
      const start = process.hrtime.bigint();
      return (data = {}) => {
        const duration = Number(process.hrtime.bigint() - start) / 1e6;
        logger.info(`${operation} completed`, { ...data, operation, duration: Math.round(duration * 100) / 100 });
        return duration;
      };
    }
  };

  return logger;
}

// ============================================================
// Exports
// ============================================================
module.exports = { createLogger, LOG_LEVELS };
