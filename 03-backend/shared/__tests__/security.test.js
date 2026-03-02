'use strict';

const {
  corsConfig,
  rateLimiterMiddleware,
  rateLimiters,
  sanitizeInput,
  securityHeaders,
  requestId,
  securityLogger,
  errorHandler,
  jwtUtils,
  fileUploadSecurity,
  tenantIsolation
} = require('../security');

// Mock Express req/res/next
function mockReq(overrides = {}) {
  return {
    ip: '127.0.0.1',
    connection: { remoteAddress: '127.0.0.1' },
    headers: { 'user-agent': 'test-agent' },
    method: 'GET',
    path: '/test',
    body: {},
    query: {},
    params: {},
    ...overrides
  };
}

function mockRes() {
  const headers = {};
  const res = {
    _status: null,
    _json: null,
    _headers: headers,
    status(code) { res._status = code; return res; },
    json(data) { res._json = data; return res; },
    setHeader(k, v) { headers[k] = v; },
    getHeader(k) { return headers[k]; },
    removeHeader(k) { delete headers[k]; },
    set(k, v) { headers[k] = v; return res; }
  };
  return res;
}

describe('Security Module', () => {

  // ──────────────────────────────────────────
  // CORS
  // ──────────────────────────────────────────
  describe('corsConfig', () => {
    test('returns valid CORS configuration object', () => {
      const config = corsConfig();
      expect(config.credentials).toBe(true);
      expect(config.methods).toContain('GET');
      expect(config.methods).toContain('POST');
      expect(config.maxAge).toBe(600);
      expect(typeof config.origin).toBe('function');
    });

    test('allows requests with no origin (server-to-server)', () => {
      const config = corsConfig();
      const cb = jest.fn();
      config.origin(undefined, cb);
      expect(cb).toHaveBeenCalledWith(null, true);
    });

    test('allows whitelisted origin', () => {
      const config = corsConfig();
      const cb = jest.fn();
      config.origin('http://localhost:3000', cb);
      expect(cb).toHaveBeenCalledWith(null, true);
    });

    test('rejects unknown origin', () => {
      const config = corsConfig();
      const cb = jest.fn();
      config.origin('http://evil.com', cb);
      expect(cb).toHaveBeenCalledWith(expect.any(Error));
    });
  });

  // ──────────────────────────────────────────
  // Rate Limiter
  // ──────────────────────────────────────────
  describe('rateLimiterMiddleware', () => {
    test('allows requests under the limit', () => {
      const limiter = rateLimiterMiddleware({ max: 5, windowMs: 60000 });
      const req = mockReq({ ip: '10.0.0.1' });
      const res = mockRes();
      const next = jest.fn();

      limiter(req, res, next);
      expect(next).toHaveBeenCalled();
      expect(res._headers['X-RateLimit-Limit']).toBe(5);
      expect(res._headers['X-RateLimit-Remaining']).toBe(4);
    });

    test('blocks requests over the limit', () => {
      const limiter = rateLimiterMiddleware({ max: 2, windowMs: 60000 });
      const req = mockReq({ ip: '10.0.0.2' });
      const res = mockRes();
      const next = jest.fn();

      limiter(req, res, next); // 1
      limiter(req, res, next); // 2
      limiter(req, res, next); // 3 -> blocked

      expect(res._status).toBe(429);
      expect(res._json.error).toMatch(/Too many requests/);
    });

    test('sets Retry-After header when blocked', () => {
      const limiter = rateLimiterMiddleware({ max: 1, windowMs: 60000 });
      const req = mockReq({ ip: '10.0.0.3' });
      const res = mockRes();
      const next = jest.fn();

      limiter(req, res, next);
      limiter(req, res, next);

      expect(res._headers['Retry-After']).toBeDefined();
    });
  });

  describe('rateLimiters presets', () => {
    test('auth limiter exists and returns middleware', () => {
      const middleware = rateLimiters.auth();
      expect(typeof middleware).toBe('function');
    });

    test('api limiter exists and returns middleware', () => {
      const middleware = rateLimiters.api();
      expect(typeof middleware).toBe('function');
    });

    test('public limiter exists and returns middleware', () => {
      const middleware = rateLimiters.public();
      expect(typeof middleware).toBe('function');
    });

    test('upload limiter exists and returns middleware', () => {
      const middleware = rateLimiters.upload();
      expect(typeof middleware).toBe('function');
    });
  });

  // ──────────────────────────────────────────
  // Input Sanitizer
  // ──────────────────────────────────────────
  describe('sanitizeInput', () => {
    test('strips script tags from body', () => {
      const req = mockReq({ body: { name: '<script>alert("xss")</script>Hello' } });
      const res = mockRes();
      const next = jest.fn();

      sanitizeInput(req, res, next);
      expect(req.body.name).not.toContain('<script>');
      expect(req.body.name).toContain('Hello');
      expect(next).toHaveBeenCalled();
    });

    test('removes null bytes', () => {
      const req = mockReq({ body: { name: 'test\0value' } });
      const res = mockRes();
      const next = jest.fn();

      sanitizeInput(req, res, next);
      expect(req.body.name).toBe('testvalue');
    });

    test('strips SQL injection patterns', () => {
      const req = mockReq({ body: { search: "'; DROP TABLE users; --" } });
      const res = mockRes();
      const next = jest.fn();

      sanitizeInput(req, res, next);
      expect(req.body.search).not.toMatch(/DROP/i);
    });

    test('prevents prototype pollution', () => {
      const req = mockReq({ body: { __proto__: { admin: true }, constructor: 'evil', name: 'safe' } });
      const res = mockRes();
      const next = jest.fn();

      sanitizeInput(req, res, next);
      expect(req.body.__proto__).toBeUndefined();
      expect(req.body.constructor).toBeUndefined();
      expect(req.body.name).toBe('safe');
    });

    test('sanitizes query params', () => {
      const req = mockReq({ query: { q: '<script>x</script>test' } });
      const res = mockRes();
      const next = jest.fn();

      sanitizeInput(req, res, next);
      expect(req.query.q).not.toContain('<script>');
    });

    test('handles nested objects', () => {
      const req = mockReq({ body: { user: { name: '<script>x</script>Bob' } } });
      const res = mockRes();
      const next = jest.fn();

      sanitizeInput(req, res, next);
      expect(req.body.user.name).not.toContain('<script>');
      expect(req.body.user.name).toContain('Bob');
    });

    test('handles arrays', () => {
      const req = mockReq({ body: { tags: ['<script>x</script>safe', 'normal'] } });
      const res = mockRes();
      const next = jest.fn();

      sanitizeInput(req, res, next);
      expect(req.body.tags[0]).not.toContain('<script>');
      expect(req.body.tags[1]).toBe('normal');
    });

    test('passes through numbers and booleans', () => {
      const req = mockReq({ body: { count: 42, active: true } });
      const res = mockRes();
      const next = jest.fn();

      sanitizeInput(req, res, next);
      expect(req.body.count).toBe(42);
      expect(req.body.active).toBe(true);
    });
  });

  // ──────────────────────────────────────────
  // Security Headers
  // ──────────────────────────────────────────
  describe('securityHeaders', () => {
    test('sets all OWASP security headers', () => {
      const req = mockReq();
      const res = mockRes();
      const next = jest.fn();

      securityHeaders(req, res, next);

      expect(res._headers['X-Content-Type-Options']).toBe('nosniff');
      expect(res._headers['X-Frame-Options']).toBe('DENY');
      expect(res._headers['Strict-Transport-Security']).toContain('max-age=');
      expect(res._headers['Cache-Control']).toContain('no-store');
      expect(res._headers['Referrer-Policy']).toBe('strict-origin-when-cross-origin');
      expect(res._headers['Content-Security-Policy']).toContain("default-src 'self'");
      expect(next).toHaveBeenCalled();
    });

    test('disables XSS protection (per OWASP recommendation)', () => {
      const req = mockReq();
      const res = mockRes();
      const next = jest.fn();

      securityHeaders(req, res, next);
      expect(res._headers['X-XSS-Protection']).toBe('0');
    });
  });

  // ──────────────────────────────────────────
  // Request ID
  // ──────────────────────────────────────────
  describe('requestId', () => {
    test('assigns UUID when no x-request-id header', () => {
      const req = mockReq();
      const res = mockRes();
      const next = jest.fn();

      requestId(req, res, next);

      expect(req.id).toBeDefined();
      expect(req.id).toMatch(/^[0-9a-f-]{36}$/);
      expect(res._headers['X-Request-Id']).toBe(req.id);
      expect(next).toHaveBeenCalled();
    });

    test('uses existing x-request-id header', () => {
      const req = mockReq({ headers: { 'x-request-id': 'custom-id-123' } });
      const res = mockRes();
      const next = jest.fn();

      requestId(req, res, next);

      expect(req.id).toBe('custom-id-123');
    });
  });

  // ──────────────────────────────────────────
  // Security Logger
  // ──────────────────────────────────────────
  describe('securityLogger', () => {
    test('creates logger with all methods', () => {
      const logger = securityLogger('test-service');
      expect(typeof logger.authSuccess).toBe('function');
      expect(typeof logger.authFailure).toBe('function');
      expect(typeof logger.accessDenied).toBe('function');
      expect(typeof logger.suspiciousActivity).toBe('function');
      expect(typeof logger.dataAccess).toBe('function');
    });

    test('authSuccess logs structured JSON', () => {
      const spy = jest.spyOn(console, 'log').mockImplementation();
      const logger = securityLogger('test-service');
      const req = mockReq({ id: 'req-123' });

      logger.authSuccess(req, 'user-456');

      expect(spy).toHaveBeenCalled();
      const logged = JSON.parse(spy.mock.calls[0][0]);
      expect(logged.event).toBe('AUTH_SUCCESS');
      expect(logged.service).toBe('test-service');
      expect(logged.userId).toBe('user-456');
      spy.mockRestore();
    });

    test('authFailure logs to console.warn', () => {
      const spy = jest.spyOn(console, 'warn').mockImplementation();
      const logger = securityLogger('test-service');
      const req = mockReq({ id: 'req-123' });

      logger.authFailure(req, 'bad password');

      expect(spy).toHaveBeenCalled();
      const logged = JSON.parse(spy.mock.calls[0][0]);
      expect(logged.event).toBe('AUTH_FAILURE');
      expect(logged.reason).toBe('bad password');
      spy.mockRestore();
    });
  });

  // ──────────────────────────────────────────
  // Error Handler
  // ──────────────────────────────────────────
  describe('errorHandler', () => {
    test('returns 500 for generic errors and hides message', () => {
      const spy = jest.spyOn(console, 'error').mockImplementation();
      const handler = errorHandler('test-service');
      const req = mockReq({ id: 'req-1' });
      const res = mockRes();
      const err = new Error('Database leaked credentials');

      handler(err, req, res, jest.fn());

      expect(res._status).toBe(500);
      expect(res._json.error).toBe('Internal server error');
      expect(res._json.requestId).toBe('req-1');
      spy.mockRestore();
    });

    test('exposes message for client errors (4xx)', () => {
      const spy = jest.spyOn(console, 'error').mockImplementation();
      const handler = errorHandler('test-service');
      const req = mockReq({ id: 'req-2' });
      const res = mockRes();
      const err = new Error('Not found');
      err.statusCode = 404;

      handler(err, req, res, jest.fn());

      expect(res._status).toBe(404);
      expect(res._json.error).toBe('Not found');
      spy.mockRestore();
    });
  });

  // ──────────────────────────────────────────
  // JWT Utilities
  // ──────────────────────────────────────────
  describe('jwtUtils', () => {
    test('generateSecret creates 128-char hex string', () => {
      const secret = jwtUtils.generateSecret();
      expect(secret).toMatch(/^[0-9a-f]{128}$/);
    });

    test('isSecretStrong rejects short secrets', () => {
      expect(jwtUtils.isSecretStrong('short')).toBe(false);
    });

    test('isSecretStrong rejects weak patterns', () => {
      expect(jwtUtils.isSecretStrong('this-is-a-change-me-secret-that-is-long-enough')).toBe(false);
      expect(jwtUtils.isSecretStrong('my-default-password-for-jwt-signing-here')).toBe(false);
    });

    test('isSecretStrong accepts strong secrets', () => {
      const strong = jwtUtils.generateSecret();
      expect(jwtUtils.isSecretStrong(strong)).toBe(true);
    });

    test('generateRefreshToken creates 96-char hex string', () => {
      const token = jwtUtils.generateRefreshToken();
      expect(token).toMatch(/^[0-9a-f]{96}$/);
    });
  });

  // ──────────────────────────────────────────
  // File Upload Security
  // ──────────────────────────────────────────
  describe('fileUploadSecurity', () => {
    test('validates JPEG magic bytes', () => {
      const jpegBuffer = Buffer.from([0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10]);
      expect(fileUploadSecurity.validateMagicBytes(jpegBuffer, 'image/jpeg')).toBe(true);
    });

    test('validates PNG magic bytes', () => {
      const pngBuffer = Buffer.from([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A]);
      expect(fileUploadSecurity.validateMagicBytes(pngBuffer, 'image/png')).toBe(true);
    });

    test('rejects mismatched magic bytes', () => {
      const fakeJpeg = Buffer.from([0x00, 0x00, 0x00, 0x00]);
      expect(fileUploadSecurity.validateMagicBytes(fakeJpeg, 'image/jpeg')).toBe(false);
    });

    test('rejects unknown MIME type', () => {
      const buf = Buffer.from([0xFF, 0xD8, 0xFF]);
      expect(fileUploadSecurity.validateMagicBytes(buf, 'application/exe')).toBe(false);
    });

    test('sanitizeFilename removes path traversal', () => {
      expect(fileUploadSecurity.sanitizeFilename('../../../etc/passwd')).toBe('etc_passwd');
    });

    test('sanitizeFilename removes null bytes', () => {
      expect(fileUploadSecurity.sanitizeFilename('file\0.txt')).toBe('file.txt');
    });

    test('sanitizeFilename removes leading dots', () => {
      expect(fileUploadSecurity.sanitizeFilename('...hidden')).toBe('hidden');
    });

    test('sanitizeFilename returns unnamed_file for empty input', () => {
      expect(fileUploadSecurity.sanitizeFilename('...')).toBe('unnamed_file');
    });

    test('sanitizeFilename limits length to 255', () => {
      const long = 'a'.repeat(300) + '.txt';
      expect(fileUploadSecurity.sanitizeFilename(long).length).toBeLessThanOrEqual(255);
    });

    test('MAX_SIZES defines expected limits', () => {
      expect(fileUploadSecurity.MAX_SIZES.image).toBe(10 * 1024 * 1024);
      expect(fileUploadSecurity.MAX_SIZES.document).toBe(50 * 1024 * 1024);
      expect(fileUploadSecurity.MAX_SIZES.video).toBe(500 * 1024 * 1024);
    });
  });

  // ──────────────────────────────────────────
  // Tenant Isolation
  // ──────────────────────────────────────────
  describe('tenantIsolation', () => {
    test('accepts valid UUID tenant ID from header', () => {
      const req = mockReq({ headers: { 'x-tenant-id': '550e8400-e29b-41d4-a716-446655440000' } });
      const res = mockRes();
      const next = jest.fn();

      tenantIsolation(req, res, next);

      expect(req.tenantId).toBe('550e8400-e29b-41d4-a716-446655440000');
      expect(next).toHaveBeenCalled();
    });

    test('accepts numeric tenant ID', () => {
      const req = mockReq({ headers: { 'x-tenant-id': '123' } });
      const res = mockRes();
      const next = jest.fn();

      tenantIsolation(req, res, next);
      expect(req.tenantId).toBe('123');
      expect(next).toHaveBeenCalled();
    });

    test('rejects missing tenant ID', () => {
      const req = mockReq({ headers: {} });
      const res = mockRes();
      const next = jest.fn();

      tenantIsolation(req, res, next);

      expect(res._status).toBe(400);
      expect(res._json.error).toContain('Tenant');
      expect(next).not.toHaveBeenCalled();
    });

    test('rejects invalid tenant ID format', () => {
      const req = mockReq({ headers: { 'x-tenant-id': 'invalid-format!' } });
      const res = mockRes();
      const next = jest.fn();

      tenantIsolation(req, res, next);

      expect(res._status).toBe(400);
      expect(next).not.toHaveBeenCalled();
    });
  });
});
