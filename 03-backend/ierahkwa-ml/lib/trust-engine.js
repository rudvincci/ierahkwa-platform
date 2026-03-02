'use strict';

// ============================================================================
// TRUST ENGINE — Dynamic Multi-Factor Trust Scoring
// Calculates trust scores (0-100) based on behavioral signals
// Backs the TrustAgent from ierahkwa-agents.js with real server-side logic
// ============================================================================

const crypto = require('crypto');

// ── Trust Factor Weights ─────────────────────────────────────
const DEFAULT_WEIGHTS = {
  sessionAge:       0.10,  // How long has this session been active
  authStrength:     0.15,  // Auth method quality (MFA > password > anon)
  behaviorConsistency: 0.20,  // Does behavior match historical patterns
  ipReputation:     0.10,  // IP consistency and known-good status
  requestPattern:   0.15,  // Normal request patterns vs suspicious
  transactionHistory: 0.15,  // Past transaction success/failure ratio
  communityVouching: 0.15   // Social trust / peer vouching (DAO)
};

// ── Trust Level Thresholds ───────────────────────────────────
const TRUST_LEVELS = [
  { min: 90, level: 'sovereign',  label: 'Soberano Verificado', permissions: ['all'] },
  { min: 75, level: 'trusted',    label: 'Confianza Alta',      permissions: ['trade', 'vote', 'transfer'] },
  { min: 50, level: 'standard',   label: 'Estándar',            permissions: ['trade', 'vote'] },
  { min: 25, level: 'limited',    label: 'Limitado',            permissions: ['view', 'vote'] },
  { min: 0,  level: 'untrusted',  label: 'Sin Confianza',       permissions: ['view'] }
];

class TrustEngine {
  /**
   * @param {Object} [opts]
   * @param {Object} [opts.weights] - Custom factor weights (must sum to 1.0)
   * @param {number} [opts.decayRate=0.02] - Trust decay per hour of inactivity
   * @param {number} [opts.boostRate=0.5]  - Max trust boost per positive action
   */
  constructor(opts = {}) {
    this.weights = opts.weights || { ...DEFAULT_WEIGHTS };
    this.decayRate = opts.decayRate || 0.02;
    this.boostRate = opts.boostRate || 0.5;

    // Per-user trust profiles: Map<userId, TrustProfile>
    this.profiles = new Map();
    this.maxProfiles = 10000;
  }

  // ─── Profile Management ─────────────────────────────────────

  _getProfile(userId) {
    if (!this.profiles.has(userId)) {
      this.profiles.set(userId, {
        userId,
        score: 50, // Start neutral
        factors: {
          sessionAge: 50,
          authStrength: 50,
          behaviorConsistency: 50,
          ipReputation: 50,
          requestPattern: 50,
          transactionHistory: 50,
          communityVouching: 50
        },
        history: [],
        lastActivity: Date.now(),
        createdAt: new Date().toISOString()
      });

      // Evict oldest if over capacity
      if (this.profiles.size > this.maxProfiles) {
        const oldest = [...this.profiles.entries()]
          .sort((a, b) => a[1].lastActivity - b[1].lastActivity)[0];
        if (oldest) this.profiles.delete(oldest[0]);
      }
    }
    return this.profiles.get(userId);
  }

  // ─── Factor Scoring Functions ───────────────────────────────

  /**
   * Score session age (longer active session = more trust)
   * @param {number} sessionStartMs - Session start timestamp
   * @returns {number} 0-100
   */
  scoreSessionAge(sessionStartMs) {
    const ageMinutes = (Date.now() - sessionStartMs) / 60000;
    // Logarithmic curve: quick gain up to ~60 min, then plateaus
    return Math.min(100, Math.round(25 * Math.log2(ageMinutes + 1)));
  }

  /**
   * Score authentication strength
   * @param {string} method - 'mfa', 'jwt', 'password', 'sbt', 'anonymous'
   * @returns {number} 0-100
   */
  scoreAuthStrength(method) {
    const scores = {
      sbt: 100,        // Soulbound Token (highest)
      mfa: 95,         // Multi-factor auth
      jwt: 75,         // JWT token
      password: 50,    // Password only
      session: 40,     // Session cookie
      anonymous: 10    // No auth
    };
    return scores[method] || 10;
  }

