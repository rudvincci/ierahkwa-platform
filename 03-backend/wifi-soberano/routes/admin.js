'use strict';
const { Router } = require('express');

module.exports = function adminRoutes(pool, redis, logger) {
  const router = Router();

  // GET /api/v1/wifi/admin/dashboard — Real-time metrics
  router.get('/dashboard', async (req, res) => {
    try {
      const [activeUsers, revenueToday, totalSessions, fleetStatus] = await Promise.all([
        pool.query("SELECT COUNT(*) as count FROM wifi_sessions WHERE status = 'active' AND expires_at > NOW()"),
        pool.query("SELECT COALESCE(SUM(p.price_wampum), 0) as total FROM wifi_sessions s JOIN wifi_plans p ON s.plan_id = p.id WHERE s.started_at > CURRENT_DATE"),
        pool.query("SELECT COUNT(*) as total FROM wifi_sessions WHERE started_at > CURRENT_DATE"),
        pool.query("SELECT COUNT(*) FILTER (WHERE status = 'online') as online, COUNT(*) as total FROM starlink_fleet")
      ]);

      const activeCount = parseInt(activeUsers.rows[0]?.count || 0);
      const revenue = parseFloat(revenueToday.rows[0]?.total || 0);
      const sessions = parseInt(totalSessions.rows[0]?.total || 0);
      const fleet = fleetStatus.rows[0];

      res.json({
        active_users: activeCount,
        revenue_today_wampum: revenue,
        sessions_today: sessions,
        fleet_online: parseInt(fleet?.online || 0),
        fleet_total: parseInt(fleet?.total || 0),
        avg_session_hours: sessions > 0 ? (revenue / sessions * 0.17).toFixed(1) : 0,
        vigilancia_status: 'active',
        atabey_status: 'monitoring'
      });
    } catch (err) {
      logger.error({ err }, 'Dashboard error');
      res.status(500).json({ error: 'Error cargando dashboard' });
    }
  });

  // GET /api/v1/wifi/admin/sessions — Active sessions list
  router.get('/sessions', async (req, res) => {
    try {
      const result = await pool.query(`
        SELECT s.id, s.ip_address, s.mac_address, s.started_at, s.expires_at, s.data_used_mb, s.status,
               p.name as plan_name, p.bandwidth_mbps, h.name as hotspot_name
        FROM wifi_sessions s
        LEFT JOIN wifi_plans p ON s.plan_id = p.id
        LEFT JOIN hotspots h ON s.hotspot_id = h.id
        WHERE s.status = 'active' AND s.expires_at > NOW()
        ORDER BY s.started_at DESC
      `);
      res.json({ sessions: result.rows, count: result.rowCount });
    } catch (err) {
      logger.error({ err }, 'Sessions list error');
      res.status(500).json({ error: 'Error cargando sesiones' });
    }
  });

  // GET /api/v1/wifi/admin/revenue — Revenue analytics
  router.get('/revenue', async (req, res) => {
    try {
      const [daily, weekly, byPlan] = await Promise.all([
        pool.query(`
          SELECT DATE(s.started_at) as date, COUNT(*) as sessions, COALESCE(SUM(p.price_wampum), 0) as revenue
          FROM wifi_sessions s JOIN wifi_plans p ON s.plan_id = p.id
          WHERE s.started_at > NOW() - INTERVAL '30 days'
          GROUP BY DATE(s.started_at) ORDER BY date DESC
        `),
        pool.query(`
          SELECT DATE_TRUNC('week', s.started_at) as week, COALESCE(SUM(p.price_wampum), 0) as revenue
          FROM wifi_sessions s JOIN wifi_plans p ON s.plan_id = p.id
          WHERE s.started_at > NOW() - INTERVAL '12 weeks'
          GROUP BY week ORDER BY week DESC
        `),
        pool.query(`
          SELECT p.name, COUNT(*) as count, COALESCE(SUM(p.price_wampum), 0) as revenue
          FROM wifi_sessions s JOIN wifi_plans p ON s.plan_id = p.id
          WHERE s.started_at > NOW() - INTERVAL '30 days'
          GROUP BY p.name ORDER BY revenue DESC
        `)
      ]);
      res.json({ daily: daily.rows, weekly: weekly.rows, by_plan: byPlan.rows });
    } catch (err) {
      logger.error({ err }, 'Revenue error');
      res.status(500).json({ error: 'Error cargando revenue' });
    }
  });

  // POST /api/v1/wifi/admin/hotspot — Add/update hotspot
  router.post('/hotspot', async (req, res) => {
    try {
      const { name, lat, lng, address, territory, starlink_kit_id, max_users } = req.body;
      if (!name) return res.status(400).json({ error: 'name requerido' });
      const result = await pool.query(
        `INSERT INTO hotspots (name, location_lat, location_lng, address, territory, starlink_kit_id, max_users)
         VALUES ($1, $2, $3, $4, $5, $6, $7) RETURNING *`,
        [name, lat, lng, address, territory, starlink_kit_id, max_users || 50]
      );
      logger.info({ name }, 'Hotspot created');
      res.json({ hotspot: result.rows[0] });
    } catch (err) {
      logger.error({ err }, 'Hotspot create error');
      res.status(500).json({ error: 'Error creando hotspot' });
    }
  });

  // POST /api/v1/wifi/admin/vip — Manage VIP protected list
  router.post('/vip', async (req, res) => {
    try {
      const { name, role, keywords } = req.body;
      if (!name) return res.status(400).json({ error: 'name requerido' });
      const result = await pool.query(
        'INSERT INTO vip_protected (name, role, keywords) VALUES ($1, $2, $3) RETURNING *',
        [name, role || '', keywords || '']
      );
      logger.info({ name, role }, 'VIP added to protection list');
      res.json({ vip: result.rows[0] });
    } catch (err) {
      logger.error({ err }, 'VIP add error');
      res.status(500).json({ error: 'Error agregando VIP' });
    }
  });

  // POST /api/v1/wifi/admin/plan — Create/update pricing plan
  router.post('/plan', async (req, res) => {
    try {
      const { name, duration_hours, price_wampum, bandwidth_mbps, data_limit_gb } = req.body;
      if (!name || !duration_hours || !price_wampum) return res.status(400).json({ error: 'name, duration_hours, price_wampum requeridos' });
      const result = await pool.query(
        'INSERT INTO wifi_plans (name, duration_hours, price_wampum, bandwidth_mbps, data_limit_gb) VALUES ($1, $2, $3, $4, $5) RETURNING *',
        [name, duration_hours, price_wampum, bandwidth_mbps || 50, data_limit_gb]
      );
      logger.info({ name, price_wampum }, 'Plan created');
      res.json({ plan: result.rows[0] });
    } catch (err) {
      logger.error({ err }, 'Plan create error');
      res.status(500).json({ error: 'Error creando plan' });
    }
  });

  return router;
};
