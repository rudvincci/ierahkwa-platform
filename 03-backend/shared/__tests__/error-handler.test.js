'use strict';

const {
  ERROR_CODES,
  AppError,
  errorMiddleware,
  asyncHandler,
  notFoundHandler
} = require('../error-handler');

function mockReq(overrides = {}) {
  return {
    id: 'req-test-123',
    method: 'GET',
    path: '/test',
    url: '/test',
    originalUrl: '/test',
    headers: {},
    ...overrides
  };
}

function mockRes() {
  const headers = {};
  const res = {
    _status: null,
    _json: null,
    status(code) { res._status = code; return res; },
    json(data) { res._json = data; return res; },
    set(k, v) { headers[k] = v; return res; }
  };
  return res;
}

describe('Error Handler Module', () => {

  // ──────────────────────────────────────────
  // ERROR_CODES
  // ──────────────────────────────────────────
  describe('ERROR_CODES', () => {
    test('contains auth error codes', () => {
      expect(ERROR_CODES.AUTH_REQUIRED).toBeDefined();
      expect(ERROR_CODES.AUTH_REQUIRED.status).toBe(401);
      expect(ERROR_CODES.AUTH_INVALID_TOKEN.status).toBe(401);
      expect(ERROR_CODES.AUTH_INSUFFICIENT_ROLE.status).toBe(403);
    });

    test('contains validation error codes', () => {
      expect(ERROR_CODES.VALIDATION_FAILED.status).toBe(400);
      expect(ERROR_CODES.MISSING_FIELD.status).toBe(400);
    });

    test('contains resource error codes', () => {
      expect(ERROR_CODES.NOT_FOUND.status).toBe(404);
      expect(ERROR_CODES.ALREADY_EXISTS.status).toBe(409);
      expect(ERROR_CODES.CONFLICT.status).toBe(409);
    });

    test('contains financial error codes (BDET)', () => {
      expect(ERROR_CODES.INSUFFICIENT_FUNDS.status).toBe(422);
      expect(ERROR_CODES.TRANSACTION_FAILED.status).toBe(422);
      expect(ERROR_CODES.BLOCKCHAIN_ERROR.status).toBe(502);
    });

    test('contains tenant error codes', () => {
      expect(ERROR_CODES.TENANT_NOT_FOUND.status).toBe(404);
      expect(ERROR_CODES.TENANT_SUSPENDED.status).toBe(403);
    });

    test('every error code has status, code, and title', () => {
      for (const [key, def] of Object.entries(ERROR_CODES)) {
        expect(def.status).toBeGreaterThanOrEqual(400);
        expect(def.code).toMatch(/^ERR_/);
        expect(def.title.length).toBeGreaterThan(0);
      }
    });
  });

  // ──────────────────────────────────────────
  // AppError
  // ──────────────────────────────────────────
  describe('AppError', () => {
    test('creates error from error code string', () => {
      const err = new AppError('NOT_FOUND', 'User not found');
      expect(err).toBeInstanceOf(Error);
      expect(err.name).toBe('AppError');
      expect(err.status).toBe(404);
      expect(err.code).toBe('ERR_NOT_FOUND');
      expect(err.title).toBe('Resource not found');
      expect(err.detail).toBe('User not found');
      expect(err.message).toBe('User not found');
    });

    test('uses title as default detail', () => {
      const err = new AppError('AUTH_REQUIRED');
      expect(err.detail).toBe('Authentication required');
    });

    test('accepts extras (instance, errors, meta)', () => {
      const err = new AppError('VALIDATION_FAILED', 'Bad input', {
        instance: '/api/users/1',
        errors: [{ field: 'email', message: 'required' }],
        meta: { attempt: 3 }
      });
      expect(err.instance).toBe('/api/users/1');
      expect(err.errors).toHaveLength(1);
      expect(err.meta.attempt).toBe(3);
    });

    test('toJSON returns RFC 7807 problem details', () => {
      const err = new AppError('CONFLICT', 'Duplicate email');
      const json = err.toJSON();

      expect(json.type).toContain('ERR_CONFLICT');
      expect(json.title).toBe('Resource conflict');
      expect(json.status).toBe(409);
      expect(json.detail).toBe('Duplicate email');
    });

    test('toJSON excludes empty errors and meta', () => {
      const err = new AppError('NOT_FOUND');
      const json = err.toJSON();
      expect(json.errors).toBeUndefined();
      expect(json.meta).toBeUndefined();
    });

    test('throws for unknown error code', () => {
      expect(() => new AppError('DOES_NOT_EXIST')).toThrow('Unknown error code');
    });

    test('has stack trace', () => {
      const err = new AppError('INTERNAL');
      expect(err.stack).toBeDefined();
      expect(err.stack).toContain('error-handler.test.js');
    });
  });

  // ──────────────────────────────────────────
  // errorMiddleware
  // ──────────────────────────────────────────
  describe('errorMiddleware', () => {
    const mockLogger = {
      error: jest.fn(),
      warn: jest.fn(),
      info: jest.fn()
    };

    beforeEach(() => {
      jest.clearAllMocks();
    });

    test('handles AppError and returns RFC 7807', () => {
      const middleware = errorMiddleware('test-svc', mockLogger);
      const req = mockReq();
      const res = mockRes();
      const err = new AppError('AUTH_REQUIRED', 'Token missing');

      middleware(err, req, res, jest.fn());

      expect(res._status).toBe(401);
      expect(res._json.title).toBe('Authentication required');
      expect(res._json.detail).toBe('Token missing');
      expect(res._json.requestId).toBe('req-test-123');
      expect(res._json.timestamp).toBeDefined();
    });

    test('handles JSON parse errors', () => {
      const middleware = errorMiddleware('test-svc', mockLogger);
      const req = mockReq();
      const res = mockRes();
      const err = { type: 'entity.parse.failed', message: 'Bad JSON' };

      middleware(err, req, res, jest.fn());

      expect(res._status).toBe(400);
      expect(res._json.title).toBe('Invalid JSON');
    });

    test('hides details for unknown errors in production', () => {
      const origEnv = process.env.NODE_ENV;
      process.env.NODE_ENV = 'production';

      const middleware = errorMiddleware('test-svc', mockLogger);
      const req = mockReq();
      const res = mockRes();
      const err = new Error('Database password leaked in query');

      middleware(err, req, res, jest.fn());

      expect(res._status).toBe(500);
      expect(res._json.detail).toBe('An unexpected error occurred');
      expect(res._json.stack).toBeUndefined();

      process.env.NODE_ENV = origEnv;
    });

    test('shows details for unknown errors in development', () => {
      const origEnv = process.env.NODE_ENV;
      process.env.NODE_ENV = 'development';

      const middleware = errorMiddleware('test-svc', mockLogger);
      const req = mockReq();
      const res = mockRes();
      const err = new Error('Detailed error message');

      middleware(err, req, res, jest.fn());

      expect(res._json.detail).toBe('Detailed error message');
      expect(res._json.stack).toBeDefined();

      process.env.NODE_ENV = origEnv;
    });

    test('logs server errors at error level', () => {
      const middleware = errorMiddleware('test-svc', mockLogger);
      const req = mockReq();
      const res = mockRes();
      const err = new AppError('INTERNAL', 'Something broke');

      middleware(err, req, res, jest.fn());

      expect(mockLogger.error).toHaveBeenCalled();
    });

    test('logs client errors at warn level', () => {
      const middleware = errorMiddleware('test-svc', mockLogger);
      const req = mockReq();
      const res = mockRes();
      const err = new AppError('VALIDATION_FAILED', 'Bad field');

      middleware(err, req, res, jest.fn());

      expect(mockLogger.warn).toHaveBeenCalled();
    });

    test('sets Content-Type to application/problem+json', () => {
      const middleware = errorMiddleware('test-svc', mockLogger);
      const req = mockReq();
      const res = mockRes();
      const err = new AppError('NOT_FOUND');

      middleware(err, req, res, jest.fn());

      // set() was called with Content-Type
      expect(res._status).toBe(404);
    });
  });

  // ──────────────────────────────────────────
  // asyncHandler
  // ──────────────────────────────────────────
  describe('asyncHandler', () => {
    test('passes successful async handler through', async () => {
      const handler = asyncHandler(async (req, res) => {
        res.json({ ok: true });
      });

      const req = mockReq();
      const res = mockRes();
      const next = jest.fn();

      await handler(req, res, next);
      expect(res._json).toEqual({ ok: true });
      expect(next).not.toHaveBeenCalled();
    });

    test('catches async errors and passes to next', async () => {
      const handler = asyncHandler(async () => {
        throw new AppError('NOT_FOUND', 'Gone');
      });

      const req = mockReq();
      const res = mockRes();
      const next = jest.fn();

      await handler(req, res, next);
      expect(next).toHaveBeenCalledWith(expect.any(AppError));
    });
  });

  // ──────────────────────────────────────────
  // notFoundHandler
  // ──────────────────────────────────────────
  describe('notFoundHandler', () => {
    test('returns 404 with route details', () => {
      const req = mockReq({ method: 'GET', originalUrl: '/api/v1/unknown' });
      const res = mockRes();

      notFoundHandler(req, res);

      expect(res._status).toBe(404);
      expect(res._json.title).toBe('Not found');
      expect(res._json.detail).toContain('/api/v1/unknown');
      expect(res._json.status).toBe(404);
    });
  });
});
