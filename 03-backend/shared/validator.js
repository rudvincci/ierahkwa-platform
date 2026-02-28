'use strict';

// ============================================================
// Ierahkwa Platform — Input Validation Middleware v1.0.0
// Lightweight schema validation (no external dependencies)
// Compatible with Joi-style schemas but zero-dependency
// ============================================================

const { AppError } = require('./error-handler');

// ============================================================
// SCHEMA TYPES — Validation rules
// ============================================================

const types = {
  string: (options = {}) => ({
    type: 'string',
    ...options,
    validate(value, field) {
      if (value === undefined || value === null || value === '') {
        return options.required ? `${field} is required` : null;
      }
      if (typeof value !== 'string') return `${field} must be a string`;
      if (options.min && value.length < options.min) return `${field} must be at least ${options.min} characters`;
      if (options.max && value.length > options.max) return `${field} must be at most ${options.max} characters`;
      if (options.pattern && !options.pattern.test(value)) return `${field} has invalid format`;
      if (options.enum && !options.enum.includes(value)) return `${field} must be one of: ${options.enum.join(', ')}`;
      if (options.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) return `${field} must be a valid email`;
      return null;
    }
  }),

  number: (options = {}) => ({
    type: 'number',
    ...options,
    validate(value, field) {
      if (value === undefined || value === null || value === '') {
        return options.required ? `${field} is required` : null;
      }
      const num = Number(value);
      if (isNaN(num)) return `${field} must be a number`;
      if (options.min !== undefined && num < options.min) return `${field} must be at least ${options.min}`;
      if (options.max !== undefined && num > options.max) return `${field} must be at most ${options.max}`;
      if (options.integer && !Number.isInteger(num)) return `${field} must be an integer`;
      return null;
    }
  }),

  boolean: (options = {}) => ({
    type: 'boolean',
    ...options,
    validate(value, field) {
      if (value === undefined || value === null) {
        return options.required ? `${field} is required` : null;
      }
      if (typeof value !== 'boolean' && value !== 'true' && value !== 'false') {
        return `${field} must be a boolean`;
      }
      return null;
    }
  }),

  array: (options = {}) => ({
    type: 'array',
    ...options,
    validate(value, field) {
      if (value === undefined || value === null) {
        return options.required ? `${field} is required` : null;
      }
      if (!Array.isArray(value)) return `${field} must be an array`;
      if (options.min && value.length < options.min) return `${field} must have at least ${options.min} items`;
      if (options.max && value.length > options.max) return `${field} must have at most ${options.max} items`;
      return null;
    }
  }),

  uuid: (options = {}) => ({
    type: 'uuid',
    ...options,
    validate(value, field) {
      if (value === undefined || value === null || value === '') {
        return options.required ? `${field} is required` : null;
      }
      if (!/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(value)) {
        return `${field} must be a valid UUID`;
      }
      return null;
    }
  }),

  date: (options = {}) => ({
    type: 'date',
    ...options,
    validate(value, field) {
      if (value === undefined || value === null || value === '') {
        return options.required ? `${field} is required` : null;
      }
      const d = new Date(value);
      if (isNaN(d.getTime())) return `${field} must be a valid date`;
      if (options.min && d < new Date(options.min)) return `${field} must be after ${options.min}`;
      if (options.max && d > new Date(options.max)) return `${field} must be before ${options.max}`;
      return null;
    }
  }),

  // WAMPUM address validation
  walletAddress: (options = {}) => ({
    type: 'walletAddress',
    ...options,
    validate(value, field) {
      if (value === undefined || value === null || value === '') {
        return options.required ? `${field} is required` : null;
      }
      if (!/^0x[0-9a-fA-F]{40}$/.test(value)) {
        return `${field} must be a valid WAMPUM wallet address (0x + 40 hex chars)`;
      }
      return null;
    }
  }),

  // WAMPUM amount validation
  wampumAmount: (options = {}) => ({
    type: 'wampumAmount',
    ...options,
    validate(value, field) {
      if (value === undefined || value === null) {
        return options.required ? `${field} is required` : null;
      }
      const num = Number(value);
      if (isNaN(num)) return `${field} must be a valid amount`;
      if (num <= 0) return `${field} must be greater than 0`;
      if (options.max && num > options.max) return `${field} exceeds maximum amount of ${options.max}`;
      // Max 8 decimal places for WAMPUM
      const decimalPart = value.toString().split('.')[1];
      if (decimalPart && decimalPart.length > 8) return `${field} cannot have more than 8 decimal places`;
      return null;
    }
  })
};

