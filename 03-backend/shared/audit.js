'use strict';

// ============================================================
// Ierahkwa Platform — Audit Logger v1.0.0
// Immutable, append-only audit trail for sensitive operations
// Designed for BDET Bank, identity, governance compliance
// UNDRIP / ILO-169 / Financial regulation compliant
// ============================================================

const crypto = require('crypto');

/**
 * Audit Event Categories
 */
const AUDIT_CATEGORIES = {
  // Financial
  TRANSACTION:      'TRANSACTION',
  BALANCE_CHANGE:   'BALANCE_CHANGE',
  ESCROW:           'ESCROW',
  REFUND:           'REFUND',

  // Authentication
  AUTH_LOGIN:        'AUTH_LOGIN',
  AUTH_LOGOUT:       'AUTH_LOGOUT',
  AUTH_FAILED:       'AUTH_FAILED',
  AUTH_MFA:          'AUTH_MFA',
  AUTH_PASSWORD:     'AUTH_PASSWORD',

  // Data Access
  DATA_READ:         'DATA_READ',
  DATA_CREATE:       'DATA_CREATE',
  DATA_UPDATE:       'DATA_UPDATE',
  DATA_DELETE:       'DATA_DELETE',
  DATA_EXPORT:       'DATA_EXPORT',

  // Admin
  ADMIN_ACTION:      'ADMIN_ACTION',
  PERMISSION_CHANGE: 'PERMISSION_CHANGE',
  CONFIG_CHANGE:     'CONFIG_CHANGE',
  USER_SUSPENDED:    'USER_SUSPENDED',

  // Governance
  VOTE_CAST:         'VOTE_CAST',
  PROPOSAL_CREATED:  'PROPOSAL_CREATED',
  SOVEREIGNTY_ACTION:'SOVEREIGNTY_ACTION',

  // System
  SYSTEM_STARTUP:    'SYSTEM_STARTUP',
  SYSTEM_SHUTDOWN:   'SYSTEM_SHUTDOWN',
  BACKUP_COMPLETED:  'BACKUP_COMPLETED',
  MIGRATION_RUN:     'MIGRATION_RUN'
};

/**
 * Risk Levels — determines retention and alerting
 */
const RISK_LEVELS = {
  LOW:      'LOW',       // Read operations, logins
  MEDIUM:   'MEDIUM',    // Data modifications
  HIGH:     'HIGH',      // Financial transactions, permission changes
  CRITICAL: 'CRITICAL'   // Admin actions, large transfers, suspensions
};

/**
 * Creates an audit logger for a service
 * @param {string} serviceName - Microservice name
 * @param {object} [options] - Audit configuration
 * @returns {AuditLogger} Audit logger instance
 */
