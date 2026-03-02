'use strict';

const { Store } = require('../lib/store');

describe('SoberanoServicios Store', () => {
  let store;

  beforeEach(() => {
    store = new Store();
  });

  // ── Providers ────────────────────────────────────────

  describe('providers', () => {
    test('seed creates initial providers', () => {
      const result = store.listProviders();
      expect(result.total).toBeGreaterThanOrEqual(3);
    });

    test('createProvider returns provider with id', () => {
      const p = store.createProvider({ name: 'Juan', specialty: 'barber', languages: ['es', 'en'] });
      expect(p.id).toBeDefined();
      expect(p.name).toBe('Juan');
      expect(p.specialty).toBe('barber');
      expect(p.verified).toBe(false);
      expect(p.rating).toBe(0);
    });

    test('getProvider returns null for unknown', () => {
      expect(store.getProvider('fake-id')).toBeNull();
    });

    test('listProviders filters by specialty', () => {
      store.createProvider({ name: 'Plumber', specialty: 'plumber' });
      const result = store.listProviders({ specialty: 'plumber' });
      expect(result.providers.every(p => p.specialty === 'plumber')).toBe(true);
    });

    test('listProviders filters by language', () => {
      store.createProvider({ name: 'Quechua Speaker', specialty: 'tutor', languages: ['qu'] });
      const result = store.listProviders({ language: 'qu' });
      expect(result.providers.length).toBeGreaterThanOrEqual(1);
    });

    test('updateProvider modifies fields', () => {
      const p = store.createProvider({ name: 'Test', specialty: 'barber' });
      const updated = store.updateProvider(p.id, { bio: 'Expert barber' });
      expect(updated.bio).toBe('Expert barber');
    });
  });

  // ── Services ─────────────────────────────────────────

  describe('services', () => {
    let provider;

    beforeEach(() => {
      provider = store.createProvider({ name: 'María', specialty: 'massage' });
    });

    test('createService links to provider', () => {
      const s = store.createService(provider.id, { name: 'Deep Tissue', category: 'massage', price: 80, duration: 60 });
      expect(s.id).toBeDefined();
      expect(s.providerId).toBe(provider.id);
      expect(s.price).toBe(80);
      expect(s.currency).toBe('WMP');
    });

    test('rejects service for unknown provider', () => {
      expect(() => store.createService('fake', { name: 'Test' })).toThrow('Provider not found');
    });

    test('listServices filters by provider', () => {
      store.createService(provider.id, { name: 'Service A', category: 'massage' });
      const result = store.listServices({ providerId: provider.id });
      expect(result.services.every(s => s.providerId === provider.id)).toBe(true);
    });
  });

  // ── Bookings ─────────────────────────────────────────

  describe('bookings', () => {
    let provider;

    beforeEach(() => {
      provider = store.createProvider({ name: 'Carlos', specialty: 'plumber' });
    });

    test('createBooking calculates fees correctly', () => {
      const b = store.createBooking('customer-1', {
        providerId: provider.id,
        date: '2026-03-15',
        time: '10:00',
        price: 200
      });
      expect(b.price).toBe(200);
      expect(b.fee).toBe(16);                     // 8% of 200
      expect(b.providerReceives).toBe(184);        // 200 - 16
      expect(b.status).toBe('pending');
      expect(b.paymentStatus).toBe('escrowed');
    });

    test('booking lifecycle: pending → confirmed → in-progress → completed', () => {
      const b = store.createBooking('cust-1', { providerId: provider.id, date: '2026-04-01', time: '14:00' });

      store.updateBookingStatus(b.id, 'confirmed');
      expect(store.getBooking(b.id).status).toBe('confirmed');

      store.updateBookingStatus(b.id, 'in-progress');
      expect(store.getBooking(b.id).status).toBe('in-progress');

      store.updateBookingStatus(b.id, 'completed');
      const completed = store.getBooking(b.id);
      expect(completed.status).toBe('completed');
      expect(completed.paymentStatus).toBe('released');
    });

    test('cancel refunds payment', () => {
      const b = store.createBooking('cust-1', { providerId: provider.id, date: '2026-04-01', time: '14:00' });
      store.updateBookingStatus(b.id, 'cancelled');
      expect(store.getBooking(b.id).paymentStatus).toBe('refunded');
    });

    test('listBookings filters by customerId', () => {
      store.createBooking('cust-A', { providerId: provider.id, date: '2026-04-01', time: '10:00' });
      store.createBooking('cust-B', { providerId: provider.id, date: '2026-04-01', time: '11:00' });
      const result = store.listBookings({ customerId: 'cust-A' });
      expect(result.bookings.every(b => b.customerId === 'cust-A')).toBe(true);
    });

    test('rejects booking for unknown provider', () => {
      expect(() => store.createBooking('cust-1', { providerId: 'fake', date: '2026-04-01', time: '10:00' }))
        .toThrow('Provider not found');
    });
  });

  // ── Reviews ──────────────────────────────────────────

  describe('reviews', () => {
    let provider, booking;

    beforeEach(() => {
      provider = store.createProvider({ name: 'Ana', specialty: 'photographer' });
      booking = store.createBooking('cust-1', { providerId: provider.id, date: '2026-04-01', time: '10:00' });
      store.updateBookingStatus(booking.id, 'completed');
    });

    test('createReview updates provider rating', () => {
      store.createReview(booking.id, 'cust-1', { rating: 5, comment: 'Excellent!' });
      const p = store.getProvider(provider.id);
      expect(p.rating).toBe(5);
      expect(p.reviews).toBe(1);
    });

    test('rejects review for non-completed booking', () => {
      const pending = store.createBooking('cust-2', { providerId: provider.id, date: '2026-04-02', time: '10:00' });
      expect(() => store.createReview(pending.id, 'cust-2', { rating: 4 })).toThrow('completed');
    });

    test('rating is clamped 1-5', () => {
      const review = store.createReview(booking.id, 'cust-1', { rating: 99 });
      expect(review.rating).toBe(5);
    });

    test('listReviews filters by provider', () => {
      store.createReview(booking.id, 'cust-1', { rating: 4 });
      const result = store.listReviews(provider.id);
      expect(result.reviews.every(r => r.providerId === provider.id)).toBe(true);
    });
  });

  // ── Availability ─────────────────────────────────────

  describe('availability', () => {
    let provider;

    beforeEach(() => {
      provider = store.createProvider({ name: 'Luis', specialty: 'tutor' });
    });

    test('setAvailability and getAvailability', () => {
      const slots = [
        { date: '2026-04-01', start: '09:00', end: '12:00' },
        { date: '2026-04-01', start: '14:00', end: '18:00' },
        { date: '2026-04-02', start: '09:00', end: '17:00' }
      ];
      store.setAvailability(provider.id, slots);
      expect(store.getAvailability(provider.id).length).toBe(3);
      expect(store.getAvailability(provider.id, '2026-04-01').length).toBe(2);
    });

    test('rejects availability for unknown provider', () => {
      expect(() => store.setAvailability('fake', [])).toThrow('Provider not found');
    });
  });

  // ── Locations / Geosearch ────────────────────────────

  describe('locations', () => {
    let p1, p2;

    beforeEach(() => {
      p1 = store.createProvider({ name: 'Near', specialty: 'plumber' });
      p2 = store.createProvider({ name: 'Far', specialty: 'plumber' });
      store.updateProviderLocation(p1.id, 19.4326, -99.1332); // CDMX
      store.updateProviderLocation(p2.id, 40.7128, -74.0060); // NYC
    });

    test('getNearbyProviders finds providers within radius', () => {
      const nearby = store.getNearbyProviders(19.4000, -99.1000, 50); // 50km from CDMX
      expect(nearby.length).toBe(1);
      expect(nearby[0].id).toBe(p1.id);
      expect(nearby[0].distance).toBeLessThan(50);
    });

    test('haversine distance is reasonable', () => {
      // CDMX to NYC ~3364 km
      const dist = store._haversine(19.4326, -99.1332, 40.7128, -74.0060);
      expect(dist).toBeGreaterThan(3000);
      expect(dist).toBeLessThan(4000);
    });
  });

  // ── Stats ────────────────────────────────────────────

  describe('stats', () => {
    test('returns aggregate data', () => {
      const stats = store.getStats();
      expect(stats.providers).toBeGreaterThanOrEqual(3);
      expect(typeof stats.revenue).toBe('number');
    });
  });

  // ── Reset ────────────────────────────────────────────

  describe('reset', () => {
    test('clears all data', () => {
      store.createProvider({ name: 'Temp', specialty: 'barber' });
      store.reset();
      expect(store.getStats().providers).toBe(0);
    });
  });
});
