'use strict';

// ============================================================================
// SWARM MONITOR — Node Health & Consensus Monitoring
// Tracks heartbeats from distributed nodes (NEXUS, mesh, agents)
// Detects network fragmentation and triggers swarm autonomy
// ============================================================================

class SwarmMonitor {
  /**
   * @param {Object} [opts]
   * @param {number} [opts.heartbeatTimeout=120000] - Node considered dead after ms
   * @param {number} [opts.fragmentationThreshold=0.51] - Fraction needed for consensus
   * @param {number} [opts.maxNodes=10000] - Max tracked nodes
   */
  constructor(opts = {}) {
    this.heartbeatTimeout = opts.heartbeatTimeout || 120000; // 2 minutes
    this.fragmentationThreshold = opts.fragmentationThreshold || 0.51;
    this.maxNodes = opts.maxNodes || 10000;

    // Node registry: Map<nodeId, NodeInfo>
    this.nodes = new Map();
    // Alert history
    this.alerts = [];
    this.maxAlerts = 500;
  }

  // ─── Node Management ────────────────────────────────────────

  /**
   * Register or update a node's heartbeat
   * @param {Object} nodeData
   * @param {string} nodeData.id - Unique node identifier
   * @param {string} [nodeData.type] - 'nexus', 'seed', 'mesh', 'agent'
   * @param {number} [nodeData.lat] - Latitude
   * @param {number} [nodeData.lng] - Longitude
   * @param {string} [nodeData.region] - Geographic region
   * @param {Object} [nodeData.metrics] - CPU, memory, etc.
   * @returns {Object} Updated node status
   */
  heartbeat(nodeData) {
    const { id, type, lat, lng, region, metrics } = nodeData;

    if (!id) throw new Error('Node ID is required');

    const now = Date.now();
    const existing = this.nodes.get(id);

    const node = {
      id,
      type: type || (existing && existing.type) || 'unknown',
      lat: lat || (existing && existing.lat) || null,
      lng: lng || (existing && existing.lng) || null,
      region: region || (existing && existing.region) || 'unknown',
      metrics: metrics || {},
      status: 'active',
      lastHeartbeat: now,
      firstSeen: existing ? existing.firstSeen : now,
      heartbeatCount: existing ? existing.heartbeatCount + 1 : 1,
      uptime: existing ? now - existing.firstSeen : 0
    };

    this.nodes.set(id, node);

    // Evict oldest if over capacity
    if (this.nodes.size > this.maxNodes) {
      let oldestKey = null;
      let oldestTime = Infinity;
      for (const [key, n] of this.nodes) {
        if (n.lastHeartbeat < oldestTime) {
          oldestTime = n.lastHeartbeat;
          oldestKey = key;
        }
      }
      if (oldestKey) this.nodes.delete(oldestKey);
    }

    return {
      nodeId: id,
      status: 'active',
      heartbeatCount: node.heartbeatCount,
      uptime: node.uptime
    };
  }

  /**
   * Check all nodes and mark stale ones as inactive
   * @returns {Object} Network health summary
   */
  sweep() {
    const now = Date.now();
    let active = 0;
    let inactive = 0;
    let dead = 0;
    const newlyDead = [];

    for (const [id, node] of this.nodes) {
      const age = now - node.lastHeartbeat;

      if (age < this.heartbeatTimeout) {
        node.status = 'active';
        active++;
      } else if (age < this.heartbeatTimeout * 3) {
        node.status = 'inactive';
        inactive++;
      } else {
        if (node.status !== 'dead') {
          newlyDead.push(id);
        }
        node.status = 'dead';
        dead++;
      }
    }

    // Check for fragmentation
    const total = this.nodes.size;
    const activeRatio = total > 0 ? active / total : 0;
    const isFragmented = activeRatio < this.fragmentationThreshold;

    if (isFragmented && total > 5) {
      this._alert({
        type: 'network_fragmentation',
        severity: 'critical',
        message: `Network fragmented: ${active}/${total} nodes active (${Math.round(activeRatio * 100)}%)`,
        activeNodes: active,
        totalNodes: total,
        activeRatio
      });
    }

    for (const nodeId of newlyDead) {
      this._alert({
        type: 'node_death',
        severity: 'high',
        message: `Node ${nodeId} declared dead (no heartbeat)`,
        nodeId
      });
    }

    return {
      total,
      active,
      inactive,
      dead,
      activeRatio: Math.round(activeRatio * 100) / 100,
      isFragmented,
      consensusReachable: activeRatio >= this.fragmentationThreshold,
      sweptAt: new Date().toISOString()
    };
  }

