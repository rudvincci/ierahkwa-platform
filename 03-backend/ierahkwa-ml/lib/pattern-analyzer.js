'use strict';

// ============================================================================
// PATTERN ANALYZER — Behavioral Pattern Learning & Matching
// Server-side complement to PatternAgent from ierahkwa-agents.js
// Learns user behavioral fingerprints and detects deviations
// ============================================================================

class PatternAnalyzer {
  /**
   * @param {Object} [opts]
   * @param {number} [opts.maxUsers=5000] - Max user profiles to track
   * @param {number} [opts.historyDepth=200] - Max actions per user
   * @param {number} [opts.similarityThreshold=0.6] - Min similarity to be "normal"
   */
  constructor(opts = {}) {
    this.maxUsers = opts.maxUsers || 5000;
    this.historyDepth = opts.historyDepth || 200;
    this.similarityThreshold = opts.similarityThreshold || 0.6;

    // Per-user behavioral profiles
    this.profiles = new Map();
  }

  // ─── Feature Extraction ─────────────────────────────────────

  /**
   * Extract behavioral features from a raw action
   * @param {Object} action - Raw action from frontend agent
   * @returns {Object} Normalized feature vector
   */
  extractFeatures(action) {
    const ts = action.timestamp ? new Date(action.timestamp) : new Date();

    return {
      hourOfDay: ts.getHours(),
      dayOfWeek: ts.getDay(),
      actionType: action.type || action.action || 'unknown',
      platform: action.platform || action.source || 'unknown',
      duration: action.duration || 0,
      hasMetadata: action.metadata ? 1 : 0,
      payloadSize: action.metadata ? JSON.stringify(action.metadata).length : 0,
      severity: this._severityToNum(action.severity),
      isTransaction: action.type === 'transaction' ? 1 : 0,
      timestamp: ts.getTime()
    };
  }

  _severityToNum(sev) {
    const map = { info: 0, low: 1, medium: 2, high: 3, critical: 4 };
    return map[sev] || 0;
  }

  // ─── Profile Management ─────────────────────────────────────

  _getProfile(userId) {
    if (!this.profiles.has(userId)) {
      this.profiles.set(userId, {
        userId,
        actions: [],
        fingerprint: null, // Computed behavioral fingerprint
        lastUpdated: Date.now()
      });

      if (this.profiles.size > this.maxUsers) {
        const oldest = [...this.profiles.entries()]
          .sort((a, b) => a[1].lastUpdated - b[1].lastUpdated)[0];
        if (oldest) this.profiles.delete(oldest[0]);
      }
    }
    return this.profiles.get(userId);
  }

  // ─── Fingerprint Computation ────────────────────────────────

  /**
   * Compute a behavioral fingerprint from action history
   * Returns distribution vectors for key features
   * @param {Object[]} actions - Array of extracted features
   * @returns {Object} Behavioral fingerprint
   */
  computeFingerprint(actions) {
    if (actions.length === 0) return null;

    // Hour distribution (24 bins)
    const hourDist = new Array(24).fill(0);
    for (const a of actions) hourDist[a.hourOfDay]++;
    const hourTotal = actions.length;
    const hourNorm = hourDist.map(c => c / hourTotal);

    // Day-of-week distribution (7 bins)
    const dayDist = new Array(7).fill(0);
    for (const a of actions) dayDist[a.dayOfWeek]++;
    const dayNorm = dayDist.map(c => c / hourTotal);

    // Action type distribution
    const actionCounts = {};
    for (const a of actions) {
      actionCounts[a.actionType] = (actionCounts[a.actionType] || 0) + 1;
    }
    const actionDist = {};
    for (const [k, v] of Object.entries(actionCounts)) {
      actionDist[k] = v / hourTotal;
    }

    // Platform distribution
    const platformCounts = {};
    for (const a of actions) {
      platformCounts[a.platform] = (platformCounts[a.platform] || 0) + 1;
    }
    const platformDist = {};
    for (const [k, v] of Object.entries(platformCounts)) {
      platformDist[k] = v / hourTotal;
    }

    // Avg session metrics
    const avgDuration = actions.reduce((s, a) => s + a.duration, 0) / actions.length;
    const avgPayload = actions.reduce((s, a) => s + a.payloadSize, 0) / actions.length;
    const avgSeverity = actions.reduce((s, a) => s + a.severity, 0) / actions.length;
    const txRatio = actions.filter(a => a.isTransaction).length / actions.length;

    return {
      hourDistribution: hourNorm,
      dayDistribution: dayNorm,
      actionDistribution: actionDist,
      platformDistribution: platformDist,
      avgDuration: Math.round(avgDuration),
      avgPayloadSize: Math.round(avgPayload),
      avgSeverity: Math.round(avgSeverity * 100) / 100,
      transactionRatio: Math.round(txRatio * 100) / 100,
      sampleSize: actions.length,
      computedAt: new Date().toISOString()
    };
  }

  // ─── Similarity Computation ─────────────────────────────────