// ============================================================
// VALIDATION MIDDLEWARE
// ============================================================

/**
 * Validate request data against a schema
 * @param {object} schema - { body: {...}, query: {...}, params: {...} }
 * @returns {Function} Express middleware
 *
 * Usage:
 *   const { validate, t } = require('../shared/validator');
 *
 *   router.post('/transfer', validate({
 *     body: {
 *       from:   t.walletAddress({ required: true }),
 *       to:     t.walletAddress({ required: true }),
 *       amount: t.wampumAmount({ required: true, max: 1000000 }),
 *       memo:   t.string({ max: 256 })
 *     }
 *   }), handler);
 */
function validate(schema) {
  return (req, res, next) => {
    const errors = [];

    for (const source of ['body', 'query', 'params']) {
      if (!schema[source]) continue;

      const data = req[source] || {};

      for (const [field, rule] of Object.entries(schema[source])) {
        const value = data[field];
        const error = rule.validate(value, field);

        if (error) {
          errors.push({
            field,
            source,
            message: error,
            value: typeof value === 'string' && value.length > 100 ? value.substring(0, 100) + '...' : value
          });
        }
      }
    }

    if (errors.length > 0) {
      return next(new AppError('VALIDATION_FAILED', `${errors.length} validation error(s)`, { errors }));
    }

    next();
  };
}

/**
 * Sanitize and coerce validated data
 * Strips unknown fields from request body
 * @param {object} allowedFields - { fieldName: type, ... }
 */
function sanitizeBody(allowedFields) {
  return (req, res, next) => {
    if (!req.body) return next();

    const clean = {};
    for (const [field, rule] of Object.entries(allowedFields)) {
      if (req.body[field] !== undefined) {
        let value = req.body[field];

        // Type coercion
        if (rule.type === 'number' && typeof value === 'string') {
          value = Number(value);
        } else if (rule.type === 'boolean' && typeof value === 'string') {
          value = value === 'true';
        } else if (rule.type === 'string' && typeof value === 'string') {
          value = value.trim();
        }

        clean[field] = value;
      }
    }

    req.body = clean;
    next();
  };
}

// ============================================================
// PRE-BUILT SCHEMAS — Common validation patterns
// ============================================================

const schemas = {
  // Pagination query params
  pagination: {
    query: {
      page:  types.number({ min: 1, integer: true }),
      limit: types.number({ min: 1, max: 100, integer: true }),
      sort:  types.string({ pattern: /^[a-zA-Z_]+:(asc|desc)$/ })
    }
  },

  // BDET transfer
  bdetTransfer: {
    body: {
      from:   types.walletAddress({ required: true }),
      to:     types.walletAddress({ required: true }),
      amount: types.wampumAmount({ required: true, max: 10000000 }),
      memo:   types.string({ max: 256 }),
      type:   types.string({ required: true, enum: ['transfer', 'payment', 'escrow', 'refund'] })
    }
  },

  // User registration
  userRegistration: {
    body: {
      email:      types.string({ required: true, email: true }),
      fullName:   types.string({ required: true, min: 2, max: 100 }),
      language:   types.string({ required: true, enum: ['es', 'en', 'qu', 'ay', 'gn', 'my', 'na', 'mp'] }),
      tenantId:   types.uuid(),
      sovereignty: types.string({ enum: ['full', 'affiliate', 'observer'] })
    }
  },

  // ID param
  idParam: {
    params: {
      id: types.uuid({ required: true })
    }
  }
};

// ============================================================
// Exports
// ============================================================
module.exports = {
  validate,
  sanitizeBody,
  schemas,
  t: types
};
