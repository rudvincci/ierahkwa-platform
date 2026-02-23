/**
 * Automatic Memory Management Panel
 * Shows automatic memory saving status for workflows and sessions
 */

import { useState, useEffect } from 'react';
import './AutoMemoryPanel.css';

interface MemoryStats {
  totalEntries: number;
  workflowsTracked: number;
  sessionsTracked: number;
  lastSaved: Date | string;
  autoSaveEnabled: boolean;
}

export default function AutoMemoryPanel() {
  const [stats, setStats] = useState<MemoryStats | null>(null);

  const loadStats = async () => {
    try {
      const response = await fetch('/api/memory/stats');
      const data = await response.json();
      setStats(data.stats || null);
    } catch (error) {
      console.error('Failed to load memory stats:', error);
    }
  };

  useEffect(() => {
    loadStats();
    const interval = setInterval(loadStats, 10000); // Refresh every 10 seconds
    return () => clearInterval(interval);
  }, []);

  return (
    <div className="auto-memory-panel">
      <div className="panel-header">
        <h2>Automatic Memory Management</h2>
        <div className="status-badge status-active">
          <span className="status-dot"></span>
          Active
        </div>
      </div>

      <div className="memory-info">
        <p className="info-text">
          Maestro automatically saves workflow and session memory. No manual intervention required.
        </p>
      </div>

      {stats && (
        <div className="memory-stats">
          <div className="stats-grid">
            <div className="stat-item">
              <span className="stat-label">Total Entries</span>
              <span className="stat-value">{stats.totalEntries}</span>
            </div>
            <div className="stat-item">
              <span className="stat-label">Workflows Tracked</span>
              <span className="stat-value">{stats.workflowsTracked}</span>
            </div>
            <div className="stat-item">
              <span className="stat-label">Sessions Tracked</span>
              <span className="stat-value">{stats.sessionsTracked}</span>
            </div>
            <div className="stat-item">
              <span className="stat-label">Last Saved</span>
              <span className="stat-value">
                {stats.lastSaved ? new Date(stats.lastSaved).toLocaleTimeString() : 'Never'}
              </span>
            </div>
          </div>
        </div>
      )}

      <div className="memory-features">
        <h3>Automatic Features</h3>
        <ul>
          <li>✅ Workflow execution memory saved automatically</li>
          <li>✅ Session context preserved across runs</li>
          <li>✅ Agent activity tracked and stored</li>
          <li>✅ Pattern extraction from completed workflows</li>
          <li>✅ Knowledge graph generation</li>
        </ul>
      </div>
    </div>
  );
}

