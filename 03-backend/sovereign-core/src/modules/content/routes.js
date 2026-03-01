'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Content CRUD Routes v1.0.0
// Generic content management for any platform
// Supports pagination, JSONB bodies, soft deletes
// ============================================================

const { Router } = require('express');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');
const { requirePlatform } = require('../../middleware/platform');

const router = Router({ mergeParams: true });
const log = createLogger('sovereign-core:content');
const audit = createAuditLogger('sovereign-core:content');

// All content routes require a platform context
router.use(requirePlatform);

// ============================================================
// Helper: build pagination metadata
// ============================================================
function paginationMeta(totalRows, page, limit) {
  const totalPages = Math.ceil(totalRows / limit);
  return {
    page,
    limit,
    total: totalRows,
    totalPages,
    hasNext: page < totalPages,
    hasPrev: page > 1
  };
}

// ============================================================
// GET / — List content for a platform (paginated)
// ============================================================
router.get('/',
  asyncHandler(async (req, res) => {
    const platform = req.platform;
    const page = Math.max(1, parseInt(req.query.page, 10) || 1);
    const limit = Math.min(100, Math.max(1, parseInt(req.query.limit, 10) || 20));
    const offset = (page - 1) * limit;
    const type = req.query.type || null;
    const sort = req.query.sort || 'created_at:desc';

    // Parse sort parameter
    const [sortField, sortDir] = sort.split(':');
    const allowedSortFields = ['created_at', 'updated_at', 'title', 'type'];
    const safeSortField = allowedSortFields.includes(sortField) ? sortField : 'created_at';
    const safeSortDir = sortDir === 'asc' ? 'ASC' : 'DESC';

    // Build query
    const conditions = [`platform = $1`, `status != 'deleted'`];
    const params = [platform];
    let paramIdx = 2;

    if (type) {
      conditions.push(`type = $${paramIdx}`);
      params.push(type);
      paramIdx++;
    }

    const whereClause = conditions.join(' AND ');

    // Count total
    const countResult = await db.query(
      `SELECT COUNT(*) AS total FROM content WHERE ${whereClause}`,
      params
    );
    const total = parseInt(countResult.rows[0].total, 10);

    // Fetch page
    const dataResult = await db.query(
      `SELECT c.id, c.platform, c.type, c.title, c.body, c.metadata, c.status,
              c.author_id, u.display_name AS author_name,
              c.created_at, c.updated_at
       FROM content c
       LEFT JOIN users u ON u.id = c.author_id
       WHERE ${whereClause}
       ORDER BY c.${safeSortField} ${safeSortDir}
       LIMIT $${paramIdx} OFFSET $${paramIdx + 1}`,
      [...params, limit, offset]
    );

    res.json({
      status: 'ok',
      data: dataResult.rows,
      pagination: paginationMeta(total, page, limit)
    });
  })
);

// ============================================================
// POST / — Create new content
// ============================================================
router.post('/',
  validate({
    body: {
      type: t.string({ required: true, min: 1, max: 64 }),
      title: t.string({ required: true, min: 1, max: 500 }),
      body: t.string({}),         // Can also be JSONB — handled below
      metadata: t.string({})      // Optional JSON metadata string
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required to create content');
    }

    const platform = req.platform;
    const { type, title } = req.body;

    // Handle body: accept string or object (JSONB)
    let bodyValue = req.body.body;
    if (typeof bodyValue === 'object' && bodyValue !== null) {
      bodyValue = JSON.stringify(bodyValue);
    }

    // Handle metadata: accept string or object
    let metadataValue = req.body.metadata;
    if (typeof metadataValue === 'string') {
      try {
        metadataValue = JSON.parse(metadataValue);
      } catch {
        metadataValue = { raw: metadataValue };
      }
    }
    if (!metadataValue) {
      metadataValue = {};
    }

    const result = await db.query(
      `INSERT INTO content (platform, type, title, body, metadata, author_id, status, created_at, updated_at)
       VALUES ($1, $2, $3, $4, $5::jsonb, $6, 'published', NOW(), NOW())
       RETURNING id, platform, type, title, body, metadata, author_id, status, created_at`,
      [platform, type.trim(), title.trim(), bodyValue, JSON.stringify(metadataValue), req.user.id]
    );

    const content = result.rows[0];

    audit.dataModify(req, 'content', content.id, { action: 'create', platform, type });
    log.info('Content created', { contentId: content.id, platform, type });

    res.status(201).json({
      status: 'ok',
      data: content
    });
  })
);

// ============================================================
// GET /categories — Distinct content types for a platform
// ============================================================
router.get('/categories', asyncHandler(async (req, res) => {
  const result = await db.query(
    `SELECT DISTINCT type, COUNT(*) AS count
     FROM content
     WHERE platform = $1 AND status != 'deleted'
     GROUP BY type
     ORDER BY count DESC`,
    [req.platform]
  );

  res.json({
    status: 'ok',
    data: result.rows
  });
}));

