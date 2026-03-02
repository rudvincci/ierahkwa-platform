'use strict';

const { Router } = require('express');

module.exports = function (store) {
  const router = Router();

  // Update provider location
  router.put('/:providerId', (req, res) => {
    try {
      const { lat, lng } = req.body;
      if (lat === undefined || lng === undefined) {
        return res.status(400).json({ error: 'lat and lng are required' });
      }
      const location = store.updateProviderLocation(req.params.providerId, Number(lat), Number(lng));
      res.json(location);
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  // Find nearby providers
  router.get('/nearby', (req, res) => {
    const { lat, lng, radius } = req.query;
    if (!lat || !lng) return res.status(400).json({ error: 'lat and lng query params are required' });
    const providers = store.getNearbyProviders(Number(lat), Number(lng), Number(radius) || 10);
    res.json({ providers, count: providers.length });
  });

  return router;
};
