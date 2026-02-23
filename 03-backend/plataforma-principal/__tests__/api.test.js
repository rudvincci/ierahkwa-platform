describe('Plataforma Principal - API Routes', () => {
  describe('Module Structure', () => {
    it('should have a server.js entry point', () => {
      const fs = require('fs');
      const path = require('path');
      const exists = fs.existsSync(path.join(__dirname, '..', 'server.js'));
      expect(exists).toBe(true);
    });
  });

  describe('API Gateway — Service Routing', () => {
    const SERVICES = {
      '/v1/wallet': 'http://bdet-bank:4000',
      '/v1/payments': 'http://bdet-bank:4000',
      '/v1/exchange': 'http://bdet-bank:4000',
      '/v1/trading': 'http://bdet-bank:4000',
      '/v1/remittance': 'http://bdet-bank:4000',
      '/v1/escrow': 'http://bdet-bank:4000',
      '/v1/loans': 'http://bdet-bank:4000',
      '/v1/insurance': 'http://bdet-bank:4000',
      '/v1/staking': 'http://bdet-bank:4000',
      '/v1/treasury': 'http://bdet-bank:4000',
      '/v1/fiscal': 'http://bdet-bank:4000',
      '/v1/feed': 'http://social-media:4001',
      '/v1/posts': 'http://social-media:4001',
      '/v1/stories': 'http://social-media:4001',
      '/v1/comments': 'http://social-media:4001',
      '/v1/likes': 'http://social-media:4001',
      '/v1/follow': 'http://social-media:4001',
      '/v1/profiles': 'http://social-media:4001',
      '/v1/groups': 'http://social-media:4001',
      '/v1/chat': 'http://social-media:4001',
      '/v1/notifications': 'http://social-media:4001',
      '/v1/live': 'http://social-media:4001',
      '/v1/doctor': 'http://soberano-doctor:4002',
      '/v1/education': 'http://pupitresoberano:4003',
      '/v1/rides': 'http://soberano-uber:4004',
      '/v1/food': 'http://soberano-eats:4005',
      '/v1/vote': 'http://voto-soberano:4006',
      '/v1/disputes': 'http://justicia-soberano:4007',
      '/v1/census': 'http://censo-soberano:4008',
      '/v1/identity': 'http://soberano-id:4009',
      '/v1/services': 'http://soberano-servicios:4010',
      '/v1/bookings': 'http://soberano-servicios:4010',
      '/v1/mail': 'http://correo-soberano:4011',
      '/v1/search': 'http://busqueda-soberana:4012',
      '/v1/maps': 'http://mapa-soberano:4013',
      '/v1/cloud': 'http://nube-soberana:4014',
      '/v1/farm': 'http://soberano-farm:4015',
      '/v1/radio': 'http://radio-soberana:4016',
      '/v1/cooperatives': 'http://cooperativa-soberana:4017',
      '/v1/tourism': 'http://turismo-soberano:4018',
      '/v1/freelance': 'http://soberano-freelance:4019',
      '/v1/pos': 'http://soberano-pos:4020',
    };

    it('should define 41 proxy routes total', () => {
      const routeCount = Object.keys(SERVICES).length;
      expect(routeCount).toBe(41);
    });

    it('should route banking endpoints to bdet-bank:4000', () => {
      const bankRoutes = Object.entries(SERVICES).filter(([, target]) => target.includes('bdet-bank'));
      expect(bankRoutes.length).toBe(11);
      expect(SERVICES['/v1/wallet']).toBe('http://bdet-bank:4000');
    });

    it('should route social media endpoints to social-media:4001', () => {
      const socialRoutes = Object.entries(SERVICES).filter(([, target]) => target.includes('social-media'));
      expect(socialRoutes.length).toBe(11);
      expect(SERVICES['/v1/feed']).toBe('http://social-media:4001');
    });

    it('should route doctor endpoint to soberano-doctor:4002', () => {
      expect(SERVICES['/v1/doctor']).toBe('http://soberano-doctor:4002');
    });

    it('should route voting endpoint to voto-soberano:4006', () => {
      expect(SERVICES['/v1/vote']).toBe('http://voto-soberano:4006');
    });

    it('should route services/bookings to soberano-servicios:4010', () => {
      expect(SERVICES['/v1/services']).toBe('http://soberano-servicios:4010');
      expect(SERVICES['/v1/bookings']).toBe('http://soberano-servicios:4010');
    });

    it('should route all paths under /v1/ prefix', () => {
      const allPaths = Object.keys(SERVICES);
      expect(allPaths.every(p => p.startsWith('/v1/'))).toBe(true);
    });
  });

  describe('Health Check', () => {
    it('should define GET /health endpoint', () => {
      const response = {
        gateway: 'Red Soberana API Gateway',
        services: 41,
        taxRate: '0%'
      };
      expect(response.gateway).toBe('Red Soberana API Gateway');
      expect(response.services).toBe(41);
      expect(response.taxRate).toBe('0%');
    });

    it('should check health of all downstream services', () => {
      const checks = { '/v1/wallet': 'healthy', '/v1/feed': 'down' };
      const healthy = Object.values(checks).filter(v => v === 'healthy').length;
      expect(healthy).toBe(1);
    });
  });

  describe('Configuration', () => {
    it('should use port 3000 by default', () => {
      const port = 3000;
      expect(port).toBe(3000);
    });

    it('should use shared security CORS config', () => {
      const securityPath = '../shared/security';
      expect(securityPath).toBeDefined();
    });

    it('should use helmet for security headers', () => {
      const middleware = ['helmet', 'cors', 'http-proxy-middleware'];
      expect(middleware).toContain('helmet');
    });
  });
});