// ============================================================
// GET /stats — Content statistics for a platform
// ============================================================
router.get('/stats', asyncHandler(async (req, res) => {
  const platform = req.platform;

  // Total content count
  const countResult = await db.query(
    `SELECT COUNT(*) AS total FROM content WHERE platform = $1 AND status != 'deleted'`,
    [platform]
  );

  // Latest content
  const latestResult = await db.query(
    `SELECT id, type, title, author_id, created_at
     FROM content
     WHERE platform = $1 AND status != 'deleted'
     ORDER BY created_at DESC
     LIMIT 5`,
    [platform]
  );

  // Content by type
  const typeResult = await db.query(
    `SELECT type, COUNT(*) AS count
     FROM content
     WHERE platform = $1 AND status != 'deleted'
     GROUP BY type
     ORDER BY count DESC`,
    [platform]
  );

  // Most active authors
  const authorsResult = await db.query(
    `SELECT c.author_id, u.display_name, COUNT(*) AS content_count
     FROM content c
     LEFT JOIN users u ON u.id = c.author_id
     WHERE c.platform = $1 AND c.status != 'deleted'
     GROUP BY c.author_id, u.display_name
     ORDER BY content_count DESC
     LIMIT 10`,
    [platform]
  );

  res.json({
    status: 'ok',
    data: {
      total: parseInt(countResult.rows[0].total, 10),
      latest: latestResult.rows,
      byType: typeResult.rows,
      topAuthors: authorsResult.rows
    }
  });
}));

// ============================================================
// GET /:id — Retrieve single content item
// ============================================================
router.get('/:id',
  validate({ params: { id: t.uuid({ required: true }) } }),
  asyncHandler(async (req, res) => {
    const result = await db.query(
      `SELECT c.id, c.platform, c.type, c.title, c.body, c.metadata, c.status,
              c.author_id, u.display_name AS author_name,
              c.created_at, c.updated_at
       FROM content c
       LEFT JOIN users u ON u.id = c.author_id
       WHERE c.id = $1 AND c.platform = $2 AND c.status != 'deleted'`,
      [req.params.id, req.platform]
    );

    if (result.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'Content not found');
    }

    res.json({
      status: 'ok',
      data: result.rows[0]
    });
  })
);

// ============================================================
// PUT /:id — Update content (author only)
// ============================================================
router.put('/:id',
  validate({
    params: { id: t.uuid({ required: true }) },
    body: {
      title: t.string({ min: 1, max: 500 }),
      body: t.string({}),
      type: t.string({ min: 1, max: 64 }),
      metadata: t.string({})
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required');
    }

    const contentId = req.params.id;

    // Verify ownership (admins can edit any content)
    const existing = await db.query(
      `SELECT id, author_id FROM content WHERE id = $1 AND platform = $2 AND status != 'deleted'`,
      [contentId, req.platform]
    );

    if (existing.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'Content not found');
    }

    if (existing.rows[0].author_id !== req.user.id && req.user.role !== 'admin') {
      throw new AppError('AUTH_INSUFFICIENT_ROLE', 'You can only edit your own content');
    }

    // Build dynamic SET clause
    const updates = [];
    const values = [];
    let paramIdx = 1;

    if (req.body.title !== undefined) {
      updates.push(`title = $${paramIdx}`);
      values.push(req.body.title.trim());
      paramIdx++;
    }

    if (req.body.body !== undefined) {
      const bodyVal = typeof req.body.body === 'object' ? JSON.stringify(req.body.body) : req.body.body;
      updates.push(`body = $${paramIdx}`);
      values.push(bodyVal);
      paramIdx++;
    }

    if (req.body.type !== undefined) {
      updates.push(`type = $${paramIdx}`);
      values.push(req.body.type.trim());
      paramIdx++;
    }

    if (req.body.metadata !== undefined) {
      let meta = req.body.metadata;
      if (typeof meta === 'string') {
        try { meta = JSON.parse(meta); } catch { meta = { raw: meta }; }
      }
      updates.push(`metadata = $${paramIdx}::jsonb`);
      values.push(JSON.stringify(meta));
      paramIdx++;
    }

    if (updates.length === 0) {
      throw new AppError('INVALID_INPUT', 'No fields to update');
    }

    updates.push(`updated_at = NOW()`);

    const result = await db.query(
      `UPDATE content SET ${updates.join(', ')}
       WHERE id = $${paramIdx} AND platform = $${paramIdx + 1}
       RETURNING id, platform, type, title, body, metadata, status, author_id, created_at, updated_at`,
      [...values, contentId, req.platform]
    );

    audit.dataModify(req, 'content', contentId, { action: 'update', fields: Object.keys(req.body) });
    log.info('Content updated', { contentId, platform: req.platform });

    res.json({
      status: 'ok',
      data: result.rows[0]
    });
  })
);

// ============================================================
// DELETE /:id — Soft-delete content (author or admin)
// ============================================================
router.delete('/:id',
  validate({ params: { id: t.uuid({ required: true }) } }),
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required');
    }

    const contentId = req.params.id;

    // Verify existence and ownership
    const existing = await db.query(
      `SELECT id, author_id FROM content WHERE id = $1 AND platform = $2 AND status != 'deleted'`,
      [contentId, req.platform]
    );

    if (existing.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'Content not found');
    }

    if (existing.rows[0].author_id !== req.user.id && req.user.role !== 'admin') {
      throw new AppError('AUTH_INSUFFICIENT_ROLE', 'You can only delete your own content');
    }

    // Soft delete
    await db.query(
      `UPDATE content SET status = 'deleted', updated_at = NOW() WHERE id = $1`,
      [contentId]
    );

    audit.dataModify(req, 'content', contentId, { action: 'soft_delete' });
    log.info('Content soft-deleted', { contentId, platform: req.platform });

    res.json({
      status: 'ok',
      message: 'Content deleted'
    });
  })
);

// ============================================================
// Exports
// ============================================================
module.exports = router;
