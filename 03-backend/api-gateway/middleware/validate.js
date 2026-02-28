'use strict';

// ============================================================
// Ierahkwa API Gateway — Route Validation Middleware v1.0.0
// Pre-built validation schemas for all 22 API route groups
// Uses shared/validator.js for zero-dependency validation
// ============================================================

const { validate, sanitizeBody, schemas, t } = require('../../shared/validator');

// ============================================================
// ROUTE VALIDATION SCHEMAS
// ============================================================

const routeSchemas = {
  // ── Auth ──────────────────────────────────────────────────
  'auth.login': {
    body: {
      fwid:   t.string({ required: true, min: 3, max: 64 }),
      nation: t.string({ max: 64 }),
      tier:   t.string({ enum: ['member', 'resident', 'citizen'] })
    }
  },
  'auth.register': {
    body: {
      name:   t.string({ required: true, min: 2, max: 100 }),
      nation: t.string({ required: true, min: 2, max: 64 }),
      email:  t.string({ email: true }),
      language: t.string({ enum: ['es', 'en', 'qu', 'ay', 'gn', 'my', 'na', 'mp'] })
    }
  },

  // ── BDET Financial ────────────────────────────────────────
  'bdet.transfer': schemas.bdetTransfer,
  'bdet.stake': {
    body: {
      amount:   t.wampumAmount({ required: true, max: 10000000 }),
      duration: t.number({ min: 1, max: 365, integer: true }),
      wallet:   t.walletAddress({ required: true })
    }
  },
  'bdet.balance': {
    params: {
      walletId: t.string({ required: true, min: 3, max: 64 })
    }
  },

  // ── Mail ──────────────────────────────────────────────────
  'mail.send': {
    body: {
      to:      t.string({ required: true, email: true }),
      subject: t.string({ required: true, min: 1, max: 256 }),
      body:    t.string({ required: true, min: 1, max: 50000 }),
      replyTo: t.uuid()
    }
  },

  // ── Social ────────────────────────────────────────────────
  'social.post': {
    body: {
      content:  t.string({ required: true, min: 1, max: 5000 }),
      type:     t.string({ enum: ['text', 'image', 'video', 'link'] }),
      tags:     t.array({ max: 10 }),
      language: t.string({ max: 5 })
    }
  },

  // ── Commerce ──────────────────────────────────────────────
  'commerce.create': {
    body: {
      name:        t.string({ required: true, min: 2, max: 200 }),
      description: t.string({ max: 5000 }),
      price:       t.wampumAmount({ required: true }),
      category:    t.string({ required: true, max: 64 }),
      stock:       t.number({ min: 0, integer: true })
    }
  },

  // ── Invest ────────────────────────────────────────────────
  'invest.order': {
    body: {
      assetId:  t.uuid({ required: true }),
      type:     t.string({ required: true, enum: ['buy', 'sell'] }),
      amount:   t.wampumAmount({ required: true }),
      price:    t.number({ required: true, min: 0 })
    }
  },

  // ── Search ────────────────────────────────────────────────
  'search.query': {
    query: {
      q:      t.string({ required: true, min: 1, max: 256 }),
      page:   t.number({ min: 1, integer: true }),
      limit:  t.number({ min: 1, max: 100, integer: true }),
      type:   t.string({ enum: ['all', 'platform', 'user', 'document', 'product'] }),
      lang:   t.string({ max: 5 })
    }
  },

  // ── Jobs ──────────────────────────────────────────────────
  'jobs.create': {
    body: {
      title:       t.string({ required: true, min: 5, max: 200 }),
      description: t.string({ required: true, min: 20, max: 10000 }),
      salary:      t.number({ min: 0 }),
      currency:    t.string({ enum: ['WMP', 'USD', 'EUR', 'BTC'] }),
      location:    t.string({ max: 200 }),
      remote:      t.boolean()
    }
  },

  // ── Docs ──────────────────────────────────────────────────
  'docs.create': {
    body: {
      title:    t.string({ required: true, min: 1, max: 256 }),
      content:  t.string({ required: true, min: 1 }),
      format:   t.string({ enum: ['markdown', 'html', 'text'] }),
      language: t.string({ max: 5 })
    }
  },

  // ── Chain (Blockchain) ────────────────────────────────────
  'chain.submit': {
    body: {
      type:     t.string({ required: true, enum: ['transaction', 'contract', 'governance'] }),
      data:     t.string({ required: true }),
      from:     t.walletAddress({ required: true }),
      gasLimit: t.number({ min: 21000, max: 10000000, integer: true })
    }
  },

  // ── AI ────────────────────────────────────────────────────
  'ai.inference': {
    body: {
      model:  t.string({ required: true, enum: ['sentiment', 'classification', 'translation', 'ner', 'image', 'embeddings'] }),
      input:  t.string({ required: true, min: 1, max: 10000 }),
      lang:   t.string({ max: 5 })
    }
  },

  // ── Voice ─────────────────────────────────────────────────
  'voice.create': {
    body: {
      channel: t.string({ required: true, min: 2, max: 64 }),
      codec:   t.string({ enum: ['opus', 'aac', 'pcm'] })
    }
  },

  // ── Edu ───────────────────────────────────────────────────
  'edu.enroll': {
    body: {
      courseId: t.uuid({ required: true }),
      tier:     t.string({ enum: ['free', 'member', 'resident', 'citizen'] })
    }
  },

  // ── Wiki ──────────────────────────────────────────────────
  'wiki.create': {
    body: {
      title:    t.string({ required: true, min: 3, max: 256 }),
      content:  t.string({ required: true, min: 10 }),
      language: t.string({ required: true, enum: ['es', 'en', 'qu', 'ay', 'gn', 'my', 'na', 'mp'] }),
      category: t.string({ max: 64 })
    }
  },

  // ── News ──────────────────────────────────────────────────
  'news.publish': {
    body: {
      title:    t.string({ required: true, min: 5, max: 256 }),
      content:  t.string({ required: true, min: 50 }),
      category: t.string({ required: true, enum: ['politics', 'economy', 'culture', 'technology', 'sovereignty', 'sports'] }),
      language: t.string({ max: 5 })
    }
  },

  // ── Map ───────────────────────────────────────────────────
  'map.create': {
    body: {
      name:     t.string({ required: true, min: 2, max: 200 }),
      type:     t.string({ required: true, enum: ['point', 'polygon', 'line', 'region'] }),
      lat:      t.number({ required: true, min: -90, max: 90 }),
      lng:      t.number({ required: true, min: -180, max: 180 }),
      category: t.string({ max: 64 })
    }
  },

  // ── Atabey (Governance) ───────────────────────────────────
  'atabey.proposal': {
    body: {
      title:       t.string({ required: true, min: 5, max: 256 }),
      description: t.string({ required: true, min: 20, max: 10000 }),
      type:        t.string({ required: true, enum: ['governance', 'funding', 'amendment', 'sovereignty'] }),
      votingEnd:   t.date({ required: true })
    }
  },
  'atabey.vote': {
    body: {
      proposalId: t.uuid({ required: true }),
      vote:       t.string({ required: true, enum: ['yes', 'no', 'abstain'] }),
      weight:     t.number({ min: 1, max: 100 })
    }
  },

  // ── Video ─────────────────────────────────────────────────
  'video.upload': {
    body: {
      title:       t.string({ required: true, min: 1, max: 256 }),
      description: t.string({ max: 5000 }),
      language:    t.string({ max: 5 }),
      category:    t.string({ max: 64 })
    }
  },

  // ── Music ─────────────────────────────────────────────────
  'music.upload': {
    body: {
      title:    t.string({ required: true, min: 1, max: 256 }),
      artist:   t.string({ required: true, min: 1, max: 200 }),
      genre:    t.string({ max: 64 }),
      language: t.string({ max: 5 })
    }
  },

  // ── Lodging ───────────────────────────────────────────────
  'lodging.create': {
    body: {
      name:        t.string({ required: true, min: 2, max: 200 }),
      location:    t.string({ required: true, max: 256 }),
      pricePerNight: t.wampumAmount({ required: true }),
      capacity:    t.number({ required: true, min: 1, max: 100, integer: true }),
      type:        t.string({ enum: ['hostel', 'hotel', 'cabin', 'community', 'eco-lodge'] })
    }
  },

  // ── Artisan ───────────────────────────────────────────────
  'artisan.create': {
    body: {
      name:        t.string({ required: true, min: 2, max: 200 }),
      description: t.string({ max: 5000 }),
      price:       t.wampumAmount({ required: true }),
      technique:   t.string({ max: 100 }),
      nation:      t.string({ max: 64 })
    }
  },

  // ── Renta (Rental) ────────────────────────────────────────
  'renta.create': {
    body: {
      title:       t.string({ required: true, min: 3, max: 200 }),
      description: t.string({ max: 5000 }),
      pricePerDay: t.wampumAmount({ required: true }),
      category:    t.string({ required: true, enum: ['vehicle', 'equipment', 'space', 'tools'] }),
      location:    t.string({ max: 256 })
    }
  },

  // ── Shared ────────────────────────────────────────────────
  pagination: schemas.pagination,
  idParam:    schemas.idParam
};

/**
 * Get validation middleware for a named schema
 * @param {string} schemaName - Key from routeSchemas
 * @returns {Function} Express validation middleware
 *
 * Usage:
 *   const { getValidator } = require('../middleware/validate');
 *   router.post('/transfer', getValidator('bdet.transfer'), handler);
 */
function getValidator(schemaName) {
  const schema = routeSchemas[schemaName];
  if (!schema) {
    throw new Error(`Unknown validation schema: ${schemaName}`);
  }
  return validate(schema);
}

/**
 * Global body sanitizer middleware
 * Trims strings, strips prototype pollution keys
 */
function globalSanitizer(req, res, next) {
  if (req.body && typeof req.body === 'object') {
    // Remove prototype pollution vectors
    delete req.body.__proto__;
    delete req.body.constructor;
    delete req.body.prototype;

    // Trim all string values at top level
    for (const [key, value] of Object.entries(req.body)) {
      if (typeof value === 'string') {
        req.body[key] = value.trim();
      }
    }
  }
  next();
}

// ============================================================
// Exports
// ============================================================
module.exports = {
  routeSchemas,
  getValidator,
  globalSanitizer,
  validate,
  sanitizeBody,
  t
};
