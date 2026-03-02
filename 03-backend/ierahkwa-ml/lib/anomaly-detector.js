'use strict';

// ============================================================================
// ANOMALY DETECTOR — Real Statistical Anomaly Detection Engine
// Methods: Z-Score, Exponential Moving Average (EMA), Interquartile Range (IQR)
// Pure math — zero external ML dependencies
// ============================================================================

class AnomalyDetector {
  /**
   * @param {Object} opts
   * @param {number} [opts.windowSize=100]    - Rolling window for stats
   * @param {number} [opts.zThreshold=2.5]    - Z-score threshold for anomaly
   * @param {number} [opts.emaAlpha=0.1]      - EMA smoothing factor (0-1)
   * @param {number} [opts.iqrMultiplier=1.5] - IQR fence multiplier
   */
  constructor(opts = {}) {
    this.windowSize = opts.windowSize || 100;
    this.zThreshold = opts.zThreshold || 2.5;
    this.emaAlpha = opts.emaAlpha || 0.1;
    this.iqrMultiplier = opts.iqrMultiplier || 1.5;

    // Per-metric rolling windows: Map<metricKey, number[]>
    this.windows = new Map();
    // EMA state per metric: Map<metricKey, number>
    this.emaState = new Map();
    // Detection history for audit
    this.detectionLog = [];
    this.maxLogSize = 1000;
  }

  // ─── Core Math ──────────────────────────────────────────────

  /** Arithmetic mean */
  _mean(arr) {
    if (arr.length === 0) return 0;
    return arr.reduce((s, v) => s + v, 0) / arr.length;
  }

  /** Population standard deviation */
  _stddev(arr) {
    if (arr.length < 2) return 0;
    const m = this._mean(arr);
    const variance = arr.reduce((s, v) => s + (v - m) ** 2, 0) / arr.length;
    return Math.sqrt(variance);
  }

  /** Median of sorted array */
  _median(sorted) {
    const n = sorted.length;
    if (n === 0) return 0;
    const mid = Math.floor(n / 2);
    return n % 2 === 0 ? (sorted[mid - 1] + sorted[mid]) / 2 : sorted[mid];
  }

  /** Interquartile range */
  _iqr(arr) {
    const sorted = [...arr].sort((a, b) => a - b);
    const mid = Math.floor(sorted.length / 2);
    const q1 = this._median(sorted.slice(0, mid));
    const q3 = this._median(sorted.slice(mid + (sorted.length % 2 === 0 ? 0 : 1)));
    return { q1, q3, iqr: q3 - q1 };
  }

  // ─── Window Management ──────────────────────────────────────

  _getWindow(metricKey) {
    if (!this.windows.has(metricKey)) {
      this.windows.set(metricKey, []);
    }
    return this.windows.get(metricKey);
  }

  _pushValue(metricKey, value) {
    const window = this._getWindow(metricKey);
    window.push(value);
    if (window.length > this.windowSize) {
      window.shift();
    }
  }

  // ─── Detection Methods ──────────────────────────────────────

  /**
   * Z-Score anomaly detection
   * @param {string} metricKey - Unique identifier for this metric stream
   * @param {number} value - New observed value
   * @returns {{ isAnomaly: boolean, zScore: number, mean: number, stddev: number }}
   */
  detectZScore(metricKey, value) {
    const window = this._getWindow(metricKey);

    if (window.length < 5) {
      this._pushValue(metricKey, value);
      return { isAnomaly: false, zScore: 0, mean: value, stddev: 0, method: 'z-score', insufficient: true };
    }

    const mean = this._mean(window);
    const stddev = this._stddev(window);

    let zScore = 0;
    if (stddev > 0) {
      zScore = Math.abs((value - mean) / stddev);
    }

    const isAnomaly = zScore > this.zThreshold;

    this._pushValue(metricKey, value);

    return { isAnomaly, zScore: Math.round(zScore * 100) / 100, mean: Math.round(mean * 100) / 100, stddev: Math.round(stddev * 100) / 100, method: 'z-score' };
  }

  /**
   * EMA (Exponential Moving Average) anomaly detection
   * Detects when a value deviates significantly from the smoothed trend
   * @param {string} metricKey
   * @param {number} value
   * @returns {{ isAnomaly: boolean, ema: number, deviation: number, deviationPct: number }}
   */
  detectEMA(metricKey, value) {
    const emaKey = `ema:${metricKey}`;

    if (!this.emaState.has(emaKey)) {
      this.emaState.set(emaKey, value);
      return { isAnomaly: false, ema: value, deviation: 0, deviationPct: 0, method: 'ema', insufficient: true };
    }

    const prevEma = this.emaState.get(emaKey);
    const newEma = this.emaAlpha * value + (1 - this.emaAlpha) * prevEma;

    const deviation = Math.abs(value - prevEma);
    const deviationPct = prevEma !== 0 ? (deviation / Math.abs(prevEma)) * 100 : 0;

    // Anomaly if deviation > 50% of EMA (configurable)
    const isAnomaly = deviationPct > 50;

    this.emaState.set(emaKey, newEma);

    return {
      isAnomaly,
      ema: Math.round(newEma * 100) / 100,
      deviation: Math.round(deviation * 100) / 100,
      deviationPct: Math.round(deviationPct * 100) / 100,
      method: 'ema'
    };
  }

