'use strict';

const { Router } = require('express');

module.exports = function (store) {
  const router = Router();

  router.get('/', (req, res) => {
    const { providerId } = req.query;
    const result = store.listReviews(providerId);
    res.json(result);
  });

  router.post('/', (req, res) => {
    try {
      const reviewerId = req.headers['x-user-id'] || req.body.reviewerId;
      if (!reviewerId) return res.status(400).json({ error: 'reviewerId is required' });
      if (!req.body.bookingId) return res.status(400).json({ error: 'bookingId is required' });
      if (!req.body.rating) return res.status(400).json({ error: 'rating is required (1-5)' });

      const review = store.createReview(req.body.bookingId, reviewerId, req.body);
      res.status(201).json(review);
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  return router;
};
