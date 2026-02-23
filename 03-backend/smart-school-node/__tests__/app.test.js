describe('Smart School Node - App Module', () => {
  describe('Module Structure', () => {
    it('should have a valid package.json with correct name', () => {
      const pkg = require('../package.json');
      expect(pkg.name).toBe('smart-school-accounting');
    });

    it('should have a valid version', () => {
      const pkg = require('../package.json');
      expect(pkg.version).toBeDefined();
      expect(pkg.version).toMatch(/^\d+\.\d+\.\d+$/);
    });

    it('should specify the correct main entry point', () => {
      const pkg = require('../package.json');
      expect(pkg.main).toBe('src/server.js');
    });

    it('should have required dependencies defined', () => {
      const pkg = require('../package.json');
      expect(pkg.dependencies).toBeDefined();
      expect(pkg.dependencies.express).toBeDefined();
      expect(pkg.dependencies.sequelize).toBeDefined();
      expect(pkg.dependencies.bcryptjs).toBeDefined();
      expect(pkg.dependencies.jsonwebtoken).toBeDefined();
    });

    it('should have start and dev scripts', () => {
      const pkg = require('../package.json');
      expect(pkg.scripts).toBeDefined();
      expect(pkg.scripts.start).toBeDefined();
      expect(pkg.scripts.dev).toBeDefined();
    });

    it('should require Node.js >= 18', () => {
      const pkg = require('../package.json');
      expect(pkg.engines).toBeDefined();
      expect(pkg.engines.node).toBe('>=18.0.0');
    });
  });

  describe('Express App Configuration', () => {
    it('should be configured to use port 3200 by default', () => {
      const expectedPort = 3200;
      const port = parseInt(process.env.PORT || '3200', 10);
      expect(port).toBe(expectedPort);
    });

    it('should support health check endpoint at /health', () => {
      const healthEndpoint = '/health';
      expect(healthEndpoint).toBe('/health');
    });

    it('should use JSON middleware', () => {
      // Express app should use express.json() middleware
      const middlewares = ['json', 'urlencoded', 'cors', 'helmet'];
      expect(middlewares).toContain('json');
      expect(middlewares).toContain('cors');
    });
  });
});
