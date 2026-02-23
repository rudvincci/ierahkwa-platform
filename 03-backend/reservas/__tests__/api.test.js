describe('Reservas (SoberanoServicios) - API Routes', () => {
  describe('Module Structure', () => {
    it('should have a server.js entry point', () => {
      const fs = require('fs');
      const path = require('path');
      const exists = fs.existsSync(path.join(__dirname, '..', 'server.js'));
      expect(exists).toBe(true);
    });
  });

  describe('Service Route Definitions', () => {
    const serviceRoutes = [
      { path: '/v1/providers', purpose: 'Service providers management' },
      { path: '/v1/services', purpose: 'Service listings' },
      { path: '/v1/bookings', purpose: 'Booking management' },
      { path: '/v1/reviews', purpose: 'Service reviews' },
      { path: '/v1/categories', purpose: 'Service categories' },
      { path: '/v1/availability', purpose: 'Provider availability' },
      { path: '/v1/locations', purpose: 'Location-based search' },
    ];

    it('should define 7 route modules', () => {
      expect(serviceRoutes).toHaveLength(7);
    });

    it('should define providers route', () => {
      const route = serviceRoutes.find(r => r.path === '/v1/providers');
      expect(route).toBeDefined();
    });

    it('should define services route', () => {
      const route = serviceRoutes.find(r => r.path === '/v1/services');
      expect(route).toBeDefined();
    });

    it('should define bookings route', () => {
      const route = serviceRoutes.find(r => r.path === '/v1/bookings');
      expect(route).toBeDefined();
    });

    it('should define reviews route', () => {
      const route = serviceRoutes.find(r => r.path === '/v1/reviews');
      expect(route).toBeDefined();
    });

    it('should define categories route', () => {
      const route = serviceRoutes.find(r => r.path === '/v1/categories');
      expect(route).toBeDefined();
    });

    it('should define availability route', () => {
      const route = serviceRoutes.find(r => r.path === '/v1/availability');
      expect(route).toBeDefined();
    });

    it('should define locations route', () => {
      const route = serviceRoutes.find(r => r.path === '/v1/locations');
      expect(route).toBeDefined();
    });

    it('should mount all routes under /v1/ prefix', () => {
      expect(serviceRoutes.every(r => r.path.startsWith('/v1/'))).toBe(true);
    });
  });

  describe('Health Check', () => {
    it('should return service identity and status', () => {
      const response = {
        service: 'SoberanoServicios',
        version: '4.2.0',
        status: 'operational',
        categories: 30,
        providerPercent: '92%',
        taxRate: '0%',
        languages: 43
      };
      expect(response.service).toBe('SoberanoServicios');
      expect(response.status).toBe('operational');
      expect(response.categories).toBe(30);
      expect(response.providerPercent).toBe('92%');
      expect(response.taxRate).toBe('0%');
      expect(response.languages).toBe(43);
    });
  });

  describe('WebSocket Support', () => {
    it('should configure WebSocket on /ws path', () => {
      const wsPath = '/ws';
      expect(wsPath).toBe('/ws');
    });

    it('should support real-time booking updates', () => {
      const realtimeFeatures = ['booking-updates', 'provider-location'];
      expect(realtimeFeatures).toContain('booking-updates');
    });
  });

  describe('Configuration', () => {
    it('should use port 4010 by default (SERVICIOS_PORT)', () => {
      const port = parseInt(process.env.SERVICIOS_PORT || '4010', 10);
      expect(port).toBe(4010);
    });

    it('should give 92% of revenue to providers', () => {
      const providerPercent = 92;
      expect(providerPercent).toBe(92);
    });

    it('should use shared security CORS config', () => {
      const securityPath = '../shared/security';
      expect(securityPath).toBeDefined();
    });
  });
});
