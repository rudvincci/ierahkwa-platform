'use strict';

const { Router } = require('express');

module.exports = function (store) {
  const router = Router();

  router.get('/:providerId', (req, res) => {
    const { date } = req.query;
    const slots = store.getAvailability(req.params.providerId, date);
    res.json({ providerId: req.params.providerId, slots });
  });

  router.put('/:providerId', (req, res) => {
    try {
      const { slots } = req.body;
      if (!Array.isArray(slots)) return res.status(400).json({ error: 'slots must be an array' });
      const result = store.setAvailability(req.params.providerId, slots);
      res.json({ providerId: req.params.providerId, slots: result });
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  return router;
};
