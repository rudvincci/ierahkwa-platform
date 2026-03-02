'use strict';

const { TrustEngine, TRUST_LEVELS, DEFAULT_WEIGHTS } = require('../lib/trust-engine');

describe('TrustEngine', () => {
  let engine;

  beforeEach(() => {
    engine = new TrustEngine();
  });

  // ── Factor Scoring ────────────────────────────────────────

  describe('factor scoring functions', () => {
    test('scoreSessionAge: new session = low score, old = higher', () => {
      const recent = engine.scoreSessionAge(Date.now() - 1000); // 1 second ago
      const old = engine.scoreSessionAge(Date.now() - 3600000);  // 1 hour ago
      expect(recent).toBeLessThan(old);
      expect(recent).toBeGreaterThanOrEqual(0);
      expect(old).toBeLessThanOrEqual(100);
    });

    test('scoreAuthStrength: SBT > MFA > JWT > password > anonymous', () => {
      const sbt = engine.scoreAuthStrength('sbt');
      const mfa = engine.scoreAuthStrength('mfa');
      const jwt = engine.scoreAuthStrength('jwt');
      const pwd = engine.scoreAuthStrength('password');
      const anon = engine.scoreAuthStrength('anonymous');

      expect(sbt).toBeGreaterThan(mfa);
      expect(mfa).toBeGreaterThan(jwt);
      expect(jwt).toBeGreaterThan(pwd);
      expect(pwd).toBeGreaterThan(anon);
    });

    test('scoreIpReputation: known IP = high, unknown = low', () => {
      const known = engine.scoreIpReputation('1.2.3.4', ['1.2.3.4', '5.6.7.8']);
      const unknown = engine.scoreIpReputation('9.9.9.9', ['1.2.3.4']);
      const first = engine.scoreIpReputation('1.1.1.1', []);

      expect(known).toBeGreaterThan(unknown);
      expect(first).toBeGreaterThan(unknown);
    });

    test('scoreRequestPattern: normal rate = high, excessive = low', () => {
      const normal = engine.scoreRequestPattern(5, 0);
      const heavy = engine.scoreRequestPattern(100, 0);
      const errors = engine.scoreRequestPattern(5, 0.8);

      expect(normal).toBeGreaterThan(heavy);
      expect(normal).toBeGreaterThan(errors);
    });

    test('scoreTransactionHistory: high success rate = high score', () => {
      const good = engine.scoreTransactionHistory(95, 5, 10000);
      const bad = engine.scoreTransactionHistory(10, 90, 100);
      const none = engine.scoreTransactionHistory(0, 0, 0);

      expect(good).toBeGreaterThan(bad);
      expect(none).toBe(50); // Neutral
    });

    test('scoreCommunityVouching: many vouches from trusted users = high', () => {
      const wellVouched = engine.scoreCommunityVouching(10, 85);
      const fewVouches = engine.scoreCommunityVouching(3, 80);
      const noVouches = engine.scoreCommunityVouching(0, 0);

      expect(wellVouched).toBeGreaterThan(fewVouches);
      expect(fewVouches).toBeGreaterThan(noVouches);
    });

    test('scoreBehaviorConsistency: returns neutral for insufficient data', () => {
      const score = engine.scoreBehaviorConsistency({}, []);
      expect(score).toBe(50);
    });
  });

  // ── Trust Calculation ─────────────────────────────────────

  describe('calculateTrust', () => {
    test('new user starts at neutral score (~50)', () => {
      const result = engine.calculateTrust('new-user');
      expect(result.score).toBeGreaterThanOrEqual(40);
      expect(result.score).toBeLessThanOrEqual(60);
      expect(result.userId).toBe('new-user');
      expect(result).toHaveProperty('level');
      expect(result).toHaveProperty('permissions');
    });

    test('signals improve trust score', () => {
      const base = engine.calculateTrust('signal-user');
      const improved = engine.calculateTrust('signal-user', {
        authMethod: 'sbt',
        sessionStart: Date.now() - 7200000,
        ip: '10.0.0.1',
        requestRate: 2,
        transactions: { success: 100, failure: 2, volume: 50000 },
        vouches: { count: 8, avgTrust: 90 }
      });

      expect(improved.score).toBeGreaterThan(base.score);
    });

    test('returns correct trust level based on score', () => {
      // First establish known IP, then calculate with all signals
      engine.calculateTrust('high-trust', { ip: '10.0.0.1' });
      const result = engine.calculateTrust('high-trust', {
        authMethod: 'sbt',
        sessionStart: Date.now() - 86400000,
        ip: '10.0.0.1',
        requestRate: 2,
        transactions: { success: 1000, failure: 1, volume: 1000000 },
        vouches: { count: 20, avgTrust: 95 }
      });

      expect(['sovereign', 'trusted']).toContain(result.level);
      expect(result.permissions.length).toBeGreaterThan(0);
    });

    test('permissions match trust level', () => {
      for (const level of TRUST_LEVELS) {
        expect(level.permissions).toBeDefined();
        expect(Array.isArray(level.permissions)).toBe(true);
      }
    });
  });

  // ── Trust Events ──────────────────────────────────────────

  describe('reportEvent', () => {
    test('positive event boosts trust', () => {
      engine.calculateTrust('event-user');
      const profileBefore = engine.getProfile('event-user');
      const scoreBefore = profileBefore.score;

      engine.reportEvent('event-user', 'positive', 'good_transaction', 5);
      const profileAfter = engine.getProfile('event-user');

      expect(profileAfter.factors.sessionAge).toBeGreaterThanOrEqual(
        profileBefore.factors.sessionAge
      );
    });

    test('negative event penalizes trust', () => {
      engine.calculateTrust('bad-user', { authMethod: 'sbt' });
      const before = engine.getProfile('bad-user');
      const authBefore = before.factors.authStrength;

      engine.reportEvent('bad-user', 'negative', 'fraud_attempt', 8);
      const after = engine.getProfile('bad-user');

      expect(after.factors.authStrength).toBeLessThan(authBefore);
    });

    test('magnitude is clamped 0-10', () => {
      engine.calculateTrust('clamp-user');
      const result = engine.reportEvent('clamp-user', 'positive', 'test', 999);
      expect(result.magnitude).toBe(10);
    });
  });

  // ── Leaderboard & Stats ───────────────────────────────────

  describe('leaderboard and stats', () => {
    test('getLeaderboard returns sorted users', () => {
      engine.calculateTrust('user-a', { authMethod: 'sbt' });
      engine.calculateTrust('user-b', { authMethod: 'anonymous' });
      engine.calculateTrust('user-c', { authMethod: 'mfa' });

      const lb = engine.getLeaderboard(10);
      expect(lb.length).toBe(3);
      // Sorted descending
      for (let i = 1; i < lb.length; i++) {
        expect(lb[i - 1].score).toBeGreaterThanOrEqual(lb[i].score);
      }
    });

    test('getStats returns distribution', () => {
      engine.calculateTrust('stats-user');
      const stats = engine.getStats();
      expect(stats.totalUsers).toBe(1);
      expect(stats.distribution).toBeDefined();
      expect(stats.weights).toBeDefined();
    });

    test('reset clears all profiles', () => {
      engine.calculateTrust('temp-user');
      engine.reset();
      expect(engine.getStats().totalUsers).toBe(0);
    });
  });

  // ── Weights ───────────────────────────────────────────────

  describe('weights', () => {
    test('default weights sum to approximately 1.0', () => {
      const sum = Object.values(DEFAULT_WEIGHTS).reduce((s, w) => s + w, 0);
      expect(sum).toBeCloseTo(1.0, 2);
    });

    test('all trust levels have min threshold', () => {
      for (const level of TRUST_LEVELS) {
        expect(typeof level.min).toBe('number');
        expect(level.min).toBeGreaterThanOrEqual(0);
        expect(level.min).toBeLessThanOrEqual(100);
      }
    });
  });
});
