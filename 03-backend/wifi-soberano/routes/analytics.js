'use strict';
const { Router } = require('express');

module.exports = function analyticsRoutes(pool, logger) {
  const router = Router();

  // GET /api/v1/wifi/admin/analytics — User analytics
  router.get('/analytics', async (req, res) => {
    try {
      const { period } = req.query; // today, week, month
      let interval = '1 day';
      if (period === 'week') interval = '7 days';
      if (period === 'month') interval = '30 days';

      const [sessions, devices, topHotspots, dataUsage] = await Promise.all([
        pool.query(`SELECT COUNT(*) as total, SUM(data_used_mb) as total_data FROM wifi_sessions WHERE started_at > NOW() - $1::interval`, [interval]),
        pool.query(`SELECT device_type, COUNT(*) as count FROM wifi_analytics WHERE timestamp > NOW() - $1::interval GROUP BY device_type ORDER BY count DESC LIMIT 10`, [interval]),
        pool.query(`SELECT h.name, COUNT(s.id) as sessions, SUM(s.data_used_mb) as data_mb FROM hotspots h LEFT JOIN wifi_sessions s ON s.hotspot_id = h.id WHERE s.started_at > NOW() - $1::interval GROUP BY h.id, h.name ORDER BY sessions DESC`, [interval]),
        pool.query(`SELECT DATE_TRUNC('hour', timestamp) as hour, SUM(bytes_down + bytes_up) as total_bytes FROM wifi_analytics WHERE timestamp > NOW() - $1::interval GROUP BY hour ORDER BY hour`, [interval])
      ]);

      res.json({
        period,
        total_sessions: parseInt(sessions.rows[0]?.total || 0),
        total_data_mb: parseFloat(sessions.rows[0]?.total_data || 0),
        devices: devices.rows,
        top_hotspots: topHotspots.rows,
        data_usage_hourly: dataUsage.rows
      });
    } catch (err) {
      logger.error({ err }, 'Analytics error');
      res.status(500).json({ error: 'Error cargando analytics' });
    }
  });

  // GET /api/v1/wifi/admin/vigilancia — Surveillance log
  router.get('/vigilancia', async (req, res) => {
    try {
      const alerts = await pool.query(
        'SELECT * FROM vigilancia_alerts ORDER BY created_at DESC LIMIT 100'
      );
      res.json({ alerts: alerts.rows });
    } catch (err) {
      logger.error({ err }, 'Vigilancia error');
      res.status(500).json({ error: 'Error cargando vigilancia' });
    }
  });

  return router;
};
