'use strict';

const { createLogger, LOG_LEVELS } = require('../logger');

describe('Logger Module', () => {

  // ──────────────────────────────────────────
  // LOG_LEVELS
  // ──────────────────────────────────────────
  describe('LOG_LEVELS', () => {
    test('defines all standard levels', () => {
      expect(LOG_LEVELS.trace).toBe(10);
      expect(LOG_LEVELS.debug).toBe(20);
      expect(LOG_LEVELS.info).toBe(30);
      expect(LOG_LEVELS.warn).toBe(40);
      expect(LOG_LEVELS.error).toBe(50);
      expect(LOG_LEVELS.fatal).toBe(60);
    });

    test('levels are in ascending order', () => {
      expect(LOG_LEVELS.trace).toBeLessThan(LOG_LEVELS.debug);
      expect(LOG_LEVELS.debug).toBeLessThan(LOG_LEVELS.info);
      expect(LOG_LEVELS.info).toBeLessThan(LOG_LEVELS.warn);
      expect(LOG_LEVELS.warn).toBeLessThan(LOG_LEVELS.error);
      expect(LOG_LEVELS.error).toBeLessThan(LOG_LEVELS.fatal);
    });
  });

  // ──────────────────────────────────────────
  // createLogger
  // ──────────────────────────────────────────
  describe('createLogger', () => {
    test('returns logger with all level methods', () => {
      const log = createLogger('test-service');
      expect(typeof log.trace).toBe('function');
      expect(typeof log.debug).toBe('function');
      expect(typeof log.info).toBe('function');
      expect(typeof log.warn).toBe('function');
      expect(typeof log.error).toBe('function');
      expect(typeof log.fatal).toBe('function');
    });

    test('returns logger with utility methods', () => {
      const log = createLogger('test-service');
      expect(typeof log.child).toBe('function');
      expect(typeof log.requestLogger).toBe('function');
      expect(typeof log.startTimer).toBe('function');
    });

    test('info writes to stdout in JSON mode', () => {
      const spy = jest.spyOn(process.stdout, 'write').mockImplementation();
      const log = createLogger('test-service', { pretty: false, level: 'info' });

      log.info('hello world', { key: 'value' });

      expect(spy).toHaveBeenCalled();
      const output = spy.mock.calls[0][0];
      const parsed = JSON.parse(output.trim());
      expect(parsed.msg).toBe('hello world');
      expect(parsed.service).toBe('test-service');
      expect(parsed.key).toBe('value');
      expect(parsed.level).toBe(30);
      expect(parsed.levelName).toBe('info');
      expect(parsed.time).toBeDefined();
      expect(parsed.pid).toBe(process.pid);

      spy.mockRestore();
    });

    test('warn writes to stderr', () => {
      const spy = jest.spyOn(process.stderr, 'write').mockImplementation();
      const log = createLogger('test-service', { pretty: false, level: 'warn' });

      log.warn('warning message');

      expect(spy).toHaveBeenCalled();
      const parsed = JSON.parse(spy.mock.calls[0][0].trim());
      expect(parsed.levelName).toBe('warn');

      spy.mockRestore();
    });

    test('error writes to stderr', () => {
      const spy = jest.spyOn(process.stderr, 'write').mockImplementation();
      const log = createLogger('test-service', { pretty: false, level: 'error' });

      log.error('error message');

      expect(spy).toHaveBeenCalled();
      spy.mockRestore();
    });

    test('respects minimum log level', () => {
      const stdoutSpy = jest.spyOn(process.stdout, 'write').mockImplementation();
      const stderrSpy = jest.spyOn(process.stderr, 'write').mockImplementation();
      const log = createLogger('test-service', { pretty: false, level: 'warn' });

      log.debug('should be suppressed');
      log.info('should be suppressed too');

      expect(stdoutSpy).not.toHaveBeenCalled();

      stdoutSpy.mockRestore();
      stderrSpy.mockRestore();
    });

    test('redacts sensitive fields', () => {
      const spy = jest.spyOn(process.stdout, 'write').mockImplementation();
      const log = createLogger('test-service', { pretty: false, level: 'info' });

      log.info('login attempt', { username: 'admin', password: 'secret123', token: 'abc' });

      const output = spy.mock.calls[0][0];
      const parsed = JSON.parse(output.trim());
      expect(parsed.username).toBe('admin');
      expect(parsed.password).toBe('[REDACTED]');
      expect(parsed.token).toBe('[REDACTED]');

      spy.mockRestore();
    });

    test('redacts nested sensitive fields', () => {
      const spy = jest.spyOn(process.stdout, 'write').mockImplementation();
      const log = createLogger('test-service', { pretty: false, level: 'info' });

      log.info('data', { user: { name: 'Bob', authorization: 'Bearer xxx' } });

      const parsed = JSON.parse(spy.mock.calls[0][0].trim());
      expect(parsed.user.name).toBe('Bob');
      expect(parsed.user.authorization).toBe('[REDACTED]');

      spy.mockRestore();
    });
  });

  // ──────────────────────────────────────────
  // child logger
  // ──────────────────────────────────────────
  describe('child logger', () => {
    test('creates child with merged context', () => {
      const spy = jest.spyOn(process.stdout, 'write').mockImplementation();
      const log = createLogger('test-service', { pretty: false, level: 'info', context: { env: 'test' } });
      const child = log.child({ requestId: 'req-1' });

      child.info('child log');

      const parsed = JSON.parse(spy.mock.calls[0][0].trim());
      expect(parsed.env).toBe('test');
      expect(parsed.requestId).toBe('req-1');
      expect(parsed.service).toBe('test-service');

      spy.mockRestore();
    });
  });

  // ──────────────────────────────────────────
  // requestLogger middleware
  // ──────────────────────────────────────────
  describe('requestLogger', () => {
    test('returns Express middleware', () => {
      const log = createLogger('test-service');
      const middleware = log.requestLogger();
      expect(typeof middleware).toBe('function');
      expect(middleware.length).toBe(3); // req, res, next
    });

    test('assigns request ID and calls next', () => {
      const spy = jest.spyOn(process.stdout, 'write').mockImplementation();
      const log = createLogger('test-service', { pretty: false, level: 'info' });
      const middleware = log.requestLogger();

      const req = {
        method: 'GET',
        url: '/test',
        originalUrl: '/test',
        ip: '127.0.0.1',
        headers: {},
        connection: {}
      };
      const res = {
        setHeader: jest.fn(),
        on: jest.fn(),
        getHeader: jest.fn()
      };
      const next = jest.fn();

      middleware(req, res, next);

      expect(req.id).toBeDefined();
      expect(req.log).toBeDefined();
      expect(res.setHeader).toHaveBeenCalledWith('X-Request-Id', req.id);
      expect(next).toHaveBeenCalled();

      spy.mockRestore();
    });
  });

  // ──────────────────────────────────────────
  // startTimer
  // ──────────────────────────────────────────
  describe('startTimer', () => {
    test('measures operation duration', () => {
      const spy = jest.spyOn(process.stdout, 'write').mockImplementation();
      const log = createLogger('test-service', { pretty: false, level: 'info' });

      const end = log.startTimer('db-query');
      const duration = end({ rows: 10 });

      expect(typeof duration).toBe('number');
      expect(duration).toBeGreaterThanOrEqual(0);
      expect(spy).toHaveBeenCalled();

      const parsed = JSON.parse(spy.mock.calls[0][0].trim());
      expect(parsed.operation).toBe('db-query');
      expect(parsed.rows).toBe(10);
      expect(parsed.duration).toBeGreaterThanOrEqual(0);

      spy.mockRestore();
    });
  });
});
