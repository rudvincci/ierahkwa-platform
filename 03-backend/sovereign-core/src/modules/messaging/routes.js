'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Messaging Routes v1.0.0
// Internal messaging system with threaded conversations
// ============================================================

const { Router } = require('express');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:messaging');
const audit = createAuditLogger('sovereign-core:messaging');

// ============================================================
// POST /send — Send a new message
// ============================================================
router.post('/send',
  validate({
    body: {
      to_user: t.uuid({ required: true }),
      subject: t.string({ required: true, min: 1, max: 300 }),
      body: t.string({ required: true, min: 1, max: 50000 }),
      thread_id: t.uuid({}),
      platform: t.string({ max: 64 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required');
    }

    const fromUser = req.user.id;
    const { to_user, subject, body, thread_id, platform } = req.body;

    // Cannot message yourself
    if (fromUser === to_user) {
      throw new AppError('INVALID_INPUT', 'Cannot send a message to yourself');
    }

    // Verify recipient exists and is active
    const recipientCheck = await db.query(
      `SELECT id, status FROM users WHERE id = $1`,
      [to_user]
    );

    if (recipientCheck.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'Recipient user not found');
    }

    if (recipientCheck.rows[0].status !== 'active') {
      throw new AppError('INVALID_INPUT', 'Recipient account is not active');
    }

    // If thread_id is provided, verify it exists and user is a participant
    if (thread_id) {
      const threadCheck = await db.query(
        `SELECT id FROM messages
         WHERE thread_id = $1 AND (from_user = $2 OR to_user = $2)
         LIMIT 1`,
        [thread_id, fromUser]
      );

      if (threadCheck.rows.length === 0) {
        throw new AppError('NOT_FOUND', 'Thread not found or you are not a participant');
      }
    }

    // Use provided thread_id or generate a new one for top-level messages
    const crypto = require('crypto');
    const resolvedThreadId = thread_id || crypto.randomUUID();

    const result = await db.query(
      `INSERT INTO messages (from_user, to_user, subject, body, thread_id, platform, is_read, created_at)
       VALUES ($1, $2, $3, $4, $5, $6, false, NOW())
       RETURNING id, from_user, to_user, subject, body, thread_id, platform, is_read, created_at`,
      [fromUser, to_user, subject.trim(), body.trim(), resolvedThreadId, platform || null]
    );

    const message = result.rows[0];

    audit.record({
      category: 'DATA_CREATE',
      action: 'message_sent',
      risk: 'LOW',
      req,
      resource: { type: 'message', id: message.id },
      details: { to: to_user, threadId: resolvedThreadId }
    });

    log.info('Message sent', { messageId: message.id, from: fromUser, to: to_user, threadId: resolvedThreadId });

    res.status(201).json({
      status: 'ok',
      data: message
    });
  })
);

// ============================================================
// GET /inbox — User's inbox (paginated)
// ============================================================
router.get('/inbox',
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required');
    }

    const userId = req.user.id;
    const page = Math.max(1, parseInt(req.query.page, 10) || 1);
    const limit = Math.min(100, Math.max(1, parseInt(req.query.limit, 10) || 20));
    const offset = (page - 1) * limit;
    const unreadOnly = req.query.unread === 'true';

    // Build conditions
    const conditions = [`m.to_user = $1`];
    const params = [userId];
    let paramIdx = 2;

    if (unreadOnly) {
      conditions.push(`m.is_read = false`);
    }

    const whereClause = conditions.join(' AND ');

    // Count total
    const countResult = await db.query(
      `SELECT COUNT(*) AS total FROM messages m WHERE ${whereClause}`,
      params
    );
    const total = parseInt(countResult.rows[0].total, 10);

    // Count unread
    const unreadResult = await db.query(
      `SELECT COUNT(*) AS unread FROM messages WHERE to_user = $1 AND is_read = false`,
      [userId]
    );
    const unreadCount = parseInt(unreadResult.rows[0].unread, 10);

    // Fetch messages
    const dataResult = await db.query(
      `SELECT m.id, m.from_user, m.to_user, m.subject, m.body, m.thread_id,
              m.platform, m.is_read, m.created_at,
              u.display_name AS from_name, u.avatar_url AS from_avatar
       FROM messages m
       LEFT JOIN users u ON u.id = m.from_user
       WHERE ${whereClause}
       ORDER BY m.created_at DESC
       LIMIT $${paramIdx} OFFSET $${paramIdx + 1}`,
      [...params, limit, offset]
    );

    res.json({
      status: 'ok',
      data: dataResult.rows,
      unreadCount,
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
// GET /thread/:threadId — All messages in a thread
// ============================================================
router.get('/thread/:threadId',
  validate({ params: { threadId: t.uuid({ required: true }) } }),
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required');
    }

    const userId = req.user.id;
    const { threadId } = req.params;

    // Verify user is a participant in this thread
    const participantCheck = await db.query(
      `SELECT id FROM messages
       WHERE thread_id = $1 AND (from_user = $2 OR to_user = $2)
       LIMIT 1`,
      [threadId, userId]
    );

    if (participantCheck.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'Thread not found or you are not a participant');
    }

    // Fetch all messages in thread, ordered chronologically
    const result = await db.query(
      `SELECT m.id, m.from_user, m.to_user, m.subject, m.body, m.thread_id,
              m.platform, m.is_read, m.created_at,
              u.display_name AS from_name, u.avatar_url AS from_avatar
       FROM messages m
       LEFT JOIN users u ON u.id = m.from_user
       WHERE m.thread_id = $1
       ORDER BY m.created_at ASC`,
      [threadId]
    );

    // Auto-mark messages sent TO this user as read
    await db.query(
      `UPDATE messages SET is_read = true
       WHERE thread_id = $1 AND to_user = $2 AND is_read = false`,
      [threadId, userId]
    );

    res.json({
      status: 'ok',
      data: result.rows,
      threadId,
      messageCount: result.rows.length
    });
  })
);

// ============================================================
// PUT /:id/read — Mark a specific message as read
// ============================================================
router.put('/:id/read',
  validate({ params: { id: t.uuid({ required: true }) } }),
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required');
    }

    const messageId = req.params.id;
    const userId = req.user.id;

    // Verify message belongs to user and update
    const result = await db.query(
      `UPDATE messages SET is_read = true
       WHERE id = $1 AND to_user = $2
       RETURNING id, from_user, to_user, subject, is_read, thread_id, created_at`,
      [messageId, userId]
    );

    if (result.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'Message not found or you are not the recipient');
    }

    log.debug('Message marked as read', { messageId, userId });

    res.json({
      status: 'ok',
      data: result.rows[0]
    });
  })
);

// ============================================================
// Exports
// ============================================================
module.exports = router;
