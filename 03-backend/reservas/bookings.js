const router = require('express').Router();
const auth = require('../../../bdet-bank/middleware/auth');
const { v4: uuid } = require('uuid');
const bookings = new Map();
const fiscal = require('../../../bdet-bank/utils/fiscal');

// Create booking
router.post('/', auth, (req, res) => {
  const { providerId, serviceId, date, time, duration, location, type = 'at-home', notes, language = 'es' } = req.body;
  const price = req.body.price || 100;
  const fee = price * 0.08; // 8% platform fee
  const providerReceives = price - fee;
  const fiscalAlloc = fiscal.allocate(fee);

  const booking = {
    id: uuid(), customerId: req.userId, providerId, serviceId,
    date, time, duration, location, type, notes, language,
    price, fee: +fee.toFixed(2), providerReceives: +providerReceives.toFixed(2),
    providerPercent: '92%',
    fiscal: fiscalAlloc,
    status: 'pending', // pending → confirmed → in-progress → completed → reviewed
    paymentStatus: 'escrowed', // funds held until service completed
    chatEnabled: true, // customer-provider chat
    createdAt: new Date()
  };
  bookings.set(booking.id, booking);
  res.status(201).json({ booking, taxPaid: 0, escrow: 'Funds held until service completed' });
});

// Provider confirms booking
router.post('/:id/confirm', auth, (req, res) => {
  const b = bookings.get(req.params.id);
  if (!b) return res.status(404).json({ error: 'Not found' });
  b.status = 'confirmed';
  b.confirmedAt = new Date();
  res.json(b);
});

// Start service
router.post('/:id/start', auth, (req, res) => {
  const b = bookings.get(req.params.id);
  if (!b) return res.status(404).json({ error: 'Not found' });
  b.status = 'in-progress';
  b.startedAt = new Date();
  res.json(b);
});

// Complete service — release payment to provider
router.post('/:id/complete', auth, (req, res) => {
  const b = bookings.get(req.params.id);
  if (!b) return res.status(404).json({ error: 'Not found' });
  b.status = 'completed';
  b.completedAt = new Date();
  b.paymentStatus = 'released';
  res.json({ booking: b, providerPaid: b.providerReceives, taxPaid: 0, message: 'Payment released to provider' });
});

// Cancel booking
router.post('/:id/cancel', auth, (req, res) => {
  const b = bookings.get(req.params.id);
  if (!b) return res.status(404).json({ error: 'Not found' });
  const canCancel = b.status === 'pending' || b.status === 'confirmed';
  if (!canCancel) return res.status(400).json({ error: 'Cannot cancel after service started' });
  b.status = 'cancelled';
  b.paymentStatus = 'refunded';
  res.json({ booking: b, refunded: true });
});

// Get my bookings (as customer or provider)
router.get('/mine', auth, (req, res) => {
  const { role = 'customer', status } = req.query;
  let list = [...bookings.values()];
  if (role === 'customer') list = list.filter(b => b.customerId === req.userId);
  else list = list.filter(b => b.providerId === req.userId);
  if (status) list = list.filter(b => b.status === status);
  list.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
  res.json({ bookings: list });
});

// Get booking details
router.get('/:id', auth, (req, res) => {
  const b = bookings.get(req.params.id);
  if (!b) return res.status(404).json({ error: 'Not found' });
  res.json(b);
});

// Reschedule
router.put('/:id/reschedule', auth, (req, res) => {
  const b = bookings.get(req.params.id);
  if (!b) return res.status(404).json({ error: 'Not found' });
  b.date = req.body.date || b.date;
  b.time = req.body.time || b.time;
  b.status = 'pending';
  res.json({ booking: b, message: 'Rescheduled — waiting provider confirmation' });
});

module.exports = router;
