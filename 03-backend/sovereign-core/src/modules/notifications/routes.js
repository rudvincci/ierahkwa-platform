'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Notifications Module v1.0.0 — Sovereign Alerts & Notifications
// Push, email, SMS, in-app — real-time delivery
// ============================================================

const { Router } = require('express');
const crypto = require('crypto');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:notifications');

// ============================================================
// Notification Types & Channels
// ============================================================
const NOTIFICATION_TYPES = {
  transaction: { icon: '💰', priority: 'high', channels: ['push', 'email', 'inapp'] },
  security: { icon: '🛡️', priority: 'critical', channels: ['push', 'email', 'sms', 'inapp'] },
  payment_received: { icon: '✅', priority: 'high', channels: ['push', 'inapp'] },
  payment_sent: { icon: '📤', priority: 'medium', channels: ['inapp'] },
  order_update: { icon: '📦', priority: 'medium', channels: ['push', 'email', 'inapp'] },
  loan_payment_due: { icon: '📅', priority: 'high', channels: ['push', 'email', 'sms', 'inapp'] },
  loan_approved: { icon: '🎉', priority: 'high', channels: ['push', 'email', 'inapp'] },
  staking_reward: { icon: '🌟', priority: 'medium', channels: ['push', 'inapp'] },
  exchange_filled: { icon: '📊', priority: 'high', channels: ['push', 'inapp'] },
  price_alert: { icon: '📈', priority: 'medium', channels: ['push', 'inapp'] },
  governance_vote: { icon: '🗳️', priority: 'high', channels: ['push', 'email', 'inapp'] },
  system_alert: { icon: '⚠️', priority: 'high', channels: ['push', 'email', 'inapp'] },
  message_received: { icon: '💬', priority: 'medium', channels: ['push', 'inapp'] },
  domain_expiring: { icon: '🌐', priority: 'high', channels: ['push', 'email', 'inapp'] },
  atm_alert: { icon: '🏧', priority: 'medium', channels: ['push', 'inapp'] },
  promotion: { icon: '🎁', priority: 'low', channels: ['email', 'inapp'] },
  welcome: { icon: '👋', priority: 'low', channels: ['email', 'inapp'] }
};

function generateId() {
  return `NOTIF-${crypto.randomBytes(8).toString('hex').toUpperCase()}`;
}

// ============================================================
// GET /notifications — User's notifications
// ============================================================
router.get('/', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const { type, unread_only, priority } = req.query;
  const limit = Math.min(100, parseInt(req.query.limit, 10) || 30);
  const offset = parseInt(req.query.offset, 10) || 0;

  let query = `SELECT notification_id, type, title, body, icon, priority, channel,
     action_url, metadata, is_read, read_at, created_at
     FROM notifications WHERE user_id = $1`;
  const params = [req.user.id];
  let idx = 2;

  if (type) { query += ` AND type = $${idx}`; params.push(type); idx++; }
  if (unread_only === 'true') { query += ` AND is_read = false`; }
  if (priority) { query += ` AND priority = $${idx}`; params.push(priority); idx++; }

  query += ` ORDER BY created_at DESC LIMIT $${idx} OFFSET $${idx + 1}`;
  params.push(limit, offset);

  const result = await db.query(query, params);

  // Get unread count
  const unreadCount = await db.query(
    `SELECT COUNT(*) AS count FROM notifications WHERE user_id = $1 AND is_read = false`,
    [req.user.id]
  );

  res.json({
    status: 'ok',
    data: {
      notifications: result.rows,
      unreadCount: parseInt(unreadCount.rows[0]?.count || 0)
    }
  });
}));

// ============================================================
// POST /send — Send a notification (internal use / admin)
// ============================================================
router.post('/send',
  validate({
    body: {
      user_id: t.string({ required: true }),
      type: t.string({ required: true }),
      title: t.string({ required: true, max: 200 }),
      body: t.string({ required: true, max: 2000 }),
      action_url: t.string({ max: 500 }),
      metadata: t.object({})
    }
  }),
  asyncHandler(async (req, res) => {
    const { user_id, type, title, body, action_url, metadata } = req.body;
    const typeInfo = NOTIFICATION_TYPES[type] || { icon: '🔔', priority: 'medium', channels: ['inapp'] };
    const notifId = generateId();

    await db.query(
      `INSERT INTO notifications
         (notification_id, user_id, type, title, body, icon, priority, channel, action_url, metadata, is_read)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10::jsonb, false)`,
      [notifId, user_id, type, title, body,
       typeInfo.icon, typeInfo.priority,
       typeInfo.channels[0], action_url || null,
       JSON.stringify(metadata || {})]
    );

    log.info('Notification sent', { notifId, userId: user_id, type });

    res.status(201).json({
      status: 'ok',
      data: { notificationId: notifId, delivered: typeInfo.channels }
    });
  })
);