  /**
   * IQR (Interquartile Range) anomaly detection
   * Outlier if value is below Q1 - 1.5*IQR or above Q3 + 1.5*IQR
   * @param {string} metricKey
   * @param {number} value
   * @returns {{ isAnomaly: boolean, q1: number, q3: number, iqr: number, lowerFence: number, upperFence: number }}
   */
  detectIQR(metricKey, value) {
    const window = this._getWindow(metricKey);

    if (window.length < 10) {
      this._pushValue(metricKey, value);
      return { isAnomaly: false, q1: 0, q3: 0, iqr: 0, lowerFence: 0, upperFence: 0, method: 'iqr', insufficient: true };
    }

    const { q1, q3, iqr } = this._iqr(window);
    const lowerFence = q1 - this.iqrMultiplier * iqr;
    const upperFence = q3 + this.iqrMultiplier * iqr;

    const isAnomaly = value < lowerFence || value > upperFence;

    this._pushValue(metricKey, value);

    return {
      isAnomaly,
      q1: Math.round(q1 * 100) / 100,
      q3: Math.round(q3 * 100) / 100,
      iqr: Math.round(iqr * 100) / 100,
      lowerFence: Math.round(lowerFence * 100) / 100,
      upperFence: Math.round(upperFence * 100) / 100,
      method: 'iqr'
    };
  }

  /**
   * Multi-method ensemble anomaly detection
   * Combines Z-Score, EMA, and IQR — anomaly if >= 2 methods agree
   * @param {string} metricKey
   * @param {number} value
   * @returns {Object} Full analysis with ensemble verdict
   */
  detect(metricKey, value) {
    const zResult = this.detectZScore(metricKey, value);
    const emaResult = this.detectEMA(metricKey, value);
    const iqrResult = this.detectIQR(metricKey, value);

    const votes = [zResult.isAnomaly, emaResult.isAnomaly, iqrResult.isAnomaly];
    const anomalyVotes = votes.filter(Boolean).length;
    const isAnomaly = anomalyVotes >= 2;

    const severity = anomalyVotes === 3 ? 'critical'
      : anomalyVotes === 2 ? 'high'
      : anomalyVotes === 1 ? 'medium'
      : 'normal';

    const result = {
      metricKey,
      value,
      isAnomaly,
      severity,
      confidence: Math.round((anomalyVotes / 3) * 100),
      methods: { zScore: zResult, ema: emaResult, iqr: iqrResult },
      timestamp: new Date().toISOString()
    };

    if (isAnomaly) {
      this._logDetection(result);
    }

    return result;
  }

  // ─── Event-based Anomaly Detection ──────────────────────────

  /**
   * Analyze a security event from the frontend agents
   * Extracts numeric features and runs ensemble detection
   * @param {Object} event - Event from ierahkwa-agents.js
   * @returns {Object} Analysis result
   */
  analyzeEvent(event) {
    const results = [];
    const source = event.source || event.agentId || 'unknown';

    // Feature: event frequency (events per minute from this source)
    if (event.timestamp) {
      const freqKey = `freq:${source}`;
      const now = Date.now();
      const window = this._getWindow(freqKey);

      // Count events in the last 60 seconds
      const recentCount = window.filter(t => (now - t) < 60000).length + 1;
      this._pushValue(freqKey, now);

      const freqResult = this.detect(`freq_rate:${source}`, recentCount);
      if (freqResult.isAnomaly) {
        results.push({ feature: 'event_frequency', ...freqResult });
      }
    }

    // Feature: severity escalation
    if (event.severity) {
      const sevMap = { info: 0, low: 1, medium: 2, high: 3, critical: 4 };
      const sevValue = sevMap[event.severity] || 0;
      const sevResult = this.detect(`severity:${source}`, sevValue);
      if (sevResult.isAnomaly) {
        results.push({ feature: 'severity_escalation', ...sevResult });
      }
    }

    // Feature: risk score from trust engine
    if (typeof event.riskScore === 'number') {
      const riskResult = this.detect(`risk:${source}`, event.riskScore);
      if (riskResult.isAnomaly) {
        results.push({ feature: 'risk_score', ...riskResult });
      }
    }

    // Feature: payload size (if metadata present)
    if (event.metadata) {
      const payloadSize = JSON.stringify(event.metadata).length;
      const sizeResult = this.detect(`payload:${source}`, payloadSize);
      if (sizeResult.isAnomaly) {
        results.push({ feature: 'payload_size', ...sizeResult });
      }
    }

    return {
      eventId: event.id,
      source,
      anomaliesDetected: results.length,
      isAnomalous: results.length > 0,
      anomalies: results,
      analyzedAt: new Date().toISOString()
    };
  }

  // ─── Logging & Stats ────────────────────────────────────────

  _logDetection(result) {
    this.detectionLog.push(result);
    if (this.detectionLog.length > this.maxLogSize) {
      this.detectionLog.shift();
    }
  }

  getDetectionLog(limit = 50) {
    return this.detectionLog.slice(-limit);
  }

  getStats() {
    return {
      metricsTracked: this.windows.size,
      emaStreams: this.emaState.size,
      detections: this.detectionLog.length,
      config: {
        windowSize: this.windowSize,
        zThreshold: this.zThreshold,
        emaAlpha: this.emaAlpha,
        iqrMultiplier: this.iqrMultiplier
      }
    };
  }

  /** Reset all state */
  reset() {
    this.windows.clear();
    this.emaState.clear();
    this.detectionLog = [];
  }
}

module.exports = { AnomalyDetector };
