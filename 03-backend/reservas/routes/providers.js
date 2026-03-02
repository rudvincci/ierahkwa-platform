'use strict';

const { Router } = require('express');

module.exports = function (store) {
  const router = Router();

  router.get('/', (req, res) => {
    const { specialty, language, available, verified, page, limit } = req.query;
    const result = store.listProviders({
      specialty, language,
      available: available !== undefined ? available === 'true' : undefined,
      verified: verified === 'true',
      page: parseInt(page) || 1,
      limit: parseInt(limit) || 20
    });
    res.json(result);
  });

  router.post('/', (req, res) => {
    try {
      const provider = store.createProvider(req.body);
      res.status(201).json(provider);
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  router.get('/:id', (req, res) => {
    const provider = store.getProvider(req.params.id);
    if (!provider) return res.status(404).json({ error: 'Provider not found' });
    res.json(provider);
  });

  router.put('/:id', (req, res) => {
    const provider = store.updateProvider(req.params.id, req.body);
    if (!provider) return res.status(404).json({ error: 'Provider not found' });
    res.json(provider);
  });

  router.get('/:id/reviews', (req, res) => {
    const result = store.listReviews(req.params.id);
    res.json(result);
  });

  router.get('/:id/services', (req, res) => {
    const result = store.listServices({ providerId: req.params.id });
    res.json(result);
  });

  return router;
};
