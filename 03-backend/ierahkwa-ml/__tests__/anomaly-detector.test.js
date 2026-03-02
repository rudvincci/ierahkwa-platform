'use strict';

const { AnomalyDetector } = require('../lib/anomaly-detector');

describe('AnomalyDetector', () => {
  let detector;

  beforeEach(() => {
    detector = new AnomalyDetector({
      windowSize: 20,
      zThreshold: 2.0,
      emaAlpha: 0.1,
      iqrMultiplier: 1.5
    });
  });

  // ── Math Helpers ──────────────────────────────────────────

  describe('internal math', () => {
    test('_mean computes correctly', () => {
      expect(detector._mean([1, 2, 3, 4, 5])).toBe(3);
      expect(detector._mean([])).toBe(0);
      expect(detector._mean([10])).toBe(10);
    });

    test('_stddev computes population std deviation', () => {
      const std = detector._stddev([2, 4, 4, 4, 5, 5, 7, 9]);
      expect(std).toBeCloseTo(2.0, 1);
    });

    test('_stddev returns 0 for single value', () => {
      expect(detector._stddev([5])).toBe(0);
    });

    test('_median computes correctly for odd and even lengths', () => {
      expect(detector._median([1, 3, 5])).toBe(3);
      expect(detector._median([1, 3, 5, 7])).toBe(4);
      expect(detector._median([])).toBe(0);
    });

    test('_iqr returns correct quartiles', () => {
      const result = detector._iqr([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
      expect(result.q1).toBeLessThan(result.q3);
      expect(result.iqr).toBeGreaterThan(0);
    });
  });

  // ── Z-Score Detection ─────────────────────────────────────

  describe('detectZScore', () => {
    test('returns insufficient for first few values', () => {
      const result = detector.detectZScore('test', 10);
      expect(result.insufficient).toBe(true);
      expect(result.isAnomaly).toBe(false);
    });

    test('normal values produce no anomaly', () => {
      // Seed window with normal data
      for (let i = 0; i < 10; i++) {
        detector.detectZScore('normal', 100 + Math.random() * 5);
      }
      const result = detector.detectZScore('normal', 102);
      expect(result.isAnomaly).toBe(false);
      expect(result.zScore).toBeLessThan(2.0);
    });

    test('extreme outlier is detected as anomaly', () => {
      // Seed with stable values
      for (let i = 0; i < 15; i++) {
        detector.detectZScore('stable', 50);
      }
      const result = detector.detectZScore('stable', 500);
      expect(result.isAnomaly).toBe(true);
      expect(result.zScore).toBeGreaterThan(2.0);
    });
  });

  // ── EMA Detection ─────────────────────────────────────────

  describe('detectEMA', () => {
    test('first value returns insufficient', () => {
      const result = detector.detectEMA('ema-test', 100);
      expect(result.insufficient).toBe(true);
      expect(result.isAnomaly).toBe(false);
    });

    test('gradual change is not anomalous', () => {
      detector.detectEMA('gradual', 100);
      const result = detector.detectEMA('gradual', 105);
      expect(result.isAnomaly).toBe(false);
    });

    test('sudden spike is anomalous', () => {
      // Build up EMA
      for (let i = 0; i < 20; i++) {
        detector.detectEMA('spike', 50);
      }
      const result = detector.detectEMA('spike', 200);
      expect(result.isAnomaly).toBe(true);
      expect(result.deviationPct).toBeGreaterThan(50);
    });
  });

  // ── IQR Detection ─────────────────────────────────────────

  describe('detectIQR', () => {
    test('returns insufficient for small windows', () => {
      const result = detector.detectIQR('iqr-test', 10);
      expect(result.insufficient).toBe(true);
    });

    test('value within fences is not anomalous', () => {
      for (let i = 0; i < 15; i++) {
        detector.detectIQR('iqr-normal', 50 + i);
      }
      const result = detector.detectIQR('iqr-normal', 55);
      expect(result.isAnomaly).toBe(false);
    });

    test('value outside fences is anomalous', () => {
      for (let i = 0; i < 15; i++) {
        detector.detectIQR('iqr-outlier', 50);
      }
      const result = detector.detectIQR('iqr-outlier', 500);
      expect(result.isAnomaly).toBe(true);
    });
  });

  // ── Ensemble Detection ────────────────────────────────────

  describe('detect (ensemble)', () => {
    test('returns full analysis object', () => {
      const result = detector.detect('ensemble', 100);
      expect(result).toHaveProperty('metricKey', 'ensemble');
      expect(result).toHaveProperty('value', 100);
      expect(result).toHaveProperty('isAnomaly');
      expect(result).toHaveProperty('severity');
      expect(result).toHaveProperty('confidence');
      expect(result).toHaveProperty('methods');
      expect(result).toHaveProperty('timestamp');
    });

    test('normal value has severity "normal"', () => {
      const result = detector.detect('norm', 50);
      expect(result.severity).toBe('normal');
    });

    test('extreme outlier has high confidence', () => {
      for (let i = 0; i < 20; i++) {
        detector.detect('extreme', 50);
      }
      const result = detector.detect('extreme', 99999);
      expect(result.isAnomaly).toBe(true);
      expect(result.confidence).toBeGreaterThanOrEqual(67);
    });
  });

  // ── Event Analysis ────────────────────────────────────────

  describe('analyzeEvent', () => {
    test('analyzes security event and returns result', () => {
      const event = {
        id: 'evt-1',
        source: 'guardian-agent',
        severity: 'high',
        timestamp: new Date().toISOString(),
        riskScore: 75,
        metadata: { action: 'blocked_request' }
      };

      const result = detector.analyzeEvent(event);
      expect(result).toHaveProperty('eventId', 'evt-1');
      expect(result).toHaveProperty('source', 'guardian-agent');
      expect(result).toHaveProperty('anomaliesDetected');
      expect(result).toHaveProperty('isAnomalous');
      expect(result).toHaveProperty('analyzedAt');
    });

    test('repeated normal events do not trigger anomaly', () => {
      for (let i = 0; i < 30; i++) {
        detector.analyzeEvent({
          source: 'agent-x',
          severity: 'info',
          timestamp: new Date().toISOString(),
          riskScore: 20
        });
      }
      const result = detector.analyzeEvent({
        source: 'agent-x',
        severity: 'info',
        timestamp: new Date().toISOString(),
        riskScore: 22
      });
      expect(result.isAnomalous).toBe(false);
    });
  });

  // ── Stats & Log ───────────────────────────────────────────

  describe('stats and log', () => {
    test('getStats returns configuration', () => {
      const stats = detector.getStats();
      expect(stats.config.windowSize).toBe(20);
      expect(stats.config.zThreshold).toBe(2.0);
    });

    test('getDetectionLog returns recent detections', () => {
      const log = detector.getDetectionLog();
      expect(Array.isArray(log)).toBe(true);
    });

    test('reset clears all state', () => {
      detector.detect('test', 100);
      detector.reset();
      expect(detector.getStats().metricsTracked).toBe(0);
    });
  });
});
