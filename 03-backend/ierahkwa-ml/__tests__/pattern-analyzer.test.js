'use strict';

const { PatternAnalyzer } = require('../lib/pattern-analyzer');

describe('PatternAnalyzer', () => {
  let analyzer;

  beforeEach(() => {
    analyzer = new PatternAnalyzer({
      maxUsers: 100,
      historyDepth: 50,
      similarityThreshold: 0.6
    });
  });

  describe('extractFeatures', () => {
    test('extracts features from raw action', () => {
      const features = analyzer.extractFeatures({
        type: 'page_view',
        platform: 'nexus-tesoro',
        severity: 'info',
        timestamp: '2026-03-01T14:30:00Z',
        metadata: { page: '/dashboard' }
      });

      // getHours() returns local time, so we check against local hour
      const expectedHour = new Date('2026-03-01T14:30:00Z').getHours();
      expect(features.hourOfDay).toBe(expectedHour);
      expect(features.actionType).toBe('page_view');
      expect(features.platform).toBe('nexus-tesoro');
      expect(features.severity).toBe(0); // info = 0
      expect(features.hasMetadata).toBe(1);
      expect(features.payloadSize).toBeGreaterThan(0);
    });

    test('handles missing fields gracefully', () => {
      const features = analyzer.extractFeatures({});
      expect(features.actionType).toBe('unknown');
      expect(features.platform).toBe('unknown');
      expect(features.severity).toBe(0);
      expect(features.hasMetadata).toBe(0);
    });
  });

  describe('analyze', () => {
    test('first analysis returns new_user deviation', () => {
      const result = analyzer.analyze('new-user', {
        type: 'login',
        platform: 'admin',
        timestamp: new Date().toISOString()
      });

      expect(result.userId).toBe('new-user');
      expect(result.actionsRecorded).toBe(1);
      expect(result.hasFingerprint).toBe(false);
      expect(result.deviations).toContain('new_user');
    });

    test('builds fingerprint after 20 actions', () => {
      for (let i = 0; i < 25; i++) {
        analyzer.analyze('active-user', {
          type: 'page_view',
          platform: 'nexus-tesoro',
          timestamp: new Date().toISOString()
        });
      }

      const fp = analyzer.getFingerprint('active-user');
      expect(fp).not.toBeNull();
      expect(fp.sampleSize).toBe(20); // computed at 20
      expect(fp.actionDistribution).toHaveProperty('page_view');
    });

    test('consistent behavior has high similarity', () => {
      // Build profile with consistent behavior
      for (let i = 0; i < 25; i++) {
        analyzer.analyze('consistent-user', {
          type: 'page_view',
          platform: 'nexus-tesoro',
          severity: 'info',
          timestamp: new Date().toISOString()
        });
      }

      const result = analyzer.analyze('consistent-user', {
        type: 'page_view',
        platform: 'nexus-tesoro',
        severity: 'info',
        timestamp: new Date().toISOString()
      });

      expect(result.isDeviation).toBe(false);
      expect(result.similarity).toBeGreaterThan(0.3);
    });
  });

  describe('computeFingerprint', () => {
    test('returns null for empty actions', () => {
      const fp = analyzer.computeFingerprint([]);
      expect(fp).toBeNull();
    });

    test('computes valid fingerprint with distributions', () => {
      const actions = [];
      for (let i = 0; i < 30; i++) {
        actions.push(analyzer.extractFeatures({
          type: i % 3 === 0 ? 'transaction' : 'page_view',
          platform: 'nexus-tesoro',
          severity: 'info',
          timestamp: new Date().toISOString()
        }));
      }

      const fp = analyzer.computeFingerprint(actions);
      expect(fp.hourDistribution).toHaveLength(24);
      expect(fp.dayDistribution).toHaveLength(7);
      expect(fp.sampleSize).toBe(30);
      expect(fp.transactionRatio).toBeGreaterThan(0);
      expect(fp.transactionRatio).toBeLessThan(1);
    });
  });

  describe('_cosineSimilarity', () => {
    test('identical vectors have similarity 1', () => {
      const sim = analyzer._cosineSimilarity([1, 2, 3], [1, 2, 3]);
      expect(sim).toBeCloseTo(1.0, 5);
    });

    test('orthogonal vectors have similarity 0', () => {
      const sim = analyzer._cosineSimilarity([1, 0, 0], [0, 1, 0]);
      expect(sim).toBeCloseTo(0, 5);
    });

    test('empty vectors return 0', () => {
      expect(analyzer._cosineSimilarity([], [])).toBe(0);
    });
  });

  describe('stats', () => {
    test('returns correct tracking counts', () => {
      analyzer.analyze('u1', { type: 'a' });
      analyzer.analyze('u2', { type: 'b' });

      const stats = analyzer.getStats();
      expect(stats.usersTracked).toBe(2);
      expect(stats.usersWithFingerprint).toBe(0); // not enough actions
    });

    test('reset clears everything', () => {
      analyzer.analyze('u1', { type: 'a' });
      analyzer.reset();
      expect(analyzer.getStats().usersTracked).toBe(0);
    });
  });
});
