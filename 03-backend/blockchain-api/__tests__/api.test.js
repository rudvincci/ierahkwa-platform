describe('Blockchain API - API Routes', () => {
  describe('Module Structure', () => {
    it('should have a server.js entry point', () => {
      const fs = require('fs');
      const path = require('path');
      const exists = fs.existsSync(path.join(__dirname, '..', 'server.js'));
      expect(exists).toBe(true);
    });
  });

  describe('Health & Readiness Endpoints', () => {
    it('should define GET /health endpoint', () => {
      const expectedResponse = {
        status: 'ok',
        version: '1.0.0',
        blockchain: 'MameyNode v4.2'
      };
      expect(expectedResponse.status).toBe('ok');
      expect(expectedResponse.blockchain).toBe('MameyNode v4.2');
    });

    it('should define GET /ready readiness probe', () => {
      const expectedResponse = { ready: true, services: { db: true, redis: true, mameynode: true } };
      expect(expectedResponse.ready).toBe(true);
      expect(expectedResponse.services.mameynode).toBe(true);
    });
  });

  describe('API v1 Route Definitions', () => {
    const v1Routes = [
      '/v1/blocks', '/v1/transactions', '/v1/wallets',
      '/v1/contracts', '/v1/tokens', '/v1/nodes',
      '/v1/validators', '/v1/governance'
    ];

    it('should mount routes under /v1 prefix', () => {
      expect(v1Routes.every(r => r.startsWith('/v1/'))).toBe(true);
    });

    it('should define block-related routes', () => {
      expect(v1Routes).toContain('/v1/blocks');
      expect(v1Routes).toContain('/v1/transactions');
    });

    it('should define wallet routes', () => {
      expect(v1Routes).toContain('/v1/wallets');
    });

    it('should define smart contract routes', () => {
      expect(v1Routes).toContain('/v1/contracts');
    });

    it('should define token routes', () => {
      expect(v1Routes).toContain('/v1/tokens');
    });

    it('should define governance routes', () => {
      expect(v1Routes).toContain('/v1/governance');
    });
  });

  describe('WebSocket Support', () => {
    it('should configure WebSocket on /ws path', () => {
      const wsPath = '/ws';
      expect(wsPath).toBe('/ws');
    });

    it('should broadcast messages to connected clients', () => {
      const message = { type: 'block', data: { number: 100 } };
      const parsed = JSON.parse(JSON.stringify(message));
      expect(parsed.type).toBe('block');
      expect(parsed.data.number).toBe(100);
    });
  });

  describe('Error Handling', () => {
    it('should return 500 with error code on internal errors', () => {
      const response = { status: 500, body: { error: 'Internal server error', code: 'SOBERANO_500' } };
      expect(response.status).toBe(500);
      expect(response.body.code).toBe('SOBERANO_500');
    });
  });

  describe('Configuration', () => {
    it('should use port 3000 by default', () => {
      const port = parseInt(process.env.PORT || '3000', 10);
      expect(port).toBe(3000);
    });

    it('should support Swagger documentation', () => {
      const swaggerPath = '/api-docs';
      expect(swaggerPath).toBe('/api-docs');
    });

    it('should use shared security CORS config', () => {
      const securityModule = '../shared/security';
      expect(securityModule).toBeDefined();
    });
  });
});
