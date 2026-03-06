'use strict';
const { Router } = require('express');

module.exports = function fleetRoutes(pool, logger) {
  const router = Router();

  // GET /api/v1/wifi/admin/fleet — Starlink fleet status
  router.get('/fleet', async (req, res) => {
    try {
      const fleet = await pool.query('SELECT * FROM starlink_fleet ORDER BY activation_date DESC');
      const summary = await pool.query(`
        SELECT
          COUNT(*) FILTER (WHERE status = 'online') as online,
          COUNT(*) FILTER (WHERE status = 'offline') as offline,
          COUNT(*) FILTER (WHERE status = 'rma') as rma,
          COUNT(*) as total
        FROM starlink_fleet
      `);
      res.json({ fleet: fleet.rows, summary: summary.rows[0] });
    } catch (err) {
      logger.error({ err }, 'Fleet error');
      res.status(500).json({ error: 'Error cargando flota' });
    }
  });

  // POST /api/v1/wifi/admin/fleet — Add/update kit
  router.post('/fleet', async (req, res) => {
    try {
      const { utid, model, account_name, activation_date, location, status } = req.body;
      if (!utid || !model) return res.status(400).json({ error: 'utid y model requeridos' });
      const transferDate = activation_date ? new Date(new Date(activation_date).getTime() + 90 * 86400000) : null;
      const result = await pool.query(
        `INSERT INTO starlink_fleet (utid, model, account_name, activation_date, transfer_eligible_date, location, status)
         VALUES ($1, $2, $3, $4, $5, $6, $7)
         ON CONFLICT (utid) DO UPDATE SET model = $2, account_name = $3, location = $6, status = $7
         RETURNING *`,
        [utid, model, account_name, activation_date, transferDate, location, status || 'online']
      );
      logger.info({ utid, model }, 'Fleet updated');
      res.json({ device: result.rows[0] });
    } catch (err) {
      logger.error({ err }, 'Fleet update error');
      res.status(500).json({ error: 'Error actualizando flota' });
    }
  });

  return router;
};
