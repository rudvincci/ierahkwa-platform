'use strict';
const { Router } = require('express');
const jwt = require('jsonwebtoken');
const { v4: uuid } = require('uuid');

module.exports = function authRoutes(pool, redis, logger) {
  const router = Router();
  const JWT_SECRET = process.env.JWT_SECRET || 'wifi-soberano-secret';

  // GET /api/v1/wifi/portal — Captive portal landing data
  router.get('/portal', async (req, res) => {
    try {
      const ip = req.headers['x-forwarded-for']?.split(',')[0] || req.socket.remoteAddress;
      const plans = await pool.query('SELECT id, name, duration_hours, price_wampum, bandwidth_mbps FROM wifi_plans WHERE is_active = true ORDER BY duration_hours');
      const activeUsers = await redis.get('wifi:active_users') || '0';
      res.json({
        portal: 'Internet Soberano',
        ip,
        plans: plans.rows,
        active_users: parseInt(activeUsers),
        signal: { speed: '150 Mbps', latency: '<20ms', backbone: 'Starlink' }
      });
    } catch (err) {
      logger.error({ err }, 'Portal data error');
      res.status(500).json({ error: 'Error cargando portal' });
    }
  });

  // POST /api/v1/wifi/connect — Create WiFi session after payment
  router.post('/connect', async (req, res) => {
    try {
      const { plan_id, payment_id, mac_address } = req.body;
      const ip = req.headers['x-forwarded-for']?.split(',')[0] || req.socket.remoteAddress;

      if (!plan_id) return res.status(400).json({ error: 'plan_id requerido' });

      const plan = await pool.query('SELECT * FROM wifi_plans WHERE id = $1 AND is_active = true', [plan_id]);
      if (!plan.rows[0]) return res.status(404).json({ error: 'Plan no encontrado' });

      const p = plan.rows[0];
      const sessionId = uuid();
      const expiresAt = new Date(Date.now() + p.duration_hours * 3600000);

      await pool.query(
        'INSERT INTO wifi_sessions (id, mac_address, ip_address, plan_id, hotspot_id, expires_at, payment_id, status) VALUES ($1, $2, $3, $4, $5, $6, $7, $8)',
        [sessionId, mac_address || 'unknown', ip, plan_id, req.body.hotspot_id || null, expiresAt, payment_id || null, 'active']
      );

      // Store session in Redis for fast lookup
      await redis.set(`wifi:session:${ip}`, JSON.stringify({
        id: sessionId, plan: p.name, expires: expiresAt.toISOString(), bandwidth: p.bandwidth_mbps
      }), { EX: p.duration_hours * 3600 });

      // Increment active user count
      await redis.incr('wifi:active_users');

      const token = jwt.sign({ sessionId, ip, plan: p.name }, JWT_SECRET, { expiresIn: `${p.duration_hours}h` });

      logger.info({ sessionId, ip, plan: p.name }, 'New WiFi session');
      res.json({
        session_id: sessionId,
        token,
        plan: p.name,
        expires_at: expiresAt.toISOString(),
        bandwidth_mbps: p.bandwidth_mbps,
        message: 'Conectado exitosamente'
      });
    } catch (err) {
      logger.error({ err }, 'Connect error');
      res.status(500).json({ error: 'Error al conectar' });
    }
  });

  return router;
};
