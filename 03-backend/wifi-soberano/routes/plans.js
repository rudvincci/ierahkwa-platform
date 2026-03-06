'use strict';
const { Router } = require('express');

module.exports = function planRoutes(pool, logger) {
  const router = Router();

  // GET /api/v1/wifi/plans — List all active plans
  router.get('/plans', async (req, res) => {
    try {
      const result = await pool.query(
        'SELECT id, name, duration_hours, price_wampum, bandwidth_mbps, data_limit_gb FROM wifi_plans WHERE is_active = true ORDER BY price_wampum ASC'
      );
      res.json({ plans: result.rows });
    } catch (err) {
      logger.error({ err }, 'Plans list error');
      res.status(500).json({ error: 'Error cargando planes' });
    }
  });

  return router;
};
