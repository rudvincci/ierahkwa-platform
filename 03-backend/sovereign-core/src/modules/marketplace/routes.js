'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Marketplace Module v1.0.0 — Sovereign Commerce
// Buy/sell goods & services with WPM/BDET
// P2P + Store + Auction — MameyNode settlement
// ============================================================

const { Router } = require('express');
const crypto = require('crypto');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:marketplace');
const audit = createAuditLogger('sovereign-core:marketplace');

const PLATFORM_TREASURY_ID = process.env.PLATFORM_TREASURY_ID || 'system-treasury';
const COMMISSION_RATE = 0.05; // 5% platform commission
const ESCROW_TIMEOUT_HOURS = 72;

const CATEGORIES = [
  'artesanias', 'alimentos', 'medicina-natural', 'tecnologia', 'servicios',
  'educacion', 'textiles', 'agricultura', 'construccion', 'transporte',
  'arte', 'musica', 'herramientas', 'animales', 'tierra',
  'joyeria', 'ceramica', 'digital', 'consultoria', 'turismo'
];

function generateId(prefix) {
  return `${prefix}-${crypto.randomBytes(8).toString('hex').toUpperCase()}`;
}

// ============================================================
// POST /listing — Create a product/service listing
// ============================================================
router.post('/listing',
  validate({
    body: {
      title: t.string({ required: true, max: 200 }),
      description: t.string({ required: true, max: 5000 }),
      category: t.string({ required: true }),
      price: t.number({ required: true, min: 0.01 }),
      currency: t.string({ enum: ['WMP', 'BDET', 'IGT', 'USD'] }),
      quantity: t.number({ min: 0 }),
      listing_type: t.string({ enum: ['fixed', 'auction', 'service', 'barter'] }),
      condition: t.string({ enum: ['new', 'like_new', 'good', 'fair', 'parts'] }),
      location: t.string({ max: 200 }),
      nation_id: t.string({ max: 10 }),
      shipping: t.string({ enum: ['local', 'national', 'international', 'digital', 'pickup'] }),
      images: t.string({ max: 2000 }),
      tags: t.string({ max: 500 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const {
      title, description, category, price, currency, quantity,
      listing_type, condition, location, nation_id, shipping, images, tags
    } = req.body;

    const listingId = generateId('LST');

    await db.query(
      `INSERT INTO marketplace_listings
         (listing_id, seller_id, title, description, category, price, currency,
          quantity, listing_type, condition, location, nation_id, shipping,
          images, tags, status)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15, 'active')`,
      [listingId, req.user.id, title, description, category,
       price, currency || 'WMP', quantity || 1,
       listing_type || 'fixed', condition || 'new',
       location || null, nation_id || null,
       shipping || 'local',
       images || null,
       tags || null]
    );

    log.info('Listing created', { listingId, category, price });

    res.status(201).json({
      status: 'ok',
      data: { listingId, title, price, currency: currency || 'WMP', status: 'active' }
    });
  })
);

// ============================================================
// GET /listings — Browse marketplace
// ============================================================
router.get('/listings', asyncHandler(async (req, res) => {
  const { category, search, min_price, max_price, currency, nation_id, shipping, sort } = req.query;
  const limit = Math.min(100, parseInt(req.query.limit, 10) || 20);
  const offset = parseInt(req.query.offset, 10) || 0;

  let query = `SELECT l.listing_id, l.title, l.description, l.category, l.price, l.currency,
     l.quantity, l.listing_type, l.condition, l.location, l.nation_id, l.shipping,
     l.images, l.tags, l.views, l.favorites, l.status, l.created_at,
     u.display_name AS seller_name
     FROM marketplace_listings l
     LEFT JOIN users u ON u.id = l.seller_id
     WHERE l.status = 'active'`;
  const params = [];
  let idx = 1;

  if (category) { query += ` AND l.category = $${idx}`; params.push(category); idx++; }
  if (search) { query += ` AND (l.title ILIKE $${idx} OR l.description ILIKE $${idx} OR l.tags ILIKE $${idx})`; params.push(`%${search}%`); idx++; }
  if (min_price) { query += ` AND l.price >= $${idx}`; params.push(parseFloat(min_price)); idx++; }
  if (max_price) { query += ` AND l.price <= $${idx}`; params.push(parseFloat(max_price)); idx++; }
  if (currency) { query += ` AND l.currency = $${idx}`; params.push(currency); idx++; }
  if (nation_id) { query += ` AND l.nation_id = $${idx}`; params.push(nation_id); idx++; }
  if (shipping) { query += ` AND l.shipping = $${idx}`; params.push(shipping); idx++; }

  const sortMap = { price_asc: 'l.price ASC', price_desc: 'l.price DESC', newest: 'l.created_at DESC', popular: 'l.views DESC' };
  query += ` ORDER BY ${sortMap[sort] || 'l.created_at DESC'}`;
  query += ` LIMIT $${idx} OFFSET $${idx + 1}`;
  params.push(limit, offset);

  const result = await db.query(query, params);

  res.json({ status: 'ok', data: { listings: result.rows, categories: CATEGORIES } });
}));

// ============================================================
// GET /listings/:listingId — Listing details
// ============================================================
router.get('/listings/:listingId', asyncHandler(async (req, res) => {
  const result = await db.query(
    `SELECT l.*, u.display_name AS seller_name, u.id AS seller_id
     FROM marketplace_listings l
     LEFT JOIN users u ON u.id = l.seller_id
     WHERE l.listing_id = $1`,
    [req.params.listingId]
  );
  if (result.rows.length === 0) throw new AppError('NOT_FOUND', 'Listing not found');

  // Increment views
  await db.query(`UPDATE marketplace_listings SET views = views + 1 WHERE listing_id = $1`, [req.params.listingId]);

  // Get seller's other listings
  const otherListings = await db.query(
    `SELECT listing_id, title, price, currency, images FROM marketplace_listings
     WHERE seller_id = $1 AND listing_id != $2 AND status = 'active' LIMIT 4`,
    [result.rows[0].seller_id, req.params.listingId]
  );

  res.json({
    status: 'ok',
    data: { listing: result.rows[0], sellerOtherListings: otherListings.rows }
  });
}));

// ============================================================
// POST /order — Place an order (buy)
// ============================================================
router.post('/order',
  validate({
    body: {
      listing_id: t.string({ required: true }),
      quantity: t.number({ min: 1 }),
      shipping_address: t.string({ max: 500 }),
      note: t.string({ max: 500 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { listing_id, quantity, shipping_address, note } = req.body;
    const qty = quantity || 1;

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Get listing
      const listing = await client.query(
        `SELECT * FROM marketplace_listings WHERE listing_id = $1 AND status = 'active' FOR UPDATE`,
        [listing_id]
      );
      if (listing.rows.length === 0) throw new AppError('NOT_FOUND', 'Listing not found or sold');
      const item = listing.rows[0];

      if (item.seller_id === req.user.id) throw new AppError('INVALID_INPUT', 'Cannot buy your own listing');
      if (item.quantity < qty) throw new AppError('INVALID_INPUT', `Only ${item.quantity} available`);

      const subtotal = parseFloat(item.price) * qty;
      const commission = subtotal * COMMISSION_RATE;
      const total = subtotal + commission;

      // Check buyer balance
      const buyerBalance = await client.query(
        `SELECT account_number, balance FROM bank_accounts
         WHERE user_id = $1 AND currency = $2 AND status = 'active' AND balance >= $3
         ORDER BY balance DESC LIMIT 1 FOR UPDATE`,
        [req.user.id, item.currency, total]
      );
      if (!buyerBalance.rows.length) {
        throw new AppError('INSUFFICIENT_FUNDS', `Need ${total} ${item.currency}`);
      }

      const orderId = generateId('ORD');
      const txHash = crypto.createHash('sha256')
        .update(`order:${orderId}:${total}:${Date.now()}`).digest('hex');

      // Debit buyer → escrow
      await client.query(
        `UPDATE bank_accounts SET balance = balance - $1, updated_at = NOW() WHERE account_number = $2`,
        [total, buyerBalance.rows[0].account_number]
      );

      // Create order
      await client.query(
        `INSERT INTO marketplace_orders
           (order_id, listing_id, buyer_id, seller_id, quantity, unit_price, subtotal,
            commission, total, currency, shipping_address, note, tx_hash, status)
         VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, 'paid')`,
        [orderId, listing_id, req.user.id, item.seller_id,
         qty, item.price, subtotal, commission, total, item.currency,
         shipping_address || null, note || null, txHash]
      );

      // Update listing quantity
      const newQty = item.quantity - qty;
      if (newQty <= 0) {
        await client.query(`UPDATE marketplace_listings SET status = 'sold', quantity = 0 WHERE listing_id = $1`, [listing_id]);
      } else {
        await client.query(`UPDATE marketplace_listings SET quantity = $1 WHERE listing_id = $2`, [newQty, listing_id]);
      }

      // Record escrow transaction
      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at)
         VALUES ($1, 'escrow-marketplace', $2, $3, 'marketplace_purchase', $4, $5, 'completed', NOW())`,
        [req.user.id, total, item.currency, `Marketplace order: ${item.title}`, txHash]
      );

      await client.query('COMMIT');

      audit.record({
        category: 'MARKETPLACE',
        action: 'order_placed',
        userId: req.user.id,
        risk: total > 1000 ? 'HIGH' : 'MEDIUM',
        details: { orderId, listingId: listing_id, total, currency: item.currency }
      });

      res.status(201).json({
        status: 'ok',
        data: {
          orderId,
          listing: { id: listing_id, title: item.title },
          quantity: qty,
          subtotal,
          commission,
          total,
          currency: item.currency,
          status: 'paid',
          escrow: { held: true, releaseCondition: 'buyer_confirms_receipt', timeoutHours: ESCROW_TIMEOUT_HOURS },
          txHash,
          blockchain: { network: 'MameyNode', chainId: 574 }
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      throw new AppError('ORDER_FAILED', 'Order could not be placed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// POST /order/:orderId/confirm — Buyer confirms receipt
// ============================================================
router.post('/order/:orderId/confirm', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const client = await db.getClient();
  try {
    await client.query('BEGIN');

    const order = await client.query(
      `SELECT * FROM marketplace_orders WHERE order_id = $1 AND buyer_id = $2 AND status = 'paid' FOR UPDATE`,
      [req.params.orderId, req.user.id]
    );
    if (order.rows.length === 0) throw new AppError('NOT_FOUND', 'Order not found or already confirmed');
    const o = order.rows[0];

    // Release escrow to seller
    const sellerPayout = parseFloat(o.subtotal);
    const platformFee = parseFloat(o.commission);

    // Credit seller
    await client.query(
      `UPDATE bank_accounts SET balance = balance + $1, updated_at = NOW()
       WHERE user_id = $2 AND currency = $3 AND status = 'active'`,
      [sellerPayout, o.seller_id, o.currency]
    );

    // Platform fee
    const feeHash = crypto.createHash('sha256').update(`marketplace-fee:${o.order_id}:${Date.now()}`).digest('hex');
    await client.query(
      `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at)
       VALUES ('escrow-marketplace', $1, $2, $3, 'marketplace_commission', $4, $5, 'completed', NOW())`,
      [PLATFORM_TREASURY_ID, platformFee, o.currency, `Marketplace commission (5%) for order ${o.order_id}`, feeHash]
    );

    // Update order status
    await client.query(
      `UPDATE marketplace_orders SET status = 'completed', completed_at = NOW() WHERE order_id = $1`,
      [req.params.orderId]
    );

    await client.query('COMMIT');

    res.json({
      status: 'ok',
      data: {
        orderId: o.order_id,
        status: 'completed',
        sellerPaid: sellerPayout,
        platformFee,
        currency: o.currency
      }
    });
  } catch (err) {
    await client.query('ROLLBACK');
    if (err instanceof AppError) throw err;
    throw new AppError('CONFIRM_FAILED', 'Order confirmation failed');
  } finally {
    client.release();
  }
}));

// ============================================================
// POST /order/:orderId/dispute — Open a dispute
// ============================================================
router.post('/order/:orderId/dispute',
  validate({
    body: {
      reason: t.string({ required: true, max: 1000 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const result = await db.query(
      `UPDATE marketplace_orders SET status = 'disputed', dispute_reason = $1, disputed_at = NOW()
       WHERE order_id = $2 AND buyer_id = $3 AND status = 'paid'
       RETURNING order_id, status`,
      [req.body.reason, req.params.orderId, req.user.id]
    );
    if (result.rows.length === 0) throw new AppError('NOT_FOUND', 'Order not found');

    audit.record({
      category: 'MARKETPLACE',
      action: 'order_disputed',
      userId: req.user.id,
      risk: 'HIGH',
      details: { orderId: req.params.orderId, reason: req.body.reason }
    });

    res.json({ status: 'ok', data: result.rows[0] });
  })
);

// ============================================================
// GET /orders — User's orders (as buyer or seller)
// ============================================================
router.get('/orders', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const { role, status } = req.query;
  const limit = Math.min(100, parseInt(req.query.limit, 10) || 20);
  const offset = parseInt(req.query.offset, 10) || 0;

  let query;
  if (role === 'seller') {
    query = `SELECT o.*, l.title AS listing_title FROM marketplace_orders o LEFT JOIN marketplace_listings l ON l.listing_id = o.listing_id WHERE o.seller_id = $1`;
  } else {
    query = `SELECT o.*, l.title AS listing_title FROM marketplace_orders o LEFT JOIN marketplace_listings l ON l.listing_id = o.listing_id WHERE o.buyer_id = $1`;
  }
  const params = [req.user.id];
  let idx = 2;

  if (status) { query += ` AND o.status = $${idx}`; params.push(status); idx++; }
  query += ` ORDER BY o.created_at DESC LIMIT $${idx} OFFSET $${idx + 1}`;
  params.push(limit, offset);

  const result = await db.query(query, params);
  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// GET /my-listings — User's own listings
// ============================================================
router.get('/my-listings', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `SELECT listing_id, title, category, price, currency, quantity, listing_type, status, views, favorites, created_at
     FROM marketplace_listings WHERE seller_id = $1 ORDER BY created_at DESC`,
    [req.user.id]
  );

  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// GET /categories — Available categories
// ============================================================
router.get('/categories', (_req, res) => {
  res.json({ status: 'ok', data: { categories: CATEGORIES, total: CATEGORIES.length } });
});

module.exports = router;
