'use strict';

const { SwarmMonitor } = require('../lib/swarm-monitor');

describe('SwarmMonitor', () => {
  let monitor;

  beforeEach(() => {
    monitor = new SwarmMonitor({
      heartbeatTimeout: 5000, // 5 seconds for testing
      fragmentationThreshold: 0.51,
      maxNodes: 100
    });
  });

  describe('heartbeat', () => {
    test('registers a new node', () => {
      const result = monitor.heartbeat({ id: 'node-1', type: 'nexus', region: 'caribe' });
      expect(result.nodeId).toBe('node-1');
      expect(result.status).toBe('active');
      expect(result.heartbeatCount).toBe(1);
    });

    test('increments heartbeat count on repeat', () => {
      monitor.heartbeat({ id: 'node-1' });
      const result = monitor.heartbeat({ id: 'node-1' });
      expect(result.heartbeatCount).toBe(2);
    });

    test('requires id', () => {
      expect(() => monitor.heartbeat({})).toThrow('Node ID is required');
    });

    test('evicts oldest node when at capacity', () => {
      for (let i = 0; i < 105; i++) {
        monitor.heartbeat({ id: `node-${i}` });
      }
      expect(monitor.getStats().totalNodes).toBeLessThanOrEqual(100);
    });
  });

  describe('sweep', () => {
    test('marks recent nodes as active', () => {
      monitor.heartbeat({ id: 'n1' });
      const health = monitor.sweep();
      expect(health.active).toBe(1);
      expect(health.inactive).toBe(0);
      expect(health.dead).toBe(0);
    });

    test('detects consensus reachability', () => {
      for (let i = 0; i < 10; i++) {
        monitor.heartbeat({ id: `n-${i}` });
      }
      const health = monitor.sweep();
      expect(health.consensusReachable).toBe(true);
      expect(health.isFragmented).toBe(false);
    });

    test('returns valid health object', () => {
      const health = monitor.sweep();
      expect(health).toHaveProperty('total');
      expect(health).toHaveProperty('active');
      expect(health).toHaveProperty('inactive');
      expect(health).toHaveProperty('dead');
      expect(health).toHaveProperty('activeRatio');
      expect(health).toHaveProperty('consensusReachable');
      expect(health).toHaveProperty('sweptAt');
    });
  });

  describe('checkConsensus', () => {
    test('returns consensus status for all nodes', () => {
      for (let i = 0; i < 5; i++) {
        monitor.heartbeat({ id: `c-${i}`, region: i < 3 ? 'norte' : 'sur' });
      }

      const consensus = monitor.checkConsensus();
      expect(consensus.canReachConsensus).toBe(true);
      expect(consensus.activeNodes).toBe(5);
      expect(consensus.totalNodes).toBe(5);
      expect(consensus.regionDistribution).toBeDefined();
    });

    test('checks specific participants', () => {
      monitor.heartbeat({ id: 'a' });
      monitor.heartbeat({ id: 'b' });
      monitor.heartbeat({ id: 'c' });

      const consensus = monitor.checkConsensus(['a', 'b']);
      expect(consensus.totalNodes).toBe(2);
    });

    test('reports geographic diversity', () => {
      monitor.heartbeat({ id: 'n1', region: 'caribe' });
      monitor.heartbeat({ id: 'n2', region: 'andes' });
      monitor.heartbeat({ id: 'n3', region: 'amazonia' });

      const consensus = monitor.checkConsensus();
      expect(consensus.geographicDiversity).toBe('high');
    });
  });

  describe('getNodesInBounds', () => {
    test('returns nodes within bounding box', () => {
      monitor.heartbeat({ id: 'pr', lat: 18.4, lng: -66.1, region: 'caribe' });
      monitor.heartbeat({ id: 'mx', lat: 19.4, lng: -99.1, region: 'norte' });
      monitor.heartbeat({ id: 'br', lat: -23.5, lng: -46.6, region: 'sur' });

      const caribbean = monitor.getNodesInBounds(15, 25, -70, -60);
      expect(caribbean.length).toBe(1);
      expect(caribbean[0].id).toBe('pr');
    });

    test('returns empty for no matches', () => {
      monitor.heartbeat({ id: 'n1', lat: 0, lng: 0 });
      const result = monitor.getNodesInBounds(50, 60, 50, 60);
      expect(result).toHaveLength(0);
    });
  });

  describe('getNodesByType', () => {
    test('groups nodes by type', () => {
      monitor.heartbeat({ id: 'a', type: 'nexus' });
      monitor.heartbeat({ id: 'b', type: 'seed' });
      monitor.heartbeat({ id: 'c', type: 'nexus' });

      const byType = monitor.getNodesByType();
      expect(byType.nexus).toHaveLength(2);
      expect(byType.seed).toHaveLength(1);
    });
  });

  describe('alerts', () => {
    test('getAlerts returns empty initially', () => {
      expect(monitor.getAlerts()).toHaveLength(0);
    });
  });

  describe('stats', () => {
    test('returns comprehensive stats', () => {
      monitor.heartbeat({ id: 'n1', type: 'nexus', region: 'caribe' });
      monitor.heartbeat({ id: 'n2', type: 'seed', region: 'andes' });

      const stats = monitor.getStats();
      expect(stats.totalNodes).toBe(2);
      expect(stats.byType.nexus).toBe(1);
      expect(stats.byType.seed).toBe(1);
      expect(stats.byRegion.caribe).toBe(1);
      expect(stats.config.heartbeatTimeout).toBe(5000);
    });

    test('reset clears everything', () => {
      monitor.heartbeat({ id: 'n1' });
      monitor.reset();
      expect(monitor.getStats().totalNodes).toBe(0);
      expect(monitor.getAlerts()).toHaveLength(0);
    });
  });
});
