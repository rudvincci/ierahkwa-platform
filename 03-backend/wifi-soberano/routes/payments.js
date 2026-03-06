'use strict';
const { Router } = require('express');
const { v4: uuid } = require('uuid');

module.exports = function paymentRoutes(pool, redis, logger) {
  const router = Router();

  // POST /api/v1/wifi/payment/create — Create WAMPUM payment
  router.post('/payment/create', async (req, res) => {
    try {
      const { plan_id, method } = req.body;
      if (!plan_id) return res.status(400).json({ error: 'plan_id requerido' });
      const plan = await pool.query('SELECT * FROM wifi_plans WHERE id = $1 AND is_active = true', [plan_id]);
      if (!plan.rows[0]) return res.status(404).json({ error: 'Plan no encontrado' });
      const p = plan.rows[0];
      const paymentId = uuid();
      // Create payment record
      await pool.query(
        'INSERT INTO wifi_payments (id, plan_id, amount_wampum, method, status, created_at) VALUES ($1, $2, $3, $4, $5, NOW())',
        [paymentId, plan_id, p.price_wampum, method || 'wampum', 'pending']
      );
      logger.info({ paymentId, plan: p.name, amount: p.price_wampum }, 'Payment created');
      res.json({
        payment_id: paymentId,
        amount: p.price_wampum,
        currency: 'WAMPUM',
        plan: p.name,
        status: 'pending',
        // In production: return BDET Bank payment URL or WAMPUM contract call
        payment_url: `/api/v1/wifi/payment/verify?id=${paymentId}`
      });
    } catch (err) {
      logger.error({ err }, 'Payment create error');
      res.status(500).json({ error: 'Error creando pago' });
    }
  });

  // POST /api/v1/wifi/payment/verify — Verify payment
  router.post('/payment/verify', async (req, res) => {
    try {
      const { payment_id, tx_hash } = req.body;
      if (!payment_id) return res.status(400).json({ error: 'payment_id requerido' });
      // In production: verify WAMPUM transaction on MameyNode blockchain
      await pool.query(
        'UPDATE wifi_payments SET status = $1, tx_hash = $2, verified_at = NOW() WHERE id = $3',
        ['verified', tx_hash || null, payment_id]
      );
      logger.info({ payment_id }, 'Payment verified');
      res.json({ verified: true, payment_id });
    } catch (err) {
      logger.error({ err }, 'Payment verify error');
      res.status(500).json({ error: 'Error verificando pago' });
    }
  });

  return router;
};
