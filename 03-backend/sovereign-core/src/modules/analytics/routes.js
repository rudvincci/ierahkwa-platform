'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Analytics Routes v1.0.0
// Platform-level analytics summaries
// Content counts, user activity, transaction volume
// ============================================================

const { Router } = require('express');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:analytics');

// ============================================================
// GET /:platform/summary — Full platform summary
// ============================================================
router.get('/:platform/summary',
  validate({ params: { platform: t.string({ required: true, min: 2, max: 64 }) } }),
  asyncHandler(async (req, res) => {
    const platform = req.params.platform.toLowerCase();
    const days = Math.min(365, Math.max(1, parseInt(req.query.days, 10) || 30));
    const sinceDate = new Date(Date.now() - days * 24 * 60 * 60 * 1000).toISOString();

    // Execute all analytics queries in parallel
    const [contentStats, userStats, txStats, recentActivity] = await Promise.all([
      // Content statistics
      db.query(
        `SELECT
           COUNT(*) AS total_content,
           COUNT(*) FILTER (WHERE created_at >= $2) AS recent_content,
           COUNT(DISTINCT type) AS content_types,
           COUNT(DISTINCT author_id) AS unique_authors
         FROM content
         WHERE platform = $1 AND status != 'deleted'`,
        [platform, sinceDate]
      ),

      // User statistics (users who have content on this platform)
      db.query(
        `SELECT
           COUNT(DISTINCT c.author_id) AS active_users,
           COUNT(DISTINCT c.author_id) FILTER (WHERE c.created_at >= $2) AS recently_active_users
         FROM content c
         WHERE c.platform = $1 AND c.status != 'deleted'`,
        [platform, sinceDate]
      ),

      // Transaction statistics (if platform is tracked in transactions metadata)
      db.query(
        `SELECT
           COUNT(*) AS total_transactions,
           COALESCE(SUM(amount), 0) AS total_volume,
           COUNT(*) FILTER (WHERE created_at >= $2) AS recent_transactions,
           COALESCE(SUM(amount) FILTER (WHERE created_at >= $2), 0) AS recent_volume
         FROM transactions
         WHERE status = 'completed'
           AND (metadata->>'platform' = $1 OR metadata->>'content_platform' = $1)`,
        [platform, sinceDate]
      ),

      // Recent activity timeline (last 10 events)
      db.query(
        `SELECT 'content' AS event_type, c.id, c.title AS label, c.type AS subtype, c.created_at,
                u.display_name AS actor_name
         FROM content c
         LEFT JOIN users u ON u.id = c.author_id
         WHERE c.platform = $1 AND c.status != 'deleted'
         ORDER BY c.created_at DESC
         LIMIT 10`,
        [platform]
      )
    ]);

    const content = contentStats.rows[0];
    const users = userStats.rows[0];
    const transactions = txStats.rows[0];

    log.debug('Analytics summary generated', { platform, days });

    res.json({
      status: 'ok',
      data: {
        platform,
        period: {
          days,
          since: sinceDate
        },
        content: {
          total: parseInt(content.total_content, 10),
          recentCount: parseInt(content.recent_content, 10),
          contentTypes: parseInt(content.content_types, 10),
          uniqueAuthors: parseInt(content.unique_authors, 10)
        },
        users: {
          activeUsers: parseInt(users.active_users, 10),
          recentlyActive: parseInt(users.recently_active_users, 10)
        },
        transactions: {
          total: parseInt(transactions.total_transactions, 10),
          totalVolume: parseFloat(transactions.total_volume),
          recentCount: parseInt(transactions.recent_transactions, 10),
          recentVolume: parseFloat(transactions.recent_volume)
        },
        recentActivity: recentActivity.rows
      }
    });
  })
);

// ============================================================
// GET /:platform/users — User activity breakdown for platform
// ============================================================
router.get('/:platform/users',
  validate({ params: { platform: t.string({ required: true, min: 2, max: 64 }) } }),
  asyncHandler(async (req, res) => {
    const platform = req.params.platform.toLowerCase();
    const page = Math.max(1, parseInt(req.query.page, 10) || 1);
    const limit = Math.min(100, Math.max(1, parseInt(req.query.limit, 10) || 20));
    const offset = (page - 1) * limit;

    // Count total active users on this platform
    const countResult = await db.query(
      `SELECT COUNT(DISTINCT c.author_id) AS total
       FROM content c
       WHERE c.platform = $1 AND c.status != 'deleted'`,
      [platform]
    );
    const total = parseInt(countResult.rows[0].total, 10);

    // Fetch user activity breakdown (paginated)
    const dataResult = await db.query(
      `SELECT
         c.author_id AS user_id,
         u.display_name,
         u.nation,
         u.avatar_url,
         COUNT(*) AS content_count,
         COUNT(DISTINCT c.type) AS content_types,
         MIN(c.created_at) AS first_activity,
         MAX(c.created_at) AS last_activity
       FROM content c
       LEFT JOIN users u ON u.id = c.author_id
       WHERE c.platform = $1 AND c.status != 'deleted'
       GROUP BY c.author_id, u.display_name, u.nation, u.avatar_url
       ORDER BY content_count DESC
       LIMIT $2 OFFSET $3`,
      [platform, limit, offset]
    );

    res.json({
      status: 'ok',
      data: dataResult.rows,
      pagination: {
        page,
        limit,
        total,
        totalPages: Math.ceil(total / limit),
        hasNext: page < Math.ceil(total / limit),
        hasPrev: page > 1
      }
    });
  })
);

// ============================================================
// Exports
// ============================================================
module.exports = router;
