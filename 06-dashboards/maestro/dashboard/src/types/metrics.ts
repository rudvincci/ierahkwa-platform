export interface TokenUsage {
  inputTokens: number;
  outputTokens: number;
  totalTokens: number;
  contextWindowSize: number;
  contextWindowUsed: number;
  contextWindowPercent: number;
  model: string;
  timestamp: Date | string;
  stepName: string;
  workflowName: string;
  cost?: number;
  cacheWriteTokens?: number;
  cacheReadTokens?: number;
}

export interface TaskProgress {
  stepName: string;
  workflowName: string;
  completionPercent: number;
  status: 'pending' | 'running' | 'completed' | 'failed' | 'needs-update';
  estimatedTimeRemaining?: number;
  issues: string[];
  subTasks: string[];
  completedSubTasks: string[];
  lastUpdate: Date | string;
}

export interface OutputAnalysis {
  stepName: string;
  workflowName: string;
  completionPercent: number;
  issues: string[];
  suggestions: string[];
  needsReAlignment: boolean;
  recommendedAgent?: string;
  recommendedModel?: string;
  confidence: number;
}

export interface DashboardMetrics {
  workflowName: string;
  executionId?: string;
  cursorAgentGuid?: string; // GUID for cursor-agent --resume to maintain context
  startTime: Date | string;
  endTime?: Date | string;
  duration: number;
  totalSteps: number;
  completedSteps: number;
  failedSteps: number;
  successRate: number;
  currentStep?: string;
  currentActivity?: string;
  status: 'running' | 'completed' | 'failed' | 'paused';
  cacheHits: number;
  cacheMisses: number;
  retryAttempts: number;
  currentTokenUsage?: TokenUsage;
  totalTokens?: number;
  contextWindowPercent?: number;
  totalCost?: number;
  costPerStep?: Record<string, number>;
  currentModel?: string;
  modelHistory?: Array<{ timestamp: Date | string; model: string; stepName?: string }>;
  currentAgent?: string;
  agentSwitches?: Array<{ timestamp: Date | string; stepName: string; fromAgent: string; toAgent: string }>;
  agentGuids?: Record<string, string>; // Map of agent name to cursor-agent resume GUID
  taskProgress?: Record<string, TaskProgress>;
  latestAnalysis?: OutputAnalysis;
  processStatus?: { running: boolean; pid?: number; exitCode?: number | null; cursorAgentRunning?: boolean };
}

export interface SystemInfo {
  processId: number;
  uptime: number; // seconds
  memory: {
    used: number; // bytes
    total: number; // bytes
    percentage: number; // 0-100
    heapUsed: number; // bytes (Node.js heap)
    heapTotal: number; // bytes (Node.js heap)
    heapLimit: number; // bytes (configured max heap size)
    external: number; // bytes (external memory)
    rss?: number;
    arrayBuffers?: number;
  };
  cpu: {
    usage: number; // percentage (0-100)
    count: number; // number of CPU cores
    model: string; // CPU model
    speed?: number;
  };
  network?: {
    bytesSent: number;
    bytesReceived: number;
    connections: number;
  };
  disk?: {
    readOps: number;
    writeOps: number;
    spaceUsed: number;
    spaceTotal: number;
    spaceFree: number;
  };
  processTree?: Array<{
    pid: number;
    name: string;
    ppid: number;
    cpu: number;
    memory: number;
    command: string;
  }>;
  eventLoop?: {
    lag: number;
    utilization: number;
  };
  activeHandles?: number;
  activeRequests?: number;
  platform: {
    type: string; // 'darwin', 'linux', 'win32', etc.
    arch: string; // 'x64', 'arm64', etc.
    hostname: string;
    nodeVersion: string;
    platform?: string;
    release?: string;
  };
  environment?: Record<string, string>;
  timestamp: Date | string;
}
