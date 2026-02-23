import { useState, useEffect } from 'react';
import { api } from '../../services/api';
import './McpServerPanel.css';

interface McpServerPanelProps {
  status: { running: boolean; pid?: number };
  onStatusChange: () => void;
}

interface McpInfo {
  running: boolean;
  port?: number;
  resources: Array<{ uri: string; name: string; description: string; mimeType?: string }>;
  tools: Array<{ name: string; description: string; inputSchema: any }>;
  resourceCount: number;
  toolCount: number;
  health: string;
  baseUrl?: string;
}

export default function McpServerPanel({ status, onStatusChange }: McpServerPanelProps) {
  const [loading, setLoading] = useState(false);
  const [mcpInfo, setMcpInfo] = useState<McpInfo | null>(null);
  const [expanded, setExpanded] = useState(false);

  useEffect(() => {
    loadMcpInfo();
    const interval = setInterval(() => {
      if (status.running) {
        loadMcpInfo();
      }
    }, 5000);
    return () => clearInterval(interval);
  }, [status.running]);

  const loadMcpInfo = async () => {
    try {
      const info = await api.getMcpInfo();
      setMcpInfo(info);
    } catch (error) {
      console.error('Failed to load MCP info:', error);
    }
  };

  const handleStart = async () => {
    setLoading(true);
    try {
      await api.startMcpServer();
      onStatusChange();
      setTimeout(() => loadMcpInfo(), 1000);
    } catch (error) {
      console.error('Failed to start MCP server:', error);
      alert('Failed to start MCP server');
    } finally {
      setLoading(false);
    }
  };

  const handleStop = async () => {
    setLoading(true);
    try {
      await api.stopMcpServer();
      onStatusChange();
      setMcpInfo(null);
    } catch (error) {
      console.error('Failed to stop MCP server:', error);
      alert('Failed to stop MCP server');
    } finally {
      setLoading(false);
    }
  };

  const getHealthColor = (health: string) => {
    switch (health) {
      case 'healthy':
        return '#4caf50';
      case 'unhealthy':
        return '#f44336';
      case 'starting':
        return '#ff9800';
      default:
        return '#9e9e9e';
    }
  };

  return (
    <div className="mcp-panel">
      <div className="mcp-header">
        <h2>MCP Server</h2>
        <button
          className="btn btn-secondary"
          onClick={() => setExpanded(!expanded)}
        >
          {expanded ? 'Collapse' : 'Expand'}
        </button>
      </div>
      <div className="mcp-content">
        <div className="mcp-status">
          <span className="status-indicator">
            {status.running ? '🟢 Running' : '🔴 Stopped'}
          </span>
          {status.pid && (
            <span className="pid-info">PID: {status.pid}</span>
          )}
          {mcpInfo && (
            <>
              {mcpInfo.port && <span className="port-info">Port: {mcpInfo.port}</span>}
              <span 
                className="health-indicator"
                style={{ color: getHealthColor(mcpInfo.health) }}
              >
                Health: {mcpInfo.health}
              </span>
            </>
          )}
        </div>
        <div className="mcp-actions">
          {status.running ? (
            <button
              className="btn btn-danger"
              onClick={handleStop}
              disabled={loading}
            >
              {loading ? 'Stopping...' : 'Stop Server'}
            </button>
          ) : (
            <button
              className="btn btn-primary"
              onClick={handleStart}
              disabled={loading}
            >
              {loading ? 'Starting...' : 'Start Server'}
            </button>
          )}
        </div>
        {expanded && mcpInfo && (
          <div className="mcp-details">
            <div className="mcp-stats">
              <div className="stat-item">
                <span className="stat-label">Resources:</span>
                <span className="stat-value">{mcpInfo.resourceCount}</span>
              </div>
              <div className="stat-item">
                <span className="stat-label">Tools:</span>
                <span className="stat-value">{mcpInfo.toolCount}</span>
              </div>
              {mcpInfo.baseUrl && (
                <div className="stat-item">
                  <span className="stat-label">Base URL:</span>
                  <span className="stat-value">{mcpInfo.baseUrl}</span>
                </div>
              )}
            </div>
            {mcpInfo.resources.length > 0 && (
              <div className="mcp-resources">
                <h3>Resources ({mcpInfo.resources.length})</h3>
                <div className="resource-list">
                  {mcpInfo.resources.map((resource, idx) => (
                    <div key={idx} className="resource-item">
                      <div className="resource-name">{resource.name}</div>
                      <div className="resource-uri">{resource.uri}</div>
                      <div className="resource-description">{resource.description}</div>
                    </div>
                  ))}
                </div>
              </div>
            )}
            {mcpInfo.tools.length > 0 && (
              <div className="mcp-tools">
                <h3>Tools ({mcpInfo.tools.length})</h3>
                <div className="tool-list">
                  {mcpInfo.tools.map((tool, idx) => (
                    <div key={idx} className="tool-item">
                      <div className="tool-name">{tool.name}</div>
                      <div className="tool-description">{tool.description}</div>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
