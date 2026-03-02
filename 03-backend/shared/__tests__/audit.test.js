'use strict';

const {
  createAuditLogger,
  AUDIT_CATEGORIES,
  RISK_LEVELS
} = require('../audit');

function mockReq(overrides = {}) {
  return {
    id: 'req-audit-test',
    ip: '192.168.1.1',
    connection: { remoteAddress: '192.168.1.1' },
    headers: { 'user-agent': 'audit-test-agent' },
    method: 'POST',
    path: '/v1/test',
    originalUrl: '/v1/test',
    user: { id: 'user-123', tenantId: 'tenant-1' },
    tenantId: 'tenant-1',
    ...overrides
  };
}

describe('Audit Module', () => {

  // ──────────────────────────────────────────
  // Constants
  // ──────────────────────────────────────────
  describe('AUDIT_CATEGORIES', () => {
    test('contains financial categories', () => {
      expect(AUDIT_CATEGORIES.TRANSACTION).toBe('TRANSACTION');
      expect(AUDIT_CATEGORIES.BALANCE_CHANGE).toBe('BALANCE_CHANGE');
      expect(AUDIT_CATEGORIES.ESCROW).toBe('ESCROW');
      expect(AUDIT_CATEGORIES.REFUND).toBe('REFUND');
    });

    test('contains auth categories', () => {
      expect(AUDIT_CATEGORIES.AUTH_LOGIN).toBe('AUTH_LOGIN');
      expect(AUDIT_CATEGORIES.AUTH_FAILED).toBe('AUTH_FAILED');
      expect(AUDIT_CATEGORIES.AUTH_MFA).toBe('AUTH_MFA');
    });

    test('contains governance categories', () => {
      expect(AUDIT_CATEGORIES.VOTE_CAST).toBe('VOTE_CAST');
      expect(AUDIT_CATEGORIES.PROPOSAL_CREATED).toBe('PROPOSAL_CREATED');
      expect(AUDIT_CATEGORIES.SOVEREIGNTY_ACTION).toBe('SOVEREIGNTY_ACTION');
    });

    test('contains system categories', () => {
      expect(AUDIT_CATEGORIES.SYSTEM_STARTUP).toBe('SYSTEM_STARTUP');
      expect(AUDIT_CATEGORIES.SYSTEM_SHUTDOWN).toBe('SYSTEM_SHUTDOWN');
    });
  });

  describe('RISK_LEVELS', () => {
    test('defines all 4 levels', () => {
      expect(RISK_LEVELS.LOW).toBe('LOW');
      expect(RISK_LEVELS.MEDIUM).toBe('MEDIUM');
      expect(RISK_LEVELS.HIGH).toBe('HIGH');
      expect(RISK_LEVELS.CRITICAL).toBe('CRITICAL');
    });
  });

  // ──────────────────────────────────────────
  // createAuditLogger
  // ──────────────────────────────────────────
  describe('createAuditLogger', () => {
    let audit;
    let capturedEvents;

    beforeEach(() => {
      capturedEvents = [];
      audit = createAuditLogger('test-service', {
        output: 'callback',
        onAuditEvent: (event) => capturedEvents.push(event),
        hashChain: true,
        alertOnCritical: false
      });
    });

    test('returns object with all methods', () => {
      expect(typeof audit.record).toBe('function');
      expect(typeof audit.transaction).toBe('function');
      expect(typeof audit.loginSuccess).toBe('function');
      expect(typeof audit.loginFailure).toBe('function');
      expect(typeof audit.dataAccess).toBe('function');
      expect(typeof audit.dataModify).toBe('function');
      expect(typeof audit.adminAction).toBe('function');
      expect(typeof audit.permissionChange).toBe('function');
      expect(typeof audit.voteCast).toBe('function');
      expect(typeof audit.middleware).toBe('function');
      expect(typeof audit.verifyChain).toBe('function');
    });

    test('exposes constants', () => {
      expect(audit.CATEGORIES).toBe(AUDIT_CATEGORIES);
      expect(audit.RISK).toBe(RISK_LEVELS);
    });
  });

  // ──────────────────────────────────────────
  // record()
  // ──────────────────────────────────────────
  describe('record', () => {
    let audit;
    let capturedEvents;

    beforeEach(() => {
      capturedEvents = [];
      audit = createAuditLogger('test-service', {
        output: 'callback',
        onAuditEvent: (event) => capturedEvents.push(event),
        alertOnCritical: false
      });
    });

    test('creates event with all required fields', () => {
      const event = audit.record({
        category: AUDIT_CATEGORIES.AUTH_LOGIN,
        action: 'login_attempt',
        risk: RISK_LEVELS.LOW,
        req: mockReq()
      });

      expect(event.eventId).toBeDefined();
      expect(event.seq).toBe(1);
      expect(event.timestamp).toBeDefined();
      expect(event.service).toBe('test-service');
      expect(event.category).toBe('AUTH_LOGIN');
      expect(event.action).toBe('login_attempt');
      expect(event.risk).toBe('LOW');
      expect(event.outcome).toBe('SUCCESS');
      expect(event.actor.id).toBe('user-123');
      expect(event.actor.ip).toBe('192.168.1.1');
    });

    test('increments sequence number', () => {
      audit.record({ category: 'TEST', action: 'a' });
      audit.record({ category: 'TEST', action: 'b' });
      const third = audit.record({ category: 'TEST', action: 'c' });

      expect(third.seq).toBe(3);
    });

    test('builds hash chain', () => {
      const first = audit.record({ category: 'TEST', action: 'first' });
      const second = audit.record({ category: 'TEST', action: 'second' });

      expect(first.hash).toBeDefined();
      expect(first.hash).toMatch(/^[0-9a-f]{64}$/);
      expect(second.previousHash).toBe(first.hash);
    });

    test('redacts sensitive fields from details', () => {
      const event = audit.record({
        category: 'TEST',
        action: 'test',
        details: { username: 'admin', password: 'secret123', token: 'abc', publicField: 'ok' }
      });

      expect(event.details.password).toBe('[REDACTED]');
      expect(event.details.token).toBe('[REDACTED]');
      expect(event.details.username).toBe('admin');
      expect(event.details.publicField).toBe('ok');
    });

    test('calls onAuditEvent callback', () => {
      audit.record({ category: 'TEST', action: 'callback_test' });
      expect(capturedEvents).toHaveLength(1);
      expect(capturedEvents[0].action).toBe('callback_test');
    });

    test('writes to stdout when output is stdout', () => {
      const spy = jest.spyOn(process.stdout, 'write').mockImplementation();
      const stdoutAudit = createAuditLogger('stdout-test', { output: 'stdout', alertOnCritical: false });

      stdoutAudit.record({ category: 'TEST', action: 'stdout_test' });

      expect(spy).toHaveBeenCalled();
      expect(spy.mock.calls[0][0]).toContain('[AUDIT]');
      spy.mockRestore();
    });

    test('writes CRITICAL events to stderr when alertOnCritical', () => {
      const spy = jest.spyOn(process.stderr, 'write').mockImplementation();
      const stdoutSpy = jest.spyOn(process.stdout, 'write').mockImplementation();
      const alertAudit = createAuditLogger('alert-test', { output: 'stdout', alertOnCritical: true });

      alertAudit.record({ category: 'TEST', action: 'critical_test', risk: RISK_LEVELS.CRITICAL });

      expect(spy).toHaveBeenCalled();
      expect(spy.mock.calls[0][0]).toContain('[AUDIT-CRITICAL]');
      spy.mockRestore();
      stdoutSpy.mockRestore();
    });
  });

  // ──────────────────────────────────────────
  // Convenience methods
  // ──────────────────────────────────────────
  describe('convenience methods', () => {
    let audit;
    let capturedEvents;

    beforeEach(() => {
      capturedEvents = [];
      audit = createAuditLogger('test-service', {
        output: 'callback',
        onAuditEvent: (event) => capturedEvents.push(event),
        alertOnCritical: false
      });
    });

    test('loginSuccess records AUTH_LOGIN', () => {
      const event = audit.loginSuccess(mockReq(), 'user-456');
      expect(event.category).toBe('AUTH_LOGIN');
      expect(event.action).toBe('login_success');
      expect(event.risk).toBe('LOW');
    });

    test('loginFailure records AUTH_FAILED', () => {
      const event = audit.loginFailure(mockReq(), 'bad password');
      expect(event.category).toBe('AUTH_FAILED');
      expect(event.outcome).toBe('FAILURE');
      expect(event.details.reason).toBe('bad password');
    });

    test('transaction sets risk based on amount', () => {
      const small = audit.transaction(mockReq(), { from: 'a', to: 'b', amount: 50, type: 'transfer' });
      expect(small.risk).toBe('MEDIUM');

      const large = audit.transaction(mockReq(), { from: 'a', to: 'b', amount: 5000, type: 'transfer' });
      expect(large.risk).toBe('HIGH');

      const critical = audit.transaction(mockReq(), { from: 'a', to: 'b', amount: 50000, type: 'transfer' });
      expect(critical.risk).toBe('CRITICAL');
    });

    test('voteCast records VOTE_CAST', () => {
      const event = audit.voteCast(mockReq(), 'election-1', { choice: 'yes', receiptHash: 'abc' });
      expect(event.category).toBe('VOTE_CAST');
      expect(event.resource.id).toBe('election-1');
      expect(event.risk).toBe('HIGH');
    });

    test('adminAction is always CRITICAL', () => {
      const event = audit.adminAction(mockReq(), 'user_banned', { userId: 'u-1' });
      expect(event.category).toBe('ADMIN_ACTION');
      expect(event.risk).toBe('CRITICAL');
    });

    test('dataModify records changed fields', () => {
      const event = audit.dataModify(mockReq(), 'user', 'u-1', { email: 'new@test.com', name: 'New' });
      expect(event.category).toBe('DATA_UPDATE');
      expect(event.details.changedFields).toContain('email');
      expect(event.details.changedFields).toContain('name');
    });

    test('permissionChange is CRITICAL', () => {
      const event = audit.permissionChange(mockReq(), 'user-1', { role: 'admin' });
      expect(event.risk).toBe('CRITICAL');
      expect(event.resource.id).toBe('user-1');
    });
  });

  // ──────────────────────────────────────────
  // verifyChain
  // ──────────────────────────────────────────
  describe('verifyChain', () => {
    test('validates intact chain', () => {
      const events = [];
      const audit = createAuditLogger('chain-test', {
        output: 'callback',
        onAuditEvent: (e) => events.push(e),
        alertOnCritical: false
      });

      audit.record({ category: 'TEST', action: 'a' });
      audit.record({ category: 'TEST', action: 'b' });
      audit.record({ category: 'TEST', action: 'c' });

      const result = audit.verifyChain(events);
      expect(result.valid).toBe(true);
      expect(result.count).toBe(3);
    });

    test('detects tampered event', () => {
      const events = [];
      const audit = createAuditLogger('chain-test', {
        output: 'callback',
        onAuditEvent: (e) => events.push(e),
        alertOnCritical: false
      });

      audit.record({ category: 'TEST', action: 'a' });
      audit.record({ category: 'TEST', action: 'b' });

      // Tamper with event hash
      events[0].hash = 'tampered_hash_value_that_is_invalid';

      const result = audit.verifyChain(events);
      expect(result.valid).toBe(false);
    });

    test('detects broken previousHash link', () => {
      const events = [];
      const audit = createAuditLogger('chain-test', {
        output: 'callback',
        onAuditEvent: (e) => events.push(JSON.parse(JSON.stringify(e))),
        alertOnCritical: false
      });

      audit.record({ category: 'TEST', action: 'a' });
      audit.record({ category: 'TEST', action: 'b' });

      // Break the chain
      events[1].previousHash = 'wrong_hash';

      const result = audit.verifyChain(events);
      expect(result.valid).toBe(false);
      expect(result.brokenAt).toBe(1);
    });

    test('validates empty chain', () => {
      const audit = createAuditLogger('empty-test', {
        output: 'callback',
        alertOnCritical: false
      });

      const result = audit.verifyChain([]);
      expect(result.valid).toBe(true);
      expect(result.count).toBe(0);
    });
  });

  // ──────────────────────────────────────────
  // middleware
  // ──────────────────────────────────────────
  describe('middleware', () => {
    test('returns Express middleware function', () => {
      const audit = createAuditLogger('mw-test', { output: 'callback', alertOnCritical: false });
      const mw = audit.middleware();
      expect(typeof mw).toBe('function');
      expect(mw.length).toBe(3);
    });

    test('skips paths not matching filter', () => {
      const events = [];
      const audit = createAuditLogger('mw-test', {
        output: 'callback',
        onAuditEvent: (e) => events.push(e),
        alertOnCritical: false
      });

      const mw = audit.middleware({ pathFilter: (p) => p.startsWith('/v1/') });
      const req = mockReq({ path: '/health' });
      const res = { on: jest.fn() };
      const next = jest.fn();

      mw(req, res, next);
      expect(next).toHaveBeenCalled();
      expect(res.on).not.toHaveBeenCalled();
    });

    test('audits write operations on matching paths', () => {
      const events = [];
      const audit = createAuditLogger('mw-test', {
        output: 'callback',
        onAuditEvent: (e) => events.push(e),
        alertOnCritical: false
      });

      const mw = audit.middleware({ pathFilter: (p) => p.startsWith('/v1/') });
      const req = mockReq({ method: 'POST', path: '/v1/elections' });
      let finishCallback;
      const res = {
        statusCode: 201,
        on: jest.fn((event, cb) => { if (event === 'finish') finishCallback = cb; })
      };
      const next = jest.fn();

      mw(req, res, next);
      expect(next).toHaveBeenCalled();

      // Simulate response finish
      finishCallback();

      expect(events).toHaveLength(1);
      expect(events[0].action).toBe('api_create');
      expect(events[0].outcome).toBe('SUCCESS');
    });
  });
});