  /**
   * Score behavior consistency against historical patterns
   * @param {Object} current - Current behavior snapshot
   * @param {Object[]} history - Historical behavior snapshots
   * @returns {number} 0-100
   */
  scoreBehaviorConsistency(current, history) {
    if (!history || history.length < 3) return 50; // Neutral when insufficient data

    // Compare feature vectors: time-of-day, action types, navigation patterns
    let matchCount = 0;
    let totalChecks = 0;

    for (const past of history.slice(-10)) {
      // Time-of-day similarity (±2 hours)
      if (current.hourOfDay !== undefined && past.hourOfDay !== undefined) {
        totalChecks++;
        if (Math.abs(current.hourOfDay - past.hourOfDay) <= 2) matchCount++;
      }

      // Action type similarity
      if (current.actionType && past.actionType) {
        totalChecks++;
        if (current.actionType === past.actionType) matchCount++;
      }

      // Platform consistency
      if (current.platform && past.platform) {
        totalChecks++;
        if (current.platform === past.platform) matchCount++;
      }
    }

    if (totalChecks === 0) return 50;
    return Math.round((matchCount / totalChecks) * 100);
  }

  /**
   * Score IP reputation (consistency)
   * @param {string} currentIp
   * @param {string[]} knownIps - Previously seen IPs
   * @returns {number} 0-100
   */
  scoreIpReputation(currentIp, knownIps = []) {
    if (knownIps.length === 0) return 40; // First time, neutral-low
    if (knownIps.includes(currentIp)) return 90; // Known IP
    // Unknown IP but has history — suspicious
    return 20;
  }

  /**
   * Score request patterns (normal vs abnormal rate/type)
   * @param {number} requestsPerMinute
   * @param {number} errorRate - 0-1 ratio of failed requests
   * @returns {number} 0-100
   */
  scoreRequestPattern(requestsPerMinute, errorRate = 0) {
    let score = 80;

    // Penalize high request rates
    if (requestsPerMinute > 60) score -= 40;
    else if (requestsPerMinute > 30) score -= 20;
    else if (requestsPerMinute > 10) score -= 5;

    // Penalize high error rates
    score -= Math.round(errorRate * 50);

    return Math.max(0, Math.min(100, score));
  }

  /**
   * Score transaction history
   * @param {number} successCount
   * @param {number} failureCount
   * @param {number} totalVolume - Total WMP transacted
   * @returns {number} 0-100
   */
  scoreTransactionHistory(successCount = 0, failureCount = 0, totalVolume = 0) {
    const total = successCount + failureCount;
    if (total === 0) return 50; // Neutral

    const successRate = successCount / total;
    let score = Math.round(successRate * 70); // Max 70 from success rate

    // Bonus for volume (logged scale)
    if (totalVolume > 0) {
      score += Math.min(30, Math.round(5 * Math.log10(totalVolume + 1)));
    }

    return Math.min(100, score);
  }

  /**
   * Score community vouching (DAO-based trust)
   * @param {number} vouchCount - Number of community vouches
   * @param {number} voucherAvgTrust - Average trust of vouchers
   * @returns {number} 0-100
   */
  scoreCommunityVouching(vouchCount = 0, voucherAvgTrust = 50) {
    if (vouchCount === 0) return 30; // No vouches, low trust

    // Weighted: more vouches from trusted people = higher score
    const vouchPower = Math.min(50, vouchCount * 5);
    const qualityFactor = voucherAvgTrust / 100;

    return Math.round(vouchPower * qualityFactor + 30 * qualityFactor);
  }

  // ─── Main Trust Calculation ─────────────────────────────────

