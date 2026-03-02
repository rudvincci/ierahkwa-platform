'use strict';

const crypto = require('crypto');

// ============================================================
// In-Memory Data Store for SoberanoServicios
// Provides: providers, services, bookings, reviews, availability
// ============================================================

function uuid() {
  return crypto.randomUUID();
}

class Store {
  constructor() {
    this.providers = new Map();
    this.services = new Map();
    this.bookings = new Map();
    this.reviews = new Map();
    this.availability = new Map(); // providerId → [slots]
    this.locations = new Map();

    this._seed();
  }

  _seed() {
    // Seed a few example providers
    const providers = [
      { name: 'María García', specialty: 'barber', rating: 4.8, reviews: 120, languages: ['es', 'en'], verified: true },
      { name: 'Carlos Huanca', specialty: 'plumber', rating: 4.9, reviews: 85, languages: ['es', 'qu'], verified: true },
      { name: 'Ana Tukuy', specialty: 'traditional-healer', rating: 5.0, reviews: 200, languages: ['es', 'ay', 'qu'], verified: true },
    ];

    for (const p of providers) {
      const id = uuid();
      this.providers.set(id, {
        id,
        ...p,
        available: true,
        completedBookings: p.reviews,
        joinedAt: '2024-06-01T00:00:00.000Z',
        location: null
      });
    }
  }

  // ── Providers ────────────────────────────────────────

  createProvider(data) {
    const id = uuid();
    const provider = {
      id,
      name: data.name,
      specialty: data.specialty,
      rating: 0,
      reviews: 0,
      languages: data.languages || ['es'],
      verified: false,
      available: true,
      completedBookings: 0,
      joinedAt: new Date().toISOString(),
      location: data.location || null,
      bio: data.bio || null,
      phone: data.phone || null
    };
    this.providers.set(id, provider);
    return provider;
  }

  getProvider(id) { return this.providers.get(id) || null; }

  listProviders(filters = {}) {
    let list = [...this.providers.values()];
    if (filters.specialty) list = list.filter(p => p.specialty === filters.specialty);
    if (filters.available !== undefined) list = list.filter(p => p.available === filters.available);
    if (filters.verified) list = list.filter(p => p.verified);
    if (filters.language) list = list.filter(p => p.languages.includes(filters.language));
    list.sort((a, b) => b.rating - a.rating);
    const page = filters.page || 1;
    const limit = Math.min(filters.limit || 20, 100);
    const start = (page - 1) * limit;
    return { providers: list.slice(start, start + limit), total: list.length, page };
  }

  updateProvider(id, data) {
    const p = this.providers.get(id);
    if (!p) return null;
    Object.assign(p, data);
    return p;
  }

  // ── Services ─────────────────────────────────────────

  createService(providerId, data) {
    if (!this.providers.has(providerId)) throw new Error('Provider not found');
    const id = uuid();
    const service = {
      id,
      providerId,
      name: data.name,
      category: data.category,
      description: data.description || '',
      price: data.price || 0,
      duration: data.duration || 60, // minutes
      currency: 'WMP',
      atHome: data.atHome !== undefined ? data.atHome : true,
      inShop: data.inShop !== undefined ? data.inShop : false,
      active: true,
      createdAt: new Date().toISOString()
    };
    this.services.set(id, service);
    return service;
  }

  getService(id) { return this.services.get(id) || null; }

  listServices(filters = {}) {
    let list = [...this.services.values()];
    if (filters.providerId) list = list.filter(s => s.providerId === filters.providerId);
    if (filters.category) list = list.filter(s => s.category === filters.category);
    if (filters.active !== undefined) list = list.filter(s => s.active === filters.active);
    return { services: list, total: list.length };
  }

  // ── Bookings ─────────────────────────────────────────

  createBooking(customerId, data) {
    if (!this.providers.has(data.providerId)) throw new Error('Provider not found');

    const price = data.price || 100;
    const fee = price * 0.08; // 8% platform fee
    const providerReceives = price - fee;

    const id = uuid();
    const booking = {
      id,
      customerId,
      providerId: data.providerId,
      serviceId: data.serviceId || null,
      date: data.date,
      time: data.time,
      duration: data.duration || 60,
      location: data.location || null,
      type: data.type || 'at-home',
      notes: data.notes || null,
      language: data.language || 'es',
      price,
      fee: +fee.toFixed(2),
      providerReceives: +providerReceives.toFixed(2),
      providerPercent: '92%',
      status: 'pending',
      paymentStatus: 'escrowed',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };
    this.bookings.set(id, booking);
    return booking;
  }