function createAuditLogger(serviceName, options = {}) {
  const {
    output = 'stdout',       // 'stdout' | 'callback'
    onAuditEvent = null,     // callback for external storage (DB, queue)
    hashChain = true,        // Enable hash chain for tamper detection
    alertOnCritical = true   // Log CRITICAL events to stderr
  } = options;

  let previousHash = '0000000000000000000000000000000000000000000000000000000000000000';
  let sequenceNumber = 0;

  /**
   * Generate SHA-256 hash for tamper detection
   * Each event hash includes the previous hash (blockchain-like chain)
   */
  function computeHash(event) {
    const payload = JSON.stringify({
      seq: event.seq,
      prev: event.previousHash,
      ts: event.timestamp,
      cat: event.category,
      act: event.action,
      actor: event.actor?.id,
      resource: event.resource?.id
    });
    return crypto.createHash('sha256').update(payload).digest('hex');
  }

  /**
   * Record an audit event
   * @param {object} params - Audit event parameters
   */
  function record(params) {
    const {
      category,
      action,
      risk = RISK_LEVELS.LOW,
      actor = {},        // { id, type, ip, userAgent, tenantId }
      resource = {},     // { type, id, name }
      details = {},      // action-specific details
      outcome = 'SUCCESS', // SUCCESS | FAILURE | DENIED | ERROR
      req = null         // Express request object (auto-extracts context)
    } = params;

    sequenceNumber++;

    const event = {
      // Identity
      eventId: crypto.randomUUID(),
      seq: sequenceNumber,

      // Timing
      timestamp: new Date().toISOString(),
      epochMs: Date.now(),

      // Classification
      service: serviceName,
      category,
      action,
      risk,
      outcome,

      // Actor (who did it)
      actor: {
        id: actor.id || req?.user?.id || 'system',
        type: actor.type || (req?.user ? 'user' : 'system'),
        ip: actor.ip || req?.ip || req?.connection?.remoteAddress || 'unknown',
        userAgent: actor.userAgent || req?.headers?.['user-agent'] || 'unknown',
        tenantId: actor.tenantId || req?.tenantId || req?.user?.tenantId,
        requestId: req?.id
      },

      // Resource (what was affected)
      resource: {
        type: resource.type,
        id: resource.id,
        name: resource.name
      },

      // Details
      details: sanitizeDetails(details),

      // Integrity chain
      previousHash
    };

    // Compute hash for tamper detection
    if (hashChain) {
      event.hash = computeHash(event);
      previousHash = event.hash;
    }

    // Output
    const jsonLine = JSON.stringify(event);

    if (output === 'stdout') {
      process.stdout.write(`[AUDIT] ${jsonLine}\n`);
    }

    if (alertOnCritical && risk === RISK_LEVELS.CRITICAL) {
      process.stderr.write(`[AUDIT-CRITICAL] ${jsonLine}\n`);
    }

    if (onAuditEvent) {
      try { onAuditEvent(event); } catch {}
    }

    return event;
  }

  /**
   * Sanitize details — remove sensitive fields
   */
  function sanitizeDetails(details) {
    const sensitiveKeys = ['password', 'secret', 'token', 'privateKey', 'mnemonic', 'seed'];
    const clean = {};

    for (const [key, value] of Object.entries(details)) {
      if (sensitiveKeys.some(s => key.toLowerCase().includes(s))) {
        clean[key] = '[REDACTED]';
      } else {
        clean[key] = value;
      }
    }

    return clean;
  }

  // ============================================================
  // Convenience methods for common audit events
  // ============================================================

  return {
    record,

    // Financial
    transaction: (req, { from, to, amount, currency = 'WMP', type, txHash, ...extra }) =>
      record({
        category: AUDIT_CATEGORIES.TRANSACTION,
        action: `${type || 'transfer'}_executed`,
        risk: amount > 10000 ? RISK_LEVELS.CRITICAL : amount > 1000 ? RISK_LEVELS.HIGH : RISK_LEVELS.MEDIUM,
        req,
        resource: { type: 'transaction', id: txHash },
        details: { from, to, amount, currency, type, txHash, ...extra }
      }),

    // Auth
    loginSuccess: (req, userId) =>
      record({
        category: AUDIT_CATEGORIES.AUTH_LOGIN,
        action: 'login_success',
        risk: RISK_LEVELS.LOW,
        req,
        actor: { id: userId },
        resource: { type: 'session', id: req?.id }
      }),

    loginFailure: (req, reason) =>
      record({
        category: AUDIT_CATEGORIES.AUTH_FAILED,
        action: 'login_failure',
        risk: RISK_LEVELS.MEDIUM,
        outcome: 'FAILURE',
        req,
        details: { reason }
      }),

    // Data
    dataAccess: (req, resourceType, resourceId, action = 'read') =>
      record({
        category: AUDIT_CATEGORIES.DATA_READ,
        action: `data_${action}`,
        risk: action === 'delete' ? RISK_LEVELS.HIGH : RISK_LEVELS.LOW,
        req,
        resource: { type: resourceType, id: resourceId }
      }),

    dataModify: (req, resourceType, resourceId, changes = {}) =>
      record({
        category: AUDIT_CATEGORIES.DATA_UPDATE,
        action: 'data_modified',
        risk: RISK_LEVELS.MEDIUM,
        req,
        resource: { type: resourceType, id: resourceId },
        details: { changedFields: Object.keys(changes) }
      }),

    // Admin
    adminAction: (req, action, details = {}) =>
      record({
        category: AUDIT_CATEGORIES.ADMIN_ACTION,
        action,
        risk: RISK_LEVELS.CRITICAL,
        req,
        details
      }),

    permissionChange: (req, userId, changes) =>
      record({
        category: AUDIT_CATEGORIES.PERMISSION_CHANGE,
        action: 'permissions_modified',
        risk: RISK_LEVELS.CRITICAL,
        req,
        resource: { type: 'user', id: userId },
        details: changes
      }),

    // Governance
    voteCast: (req, proposalId, vote) =>
      record({
        category: AUDIT_CATEGORIES.VOTE_CAST,
        action: 'vote_cast',
        risk: RISK_LEVELS.HIGH,
        req,
        resource: { type: 'proposal', id: proposalId },
        details: { vote }
      }),

    // Express middleware — auto-audit all requests
    middleware: (options = {}) => {
      const { auditReads = false, pathFilter = null } = options;

      return (req, res, next) => {
        // Skip if path doesn't match filter
        if (pathFilter && !pathFilter(req.path)) return next();

        const start = Date.now();

        res.on('finish', () => {
          const method = req.method;
          const isWrite = ['POST', 'PUT', 'PATCH', 'DELETE'].includes(method);

          // Skip GET unless auditReads is enabled
          if (!isWrite && !auditReads) return;

          const duration = Date.now() - start;
          const action = { GET: 'read', POST: 'create', PUT: 'update', PATCH: 'update', DELETE: 'delete' }[method] || method;

          record({
            category: isWrite ? AUDIT_CATEGORIES.DATA_UPDATE : AUDIT_CATEGORIES.DATA_READ,
            action: `api_${action}`,
            risk: method === 'DELETE' ? RISK_LEVELS.HIGH : isWrite ? RISK_LEVELS.MEDIUM : RISK_LEVELS.LOW,
            outcome: res.statusCode < 400 ? 'SUCCESS' : res.statusCode < 500 ? 'DENIED' : 'ERROR',
            req,
            resource: { type: 'api', id: req.originalUrl || req.url },
            details: { method, statusCode: res.statusCode, duration }
          });
        });

        next();
      };
    },

    // Verify hash chain integrity
    verifyChain: (events) => {
      let prevHash = '0000000000000000000000000000000000000000000000000000000000000000';
      for (let i = 0; i < events.length; i++) {
        const event = events[i];
        if (event.previousHash !== prevHash) {
          return { valid: false, brokenAt: i, event: event.eventId, expected: prevHash, got: event.previousHash };
        }
        const computed = computeHash(event);
        if (event.hash !== computed) {
          return { valid: false, brokenAt: i, event: event.eventId, reason: 'hash_mismatch' };
        }
        prevHash = event.hash;
      }
      return { valid: true, count: events.length };
    },

    // Constants
    CATEGORIES: AUDIT_CATEGORIES,
    RISK: RISK_LEVELS
  };
}

// ============================================================
// Exports
// ============================================================
module.exports = {
  createAuditLogger,
  AUDIT_CATEGORIES,
  RISK_LEVELS
};
