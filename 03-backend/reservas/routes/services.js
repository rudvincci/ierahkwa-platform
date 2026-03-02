'use strict';

const { Router } = require('express');

module.exports = function (store) {
  const router = Router();

  router.get('/', (req, res) => {
    const { providerId, category, active } = req.query;
    const result = store.listServices({
      providerId, category,
      active: active !== undefined ? active === 'true' : undefined
    });
    res.json(result);
  });

  router.post('/', (req, res) => {
    try {
      const { providerId, ...data } = req.body;
      if (!providerId) return res.status(400).json({ error: 'providerId is required' });
      const service = store.createService(providerId, data);
      res.status(201).json(service);
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  router.get('/:id', (req, res) => {
    const service = store.getService(req.params.id);
    if (!service) return res.status(404).json({ error: 'Service not found' });
    res.json(service);
  });

  return router;
};
