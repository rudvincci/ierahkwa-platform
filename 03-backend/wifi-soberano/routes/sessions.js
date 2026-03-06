'use strict';
const { Router } = require('express');

module.exports = function sessionRoutes(pool, redis, logger) {
  const router = Router();

  // GET /api/v1/wifi/session/status — Check current session
  router.get('/session/status', async (req, res) => {
    try {
      const ip = req.headers['x-forwarded-for']?.split(',')[0] || req.socket.remoteAddress;
      const cached = await redis.get(`wifi:session:${ip}`);
      if (cached) {
        const session = JSON.parse(cached);
        const remaining = new Date(session.expires) - Date.now();
        if (remaining > 0) {
          return res.json({ active: true, ...session, remaining_ms: remaining, remaining_formatted: formatTime(remaining) });
        }
      }
      // Check DB
      const result = await pool.query(
        "SELECT s.*, p.name as plan_name, p.bandwidth_mbps FROM wifi_sessions s JOIN wifi_plans p ON s.plan_id = p.id WHERE s.ip_address = $1 AND s.status = 'active' AND s.expires_at > NOW() ORDER BY s.expires_at DESC LIMIT 1",
        [ip]
      );
      if (result.rows[0]) {
        const s = result.rows[0];
        return res.json({ active: true, id: s.id, plan: s.plan_name, expires: s.expires_at, bandwidth: s.bandwidth_mbps, data_used_mb: s.data_used_mb });
      }
      res.json({ active: false, message: 'No hay sesión activa. Seleccione un plan.' });
    } catch (err) {
      logger.error({ err }, 'Session status error');
      res.status(500).json({ error: 'Error verificando sesión' });
    }
  });

  // POST /api/v1/wifi/session/extend — Extend current session
  router.post('/session/extend', async (req, res) => {
    try {
      const { session_id, plan_id, payment_id } = req.body;
      if (!session_id || !plan_id) return res.status(400).json({ error: 'session_id y plan_id requeridos' });
      const plan = await pool.query('SELECT * FROM wifi_plans WHERE id = $1', [plan_id]);
      if (!plan.rows[0]) return res.status(404).json({ error: 'Plan no encontrado' });
      const p = plan.rows[0];
      await pool.query(
        'UPDATE wifi_sessions SET expires_at = expires_at + ($1 || \' hours\')::interval, plan_id = $2 WHERE id = $3',
        [p.duration_hours, plan_id, session_id]
      );
      logger.info({ session_id, plan: p.name }, 'Session extended');
      res.json({ success: true, extended_hours: p.duration_hours });
    } catch (err) {
      logger.error({ err }, 'Session extend error');
      res.status(500).json({ error: 'Error extendiendo sesión' });
    }
  });

  return router;
};

function formatTime(ms) {
  const h = Math.floor(ms / 3600000);
  const m = Math.floor((ms % 3600000) / 60000);
  const s = Math.floor((ms % 60000) / 1000);
  return `${String(h).padStart(2,'0')}:${String(m).padStart(2,'0')}:${String(s).padStart(2,'0')}`;
}
