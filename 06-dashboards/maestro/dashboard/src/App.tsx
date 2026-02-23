import { useState, useEffect } from 'react';
import { useWebSocket } from './hooks/useWebSocket';
import type { DashboardMessage } from './types/websocket';
import type { DashboardMetrics } from './types/metrics';
import type { WorkflowDefinition } from './types/workflow';
import { api } from './services/api';
import Header from './components/layout/Header';
import WorkflowList from './components/workflows/WorkflowList';
import WorkflowControls from './components/workflows/WorkflowControls';
import SystemInfo from './components/metrics/SystemInfo';
import LogViewer from './components/logs/LogViewer';
import McpServerPanel from './components/mcp/McpServerPanel';
import AutoMemoryPanel from './components/memory/AutoMemoryPanel';
import MemoryContextPanel from './components/memory/MemoryContextPanel';
import PatternPanel from './components/memory/PatternPanel';
import CursorApiPanel from './components/cursor/CursorApiPanel';
import CursorProjectPanel from './components/cursor/CursorProjectPanel';
import CursorFileChangesPanel from './components/cursor/CursorFileChangesPanel';
import CursorAgentHistoryPanel from './components/cursor/CursorAgentHistoryPanel';
import './App.css';

function App() {
  const [workflows, setWorkflows] = useState<WorkflowDefinition[]>([]);
  const [metrics, setMetrics] = useState<Map<string, DashboardMetrics>>(new Map());
  const [systemInfo, setSystemInfo] = useState<any>(null);
  const [logs, setLogs] = useState<Array<{ workflow: string; level: string; message: string; timestamp: string }>>([]);
  const [mcpStatus, setMcpStatus] = useState<{ running: boolean; pid?: number }>({ running: false });

  // Load initial data
  useEffect(() => {
    loadWorkflows();
    loadActiveWorkflows();
    loadSystemInfo();
    loadMcpStatus();
    
    const interval = setInterval(() => {
      loadSystemInfo();
      loadMcpStatus();
      loadActiveWorkflows(); // Reload active workflows to get updated details
    }, 5000);

    return () => clearInterval(interval);
  }, []);

  const loadWorkflows = async () => {
    try {
      // Load available workflows
      const wfs = await api.getWorkflows();
      
      // For each workflow, load full details including YAML
      const workflowsWithDetails = await Promise.all(
        wfs.map(async (wf) => {
          try {
            const fullDetails = await api.getWorkflow(wf.name);
            // Ensure steps is always an array and normalize all data
            return {
              name: fullDetails.name || wf.name,
              description: fullDetails.description || wf.description,
              steps: Array.isArray(fullDetails.steps) ? fullDetails.steps : (fullDetails.steps ? [fullDetails.steps] : []),
              agents: Array.isArray(fullDetails.agents) ? fullDetails.agents : (fullDetails.agents ? [fullDetails.agents] : []),
              metadata: fullDetails.metadata && typeof fullDetails.metadata === 'object' && !Array.isArray(fullDetails.metadata) ? fullDetails.metadata : {},
              yamlPath: fullDetails.yamlPath,
              yamlContent: fullDetails.yamlContent,
            };
          } catch (error) {
            // If loading full details fails, use basic workflow with empty arrays
            return {
              name: wf.name,
              description: wf.description,
              steps: [],
              agents: [],
              metadata: {},
            };
          }
        })
      );
      
      setWorkflows(workflowsWithDetails);
      
      // Load metrics for each workflow
      const allMetrics = await api.getAllMetrics();
      const metricsMap = new Map<string, DashboardMetrics>();
      allMetrics.forEach(m => {
        metricsMap.set(m.workflowName, m);
      });
      setMetrics(metricsMap);
    } catch (error) {
      console.error('Failed to load workflows:', error);
    }
  };

  const loadSystemInfo = async () => {
    try {
      const info = await api.getSystemInfo();
      setSystemInfo(info);
    } catch (error) {
      console.error('Failed to load system info:', error);
    }
  };

  const loadMcpStatus = async () => {
    try {
      const status = await api.getMcpStatus();
      setMcpStatus(status);
    } catch (error) {
      console.error('Failed to load MCP status:', error);
    }
  };

  // Load active workflows with full details
  const loadActiveWorkflows = async () => {
    try {
      const response = await api.getActiveWorkflows();
      const activeWorkflows = response.workflows;
      
      // Update metrics
      const metricsMap = new Map<string, DashboardMetrics>();
      activeWorkflows.forEach(w => {
        metricsMap.set(w.workflowName, w);
      });
      setMetrics(metricsMap);
      
      // Update workflows list with full details from active workflows
      // Merge active workflow details with available workflows
      setWorkflows(prev => {
        const updated = new Map(prev.map(w => [w.name, w]));
        activeWorkflows.forEach(active => {
          const existing = updated.get(active.workflowName);
          if (existing) {
            // Merge active workflow details (metadata, agents, steps) with existing
            // Always prefer YAML-loaded data over basic workflow data
            updated.set(active.workflowName, {
              ...existing,
              name: active.workflowName || existing.name,
              description: active.description || existing.description,
              metadata: active.metadata || existing.metadata || {},
              agents: active.agents || existing.agents || [],
              steps: active.steps || existing.steps || [], // Ensure steps is always an array
              yamlPath: active.yamlPath || existing.yamlPath,
              yamlContent: active.yamlContent || existing.yamlContent,
            });
          } else {
            // Add new active workflow
            updated.set(active.workflowName, {
              name: active.workflowName,
              description: active.description,
              metadata: active.metadata || {},
              agents: active.agents || [],
              steps: active.steps || [], // Ensure steps is always an array
              yamlPath: active.yamlPath,
              yamlContent: active.yamlContent,
            });
          }
        });
        return Array.from(updated.values());
      });
    } catch (error) {
      console.error('Failed to load active workflows:', error);
    }
  };

  // WebSocket message handler
  const handleWebSocketMessage = (message: DashboardMessage) => {
    switch (message.type) {
      case 'metrics':
      case 'workflow-start':
      case 'step-update':
      case 'workflow-end':
        if (message.data?.metrics) {
          setMetrics(prev => {
            const next = new Map(prev);
            next.set(message.data.metrics.workflowName, message.data.metrics);
            return next;
          });
          // Reload active workflows to get full details
          loadActiveWorkflows();
        }
        break;
      case 'log':
        setLogs(prev => {
          const newLogs = [...prev, message.data];
          // Keep only last 1000 logs
          return newLogs.slice(-1000);
        });
        break;
    }
  };

  // Determine WebSocket URL - use same origin if available, otherwise default to port 3000
  const wsProtocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
  const wsHost = window.location.hostname;
  const wsPort = window.location.port || '3000';
  const wsUrl = `${wsProtocol}//${wsHost}:${wsPort}/ws`;
  
  const { isConnected } = useWebSocket({
    url: wsUrl,
    onMessage: handleWebSocketMessage,
  });

  return (
    <div className="app">
      <Header isConnected={isConnected} />
      
      <div className="controls">
        <h2>Workflow Controls</h2>
        <WorkflowControls 
          workflows={workflows} 
          onWorkflowStart={loadWorkflows}
        />
      </div>

      <WorkflowList workflows={workflows} metrics={metrics} />

      {systemInfo && <SystemInfo info={systemInfo} />}

      <McpServerPanel status={mcpStatus} onStatusChange={loadMcpStatus} />

      <LogViewer logs={logs} />

      <div className="memory-section">
        <h2>Memory Management</h2>
        <AutoMemoryPanel />
        <MemoryContextPanel />
        <PatternPanel />
      </div>

      <div className="cursor-section">
        <h2>Cursor Integration</h2>
        <CursorApiPanel />
        <CursorProjectPanel />
        <CursorFileChangesPanel />
        <CursorAgentHistoryPanel />
      </div>
    </div>
  );
}

export default App;
