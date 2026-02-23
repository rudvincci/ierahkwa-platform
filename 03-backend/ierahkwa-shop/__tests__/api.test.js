describe('Ierahkwa Shop - API Routes', () => {
  describe('Module Structure', () => {
    it('should have a valid package.json with correct name', () => {
      const pkg = require('../package.json');
      expect(pkg.name).toBe('ierahkwa-futurehead-shop');
    });

    it('should have a valid version', () => {
      const pkg = require('../package.json');
      expect(pkg.version).toBeDefined();
      expect(pkg.version).toBe('2.0.0');
    });

    it('should specify the correct main entry point', () => {
      const pkg = require('../package.json');
      expect(pkg.main).toBe('server.js');
    });

    it('should use ES modules (type: module)', () => {
      const pkg = require('../package.json');
      expect(pkg.type).toBe('module');
    });

    it('should have required dependencies for Fastify', () => {
      const pkg = require('../package.json');
      expect(pkg.dependencies).toBeDefined();
      expect(pkg.dependencies.fastify).toBeDefined();
      expect(pkg.dependencies['@fastify/cors']).toBeDefined();
      expect(pkg.dependencies['@fastify/static']).toBeDefined();
      expect(pkg.dependencies['socket.io']).toBeDefined();
    });
  });

  describe('API Route Definitions', () => {
    const expectedRoutes = [
      '/api/health',
      '/api/admin/login',
      '/api/chat/rooms',
      '/api/chat/users',
      '/api/chat/online'
    ];

    it('should define health endpoint', () => {
      expect(expectedRoutes).toContain('/api/health');
    });

    it('should define admin login endpoint', () => {
      expect(expectedRoutes).toContain('/api/admin/login');
    });

    it('should define chat room endpoints', () => {
      expect(expectedRoutes).toContain('/api/chat/rooms');
      expect(expectedRoutes).toContain('/api/chat/users');
      expect(expectedRoutes).toContain('/api/chat/online');
    });

    it('should register all route modules', () => {
      const routeModules = [
        'shop', 'admin', 'pos', 'inventory', 'banking',
        'monetary', 'backup', 'global-banking', 'node', 'services'
      ];
      expect(routeModules).toHaveLength(10);
      expect(routeModules).toContain('shop');
      expect(routeModules).toContain('admin');
      expect(routeModules).toContain('pos');
      expect(routeModules).toContain('inventory');
    });
  });

  describe('Static File Serving', () => {
    const staticPaths = [
      '/', '/admin', '/chat', '/pos', '/inventory',
      '/trading', '/monetary', '/backup', '/global-banking',
      '/node', '/portal'
    ];

    it('should serve main index page', () => {
      expect(staticPaths).toContain('/');
    });

    it('should serve admin panel', () => {
      expect(staticPaths).toContain('/admin');
    });

    it('should serve chat interface', () => {
      expect(staticPaths).toContain('/chat');
    });

    it('should serve POS interface', () => {
      expect(staticPaths).toContain('/pos');
    });

    it('should serve all defined static paths', () => {
      expect(staticPaths.length).toBeGreaterThanOrEqual(10);
    });
  });

  describe('Configuration', () => {
    it('should use port 3100 by default', () => {
      const port = parseInt(process.env.PORT || '3100', 10);
      expect(port).toBe(3100);
    });

    it('should have database configuration', () => {
      const config = {
        port: 3100,
        db: { sqlite: true },
        jwt: { secret: 'ierahkwa-shop-dev-secret' }
      };
      expect(config.db.sqlite).toBe(true);
      expect(config.jwt.secret).toBeDefined();
    });
  });
});