  /**
   * Cosine similarity between two distribution vectors
   * @param {number[]} a
   * @param {number[]} b
   * @returns {number} 0-1 similarity
   */
  _cosineSimilarity(a, b) {
    if (a.length !== b.length || a.length === 0) return 0;

    let dotProduct = 0;
    let normA = 0;
    let normB = 0;

    for (let i = 0; i < a.length; i++) {
      dotProduct += a[i] * b[i];
      normA += a[i] ** 2;
      normB += b[i] ** 2;
    }

    const denom = Math.sqrt(normA) * Math.sqrt(normB);
    return denom === 0 ? 0 : dotProduct / denom;
  }

  /**
   * Compare current action against user's behavioral fingerprint
   * @param {Object} fingerprint - Stored fingerprint
   * @param {Object} currentFeatures - Current action features
   * @returns {{ similarity: number, deviations: string[] }}
   */
  compareToFingerprint(fingerprint, currentFeatures) {
    if (!fingerprint) return { similarity: 0.5, deviations: ['insufficient_data'] };

    const deviations = [];
    let totalSimilarity = 0;
    let checks = 0;

    // Hour-of-day check
    const hourProb = fingerprint.hourDistribution[currentFeatures.hourOfDay] || 0;
    if (hourProb < 0.02) {
      deviations.push(`unusual_hour:${currentFeatures.hourOfDay}`);
    }
    totalSimilarity += Math.min(1, hourProb * 10);
    checks++;

    // Day-of-week check
    const dayProb = fingerprint.dayDistribution[currentFeatures.dayOfWeek] || 0;
    if (dayProb < 0.05) {
      deviations.push(`unusual_day:${currentFeatures.dayOfWeek}`);
    }
    totalSimilarity += Math.min(1, dayProb * 7);
    checks++;

    // Action type check
    const actionProb = fingerprint.actionDistribution[currentFeatures.actionType] || 0;
    if (actionProb < 0.01) {
      deviations.push(`rare_action:${currentFeatures.actionType}`);
    }
    totalSimilarity += Math.min(1, actionProb * 5);
    checks++;

    // Platform check
    const platProb = fingerprint.platformDistribution[currentFeatures.platform] || 0;
    if (platProb < 0.01) {
      deviations.push(`unknown_platform:${currentFeatures.platform}`);
    }
    totalSimilarity += Math.min(1, platProb * 5);
    checks++;

    // Severity deviation
    if (currentFeatures.severity > fingerprint.avgSeverity + 2) {
      deviations.push('severity_spike');
    }
    totalSimilarity += currentFeatures.severity <= fingerprint.avgSeverity + 1 ? 1 : 0.3;
    checks++;

    const similarity = checks > 0 ? Math.round((totalSimilarity / checks) * 100) / 100 : 0.5;

    return { similarity, deviations };
  }

  // ─── Main API ───────────────────────────────────────────────

  /**
   * Record an action and analyze it against the user's pattern
   * @param {string} userId
   * @param {Object} rawAction - Raw action from frontend
   * @returns {Object} Pattern analysis result
   */
  analyze(userId, rawAction) {
    const profile = this._getProfile(userId);
    const features = this.extractFeatures(rawAction);

    // Compare against existing fingerprint
    let comparison = { similarity: 0.5, deviations: ['new_user'] };
    if (profile.fingerprint) {
      comparison = this.compareToFingerprint(profile.fingerprint, features);
    }

    // Record the action
    profile.actions.push(features);
    if (profile.actions.length > this.historyDepth) {
      profile.actions.shift();
    }

    // Recompute fingerprint every 10 actions
    if (profile.actions.length % 10 === 0 && profile.actions.length >= 20) {
      profile.fingerprint = this.computeFingerprint(profile.actions);
    }

    profile.lastUpdated = Date.now();

    const isDeviation = comparison.similarity < this.similarityThreshold;

    return {
      userId,
      similarity: comparison.similarity,
      isDeviation,
      deviations: comparison.deviations,
      actionsRecorded: profile.actions.length,
      hasFingerprint: !!profile.fingerprint,
      analyzedAt: new Date().toISOString()
    };
  }

  /**
   * Get a user's behavioral fingerprint
   */
  getFingerprint(userId) {
    const profile = this.profiles.get(userId);
    if (!profile) return null;

    // Compute fresh if stale
    if (!profile.fingerprint && profile.actions.length >= 20) {
      profile.fingerprint = this.computeFingerprint(profile.actions);
    }

    return profile.fingerprint;
  }

  getStats() {
    return {
      usersTracked: this.profiles.size,
      usersWithFingerprint: [...this.profiles.values()].filter(p => p.fingerprint).length,
      config: {
        maxUsers: this.maxUsers,
        historyDepth: this.historyDepth,
        similarityThreshold: this.similarityThreshold
      }
    };
  }

  reset() {
    this.profiles.clear();
  }
}

module.exports = { PatternAnalyzer };
