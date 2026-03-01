'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// User Profile Routes v1.0.0
// Profile management, public profiles, user search
// ============================================================

const { Router } = require('express');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:users');
const audit = createAuditLogger('sovereign-core:users');

// ============================================================
// GET /profile — Current user's full profile
// ============================================================
router.get('/profile', asyncHandler(async (req, res) => {
  if (!req.user) {
    throw new AppError('AUTH_REQUIRED', 'Authentication required');
  }

  const result = await db.query(
    `SELECT id, email, display_name, nation, language, role, status,
            bio, avatar_url, website, social_links,
            created_at, last_login_at, updated_at
     FROM users
     WHERE id = $1 AND status != 'deleted'`,
    [req.user.id]
  );

  if (result.rows.length === 0) {
    throw new AppError('NOT_FOUND', 'User profile not found');
  }

  // Fetch user stats
  const statsResult = await db.query(
    `SELECT
       (SELECT COUNT(*) FROM content WHERE author_id = $1 AND status != 'deleted') AS content_count,
       (SELECT COUNT(*) FROM transactions WHERE from_user = $1 OR to_user = $1) AS transaction_count,
       (SELECT COUNT(DISTINCT platform) FROM content WHERE author_id = $1 AND status != 'deleted') AS platforms_active
    `,
    [req.user.id]
  );

  const profile = result.rows[0];
  const stats = statsResult.rows[0];

  res.json({
    status: 'ok',
    data: {
      ...profile,
      stats: {
        contentCount: parseInt(stats.content_count, 10),
        transactionCount: parseInt(stats.transaction_count, 10),
        platformsActive: parseInt(stats.platforms_active, 10)
      }
    }
  });
}));

// ============================================================
// PUT /profile — Update current user's profile
// ============================================================
router.put('/profile',
  validate({
    body: {
      display_name: t.string({ min: 2, max: 100 }),
      nation: t.string({ max: 100 }),
      language: t.string({ enum: ['es', 'en', 'qu', 'ay', 'gn', 'my', 'na', 'mp'] }),
      bio: t.string({ max: 2000 }),
      avatar_url: t.string({ max: 500 }),
      website: t.string({ max: 500 }),
      social_links: t.string({})    // JSON string of social links
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required');
    }

    // Build dynamic SET clause from provided fields
    const allowedFields = ['display_name', 'nation', 'language', 'bio', 'avatar_url', 'website', 'social_links'];
    const updates = [];
    const values = [];
    let paramIdx = 1;

    for (const field of allowedFields) {
      if (req.body[field] !== undefined) {
        let value = req.body[field];

        // Handle social_links as JSONB
        if (field === 'social_links') {
          if (typeof value === 'string') {
            try { value = JSON.parse(value); } catch { value = {}; }
          }
          updates.push(`${field} = $${paramIdx}::jsonb`);
          values.push(JSON.stringify(value));
        } else {
          updates.push(`${field} = $${paramIdx}`);
          values.push(typeof value === 'string' ? value.trim() : value);
        }
        paramIdx++;
      }
    }

    if (updates.length === 0) {
      throw new AppError('INVALID_INPUT', 'No valid fields to update. Allowed: display_name, nation, language, bio, avatar_url, website, social_links');
    }

    updates.push(`updated_at = NOW()`);

    const result = await db.query(
      `UPDATE users SET ${updates.join(', ')}
       WHERE id = $${paramIdx} AND status != 'deleted'
       RETURNING id, email, display_name, nation, language, role, bio, avatar_url, website, social_links, updated_at`,
      [...values, req.user.id]
    );

    if (result.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'User profile not found');
    }

    audit.dataModify(req, 'user', req.user.id, { fields: Object.keys(req.body) });
    log.info('Profile updated', { userId: req.user.id, fields: Object.keys(req.body) });

    res.json({
      status: 'ok',
      data: result.rows[0]
    });
  })
);

// ============================================================
// GET /:id/public — Public profile for any user
// ============================================================
router.get('/:id/public',
  validate({ params: { id: t.uuid({ required: true }) } }),
  asyncHandler(async (req, res) => {
    const result = await db.query(
      `SELECT id, display_name, nation, bio, avatar_url, website, social_links, created_at
       FROM users
       WHERE id = $1 AND status = 'active'`,
      [req.params.id]
    );

    if (result.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'User not found');
    }

    // Fetch public stats
    const statsResult = await db.query(
      `SELECT
         (SELECT COUNT(*) FROM content WHERE author_id = $1 AND status = 'published') AS content_count,
         (SELECT COUNT(DISTINCT platform) FROM content WHERE author_id = $1 AND status = 'published') AS platforms_active
      `,
      [req.params.id]
    );

    const profile = result.rows[0];
    const stats = statsResult.rows[0];

    res.json({
      status: 'ok',
      data: {
        ...profile,
        stats: {
          contentCount: parseInt(stats.content_count, 10),
          platformsActive: parseInt(stats.platforms_active, 10)
        }
      }
    });
  })
);

// ============================================================
// Exports
// ============================================================
module.exports = router;
