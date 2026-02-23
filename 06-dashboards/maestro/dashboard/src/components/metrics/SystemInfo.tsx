import { useState } from 'react';
import type { SystemInfo as SystemInfoType } from '../../types/metrics';
import { formatPercent, formatBytes } from '../../utils/formatters';
import './SystemInfo.css';

interface SystemInfoProps {
  info: SystemInfoType;
}

export default function SystemInfo({ info }: SystemInfoProps) {
  const [expanded, setExpanded] = useState(false);

  return (
    <div className="system-info">
      <div className="system-header">
        <h2>System Information</h2>
        <button
          className="btn btn-secondary"
          onClick={() => setExpanded(!expanded)}
        >
          {expanded ? 'Collapse' : 'Expand'}
        </button>
      </div>

      <div className="system-grid">
        <div className="system-metric">
          <div className="metric-label">CPU Usage</div>
          <div className="metric-value">{formatPercent(info.cpu.usage)}</div>
          <div className="metric-bar">
            <div 
              className="metric-bar-fill" 
              style={{ width: `${info.cpu.usage}%` }}
            />
          </div>
          {expanded && (
            <div className="metric-details">
              <div>CPU Cores: {info.cpu.count}</div>
              <div>CPU Model: {info.cpu.model}</div>
              {info.cpu.speed && <div>CPU Speed: {info.cpu.speed} MHz</div>}
            </div>
          )}
        </div>
        <div className="system-metric">
          <div className="metric-label">Memory Usage</div>
          <div className="metric-value">{formatPercent(info.memory.percentage)}</div>
          <div className="metric-bar">
            <div 
              className="metric-bar-fill" 
              style={{ width: `${info.memory.percentage}%` }}
            />
          </div>
          {expanded && (
            <div className="metric-details">
              <div>Used: {formatBytes(info.memory.used)}</div>
              <div>Total: {formatBytes(info.memory.total)}</div>
              <div>Free: {formatBytes(info.memory.total - info.memory.used)}</div>
            </div>
          )}
        </div>
        <div className="system-metric">
          <div className="metric-label">Heap Usage</div>
          <div className="metric-value">
            {formatPercent((info.memory.heapUsed / info.memory.heapLimit) * 100)}
          </div>
          <div className="metric-bar">
            <div 
              className="metric-bar-fill" 
              style={{ width: `${(info.memory.heapUsed / info.memory.heapLimit) * 100}%` }}
            />
          </div>
          {expanded && (
            <div className="metric-details">
              <div>Heap Used: {formatBytes(info.memory.heapUsed)}</div>
              <div>Heap Total: {formatBytes(info.memory.heapTotal)}</div>
              <div>Heap Limit: {formatBytes(info.memory.heapLimit)}</div>
              <div>External: {formatBytes(info.memory.external)}</div>
              {info.memory.rss && <div>RSS: {formatBytes(info.memory.rss)}</div>}
              {info.memory.arrayBuffers && <div>Array Buffers: {formatBytes(info.memory.arrayBuffers)}</div>}
            </div>
          )}
        </div>
        <div className="system-metric">
          <div className="metric-label">Uptime</div>
          <div className="metric-value">
            {Math.floor(info.uptime / 3600)}h {Math.floor((info.uptime % 3600) / 60)}m
          </div>
        </div>
        <div className="system-metric">
          <div className="metric-label">Platform</div>
          <div className="metric-value">{info.platform.type} ({info.platform.arch})</div>
          {expanded && (
            <div className="metric-details">
              <div>Hostname: {info.platform.hostname}</div>
              {info.platform.release && <div>Release: {info.platform.release}</div>}
              {info.platform.platform && <div>Platform: {info.platform.platform}</div>}
            </div>
          )}
        </div>
        <div className="system-metric">
          <div className="metric-label">Node Version</div>
          <div className="metric-value">{info.platform.nodeVersion}</div>
        </div>
        {expanded && info.eventLoop && (
          <>
            <div className="system-metric">
              <div className="metric-label">Event Loop Lag</div>
              <div className="metric-value">{info.eventLoop.lag.toFixed(2)} ms</div>
            </div>
            <div className="system-metric">
              <div className="metric-label">Event Loop Utilization</div>
              <div className="metric-value">{formatPercent(info.eventLoop.utilization * 100)}</div>
            </div>
            <div className="system-metric">
              <div className="metric-label">Active Handles</div>
              <div className="metric-value">{info.activeHandles || 0}</div>
            </div>
            <div className="system-metric">
              <div className="metric-label">Active Requests</div>
              <div className="metric-value">{info.activeRequests || 0}</div>
            </div>
          </>
        )}
        {expanded && info.network && (
          <div className="system-metric">
            <div className="metric-label">Network I/O</div>
            <div className="metric-value">
              {formatBytes(info.network.bytesSent)} sent / {formatBytes(info.network.bytesReceived)} received
            </div>
            <div className="metric-details">
              <div>Connections: {info.network.connections}</div>
            </div>
          </div>
        )}
        {expanded && info.disk && (
          <div className="system-metric">
            <div className="metric-label">Disk I/O</div>
            <div className="metric-value">
              {info.disk.readOps} reads / {info.disk.writeOps} writes
            </div>
            <div className="metric-details">
              <div>Space Used: {formatBytes(info.disk.spaceUsed)}</div>
              <div>Space Free: {formatBytes(info.disk.spaceFree)}</div>
              <div>Space Total: {formatBytes(info.disk.spaceTotal)}</div>
            </div>
          </div>
        )}
        {expanded && info.processTree && info.processTree.length > 0 && (
          <div className="system-metric full-width">
            <div className="metric-label">Process Tree ({info.processTree.length})</div>
            <div className="process-tree">
              {info.processTree.slice(0, 10).map((proc, idx) => (
                <div key={idx} className="process-item">
                  <span className="process-name">{proc.name}</span>
                  <span className="process-pid">PID: {proc.pid}</span>
                  <span className="process-cpu">CPU: {proc.cpu.toFixed(1)}%</span>
                  <span className="process-memory">Mem: {formatBytes(proc.memory)}</span>
                </div>
              ))}
              {info.processTree.length > 10 && (
                <div className="process-more">... and {info.processTree.length - 10} more processes</div>
              )}
            </div>
          </div>
        )}
        {expanded && info.environment && Object.keys(info.environment).length > 0 && (
          <div className="system-metric full-width">
            <div className="metric-label">Environment Variables ({Object.keys(info.environment).length})</div>
            <div className="environment-vars">
              {Object.entries(info.environment).slice(0, 20).map(([key, value]) => (
                <div key={key} className="env-item">
                  <span className="env-key">{key}</span>
                  <span className="env-value">{value}</span>
                </div>
              ))}
              {Object.keys(info.environment).length > 20 && (
                <div className="env-more">... and {Object.keys(info.environment).length - 20} more variables</div>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