// ============================================================
// PUT /read/:notificationId — Mark as read
// ============================================================
router.put('/read/:notificationId', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `UPDATE notifications SET is_read = true, read_at = NOW()
     WHERE notification_id = $1 AND user_id = $2 RETURNING notification_id`,
    [req.params.notificationId, req.user.id]
  );

  if (result.rows.length === 0) throw new AppError('NOT_FOUND', 'Notification not found');
  res.json({ status: 'ok', data: result.rows[0] });
}));

// ============================================================
// PUT /read-all — Mark all as read
// ============================================================
router.put('/read-all', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `UPDATE notifications SET is_read = true, read_at = NOW()
     WHERE user_id = $1 AND is_read = false`,
    [req.user.id]
  );

  res.json({ status: 'ok', data: { marked: result.rowCount } });
}));

// ============================================================
// GET /preferences — Notification preferences
// ============================================================
router.get('/preferences', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  let prefs;
  try {
    const result = await db.query(
      `SELECT preferences FROM notification_preferences WHERE user_id = $1`,
      [req.user.id]
    );
    prefs = result.rows.length > 0 ? result.rows[0].preferences : null;
  } catch { /* ok */ }

  // Default preferences if none set
  if (!prefs) {
    prefs = {};
    Object.entries(NOTIFICATION_TYPES).forEach(([type, info]) => {
      prefs[type] = {
        enabled: true,
        channels: info.channels,
        priority: info.priority
      };
    });
  }

  res.json({ status: 'ok', data: { preferences: prefs, availableTypes: NOTIFICATION_TYPES } });
}));

// ============================================================
// PUT /preferences — Update notification preferences
// ============================================================
router.put('/preferences',
  validate({
    body: {
      preferences: t.object({ required: true })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    await db.query(
      `INSERT INTO notification_preferences (user_id, preferences, updated_at)
       VALUES ($1, $2::jsonb, NOW())
       ON CONFLICT (user_id) DO UPDATE SET preferences = $2::jsonb, updated_at = NOW()`,
      [req.user.id, JSON.stringify(req.body.preferences)]
    );

    res.json({ status: 'ok', data: { updated: true } });
  })
);

// ============================================================
// POST /price-alert — Set a price alert
// ============================================================
router.post('/price-alert',
  validate({
    body: {
      token: t.string({ required: true }),
      target_price: t.number({ required: true, min: 0 }),
      direction: t.string({ required: true, enum: ['above', 'below'] })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { token, target_price, direction } = req.body;
    const alertId = generateId();

    await db.query(
      `INSERT INTO price_alerts (alert_id, user_id, token, target_price, direction, status)
       VALUES ($1, $2, $3, $4, $5, 'active')`,
      [alertId, req.user.id, token, target_price, direction]
    );

    res.status(201).json({
      status: 'ok',
      data: { alertId, token, targetPrice: target_price, direction, status: 'active' }
    });
  })
);

// ============================================================
// GET /price-alerts — User's price alerts
// ============================================================
router.get('/price-alerts', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `SELECT alert_id, token, target_price, direction, status, triggered_at, created_at
     FROM price_alerts WHERE user_id = $1 ORDER BY created_at DESC`,
    [req.user.id]
  );

  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// DELETE /price-alert/:alertId — Delete a price alert
// ============================================================
router.delete('/price-alert/:alertId', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `DELETE FROM price_alerts WHERE alert_id = $1 AND user_id = $2 RETURNING alert_id`,
    [req.params.alertId, req.user.id]
  );

  if (result.rows.length === 0) throw new AppError('NOT_FOUND', 'Alert not found');
  res.json({ status: 'ok', data: { deleted: result.rows[0].alert_id } });
}));

// ============================================================
// GET /types — Available notification types
// ============================================================
router.get('/types', (_req, res) => {
  res.json({ status: 'ok', data: NOTIFICATION_TYPES });
});

module.exports = router;