  getBooking(id) { return this.bookings.get(id) || null; }

  updateBookingStatus(id, status, extra = {}) {
    const b = this.bookings.get(id);
    if (!b) return null;
    b.status = status;
    b.updatedAt = new Date().toISOString();
    Object.assign(b, extra);
    if (status === 'completed') {
      b.paymentStatus = 'released';
      b.completedAt = new Date().toISOString();
      const provider = this.providers.get(b.providerId);
      if (provider) provider.completedBookings++;
    }
    if (status === 'cancelled') {
      b.paymentStatus = 'refunded';
    }
    return b;
  }

  listBookings(filters = {}) {
    let list = [...this.bookings.values()];
    if (filters.customerId) list = list.filter(b => b.customerId === filters.customerId);
    if (filters.providerId) list = list.filter(b => b.providerId === filters.providerId);
    if (filters.status) list = list.filter(b => b.status === filters.status);
    list.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
    return { bookings: list, total: list.length };
  }

  // ── Reviews ──────────────────────────────────────────

  createReview(bookingId, reviewerId, data) {
    const booking = this.bookings.get(bookingId);
    if (!booking) throw new Error('Booking not found');
    if (booking.status !== 'completed') throw new Error('Can only review completed bookings');

    const id = uuid();
    const review = {
      id,
      bookingId,
      reviewerId,
      providerId: booking.providerId,
      rating: Math.min(5, Math.max(1, data.rating)),
      comment: data.comment || '',
      createdAt: new Date().toISOString()
    };
    this.reviews.set(id, review);

    // Update provider rating
    const providerReviews = [...this.reviews.values()].filter(r => r.providerId === booking.providerId);
    const provider = this.providers.get(booking.providerId);
    if (provider) {
      provider.reviews = providerReviews.length;
      provider.rating = +(providerReviews.reduce((s, r) => s + r.rating, 0) / providerReviews.length).toFixed(1);
    }

    return review;
  }

  listReviews(providerId) {
    let list = [...this.reviews.values()];
    if (providerId) list = list.filter(r => r.providerId === providerId);
    list.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
    return { reviews: list, total: list.length };
  }

  // ── Availability ─────────────────────────────────────

  setAvailability(providerId, slots) {
    if (!this.providers.has(providerId)) throw new Error('Provider not found');
    this.availability.set(providerId, slots);
    return slots;
  }

  getAvailability(providerId, date) {
    const slots = this.availability.get(providerId) || [];
    if (date) return slots.filter(s => s.date === date);
    return slots;
  }

  // ── Locations ────────────────────────────────────────

  updateProviderLocation(providerId, lat, lng) {
    if (!this.providers.has(providerId)) throw new Error('Provider not found');
    this.locations.set(providerId, { lat, lng, updatedAt: new Date().toISOString() });
    return this.locations.get(providerId);
  }

  getNearbyProviders(lat, lng, radiusKm = 10) {
    const nearby = [];
    for (const [providerId, loc] of this.locations) {
      const dist = this._haversine(lat, lng, loc.lat, loc.lng);
      if (dist <= radiusKm) {
        const provider = this.providers.get(providerId);
        if (provider) nearby.push({ ...provider, distance: +dist.toFixed(2), location: loc });
      }
    }
    return nearby.sort((a, b) => a.distance - b.distance);
  }

  _haversine(lat1, lng1, lat2, lng2) {
    const R = 6371;
    const dLat = (lat2 - lat1) * Math.PI / 180;
    const dLng = (lng2 - lng1) * Math.PI / 180;
    const a = Math.sin(dLat / 2) ** 2 +
      Math.cos(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) *
      Math.sin(dLng / 2) ** 2;
    return R * 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
  }

  // ── Stats ────────────────────────────────────────────

  getStats() {
    return {
      providers: this.providers.size,
      services: this.services.size,
      bookings: this.bookings.size,
      reviews: this.reviews.size,
      completedBookings: [...this.bookings.values()].filter(b => b.status === 'completed').length,
      revenue: [...this.bookings.values()]
        .filter(b => b.status === 'completed')
        .reduce((sum, b) => sum + b.fee, 0)
    };
  }

  reset() {
    this.providers.clear();
    this.services.clear();
    this.bookings.clear();
    this.reviews.clear();
    this.availability.clear();
    this.locations.clear();
  }
}

module.exports = { Store };
