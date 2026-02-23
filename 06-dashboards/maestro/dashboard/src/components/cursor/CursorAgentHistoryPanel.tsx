/**
 * Cursor Agent History Panel
 * Displays agent history and management
 */

import { useState, useEffect, useCallback } from 'react';
import './CursorAgentHistoryPanel.css';

interface AgentInfo {
  id: string;
  name: string;
  status: 'active' | 'idle' | 'stopped';
  created_at: Date | string;
  last_activity?: Date | string;
  history?: Array<{
    timestamp: Date | string;
    action: string;
    details?: Record<string, any>;
  }>;
}

interface AgentStats {
  total: number;
  active: number;
  idle: number;
  stopped: number;
}

export default function CursorAgentHistoryPanel() {
  const [agents, setAgents] = useState<AgentInfo[]>([]);
  const [stats, setStats] = useState<AgentStats | null>(null);
  const [selectedAgent, setSelectedAgent] = useState<string | null>(null);
  const [agentDetails, setAgentDetails] = useState<AgentInfo | null>(null);
  const [loading, setLoading] = useState(false);

  const loadAgents = useCallback(async () => {
    setLoading(true);
    try {
      const response = await fetch('/api/cursor/agents');
      const data = await response.json();
      setAgents(data.agents || []);
      setStats(data.stats || null);
    } catch (error) {
      console.error('Failed to load agents:', error);
    } finally {
      setLoading(false);
    }
  }, []);

  const loadAgentDetails = useCallback(async (agentId: string) => {
    try {
      const response = await fetch(`/api/cursor/agents/${agentId}`);
      const data = await response.json();
      setAgentDetails(data.agent);
    } catch (error) {
      console.error('Failed to load agent details:', error);
    }
  }, []);

  const handleDeleteAgent = async (agentId: string) => {
    if (!confirm(`Are you sure you want to delete agent "${agentId}"?`)) {
      return;
    }

    try {
      const response = await fetch(`/api/cursor/agents/${agentId}`, {
        method: 'DELETE',
      });
      
      if (response.ok) {
        await loadAgents();
        if (selectedAgent === agentId) {
          setSelectedAgent(null);
          setAgentDetails(null);
        }
      } else {
        const error = await response.json();
        alert(`Failed to delete agent: ${error.error || 'Unknown error'}`);
      }
    } catch (error) {
      console.error('Failed to delete agent:', error);
      alert('Failed to delete agent');
    }
  };

  const handleOpenTerminal = async (agentId: string) => {
    try {
      const response = await fetch(`/api/cursor/agents/${agentId}/terminal`, {
        method: 'POST',
      });
      
      if (response.ok) {
        const data = await response.json();
        alert(`Terminal command: ${data.command || 'Terminal opened'}`);
      } else {
        const error = await response.json();
        alert(`Failed to open terminal: ${error.error || 'Unknown error'}`);
      }
    } catch (error) {
      console.error('Failed to open terminal:', error);
      alert('Failed to open terminal');
    }
  };

  useEffect(() => {
    loadAgents();
    const interval = setInterval(loadAgents, 30000); // Refresh every 30 seconds
    return () => clearInterval(interval);
  }, [loadAgents]);

  useEffect(() => {
    if (selectedAgent) {
      loadAgentDetails(selectedAgent);
    }
  }, [selectedAgent, loadAgentDetails]);

  return (
    <div className="cursor-agent-history-panel">
      <div className="panel-header">
        <h2>Cursor Agents</h2>
        <button className="btn btn-secondary" onClick={loadAgents} disabled={loading}>
          {loading ? 'Refreshing...' : 'Refresh'}
        </button>
      </div>

      {stats && (
        <div className="agent-stats">
          <div className="stats-grid">
            <div className="stat-item">
              <span className="stat-label">Total</span>
              <span className="stat-value">{stats.total}</span>
            </div>
            <div className="stat-item stat-active">
              <span className="stat-label">Active</span>
              <span className="stat-value">{stats.active}</span>
            </div>
            <div className="stat-item stat-idle">
              <span className="stat-label">Idle</span>
              <span className="stat-value">{stats.idle}</span>
            </div>
            <div className="stat-item stat-stopped">
              <span className="stat-label">Stopped</span>
              <span className="stat-value">{stats.stopped}</span>
            </div>
          </div>
        </div>
      )}

      <div className="agents-list">
        <h3>Agents ({agents.length})</h3>
        {loading && agents.length === 0 ? (
          <p>Loading agents...</p>
        ) : agents.length > 0 ? (
          <div className="agents-items">
            {agents.map((agent) => (
              <div
                key={agent.id}
                className={`agent-item agent-${agent.status} ${selectedAgent === agent.id ? 'selected' : ''}`}
              >
                <div 
                  className="agent-content"
                  onClick={() => setSelectedAgent(selectedAgent === agent.id ? null : agent.id)}
                >
                  <div className="agent-header">
                    <span className="agent-name">{agent.name}</span>
                    <span className={`agent-status status-${agent.status}`}>
                      {agent.status}
                    </span>
                  </div>
                  <div className="agent-info">
                    <span className="agent-id">ID: {agent.id}</span>
                    <span className="agent-created">
                      Created: {new Date(agent.created_at).toLocaleString()}
                    </span>
                    {agent.last_activity && (
                      <span className="agent-activity">
                        Last Activity: {new Date(agent.last_activity).toLocaleString()}
                      </span>
                    )}
                  </div>
                </div>
                <div className="agent-actions" onClick={(e) => e.stopPropagation()}>
                  <button
                    className="btn btn-secondary btn-small"
                    onClick={() => handleOpenTerminal(agent.id)}
                    title="Open in Terminal"
                  >
                    Terminal
                  </button>
                  <button
                    className="btn btn-danger btn-small"
                    onClick={() => handleDeleteAgent(agent.id)}
                    title="Delete Agent"
                  >
                    Delete
                  </button>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p>No agents found</p>
        )}
      </div>

      {agentDetails && (
        <div className="agent-details">
          <h3>Agent Details: {agentDetails.name}</h3>
          <div className="details-info">
            <div className="info-row">
              <span className="info-label">ID:</span>
              <span className="info-value">{agentDetails.id}</span>
            </div>
            <div className="info-row">
              <span className="info-label">Status:</span>
              <span className={`info-value status-${agentDetails.status}`}>
                {agentDetails.status}
              </span>
            </div>
            <div className="info-row">
              <span className="info-label">Created:</span>
              <span className="info-value">
                {new Date(agentDetails.created_at).toLocaleString()}
              </span>
            </div>
            {agentDetails.last_activity && (
              <div className="info-row">
                <span className="info-label">Last Activity:</span>
                <span className="info-value">
                  {new Date(agentDetails.last_activity).toLocaleString()}
                </span>
              </div>
            )}
          </div>

          {agentDetails.history && agentDetails.history.length > 0 && (
            <div className="agent-history">
              <h4>History ({agentDetails.history.length})</h4>
              <div className="history-items">
                {agentDetails.history.map((entry, idx) => (
                  <div key={idx} className="history-item">
                    <div className="history-time">
                      {new Date(entry.timestamp).toLocaleString()}
                    </div>
                    <div className="history-action">{entry.action}</div>
                    {entry.details && Object.keys(entry.details).length > 0 && (
                      <div className="history-details">
                        <pre>{JSON.stringify(entry.details, null, 2)}</pre>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}