  /**
   * Calculate composite trust score for a user
   * @param {string} userId
   * @param {Object} signals - Raw trust signals from frontend/backend
   * @returns {Object} Trust assessment
   */
  calculateTrust(userId, signals = {}) {
    const profile = this._getProfile(userId);

    // Apply time decay for inactivity
    const hoursSinceActivity = (Date.now() - profile.lastActivity) / 3600000;
    if (hoursSinceActivity > 1) {
      const decay = Math.min(hoursSinceActivity * this.decayRate, 0.3);
      for (const key of Object.keys(profile.factors)) {
        profile.factors[key] = Math.max(0, profile.factors[key] * (1 - decay));
      }
    }

    // Update individual factors from signals
    if (signals.sessionStart) {
      profile.factors.sessionAge = this.scoreSessionAge(signals.sessionStart);
    }
    if (signals.authMethod) {
      profile.factors.authStrength = this.scoreAuthStrength(signals.authMethod);
    }
    if (signals.behavior) {
      profile.factors.behaviorConsistency = this.scoreBehaviorConsistency(
        signals.behavior, profile.history
      );
      profile.history.push(signals.behavior);
      if (profile.history.length > 50) profile.history.shift();
    }
    if (signals.ip) {
      const knownIps = profile.knownIps || [];
      profile.factors.ipReputation = this.scoreIpReputation(signals.ip, knownIps);
      if (!knownIps.includes(signals.ip)) {
        knownIps.push(signals.ip);
        if (knownIps.length > 20) knownIps.shift();
      }
      profile.knownIps = knownIps;
    }
    if (signals.requestRate !== undefined) {
      profile.factors.requestPattern = this.scoreRequestPattern(
        signals.requestRate, signals.errorRate || 0
      );
    }
    if (signals.transactions) {
      const t = signals.transactions;
      profile.factors.transactionHistory = this.scoreTransactionHistory(
        t.success, t.failure, t.volume
      );
    }
    if (signals.vouches) {
      profile.factors.communityVouching = this.scoreCommunityVouching(
        signals.vouches.count, signals.vouches.avgTrust
      );
    }

    // Calculate weighted composite score
    let compositeScore = 0;
    for (const [factor, weight] of Object.entries(this.weights)) {
      compositeScore += (profile.factors[factor] || 0) * weight;
    }
    compositeScore = Math.round(Math.max(0, Math.min(100, compositeScore)));

    profile.score = compositeScore;
    profile.lastActivity = Date.now();

    // Determine trust level
    const trustLevel = TRUST_LEVELS.find(t => compositeScore >= t.min) || TRUST_LEVELS[TRUST_LEVELS.length - 1];

    return {
      userId,
      score: compositeScore,
      level: trustLevel.level,
      label: trustLevel.label,
      permissions: trustLevel.permissions,
      factors: { ...profile.factors },
      weights: { ...this.weights },
      calculatedAt: new Date().toISOString()
    };
  }

  /**
   * Report a positive or negative event that adjusts trust
   * @param {string} userId
   * @param {'positive'|'negative'} type
   * @param {string} reason
   * @param {number} [magnitude=1] - 0-10 scale
   */
  reportEvent(userId, type, reason, magnitude = 1) {
    const profile = this._getProfile(userId);
    const clampedMag = Math.max(0, Math.min(10, magnitude));

    if (type === 'positive') {
      const boost = (clampedMag / 10) * this.boostRate;
      for (const key of Object.keys(profile.factors)) {
        profile.factors[key] = Math.min(100, profile.factors[key] + boost * 10);
      }
    } else if (type === 'negative') {
      const penalty = (clampedMag / 10) * 15; // Penalties hit harder
      for (const key of Object.keys(profile.factors)) {
        profile.factors[key] = Math.max(0, profile.factors[key] - penalty);
      }
    }

    profile.lastActivity = Date.now();

    return { userId, type, reason, magnitude: clampedMag, newScore: profile.score };
  }

  // ─── Query & Stats ──────────────────────────────────────────

  getProfile(userId) {
    return this.profiles.get(userId) || null;
  }

  getLeaderboard(limit = 20) {
    return [...this.profiles.values()]
      .sort((a, b) => b.score - a.score)
      .slice(0, limit)
      .map(p => ({
        userId: p.userId,
        score: p.score,
        level: (TRUST_LEVELS.find(t => p.score >= t.min) || TRUST_LEVELS[TRUST_LEVELS.length - 1]).level,
        lastActivity: new Date(p.lastActivity).toISOString()
      }));
  }

  getStats() {
    const profiles = [...this.profiles.values()];
    const scores = profiles.map(p => p.score);

    return {
      totalUsers: this.profiles.size,
      averageScore: scores.length > 0 ? Math.round(scores.reduce((s, v) => s + v, 0) / scores.length) : 0,
      distribution: {
        sovereign: scores.filter(s => s >= 90).length,
        trusted: scores.filter(s => s >= 75 && s < 90).length,
        standard: scores.filter(s => s >= 50 && s < 75).length,
        limited: scores.filter(s => s >= 25 && s < 50).length,
        untrusted: scores.filter(s => s < 25).length
      },
      weights: { ...this.weights }
    };
  }

  reset() {
    this.profiles.clear();
  }
}

module.exports = { TrustEngine, TRUST_LEVELS, DEFAULT_WEIGHTS };
