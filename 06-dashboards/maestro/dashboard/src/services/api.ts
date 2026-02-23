import type { WorkflowDefinition } from '../types/workflow';
import type { DashboardMetrics, SystemInfo } from '../types/metrics';

const API_BASE = '/api';

async function fetchJson<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(url, options);
  if (!response.ok) {
    throw new Error(`API error: ${response.statusText}`);
  }
  return response.json();
}

export const api = {
  // Workflow endpoints
  getWorkflows: (): Promise<WorkflowDefinition[]> => 
    fetchJson<WorkflowDefinition[]>(`${API_BASE}/workflows`),
  
  getWorkflow: (name: string): Promise<WorkflowDefinition> => 
    fetchJson<WorkflowDefinition>(`${API_BASE}/workflows/${encodeURIComponent(name)}`),
  
  startWorkflow: (name: string, modelId?: string): Promise<{ executionId: string }> => 
    fetchJson<{ executionId: string }>(`${API_BASE}/workflows/${encodeURIComponent(name)}/start`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ modelId }),
    }),
  
  stopWorkflow: (name: string, executionId?: string): Promise<void> => 
    fetch(`${API_BASE}/workflows/${encodeURIComponent(name)}/stop`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ executionId }),
    }).then(() => undefined),
  
  getWorkflowMetrics: (name: string): Promise<DashboardMetrics> => 
    fetchJson<DashboardMetrics>(`${API_BASE}/workflows/${encodeURIComponent(name)}/metrics`),
  
  getAllMetrics: (): Promise<DashboardMetrics[]> => 
    fetchJson<DashboardMetrics[]>(`${API_BASE}/metrics`),
  
  // Get active workflows with full details
  getActiveWorkflows: (): Promise<{ workflows: Array<DashboardMetrics & Partial<WorkflowDefinition>> }> => 
    fetchJson<{ workflows: Array<DashboardMetrics & Partial<WorkflowDefinition>> }>(`${API_BASE}/workflows/active`),
  
  // System info
  getSystemInfo: (): Promise<SystemInfo> => 
    fetchJson<SystemInfo>(`${API_BASE}/system/info`),
  
  // MCP server
  getMcpStatus: (): Promise<{ running: boolean; pid?: number }> => 
    fetchJson<{ running: boolean; pid?: number }>(`${API_BASE}/mcp/status`),
  
  getMcpInfo: (): Promise<{ running: boolean; port?: number; resources: any[]; tools: any[]; resourceCount: number; toolCount: number; health: string; baseUrl?: string }> => 
    fetchJson<{ running: boolean; port?: number; resources: any[]; tools: any[]; resourceCount: number; toolCount: number; health: string; baseUrl?: string }>(`${API_BASE}/mcp/info`),
  
  startMcpServer: (): Promise<{ success: boolean; message: string }> => 
    fetchJson<{ success: boolean; message: string }>(`${API_BASE}/mcp/start`, { method: 'POST' }),
  
  stopMcpServer: (): Promise<{ success: boolean; message: string }> => 
    fetchJson<{ success: boolean; message: string }>(`${API_BASE}/mcp/stop`, { method: 'POST' }),
  
  // Models - returns ModelInfo[] (objects with id, name, etc.)
  getModels: (): Promise<any[]> => 
    fetchJson<any[]>(`${API_BASE}/models`),
  
  // Agents
  getAgents: (): Promise<string[]> => 
    fetchJson<string[]>(`${API_BASE}/agents`),
  
  // Workflow generation
  generateWorkflow: (prompt: string): Promise<WorkflowDefinition> => 
    fetchJson<WorkflowDefinition>(`${API_BASE}/workflows/generate`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ prompt }),
    }),
  
  // Workflow creation
  createWorkflow: (workflow: WorkflowDefinition): Promise<{ success: boolean }> => 
    fetchJson<{ success: boolean }>(`${API_BASE}/workflows`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(workflow),
    }),
  
  // Workflow update
  updateWorkflow: (name: string, workflow: Partial<WorkflowDefinition>): Promise<{ success: boolean }> => 
    fetchJson<{ success: boolean }>(`${API_BASE}/workflows/${encodeURIComponent(name)}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(workflow),
    }),
  
  // Workflow deletion
  deleteWorkflow: (name: string): Promise<{ success: boolean }> => 
    fetchJson<{ success: boolean }>(`${API_BASE}/workflows/${encodeURIComponent(name)}`, {
      method: 'DELETE',
    }),

  // Workflow reevaluation (scrum master reassignment)
  reevaluateWorkflow: (name: string): Promise<{ success: boolean; message: string }> => 
    fetchJson<{ success: boolean; message: string }>(`${API_BASE}/workflows/${encodeURIComponent(name)}/reevaluate`, {
      method: 'POST',
    }),

  // Workflow backlog
  getWorkflowBacklog: (name: string): Promise<{ backlog: Array<{ stepName: string; taskName?: string; priority: number; description?: string; estimatedEffort?: number; dependencies?: string[]; addedAt: string }> }> => 
    fetchJson<{ backlog: Array<{ stepName: string; taskName?: string; priority: number; description?: string; estimatedEffort?: number; dependencies?: string[]; addedAt: string }> }>(`${API_BASE}/workflows/${encodeURIComponent(name)}/backlog`),

  updateBacklogPriority: (name: string, stepName: string, taskName: string | undefined, priority: number): Promise<{ success: boolean }> => 
    fetchJson<{ success: boolean }>(`${API_BASE}/workflows/${encodeURIComponent(name)}/backlog/priority`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ stepName, taskName, priority }),
    }),

  // Memory endpoints
  addMemory: (content: string, type?: string, role?: string, tags?: string[], isPublic?: boolean): Promise<{ success: boolean; file_path: string }> =>
    fetchJson<{ success: boolean; file_path: string }>(`${API_BASE}/memory/add`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ content, type, role, tags, public: isPublic }),
    }),

  searchMemory: (query: string, topK?: number, memoryType?: string): Promise<{ results: any[] }> =>
    fetchJson<{ results: any[] }>(`${API_BASE}/memory/search?q=${encodeURIComponent(query)}&top_k=${topK || 10}${memoryType ? `&type=${memoryType}` : ''}`),

  loadContext: (trigger: string, files?: string[]): Promise<any> =>
    fetchJson<any>(`${API_BASE}/memory/context?trigger=${encodeURIComponent(trigger)}${files ? `&files=${files.join(',')}` : ''}`),

  getPatterns: (query?: string): Promise<{ patterns: any[] }> =>
    fetchJson<{ patterns: any[] }>(`${API_BASE}/memory/patterns${query ? `?q=${encodeURIComponent(query)}` : ''}`),

  extractPatterns: (): Promise<{ success: boolean; patterns: any[]; count: number }> =>
    fetchJson<{ success: boolean; patterns: any[]; count: number }>(`${API_BASE}/memory/patterns/extract`, {
      method: 'POST',
    }),

  getSessions: (limit?: number): Promise<{ sessions: any[] }> =>
    fetchJson<{ sessions: any[] }>(`${API_BASE}/memory/sessions${limit ? `?limit=${limit}` : ''}`),

  getLatestSession: (): Promise<{ session: any }> =>
    fetchJson<{ session: any }>(`${API_BASE}/memory/sessions/latest`),

  saveSession: (content: string, metadata?: Record<string, any>): Promise<{ success: boolean; file_path: string }> =>
    fetchJson<{ success: boolean; file_path: string }>(`${API_BASE}/memory/session`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ content, metadata }),
    }),

  getKnowledgeGraph: (nodeId?: string, maxResults?: number): Promise<any> =>
    fetchJson<any>(`${API_BASE}/memory/graph${nodeId ? `?node_id=${encodeURIComponent(nodeId)}&max_results=${maxResults || 10}` : ''}`),

  buildKnowledgeGraph: (): Promise<{ success: boolean; message: string }> =>
    fetchJson<{ success: boolean; message: string }>(`${API_BASE}/memory/graph/build`, {
      method: 'POST',
    }),

  // Cursor API endpoints
  setCursorApiKey: (apiKey: string): Promise<{ success: boolean; message: string }> =>
    fetchJson<{ success: boolean; message: string }>(`${API_BASE}/cursor/api-key`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ api_key: apiKey }),
    }),

  getCursorApiKey: (): Promise<{ configured: boolean; api_key?: string }> =>
    fetchJson<{ configured: boolean; api_key?: string }>(`${API_BASE}/cursor/api-key`),

  getCursorProject: (): Promise<{ project: any; stats: any }> =>
    fetchJson<{ project: any; stats: any }>(`${API_BASE}/cursor/project`),

  getCursorFileChanges: (limit?: number): Promise<{ changes: any[] }> =>
    fetchJson<{ changes: any[] }>(`${API_BASE}/cursor/files?limit=${limit || 50}`),

  getCursorFileHistory: (filePath: string, limit?: number): Promise<{ history: any[] }> =>
    fetchJson<{ history: any[] }>(`${API_BASE}/cursor/files/${encodeURIComponent(filePath)}/history?limit=${limit || 20}`),

  getCursorFileDiff: (filePath: string, commit1?: string, commit2?: string): Promise<{ diff: string }> =>
    fetchJson<{ diff: string }>(`${API_BASE}/cursor/files/${encodeURIComponent(filePath)}/diff${commit1 ? `?commit1=${commit1}${commit2 ? `&commit2=${commit2}` : ''}` : ''}`),

  getCursorChangesSummary: (days?: number): Promise<{ summary: any }> =>
    fetchJson<{ summary: any }>(`${API_BASE}/cursor/files/summary?days=${days || 7}`),

  getCursorAgents: (): Promise<{ agents: any[]; stats: any }> =>
    fetchJson<{ agents: any[]; stats: any }>(`${API_BASE}/cursor/agents`),

  getCursorAgent: (agentId: string, limit?: number): Promise<{ agent: any; activity: any[] }> =>
    fetchJson<{ agent: any; activity: any[] }>(`${API_BASE}/cursor/agents/${encodeURIComponent(agentId)}?limit=${limit || 50}`),

  getCursorHealth: (): Promise<{ status: string; message?: string }> =>
    fetchJson<{ status: string; message?: string }>(`${API_BASE}/cursor/health`),
};