  // ─── Consensus Check ────────────────────────────────────────

  /**
   * Check if consensus can be reached among active nodes
   * For voting, governance decisions, etc.
   * @param {string[]} [participantIds] - Specific nodes to check (null = all)
   * @returns {Object} Consensus status
   */
  checkConsensus(participantIds = null) {
    let nodes;
    if (participantIds) {
      nodes = participantIds.map(id => this.nodes.get(id)).filter(Boolean);
    } else {
      nodes = [...this.nodes.values()];
    }

    const total = nodes.length;
    const active = nodes.filter(n => n.status === 'active').length;
    const ratio = total > 0 ? active / total : 0;
    const canReachConsensus = ratio >= this.fragmentationThreshold;

    // Group by region for geographic distribution check
    const byRegion = {};
    for (const n of nodes.filter(n => n.status === 'active')) {
      byRegion[n.region] = (byRegion[n.region] || 0) + 1;
    }

    // Geographic diversity: at least 3 regions for critical decisions
    const regionCount = Object.keys(byRegion).length;
    const geographicDiversity = regionCount >= 3 ? 'high' : regionCount >= 2 ? 'medium' : 'low';

    return {
      canReachConsensus,
      activeNodes: active,
      totalNodes: total,
      activeRatio: Math.round(ratio * 100) / 100,
      requiredRatio: this.fragmentationThreshold,
      regionDistribution: byRegion,
      geographicDiversity,
      checkedAt: new Date().toISOString()
    };
  }

  // ─── Geographic Queries ─────────────────────────────────────

  /**
   * Get nodes within a bounding box
   * @param {number} minLat
   * @param {number} maxLat
   * @param {number} minLng
   * @param {number} maxLng
   * @returns {Object[]}
   */
  getNodesInBounds(minLat, maxLat, minLng, maxLng) {
    return [...this.nodes.values()]
      .filter(n =>
        n.lat !== null && n.lng !== null &&
        n.lat >= minLat && n.lat <= maxLat &&
        n.lng >= minLng && n.lng <= maxLng
      )
      .map(n => ({
        id: n.id,
        type: n.type,
        lat: n.lat,
        lng: n.lng,
        region: n.region,
        status: n.status,
        lastHeartbeat: new Date(n.lastHeartbeat).toISOString()
      }));
  }

  /**
   * Get all nodes grouped by type
   */
  getNodesByType() {
    const result = {};
    for (const node of this.nodes.values()) {
      if (!result[node.type]) result[node.type] = [];
      result[node.type].push({
        id: node.id,
        status: node.status,
        region: node.region,
        lastHeartbeat: new Date(node.lastHeartbeat).toISOString()
      });
    }
    return result;
  }

  // ─── Alerts ─────────────────────────────────────────────────

  _alert(alert) {
    alert.timestamp = new Date().toISOString();
    this.alerts.push(alert);
    if (this.alerts.length > this.maxAlerts) {
      this.alerts.shift();
    }
  }

  getAlerts(limit = 50) {
    return this.alerts.slice(-limit);
  }

  // ─── Stats ──────────────────────────────────────────────────

  getStats() {
    const nodes = [...this.nodes.values()];

    const byType = {};
    const byStatus = { active: 0, inactive: 0, dead: 0 };
    const byRegion = {};

    for (const n of nodes) {
      byType[n.type] = (byType[n.type] || 0) + 1;
      byStatus[n.status] = (byStatus[n.status] || 0) + 1;
      byRegion[n.region] = (byRegion[n.region] || 0) + 1;
    }

    return {
      totalNodes: this.nodes.size,
      byType,
      byStatus,
      byRegion,
      alerts: this.alerts.length,
      config: {
        heartbeatTimeout: this.heartbeatTimeout,
        fragmentationThreshold: this.fragmentationThreshold
      }
    };
  }

  reset() {
    this.nodes.clear();
    this.alerts = [];
  }
}

module.exports = { SwarmMonitor };
