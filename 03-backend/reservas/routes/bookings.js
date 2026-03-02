'use strict';

const { Router } = require('express');

module.exports = function (store) {
  const router = Router();

  router.get('/', (req, res) => {
    const { customerId, providerId, status } = req.query;
    const result = store.listBookings({ customerId, providerId, status });
    res.json(result);
  });

  router.post('/', (req, res) => {
    try {
      const customerId = req.headers['x-user-id'] || req.body.customerId;
      if (!customerId) return res.status(400).json({ error: 'customerId is required (header X-User-Id or body)' });
      if (!req.body.providerId) return res.status(400).json({ error: 'providerId is required' });
      if (!req.body.date || !req.body.time) return res.status(400).json({ error: 'date and time are required' });

      const booking = store.createBooking(customerId, req.body);
      res.status(201).json({
        booking,
        taxPaid: 0,
        escrow: 'Funds held until service completed',
        providerPercent: '92%'
      });
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  });

  router.get('/:id', (req, res) => {
    const booking = store.getBooking(req.params.id);
    if (!booking) return res.status(404).json({ error: 'Booking not found' });
    res.json(booking);
  });

  router.post('/:id/confirm', (req, res) => {
    const booking = store.updateBookingStatus(req.params.id, 'confirmed', { confirmedAt: new Date().toISOString() });
    if (!booking) return res.status(404).json({ error: 'Booking not found' });
    res.json(booking);
  });

  router.post('/:id/start', (req, res) => {
    const booking = store.updateBookingStatus(req.params.id, 'in-progress', { startedAt: new Date().toISOString() });
    if (!booking) return res.status(404).json({ error: 'Booking not found' });
    res.json(booking);
  });

  router.post('/:id/complete', (req, res) => {
    const booking = store.updateBookingStatus(req.params.id, 'completed');
    if (!booking) return res.status(404).json({ error: 'Booking not found' });
    res.json({ booking, providerPaid: booking.providerReceives, taxPaid: 0, message: 'Payment released to provider' });
  });

  router.post('/:id/cancel', (req, res) => {
    const booking = store.getBooking(req.params.id);
    if (!booking) return res.status(404).json({ error: 'Booking not found' });
    if (booking.status !== 'pending' && booking.status !== 'confirmed') {
      return res.status(400).json({ error: 'Cannot cancel after service started' });
    }
    const updated = store.updateBookingStatus(req.params.id, 'cancelled');
    res.json({ booking: updated, refunded: true });
  });

  router.put('/:id/reschedule', (req, res) => {
    const booking = store.getBooking(req.params.id);
    if (!booking) return res.status(404).json({ error: 'Booking not found' });
    booking.date = req.body.date || booking.date;
    booking.time = req.body.time || booking.time;
    booking.status = 'pending';
    booking.updatedAt = new Date().toISOString();
    res.json({ booking, message: 'Rescheduled — waiting provider confirmation' });
  });

  return router;
};
