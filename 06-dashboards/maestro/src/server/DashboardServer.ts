/**
 * Real-time Monitoring Dashboard Server
 * 
 * Provides web server and WebSocket for real-time workflow monitoring.
 */

import express, { Request, Response } from 'express';
import * as http from 'http';
import * as path from 'path';
import * as fs from 'fs';
import * as mimeTypes from 'mime-types';
import { WebSocketServer, WebSocket } from 'ws';
import { ExecutionResult } from '../workflow/EnhancedWorkflowExecutor';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';
import { ReportingService } from '../services/ReportingService';
import { StateManager, MemoryStateStorage } from '../services/StateManager';
import { ActivityTracker } from '../services/ActivityTracker';
import { WorkflowManager } from '../services/WorkflowManager';
import { WorkflowCreator } from '../services/WorkflowCreator';
import { WorkflowGenerator } from '../services/WorkflowGenerator';
import { TokenUsageTracker, TokenUsage } from '../services/TokenUsageTracker';
import { ModelManager } from '../services/ModelManager';
import { AgentSwitcher } from '../services/AgentSwitcher';
import { OutputMonitor, OutputAnalysis } from '../services/OutputMonitor';
import { TaskProgressTracker, TaskProgress } from '../services/TaskProgressTracker';
import { SystemInfoService } from '../services/SystemInfo';
import { AgentCleanupService } from '../services/AgentCleanupService';
import { findRepoRoot } from '../cli/utils';
import { MemoryService } from '../services/memory/MemoryService';
import { ContextLoader } from '../services/memory/claude/ContextLoader';
import { PatternExtractor } from '../services/memory/claude/PatternExtractor';
import { SessionManager } from '../services/memory/claude/SessionManager';
import { KnowledgeGraph } from '../services/memory/claude/KnowledgeGraph';
import { CursorApiClient } from '../services/cursor/CursorApiClient';
import { CursorProjectService } from '../services/cursor/CursorProjectService';
import { CursorFileService } from '../services/cursor/CursorFileService';
import { CursorAgentService } from '../services/cursor/CursorAgentService';

export interface DashboardMetrics {
  workflowName: string;
  executionId?: string; // Execution ID for stopping workflows
  cursorAgentGuid?: string; // GUID for cursor-agent --resume to maintain context
  startTime: Date;
  endTime?: Date;
  duration: number;
  totalSteps: number;
  completedSteps: number;
  failedSteps: number;
  successRate: number;
  currentStep?: string;
  currentActivity?: string; // What cursor-agent is currently doing
  status: 'running' | 'completed' | 'failed' | 'paused';
  cacheHits: number;
  cacheMisses: number;
  retryAttempts: number;
  // Token and context tracking
  currentTokenUsage?: TokenUsage;
  totalTokens?: number;
  contextWindowPercent?: number;
  totalCost?: number; // Total estimated cost in USD
  costPerStep?: Map<string, number>; // Cost per step
  // Model tracking
  currentModel?: string;
  modelHistory?: Array<{ timestamp: Date; model: string; stepName?: string }>;
  // Agent tracking
  currentAgent?: string;
  agentSwitches?: Array<{ timestamp: Date; stepName: string; fromAgent: string; toAgent: string }>;
  agentGuids?: Record<string, string>; // Map of agent name to cursor-agent resume GUID
  // Task progress
  taskProgress?: Map<string, TaskProgress>;
  // Output analysis
  latestAnalysis?: OutputAnalysis;
  // Process status
  processStatus?: { running: boolean; pid?: number; exitCode?: number | null; cursorAgentRunning?: boolean };
}

export interface DashboardMessage {
  type: 'metrics' | 'workflow-start' | 'workflow-end' | 'step-update' | 'activity-update' | 'error' | 'checkpoint' | 'token-update' | 'model-change' | 'agent-switch' | 'progress-update' | 'log';
  data: any;
  timestamp: Date;
}

/**
 * Dashboard Server
 */
export class DashboardServer {
  private app: express.Application;
  private server: http.Server;
  private wss: WebSocketServer;
  private port: number;
  private clients: Set<WebSocket> = new Set();
  private activeWorkflows: Map<string, DashboardMetrics> = new Map();
  private dashboardDistDir: string | null = null;
  private stepStates: Map<string, Map<string, 'running' | 'completed' | 'failed'>> = new Map(); // Track step states per workflow
  private workflowLogs: Map<string, Array<{ timestamp: Date; level: 'info' | 'warn' | 'error' | 'debug'; message: string }>> = new Map(); // Store logs per workflow
  private maxLogsPerWorkflow: number = 1000; // Maximum logs to keep per workflow
  private reportingService: ReportingService;
  private stateManager: StateManager;
  private activityTracker: ActivityTracker;
  private workflowManager: WorkflowManager;
  private workflowCreator: WorkflowCreator;
  private workflowGenerator: WorkflowGenerator;
  private tokenUsageTracker: TokenUsageTracker;
  private modelManager: ModelManager;
  private agentSwitcher: AgentSwitcher;
  private outputMonitor: OutputMonitor;
  private taskProgressTracker: TaskProgressTracker;
  private systemInfoService: SystemInfoService;
  private agentCleanupService!: AgentCleanupService; // Initialized in initializeAgentCleanup
  private memoryService?: MemoryService;
  private contextLoader?: ContextLoader;
  private patternExtractor?: PatternExtractor;
  private sessionManager?: SessionManager;
  private knowledgeGraph?: KnowledgeGraph;
  private cursorApiClient?: CursorApiClient;
  private cursorProjectService?: CursorProjectService;
  private cursorFileService?: CursorFileService;
  private cursorAgentService?: CursorAgentService;
  private maxWorkflows: number = 20; // Maximum workflows to keep in memory (reduced to prevent memory issues)
  private maxWorkflowAge: number = 10 * 60 * 1000; // 10 minutes (reduced from 30)
  private cleanupInterval?: NodeJS.Timeout;
  private updateInterval?: NodeJS.Timeout; // For real-time duration updates

  constructor(port: number = 3000, repositoryRoot: string = process.cwd()) {
    this.port = port;
    this.app = express();
    
    // Set up CORS FIRST, before creating server
    this.app.use((req: Request, res: Response, next: express.NextFunction) => {
      const origin = req.headers.origin;
      if (origin) {
        res.header('Access-Control-Allow-Origin', origin);
        res.header('Access-Control-Allow-Credentials', 'true');
      } else {
        res.header('Access-Control-Allow-Origin', '*');
      }
      res.header('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS, PATCH');
      res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept, Authorization');
      res.header('Access-Control-Expose-Headers', 'Content-Length, Content-Type');
      if (req.method === 'OPTIONS') {
        res.sendStatus(200);
        return;
      }
      next();
    });
    
    // Enable JSON body parsing
    this.app.use(express.json());
    this.app.use(express.urlencoded({ extended: true }));
    
    this.server = http.createServer(this.app);
    // WebSocket server - handle /ws path
    // We'll handle the upgrade manually to ensure Express doesn't intercept it
    this.wss = new WebSocketServer({ noServer: true });
    
    // Handle WebSocket upgrade requests manually
    // This MUST be set up before the server starts listening AND before Express routes
    // We need to handle this on the raw HTTP server, not through Express
    this.server.on('upgrade', (request: http.IncomingMessage, socket: any, head: Buffer) => {
      const pathname = request.url?.split('?')[0] || '';
      
      console.log(`📊 Dashboard: Upgrade request received for ${pathname} from ${request.socket.remoteAddress || 'unknown'}`);
      
      if (pathname === '/ws' || pathname.startsWith('/ws')) {
        console.log(`📊 Dashboard: Accepting WebSocket upgrade for ${pathname}`);
        
        // Prevent socket from being destroyed
        socket.on('error', (err: Error) => {
          console.error(`📊 Dashboard: Socket error during upgrade:`, err);
        });
        
        try {
          this.wss.handleUpgrade(request, socket, head, (ws: WebSocket) => {
            console.log(`📊 Dashboard: WebSocket upgrade successful, emitting connection event`);
            // Add the connection to the set and emit the connection event
            this.wss.emit('connection', ws, request);
          });
        } catch (error: any) {
          console.error(`📊 Dashboard: Error handling WebSocket upgrade:`, error);
          console.error(`📊 Dashboard: Error stack:`, error.stack);
          if (!socket.destroyed) {
            socket.write('HTTP/1.1 500 Internal Server Error\r\n\r\n');
            socket.destroy();
          }
        }
      } else {
        // Reject other upgrade requests with proper HTTP response
        console.log(`📊 Dashboard: Rejecting upgrade request for ${pathname}`);
        socket.write('HTTP/1.1 404 Not Found\r\n\r\n');
        socket.destroy();
      }
    });
    this.reportingService = new ReportingService(repositoryRoot);
    
    const stateStorage = new MemoryStateStorage(repositoryRoot);
    this.stateManager = new StateManager(stateStorage);
    this.activityTracker = new ActivityTracker();
    this.workflowManager = new WorkflowManager();

    // Initialize memory services
    try {
      const memoryRoot = path.join(repositoryRoot, '.maestro', 'memory');
      if (!fs.existsSync(memoryRoot)) {
        fs.mkdirSync(memoryRoot, { recursive: true });
      }
      this.memoryService = new MemoryService(memoryRoot, true);
      this.contextLoader = new ContextLoader(memoryRoot);
      this.patternExtractor = new PatternExtractor(memoryRoot);
      this.sessionManager = new SessionManager(memoryRoot);
      this.knowledgeGraph = new KnowledgeGraph(memoryRoot);
    } catch (error: any) {
      console.warn('⚠️  Failed to initialize memory services:', error);
    }

    // Initialize Cursor API services
    try {
      this.cursorApiClient = new CursorApiClient(repositoryRoot);
      this.cursorProjectService = new CursorProjectService(this.cursorApiClient);
      this.cursorFileService = new CursorFileService(this.cursorApiClient);
      this.cursorAgentService = new CursorAgentService(this.cursorApiClient);
    } catch (error: any) {
      console.warn('⚠️  Failed to initialize Cursor API services:', error);
    }
    
    // Set up log callback to forward workflow logs to dashboard
    this.workflowManager.setLogCallback((workflowName, level, message) => {
      this.recordLog(workflowName, level, message);
    });
    // Set dashboard server reference in workflow manager for log forwarding
    (this.workflowManager as any).setDashboardServer(this);
    this.workflowCreator = new WorkflowCreator();
    this.workflowGenerator = new WorkflowGenerator();
    this.modelManager = new ModelManager();
    this.tokenUsageTracker = new TokenUsageTracker(this.modelManager);
    this.agentSwitcher = new AgentSwitcher();
    this.outputMonitor = new OutputMonitor();
    this.taskProgressTracker = new TaskProgressTracker();
    this.systemInfoService = new SystemInfoService();
    
    // Initialize agent cleanup service (async, but don't await - it will start in background)
    this.initializeAgentCleanup().catch(err => {
      console.error('🧹 Agent Cleanup Service: Failed to initialize:', err);
    });

    // Setup API routes first (they need CORS)
    this.setupApiRoutes();
    // Then setup static file serving
    this.setupRoutes();
    this.setupWebSocket();
    this.startCleanup();
    this.startRealTimeUpdates();
  }

  /**
   * Initialize agent cleanup service
   */
  private async initializeAgentCleanup(): Promise<void> {
    try {
      const { ConfigLoader } = await import('../config/OrchestratorConfig');
      const config = await ConfigLoader.load();
      
      if (config.agentCleanup) {
        this.agentCleanupService = new AgentCleanupService(config.agentCleanup);
        if (config.agentCleanup.enabled) {
          this.agentCleanupService.start();
          console.log('🧹 Agent Cleanup Service: Initialized and started');
        }
      } else {
        // Use defaults
        this.agentCleanupService = new AgentCleanupService();
        this.agentCleanupService.start();
        console.log('🧹 Agent Cleanup Service: Initialized with defaults');
      }
    } catch (error) {
      console.error('🧹 Agent Cleanup Service: Failed to initialize:', error);
      // Create with defaults even if config load fails
      this.agentCleanupService = new AgentCleanupService();
      this.agentCleanupService.start();
    }
  }

  /**
   * Start periodic cleanup
   */
  private startCleanup(): void {
    // Cleanup every 2 minutes (more frequent to prevent memory buildup)
    this.cleanupInterval = setInterval(() => {
      this.cleanupOldWorkflows();
      this.cleanupDisconnectedClients();
      this.cleanupOrphanedData(); // Clean up data for workflows that no longer exist
      this.checkMemoryAndCleanup();
      this.forceGarbageCollection();
    }, 2 * 60 * 1000);
  }

  /**
   * Clean up orphaned data (data for workflows that no longer exist)
   */
  private cleanupOrphanedData(): void {
    const activeWorkflowNames = new Set(this.activeWorkflows.keys());
    let cleanedCount = 0;

    // Clean up activity tracking for non-existent workflows
    const activityWorkflows = this.activityTracker.getActiveWorkflows();
    for (const workflowName of activityWorkflows) {
      if (!activeWorkflowNames.has(workflowName)) {
        this.activityTracker.clearWorkflow(workflowName);
        cleanedCount++;
      }
    }

    // Clean up token usage for non-existent workflows
    if (this.tokenUsageTracker && typeof (this.tokenUsageTracker as any).getWorkflowNames === 'function') {
      const tokenWorkflows = (this.tokenUsageTracker as any).getWorkflowNames();
      for (const workflowName of tokenWorkflows) {
        if (!activeWorkflowNames.has(workflowName)) {
          this.clearTokenUsage(workflowName);
          cleanedCount++;
        }
      }
    }

    // Clean up output analyses for non-existent workflows
    if (this.outputMonitor && typeof (this.outputMonitor as any).getWorkflowNames === 'function') {
      const analysisWorkflows = (this.outputMonitor as any).getWorkflowNames();
      for (const workflowName of analysisWorkflows) {
        if (!activeWorkflowNames.has(workflowName)) {
          this.clearOutputAnalyses(workflowName);
          cleanedCount++;
        }
      }
    }

    // Clean up task progress for non-existent workflows
    if (this.taskProgressTracker && typeof (this.taskProgressTracker as any).getWorkflowNames === 'function') {
      const progressWorkflows = (this.taskProgressTracker as any).getWorkflowNames();
      for (const workflowName of progressWorkflows) {
        if (!activeWorkflowNames.has(workflowName)) {
          this.clearTaskProgress(workflowName);
          cleanedCount++;
        }
      }
    }

    if (cleanedCount > 0) {
      console.log(`🧹 Cleaned up ${cleanedCount} orphaned data entries`);
    }
  }

  /**
   * Check memory usage and perform aggressive cleanup if needed
   */
  private checkMemoryAndCleanup(): void {
    const memUsage = process.memoryUsage();
    const heapUsedMB = memUsage.heapUsed / 1024 / 1024;
    const heapTotalMB = memUsage.heapTotal / 1024 / 1024;
    const heapLimitMB = this.systemInfoService.getSystemInfo().memory.heapLimit / 1024 / 1024;
    const heapPercent = (memUsage.heapUsed / this.systemInfoService.getSystemInfo().memory.heapLimit) * 100;
    const externalMB = memUsage.external / 1024 / 1024;
    const rssMB = memUsage.rss / 1024 / 1024;

    // Log memory usage (including external and RSS which contribute to compressed memory)
    console.log(`💾 Memory: Heap ${heapUsedMB.toFixed(1)}MB / ${heapLimitMB.toFixed(1)}MB (${heapPercent.toFixed(1)}%) | External: ${externalMB.toFixed(1)}MB | RSS: ${rssMB.toFixed(1)}MB`);

    // Check both heap and RSS (RSS includes external memory which contributes to compressed memory)
    const rssPercent = (memUsage.rss / this.systemInfoService.getSystemInfo().memory.heapLimit) * 100;
    const memoryPressure = Math.max(heapPercent, rssPercent);

    // If memory usage is above 80%, perform aggressive cleanup
    if (memoryPressure > 80) {
      console.log(`⚠️  High memory usage (Heap: ${heapPercent.toFixed(1)}%, RSS: ${rssPercent.toFixed(1)}%), performing aggressive cleanup...`);
      
      // First, clean up orphaned data
      this.cleanupOrphanedData();
      
      // Remove all completed/failed workflows immediately
      const now = Date.now();
      const toRemove: string[] = [];
      
      for (const [name, metrics] of this.activeWorkflows.entries()) {
        if (metrics.status === 'completed' || metrics.status === 'failed') {
          toRemove.push(name);
        } else if (metrics.status === 'running') {
          // Remove running workflows older than 1 hour if memory is high
          const age = now - metrics.startTime.getTime();
          if (age > 60 * 60 * 1000) {
            toRemove.push(name);
          }
        }
      }

      // If still high, remove oldest workflows regardless of status
      if (memoryPressure > 90 && this.activeWorkflows.size > 5) {
        const sorted = Array.from(this.activeWorkflows.entries())
          .sort((a, b) => {
            const aTime = a[1].endTime?.getTime() || a[1].startTime.getTime();
            const bTime = b[1].endTime?.getTime() || b[1].startTime.getTime();
            return aTime - bTime;
          });
        
        // Keep only the 5 most recent workflows
        const keepCount = 5;
        for (let i = 0; i < sorted.length - keepCount; i++) {
          toRemove.push(sorted[i][0]);
        }
      }

      for (const name of toRemove) {
        this.activeWorkflows.delete(name);
        this.stepStates.delete(name);
        this.activityTracker.clearWorkflow(name);
        this.clearTokenUsage(name);
        this.clearOutputAnalyses(name);
        this.clearTaskProgress(name);
      }

      if (toRemove.length > 0) {
        console.log(`🧹 Aggressive cleanup: Removed ${toRemove.length} workflow(s) (${this.activeWorkflows.size} remaining)`);
      }

      // Also clean up old activity/token/analysis data for remaining workflows
      this.trimOldDataForWorkflows();

      // Force garbage collection
      this.forceGarbageCollection();
    }
  }

  /**
   * Force garbage collection if available (Node.js with --expose-gc flag)
   */
  private forceGarbageCollection(): void {
    if (global.gc) {
      try {
        global.gc();
        console.log('🧹 Dashboard: Forced garbage collection');
      } catch (error) {
        // Ignore errors
      }
    }
  }

  /**
   * Start real-time updates for running workflows
   */
  private startRealTimeUpdates(): void {
    // Update duration and metrics every second for running workflows
    this.updateInterval = setInterval(() => {
      const now = Date.now();
      for (const [name, metrics] of this.activeWorkflows.entries()) {
        if (metrics.status === 'running') {
          // Update duration in real-time
          const oldDuration = metrics.duration;
          metrics.duration = now - metrics.startTime.getTime();
          
          // Check process status and update metrics
          const executions = this.workflowManager.getActiveExecutions();
          const execution = executions.find(e => e.workflowName === name);
          if (execution) {
            // Update executionId if missing
            if (!metrics.executionId) {
              metrics.executionId = execution.id;
            }
            
            const processStatus = this.workflowManager.getProcessStatus(execution.id);
            // Always update processStatus to ensure it's current
            metrics.processStatus = {
              running: processStatus.running,
              pid: processStatus.pid,
              exitCode: processStatus.exitCode,
              cursorAgentRunning: processStatus.cursorAgentRunning,
            };
            
            // Update status if process died
            if (!processStatus.running && metrics.status === 'running') {
              metrics.status = 'failed';
              metrics.endTime = new Date();
              console.log(`📊 Dashboard: Workflow ${name} process died, marking as failed`);
            }
            
            // Update activity if cursor-agent is not running but workflow says it's running
            if (metrics.status === 'running' && !processStatus.cursorAgentRunning && processStatus.running) {
              // Workflow process is running but cursor-agent is not - might be stuck
              if (!metrics.currentActivity || metrics.currentActivity === 'Processing...') {
                metrics.currentActivity = 'Waiting for cursor-agent to start...';
              }
            }
          } else {
            // No execution found - mark process as unknown
            metrics.processStatus = {
              running: false,
              cursorAgentRunning: false,
            };
          }
          
          // Always broadcast updates (removed the duration check to ensure real-time updates)
          this.broadcast({
            type: 'metrics',
            data: { workflow: name, metrics },
            timestamp: new Date(),
          });
        }
      }
    }, 1000); // Update every second
  }

  /**
   * Cleanup old workflows
   */
  private cleanupOldWorkflows(): void {
    const now = Date.now();
    const toRemove: string[] = [];

    for (const [name, metrics] of this.activeWorkflows.entries()) {
      // Remove completed/failed workflows older than max age
      if ((metrics.status === 'completed' || metrics.status === 'failed') && metrics.endTime) {
        const age = now - metrics.endTime.getTime();
        if (age > this.maxWorkflowAge) {
          toRemove.push(name);
        }
      }
    }

    // Remove oldest workflows if over limit (more aggressive)
    if (this.activeWorkflows.size > this.maxWorkflows) {
      const sorted = Array.from(this.activeWorkflows.entries())
        .sort((a, b) => {
          const aTime = a[1].endTime?.getTime() || a[1].startTime.getTime();
          const bTime = b[1].endTime?.getTime() || b[1].startTime.getTime();
          return aTime - bTime;
        });
      
      // Remove enough to get back to 80% of max (more aggressive cleanup)
      const targetSize = Math.floor(this.maxWorkflows * 0.8);
      const toRemoveCount = this.activeWorkflows.size - targetSize;
      for (let i = 0; i < toRemoveCount; i++) {
        toRemove.push(sorted[i][0]);
      }
    }

    // Also remove any running workflows that are very old (stale)
    for (const [name, metrics] of this.activeWorkflows.entries()) {
      if (metrics.status === 'running') {
        const age = now - metrics.startTime.getTime();
        // Remove running workflows older than 2 hours (likely stale)
        if (age > 2 * 60 * 60 * 1000) {
          toRemove.push(name);
        }
      }
    }

    for (const name of toRemove) {
      this.activeWorkflows.delete(name);
      this.stepStates.delete(name); // Also cleanup step states
      // Clean up any activity tracking for this workflow
      this.activityTracker.clearWorkflow(name);
      // Clean up token usage tracking
      this.clearTokenUsage(name);
      // Clean up output analyses
      this.clearOutputAnalyses(name);
      // Clean up task progress
      this.clearTaskProgress(name);
    }

    if (toRemove.length > 0) {
      console.log(`🧹 Dashboard: Cleaned up ${toRemove.length} old workflow(s) (${this.activeWorkflows.size} remaining)`);
    }
  }

  /**
   * Clear token usage for a workflow
   */
  private clearTokenUsage(workflowName: string): void {
    if (this.tokenUsageTracker && typeof (this.tokenUsageTracker as any).clearUsage === 'function') {
      (this.tokenUsageTracker as any).clearUsage(workflowName);
    }
  }

  /**
   * Clear output analyses for a workflow
   */
  private clearOutputAnalyses(workflowName: string): void {
    if (this.outputMonitor && typeof (this.outputMonitor as any).clearWorkflow === 'function') {
      (this.outputMonitor as any).clearWorkflow(workflowName);
    }
  }

  /**
   * Clear task progress for a workflow
   */
  private clearTaskProgress(workflowName: string): void {
    if (this.taskProgressTracker && typeof (this.taskProgressTracker as any).clearWorkflow === 'function') {
      (this.taskProgressTracker as any).clearWorkflow(workflowName);
    }
  }

  /**
   * Trim old data from tracking services to reduce memory
   */
  private trimOldDataForWorkflows(): void {
    // For each active workflow, trim old activity/token/analysis data
    for (const workflowName of this.activeWorkflows.keys()) {
      // Activity tracker already limits per workflow, but we can force trim
      const activities = (this.activityTracker as any).activities?.get(workflowName);
      if (activities && activities.length > 25) {
        // Keep only last 25 activities
        activities.splice(0, activities.length - 25);
      }

      // Token usage tracker already limits, but trim further if needed
      const tokenUsages = (this.tokenUsageTracker as any).usages?.get(workflowName);
      if (tokenUsages && tokenUsages.length > 25) {
        tokenUsages.splice(0, tokenUsages.length - 25);
      }

      // Output analyses already limits, but trim further if needed
      const analyses = (this.outputMonitor as any).analyses?.get(workflowName);
      if (analyses && analyses.length > 10) {
        analyses.splice(0, analyses.length - 10);
      }
    }
  }

  /**
   * Cleanup disconnected WebSocket clients
   */
  private cleanupDisconnectedClients(): void {
    const toRemove: WebSocket[] = [];
    
    for (const client of this.clients) {
      if (client.readyState !== WebSocket.OPEN) {
        toRemove.push(client);
      }
    }

    for (const client of toRemove) {
      this.clients.delete(client);
      try {
        client.terminate();
      } catch (error) {
        // Ignore errors
      }
    }

    if (toRemove.length > 0) {
      console.log(`🧹 Dashboard: Cleaned up ${toRemove.length} disconnected client(s)`);
    }
  }

  /**
   * Setup Express routes
   */
  private setupRoutes(): void {
    // CORS and JSON parsing are already set up in constructor
    // This method only handles static file serving

    // Middleware to set correct MIME types for static files
    this.app.use((req: Request, res: Response, next: express.NextFunction) => {
      const url = req.url;
      if (url.endsWith('.js') || url.endsWith('.mjs')) {
        res.type('application/javascript');
      } else if (url.endsWith('.css')) {
        res.type('text/css');
      } else if (url.endsWith('.json')) {
        res.type('application/json');
      } else if (url.endsWith('.html')) {
        res.type('text/html');
      } else if (url.endsWith('.wasm')) {
        res.type('application/wasm');
      }
      next();
    });

    // Serve static dashboard files from React build
    // React builds to dashboard/dist
    // Use findRepoRoot to get the correct path regardless of where the code is running from
    const repoRoot = findRepoRoot();
    const dashboardDist = path.join(repoRoot, '.maestro', 'dashboard', 'dist');
    
    if (fs.existsSync(dashboardDist)) {
      this.dashboardDistDir = dashboardDist;
      // Serve static files from React build
      this.app.use(express.static(dashboardDist, {
        setHeaders: (res: Response, filePath: string) => {
          // Set correct MIME types
          const ext = path.extname(filePath).toLowerCase();
          if (ext === '.js' || ext === '.mjs') {
            res.type('application/javascript');
          } else if (ext === '.css') {
            res.type('text/css');
          } else if (ext === '.json') {
            res.type('application/json');
          } else if (ext === '.html') {
            res.type('text/html');
          }
        }
      }));
      
      console.log(`📊 Serving React dashboard from: ${dashboardDist}`);
    } else {
      console.log(`⚠️  React dashboard dist directory not found at: ${dashboardDist}`);
      console.log(`   Current working directory: ${process.cwd()}`);
      console.log(`   Repo root: ${repoRoot}`);
      console.log(`   Run "npm run build" in .maestro/dashboard to build the dashboard`);
      console.log(`   Dashboard will be detected automatically once built`);
    }
  }

  private setupApiRoutes(): void {
    // Health check endpoint
    this.app.get('/api/health', (req: Request, res: Response) => {
      res.json({ status: 'ok', websocket: 'ready' });
    });

    // API routes
    this.app.get('/api/metrics', async (req: Request, res: Response) => {
      const workflows = Array.from(this.activeWorkflows.values());
      res.json(workflows);
    });
    
    // Get all workflows (alias for /api/workflows/available)
    this.app.get('/api/workflows', async (req: Request, res: Response) => {
      try {
        // Get workflows from config
        const configWorkflows = await this.workflowManager.listWorkflows();
        
        // Also scan for individual workflow YAML files
        const repoRoot = findRepoRoot();
        const configDir = path.join(repoRoot, '.maestro', 'config');
        const discoveredWorkflows: Array<{ name: string; description?: string; steps: number }> = [];
        
        if (fs.existsSync(configDir) && fs.statSync(configDir).isDirectory()) {
          const yamlFiles = fs.readdirSync(configDir).filter(f => {
            const lower = f.toLowerCase();
            return (lower.endsWith('.yml') || lower.endsWith('.yaml')) && 
                   !lower.includes('config') && 
                   !lower.includes('orchestration') &&
                   !lower.includes('llm') &&
                   !lower.includes('memory') &&
                   !lower.includes('claude') &&
                   !lower.includes('cursor') &&
                   !lower.includes('enhanced');
          });
          
          for (const file of yamlFiles) {
            try {
              const filePath = path.join(configDir, file);
              const yamlContent = fs.readFileSync(filePath, 'utf-8');
              const yaml = require('js-yaml');
              const workflow = yaml.load(yamlContent) as any;
              
              if (workflow && (workflow.name || (workflow.steps && Array.isArray(workflow.steps)))) {
                const workflowName = workflow.name || file.replace(/\.(yml|yaml)$/i, '').replace(/-workflow$/, '');
                // Check if already in configWorkflows
                if (!configWorkflows.find(w => w.name === workflowName)) {
                  discoveredWorkflows.push({
                    name: workflowName,
                    description: workflow.description,
                    steps: Array.isArray(workflow.steps) ? workflow.steps.length : 0,
                  });
                }
              }
            } catch (error: any) {
              // Skip invalid YAML files
              console.warn(`⚠️  Skipping invalid workflow file: ${file}`, error.message);
            }
          }
        }
        
        // Combine config workflows with discovered workflows
        const allWorkflows = [...configWorkflows, ...discoveredWorkflows];
        res.json(allWorkflows);
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Get current activity for a workflow
    this.app.get('/api/workflows/:name/activity', (req: Request, res: Response) => {
      const workflowName = decodeURIComponent(req.params.name);
      const current = this.activityTracker.getCurrentActivity(workflowName);
      const recent = this.activityTracker.getRecentActivities(workflowName, 20);
      res.json({ current, recent });
    });

    // Get all metrics (for MCP server)
    this.app.get('/api/metrics/all', async (req: Request, res: Response) => {
      const workflows = Array.from(this.activeWorkflows.values());
      res.json({ workflows });
    });

    // Workflow management endpoints
    this.app.get('/api/workflows/available', async (req: Request, res: Response) => {
      try {
        // Get workflows from config
        const configWorkflows = await this.workflowManager.listWorkflows();
        
        // Also scan for individual workflow YAML files
        const repoRoot = findRepoRoot();
        const configDir = path.join(repoRoot, '.maestro', 'config');
        const discoveredWorkflows: Array<{ name: string; description?: string; steps: number }> = [];
        
        if (fs.existsSync(configDir) && fs.statSync(configDir).isDirectory()) {
          const yamlFiles = fs.readdirSync(configDir).filter(f => {
            const lower = f.toLowerCase();
            return (lower.endsWith('.yml') || lower.endsWith('.yaml')) && 
                   !lower.includes('config') && 
                   !lower.includes('orchestration') &&
                   !lower.includes('llm') &&
                   !lower.includes('memory') &&
                   !lower.includes('claude') &&
                   !lower.includes('cursor') &&
                   !lower.includes('enhanced');
          });
          
          for (const file of yamlFiles) {
            try {
              const filePath = path.join(configDir, file);
              const yamlContent = fs.readFileSync(filePath, 'utf-8');
              const yaml = require('js-yaml');
              const workflow = yaml.load(yamlContent) as any;
              
              if (workflow && (workflow.name || (workflow.steps && Array.isArray(workflow.steps)))) {
                const workflowName = workflow.name || file.replace(/\.(yml|yaml)$/i, '').replace(/-workflow$/, '');
                // Check if already in configWorkflows
                if (!configWorkflows.find(w => w.name === workflowName)) {
                  discoveredWorkflows.push({
                    name: workflowName,
                    description: workflow.description,
                    steps: Array.isArray(workflow.steps) ? workflow.steps.length : 0,
                  });
                }
              }
            } catch (error: any) {
              // Skip invalid YAML files
              console.warn(`⚠️  Skipping invalid workflow file: ${file}`, error.message);
            }
          }
        }
        
        // Combine config workflows with discovered workflows
        const allWorkflows = [...configWorkflows, ...discoveredWorkflows];
        res.json({ workflows: allWorkflows });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Get workflow YAML content
    this.app.get('/api/workflows/:name/yaml', async (req: Request, res: Response) => {
      try {
        const workflowName = decodeURIComponent(req.params.name);
        const repoRoot = findRepoRoot();
        const configDir = path.join(repoRoot, '.maestro', 'config');
        
        // Try multiple possible file name patterns
        const possiblePaths = [
          path.join(configDir, `${workflowName}-workflow.yml`),
          path.join(configDir, `${workflowName}.yml`),
          path.join(configDir, `${workflowName}.yaml`),
        ];
        
        // Also try to find by searching all YAML files in config directory
        let yamlContent = '';
        let filePath = '';
        
        // First, try exact matches
        for (const possiblePath of possiblePaths) {
          if (fs.existsSync(possiblePath)) {
            filePath = possiblePath;
            yamlContent = fs.readFileSync(possiblePath, 'utf-8');
            break;
          }
        }
        
        // If not found, search for files that contain the workflow name
        if (!yamlContent && fs.existsSync(configDir)) {
          const files = fs.readdirSync(configDir);
          // Try exact match first (case-insensitive)
          const exactMatch = files.find(f => {
            const nameWithoutExt = f.replace(/\.(yml|yaml)$/i, '');
            return nameWithoutExt.toLowerCase() === workflowName.toLowerCase() || 
                   nameWithoutExt.toLowerCase() === `${workflowName}-workflow`.toLowerCase();
          });
          
          if (exactMatch) {
            filePath = path.join(configDir, exactMatch);
            yamlContent = fs.readFileSync(filePath, 'utf-8');
          } else {
            // Try partial match (workflow name is contained in filename)
            const partialMatch = files.find(f => 
              (f.includes(workflowName) || workflowName.includes(f.replace(/\.(yml|yaml)$/i, ''))) &&
              (f.endsWith('.yml') || f.endsWith('.yaml'))
            );
            if (partialMatch) {
              filePath = path.join(configDir, partialMatch);
              yamlContent = fs.readFileSync(filePath, 'utf-8');
            }
          }
        }
        
        if (!yamlContent) {
          return res.status(404).json({ 
            error: `Workflow YAML file not found for: ${workflowName}`,
            searchedPaths: possiblePaths.map(p => p.replace(repoRoot, '').replace(/^[\/\\]/, ''))
          });
        }
        
        res.json({ 
          workflowName,
          yaml: yamlContent,
          filePath: filePath.replace(repoRoot, '').replace(/^[\/\\]/, '') // Relative path
        });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Save workflow YAML content
    this.app.post('/api/workflows/:name/yaml', async (req: Request, res: Response) => {
      try {
        const workflowName = decodeURIComponent(req.params.name);
        const { yaml: yamlContent, filePath: providedFilePath } = req.body;
        
        if (!yamlContent || typeof yamlContent !== 'string') {
          return res.status(400).json({ error: 'YAML content is required' });
        }
        
        const repoRoot = findRepoRoot();
        const configDir = path.join(repoRoot, '.maestro', 'config');
        
        let filePath = '';
        
        // If file path was provided (from GET request), use it
        if (providedFilePath && typeof providedFilePath === 'string') {
          filePath = path.join(repoRoot, providedFilePath);
        } else {
          // Try to find existing file first
          const possiblePaths = [
            path.join(configDir, `${workflowName}-workflow.yml`),
            path.join(configDir, `${workflowName}.yml`),
            path.join(configDir, `${workflowName}.yaml`),
          ];
          
          for (const possiblePath of possiblePaths) {
            if (fs.existsSync(possiblePath)) {
              filePath = possiblePath;
              break;
            }
          }
          
          // If not found, create new file with -workflow suffix
          if (!filePath) {
            filePath = path.join(configDir, `${workflowName}-workflow.yml`);
          }
        }
        
        // Ensure config directory exists
        if (!fs.existsSync(configDir)) {
          fs.mkdirSync(configDir, { recursive: true });
        }
        
        // Write YAML content
        fs.writeFileSync(filePath, yamlContent, 'utf-8');
        
        res.json({ 
          success: true,
          workflowName,
          filePath: filePath.replace(repoRoot, '').replace(/^[\/\\]/, '') // Relative path
        });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.post('/api/workflows/start', async (req: Request, res: Response) => {
      try {
        const options = req.body;
        const executionId = await this.workflowManager.startWorkflow(options);
        
        // Load workflow definition and record it in dashboard with executionId
        try {
          const workflows = await this.workflowManager.listWorkflows();
          const workflow = workflows.find(w => w.name === options.flow);
          if (workflow) {
            // Create a basic workflow definition for tracking
            const workflowDef: WorkflowDefinition = {
              name: workflow.name,
              description: workflow.description || '',
              steps: Array.from({ length: workflow.steps }, (_, i) => ({
                name: `step-${i + 1}`,
                agent: 'Backend',
                description: `Step ${i + 1}`,
                dependsOn: [],
              })),
            };
            
            // Record workflow start in dashboard with executionId
            this.recordWorkflowStart(workflowDef, options.model, executionId);
          } else {
            // Still create a basic entry even if workflow not found
            const basicWorkflow: WorkflowDefinition = {
              name: options.flow,
              description: '',
              steps: [],
            };
            this.recordWorkflowStart(basicWorkflow, options.model, executionId);
          }
        } catch (err) {
          console.error('Failed to load workflow definition:', err);
          // Still create a basic entry
          const basicWorkflow: WorkflowDefinition = {
            name: options.flow,
            description: '',
            steps: [],
          };
          this.recordWorkflowStart(basicWorkflow, options.model, executionId);
        }
        
        res.json({ success: true, executionId });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.post('/api/workflows/:executionId/stop', async (req: Request, res: Response) => {
      try {
        const { executionId } = req.params;
        const stopped = await this.workflowManager.stopWorkflow(executionId);
        res.json({ success: stopped });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/workflows/executions', (req: Request, res: Response) => {
      const executions = this.workflowManager.getActiveExecutions();
      res.json({ executions });
    });

    // Agent cleanup endpoints
    this.app.get('/api/agents/list', async (req: Request, res: Response) => {
      try {
        const agents = await this.agentCleanupService.listAgents();
        res.json({ agents });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.post('/api/agents/cleanup', async (req: Request, res: Response) => {
      try {
        const result = await this.agentCleanupService.cleanup();
        res.json({ success: true, ...result });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/agents/cleanup/status', (req: Request, res: Response) => {
      try {
        const status = this.agentCleanupService.getStatus();
        res.json(status);
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Get active workflows from dashboard (includes metrics)
    this.app.get('/api/workflows/active', async (req: Request, res: Response) => {
      const workflows = Array.from(this.activeWorkflows.values());
      
      // Enhance with process status, executionId, and full YAML details
      const enhancedWorkflows = await Promise.all(workflows.map(async (w) => {
        const executions = this.workflowManager.getActiveExecutions();
        const execution = executions.find(e => e.workflowName === w.workflowName);
        
        // Use executionId from metrics if available, otherwise try to find it
        let executionId = w.executionId;
        if (!executionId && execution) {
          executionId = execution.id;
          // Update metrics with executionId
          w.executionId = executionId;
        }
        
        let processStatus: { running: boolean; pid?: number; exitCode?: number | null; cursorAgentRunning?: boolean } = {
          running: false,
          cursorAgentRunning: false,
        };
        
        if (execution) {
          processStatus = this.workflowManager.getProcessStatus(execution.id);
        } else if (w.status === 'running') {
          // Workflow is marked as running but no execution found - mark as unknown
          processStatus = {
            running: false,
            cursorAgentRunning: false,
          };
        }
        
        // Update metrics with process status
        w.processStatus = processStatus;
        
        // Load full workflow details from YAML
        let workflowDetails: any = {};
        try {
          const repoRoot = findRepoRoot();
          const configDir = path.join(repoRoot, '.maestro', 'config');
          const possiblePaths = [
            path.join(configDir, `${w.workflowName}-workflow.yml`),
            path.join(configDir, `${w.workflowName}.yml`),
            path.join(configDir, `${w.workflowName}.yaml`),
            path.join(repoRoot, `${w.workflowName}-workflow.yml`),
            path.join(repoRoot, `${w.workflowName}.yml`),
            path.join(repoRoot, `${w.workflowName}.yaml`),
          ];
          
          for (const possiblePath of possiblePaths) {
            if (fs.existsSync(possiblePath)) {
              const yamlContent = fs.readFileSync(possiblePath, 'utf-8');
              const yaml = require('js-yaml');
              const fullWorkflow = yaml.load(yamlContent) as any;
              
              workflowDetails = {
                description: fullWorkflow.description,
                metadata: fullWorkflow.metadata || {},
                agents: fullWorkflow.agents || [],
                steps: fullWorkflow.steps || [],
                yamlPath: possiblePath.replace(repoRoot, '').replace(/^[\/\\]/, ''),
                yamlContent: yamlContent, // Include full YAML content
              };
              break;
            }
          }
        } catch (yamlError: any) {
          // If YAML loading fails, continue without details
          console.error(`Error loading YAML for workflow ${w.workflowName}:`, yamlError);
        }
        
        return {
          ...w,
          processStatus,
          executionId: executionId || null,
          ...workflowDetails,
        };
      }));
      
      res.json({ workflows: enhancedWorkflows });
    });

    // Get workflow metrics for a specific execution
    this.app.get('/api/workflows/:executionId/metrics', (req: Request, res: Response) => {
      try {
        const { executionId } = req.params;
        const execution = this.workflowManager.getExecution(executionId);
        if (!execution) {
          return res.status(404).json({ error: 'Execution not found' });
        }
        
        // Get basic metrics from execution
        const metrics = {
          workflowName: execution.workflowName,
          status: 'running', // Could be enhanced to track actual status
          startTime: execution.startTime,
          duration: Date.now() - execution.startTime.getTime(),
          totalSteps: 0,
          completedSteps: 0,
          failedSteps: 0,
          successRate: 0,
          currentStep: 'N/A',
        };
        
        res.json({ metrics });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // MCP server management endpoints
    this.app.get('/api/mcp/status', (req: Request, res: Response) => {
      const status = this.workflowManager.getMcpServerStatus();
      res.json(status);
    });

    // Get system information
    this.app.get('/api/system/info', (req: Request, res: Response) => {
      try {
        const systemInfo = this.systemInfoService.getSystemInfo();
        res.json(systemInfo);
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Get detailed MCP server information
    this.app.get('/api/mcp/info', async (req: Request, res: Response) => {
      try {
        const status = this.workflowManager.getMcpServerStatus();
        
        if (!status.running) {
          return res.json({
            running: false,
            port: status.port,
            resources: [],
            tools: [],
            resourceCount: 0,
            toolCount: 0,
            health: 'stopped',
          });
        }

        // Fetch resources and tools from MCP server
        const port = status.port || 3001;
        const baseUrl = `http://localhost:${port}`;
        
        let resources: any[] = [];
        let tools: any[] = [];
        let health = 'unknown';

        try {
          // Get resources
          const resourcesRes = await fetch(`${baseUrl}/mcp/resources`);
          if (resourcesRes.ok) {
            const resourcesData = await resourcesRes.json() as { resources?: any[] };
            resources = resourcesData.resources || [];
          }

          // Get tools
          const toolsRes = await fetch(`${baseUrl}/mcp/tools`);
          if (toolsRes.ok) {
            const toolsData = await toolsRes.json() as { tools?: any[] };
            tools = toolsData.tools || [];
          }

          // Check health
          const healthRes = await fetch(`${baseUrl}/mcp/health`);
          if (healthRes.ok) {
            health = 'healthy';
          } else {
            health = 'unhealthy';
          }
        } catch (error) {
          // MCP server might not be fully ready
          health = 'starting';
        }

        res.json({
          running: true,
          port: status.port || 3001,
          resources,
          tools,
          resourceCount: resources.length,
          toolCount: tools.length,
          health,
          baseUrl: `http://localhost:${port}`,
        });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.post('/api/mcp/start', async (req: Request, res: Response) => {
      try {
        // Handle both JSON body and query parameter
        const port = req.body?.port || req.query?.port ? parseInt(String(req.query.port), 10) : 3001;
        console.log(`📡 API: Starting MCP server on port ${port}...`);
        console.log(`   Request body:`, JSON.stringify(req.body));
        console.log(`   Request query:`, JSON.stringify(req.query));
        
        // Check if MCP server is already running
        const mcpStatus = this.workflowManager.getMcpServerStatus();
        if (mcpStatus.running) {
          console.log(`ℹ️  API: MCP Server is already running on port ${mcpStatus.port}`);
          res.json({ success: true, port: mcpStatus.port, message: 'MCP Server is already running' });
          return;
        }
        
        try {
          const started = await this.workflowManager.startMcpServer(port);
          if (started) {
            console.log(`✅ API: MCP Server started successfully on port ${port}`);
            res.json({ success: true, port });
          } else {
            console.error(`❌ API: MCP Server failed to start on port ${port} (startMcpServer returned false)`);
            res.status(400).json({ success: false, error: 'MCP Server failed to start. Could not find CLI script or process failed to start. Check console logs for details.' });
          }
        } catch (mcpError: any) {
          console.error(`❌ API: MCP Server startup exception:`, mcpError);
          console.error(`   Error message:`, mcpError.message);
          console.error(`   Stack:`, mcpError.stack);
          res.status(400).json({ 
            success: false, 
            error: mcpError.message || 'MCP Server failed to start. Check console logs for details.',
            details: mcpError.stack 
          });
        }
      } catch (error: any) {
        console.error('❌ API: MCP Server startup error:', error);
        console.error('   Error details:', error.message);
        console.error('   Stack:', error.stack);
        res.status(500).json({ success: false, error: error.message || 'Failed to start MCP Server' });
      }
    });

    this.app.post('/api/mcp/stop', async (req: Request, res: Response) => {
      try {
        const stopped = await this.workflowManager.stopMcpServer();
        res.json({ success: stopped });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Workflow creation endpoints
    this.app.get('/api/workflows/roles', async (req: Request, res: Response) => {
      try {
        const roles = await this.workflowCreator.getAvailableRoles();
        res.json({ roles });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.post('/api/workflows/create', async (req: Request, res: Response) => {
      try {
        const result = await this.workflowCreator.createWorkflow(req.body);
        res.json(result);
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Generate workflow from prompt
    this.app.post('/api/workflows/generate', async (req: Request, res: Response) => {
      try {
        const { prompt, context } = req.body;
        if (!prompt || prompt.trim() === '') {
          return res.status(400).json({ error: 'Prompt is required' });
        }

        // Get available roles for context
        const roles = await this.workflowCreator.getAvailableRoles();
        const availableRoles = roles.map(r => r.name);

        // Get existing workflows for context
        const workflows = await this.workflowManager.listWorkflows();
        const existingWorkflows = workflows.map(w => w.name);

        const result = await this.workflowGenerator.generateWorkflow({
          prompt: prompt.trim(),
          context: {
            availableRoles,
            existingWorkflows,
            ...context,
          },
        });

        res.json(result);
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.put('/api/workflows/:name', async (req: Request, res: Response) => {
      try {
        const workflowName = decodeURIComponent(req.params.name);
        const workflow = req.body;
        
        // Save workflow to YAML file
        const repoRoot = findRepoRoot();
        const configDir = path.join(repoRoot, '.maestro', 'config');
        
        // Ensure config directory exists
        if (!fs.existsSync(configDir)) {
          fs.mkdirSync(configDir, { recursive: true });
        }
        
        // Try to find existing file first
        let filePath = '';
        const possiblePaths = [
          path.join(configDir, `${workflowName}-workflow.yml`),
          path.join(configDir, `${workflowName}.yml`),
          path.join(configDir, `${workflowName}.yaml`),
        ];
        
        for (const possiblePath of possiblePaths) {
          if (fs.existsSync(possiblePath)) {
            filePath = possiblePath;
            break;
          }
        }
        
        // If not found, create new file
        if (!filePath) {
          filePath = path.join(configDir, `${workflowName}-workflow.yml`);
        }
        
        // Convert workflow object to YAML
        const yaml = require('js-yaml');
        // Remove yamlContent and yamlPath from workflow before saving
        const { yamlContent: _, yamlPath: __, ...workflowToSave } = workflow;
        const yamlOutput = yaml.dump(workflowToSave, {
          indent: 2,
          lineWidth: -1,
          noRefs: true,
          sortKeys: false,
        });
        
        // Write to file
        fs.writeFileSync(filePath, yamlOutput, 'utf-8');
        
        // Also update via WorkflowCreator if needed
        let result = { success: true };
        try {
          result = await this.workflowCreator.updateWorkflow(workflowName, workflowToSave);
        } catch (error) {
          // Ignore if WorkflowCreator update fails, YAML save is primary
          console.warn('WorkflowCreator update failed, but YAML was saved:', error);
        }
        
        res.json({ 
          ...result, 
          filePath: filePath.replace(repoRoot, '').replace(/^[\/\\]/, '') 
        });
      } catch (error: any) {
        console.error('Error saving workflow:', error);
        res.status(500).json({ error: error.message });
      }
    });

    // Get workflow backlog
    this.app.get('/api/workflows/:name/backlog', async (req: Request, res: Response) => {
      try {
        const workflowName = decodeURIComponent(req.params.name);
        const backlog = this.workflowManager.getScrumMaster().getBacklog(workflowName);
        res.json({ backlog });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Update backlog item priority
    this.app.put('/api/workflows/:name/backlog/priority', async (req: Request, res: Response) => {
      try {
        const workflowName = decodeURIComponent(req.params.name);
        const { stepName, taskName, priority } = req.body;
        
        if (typeof priority !== 'number') {
          return res.status(400).json({ error: 'Priority must be a number' });
        }

        const updated = this.workflowManager.getScrumMaster().updateBacklogPriority(
          workflowName,
          stepName,
          taskName,
          priority
        );

        if (updated) {
          res.json({ success: true });
        } else {
          res.status(404).json({ error: 'Backlog item not found' });
        }
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Workflow reevaluation endpoint (scrum master reassignment)
    this.app.post('/api/workflows/:name/reevaluate', async (req: Request, res: Response) => {
      try {
        const workflowName = decodeURIComponent(req.params.name);
        const metrics = this.activeWorkflows.get(workflowName);
        
        if (!metrics || metrics.status !== 'running') {
          return res.status(400).json({ 
            success: false, 
            message: 'Workflow must be running to reevaluate' 
          });
        }

        // Trigger reevaluation - this will reassign steps to agents
        const result = await this.workflowManager.reevaluateWorkflow(workflowName);
        
        res.json({ 
          success: true, 
          message: result.message || 'Workflow reevaluation started. Maestro will reassign steps to agents.' 
        });
      } catch (error: any) {
        res.status(500).json({ 
          success: false, 
          message: error.message || 'Failed to reevaluate workflow' 
        });
      }
    });

    this.app.delete('/api/workflows/:name', async (req: Request, res: Response) => {
      try {
        const { name } = req.params;
        const result = await this.workflowCreator.deleteWorkflow(name);
        res.json(result);
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Model management endpoints
    this.app.get('/api/models', (req: Request, res: Response) => {
      const models = this.modelManager.getAvailableModels();
      res.json(models);
    });

    this.app.post('/api/workflows/:workflow/model', (req: Request, res: Response) => {
      try {
        const { workflow } = req.params;
        const { modelId } = req.body;
        this.recordModelChange(workflow, '', modelId);
        res.json({ success: true });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.post('/api/workflows/:workflow/steps/:step/model', (req: Request, res: Response) => {
      try {
        const { workflow, step } = req.params;
        const { modelId } = req.body;
        this.recordModelChange(workflow, step, modelId);
        res.json({ success: true });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/workflows/:workflow/model-history', (req: Request, res: Response) => {
      const { workflow } = req.params;
      const history = this.modelManager.getChangeHistory(workflow);
      res.json({ history });
    });

    // Agent management endpoints
    this.app.get('/api/agents', (req: Request, res: Response) => {
      // Get available agents from workflow creator
      this.workflowCreator.getAvailableRoles().then(roles => {
        res.json(roles.map(r => r.name));
      }).catch(() => {
        res.json(['Architect', 'Backend', 'Frontend', 'DevOps', 'Security', 'QA']);
      });
    });

    this.app.post('/api/workflows/:workflow/steps/:step/agent', (req: Request, res: Response) => {
      try {
        const { workflow, step } = req.params;
        const { agent, reason } = req.body;
        this.recordAgentSwitch(workflow, step, agent, reason);
        res.json({ success: true });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/workflows/:workflow/agent-history', (req: Request, res: Response) => {
      const { workflow } = req.params;
      const history = this.agentSwitcher.getSwitchHistory(workflow);
      res.json({ history });
    });

    // Token usage endpoints
    this.app.get('/api/workflows/:workflow/tokens', (req: Request, res: Response) => {
      const { workflow } = req.params;
      const summary = this.tokenUsageTracker.getSummary(workflow);
      res.json({ summary });
    });

    this.app.get('/api/workflows/:workflow/steps/:step/tokens', (req: Request, res: Response) => {
      const { workflow, step } = req.params;
      const summary = this.tokenUsageTracker.getSummary(workflow);
      const stepUsage = summary?.stepUsages.get(step);
      res.json({ usage: stepUsage });
    });

    // Progress and analysis endpoints
    this.app.get('/api/workflows/:workflow/progress', (req: Request, res: Response) => {
      const { workflow } = req.params;
      const progress = this.taskProgressTracker.getAllProgress(workflow);
      const progressArray = Array.from(progress.entries()).map(([step, p]) => ({
        step,
        ...p,
      }));
      res.json({ progress: progressArray });
    });

    this.app.get('/api/workflows/:workflow/steps/:step/analysis', (req: Request, res: Response) => {
      const { workflow, step } = req.params;
      const analysis = this.outputMonitor.getLatestAnalysis(workflow, step);
      res.json({ analysis });
    });

    this.app.get('/api/workflows/:workflow/recommendations', (req: Request, res: Response) => {
      const { workflow } = req.params;
      const recommendations = this.outputMonitor.getReAlignmentRecommendations(workflow);
      res.json({ recommendations });
    });

    // Health check endpoint
    this.app.get('/api/health', (req: Request, res: Response) => {
      res.json({ status: 'ok', timestamp: new Date().toISOString() });
    });

    // API endpoint to register workflow events from external processes
    this.app.post('/api/workflows/:name/start', async (req: Request, res: Response) => {
      try {
        const workflowName = decodeURIComponent(req.params.name);
        const { modelId } = req.body;
        
        // Load full workflow definition
        const workflows = await this.workflowManager.listWorkflows();
        const workflowInfo = workflows.find(w => w.name === workflowName);
        
        if (!workflowInfo) {
          return res.status(404).json({ error: `Workflow "${workflowName}" not found` });
        }

        // Load full workflow from YAML
        const repoRoot = findRepoRoot();
        const configDir = path.join(repoRoot, '.maestro', 'config');
        const possiblePaths = [
          path.join(configDir, `${workflowName}-workflow.yml`),
          path.join(configDir, `${workflowName}.yml`),
          path.join(configDir, `${workflowName}.yaml`),
        ];

        let workflow: WorkflowDefinition | null = null;
        for (const possiblePath of possiblePaths) {
          if (fs.existsSync(possiblePath)) {
            try {
              const yaml = require('js-yaml');
              const content = fs.readFileSync(possiblePath, 'utf-8');
              workflow = yaml.load(content) as WorkflowDefinition;
              workflow.name = workflowName;
              break;
            } catch (error) {
              console.error(`Error loading workflow from ${possiblePath}:`, error);
            }
          }
        }

        if (!workflow) {
          // Fallback to minimal workflow
          workflow = {
            name: workflowName,
            steps: Array(workflowInfo.steps || 0).fill(null).map((_, i) => ({
              name: `step-${i}`,
              agent: 'Architect',
              description: `Step ${i + 1}`,
            })),
          };
        }

        // Start workflow execution
        const executionId = await this.workflowManager.startWorkflow({
          flow: workflowName,
          runner: 'cursor',
          model: modelId,
        });

        // Record in dashboard
        this.recordWorkflowStart(workflow, modelId, executionId);

        // Automatically start scrum master monitoring
        try {
          await this.workflowManager.reevaluateWorkflow(workflowName);
          console.log(`🎯 Scrum Master: Auto-started for workflow "${workflowName}"`);
        } catch (error) {
          console.warn(`⚠️  Failed to auto-start scrum master:`, error);
          // Don't fail the workflow start if scrum master fails
        }

        res.json({ success: true, executionId });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.post('/api/workflows/:name/step', async (req: Request, res: Response) => {
      const workflowName = req.params.name;
      const { stepName, status } = req.body;
      const dashboardStatus = status === 'completed' ? 'completed' :
        status === 'failed' ? 'failed' : 'running';
      this.recordStepUpdate(workflowName, stepName, dashboardStatus);
      res.json({ success: true });
    });

    this.app.post('/api/workflows/:name/end', async (req: Request, res: Response) => {
      const workflowName = req.params.name;
      const { success, completedTasks, failedTasks, duration } = req.body;
      const result: ExecutionResult = {
        success: success || false,
        completedTasks: completedTasks || [],
        failedTasks: failedTasks || [],
        nestedWorkflows: new Map(),
      };
      this.recordWorkflowEnd(workflowName, result, duration || 0);
      res.json({ success: true });
    });

    this.app.get('/api/workflows/:name', async (req: Request, res: Response) => {
      try {
        const workflowName = decodeURIComponent(req.params.name);
        const workflows = await this.workflowManager.listWorkflows();
        const workflow = workflows.find(w => w.name === workflowName);
        
        // Load full workflow details from YAML file
        // Try to find by workflow name first, then scan all YAML files
        try {
          const repoRoot = findRepoRoot();
          const configDir = path.join(repoRoot, '.maestro', 'config');
          
          // First, try exact name matches
          const possiblePaths = [
            path.join(configDir, `${workflowName}-workflow.yml`),
            path.join(configDir, `${workflowName}.yml`),
            path.join(configDir, `${workflowName}.yaml`),
            path.join(repoRoot, `${workflowName}-workflow.yml`),
            path.join(repoRoot, `${workflowName}.yml`),
            path.join(repoRoot, `${workflowName}.yaml`),
          ];
          
          let yamlPath = '';
          let yamlContent = '';
          
          // Try exact matches first
          for (const possiblePath of possiblePaths) {
            if (fs.existsSync(possiblePath)) {
              yamlPath = possiblePath;
              yamlContent = fs.readFileSync(possiblePath, 'utf-8');
              break;
            }
          }
          
          // If not found, scan all YAML files for matching workflow name
          if (!yamlContent && fs.existsSync(configDir) && fs.statSync(configDir).isDirectory()) {
            const yamlFiles = fs.readdirSync(configDir).filter(f => {
              const lower = f.toLowerCase();
              return (lower.endsWith('.yml') || lower.endsWith('.yaml')) && 
                     !lower.includes('config') && 
                     !lower.includes('orchestration') &&
                     !lower.includes('llm') &&
                     !lower.includes('memory') &&
                     !lower.includes('claude') &&
                     !lower.includes('cursor') &&
                     !lower.includes('enhanced');
            });
            
            for (const file of yamlFiles) {
              try {
                const filePath = path.join(configDir, file);
                const fileContent = fs.readFileSync(filePath, 'utf-8');
                const yaml = require('js-yaml');
                const parsedWorkflow = yaml.load(fileContent) as any;
                
                // Check if this file contains the workflow we're looking for
                if (parsedWorkflow && parsedWorkflow.name === workflowName) {
                  yamlPath = filePath;
                  yamlContent = fileContent;
                  break;
                }
              } catch (error) {
                // Skip invalid files
                continue;
              }
            }
          }
          
          // Parse YAML to get full details
          if (yamlContent) {
            const yaml = require('js-yaml');
            const fullWorkflow = yaml.load(yamlContent) as any;
            
            // Enhance workflow with full YAML details
            const enhancedWorkflow = {
              name: workflowName,
              description: fullWorkflow.description || workflow?.description,
              metadata: fullWorkflow.metadata || {},
              agents: Array.isArray(fullWorkflow.agents) ? fullWorkflow.agents : [],
              steps: Array.isArray(fullWorkflow.steps) ? fullWorkflow.steps : [],
              yamlContent,
              yamlPath: yamlPath.replace(repoRoot, '').replace(/^[\/\\]/, ''),
            };
            
            res.json(enhancedWorkflow);
          } else if (workflow) {
            // Return basic workflow if YAML not found
            res.json({
              ...workflow,
              steps: [],
              agents: [],
              metadata: {},
            });
          } else {
            res.status(404).json({ error: `Workflow "${workflowName}" not found` });
          }
        } catch (yamlError: any) {
          // If YAML loading fails, return basic workflow or 404
          console.error(`Error loading YAML for workflow ${workflowName}:`, yamlError);
          if (workflow) {
            res.json({
              ...workflow,
              steps: [],
              agents: [],
              metadata: {},
            });
          } else {
            res.status(404).json({ error: `Workflow "${workflowName}" not found` });
          }
        }
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });
    
    // Get workflow metrics
    this.app.get('/api/workflows/:name/metrics', async (req: Request, res: Response) => {
      const workflowName = decodeURIComponent(req.params.name);
      const metrics = this.activeWorkflows.get(workflowName);
      if (metrics) {
        res.json(metrics);
      } else {
        res.status(404).json({ error: 'Workflow metrics not found' });
      }
    });

    this.app.get('/api/checkpoints', async (req: Request, res: Response) => {
      try {
        const checkpoints = await this.stateManager.list();
        res.json({ checkpoints });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/reports', async (req: Request, res: Response) => {
      try {
        const reportsDir = path.join(process.cwd(), '.maestro', 'reports');
        const files = fs.existsSync(reportsDir)
          ? fs.readdirSync(reportsDir).filter(f => f.endsWith('.md') || f.endsWith('.json') || f.endsWith('.html'))
          : [];
        res.json({ reports: files });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Memory API endpoints
    this.app.post('/api/memory/add', async (req: Request, res: Response) => {
      try {
        if (!this.memoryService) {
          return res.status(503).json({ error: 'Memory service not available' });
        }

        const { content, type, role, tags, public: isPublic, auto_commit } = req.body;
        if (!content) {
          return res.status(400).json({ error: 'Content is required' });
        }

        const filePath = this.memoryService.add(
          content,
          type || 'mid-term',
          role,
          tags,
          isPublic !== false,
          auto_commit
        );

        res.json({ success: true, file_path: filePath });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/memory/search', async (req: Request, res: Response) => {
      try {
        if (!this.memoryService) {
          return res.status(503).json({ error: 'Memory service not available' });
        }

        const query = req.query.q as string;
        const topK = parseInt(req.query.top_k as string) || 5;
        const memoryType = req.query.type as string;
        const useIndex = req.query.use_index !== 'false';
        const useHybrid = req.query.use_hybrid !== 'false';
        const useLLMReranking = req.query.use_llm_reranking === 'true';
        const isPublic = req.query.public !== 'false';

        if (!query) {
          return res.status(400).json({ error: 'Query parameter "q" is required' });
        }

        const results = await this.memoryService.search(
          query,
          topK,
          memoryType as any,
          useIndex,
          useHybrid,
          useLLMReranking,
          isPublic
        );

        res.json({ results });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/memory/list', async (req: Request, res: Response) => {
      try {
        if (!this.memoryService) {
          return res.status(503).json({ error: 'Memory service not available' });
        }

        const type = req.query.type as string | undefined;
        const role = req.query.role as string | undefined;
        const isPublic = req.query.public !== 'false';
        const limit = req.query.limit ? parseInt(req.query.limit as string) : undefined;

        const entries = this.memoryService.list(
          type as any,
          role as any,
          isPublic,
          limit
        );

        // Transform entries to match frontend expectations
        const transformedEntries = entries.map(entry => ({
          content: entry.content,
          type: entry.type,
          role: entry.role,
          tags: entry.tags,
          public: entry.public,
          filePath: entry.filePath,
          timestamp: entry.timestamp,
        }));

        res.json({ entries: transformedEntries });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/memory/context', async (req: Request, res: Response) => {
      try {
        if (!this.contextLoader) {
          return res.status(503).json({ error: 'Context loader not available' });
        }

        const trigger = req.query.trigger as string || 'conversation_start';
        const contextFiles = req.query.files ? (req.query.files as string).split(',') : undefined;

        const context = await this.contextLoader.loadContext(trigger as any, contextFiles);
        res.json(context);
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/memory/patterns', async (req: Request, res: Response) => {
      try {
        if (!this.patternExtractor) {
          return res.status(503).json({ error: 'Pattern extractor not available' });
        }

        const query = req.query.q as string;
        if (query) {
          const patterns = this.patternExtractor.searchPatterns(query);
          res.json({ patterns });
        } else {
          const patterns = this.patternExtractor.getPatterns();
          res.json({ patterns });
        }
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.post('/api/memory/patterns/extract', async (req: Request, res: Response) => {
      try {
        if (!this.patternExtractor) {
          return res.status(503).json({ error: 'Pattern extractor not available' });
        }

        const patterns = await this.patternExtractor.extractPatterns();
        res.json({ success: true, patterns, count: patterns.length });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/memory/sessions', async (req: Request, res: Response) => {
      try {
        if (!this.sessionManager) {
          return res.status(503).json({ error: 'Session manager not available' });
        }

        const limit = req.query.limit ? parseInt(req.query.limit as string) : undefined;
        const sessions = this.sessionManager.getSessions(limit);
        res.json({ sessions });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/memory/sessions/latest', async (req: Request, res: Response) => {
      try {
        if (!this.sessionManager) {
          return res.status(503).json({ error: 'Session manager not available' });
        }

        const session = this.sessionManager.getLatestSession();
        if (!session) {
          return res.status(404).json({ error: 'No sessions found' });
        }

        res.json({ session });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.post('/api/memory/session', async (req: Request, res: Response) => {
      try {
        if (!this.sessionManager) {
          return res.status(503).json({ error: 'Session manager not available' });
        }

        const { content, metadata } = req.body;
        if (!content) {
          return res.status(400).json({ error: 'Content is required' });
        }

        const filePath = this.sessionManager.saveSession(content, metadata);
        res.json({ success: true, file_path: filePath });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/memory/graph', async (req: Request, res: Response) => {
      try {
        if (!this.knowledgeGraph) {
          return res.status(503).json({ error: 'Knowledge graph not available' });
        }

        const nodeId = req.query.node_id as string;
        const maxResults = req.query.max_results ? parseInt(req.query.max_results as string) : 10;

        if (nodeId) {
          const related = this.knowledgeGraph.queryGraph(nodeId, maxResults);
          res.json({ related });
        } else {
          const graphData = this.knowledgeGraph.getGraphData();
          res.json(graphData);
        }
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.post('/api/memory/graph/build', async (req: Request, res: Response) => {
      try {
        if (!this.knowledgeGraph) {
          return res.status(503).json({ error: 'Knowledge graph not available' });
        }

        await this.knowledgeGraph.buildGraph();
        res.json({ success: true, message: 'Knowledge graph built successfully' });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Cursor API endpoints
    this.app.post('/api/cursor/api-key', async (req: Request, res: Response) => {
      try {
        if (!this.cursorApiClient) {
          return res.status(503).json({ error: 'Cursor API client not available' });
        }

        const { api_key } = req.body;
        if (!api_key) {
          return res.status(400).json({ error: 'API key is required' });
        }

        this.cursorApiClient.setApiKey(api_key);
        res.json({ success: true, message: 'API key configured' });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/cursor/api-key', async (req: Request, res: Response) => {
      try {
        if (!this.cursorApiClient) {
          return res.status(503).json({ error: 'Cursor API client not available' });
        }

        const maskedKey = this.cursorApiClient.getApiKey();
        const isConfigured = this.cursorApiClient.isConfigured();
        res.json({ configured: isConfigured, api_key: maskedKey });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/cursor/project', async (req: Request, res: Response) => {
      try {
        if (!this.cursorProjectService) {
          return res.status(503).json({ error: 'Cursor project service not available' });
        }

        const project = await this.cursorProjectService.getProjectInfo();
        if (!project) {
          return res.status(404).json({ error: 'Project not found' });
        }

        const stats = await this.cursorProjectService.getProjectStats();
        res.json({ project, stats });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/cursor/files', async (req: Request, res: Response) => {
      try {
        if (!this.cursorFileService) {
          return res.status(503).json({ error: 'Cursor file service not available' });
        }

        const limit = req.query.limit ? parseInt(req.query.limit as string) : 50;
        const changes = await this.cursorFileService.getFileChanges(limit);
        res.json({ changes });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/cursor/files/:path/history', async (req: Request, res: Response) => {
      try {
        if (!this.cursorFileService) {
          return res.status(503).json({ error: 'Cursor file service not available' });
        }

        const filePath = decodeURIComponent(req.params.path);
        const limit = req.query.limit ? parseInt(req.query.limit as string) : 20;
        const history = await this.cursorFileService.getFileHistory(filePath, limit);
        res.json({ history });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/cursor/files/:path/diff', async (req: Request, res: Response) => {
      try {
        if (!this.cursorFileService) {
          return res.status(503).json({ error: 'Cursor file service not available' });
        }

        const filePath = decodeURIComponent(req.params.path);
        const commit1 = req.query.commit1 as string;
        const commit2 = req.query.commit2 as string;
        const diff = await this.cursorFileService.getFileDiff(filePath, commit1, commit2);
        res.json({ diff });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/cursor/files/summary', async (req: Request, res: Response) => {
      try {
        if (!this.cursorFileService) {
          return res.status(503).json({ error: 'Cursor file service not available' });
        }

        const days = req.query.days ? parseInt(req.query.days as string) : 7;
        const summary = await this.cursorFileService.getRecentChangesSummary(days);
        res.json({ summary });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/cursor/agents', async (req: Request, res: Response) => {
      try {
        if (!this.cursorAgentService) {
          return res.status(503).json({ error: 'Cursor agent service not available' });
        }

        const agents = await this.cursorAgentService.getAgents();
        const stats = await this.cursorAgentService.getAgentStats();
        res.json({ agents, stats });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Delete cursor agent
    this.app.delete('/api/cursor/agents/:id', async (req: Request, res: Response) => {
      try {
        if (!this.cursorAgentService) {
          return res.status(503).json({ error: 'Cursor agent service not available' });
        }
        const agentId = req.params.id;
        const deleted = await this.cursorAgentService.deleteAgent(agentId);
        if (deleted) {
          res.json({ success: true });
        } else {
          res.status(404).json({ error: 'Agent not found' });
        }
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Open terminal for agent
    this.app.post('/api/cursor/agents/:id/terminal', async (req: Request, res: Response) => {
      try {
        if (!this.cursorAgentService) {
          return res.status(503).json({ error: 'Cursor agent service not available' });
        }
        const agentId = req.params.id;
        const agent = await this.cursorAgentService.getAgent(agentId);
        if (!agent) {
          return res.status(404).json({ error: 'Agent not found' });
        }

        // Return command to open terminal with agent context
        const command = `cd "${process.cwd()}" && cursor-agent --resume="${agentId}"`;
        res.json({ command, message: 'Terminal command ready' });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Memory stats endpoint
    this.app.get('/api/memory/stats', async (req: Request, res: Response) => {
      try {
        if (!this.memoryService) {
          return res.status(503).json({ error: 'Memory service not available' });
        }
        const stats = await this.memoryService.getStats();
        res.json({ stats });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/cursor/agents/:id', async (req: Request, res: Response) => {
      try {
        if (!this.cursorAgentService) {
          return res.status(503).json({ error: 'Cursor agent service not available' });
        }

        const agentId = req.params.id;
        const agent = await this.cursorAgentService.getAgent(agentId);
        if (!agent) {
          return res.status(404).json({ error: 'Agent not found' });
        }

        const limit = req.query.limit ? parseInt(req.query.limit as string) : 50;
        const activity = await this.cursorAgentService.getAgentActivity(agentId, limit);
        res.json({ agent, activity });
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    this.app.get('/api/cursor/health', async (req: Request, res: Response) => {
      try {
        if (!this.cursorApiClient) {
          return res.status(503).json({ error: 'Cursor API client not available' });
        }

        const health = await this.cursorApiClient.healthCheck();
        res.json(health);
      } catch (error: any) {
        res.status(500).json({ error: error.message });
      }
    });

    // Serve React dashboard for all non-API routes (SPA routing)
    // This must be last, after all API routes and static file serving
    // Re-check for dashboard directory on each request (in case it was built after server started)
    this.app.get('*', (req: Request, res: Response) => {
      // Don't serve index.html for API or WebSocket routes
      if (req.path.startsWith('/api') || req.path.startsWith('/ws')) {
        res.status(404).json({ error: 'Not found' });
        return;
      }

      // Don't intercept static asset requests (CSS, JS, images, etc.)
      // These should be handled by express.static middleware
      // If we reach here for a static file, it means static middleware didn't handle it
      // So we should let it pass through (next() would be ideal, but we're in a route handler)
      const staticExtensions = ['.css', '.js', '.mjs', '.json', '.png', '.jpg', '.jpeg', '.gif', '.svg', '.ico', '.woff', '.woff2', '.ttf', '.eot', '.map', '.wasm'];
      const ext = path.extname(req.path).toLowerCase();
      if (staticExtensions.includes(ext)) {
        // Try to serve the static file directly if static middleware didn't catch it
        const repoRoot = findRepoRoot();
        const dashboardDist = path.join(repoRoot, '.maestro', 'dashboard', 'dist');
        const filePath = path.join(dashboardDist, req.path);
        
        if (fs.existsSync(filePath)) {
          // Set correct MIME type
          if (ext === '.css') {
            res.type('text/css');
          } else if (ext === '.js' || ext === '.mjs') {
            res.type('application/javascript');
          } else if (ext === '.json') {
            res.type('application/json');
          }
          res.sendFile(filePath);
        } else {
          res.status(404).send('Static file not found');
        }
        return;
      }

      // Re-check for dashboard directory (in case it was built after server started)
      const repoRoot = findRepoRoot();
      const dashboardDist = path.join(repoRoot, '.maestro', 'dashboard', 'dist');
      
      // Always check if dashboard exists (in case it was built after server started)
      if (!this.dashboardDistDir && fs.existsSync(dashboardDist)) {
        // Dashboard was just built, update the directory
        this.dashboardDistDir = dashboardDist;
        console.log(`📊 Dashboard directory detected: ${dashboardDist}`);
        
        // Set up static file serving for the newly detected dashboard
        // Insert before the catch-all route
        const staticMiddleware = express.static(dashboardDist, {
          setHeaders: (res: Response, filePath: string) => {
            const fileExt = path.extname(filePath).toLowerCase();
            if (fileExt === '.js' || fileExt === '.mjs') {
              res.type('application/javascript');
            } else if (fileExt === '.css') {
              res.type('text/css');
            } else if (fileExt === '.json') {
              res.type('application/json');
            } else if (fileExt === '.html') {
              res.type('text/html');
            }
          }
        });
        
        // Insert static middleware before catch-all route
        // We need to remove the catch-all route, add static middleware, then re-add catch-all
        // For now, just set up the middleware - it will work for subsequent requests
        this.app.use(staticMiddleware);
      }

      // Use detected directory or check again
      const dashboardDir = this.dashboardDistDir || (fs.existsSync(dashboardDist) ? dashboardDist : null);
      
      if (dashboardDir) {
        const indexHtml = path.join(dashboardDir, 'index.html');
        if (fs.existsSync(indexHtml)) {
          res.sendFile(indexHtml);
        } else {
          res.status(404).send('Dashboard not found. Please build the React dashboard first.');
        }
      } else {
        // Fallback if dashboard not built
        res.status(200).json({
          message: 'Maestro API Server',
          version: '1.0.0',
          api: {
            baseUrl: `http://localhost:${this.port}`,
            health: '/api/health',
            docs: 'API endpoints are available at /api/*'
          },
          dashboard: {
            message: 'React dashboard not built',
            note: 'Run "npm run build" in .maestro/dashboard to build the dashboard',
            expectedPath: dashboardDist
          }
        });
      }
    });
  }

  /**
   * Setup WebSocket
   */
  private setupWebSocket(): void {
    console.log('📊 Setting up WebSocket server...');
    
    this.wss.on('connection', (ws: WebSocket, req: http.IncomingMessage) => {
      console.log(`📊 Dashboard: New WebSocket connection attempt from ${req.socket.remoteAddress}`);
      this.clients.add(ws);
      console.log(`📊 Dashboard client connected (${this.clients.size} total)`);

      // Send current state immediately
      try {
        const message: DashboardMessage = {
          type: 'metrics',
          data: { workflows: Array.from(this.activeWorkflows.values()) },
          timestamp: new Date(),
        };
        ws.send(JSON.stringify(message));
        console.log('📊 Dashboard: Sent initial state to client');
      } catch (error) {
        console.error('📊 Dashboard: Error sending initial state:', error);
      }

      ws.on('close', (code: number, reason: Buffer) => {
        this.clients.delete(ws);
        console.log(`📊 Dashboard client disconnected (code: ${code}, reason: ${reason.toString()}, ${this.clients.size} total)`);
      });

      ws.on('error', (error: Error) => {
        console.error('📊 Dashboard: WebSocket error:', error);
        this.clients.delete(ws);
      });

      ws.on('message', (data: Buffer) => {
        console.log('📊 Dashboard: Received message from client:', data.toString());
      });
    });

    this.wss.on('error', (error: Error) => {
      console.error('📊 Dashboard: WebSocket server error:', error);
    });

    console.log('📊 WebSocket server setup complete');
  }

  /**
   * Broadcast message to all clients
   */
  private broadcast(message: DashboardMessage): void {
    const data = JSON.stringify(message);
    this.clients.forEach((client) => {
      if (client.readyState === WebSocket.OPEN) {
        client.send(data);
      }
    });
  }

  /**
   * Update agent assignments (called by scrum master)
   */
  async updateAgentAssignments(workflowName: string, assignments: any[]): Promise<void> {
    const metrics = this.activeWorkflows.get(workflowName);
    if (!metrics) return;

    // Update current activity
    const assignmentSummary = assignments
      .map(a => `${a.agentName}: ${a.taskName || a.stepName}`)
      .join(', ');
    metrics.currentActivity = `Sprint: ${assignmentSummary}`;

    // Broadcast update
    this.broadcast({
      type: 'metrics',
      data: { workflow: workflowName, metrics },
      timestamp: new Date(),
    });
  }

  /**
   * Update workflow agent GUIDs (called by scrum master)
   */
  updateWorkflowAgentGuids(workflowName: string, agentGuids: Record<string, string>): void {
    const metrics = this.activeWorkflows.get(workflowName);
    if (metrics) {
      metrics.agentGuids = agentGuids;
      
      // Broadcast update
      this.broadcast({
        type: 'metrics',
        data: { workflow: workflowName, metrics },
        timestamp: new Date(),
      });
    }
  }

  /**
   * Record workflow start
   */
  recordWorkflowStart(workflow: WorkflowDefinition, modelId?: string, executionId?: string): void {
    // Set default model if provided
    if (modelId) {
      this.modelManager.setWorkflowModel(workflow.name, modelId);
    }

    // Generate or retrieve cursor-agent GUID for this workflow
    // Use a consistent GUID based on workflow name so context persists across runs
    let cursorAgentGuid: string;
    const existingMetrics = this.activeWorkflows.get(workflow.name);
    if (existingMetrics?.cursorAgentGuid) {
      // Reuse existing GUID to maintain context
      cursorAgentGuid = existingMetrics.cursorAgentGuid;
    } else {
      // Generate new GUID (format: workflow-name-based UUID)
      const crypto = require('crypto');
      const workflowHash = crypto.createHash('md5').update(workflow.name).digest('hex');
      // Create a UUID-like format from the hash: xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx
      cursorAgentGuid = [
        workflowHash.substring(0, 8),
        workflowHash.substring(8, 12),
        '4' + workflowHash.substring(13, 16),
        ((parseInt(workflowHash[16], 16) & 0x3) | 0x8).toString(16) + workflowHash.substring(17, 20),
        workflowHash.substring(20, 32)
      ].join('-');
    }

    // Get agent GUIDs from scrum master if available
    const agentGuids = this.workflowManager.getScrumMaster().getAgentGuids(workflow.name);

    const metrics: DashboardMetrics = {
      workflowName: workflow.name,
      executionId: executionId,
      cursorAgentGuid: cursorAgentGuid,
      agentGuids: Object.keys(agentGuids).length > 0 ? agentGuids : undefined,
      startTime: new Date(),
      duration: 0,
      totalSteps: workflow.steps.length,
      completedSteps: 0,
      failedSteps: 0,
      successRate: 0,
      status: 'running',
      cacheHits: 0,
      cacheMisses: 0,
      retryAttempts: 0,
      currentModel: modelId || this.modelManager.getWorkflowModel(workflow.name) || undefined,
      currentActivity: 'Spinning up workflow...',
    };

    this.activeWorkflows.set(workflow.name, metrics);
    // Initialize step states tracking for this workflow
    this.stepStates.set(workflow.name, new Map());

    this.broadcast({
      type: 'workflow-start',
      data: { workflow: workflow.name, metrics },
      timestamp: new Date(),
    });
  }

  /**
   * Record log message from workflow process
   */
  recordLog(workflowName: string, level: 'stdout' | 'stderr', message: string): void {
    // Broadcast log message to all connected clients
    this.broadcast({
      type: 'log',
      data: {
        workflow: workflowName,
        level,
        message,
        timestamp: new Date().toISOString(),
      },
      timestamp: new Date(),
    });
  }

  /**
   * Record workflow end
   */
  recordWorkflowEnd(workflowName: string, result: ExecutionResult, duration: number): void {
    const metrics = this.activeWorkflows.get(workflowName);
    if (!metrics) {
      return;
    }

    metrics.endTime = new Date();
    metrics.duration = duration;
    
    // Use the actual counts from the execution result (source of truth)
    // This ensures accuracy even if step updates were missed or duplicated
    metrics.completedSteps = result.completedTasks.length;
    metrics.failedSteps = result.failedTasks.length;
    
    const totalProcessed = metrics.completedSteps + metrics.failedSteps;
    metrics.successRate = totalProcessed === 0 ? 0 :
      (metrics.completedSteps / totalProcessed) * 100;
    metrics.status = result.success ? 'completed' : 'failed';

    this.broadcast({
      type: 'workflow-end',
      data: { workflow: workflowName, metrics, result },
      timestamp: new Date(),
    });

    // Keep in active workflows for 5 minutes, then remove
    setTimeout(() => {
      this.activeWorkflows.delete(workflowName);
      this.stepStates.delete(workflowName); // Cleanup step states
    }, 5 * 60 * 1000);
  }

  /**
   * Record step update
   */
  recordStepUpdate(workflowName: string, stepName: string, status: 'running' | 'completed' | 'failed'): void {
    // Update scrum master state
    if (status === 'completed' || status === 'failed') {
      this.workflowManager.getScrumMaster().updateStepStatus(workflowName, stepName, status);
    }
    
    const metrics = this.activeWorkflows.get(workflowName);
    if (!metrics) {
      console.log(`⚠️  Dashboard: Workflow "${workflowName}" not found for step update "${stepName}"`);
      console.log(`   Available workflows: ${Array.from(this.activeWorkflows.keys()).join(', ')}`);
      // Try to find workflow by partial match (case-insensitive)
      const matchingKey = Array.from(this.activeWorkflows.keys()).find(
        key => key.toLowerCase() === workflowName.toLowerCase()
      );
      if (matchingKey) {
        console.log(`   Found similar workflow: "${matchingKey}" - using that instead`);
        this.recordStepUpdate(matchingKey, stepName, status);
        return;
      }
      return;
    }

    // Get step states for this workflow
    const workflowStepStates = this.stepStates.get(workflowName);
    if (!workflowStepStates) {
      // Initialize if not exists (shouldn't happen, but be safe)
      this.stepStates.set(workflowName, new Map());
      return;
    }

    // Get previous state of this step
    const previousState = workflowStepStates.get(stepName);

    // Update current step
    metrics.currentStep = stepName;
    
    // Only count transitions from non-final states to final states
    // This prevents double-counting when the same step is reported multiple times
    if (status === 'completed' && previousState !== 'completed') {
      // Only increment if this step wasn't already completed
      if (previousState === 'failed') {
        // If it was previously failed, decrement failed count
        metrics.failedSteps = Math.max(0, metrics.failedSteps - 1);
      }
      metrics.completedSteps++;
      workflowStepStates.set(stepName, 'completed');
    } else if (status === 'failed' && previousState !== 'failed') {
      // Only increment if this step wasn't already failed
      if (previousState === 'completed') {
        // If it was previously completed, decrement completed count
        metrics.completedSteps = Math.max(0, metrics.completedSteps - 1);
      }
      metrics.failedSteps++;
      workflowStepStates.set(stepName, 'failed');
    } else if (status === 'running' && !previousState) {
      // Track that this step started running (first time only)
      workflowStepStates.set(stepName, 'running');
    }

    // Recalculate duration (real-time)
    metrics.duration = Date.now() - metrics.startTime.getTime();

    // Recalculate success rate (real-time)
    const totalProcessed = metrics.completedSteps + metrics.failedSteps;
    if (totalProcessed > 0) {
      metrics.successRate = (metrics.completedSteps / totalProcessed) * 100;
    } else {
      metrics.successRate = 0;
    }

    // Update current activity from activity tracker
    const currentActivity = this.activityTracker.getCurrentActivity(workflowName);
    if (currentActivity) {
      metrics.currentActivity = currentActivity.details;
    }

    console.log(`📊 Dashboard: Step update - ${workflowName} / ${stepName} / ${status} (${metrics.completedSteps + metrics.failedSteps}/${metrics.totalSteps})`);

    this.broadcast({
      type: 'step-update',
      data: { workflow: workflowName, step: stepName, status, metrics },
      timestamp: new Date(),
    });
  }

  /**
   * Update current activity
   */
  updateActivity(workflowName: string, stepName: string, activity: string, type: 'file_read' | 'file_write' | 'file_edit' | 'command' | 'thinking' | 'analysis' | 'completion' = 'analysis'): void {
    this.activityTracker.recordActivity(workflowName, stepName, type, activity);
    
    // Update metrics with current activity
    const metrics = this.activeWorkflows.get(workflowName);
    if (metrics) {
      metrics.currentActivity = activity;
      this.broadcast({
        type: 'activity-update' as const,
        data: { workflow: workflowName, activity, type },
        timestamp: new Date(),
      });
    }
  }

  /**
   * Update metrics
   */
  updateMetrics(workflowName: string, updates: Partial<DashboardMetrics>): void {
    const metrics = this.activeWorkflows.get(workflowName);
    if (!metrics) {
      return;
    }

    Object.assign(metrics, updates);

    this.broadcast({
      type: 'metrics',
      data: { workflow: workflowName, metrics },
      timestamp: new Date(),
    });
  }

  /**
   * Record token usage
   */
  recordTokenUsage(workflowName: string, stepName: string, usage: TokenUsage): void {
    this.tokenUsageTracker.recordUsage(workflowName, usage);
    
    const metrics = this.activeWorkflows.get(workflowName);
    if (metrics) {
      metrics.currentTokenUsage = usage;
      metrics.totalTokens = this.tokenUsageTracker.getSummary(workflowName)?.totalTokens || 0;
      metrics.contextWindowPercent = usage.contextWindowPercent;
      
      // Update total cost
      if (usage.cost !== undefined) {
        const currentCost = metrics.totalCost || 0;
        metrics.totalCost = currentCost + usage.cost;
        
        // Update cost per step
        if (!metrics.costPerStep) {
          metrics.costPerStep = new Map();
        }
        const stepCost = metrics.costPerStep.get(stepName) || 0;
        metrics.costPerStep.set(stepName, stepCost + usage.cost);
      }
      
      this.broadcast({
        type: 'token-update',
        data: { workflow: workflowName, step: stepName, usage },
        timestamp: new Date(),
      });
    }
  }

  /**
   * Record model change
   */
  recordModelChange(workflowName: string, stepName: string, modelId: string): void {
    const success = this.modelManager.changeModel({
      workflowName,
      stepName,
      modelId,
      applyToRemaining: false,
    });

    if (success) {
      const metrics = this.activeWorkflows.get(workflowName);
      if (metrics) {
        metrics.currentModel = modelId;
        metrics.modelHistory = this.modelManager.getChangeHistory(workflowName).map(h => ({
          timestamp: h.timestamp,
          model: h.toModel,
          stepName: h.stepName,
        }));

        this.broadcast({
          type: 'model-change',
          data: { workflow: workflowName, step: stepName, model: modelId },
          timestamp: new Date(),
        });
      }
    }
  }

  /**
   * Record agent switch
   */
  recordAgentSwitch(workflowName: string, stepName: string, newAgent: string, reason?: string): void {
    const success = this.agentSwitcher.switchAgent({
      workflowName,
      stepName,
      newAgent,
      reason,
    });

    if (success) {
      const metrics = this.activeWorkflows.get(workflowName);
      if (metrics) {
        metrics.currentAgent = newAgent;
        metrics.agentSwitches = this.agentSwitcher.getSwitchHistory(workflowName);

        this.broadcast({
          type: 'agent-switch',
          data: { workflow: workflowName, step: stepName, agent: newAgent, reason },
          timestamp: new Date(),
        });
      }
    }
  }

  /**
   * Record output analysis and task progress
   */
  recordOutputAnalysis(
    workflowName: string,
    stepName: string,
    analysis: OutputAnalysis,
    result?: any
  ): void {
    this.outputMonitor.analyzeOutput(workflowName, stepName, result, {
      stepName,
      role: 'unknown',
      description: '',
      inputs: [],
      expectedOutput: '',
    } as any);

    const progress = this.taskProgressTracker.updateProgress(workflowName, stepName, analysis, result);
    
    const metrics = this.activeWorkflows.get(workflowName);
    if (metrics) {
      metrics.latestAnalysis = analysis;
      if (!metrics.taskProgress) {
        metrics.taskProgress = new Map();
      }
      metrics.taskProgress.set(stepName, progress);

      this.broadcast({
        type: 'progress-update',
        data: { workflow: workflowName, step: stepName, analysis, progress },
        timestamp: new Date(),
      });
    }
  }

  /**
   * Get services (for external access)
   */
  getServices() {
    return {
      tokenUsageTracker: this.tokenUsageTracker,
      modelManager: this.modelManager,
      agentSwitcher: this.agentSwitcher,
      outputMonitor: this.outputMonitor,
      taskProgressTracker: this.taskProgressTracker,
      activityTracker: this.activityTracker,
    };
  }

  /**
   * Start server
   */
  async start(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.server.on('error', (error: any) => {
        reject(error);
      });

      this.server.listen(this.port, async () => {
        console.log(`\n📊 Dashboard server running at http://localhost:${this.port}`);
        console.log(`   WebSocket: ws://localhost:${this.port}`);
        console.log(`   API: http://localhost:${this.port}/api/metrics\n`);
        
        // Automatically start MCP server when dashboard starts (non-blocking)
        // Use setImmediate to ensure it runs after the server is fully initialized
        setImmediate(async () => {
          try {
            // Wait a moment for dashboard to be fully ready
            await new Promise(resolve => setTimeout(resolve, 1500));
            
            console.log('🔌 Starting MCP server automatically...');
            
            // Try to start MCP server with retry logic
            let retries = 3;
            let started = false;
            
            while (retries > 0 && !started) {
              started = await this.workflowManager.startMcpServer(3001);
              
              if (started) {
                // Give it a moment to actually start
                await new Promise(resolve => setTimeout(resolve, 1000));
                
                // Verify it's actually running by checking the health endpoint
                const http = require('http');
                const isRunning = await new Promise<boolean>((resolve) => {
                  const req = http.request(
                    { host: 'localhost', port: 3001, path: '/mcp/health', method: 'GET', timeout: 2000 },
                    (res: any) => {
                      resolve(res.statusCode === 200);
                    }
                  );
                  req.on('error', () => resolve(false));
                  req.on('timeout', () => {
                    req.destroy();
                    resolve(false);
                  });
                  req.end();
                });
                
                if (isRunning) {
                  console.log(`✅ MCP Server started automatically on port 3001\n`);
                  break;
                } else {
                  console.log(`⚠️  MCP Server process started but not responding, retrying... (${retries - 1} attempts left)\n`);
                  retries--;
                  if (retries > 0) {
                    await new Promise(resolve => setTimeout(resolve, 2000));
                  }
                }
              } else {
                // Check if it's already running
                const http = require('http');
                const isRunning = await new Promise<boolean>((resolve) => {
                  const req = http.request(
                    { host: 'localhost', port: 3001, path: '/mcp/health', method: 'GET', timeout: 2000 },
                    (res: any) => {
                      resolve(res.statusCode === 200);
                    }
                  );
                  req.on('error', () => resolve(false));
                  req.on('timeout', () => {
                    req.destroy();
                    resolve(false);
                  });
                  req.end();
                });
                
                if (isRunning) {
                  console.log(`ℹ️  MCP Server is already running on port 3001\n`);
                  started = true;
                  break;
                } else {
                  retries--;
                  if (retries > 0) {
                    console.log(`⚠️  MCP Server could not be started, retrying... (${retries} attempts left)\n`);
                    await new Promise(resolve => setTimeout(resolve, 2000));
                  }
                }
              }
            }
            
            if (!started) {
              console.log(`❌ MCP Server failed to start after 3 attempts. You can start it manually from the dashboard.\n`);
            }
          } catch (error: any) {
            // Log error but don't fail dashboard startup
            if (error.code !== 'EADDRINUSE') {
              console.log(`⚠️  MCP Server could not be started: ${error.message}\n`);
            } else {
              console.log(`ℹ️  MCP Server port 3001 is already in use (likely already running)\n`);
            }
          }
        });
        
        resolve();
      });
    });
  }

  /**
   * Check if MCP server is running
   */
  private async checkMcpServerRunning(port: number): Promise<boolean> {
    return new Promise((resolve) => {
      const http = require('http');
      const req = http.request(
        {
          host: 'localhost',
          port,
          path: '/mcp/health',
          method: 'GET',
          timeout: 1000,
        },
        (res: any) => {
          resolve(res.statusCode === 200 || res.statusCode === 404);
        }
      );

      req.on('error', () => resolve(false));
      req.on('timeout', () => {
        req.destroy();
        resolve(false);
      });

      req.end();
    });
  }

  /**
   * Stop server
   */
  stop(): Promise<void> {
    return new Promise(async (resolve) => {
      // Stop MCP server if it was started by the dashboard
      try {
        await this.workflowManager.stopMcpServer();
      } catch (error) {
        // Ignore errors stopping MCP server
      }

      // Stop cleanup interval
      if (this.cleanupInterval) {
        clearInterval(this.cleanupInterval);
        this.cleanupInterval = undefined;
      }

      // Stop real-time updates
      if (this.updateInterval) {
        clearInterval(this.updateInterval);
        this.updateInterval = undefined;
      }

      // Close all WebSocket connections
      for (const client of this.clients) {
        try {
          client.terminate();
        } catch (error) {
          // Ignore errors
        }
      }
      this.clients.clear();

      // Clear workflows and step states
      this.activeWorkflows.clear();
      this.stepStates.clear();

      this.wss.close(() => {
        this.server.close(() => {
          console.log('📊 Dashboard server stopped');
          resolve();
        });
      });
    });
  }

  /**
   * Get dashboard HTML (DEPRECATED - No longer used, React dashboard is served from dist/)
   * This method is kept for reference but is never called.
   */
  private getDashboardHTML(): string {
    // This method is deprecated and not used. The React dashboard is served from dist/
    return '';
    return `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Maestro Dashboard</title>
  <style>
    * { margin: 0; padding: 0; box-sizing: border-box; }
    body {
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
      background: linear-gradient(135deg, #0a0e27 0%, #1a1f3a 50%, #0f1422 100%);
      background-attachment: fixed;
      color: #e0e0e0;
      padding: 20px;
      min-height: 100vh;
    }
    .header {
      text-align: center;
      margin-bottom: 30px;
      padding: 30px;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%);
      border-radius: 15px;
      box-shadow: 0 10px 40px rgba(102, 126, 234, 0.3);
      position: relative;
      overflow: hidden;
    }
    .header::before {
      content: '';
      position: absolute;
      top: -50%;
      left: -50%;
      width: 200%;
      height: 200%;
      background: radial-gradient(circle, rgba(255,255,255,0.1) 0%, transparent 70%);
      animation: shimmer 3s infinite;
    }
          @keyframes shimmer {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
          }
          @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
          }
    .header h1 { 
      color: white; 
      margin-bottom: 10px; 
      font-size: 2.5em;
      font-weight: 700;
      text-shadow: 0 2px 10px rgba(0,0,0,0.3);
      position: relative;
      z-index: 1;
    }
    .status { 
      display: inline-block; 
      padding: 8px 20px; 
      background: rgba(255,255,255,0.25); 
      border-radius: 25px; 
      backdrop-filter: blur(10px);
      font-weight: 600;
      position: relative;
      z-index: 1;
    }
    .controls {
      background: linear-gradient(135deg, rgba(26, 31, 58, 0.95) 0%, rgba(15, 20, 34, 0.95) 100%);
      border-radius: 15px;
      padding: 25px;
      margin-bottom: 30px;
      border: 1px solid rgba(102, 126, 234, 0.3);
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
      backdrop-filter: blur(10px);
    }
    .controls h2 {
      margin-bottom: 20px;
      color: #667eea;
      font-size: 20px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 1px;
    }
    .control-group {
      display: flex;
      gap: 10px;
      align-items: center;
      margin-bottom: 15px;
      flex-wrap: wrap;
    }
    .control-group label {
      min-width: 120px;
      color: #aaa;
    }
    .control-group select, .control-group input {
      flex: 1;
      padding: 12px 40px 12px 16px;
      background: linear-gradient(135deg, rgba(15, 20, 34, 0.95) 0%, rgba(26, 31, 58, 0.95) 100%);
      border: 2px solid rgba(102, 126, 234, 0.4);
      border-radius: 10px;
      color: #e0e0e0;
      min-width: 200px;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      font-size: 14px;
      font-weight: 500;
      cursor: pointer;
      appearance: none;
      -webkit-appearance: none;
      -moz-appearance: none;
      background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%23667eea' d='M6 9L1 4h10z'/%3E%3C/svg%3E");
      background-repeat: no-repeat;
      background-position: right 14px center;
      background-size: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
      backdrop-filter: blur(10px);
    }
    .control-group input {
      padding: 12px 16px;
      background-image: none;
      cursor: text;
    }
    .control-group select:focus, .control-group input:focus {
      outline: none;
      border-color: rgba(102, 126, 234, 0.8);
      background: linear-gradient(135deg, rgba(20, 25, 44, 1) 0%, rgba(31, 36, 68, 1) 100%);
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.2), 0 4px 16px rgba(102, 126, 234, 0.3);
      transform: translateY(-1px);
    }
    .control-group select:hover, .control-group input:hover {
      border-color: rgba(102, 126, 234, 0.6);
      background: linear-gradient(135deg, rgba(20, 25, 44, 0.98) 0%, rgba(31, 36, 68, 0.98) 100%);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);
      transform: translateY(-1px);
    }
    .control-group input:hover {
      transform: none;
    }
    .btn {
      padding: 10px 20px;
      border: none;
      border-radius: 8px;
      cursor: pointer;
      font-weight: 600;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
      position: relative;
      overflow: hidden;
    }
    .btn::before {
      content: '';
      position: absolute;
      top: 50%;
      left: 50%;
      width: 0;
      height: 0;
      border-radius: 50%;
      background: rgba(255, 255, 255, 0.2);
      transform: translate(-50%, -50%);
      transition: width 0.6s, height 0.6s;
    }
    .btn:hover::before {
      width: 300px;
      height: 300px;
    }
    .btn-primary {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }
    .btn-primary:hover {
      background: linear-gradient(135deg, #5568d3 0%, #6a3d91 100%);
      transform: translateY(-2px);
      box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
    }
    .btn-danger {
      background: linear-gradient(135deg, #e53e3e 0%, #c53030 100%);
      color: white;
    }
    .btn-danger:hover {
      background: linear-gradient(135deg, #c53030 0%, #9b2c2c 100%);
      transform: translateY(-2px);
      box-shadow: 0 6px 20px rgba(229, 62, 62, 0.4);
    }
    .btn-success {
      background: linear-gradient(135deg, #38a169 0%, #2f855a 100%);
      color: white;
    }
    .btn-success:hover {
      background: linear-gradient(135deg, #2f855a 0%, #276749 100%);
      transform: translateY(-2px);
      box-shadow: 0 6px 20px rgba(56, 161, 105, 0.4);
    }
    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
      transform: none !important;
    }
    .status-badge {
      display: inline-block;
      padding: 6px 16px;
      border-radius: 20px;
      font-size: 12px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
    }
    .status-running { 
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      animation: pulse 2s infinite;
    }
    @keyframes pulse {
      0%, 100% { opacity: 1; transform: scale(1); }
      50% { opacity: 0.5; transform: scale(1.2); }
    }
    .status-stopped { 
      background: linear-gradient(135deg, #718096 0%, #4a5568 100%);
      color: white;
    }
    .modal {
      display: none;
      position: fixed;
      z-index: 1000;
      left: 0;
      top: 0;
      width: 100%;
      height: 100%;
      background-color: rgba(0,0,0,0.7);
    }
    .modal-content {
      background: linear-gradient(135deg, rgba(26, 31, 58, 0.98) 0%, rgba(15, 20, 34, 0.98) 100%);
      margin: 5% auto;
      padding: 35px;
      border: 1px solid rgba(102, 126, 234, 0.3);
      border-radius: 20px;
      width: 90%;
      max-width: 800px;
      max-height: 80vh;
      overflow-y: auto;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5);
      backdrop-filter: blur(20px);
    }
    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 25px;
      padding-bottom: 15px;
      border-bottom: 2px solid rgba(102, 126, 234, 0.3);
    }
    .modal-header h2 {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
      margin: 0;
      font-size: 1.8em;
      font-weight: 700;
    }
    .close {
      color: #aaa;
      font-size: 28px;
      font-weight: bold;
      cursor: pointer;
    }
    .close:hover {
      color: #fff;
    }
    .form-group {
      margin-bottom: 15px;
    }
    .form-group label {
      display: block;
      margin-bottom: 5px;
      color: #aaa;
      font-size: 14px;
    }
    .form-group input, .form-group textarea, .form-group select {
      width: 100%;
      padding: 12px 16px;
      background: linear-gradient(135deg, rgba(10, 14, 39, 0.9) 0%, rgba(26, 31, 58, 0.9) 100%);
      border: 1px solid rgba(102, 126, 234, 0.3);
      border-radius: 8px;
      color: #e0e0e0;
      font-size: 14px;
      transition: all 0.3s ease;
    }
    .form-group input:focus, .form-group textarea:focus, .form-group select:focus {
      outline: none;
      border-color: rgba(102, 126, 234, 0.6);
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }
    
    /* Beautiful Select/Dropdown Styling */
    select {
      width: 100%;
      padding: 12px 40px 12px 16px;
      background: linear-gradient(135deg, rgba(15, 20, 34, 0.95) 0%, rgba(26, 31, 58, 0.95) 100%);
      border: 2px solid rgba(102, 126, 234, 0.4);
      border-radius: 10px;
      color: #e0e0e0;
      font-size: 14px;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      appearance: none;
      -webkit-appearance: none;
      -moz-appearance: none;
      background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%23667eea' d='M6 9L1 4h10z'/%3E%3C/svg%3E");
      background-repeat: no-repeat;
      background-position: right 14px center;
      background-size: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
      backdrop-filter: blur(10px);
    }
    
    select:hover {
      border-color: rgba(102, 126, 234, 0.6);
      background: linear-gradient(135deg, rgba(20, 25, 44, 0.98) 0%, rgba(31, 36, 68, 0.98) 100%);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);
      transform: translateY(-1px);
    }
    
    select:focus {
      outline: none;
      border-color: rgba(102, 126, 234, 0.8);
      background: linear-gradient(135deg, rgba(20, 25, 44, 1) 0%, rgba(31, 36, 68, 1) 100%);
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.2), 0 4px 16px rgba(102, 126, 234, 0.3);
      transform: translateY(-1px);
    }
    
    select:active {
      transform: translateY(0);
    }
    
    select:disabled {
      opacity: 0.5;
      cursor: not-allowed;
      background: rgba(15, 20, 34, 0.5);
      border-color: rgba(102, 126, 234, 0.2);
    }
    
    /* Option Styling */
    select option {
      background: rgba(15, 20, 34, 0.98);
      color: #e0e0e0;
      padding: 12px 16px;
      font-size: 14px;
      font-weight: 500;
    }
    
    select option:hover {
      background: rgba(102, 126, 234, 0.2);
    }

    /* Sexy Toggle Switch Styles */
    .toggle-switch {
      position: relative;
      display: inline-block;
      width: 60px;
      height: 32px;
    }

    .toggle-switch input {
      opacity: 0;
      width: 0;
      height: 0;
    }

    .toggle-slider {
      position: absolute;
      cursor: pointer;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background-color: #666;
      transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
      border-radius: 34px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3), inset 0 1px 2px rgba(255, 255, 255, 0.1);
    }

    .toggle-slider:before {
      position: absolute;
      content: "";
      height: 24px;
      width: 24px;
      left: 4px;
      bottom: 4px;
      background: linear-gradient(135deg, #ffffff 0%, #f0f0f0 100%);
      transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
      border-radius: 50%;
      box-shadow: 0 2px 6px rgba(0, 0, 0, 0.4), 0 0 0 2px rgba(102, 102, 102, 0.2);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 10px;
    }

    .toggle-switch input:checked + .toggle-slider {
      background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
      box-shadow: 0 2px 12px rgba(76, 175, 80, 0.4), inset 0 1px 2px rgba(255, 255, 255, 0.2);
    }

    .toggle-switch input:checked + .toggle-slider:before {
      transform: translateX(28px);
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.4), 0 0 0 2px rgba(76, 175, 80, 0.3);
    }

    .toggle-switch input:disabled + .toggle-slider {
      opacity: 0.5;
      cursor: not-allowed;
      background-color: #444;
    }

    .toggle-switch input:disabled + .toggle-slider:before {
      cursor: not-allowed;
    }

    .toggle-switch:hover:not(:has(input:disabled)) .toggle-slider {
      box-shadow: 0 3px 12px rgba(0, 0, 0, 0.4), inset 0 1px 2px rgba(255, 255, 255, 0.15);
    }

    .toggle-switch input:checked:hover:not(:disabled) + .toggle-slider {
      box-shadow: 0 3px 16px rgba(76, 175, 80, 0.5), inset 0 1px 2px rgba(255, 255, 255, 0.25);
    }

    .toggle-knob {
      position: absolute;
      height: 24px;
      width: 24px;
      left: 4px;
      bottom: 4px;
      background: linear-gradient(135deg, #ffffff 0%, #f0f0f0 100%);
      transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
      border-radius: 50%;
      box-shadow: 0 2px 6px rgba(0, 0, 0, 0.4), 0 0 0 2px rgba(102, 102, 102, 0.2);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 10px;
      color: #333;
      font-weight: 700;
    }

    .toggle-switch input:checked ~ .toggle-knob,
    .toggle-switch input:checked + .toggle-slider .toggle-knob {
      transform: translateX(28px);
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.4), 0 0 0 2px rgba(76, 175, 80, 0.3);
    }
    
    select option:checked {
      background: linear-gradient(135deg, rgba(102, 126, 234, 0.3) 0%, rgba(118, 75, 162, 0.3) 100%);
      color: #fff;
      font-weight: 600;
    }
    
    /* Optgroup Styling */
    select optgroup {
      background: rgba(26, 31, 58, 0.95);
      color: #667eea;
      font-weight: 700;
      font-size: 12px;
      text-transform: uppercase;
      letter-spacing: 1px;
      padding: 8px 12px;
    }
    
    select optgroup option {
      padding-left: 24px;
      font-weight: 400;
      text-transform: none;
      letter-spacing: normal;
    }
    .form-group textarea {
      min-height: 60px;
      resize: vertical;
    }
    .step-item {
      background: linear-gradient(135deg, rgba(15, 20, 34, 0.8) 0%, rgba(26, 31, 58, 0.8) 100%);
      padding: 18px;
      border-radius: 12px;
      margin-bottom: 12px;
      border: 1px solid rgba(102, 126, 234, 0.2);
      transition: all 0.3s ease;
      backdrop-filter: blur(10px);
    }
    .step-item:hover {
      transform: translateX(5px);
      border-color: rgba(102, 126, 234, 0.4);
      box-shadow: 0 4px 15px rgba(102, 126, 234, 0.2);
    }
    .step-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 10px;
    }
    .step-number {
      font-weight: bold;
      color: #667eea;
    }
    .step-actions {
      display: flex;
      gap: 5px;
    }
    .btn-small {
      padding: 4px 8px;
      font-size: 12px;
    }
    .workflows {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
      gap: 20px;
    }
    .workflow-card {
      background: linear-gradient(135deg, rgba(26, 31, 58, 0.95) 0%, rgba(15, 20, 34, 0.95) 100%);
      border-radius: 15px;
      padding: 25px;
      border: 1px solid rgba(102, 126, 234, 0.3);
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
      backdrop-filter: blur(10px);
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    }
    .workflow-card:hover {
      transform: translateY(-5px);
      box-shadow: 0 12px 40px rgba(102, 126, 234, 0.2);
      border-color: rgba(102, 126, 234, 0.5);
    }
    .workflow-card h2 { 
      color: #667eea; 
      margin-bottom: 20px; 
      font-size: 1.5em;
      font-weight: 700;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }
    .metrics {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 15px;
      margin-top: 15px;
    }
    .metric {
      background: linear-gradient(135deg, rgba(15, 20, 34, 0.8) 0%, rgba(26, 31, 58, 0.8) 100%);
      padding: 15px;
      border-radius: 12px;
      border: 1px solid rgba(102, 126, 234, 0.2);
      transition: all 0.3s ease;
      backdrop-filter: blur(10px);
    }
    .metric:hover {
      transform: translateY(-2px);
      border-color: rgba(102, 126, 234, 0.4);
      box-shadow: 0 4px 15px rgba(102, 126, 234, 0.2);
    }
    .metric-label { 
      font-size: 11px; 
      color: #aaa; 
      text-transform: uppercase;
      letter-spacing: 1px;
      font-weight: 600;
      margin-bottom: 8px;
    }
    .metric-value { 
      font-size: 24px; 
      font-weight: 700; 
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }
    .status-badge {
      display: inline-block;
      padding: 6px 14px;
      border-radius: 20px;
      font-size: 11px;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      margin-top: 10px;
      box-shadow: 0 2px 10px rgba(0, 0, 0, 0.3);
    }
    .status-running { 
      background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
      color: white;
      animation: pulse-glow 2s infinite;
    }
    .status-completed { 
      background: linear-gradient(135deg, #2196F3 0%, #1976d2 100%);
      color: white;
    }
    .status-failed { 
      background: linear-gradient(135deg, #f44336 0%, #d32f2f 100%);
      color: white;
    }
    .status-paused { 
      background: linear-gradient(135deg, #ff9800 0%, #f57c00 100%);
      color: white;
    }
    @keyframes pulse-glow {
      0%, 100% { 
        opacity: 1;
        box-shadow: 0 2px 10px rgba(76, 175, 80, 0.4);
      }
      50% { 
        opacity: 0.9;
        box-shadow: 0 2px 20px rgba(76, 175, 80, 0.6);
      }
    }
    .progress-bar {
      width: 100%;
      height: 10px;
      background: rgba(15, 20, 34, 0.5);
      border-radius: 10px;
      overflow: hidden;
      margin-top: 10px;
      box-shadow: inset 0 2px 5px rgba(0, 0, 0, 0.3);
    }
    .progress-fill {
      height: 100%;
      background: linear-gradient(90deg, #667eea 0%, #764ba2 50%, #f093fb 100%);
      transition: width 0.5s cubic-bezier(0.4, 0, 0.2, 1);
      box-shadow: 0 0 10px rgba(102, 126, 234, 0.5);
      position: relative;
      overflow: hidden;
    }
    .progress-fill::after {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      bottom: 0;
      right: 0;
      background: linear-gradient(90deg, transparent, rgba(255,255,255,0.3), transparent);
      animation: shimmer-progress 2s infinite;
    }
    @keyframes shimmer-progress {
      0% { transform: translateX(-100%); }
      100% { transform: translateX(100%); }
    }
    .no-workflows {
      text-align: center;
      padding: 60px;
      color: #888;
      font-size: 18px;
      background: linear-gradient(135deg, rgba(26, 31, 58, 0.3) 0%, rgba(15, 20, 34, 0.3) 100%);
      border-radius: 15px;
      border: 2px dashed rgba(102, 126, 234, 0.3);
    }
    
    /* Tooltip Styles */
    .tooltip-container {
      position: relative;
      display: inline-block;
      cursor: help;
    }
    .help-icon {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 18px;
      height: 18px;
      border-radius: 50%;
      background: rgba(102, 126, 234, 0.2);
      color: #667eea;
      font-size: 12px;
      font-weight: bold;
      margin-left: 6px;
      cursor: help;
      transition: all 0.2s ease;
      vertical-align: middle;
    }
    .help-icon:hover {
      background: rgba(102, 126, 234, 0.4);
      transform: scale(1.1);
    }
    .tooltip {
      visibility: hidden;
      opacity: 0;
      position: absolute;
      z-index: 10000;
      bottom: 125%;
      left: 50%;
      transform: translateX(-50%);
      background: linear-gradient(135deg, rgba(26, 31, 58, 0.98) 0%, rgba(15, 20, 34, 0.98) 100%);
      color: #e0e0e0;
      padding: 12px 16px;
      border-radius: 8px;
      font-size: 13px;
      font-weight: 400;
      white-space: normal;
      width: 280px;
      max-width: 90vw;
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.5);
      border: 1px solid rgba(102, 126, 234, 0.3);
      backdrop-filter: blur(20px);
      transition: opacity 0.3s ease, visibility 0.3s ease, transform 0.3s ease;
      transform: translateX(-50%) translateY(-5px);
      pointer-events: none;
      line-height: 1.5;
    }
    .tooltip::after {
      content: '';
      position: absolute;
      top: 100%;
      left: 50%;
      transform: translateX(-50%);
      border: 6px solid transparent;
      border-top-color: rgba(26, 31, 58, 0.98);
    }
    .tooltip-container:hover .tooltip,
    .help-icon:hover + .tooltip,
    .tooltip-container:focus .tooltip {
      visibility: visible;
      opacity: 1;
      transform: translateX(-50%) translateY(0);
    }
    .tooltip-right {
      left: auto;
      right: 0;
      transform: translateX(0) translateY(-50%);
      top: 50%;
      bottom: auto;
    }
    .tooltip-right::after {
      top: 50%;
      left: -12px;
      right: auto;
      transform: translateY(-50%);
      border-left-color: rgba(26, 31, 58, 0.98);
      border-top-color: transparent;
    }
    .tooltip-left {
      left: 0;
      right: auto;
      transform: translateX(0) translateY(-50%);
      top: 50%;
      bottom: auto;
    }
    .tooltip-left::after {
      top: 50%;
      right: -12px;
      left: auto;
      transform: translateY(-50%);
      border-right-color: rgba(26, 31, 58, 0.98);
      border-top-color: transparent;
    }
    .help-text {
      font-size: 12px;
      color: #aaa;
      margin-top: 5px;
      line-height: 1.4;
    }
    .help-section {
      background: rgba(102, 126, 234, 0.1);
      border-left: 3px solid rgba(102, 126, 234, 0.5);
      padding: 12px 15px;
      margin-top: 10px;
      border-radius: 5px;
      font-size: 13px;
      color: #ccc;
      line-height: 1.5;
    }
    .system-info-panel {
      animation: fadeIn 0.5s ease-in;
    }
    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(-10px); }
      to { opacity: 1; transform: translateY(0); }
    }
    @keyframes slideIn {
      from { 
        opacity: 0; 
        transform: translateX(100%); 
      }
      to { 
        opacity: 1; 
        transform: translateX(0); 
      }
    }
    @keyframes slideOut {
      from { 
        opacity: 1; 
        transform: translateX(0); 
      }
      to { 
        opacity: 0; 
        transform: translateX(100%); 
      }
    }
    .system-metric {
      padding: 12px;
      background: rgba(15, 20, 34, 0.4);
      border-radius: 8px;
      border: 1px solid rgba(102, 126, 234, 0.2);
      transition: all 0.3s ease;
    }
    .system-metric:hover {
      transform: translateY(-2px);
      border-color: rgba(102, 126, 234, 0.4);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);
    }
    .help-link {
      color: #667eea;
      text-decoration: none;
      font-weight: 600;
      border-bottom: 1px dotted rgba(102, 126, 234, 0.5);
      transition: all 0.2s ease;
    }
    .help-link:hover {
      color: #764ba2;
      border-bottom-color: rgba(118, 75, 162, 0.5);
    }
  </style>
</head>
<body>
  <div class="header">
    <h1>🎼 Maestro Dashboard</h1>
    <div class="status" id="connectionStatus">Connecting...</div>
    <div id="script-status" style="position: fixed; top: 10px; right: 10px; padding: 10px; background: rgba(76, 175, 80, 0.9); color: white; border-radius: 6px; font-size: 12px; z-index: 10000; font-weight: 600;">✅ Script Loaded</div>
    <div class="help-section" style="margin-top: 15px; text-align: left; max-width: 800px; margin-left: auto; margin-right: auto;">
      <strong style="color: #667eea;">Welcome to Maestro!</strong> Orchestrate AI agents to execute complex workflows. 
      <span class="tooltip-container">
        <span class="help-icon">?</span>
        <span class="tooltip">
          Maestro coordinates multiple AI agents (Architect, Backend, Frontend, etc.) to complete complex tasks. 
          Workflows are defined as a series of steps, each assigned to a specific agent role.
        </span>
      </span>
    </div>
  </div>

  <!-- System Information Panel -->
  <div class="system-info-panel" style="margin-bottom: 20px; padding: 20px; background: linear-gradient(135deg, rgba(26, 31, 58, 0.8) 0%, rgba(15, 20, 34, 0.8) 100%); border-radius: 12px; border: 1px solid rgba(102, 126, 234, 0.3); backdrop-filter: blur(10px); box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);">
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px;">
      <h2 style="margin: 0; color: #667eea; font-size: 18px; font-weight: 600;">
        💻 System Information
        <span class="tooltip-container">
          <span class="help-icon" style="font-size: 12px; width: 14px; height: 14px;">?</span>
          <span class="tooltip" style="width: 250px; font-size: 12px;">
            Real-time system information about the Maestro process. Shows process ID, memory usage, CPU usage, and platform details. Updates every 2 seconds.
          </span>
        </span>
      </h2>
      <div style="font-size: 10px; color: #888; font-style: italic;">Live Updates</div>
    </div>
    
    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px;">
      <!-- Process ID -->
      <div class="system-metric">
        <div class="metric-label" style="font-size: 11px; color: #888; margin-bottom: 5px;">
          Process ID
          <span class="tooltip-container">
            <span class="help-icon" style="font-size: 9px; width: 12px; height: 12px;">?</span>
            <span class="tooltip" style="width: 200px; font-size: 12px;">
              The operating system process identifier (PID) for the Maestro dashboard server.
            </span>
          </span>
        </div>
        <div id="system-pid" style="font-size: 16px; color: #667eea; font-weight: 700; font-family: 'Courier New', monospace;">-</div>
      </div>

      <!-- Uptime -->
      <div class="system-metric">
        <div class="metric-label" style="font-size: 11px; color: #888; margin-bottom: 5px;">
          Uptime
          <span class="tooltip-container">
            <span class="help-icon" style="font-size: 9px; width: 12px; height: 12px;">?</span>
            <span class="tooltip" style="width: 200px; font-size: 12px;">
              How long the Maestro dashboard has been running since it was started.
            </span>
          </span>
        </div>
        <div id="system-uptime" style="font-size: 16px; color: #4CAF50; font-weight: 700;">-</div>
      </div>

      <!-- Memory Usage -->
      <div class="system-metric">
        <div class="metric-label" style="font-size: 11px; color: #888; margin-bottom: 5px;">
          Memory Usage
          <span class="tooltip-container">
            <span class="help-icon" style="font-size: 9px; width: 12px; height: 12px;">?</span>
            <span class="tooltip" style="width: 200px; font-size: 12px;">
              Total system memory usage. Shows used memory out of total available system memory.
            </span>
          </span>
        </div>
        <div id="system-memory" style="font-size: 16px; color: #2196F3; font-weight: 700;">-</div>
        <div id="system-memory-percent" style="font-size: 11px; color: #888; margin-top: 3px;">-</div>
        <div style="width: 100%; height: 6px; background: rgba(15, 20, 34, 0.5); border-radius: 3px; margin-top: 5px; overflow: hidden;">
          <div id="system-memory-bar" style="height: 100%; background: linear-gradient(90deg, #2196F3 0%, #21CBF3 100%); width: 0%; transition: width 0.5s ease; border-radius: 3px;"></div>
        </div>
      </div>

      <!-- Heap Memory -->
      <div class="system-metric">
        <div class="metric-label" style="font-size: 11px; color: #888; margin-bottom: 5px;">
          Heap Memory
          <span class="tooltip-container">
            <span class="help-icon" style="font-size: 9px; width: 12px; height: 12px;">?</span>
            <span class="tooltip" style="width: 250px; font-size: 12px;">
              Node.js heap memory usage. Shows current usage vs configured maximum limit (--max-old-space-size). The limit is set to prevent crashes during large workflows.
            </span>
          </span>
        </div>
        <div id="system-heap" style="font-size: 16px; color: #9C27B0; font-weight: 700;">-</div>
        <div id="system-heap-limit" style="font-size: 11px; color: #888; margin-top: 3px;">-</div>
        <div style="width: 100%; height: 6px; background: rgba(15, 20, 34, 0.5); border-radius: 3px; margin-top: 5px; overflow: hidden;">
          <div id="system-heap-bar" style="height: 100%; background: linear-gradient(90deg, #9C27B0 0%, #E91E63 100%); width: 0%; transition: width 0.5s ease; border-radius: 3px;"></div>
        </div>
      </div>

      <!-- CPU Usage -->
      <div class="system-metric">
        <div class="metric-label" style="font-size: 11px; color: #888; margin-bottom: 5px;">
          CPU Usage
          <span class="tooltip-container">
            <span class="help-icon" style="font-size: 9px; width: 12px; height: 12px;">?</span>
            <span class="tooltip" style="width: 200px; font-size: 12px;">
              Current CPU usage percentage for the Maestro process. Calculated based on process CPU time.
            </span>
          </span>
        </div>
        <div id="system-cpu" style="font-size: 16px; color: #FF9800; font-weight: 700;">-</div>
        <div style="width: 100%; height: 6px; background: rgba(15, 20, 34, 0.5); border-radius: 3px; margin-top: 5px; overflow: hidden;">
          <div id="system-cpu-bar" style="height: 100%; background: linear-gradient(90deg, #FF9800 0%, #FF5722 100%); width: 0%; transition: width 0.5s ease; border-radius: 3px;"></div>
        </div>
      </div>

      <!-- Platform Info -->
      <div class="system-metric">
        <div class="metric-label" style="font-size: 11px; color: #888; margin-bottom: 5px;">
          Platform
          <span class="tooltip-container">
            <span class="help-icon" style="font-size: 9px; width: 12px; height: 12px;">?</span>
            <span class="tooltip" style="width: 200px; font-size: 12px;">
              Operating system and architecture information. Shows the platform type, architecture, and Node.js version.
            </span>
          </span>
        </div>
        <div id="system-platform" style="font-size: 13px; color: #e0e0e0; font-weight: 600; line-height: 1.4;">-</div>
        <div id="system-hostname" style="font-size: 10px; color: #888; margin-top: 3px;">-</div>
      </div>
    </div>
  </div>
  
    <!-- Quick Create Workflow Button (minimal, at top) -->
    <div style="margin-bottom: 20px; text-align: center;">
      <button class="btn btn-success" id="create-workflow-btn" style="padding: 12px 24px; font-size: 15px; font-weight: 600;" onclick="window.showCreateWorkflowModal()">
        ➕ Create New Workflow
      </button>
    </div>

  <div class="controls">
    <h2>
      🔌 MCP Server
      <span class="tooltip-container">
        <span class="help-icon">?</span>
        <span class="tooltip">
          Model Context Protocol (MCP) Server exposes Maestro's workflow state and operations via a standardized protocol. 
          This allows external tools and AI assistants to query workflow status, access resources, and execute tools.
        </span>
      </span>
    </h2>
    <div class="control-group">
      <label>
        Status:
        <span class="tooltip-container">
          <span class="help-icon">?</span>
          <span class="tooltip">
            Current status of the MCP server. When running, the server is accessible on the configured port 
            and can respond to MCP protocol requests.
          </span>
        </span>
      </label>
      <span id="mcp-status" class="status-badge status-running" title="MCP Server is running and accepting connections">Running</span>
      <button class="btn btn-success" id="start-mcp-btn" onclick="startMcpServer()" style="display: none;" title="Start the MCP server on port 3001">
        Start MCP Server
      </button>
      <button class="btn btn-danger" id="stop-mcp-btn" onclick="stopMcpServer()" title="Stop the MCP server">
        Stop MCP Server
      </button>
      <span id="mcp-port" style="color: #667eea; margin-left: 10px; font-weight: 600;" title="Port number where MCP server is listening">Port: 3001</span>
    </div>
    
    <!-- MCP Server Details Panel -->
    <div id="mcp-details-panel" style="margin-top: 15px; padding: 15px; background: rgba(26, 31, 58, 0.5); border-radius: 8px; border: 1px solid #2a2f4a; display: none;">
      <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px; margin-bottom: 15px;">
        <div class="metric">
          <div class="metric-label" style="font-size: 11px; color: #888;">Health Status
            <span class="tooltip-container">
              <span class="help-icon" style="font-size: 9px; width: 12px; height: 12px;">?</span>
              <span class="tooltip" style="width: 200px; font-size: 12px;">
                Current health status of the MCP server. "Healthy" means the server is running and responding to requests.
              </span>
            </span>
          </div>
          <div id="mcp-health" style="font-size: 14px; color: #4CAF50; font-weight: 600;">-</div>
        </div>
        <div class="metric">
          <div class="metric-label" style="font-size: 11px; color: #888;">Resources
            <span class="tooltip-container">
              <span class="help-icon" style="font-size: 9px; width: 12px; height: 12px;">?</span>
              <span class="tooltip" style="width: 200px; font-size: 12px;">
                Number of resources exposed by the MCP server. Resources provide access to workflow data and state.
              </span>
            </span>
          </div>
          <div id="mcp-resource-count" style="font-size: 14px; color: #667eea; font-weight: 600;">0</div>
        </div>
        <div class="metric">
          <div class="metric-label" style="font-size: 11px; color: #888;">Tools
            <span class="tooltip-container">
              <span class="help-icon" style="font-size: 9px; width: 12px; height: 12px;">?</span>
              <span class="tooltip" style="width: 200px; font-size: 12px;">
                Number of tools available via MCP. Tools allow external systems to interact with Maestro workflows.
              </span>
            </span>
          </div>
          <div id="mcp-tool-count" style="font-size: 14px; color: #2196F3; font-weight: 600;">0</div>
        </div>
        <div class="metric">
          <div class="metric-label" style="font-size: 11px; color: #888;">Base URL
            <span class="tooltip-container">
              <span class="help-icon" style="font-size: 9px; width: 12px; height: 12px;">?</span>
              <span class="tooltip" style="width: 200px; font-size: 12px;">
                Base URL for accessing the MCP server. Use this URL to connect external tools and AI assistants.
              </span>
            </span>
          </div>
          <div id="mcp-base-url" style="font-size: 11px; color: #888; word-break: break-all;">-</div>
        </div>
      </div>
      
      <!-- Resources List -->
      <div id="mcp-resources-section" style="margin-top: 15px; display: none;">
        <div style="font-size: 12px; color: #667eea; font-weight: 600; margin-bottom: 10px;">
          📋 Available Resources
        </div>
        <div id="mcp-resources-list" style="max-height: 150px; overflow-y: auto; font-size: 11px;">
          <!-- Resources will be populated here -->
        </div>
      </div>
      
      <!-- Tools List -->
      <div id="mcp-tools-section" style="margin-top: 15px; display: none;">
        <div style="font-size: 12px; color: #2196F3; font-weight: 600; margin-bottom: 10px;">
          🛠️ Available Tools
        </div>
        <div id="mcp-tools-list" style="max-height: 150px; overflow-y: auto; font-size: 11px;">
          <!-- Tools will be populated here -->
        </div>
      </div>
    </div>
    
    <div class="help-section">
      <strong>💡 MCP Server Features:</strong>
      <ul style="margin: 8px 0 0 20px; padding: 0;">
        <li>Expose workflow resources (active workflows, status, activity)</li>
        <li>Provide tools for workflow management</li>
        <li>Enable integration with AI assistants and external tools</li>
        <li>Access via: <code style="background: rgba(102, 126, 234, 0.2); padding: 2px 6px; border-radius: 3px;">http://localhost:3001/mcp/</code></li>
      </ul>
    </div>
  </div>
  
  <!-- All Workflows Panel (shows all workflows, running or not) -->
  <div style="margin-bottom: 20px;">
    <h2 style="margin: 0 0 15px 0; color: #667eea; font-size: 20px; font-weight: 600;">
      📋 All Workflows
      <span class="tooltip-container">
        <span class="help-icon" style="font-size: 12px; width: 14px; height: 14px;">?</span>
        <span class="tooltip" style="width: 300px; font-size: 12px;">
          All workflows (running, completed, and available). Each workflow card has its own controls to start, stop, and configure. Workflows are automatically displayed when they start running.
        </span>
      </span>
    </h2>
  </div>
  
  <div class="workflows" id="workflows" style="display: grid; grid-template-columns: repeat(auto-fill, minmax(400px, 1fr)); gap: 20px; padding: 20px; min-height: 200px;">
    <div class="no-workflows">No workflows yet. Create or start a workflow to see it here.</div>
  </div>

  <!-- Logging Panel -->
  <div class="logging-panel" style="margin-top: 30px; padding: 20px; background: linear-gradient(135deg, rgba(26, 31, 58, 0.8) 0%, rgba(15, 20, 34, 0.8) 100%); border-radius: 12px; border: 1px solid rgba(102, 126, 234, 0.3); backdrop-filter: blur(10px); box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);">
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px;">
      <h2 style="margin: 0; color: #667eea; font-size: 18px; font-weight: 600;">
        📋 Process Logs
        <span class="tooltip-container">
          <span class="help-icon" style="font-size: 12px; width: 14px; height: 14px;">?</span>
          <span class="tooltip" style="width: 300px; font-size: 12px;">
            Real-time logs from workflow processes. Shows stdout (standard output) and stderr (error output) from running workflows. Logs are automatically scrolled to show the latest entries.
          </span>
        </span>
      </h2>
      <div style="display: flex; gap: 10px; align-items: center;">
        <select id="log-workflow-filter" style="padding: 6px 10px; background: rgba(15, 20, 34, 0.8); border: 1px solid rgba(102, 126, 234, 0.3); border-radius: 6px; color: #e0e0e0; font-size: 12px; cursor: pointer;">
          <option value="">All Workflows</option>
        </select>
        <select id="log-level-filter" style="padding: 6px 10px; background: rgba(15, 20, 34, 0.8); border: 1px solid rgba(102, 126, 234, 0.3); border-radius: 6px; color: #e0e0e0; font-size: 12px; cursor: pointer;">
          <option value="">All Levels</option>
          <option value="stdout">stdout</option>
          <option value="stderr">stderr</option>
        </select>
        <button id="clear-logs-btn" style="padding: 6px 12px; background: rgba(244, 67, 54, 0.3); border: 1px solid rgba(244, 67, 54, 0.5); border-radius: 6px; color: #ff6b6b; font-size: 12px; cursor: pointer; transition: all 0.2s;" onmouseover="this.style.background='rgba(244, 67, 54, 0.5)'" onmouseout="this.style.background='rgba(244, 67, 54, 0.3)'">
          Clear
        </button>
      </div>
    </div>
    <div id="logs-container" style="background: rgba(0, 0, 0, 0.4); border-radius: 8px; padding: 15px; max-height: 500px; overflow-y: auto; font-family: 'Courier New', monospace; font-size: 12px; line-height: 1.6; color: #e0e0e0;">
      <div style="color: #888; font-style: italic; text-align: center; padding: 20px;">No logs yet. Start a workflow to see process output.</div>
    </div>
  </div>

  <!-- Custom Dialog Modal -->
  <div id="custom-dialog-modal" class="modal" style="display: none; z-index: 10000;">
    <div class="modal-content" style="max-width: 500px; background: linear-gradient(135deg, rgba(15, 20, 34, 0.98) 0%, rgba(26, 31, 58, 0.98) 100%); border: 2px solid rgba(102, 126, 234, 0.4); border-radius: 12px; box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5);">
      <div class="modal-header" style="padding: 20px; border-bottom: 2px solid rgba(102, 126, 234, 0.3);">
        <h2 id="dialog-title" style="margin: 0; color: #667eea; font-size: 20px; font-weight: 600;"></h2>
        <span class="close" onclick="window.closeDialog()" style="font-size: 28px; color: #888; cursor: pointer; transition: color 0.2s;">&times;</span>
      </div>
      <div style="padding: 25px;">
        <div id="dialog-message" style="color: #e0e0e0; font-size: 15px; line-height: 1.6; margin-bottom: 25px;"></div>
        <div style="display: flex; gap: 10px; justify-content: flex-end;">
          <button id="dialog-cancel-btn" class="btn btn-secondary" onclick="window.closeDialog()" style="display: none; padding: 10px 20px; font-size: 14px;">Cancel</button>
          <button id="dialog-ok-btn" class="btn btn-primary" onclick="window.dialogConfirm()" style="padding: 10px 20px; font-size: 14px;">OK</button>
        </div>
      </div>
    </div>
  </div>

  <!-- Create Workflow Modal - Simplified Design -->
  <div id="create-workflow-modal" class="modal" style="display: none;">
    <div class="modal-content" style="max-width: 800px; max-height: 85vh; overflow-y: auto; background: linear-gradient(135deg, rgba(15, 20, 34, 0.98) 0%, rgba(26, 31, 58, 0.98) 100%); border: 2px solid rgba(102, 126, 234, 0.4);">
      <div class="modal-header" style="padding: 20px; border-bottom: 2px solid rgba(102, 126, 234, 0.3);">
        <h2 style="margin: 0; color: #667eea; font-size: 22px;">Create New Workflow</h2>
        <span class="close" onclick="window.closeCreateWorkflowModal()" style="font-size: 28px; color: #888; cursor: pointer; transition: color 0.2s;">&times;</span>
      </div>
      
      <div style="padding: 25px;">
        <!-- Simple Prompt Input -->
        <div style="margin-bottom: 25px;">
          <label style="display: block; margin-bottom: 10px; color: #e0e0e0; font-weight: 600; font-size: 15px;">
            Describe what you want the workflow to do
          </label>
          <textarea 
            id="workflow-prompt-input" 
            placeholder="Example: Create a workflow that reviews code quality, runs automated tests, and generates a compliance report. The workflow should have 3 steps: code analysis, test execution, and report generation."
            style="width: 100%; min-height: 180px; padding: 15px; background: rgba(15, 20, 34, 0.6); border: 2px solid rgba(102, 126, 234, 0.4); border-radius: 8px; color: #e0e0e0; font-size: 14px; line-height: 1.6; font-family: inherit; resize: vertical;"
          ></textarea>
          <div style="margin-top: 8px; font-size: 12px; color: #888;">
            💡 Be specific about steps, agents, and what each step should accomplish
          </div>
        </div>

        <!-- Generate Button -->
        <div style="display: flex; gap: 10px; margin-bottom: 25px;">
          <button type="button" class="btn btn-primary" id="generate-workflow-btn" onclick="window.generateWorkflowFromPrompt()" style="flex: 1; padding: 14px 24px; font-size: 15px; font-weight: 600;">
            ✨ Generate Workflow
          </button>
        </div>

        <!-- Generation Status -->
        <div id="workflow-generation-status" style="display: none; padding: 15px; background: rgba(102, 126, 234, 0.1); border-radius: 8px; text-align: center; color: #667eea; margin-bottom: 20px;">
          <div style="display: inline-block; width: 20px; height: 20px; border: 3px solid #667eea; border-top-color: transparent; border-radius: 50%; animation: spin 1s linear infinite; margin-right: 10px; vertical-align: middle;"></div>
          <span style="font-size: 14px; font-weight: 500;">Generating workflow... This may take a moment</span>
        </div>

        <!-- Generated Workflow Preview -->
        <div id="generated-workflow-preview" style="display: none; margin-top: 25px; padding: 20px; background: rgba(76, 175, 80, 0.1); border: 2px solid rgba(76, 175, 80, 0.4); border-radius: 8px;">
          <div style="display: flex; align-items: center; gap: 10px; margin-bottom: 20px;">
            <div style="width: 8px; height: 8px; background: #4CAF50; border-radius: 50%;"></div>
            <h3 style="margin: 0; color: #4CAF50; font-size: 18px; font-weight: 600;">Generated Workflow</h3>
          </div>
          
          <div id="generated-workflow-content" style="margin-bottom: 20px; padding: 15px; background: rgba(15, 20, 34, 0.6); border-radius: 6px; max-height: 350px; overflow-y: auto; font-size: 13px; line-height: 1.6;"></div>
          
          <div style="background: rgba(102, 126, 234, 0.1); padding: 12px; border-radius: 6px; margin-bottom: 20px; border-left: 4px solid #667eea;">
            <div style="font-size: 12px; color: #ccc; line-height: 1.5;">
              <strong style="color: #667eea;">Review the workflow above.</strong> Make sure all steps and agents are correct before saving.
            </div>
          </div>
          
          <div style="display: flex; gap: 10px; flex-wrap: wrap;">
            <button type="button" class="btn btn-success" onclick="window.saveGeneratedWorkflow()" id="save-generated-workflow-btn" style="flex: 1; min-width: 140px; padding: 12px 20px; font-size: 14px; font-weight: 600;">
              ✅ Save Workflow
            </button>
            <button type="button" class="btn btn-primary" onclick="window.reworkGeneratedWorkflow()" id="rework-workflow-btn" style="flex: 1; min-width: 120px; padding: 12px 20px; font-size: 14px; font-weight: 600;">
              🔄 Regenerate
            </button>
            <button type="button" class="btn btn-secondary" onclick="window.editGeneratedWorkflow()" id="edit-workflow-btn" style="flex: 1; min-width: 120px; padding: 12px 20px; font-size: 14px; font-weight: 600;">
              ✏️ Edit
            </button>
            <button type="button" class="btn btn-danger" onclick="window.closeCreateWorkflowModal()" style="flex: 1; min-width: 100px; padding: 12px 20px; font-size: 14px; font-weight: 600;">
              Cancel
            </button>
          </div>
        </div>

        <!-- Manual Creation (Hidden by default, shown when Edit is clicked) -->
        <div id="workflow-tab-content-manual" style="display: none;">
          <form id="create-workflow-form">
            <div style="margin-bottom: 20px;">
              <label style="display: block; margin-bottom: 8px; color: #e0e0e0; font-weight: 600; font-size: 14px;">
                Workflow Name *
              </label>
              <input type="text" id="workflow-name-input" required placeholder="e.g., code-review-workflow" style="width: 100%; padding: 12px; background: rgba(15, 20, 34, 0.6); border: 2px solid rgba(102, 126, 234, 0.4); border-radius: 6px; color: #e0e0e0; font-size: 14px;">
            </div>
            <div style="margin-bottom: 20px;">
              <label style="display: block; margin-bottom: 8px; color: #e0e0e0; font-weight: 600; font-size: 14px;">
                Description
              </label>
              <textarea id="workflow-description-input" placeholder="Describe what this workflow does..." style="width: 100%; min-height: 80px; padding: 12px; background: rgba(15, 20, 34, 0.6); border: 2px solid rgba(102, 126, 234, 0.4); border-radius: 6px; color: #e0e0e0; font-size: 14px; resize: vertical;"></textarea>
            </div>
            <div style="margin-bottom: 20px;">
              <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px;">
                <label style="color: #e0e0e0; font-weight: 600; font-size: 14px;">Steps</label>
                <button type="button" class="btn btn-primary btn-small" onclick="window.addWorkflowStep()" style="padding: 8px 16px; font-size: 13px;">
                  + Add Step
                </button>
              </div>
              <div id="workflow-steps-container"></div>
            </div>
            <div style="display: flex; gap: 10px; margin-top: 25px;">
              <button type="submit" class="btn btn-success" style="flex: 1; padding: 12px 20px; font-size: 15px; font-weight: 600;">
                Create Workflow
              </button>
              <button type="button" class="btn btn-danger" onclick="window.closeCreateWorkflowModal()" style="flex: 1; padding: 12px 20px; font-size: 15px; font-weight: 600;">
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>

  <script>
    // Custom Dialog System
    var dialogResolve = null;
    window.showDialog = function(title, message, showCancel) {
      var modal = document.getElementById('custom-dialog-modal');
      var titleEl = document.getElementById('dialog-title');
      var messageEl = document.getElementById('dialog-message');
      var cancelBtn = document.getElementById('dialog-cancel-btn');
      var okBtn = document.getElementById('dialog-ok-btn');
      
      if (modal && titleEl && messageEl && cancelBtn && okBtn) {
        titleEl.textContent = title || 'Notification';
        messageEl.textContent = message || '';
        cancelBtn.style.display = showCancel ? 'inline-block' : 'none';
        modal.style.display = 'block';
        
        return new Promise(function(resolve) {
          dialogResolve = resolve;
        });
      }
      return Promise.resolve();
    };
    
    window.closeDialog = function() {
      var modal = document.getElementById('custom-dialog-modal');
      if (modal) {
        modal.style.display = 'none';
      }
      if (dialogResolve) {
        dialogResolve(false);
        dialogResolve = null;
      }
    };
    
    window.dialogConfirm = function() {
      var modal = document.getElementById('custom-dialog-modal');
      if (modal) {
        modal.style.display = 'none';
      }
      if (dialogResolve) {
        dialogResolve(true);
        dialogResolve = null;
      }
    };
    
    // Close dialog on outside click
    window.onclick = function(event) {
      var modal = document.getElementById('custom-dialog-modal');
      if (event.target === modal) {
        window.closeDialog();
      }
    };

    // Load available workflows
    async function loadWorkflows() {
      try {
        const response = await fetch('/api/workflows/available');
        const data = await response.json();
        const select = document.getElementById('workflow-select');
        if (select && data.workflows) {
          select.innerHTML = '<option value="">Select a workflow...</option>';
          data.workflows.forEach(function(w) {
            var option = document.createElement('option');
            option.value = w.name;
            option.textContent = w.name + ' (' + w.steps + ' steps)';
            select.appendChild(option);
          });
        }
      } catch (error) {
        console.error('Failed to load workflows:', error);
      }
    }

    // Load MCP server status and detailed info
    async function loadMcpStatus() {
      try {
        console.log('📊 Dashboard: Loading MCP status...');
        var response = await fetch('/api/mcp/status');
        if (!response.ok) {
          console.error('📊 Dashboard: MCP status API returned error:', response.status, response.statusText);
          return;
        }
        var data = await response.json();
        console.log('📊 Dashboard: MCP status data:', data);
        var statusBadge = document.getElementById('mcp-status');
        var startBtn = document.getElementById('start-mcp-btn');
        var stopBtn = document.getElementById('stop-mcp-btn');
        var portSpan = document.getElementById('mcp-port');
        var detailsPanel = document.getElementById('mcp-details-panel');
        
        if (!statusBadge) {
          console.error('📊 Dashboard: mcp-status element not found');
          return;
        }
        
        if (data.running) {
          console.log('📊 Dashboard: MCP server is running');
          statusBadge.textContent = 'Running';
          statusBadge.className = 'status-badge status-running';
          if (startBtn) {
            startBtn.style.display = 'none';
            startBtn.disabled = true;
          }
          if (stopBtn) {
            stopBtn.style.display = 'inline-block';
            stopBtn.disabled = false;
          }
          if (portSpan && data.port) {
            portSpan.textContent = 'Port: ' + data.port;
            portSpan.style.color = '#667eea';
            portSpan.style.fontWeight = '600';
          }
          
          // Load detailed info if server is running
          if (detailsPanel) {
            detailsPanel.style.display = 'block';
            loadMcpDetails();
          }
        } else {
          console.log('📊 Dashboard: MCP server is stopped');
          statusBadge.textContent = 'Stopped';
          statusBadge.className = 'status-badge status-stopped';
          if (startBtn) {
            startBtn.style.display = 'inline-block';
            startBtn.disabled = false;
          }
          if (stopBtn) {
            stopBtn.style.display = 'none';
            stopBtn.disabled = true;
          }
          if (portSpan) portSpan.textContent = '';
          if (detailsPanel) {
            detailsPanel.style.display = 'none';
            // Clear details
            var healthEl = document.getElementById('mcp-health');
            var resourceCountEl = document.getElementById('mcp-resource-count');
            var toolCountEl = document.getElementById('mcp-tool-count');
            var baseUrlEl = document.getElementById('mcp-base-url');
            if (healthEl) healthEl.textContent = '-';
            if (resourceCountEl) resourceCountEl.textContent = '0';
            if (toolCountEl) toolCountEl.textContent = '0';
            if (baseUrlEl) baseUrlEl.textContent = '-';
          }
        }
      } catch (error) {
        console.error('📊 Dashboard: Failed to load MCP status:', error);
      }
    }

    // Load detailed MCP server information
    async function loadMcpDetails() {
      try {
        var response = await fetch('/api/mcp/info');
        var data = await response.json();
        
        // Update health status
        var healthEl = document.getElementById('mcp-health');
        if (healthEl) {
          if (data.health === 'healthy') {
            healthEl.textContent = '✓ Healthy';
            healthEl.style.color = '#4CAF50';
          } else if (data.health === 'starting') {
            healthEl.textContent = '⏳ Starting...';
            healthEl.style.color = '#ff9800';
          } else if (data.health === 'unhealthy') {
            healthEl.textContent = '✗ Unhealthy';
            healthEl.style.color = '#f44336';
          } else {
            healthEl.textContent = '? Unknown';
            healthEl.style.color = '#888';
          }
        }
        
        // Update resource count
        var resourceCountEl = document.getElementById('mcp-resource-count');
        if (resourceCountEl) {
          resourceCountEl.textContent = data.resourceCount || 0;
        }
        
        // Update tool count
        var toolCountEl = document.getElementById('mcp-tool-count');
        if (toolCountEl) {
          toolCountEl.textContent = data.toolCount || 0;
        }
        
        // Update base URL
        var baseUrlEl = document.getElementById('mcp-base-url');
        if (baseUrlEl && data.baseUrl) {
          baseUrlEl.textContent = data.baseUrl;
          baseUrlEl.style.cursor = 'pointer';
          baseUrlEl.title = 'Click to copy';
          baseUrlEl.onclick = function() {
            navigator.clipboard.writeText(data.baseUrl);
            var original = baseUrlEl.textContent;
            if (original) {
              baseUrlEl.textContent = 'Copied!';
              setTimeout(function() {
                if (baseUrlEl) {
                  baseUrlEl.textContent = original;
                }
              }, 2000);
            }
          };
        }
        
        // Display resources list
        var resourcesSection = document.getElementById('mcp-resources-section');
        var resourcesList = document.getElementById('mcp-resources-list');
        if (resourcesSection && resourcesList && data.resources && data.resources.length > 0) {
          resourcesSection.style.display = 'block';
          resourcesList.innerHTML = data.resources.map(function(resource) {
            return '<div style="padding: 8px; margin-bottom: 6px; background: rgba(102, 126, 234, 0.1); border-left: 3px solid #667eea; border-radius: 4px;">' +
              '<div style="font-weight: 600; color: #667eea; margin-bottom: 3px;">' + (resource.name || 'Unnamed') + '</div>' +
              '<div style="color: #888; font-size: 10px; margin-bottom: 3px;">' + (resource.uri || '') + '</div>' +
              '<div style="color: #e0e0e0; font-size: 10px;">' + (resource.description || '') + '</div>' +
            '</div>';
          }).join('');
        } else if (resourcesSection) {
          resourcesSection.style.display = 'none';
        }
        
        // Display tools list
        var toolsSection = document.getElementById('mcp-tools-section');
        var toolsList = document.getElementById('mcp-tools-list');
        if (toolsSection && toolsList && data.tools && data.tools.length > 0) {
          toolsSection.style.display = 'block';
          toolsList.innerHTML = data.tools.map(function(tool) {
            return '<div style="padding: 8px; margin-bottom: 6px; background: rgba(33, 150, 243, 0.1); border-left: 3px solid #2196F3; border-radius: 4px;">' +
              '<div style="font-weight: 600; color: #2196F3; margin-bottom: 3px;">' + (tool.name || 'Unnamed') + '</div>' +
              '<div style="color: #e0e0e0; font-size: 10px;">' + (tool.description || '') + '</div>' +
            '</div>';
          }).join('');
        } else if (toolsSection) {
          toolsSection.style.display = 'none';
        }
      } catch (error) {
        console.error('Failed to load MCP details:', error);
      }
    }

    // Save workflow state to localStorage
    function saveWorkflowState() {
      try {
        var state = {
          workflows: Array.from(workflows.entries()).map(function(entry) {
            return {
              name: entry[0],
              metrics: entry[1]
            };
          }),
          collapsedWorkflows: Array.from(collapsedWorkflows.entries()),
          timestamp: Date.now()
        };
        localStorage.setItem('maestro-dashboard-state', JSON.stringify(state));
      } catch (error) {
        console.warn('Failed to save workflow state:', error);
      }
    }

    // Load workflow state from localStorage
    function loadWorkflowState() {
      try {
        var savedState = localStorage.getItem('maestro-dashboard-state');
        if (savedState) {
          var state = JSON.parse(savedState);
          
          // Restore collapsed workflows
          if (state.collapsedWorkflows) {
            collapsedWorkflows.clear();
            for (var i = 0; i < state.collapsedWorkflows.length; i++) {
              var entry = state.collapsedWorkflows[i];
              collapsedWorkflows.set(entry[0], entry[1]);
            }
          }
          
          // Restore workflow metrics (but mark as stale if older than 5 minutes)
          var now = Date.now();
          var staleThreshold = 5 * 60 * 1000; // 5 minutes
          if (state.workflows && (now - (state.timestamp || 0)) < staleThreshold) {
            workflows.clear();
            for (var j = 0; j < state.workflows.length; j++) {
              var wf = state.workflows[j];
              // Convert date strings back to Date objects
              if (wf.metrics.startTime) {
                wf.metrics.startTime = new Date(wf.metrics.startTime);
              }
              if (wf.metrics.endTime) {
                wf.metrics.endTime = new Date(wf.metrics.endTime);
              }
              workflows.set(wf.name, wf.metrics);
            }
            console.log('📊 Dashboard: Restored', workflows.size, 'workflow(s) from localStorage');
            // Don't call renderWorkflows here - it will be called after all functions are defined
            return true; // Indicate state was restored
          } else {
            // State is stale, clear it
            localStorage.removeItem('maestro-dashboard-state');
          }
        }
      } catch (error) {
        console.warn('Failed to load workflow state:', error);
        localStorage.removeItem('maestro-dashboard-state');
      }
      return false;
    }

        // Load active executions and workflows
    async function loadExecutions() {
      try {
        console.log('📊 Dashboard: Loading executions and workflows...');
        // First, get active workflows from dashboard (has full metrics)
        var workflowsResponse = await fetch('/api/workflows/active');
        if (!workflowsResponse.ok) {
          console.error('📊 Dashboard: Workflows API returned error:', workflowsResponse.status, workflowsResponse.statusText);
          return;
        }
        var workflowsData = await workflowsResponse.json();
        console.log('📊 Dashboard: Received workflows data:', workflowsData);
        
        // Update execution count
        var executionsResponse = await fetch('/api/workflows/executions');
        var executionsData = await executionsResponse.json();
        var countSpan = document.getElementById('execution-count');
        var stopBtn = document.getElementById('stop-workflow-btn');
        var activeCount = executionsData.executions ? executionsData.executions.length : 0;
        
        if (countSpan) {
          countSpan.textContent = activeCount;
        }
        
        // Enable/disable stop button based on active workflows
        if (stopBtn) {
          stopBtn.disabled = activeCount === 0;
          if (activeCount === 0) {
            stopBtn.style.opacity = '0.5';
            stopBtn.style.cursor = 'not-allowed';
          } else {
            stopBtn.style.opacity = '1';
            stopBtn.style.cursor = 'pointer';
          }
        }
        
        // Clear existing workflows
        workflows.clear();
        
        // Create a map of executionId by workflowName for stop functionality
        var executionIdMap = new Map();
        if (executionsData.executions && executionsData.executions.length > 0) {
          for (var i = 0; i < executionsData.executions.length; i++) {
            var exec = executionsData.executions[i];
            if (exec.workflowName && exec.id) {
              executionIdMap.set(exec.workflowName, exec.id);
              console.log('📊 Dashboard: Mapped workflow', exec.workflowName, 'to executionId', exec.id);
            }
          }
        }
        
        // Add workflows from dashboard (these have full metrics)
        if (workflowsData.workflows && workflowsData.workflows.length > 0) {
          workflowsData.workflows.forEach(function(w) {
            var workflowName = w.workflowName || '';
            var executionId = executionIdMap.get(workflowName) || w.executionId || null;
            
            // Debug: Log if executionId is missing
            if (!executionId) {
              console.log('📊 Dashboard: Warning - No executionId found for workflow', workflowName);
              console.log('   Available executions:', executionsData.executions);
              console.log('   ExecutionIdMap keys:', Array.from(executionIdMap.keys()));
            }
            
            // Ensure all required fields with defaults
            var workflowMetrics = {
              workflowName: workflowName,
              executionId: executionId,
              status: w.status || 'running',
              duration: w.duration || 0,
              totalSteps: w.totalSteps || 0,
              completedSteps: w.completedSteps || 0,
              failedSteps: w.failedSteps || 0,
              successRate: w.successRate || 0,
              currentStep: w.currentStep || 'N/A',
              currentTokenUsage: w.currentTokenUsage || {},
              totalTokens: w.totalTokens || 0,
              contextWindowPercent: w.contextWindowPercent || 0,
              currentModel: w.currentModel || 'auto',
              currentAgent: w.currentAgent || 'N/A',
              totalCost: w.totalCost || 0,
              currentActivity: w.currentActivity || 'Processing...',
              processStatus: w.processStatus || { running: false, cursorAgentRunning: false },
            };
            workflows.set(workflowName, workflowMetrics);
          });
        }
        
        // Also check executions and add any missing workflows
        if (executionsData.executions && executionsData.executions.length > 0) {
          for (var i = 0; i < executionsData.executions.length; i++) {
            var exec = executionsData.executions[i];
            // Only add if not already in workflows Map
            if (!workflows.has(exec.workflowName)) {
              // Add basic workflow info with "spinning up" status
              workflows.set(exec.workflowName, {
                workflowName: exec.workflowName,
                executionId: exec.id,
                status: 'running',
                duration: Date.now() - new Date(exec.startTime).getTime(),
                totalSteps: 0,
                completedSteps: 0,
                failedSteps: 0,
                successRate: 0,
                currentStep: 'Initializing...',
                currentTokenUsage: {},
                totalTokens: 0,
                contextWindowPercent: 0,
                currentModel: 'auto',
                currentAgent: 'N/A',
                totalCost: 0,
                currentActivity: 'Spinning up workflow...',
                processStatus: { running: false, cursorAgentRunning: false },
              });
            }
          }
        }
        
        // Load all available workflows and add any that aren't already displayed
        try {
          console.log('📊 Dashboard: Loading available workflows...');
          var availableResponse = await fetch('/api/workflows/available');
          if (!availableResponse.ok) {
            console.error('📊 Dashboard: Available workflows API returned error:', availableResponse.status, availableResponse.statusText);
          } else {
            var availableData = await availableResponse.json();
            console.log('📊 Dashboard: Available workflows data:', availableData);
            if (availableData.workflows && availableData.workflows.length > 0) {
              console.log('📊 Dashboard: Found', availableData.workflows.length, 'available workflow(s)');
              for (var j = 0; j < availableData.workflows.length; j++) {
                var availableWorkflow = availableData.workflows[j];
                var workflowName = availableWorkflow.name || '';
                console.log('📊 Dashboard: Processing available workflow:', workflowName);
                // Only add if not already in workflows Map
                if (workflowName && !workflows.has(workflowName)) {
                  console.log('📊 Dashboard: Adding workflow to display:', workflowName);
                  // Add as available (not running) workflow
                  workflows.set(workflowName, {
                    workflowName: workflowName,
                    executionId: null,
                    status: 'pending',
                    duration: 0,
                    totalSteps: availableWorkflow.steps || 0,
                    completedSteps: 0,
                    failedSteps: 0,
                    successRate: 0,
                    currentStep: 'Not started',
                    currentTokenUsage: {},
                    totalTokens: 0,
                    contextWindowPercent: 0,
                    currentModel: 'auto',
                    currentAgent: 'N/A',
                    totalCost: 0,
                    currentActivity: 'Ready to start',
                    processStatus: { running: false, cursorAgentRunning: false },
                  });
                } else {
                  console.log('📊 Dashboard: Workflow already exists or name is empty:', workflowName);
                }
              }
            } else {
              console.log('📊 Dashboard: No available workflows found (empty array or null)');
            }
          }
        } catch (error) {
          console.error('📊 Dashboard: Failed to load available workflows:', error);
        }
        
        console.log('📊 Dashboard: Loaded', workflows.size, 'workflow(s)');
        console.log('📊 Dashboard: Workflow names:', Array.from(workflows.keys()));
        
        // Update visual status
        var scriptStatusEl = document.getElementById('script-status');
        if (scriptStatusEl) {
          scriptStatusEl.textContent = '✅ Loaded ' + workflows.size + ' workflow(s)';
          scriptStatusEl.style.background = workflows.size > 0 ? 'rgba(76, 175, 80, 0.9)' : 'rgba(255, 152, 0, 0.9)';
        }
        
        // Save state after loading
        saveWorkflowState();
        
        // Render all workflows (ensure workflowsDiv exists)
        if (workflowsDiv) {
          console.log('📊 Dashboard: Calling renderWorkflows()');
          renderWorkflows();
          if (scriptStatusEl) {
            scriptStatusEl.textContent = '✅ Dashboard Ready (' + workflows.size + ' workflows)';
            scriptStatusEl.style.background = 'rgba(76, 175, 80, 0.9)';
          }
        } else {
          console.error('workflowsDiv not found when trying to render workflows');
          if (scriptStatusEl) {
            scriptStatusEl.textContent = '❌ workflowsDiv missing!';
            scriptStatusEl.style.background = 'rgba(244, 67, 54, 0.9)';
          }
          // Try to find it again
          workflowsDiv = document.getElementById('workflows');
          if (workflowsDiv) {
            console.log('📊 Dashboard: Found workflowsDiv on retry, rendering workflows');
            renderWorkflows();
            if (scriptStatusEl) {
              scriptStatusEl.textContent = '✅ Dashboard Ready (retry)';
              scriptStatusEl.style.background = 'rgba(76, 175, 80, 0.9)';
            }
          }
        }
      } catch (error) {
        console.error('Failed to load executions:', error);
      }
    }

    // Load available roles
    var availableRoles = [];
    async function loadRoles() {
      try {
        var response = await fetch('/api/workflows/roles');
        var data = await response.json();
        availableRoles = data.roles || [];
      } catch (error) {
        console.error('Failed to load roles:', error);
      }
    }

    // Start workflow
    window.startWorkflow = async function() {
      var select = document.getElementById('workflow-select');
      var runnerSelect = document.getElementById('runner-select');
      var modelSelect = document.getElementById('model-select');
      var verboseCheckbox = document.getElementById('verbose-checkbox');
      
      if (!select || select.value === '') {
        await window.showDialog('No Workflow Selected', 'Please select a workflow from the dropdown before starting.');
        return;
      }

      var options = {
        flow: select.value,
        runner: runnerSelect ? runnerSelect.value : 'cursor',
        model: modelSelect && modelSelect.value && modelSelect.value !== '' ? modelSelect.value : 'auto',
        verbose: verboseCheckbox ? verboseCheckbox.checked : false,
      };

      try {
        var fetchOptions = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(options)
        };
        
        // Immediately show workflow as "spinning up"
        var workflowName = select.value;
        workflows.set(workflowName, {
          workflowName: workflowName,
          status: 'running',
          duration: 0,
          totalSteps: 0,
          completedSteps: 0,
          failedSteps: 0,
          successRate: 0,
          currentStep: 'Initializing...',
          currentTokenUsage: {},
          totalTokens: 0,
          contextWindowPercent: 0,
          currentModel: options.model || 'auto',
          currentAgent: 'N/A',
          totalCost: 0,
          currentActivity: 'Spinning up workflow...',
        });
        renderWorkflows();
        
        // Enable stop button immediately
        var stopBtn = document.getElementById('stop-workflow-btn');
        if (stopBtn) {
          stopBtn.disabled = false;
          stopBtn.style.opacity = '1';
          stopBtn.style.cursor = 'pointer';
        }
        
        var response = await fetch('/api/workflows/start', fetchOptions);
        var data = await response.json();
        if (data.success) {
          await window.showDialog('Workflow Started', 'Workflow "' + workflowName + '" is starting up. Execution ID: ' + data.executionId);
          // Refresh after a short delay to get real metrics
          setTimeout(function() {
            loadExecutions();
          }, 500);
          // Continue refreshing periodically
          setTimeout(function() {
            loadExecutions();
          }, 2000);
        } else {
          await window.showDialog('Failed to Start Workflow', 'Error: ' + (data.error || 'Unknown error'));
          // Remove from workflows if failed
          workflows.delete(workflowName);
          renderWorkflows();
        }
      } catch (error) {
        await window.showDialog('Error Starting Workflow', 'Error: ' + error);
        // Remove from workflows if error
        var workflowName = select.value;
        workflows.delete(workflowName);
        renderWorkflows();
      }
    };

    // Stop workflow by execution ID (called from toggle switch or button)
    window.stopWorkflowById = async function(executionId, workflowName) {
      if (!executionId || executionId === 'null' || executionId === '') {
        // No execution ID, just update toggle state
        var toggleId = 'toggle-' + workflowName.replace(/[^a-zA-Z0-9]/g, '-');
        var toggleInput = document.getElementById(toggleId);
        if (toggleInput) {
          toggleInput.checked = false;
        }
        console.warn('Cannot stop workflow without execution ID');
        return;
      }
      
      // Update toggle switch immediately for visual feedback
      var toggleId = 'toggle-' + workflowName.replace(/[^a-zA-Z0-9]/g, '-');
      var toggleInput = document.getElementById(toggleId);
      
      // Show confirmation dialog
      var confirmed = await window.showDialog(
        'Stop Workflow',
        'Are you sure you want to stop workflow "' + workflowName + '"? This action cannot be undone.',
        true
      );
      
      if (!confirmed) {
        // Revert toggle switch if user cancels
        if (toggleInput) {
          toggleInput.checked = true;
        }
        return;
      }

      try {
        var response = await fetch('/api/workflows/' + executionId + '/stop', {
          method: 'POST',
        });
        var data = await response.json();
        if (data.success) {
          // Update workflow status
          var workflow = workflows.get(workflowName);
          if (workflow) {
            workflow.status = 'stopped';
            workflow.processStatus = { running: false, cursorAgentRunning: false };
            workflow.endTime = new Date();
            workflows.set(workflowName, workflow);
          }
          
          // Update toggle switch
          if (toggleInput) {
            toggleInput.checked = false;
            toggleInput.setAttribute('data-execution-id', '');
          }
          
          // Save state after stop
          saveWorkflowState();
          
          renderWorkflows();
          loadExecutions();
        } else {
          // Revert toggle switch on failure
          if (toggleInput) {
            toggleInput.checked = true;
          }
          await window.showDialog('Failed to Stop Workflow', 'Could not stop the workflow. Please try again.');
        }
      } catch (error) {
        // Revert toggle switch on error
        if (toggleInput) {
          toggleInput.checked = true;
        }
        await window.showDialog('Error Stopping Workflow', 'Error: ' + error);
      }
    };

    // Start workflow by name (called from toggle switch or button)
    window.startWorkflowByName = async function(workflowName) {
      if (!workflowName) {
        await window.showDialog('Error', 'Workflow name is required.');
        return;
      }
      
      // Update toggle switch immediately for visual feedback
      var toggleId = 'toggle-' + workflowName.replace(/[^a-zA-Z0-9]/g, '-');
      var toggleInput = document.getElementById(toggleId);
      if (toggleInput) {
        toggleInput.checked = true;
      }
      
      // Get model and runner from the workflow card controls if available
      var modelSelectId = 'model-select-' + workflowName.replace(/[^a-zA-Z0-9]/g, '-');
      var runnerSelectId = 'runner-select-' + workflowName.replace(/[^a-zA-Z0-9]/g, '-');
      var modelSelect = document.getElementById(modelSelectId);
      var runnerSelect = document.getElementById(runnerSelectId);
      
      var options = {
        flow: workflowName,
        runner: runnerSelect && runnerSelect.value ? runnerSelect.value : 'cursor',
        model: modelSelect && modelSelect.value && modelSelect.value !== '' ? modelSelect.value : 'auto',
        verbose: false,
      };
      
      try {
        // Immediately show workflow as "spinning up"
        workflows.set(workflowName, {
          workflowName: workflowName,
          executionId: null,
          status: 'running',
          duration: 0,
          totalSteps: 0,
          completedSteps: 0,
          failedSteps: 0,
          successRate: 0,
          currentStep: 'Initializing...',
          currentTokenUsage: {},
          totalTokens: 0,
          contextWindowPercent: 0,
          currentModel: options.model || 'auto',
          currentAgent: 'N/A',
          totalCost: 0,
          currentActivity: 'Spinning up workflow...',
          processStatus: { running: false, cursorAgentRunning: false },
          startTime: new Date(),
        });
        saveWorkflowState();
        renderWorkflows();
        
        var fetchOptions = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(options)
        };
        
        var response = await fetch('/api/workflows/start', fetchOptions);
        var data = await response.json();
        if (data.success) {
          // Update with execution ID
          var workflow = workflows.get(workflowName);
          if (workflow) {
            workflow.executionId = data.executionId;
            workflows.set(workflowName, workflow);
          }
          
          // Update execution ID in toggle switch
          if (toggleInput) {
            toggleInput.setAttribute('data-execution-id', data.executionId || '');
          }
          
          // Save state after successful start
          saveWorkflowState();
          
          // Refresh after a short delay to get real metrics
          setTimeout(function() {
            loadExecutions();
          }, 500);
          setTimeout(function() {
            loadExecutions();
          }, 2000);
        } else {
          // Revert toggle switch on failure
          if (toggleInput) {
            toggleInput.checked = false;
          }
          await window.showDialog('Failed to Start Workflow', 'Error: ' + (data.error || 'Unknown error'));
          // Update status to failed
          var workflow = workflows.get(workflowName);
          if (workflow) {
            workflow.status = 'failed';
            workflows.set(workflowName, workflow);
            renderWorkflows();
          }
        }
      } catch (error) {
        // Revert toggle switch on error
        if (toggleInput) {
          toggleInput.checked = false;
        }
        await window.showDialog('Error Starting Workflow', 'Error: ' + error);
        // Update status to failed
        var workflow = workflows.get(workflowName);
        if (workflow) {
          workflow.status = 'failed';
          workflows.set(workflowName, workflow);
          renderWorkflows();
        }
      }
    };

    // Stop workflow (legacy function for the main stop button)
    window.stopWorkflow = async function() {
      var executionsResponse = await fetch('/api/workflows/executions');
      var executions = await executionsResponse.json();
      if (!executions.executions || executions.executions.length === 0) {
        await window.showDialog('No Active Workflows', 'There are no active workflows to stop.');
        return;
      }

      // If only one workflow, stop it directly
      if (executions.executions.length === 1) {
        var exec = executions.executions[0];
        await window.stopWorkflowById(exec.id, exec.workflowName);
        return;
      }

      // Multiple workflows - show list (for now, stop first one)
      var confirmed = await window.showDialog(
        'Stop Workflow',
        'Multiple workflows are running. Stopping the first workflow: "' + executions.executions[0].workflowName + '". This action cannot be undone.',
        true
      );
      
      if (!confirmed) {
        return;
      }

      var executionId = executions.executions[0].id;
      var workflowName = executions.executions[0].workflowName;
      try {
        var response = await fetch('/api/workflows/' + executionId + '/stop', {
          method: 'POST',
        });
        var data = await response.json();
        if (data.success) {
          await window.showDialog('Workflow Stopped', 'Workflow "' + workflowName + '" has been stopped.');
          workflows.delete(workflowName);
          renderWorkflows();
          loadExecutions();
        } else {
          await window.showDialog('Failed to Stop Workflow', 'Could not stop the workflow. Please try again.');
        }
      } catch (error) {
        await window.showDialog('Error Stopping Workflow', 'Error: ' + error);
      }
    };

    // Start MCP server
    window.startMcpServer = async function() {
      try {
        var portData = { port: 3001 };
        var fetchOptions = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(portData)
        };
        var response = await fetch('/api/mcp/start', fetchOptions);
        var data = await response.json();
        if (data.success) {
          await window.showDialog('MCP Server Started', 'MCP Server is now running on port ' + data.port);
          setTimeout(function() {
            loadMcpStatus();
            loadMcpDetails();
          }, 1500); // Refresh status and details after a moment
        } else {
          await window.showDialog('Failed to Start MCP Server', 'Error: ' + (data.error || 'Unknown error'));
        }
      } catch (error) {
        await window.showDialog('Error Starting MCP Server', 'Error: ' + error);
      }
    };

    // Stop MCP server
    window.stopMcpServer = async function() {
      try {
        var response = await fetch('/api/mcp/stop', {
          method: 'POST',
        });
        var data = await response.json();
        if (data.success) {
          await window.showDialog('MCP Server Stopped', 'MCP Server has been stopped.');
          loadMcpStatus();
          // Clear details panel
          var detailsPanel = document.getElementById('mcp-details-panel');
          if (detailsPanel) {
            detailsPanel.style.display = 'none';
          }
        } else {
          await window.showDialog('Failed to Stop MCP Server', 'Could not stop the MCP server. Please try again.');
        }
      } catch (error) {
        await window.showDialog('Error Stopping MCP Server', 'Error: ' + error);
      }
    };

    // Workflow creation functions
    var stepCounter = 0;
    window.addWorkflowStep = function() {
      stepCounter++;
      var container = document.getElementById('workflow-steps-container');
      if (!container) return;

      var stepDiv = document.createElement('div');
      stepDiv.className = 'step-item';
      stepDiv.id = 'step-' + stepCounter;
      
      var stepSelect = availableRoles.map(function(r) {
        return '<option value="' + r.name + '">' + r.name + ' - ' + r.description + '</option>';
      }).join('');

      stepDiv.innerHTML = 
        '<div class="step-header">' +
          '<span class="step-number">Step ' + stepCounter + '</span>' +
          '<div class="step-actions">' +
            '<button type="button" class="btn btn-danger btn-small" onclick="removeWorkflowStep(' + stepCounter + ')">Remove</button>' +
          '</div>' +
        '</div>' +
        '<div class="form-group">' +
          '<label>Step Name *</label>' +
          '<input type="text" name="step-name-' + stepCounter + '" required placeholder="e.g., analyze_code">' +
          '<div class="help-text">Use lowercase letters, numbers, and underscores only</div>' +
        '</div>' +
        '<div class="form-group">' +
          '<label>Agent Role *</label>' +
          '<select name="step-agent-' + stepCounter + '" required>' +
            '<option value="">Select an agent...</option>' +
            stepSelect +
          '</select>' +
        '</div>' +
        '<div class="form-group">' +
          '<label>Description *</label>' +
          '<textarea name="step-description-' + stepCounter + '" required placeholder="What does this step do? Be specific about requirements and expected outcomes."></textarea>' +
          '<div class="help-text">This description is sent to the AI agent as the task prompt</div>' +
        '</div>' +
        '<div class="form-group">' +
          '<label>Depends On</label>' +
          '<input type="text" name="step-depends-' + stepCounter + '" placeholder="step1, step2">' +
          '<div class="help-text">Optional: List step names separated by commas</div>' +
        '</div>';
      
      container.appendChild(stepDiv);
    };

    window.removeWorkflowStep = function(stepId) {
      var stepDiv = document.getElementById('step-' + stepId);
      if (stepDiv) {
        stepDiv.remove();
      }
    };

    window.showCreateWorkflowModal = function() {
      var modal = document.getElementById('create-workflow-modal');
      if (modal) {
        modal.style.display = 'block';
        stepCounter = 0;
        var stepsContainer = document.getElementById('workflow-steps-container');
        if (stepsContainer) stepsContainer.innerHTML = '';
        var form = document.getElementById('create-workflow-form');
        if (form) form.reset();
        var promptInput = document.getElementById('workflow-prompt-input');
        if (promptInput) {
          promptInput.value = '';
          promptInput.focus();
        }
        var preview = document.getElementById('generated-workflow-preview');
        if (preview) preview.style.display = 'none';
        var status = document.getElementById('workflow-generation-status');
        if (status) status.style.display = 'none';
        var manualTab = document.getElementById('workflow-tab-content-manual');
        if (manualTab) manualTab.style.display = 'none';
      }
    };

    window.closeCreateWorkflowModal = function() {
      var modal = document.getElementById('create-workflow-modal');
      if (modal) {
        modal.style.display = 'none';
      }
    };

    // Switch between AI and Manual tabs (for manual editing)
    window.switchWorkflowTab = function(tab) {
      var manualContent = document.getElementById('workflow-tab-content-manual');
      if (tab === 'manual' && manualContent) {
        manualContent.style.display = 'block';
        var preview = document.getElementById('generated-workflow-preview');
        if (preview) preview.style.display = 'none';
        var promptInput = document.getElementById('workflow-prompt-input');
        if (promptInput) {
          var promptContainer = promptInput.parentElement;
          if (promptContainer) promptContainer.style.display = 'none';
        }
        var generateBtn = document.getElementById('generate-workflow-btn');
        if (generateBtn) generateBtn.style.display = 'none';
        var stepsContainer = document.getElementById('workflow-steps-container');
        if (stepsContainer && stepsContainer.children.length === 0) {
          window.addWorkflowStep();
        }
      }
    };

    // Generate workflow from prompt
    window.generateWorkflowFromPrompt = async function() {
      var promptInput = document.getElementById('workflow-prompt-input');
      if (!promptInput) {
        await window.showDialog('Error', 'Prompt input not found. Please refresh the page.');
        return;
      }
      var prompt = promptInput.value.trim();

      if (!prompt) {
        await window.showDialog('Prompt Required', 'Please enter a description of the workflow you want to create.');
        promptInput.focus();
        return;
      }

      var generateBtn = document.getElementById('generate-workflow-btn');
      var statusDiv = document.getElementById('workflow-generation-status');
      var previewDiv = document.getElementById('generated-workflow-preview');

      // Show loading state
      if (generateBtn) generateBtn.disabled = true;
      if (statusDiv) statusDiv.style.display = 'flex';
      if (previewDiv) previewDiv.style.display = 'none';

      try {
        var promptData = { prompt: prompt };
        var fetchOptions = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(promptData)
        };
        const response = await fetch('/api/workflows/generate', fetchOptions);

        const data = await response.json();

        if (data.success && data.workflow) {
          // Display generated workflow
          displayGeneratedWorkflow(data.workflow);
          if (previewDiv) {
            previewDiv.style.display = 'block';
            setTimeout(function() {
              previewDiv.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
            }, 100);
          }
          window.generatedWorkflow = data.workflow;
        } else {
          alert('Failed to generate workflow: ' + (data.error || 'Unknown error'));
        }
      } catch (error) {
        alert('Error generating workflow: ' + error);
      } finally {
        if (generateBtn) generateBtn.disabled = false;
        if (statusDiv) statusDiv.style.display = 'none';
      }
    };

    // Display generated workflow
    function displayGeneratedWorkflow(workflow) {
      var contentDiv = document.getElementById('generated-workflow-content');
      if (!contentDiv) return;
      
      var html = '<div style="margin-bottom: 15px;"><strong style="color: #667eea;">Name:</strong> <span style="color: #e0e0e0;">' + (workflow.name || 'Untitled') + '</span></div>';
      html += '<div style="margin-bottom: 15px;"><strong style="color: #667eea;">Description:</strong><div style="color: #e0e0e0; margin-top: 5px;">' + (workflow.description || 'No description') + '</div></div>';
      html += '<div style="margin-bottom: 15px;"><strong style="color: #667eea;">Steps (' + (workflow.steps ? workflow.steps.length : 0) + '):</strong><div style="margin-top: 10px;">';

      if (workflow.steps && workflow.steps.length > 0) {
        workflow.steps.forEach(function(step, index) {
          var dependsHtml = '';
          if (step.dependsOn && step.dependsOn.length > 0) {
            dependsHtml = '<div style="color: #888; font-size: 11px; margin-top: 5px;">Depends on: ' + step.dependsOn.join(', ') + '</div>';
          }
          html += '<div style="padding: 10px; margin-bottom: 10px; background: rgba(102, 126, 234, 0.1); border-left: 3px solid #667eea; border-radius: 4px;">';
          html += '<div style="font-weight: 600; color: #667eea; margin-bottom: 5px;">Step ' + (index + 1) + ': ' + (step.name || 'Unnamed') + '</div>';
          html += '<div style="color: #888; font-size: 12px; margin-bottom: 5px;">Agent: <span style="color: #e0e0e0;">' + (step.agent || 'Not specified') + '</span></div>';
          html += '<div style="color: #e0e0e0; font-size: 13px;">' + (step.description || 'No description') + '</div>';
          html += dependsHtml;
          html += '</div>';
        });
      }

      html += '</div></div>';
      if (workflow.reasoning) {
        html += '<div style="margin-top: 15px; padding: 10px; background: rgba(102, 126, 234, 0.05); border-radius: 4px; font-size: 12px; color: #888;"><strong>AI Reasoning:</strong> ' + workflow.reasoning + '</div>';
      }

      contentDiv.innerHTML = html;
    }

    // Save generated workflow
    window.saveGeneratedWorkflow = async function() {
      if (!window.generatedWorkflow) {
        alert('No workflow to save');
        return;
      }

      try {
        var workflowData = {
          name: window.generatedWorkflow.name,
          description: window.generatedWorkflow.description,
          steps: window.generatedWorkflow.steps
        };
        var fetchOptions = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(workflowData)
        };
        const response = await fetch('/api/workflows/create', fetchOptions);

        const data = await response.json();

        if (data.success) {
          // Show success message with better UX
          const successMsg = document.createElement('div');
          successMsg.style.cssText = 'position: fixed; top: 20px; right: 20px; background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%); color: white; padding: 15px 25px; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.3); z-index: 10000; font-weight: 600; animation: slideIn 0.3s ease-out;';
          successMsg.textContent = '✅ Workflow created successfully!';
          document.body.appendChild(successMsg);
          setTimeout(() => {
            successMsg.style.animation = 'slideOut 0.3s ease-out';
            setTimeout(() => successMsg.remove(), 300);
          }, 3000);
          
          window.closeCreateWorkflowModal();
          loadWorkflows(); // Refresh workflow list
        } else {
          alert('Failed to save workflow: ' + (data.error || 'Unknown error'));
        }
      } catch (error) {
        alert('Error saving workflow: ' + error);
      }
    };

    // Rework generated workflow
    window.reworkGeneratedWorkflow = function() {
      var previewDiv = document.getElementById('generated-workflow-preview');
      var statusDiv = document.getElementById('workflow-generation-status');
      if (previewDiv) previewDiv.style.display = 'none';
      if (statusDiv) statusDiv.style.display = 'block';
      window.generateWorkflowFromPrompt();
    };

    // Edit generated workflow manually
    window.editGeneratedWorkflow = function() {
      if (!window.generatedWorkflow) {
        alert('No workflow to edit');
        return;
      }

      // Switch to manual tab
      window.switchWorkflowTab('manual');

      // Populate form with generated workflow
      var nameInput = document.getElementById('workflow-name-input');
      var descInput = document.getElementById('workflow-description-input');
      if (nameInput) nameInput.value = window.generatedWorkflow.name || '';
      if (descInput) descInput.value = window.generatedWorkflow.description || '';

      // Clear existing steps and add generated ones
      var container = document.getElementById('workflow-steps-container');
      if (container) {
        container.innerHTML = '';
        stepCounter = 0;
      }

      window.generatedWorkflow.steps.forEach(function(step) {
        stepCounter++;
        var stepDiv = document.createElement('div');
        stepDiv.className = 'step-item';
        stepDiv.id = 'step-' + stepCounter;

        var stepSelect = availableRoles.map(function(r) {
          var selected = r.name === step.agent ? 'selected' : '';
          return '<option value="' + r.name + '" ' + selected + '>' + r.name + ' - ' + r.description + '</option>';
        }).join('');

        var dependsValue = '';
        if (step.dependsOn && step.dependsOn.length > 0) {
          dependsValue = step.dependsOn.join(', ');
        }
        stepDiv.innerHTML = 
          '<div class="step-header">' +
            '<span class="step-number">Step ' + stepCounter + '</span>' +
            '<div class="step-actions">' +
              '<button type="button" class="btn btn-danger btn-small" onclick="removeWorkflowStep(' + stepCounter + ')">Remove</button>' +
            '</div>' +
          '</div>' +
          '<div class="form-group">' +
            '<label>Step Name *</label>' +
            '<input type="text" name="step-name-' + stepCounter + '" required value="' + (step.name || '') + '">' +
          '</div>' +
          '<div class="form-group">' +
            '<label>Agent Role *</label>' +
            '<select name="step-agent-' + stepCounter + '" required>' +
              '<option value="">Select an agent...</option>' +
              stepSelect +
            '</select>' +
          '</div>' +
          '<div class="form-group">' +
            '<label>Description *</label>' +
            '<textarea name="step-description-' + stepCounter + '" required>' + (step.description || '') + '</textarea>' +
          '</div>' +
          '<div class="form-group">' +
            '<label>Depends On</label>' +
            '<input type="text" name="step-depends-' + stepCounter + '" value="' + dependsValue + '">' +
          '</div>';

        container.appendChild(stepDiv);
      });
    };

    window.closeCreateWorkflowModal = function() {
      var modal = document.getElementById('create-workflow-modal');
      if (modal) {
        modal.style.display = 'none';
      }
    };

    // Handle workflow creation form submission
    var createWorkflowForm = document.getElementById('create-workflow-form');
    if (createWorkflowForm) {
      createWorkflowForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        var name = document.getElementById('workflow-name-input').value.trim();
        var description = document.getElementById('workflow-description-input').value.trim();
        
        // Collect steps
        var steps = [];
        var stepItems = document.querySelectorAll('.step-item');
          stepItems.forEach(function(item) {
            // Extract step ID from the item's ID (e.g., "step-1" -> 1)
            var stepIdMatch = item.id.match(/step-(\\d+)/);
            if (!stepIdMatch) return;
            var stepId = stepIdMatch[1];
            
            var nameInput = item.querySelector('[name="step-name-' + stepId + '"]');
            var stepName = nameInput && nameInput.value ? nameInput.value.trim() : '';
            var agentInput = item.querySelector('[name="step-agent-' + stepId + '"]');
            var stepAgent = agentInput ? agentInput.value : '';
            var descInput = item.querySelector('[name="step-description-' + stepId + '"]');
            var stepDescription = descInput && descInput.value ? descInput.value.trim() : '';
            var dependsInput = item.querySelector('[name="step-depends-' + stepId + '"]');
            var stepDepends = dependsInput && dependsInput.value ? dependsInput.value.trim() : '';
            
            if (stepName && stepAgent && stepDescription) {
              var dependsArray = [];
              if (stepDepends) {
                dependsArray = stepDepends.split(',').map(function(d) {
                  return d.trim();
                }).filter(function(d) {
                  return d;
                });
              }
              steps.push({
                name: stepName,
                agent: stepAgent,
                description: stepDescription,
                dependsOn: dependsArray
              });
            }
          });

          if (steps.length === 0) {
            alert('Please add at least one step');
            return;
          }

          try {
            var workflowPayload = {
              name: name,
              description: description || undefined,
              steps: steps
            };
            var fetchOptions = {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify(workflowPayload)
            };
            const response = await fetch('/api/workflows/create', fetchOptions);
            
            const data = await response.json();
            if (data.success) {
              alert('Workflow "' + name + '" created successfully!');
              closeCreateWorkflowModal();
              loadWorkflows(); // Refresh workflow list
            } else {
              alert('Failed to create workflow: ' + (data.error || 'Unknown error'));
            }
          } catch (error) {
            alert('Error creating workflow: ' + error);
          }
        });
      }

    // Close modal when clicking outside
    window.onclick = function(event) {
      var modal = document.getElementById('create-workflow-modal');
      if (event.target === modal) {
        window.closeCreateWorkflowModal();
      }
    };

    // Load system information
    async function loadSystemInfo() {
      try {
        var response = await fetch('/api/system/info');
        var data = await response.json();
        
        // Update Process ID
        var pidEl = document.getElementById('system-pid');
        if (pidEl) {
          pidEl.textContent = 'PID: ' + data.processId;
        }
        
        // Update Uptime
        var uptimeEl = document.getElementById('system-uptime');
        if (uptimeEl) {
          var days = Math.floor(data.uptime / 86400);
          var hours = Math.floor((data.uptime % 86400) / 3600);
          var minutes = Math.floor((data.uptime % 3600) / 60);
          var seconds = data.uptime % 60;
          
          if (days > 0) {
            uptimeEl.textContent = days + 'd ' + hours + 'h ' + minutes + 'm';
          } else if (hours > 0) {
            uptimeEl.textContent = hours + 'h ' + minutes + 'm ' + seconds + 's';
          } else if (minutes > 0) {
            uptimeEl.textContent = minutes + 'm ' + seconds + 's';
          } else {
            uptimeEl.textContent = seconds + 's';
          }
        }
        
        // Update Memory Usage
        var memoryEl = document.getElementById('system-memory');
        var memoryPercentEl = document.getElementById('system-memory-percent');
        var memoryBarEl = document.getElementById('system-memory-bar');
        if (memoryEl && memoryPercentEl && memoryBarEl) {
          var usedGB = (data.memory.used / 1024 / 1024 / 1024).toFixed(2);
          var totalGB = (data.memory.total / 1024 / 1024 / 1024).toFixed(2);
          
          memoryEl.textContent = usedGB + ' GB / ' + totalGB + ' GB';
          memoryPercentEl.textContent = data.memory.percentage.toFixed(1) + '% used';
          
          var percent = Math.min(100, Math.max(0, data.memory.percentage));
          memoryBarEl.style.width = percent + '%';
          
          // Color based on usage
          if (percent > 80) {
            memoryBarEl.style.background = 'linear-gradient(90deg, #f44336 0%, #d32f2f 100%)';
          } else if (percent > 60) {
            memoryBarEl.style.background = 'linear-gradient(90deg, #ff9800 0%, #f57c00 100%)';
          } else {
            memoryBarEl.style.background = 'linear-gradient(90deg, #2196F3 0%, #21CBF3 100%)';
          }
        }
        
        // Update Heap Memory
        var heapEl = document.getElementById('system-heap');
        var heapLimitEl = document.getElementById('system-heap-limit');
        var heapBarEl = document.getElementById('system-heap-bar');
        if (heapEl && heapBarEl && heapLimitEl) {
          var heapUsedMB = (data.memory.heapUsed / 1024 / 1024).toFixed(1);
          var heapLimitMB = (data.memory.heapLimit / 1024 / 1024).toFixed(0);
          var heapPercent = (data.memory.heapUsed / data.memory.heapLimit) * 100;
          
          // Format based on size
          var heapUsedDisplay = heapUsedMB;
          var heapLimitDisplay = heapLimitMB;
          if (parseFloat(heapLimitMB) >= 1024) {
            // Show in GB if >= 1GB
            heapUsedDisplay = (data.memory.heapUsed / 1024 / 1024 / 1024).toFixed(2);
            heapLimitDisplay = (data.memory.heapLimit / 1024 / 1024 / 1024).toFixed(0);
            heapEl.textContent = heapUsedDisplay + ' GB / ' + heapLimitDisplay + ' GB';
            heapLimitEl.textContent = 'Limit: ' + heapLimitDisplay + ' GB (configured)';
          } else {
            heapEl.textContent = heapUsedDisplay + ' MB / ' + heapLimitDisplay + ' MB';
            heapLimitEl.textContent = 'Limit: ' + heapLimitDisplay + ' MB (configured)';
          }
          
          var heapPercentValue = Math.min(100, Math.max(0, heapPercent));
          heapBarEl.style.width = heapPercentValue + '%';
          
          // Color based on usage
          if (heapPercentValue > 80) {
            heapBarEl.style.background = 'linear-gradient(90deg, #f44336 0%, #d32f2f 100%)';
          } else if (heapPercentValue > 60) {
            heapBarEl.style.background = 'linear-gradient(90deg, #ff9800 0%, #f57c00 100%)';
          } else {
            heapBarEl.style.background = 'linear-gradient(90deg, #9C27B0 0%, #E91E63 100%)';
          }
        }
        
        // Update CPU Usage
        var cpuEl = document.getElementById('system-cpu');
        var cpuBarEl = document.getElementById('system-cpu-bar');
        if (cpuEl && cpuBarEl) {
          cpuEl.textContent = data.cpu.usage.toFixed(1) + '%';
          
          var cpuPercent = Math.min(100, Math.max(0, data.cpu.usage));
          cpuBarEl.style.width = cpuPercent + '%';
          
          // Color based on usage
          if (cpuPercent > 80) {
            cpuBarEl.style.background = 'linear-gradient(90deg, #f44336 0%, #d32f2f 100%)';
          } else if (cpuPercent > 60) {
            cpuBarEl.style.background = 'linear-gradient(90deg, #ff9800 0%, #f57c00 100%)';
          } else {
            cpuBarEl.style.background = 'linear-gradient(90deg, #FF9800 0%, #FF5722 100%)';
          }
        }
        
        // Update Platform Info
        var platformEl = document.getElementById('system-platform');
        var hostnameEl = document.getElementById('system-hostname');
        if (platformEl && hostnameEl) {
          var platformName = data.platform.type === 'Darwin' ? 'macOS' : 
                               data.platform.type === 'Linux' ? 'Linux' :
                               data.platform.type === 'Windows_NT' ? 'Windows' : data.platform.type;
          platformEl.textContent = platformName + ' (' + data.platform.arch + ')';
          hostnameEl.textContent = data.platform.hostname + ' • Node ' + data.platform.nodeVersion;
        }
      } catch (error) {
        console.error('Failed to load system info:', error);
      }
    }

    // CRITICAL: Verify script is executing
    console.log('🚀 Maestro Dashboard Script Loaded');
    console.log('🚀 Document ready state:', document.readyState);
    console.log('🚀 Window location:', window.location.href);
    
    // Update visual status indicator
    var scriptStatusEl = document.getElementById('script-status');
    if (scriptStatusEl) {
      scriptStatusEl.textContent = '✅ Script Executing';
      scriptStatusEl.style.background = 'rgba(76, 175, 80, 0.9)';
    }
    
    var ws = null;
    var workflowsDiv = document.getElementById('workflows');
    var statusDiv = document.getElementById('connectionStatus');
    var workflows = new Map();
    var collapsedWorkflows = new Map(); // Track which workflows are collapsed
    
    // CRITICAL: Verify DOM elements exist
    console.log('🚀 workflowsDiv found:', !!workflowsDiv);
    console.log('🚀 statusDiv found:', !!statusDiv);
    if (!workflowsDiv) {
      console.error('❌ CRITICAL: workflows div not found!');
      console.error('❌ Available elements with id:', Array.from(document.querySelectorAll('[id]')).map(function(el) { return el.id; }));
      if (scriptStatusEl) {
        scriptStatusEl.textContent = '❌ workflows div missing!';
        scriptStatusEl.style.background = 'rgba(244, 67, 54, 0.9)';
      }
    } else {
      console.log('✅ workflowsDiv found successfully');
    }
    if (!statusDiv) {
      console.error('❌ CRITICAL: status div not found!');
    } else {
      console.log('✅ statusDiv found successfully');
    }

    // Initialize WebSocket connection
    function initWebSocket() {
      try {
        var wsUrl = 'ws://' + window.location.host;
        console.log('📊 Dashboard: Connecting to WebSocket at', wsUrl);
        ws = new WebSocket(wsUrl);

        ws.onopen = function() {
          console.log('📊 Dashboard: WebSocket connected');
          if (statusDiv) {
            statusDiv.textContent = 'Connected';
            statusDiv.style.background = 'rgba(76, 175, 80, 0.3)';
          }
        };

        ws.onclose = function(event) {
          console.log('📊 Dashboard: WebSocket closed', event.code, event.reason);
          if (statusDiv) {
            statusDiv.textContent = 'Disconnected';
            statusDiv.style.background = 'rgba(244, 67, 54, 0.3)';
          }
          // Attempt to reconnect after 3 seconds
          setTimeout(function() {
            console.log('📊 Dashboard: Attempting to reconnect...');
            initWebSocket();
          }, 3000);
        };

        ws.onerror = function(error) {
          console.error('📊 Dashboard: WebSocket error', error);
          if (statusDiv) {
            statusDiv.textContent = 'Connection Error';
            statusDiv.style.background = 'rgba(244, 67, 54, 0.3)';
          }
        };

        ws.onmessage = function(event) {
          try {
            var message = JSON.parse(event.data);
            console.log('📊 Dashboard: Received message', message.type);
            handleMessage(message);
            
            // Handle log messages separately (they don't need workflow data structure)
            if (message.type === 'log') {
              var logData = message.data;
              if (typeof addLog === 'function') {
                addLog(logData.workflow, logData.level, logData.message, logData.timestamp);
                if (typeof updateLogWorkflowFilter === 'function') {
                  updateLogWorkflowFilter();
                }
              }
            }
          } catch (error) {
            console.error('📊 Dashboard: Error parsing message', error);
          }
        };
      } catch (error) {
        console.error('📊 Dashboard: Failed to create WebSocket', error);
        if (statusDiv) {
          statusDiv.textContent = 'Connection Failed';
          statusDiv.style.background = 'rgba(244, 67, 54, 0.3)';
        }
      }
    }

    // Log handling (must be defined before WebSocket handlers)
    var logs = [];
    var maxLogs = 1000; // Maximum number of log entries to keep
    var selectedWorkflow = '';
    var selectedLevel = '';

    function addLog(workflow, level, message, timestamp) {
      var logEntry = {
        workflow: workflow,
        level: level,
        message: message,
        timestamp: timestamp || new Date().toISOString()
      };
      logs.push(logEntry);
      
      // Limit log entries
      if (logs.length > maxLogs) {
        logs.shift(); // Remove oldest
      }
      
      // Update display
      renderLogs();
    }

    function renderLogs() {
      var container = document.getElementById('logs-container');
      if (!container) return;
      
      // Filter logs
      var filtered = logs.filter(function(log) {
        if (selectedWorkflow && log.workflow !== selectedWorkflow) return false;
        if (selectedLevel && log.level !== selectedLevel) return false;
        return true;
      });
      
      if (filtered.length === 0) {
        container.innerHTML = '<div style="color: #888; font-style: italic; text-align: center; padding: 20px;">No logs to display' + (selectedWorkflow || selectedLevel ? ' (filtered)' : '') + '.</div>';
        return;
      }
      
      var html = '';
      for (var i = 0; i < filtered.length; i++) {
        var log = filtered[i];
        var time = new Date(log.timestamp);
        var timeStr = time.toLocaleTimeString();
        var levelColor = log.level === 'stderr' ? '#ff6b6b' : '#4CAF50';
        var levelBg = log.level === 'stderr' ? 'rgba(244, 67, 54, 0.2)' : 'rgba(76, 175, 80, 0.2)';
        
        html += '<div style="margin-bottom: 8px; padding: 8px; background: rgba(255, 255, 255, 0.02); border-radius: 4px; border-left: 3px solid ' + levelColor + ';">';
        html += '<div style="display: flex; gap: 10px; margin-bottom: 4px; font-size: 11px;">';
        html += '<span style="color: #888;">[' + timeStr + ']</span>';
        html += '<span style="background: ' + levelBg + '; color: ' + levelColor + '; padding: 2px 6px; border-radius: 3px; font-weight: 600; font-size: 10px;">' + log.level.toUpperCase() + '</span>';
        html += '<span style="color: #667eea; font-weight: 600;">' + log.workflow + '</span>';
        html += '</div>';
        html += '<div style="color: #e0e0e0; word-break: break-word;">' + escapeHtml(log.message) + '</div>';
        html += '</div>';
      }
      
      container.innerHTML = html;
      
      // Auto-scroll to bottom
      container.scrollTop = container.scrollHeight;
    }

    function escapeHtml(text) {
      var div = document.createElement('div');
      div.textContent = text;
      return div.innerHTML;
    }

    function updateLogFilters() {
      var workflowFilter = document.getElementById('log-workflow-filter');
      var levelFilter = document.getElementById('log-level-filter');
      
      if (workflowFilter) {
        selectedWorkflow = workflowFilter.value;
      }
      if (levelFilter) {
        selectedLevel = levelFilter.value;
      }
      
      renderLogs();
    }

    function updateLogWorkflowFilter() {
      var filter = document.getElementById('log-workflow-filter');
      if (!filter) return;
      
      var currentValue = filter.value;
      filter.innerHTML = '<option value="">All Workflows</option>';
      
      var workflows = Array.from(new Set(logs.map(function(log) { return log.workflow; })));
      for (var i = 0; i < workflows.length; i++) {
        var option = document.createElement('option');
        option.value = workflows[i];
        option.textContent = workflows[i];
        filter.appendChild(option);
      }
      
      // Restore selection
      filter.value = currentValue;
    }

    // Set up filter event listeners
    var logWorkflowFilter = document.getElementById('log-workflow-filter');
    var logLevelFilter = document.getElementById('log-level-filter');
    var clearLogsBtn = document.getElementById('clear-logs-btn');
    
    if (logWorkflowFilter) {
      logWorkflowFilter.addEventListener('change', function() {
        updateLogFilters();
      });
    }
    
    if (logLevelFilter) {
      logLevelFilter.addEventListener('change', function() {
        updateLogFilters();
      });
    }
    
    if (clearLogsBtn) {
      clearLogsBtn.addEventListener('click', function() {
        logs = [];
        renderLogs();
        updateLogWorkflowFilter();
      });
    }

    // Start WebSocket connection
    initWebSocket();

    function handleMessage(message) {
      console.log('📊 Dashboard: Handling message', message.type, message.data);
      if (message.type === 'metrics' || message.type === 'workflow-start' || message.type === 'step-update' || message.type === 'workflow-end' || message.type === 'activity-update' || message.type === 'token-update' || message.type === 'model-change' || message.type === 'agent-switch' || message.type === 'progress-update' || message.type === 'log') {
        var data = message.data;
        if (data.workflow) {
          // For step-update and metrics, use the metrics from the message
          var metrics = (message.type === 'step-update' || message.type === 'metrics') ? data.metrics : (data.metrics || data);
          
          // Handle activity updates
          if (message.type === 'activity-update') {
            var workflow = workflows.get(data.workflow);
            if (workflow) {
              workflow.currentActivity = data.activity || 'Processing...';
              updateWorkflow(data.workflow, workflow);
              // Update activity text element in real-time
              var activityEl = document.getElementById('activity-text-' + data.workflow);
              if (activityEl) {
                activityEl.textContent = workflow.currentActivity;
                // Add visual feedback
                activityEl.style.transition = 'all 0.3s ease';
                activityEl.style.color = '#4CAF50';
                setTimeout(function() {
                  if (activityEl) {
                    activityEl.style.color = '#e0e0e0';
                  }
                }, 500);
              }
              // Refresh activity history when activity updates
              updateActivityHistory(data.workflow);
            }
          } else if (message.type === 'token-update') {
            var workflow = workflows.get(data.workflow);
            if (workflow) {
              workflow.currentTokenUsage = data.usage;
              workflow.totalTokens = data.usage.totalTokens;
              workflow.contextWindowPercent = data.usage.contextWindowPercent;
              updateWorkflow(data.workflow, workflow);
            }
          } else if (message.type === 'model-change') {
            var workflow = workflows.get(data.workflow);
            if (workflow) {
              workflow.currentModel = data.model;
              updateWorkflow(data.workflow, workflow);
            }
          } else if (message.type === 'agent-switch') {
            var workflow = workflows.get(data.workflow);
            if (workflow) {
              workflow.currentAgent = data.agent;
              updateWorkflow(data.workflow, workflow);
            }
          } else if (message.type === 'progress-update') {
            var workflow = workflows.get(data.workflow);
            if (workflow) {
              if (!workflow.taskProgress) workflow.taskProgress = new Map();
              workflow.taskProgress.set(data.step, data.progress);
              workflow.latestAnalysis = data.analysis;
              updateWorkflow(data.workflow, workflow);
            }
          } else {
            console.log('📊 Dashboard: Updating workflow', data.workflow, 'with metrics', metrics);
            updateWorkflow(data.workflow, metrics);
            // Refresh activity history on step updates
            if (message.type === 'step-update') {
              updateActivityHistory(data.workflow);
            }
          }
        } else if (data.workflows) {
          data.workflows.forEach(function(w) {
            updateWorkflow(w.workflowName, w);
          });
        }
      }
    }

    function updateWorkflow(name, metrics) {
      console.log('📊 Dashboard: updateWorkflow called', name, 'duration:', metrics.duration, 'successRate:', metrics.successRate, 'currentStep:', metrics.currentStep);
      
      // Merge with existing workflow data to preserve all fields
      var existing = workflows.get(name);
      if (existing) {
        // Merge metrics, preserving existing values if new ones are missing
        var merged = {
          workflowName: metrics.workflowName || existing.workflowName || name,
          executionId: metrics.executionId !== undefined ? metrics.executionId : existing.executionId,
          startTime: metrics.startTime || existing.startTime || new Date(),
          endTime: metrics.endTime !== undefined ? metrics.endTime : existing.endTime,
          duration: metrics.duration !== undefined ? metrics.duration : existing.duration,
          totalSteps: metrics.totalSteps !== undefined ? metrics.totalSteps : existing.totalSteps,
          completedSteps: metrics.completedSteps !== undefined ? metrics.completedSteps : existing.completedSteps,
          failedSteps: metrics.failedSteps !== undefined ? metrics.failedSteps : existing.failedSteps,
          successRate: metrics.successRate !== undefined ? metrics.successRate : existing.successRate,
          currentStep: metrics.currentStep || existing.currentStep || 'N/A',
          status: metrics.status || existing.status || 'running',
          cacheHits: metrics.cacheHits !== undefined ? metrics.cacheHits : (existing.cacheHits || 0),
          cacheMisses: metrics.cacheMisses !== undefined ? metrics.cacheMisses : (existing.cacheMisses || 0),
          retryAttempts: metrics.retryAttempts !== undefined ? metrics.retryAttempts : (existing.retryAttempts || 0),
          currentTokenUsage: metrics.currentTokenUsage || existing.currentTokenUsage || {},
          totalTokens: metrics.totalTokens !== undefined ? metrics.totalTokens : (existing.totalTokens || 0),
          contextWindowPercent: metrics.contextWindowPercent !== undefined ? metrics.contextWindowPercent : (existing.contextWindowPercent || 0),
          totalCost: metrics.totalCost !== undefined ? metrics.totalCost : (existing.totalCost || 0),
          currentModel: metrics.currentModel || existing.currentModel || 'auto',
          currentAgent: metrics.currentAgent || existing.currentAgent || 'N/A',
          currentActivity: metrics.currentActivity || existing.currentActivity || 'Processing...',
          processStatus: metrics.processStatus || existing.processStatus || { running: false, cursorAgentRunning: false },
        };
        workflows.set(name, merged);
      } else {
        workflows.set(name, metrics);
      }
      
      // Save state after update
      saveWorkflowState();
      
      // Update activity text element if it exists (for real-time updates without full re-render)
      var activityEl = document.getElementById('activity-text-' + name);
      if (activityEl) {
        var finalMetrics = workflows.get(name);
        if (finalMetrics && finalMetrics.currentActivity) {
          activityEl.textContent = finalMetrics.currentActivity;
          // Add visual feedback
          activityEl.style.transition = 'all 0.3s ease';
          activityEl.style.color = '#4CAF50';
          setTimeout(function() {
            if (activityEl) {
              activityEl.style.color = '#e0e0e0';
            }
          }, 500);
        }
      }
      
      // Re-render workflows to show updated metrics
      renderWorkflows();
      
      // Update activity history
      updateActivityHistory(name);
    }

    async function updateActivityHistory(workflowName) {
      try {
        var response = await fetch('/api/workflows/' + encodeURIComponent(workflowName) + '/activity');
        var data = await response.json();
        var historyDiv = document.getElementById('activity-history-' + workflowName);
        if (historyDiv && data.recent) {
          var activities = data.recent.slice(-10).reverse(); // Show last 10, most recent first
          if (activities.length === 0) {
            historyDiv.innerHTML = '<div style="padding: 2px 0; color: #999;">No activity yet</div>';
          } else {
            historyDiv.innerHTML = activities.map(function(a) {
              var timeStr = new Date(a.timestamp).toLocaleTimeString();
              var details = a.details || '';
              var detailsShort = details.length > 100 ? details.substring(0, 100) + '...' : details;
              return '<div style="padding: 2px 0; border-left: 2px solid #667eea; padding-left: 8px; margin-bottom: 4px;">' +
                '<span style="color: #888; font-size: 9px;">' + timeStr + '</span>' +
                '<div style="margin-top: 2px;">' + detailsShort + '</div>' +
              '</div>';
            }).join('');
          }
        }
      } catch (error) {
        console.error('Failed to fetch activity history:', error);
      }
    }

    function renderWorkflows() {
      console.log('📊 Dashboard: renderWorkflows() called, workflows.size =', workflows.size);
      
      if (!workflowsDiv) {
        console.error('workflowsDiv not found in renderWorkflows');
        workflowsDiv = document.getElementById('workflows');
        if (!workflowsDiv) {
          console.error('Still cannot find workflowsDiv');
          return;
        }
      }
      
      if (workflows.size === 0) {
        console.log('📊 Dashboard: No workflows to render, showing empty state');
        workflowsDiv.innerHTML = '<div class="no-workflows">No workflows yet. Create or start a workflow to see it here.</div>';
        return;
      }

      console.log('📊 Dashboard: Rendering', workflows.size, 'workflow(s)');
      workflowsDiv.innerHTML = '';
      workflows.forEach(function(metrics, name) {
        console.log('📊 Dashboard: Rendering workflow:', name);
        var card = document.createElement('div');
        card.className = 'workflow-card';
        
        // Initialize html variable for this workflow card
        var html = '';
        
        var progress = metrics.totalSteps > 0 
          ? ((metrics.completedSteps + metrics.failedSteps) / metrics.totalSteps * 100).toFixed(1)
          : 0;

        // Token usage display
        var tokenUsage = metrics.currentTokenUsage || {};
        var totalTokens = metrics.totalTokens || 0;
        var contextPercent = metrics.contextWindowPercent || 0;
        var contextColor = contextPercent > 80 ? '#f44336' : contextPercent > 60 ? '#ff9800' : '#4CAF50';
        
        // Model display
        var currentModel = metrics.currentModel || 'Not set';
        var modelInfo = metrics.modelHistory && metrics.modelHistory.length > 0 
          ? metrics.modelHistory[metrics.modelHistory.length - 1].model 
          : currentModel;
        
        // Agent display
        var currentAgent = metrics.currentAgent || 'Not set';
        
        // Task progress
        var taskProgress = metrics.taskProgress || new Map();
        var progressItems = [];
        if (taskProgress && typeof taskProgress.forEach === 'function') {
          taskProgress.forEach(function(p, step) {
            progressItems.push({
              step: step,
              percent: p.completionPercent || 0,
              status: p.status || 'pending',
            });
          });
        }
        
        // Analysis display
        var analysis = metrics.latestAnalysis;
        var hasIssues = analysis && analysis.issues && analysis.issues.length > 0;
        var needsReAlignment = analysis && analysis.needsReAlignment;

        // Build HTML using string concatenation
        // Ensure html is initialized (should already be done above, but double-check)
        if (typeof html === 'undefined') {
          html = '';
        }
        
        var statusUpper = (metrics.status || 'running').toUpperCase();
        var currentStep = metrics.currentStep || 'N/A';
        var successRate = (metrics.successRate || 0).toFixed(1);
        var stepsCompleted = (metrics.completedSteps || 0) + (metrics.failedSteps || 0);
        var totalSteps = metrics.totalSteps || 0;
        var duration = formatDuration(metrics.duration || 0);
        
        // Process status
        var processStatus = metrics.processStatus || { running: false, pid: undefined, exitCode: null, cursorAgentRunning: false };
        var processRunning = processStatus.running === true;
        var cursorAgentRunning = processStatus.cursorAgentRunning === true;
        var processStatusText = processRunning ? 'Running' : (processStatus.exitCode !== null ? 'Stopped' : 'Unknown');
        var processStatusColor = processRunning ? '#4CAF50' : '#f44336';
        var cursorAgentStatusText = cursorAgentRunning ? 'Active' : 'Inactive';
        var cursorAgentStatusColor = cursorAgentRunning ? '#4CAF50' : '#ff9800';
        
        // Cost information
        var totalCost = metrics.totalCost || 0;
        var costDisplay = totalCost > 0 ? '$' + totalCost.toFixed(4) : 'N/A';
        
        // Token information
        var tokenDisplay = totalTokens > 0 ? (totalTokens / 1000).toFixed(1) + 'K' : '0';
        var contextDisplay = contextPercent > 0 ? contextPercent.toFixed(1) + '%' : '0%';
        
        // Check if workflow is collapsed
        var isCollapsed = collapsedWorkflows.get(name) === true;
        var collapseIcon = isCollapsed ? '▶' : '▼';
        
        // Stop button logic (calculate before header)
        var isActive = metrics.status === 'running' || metrics.status === 'pending' || (metrics.status !== 'completed' && metrics.status !== 'failed');
        var hasExecutionId = metrics.executionId && metrics.executionId !== null && metrics.executionId !== '';
        var canStop = isActive && hasExecutionId;
        
        // Header with workflow name, status, and toggle switch
        html += '<div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 15px; gap: 15px; flex-wrap: wrap;">';
        html += '<div style="flex: 1; min-width: 200px;">';
        html += '<h2 style="margin: 0 0 5px 0; font-size: 18px; color: #667eea; font-weight: 600;">' + name + '</h2>';
        html += '<div class="status-badge status-' + (metrics.status || 'pending') + '" style="display: inline-block; font-size: 11px; padding: 3px 8px; border-radius: 4px; font-weight: 600;">' + statusUpper + '</div>';
        html += '</div>';
        html += '<div style="display: flex; align-items: center; gap: 12px; flex-wrap: wrap;">';
        
        // Sexy Toggle Switch
        var isRunning = metrics.status === 'running' || metrics.status === 'pending';
        var toggleId = 'toggle-' + name.replace(/[^a-zA-Z0-9]/g, '-');
        var toggleChecked = isRunning && canStop ? 'checked' : '';
        var safeNameForToggle = name.replace(/"/g, '&quot;').replace(/'/g, '&#39;');
        var executionIdForToggle = String(metrics.executionId || '');
        
        html += '<div style="display: flex; flex-direction: column; align-items: center; gap: 4px;">';
        html += '<label style="font-size: 10px; color: #888; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px;">' + (isRunning ? 'Running' : 'Stopped') + '</label>';
        html += '<label class="toggle-switch" style="position: relative; display: inline-block; width: 60px; height: 32px; cursor: pointer;">';
        html += '<input type="checkbox" id="' + toggleId + '" ' + toggleChecked + ' data-workflow-name="' + safeNameForToggle + '" data-execution-id="' + executionIdForToggle + '" style="opacity: 0; width: 0; height: 0;">';
        html += '<span class="toggle-slider" style="position: absolute; cursor: pointer; top: 0; left: 0; right: 0; bottom: 0; background-color: ' + (isRunning ? '#4CAF50' : '#666') + '; transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1); border-radius: 34px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3), inset 0 1px 2px rgba(255, 255, 255, 0.1);">';
        html += '<span class="toggle-knob" style="position: absolute; content: ""; height: 24px; width: 24px; left: 4px; bottom: 4px; background: linear-gradient(135deg, #ffffff 0%, #f0f0f0 100%); transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1); border-radius: 50%; box-shadow: 0 2px 6px rgba(0, 0, 0, 0.4), 0 0 0 2px ' + (isRunning ? 'rgba(76, 175, 80, 0.2)' : 'rgba(102, 102, 102, 0.2)') + '; display: flex; align-items: center; justify-content: center; font-size: 10px;">';
        html += (isRunning ? '▶' : '⏸');
        html += '</span>';
        html += '</span>';
        html += '</label>';
        html += '</div>';
        
        // Collapse button - use data attribute to avoid quote escaping issues
        var safeName = name.replace(/"/g, '&quot;').replace(/'/g, '&#39;');
        html += '<button data-workflow-name="' + safeName + '" class="collapse-btn" style="background: rgba(102, 126, 234, 0.2); border: 1px solid rgba(102, 126, 234, 0.4); color: #667eea; padding: 6px 12px; border-radius: 6px; cursor: pointer; font-size: 14px; font-weight: 600; transition: all 0.2s ease; display: flex; align-items: center; gap: 6px;">';
        html += '<span style="font-size: 10px;">' + collapseIcon + '</span>';
        html += '<span>' + (isCollapsed ? 'Expand' : 'Collapse') + '</span>';
        html += '</button>';
        html += '</div>';
        html += '</div>';
        
        // Collapsible content wrapper
        var contentId = 'workflow-content-' + name.replace(/[^a-zA-Z0-9]/g, '-');
        // Use display: none for collapsed state instead of max-height to avoid rendering issues
        html += '<div id="' + contentId + '" style="' + (isCollapsed ? 'display: none;' : 'display: block;') + '">';
        
        // Workflow Details Section (always visible when expanded)
        html += '<div style="background: rgba(26, 31, 58, 0.4); border-radius: 8px; padding: 15px; margin-bottom: 15px; border: 1px solid rgba(102, 126, 234, 0.2);">';
        html += '<div style="font-size: 12px; color: #667eea; font-weight: 600; margin-bottom: 10px; display: flex; align-items: center; gap: 8px;">';
        html += '📋 Workflow Details';
        html += '</div>';
        
        // Process and Execution Info
        html += '<div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 10px; margin-bottom: 10px;">';
        
        // Process Status
        html += '<div style="padding: 8px; background: rgba(15, 20, 34, 0.6); border-radius: 6px; border-left: 3px solid ' + processStatusColor + ';">';
        html += '<div style="font-size: 10px; color: #888; margin-bottom: 4px;">Process Status</div>';
        html += '<div style="display: flex; align-items: center; gap: 6px;">';
        html += '<span style="display: inline-block; width: 8px; height: 8px; background: ' + processStatusColor + '; border-radius: 50%;"></span>';
        html += '<span style="font-size: 12px; color: ' + processStatusColor + '; font-weight: 600;">' + processStatusText + '</span>';
        html += '</div>';
        if (processStatus.pid) {
          html += '<div style="font-size: 10px; color: #888; margin-top: 4px;">PID: ' + processStatus.pid + '</div>';
        } else {
          html += '<div style="font-size: 10px; color: #888; margin-top: 4px;">PID: N/A</div>';
        }
        html += '</div>';
        
        // Cursor Agent Status
        html += '<div style="padding: 8px; background: rgba(15, 20, 34, 0.6); border-radius: 6px; border-left: 3px solid ' + cursorAgentStatusColor + ';">';
        html += '<div style="font-size: 10px; color: #888; margin-bottom: 4px;">cursor-agent</div>';
        html += '<div style="display: flex; align-items: center; gap: 6px;">';
        html += '<span style="display: inline-block; width: 8px; height: 8px; background: ' + cursorAgentStatusColor + '; border-radius: 50%;"></span>';
        html += '<span style="font-size: 12px; color: ' + cursorAgentStatusColor + '; font-weight: 600;">' + cursorAgentStatusText + '</span>';
        html += '</div>';
        html += '</div>';
        
        // Execution ID
        html += '<div style="padding: 8px; background: rgba(15, 20, 34, 0.6); border-radius: 6px; border-left: 3px solid #667eea;">';
        html += '<div style="font-size: 10px; color: #888; margin-bottom: 4px;">Execution ID</div>';
        html += '<div style="font-size: 11px; color: #667eea; font-weight: 600; word-break: break-all; font-family: monospace;">' + (metrics.executionId || 'N/A') + '</div>';
        html += '</div>';
        
        // Model and Agent (if available)
        if (modelInfo || currentAgent) {
          html += '<div style="padding: 8px; background: rgba(15, 20, 34, 0.6); border-radius: 6px; border-left: 3px solid #9C27B0;">';
          html += '<div style="font-size: 10px; color: #888; margin-bottom: 4px;">AI Model</div>';
          html += '<div style="font-size: 12px; color: #9C27B0; font-weight: 600;">' + (modelInfo || 'auto') + '</div>';
          html += '</div>';
        }
        
        html += '</div>'; // Close grid
        
        // Workflow YAML Configuration Panel (collapsible)
        html += '<div style="margin-top: 15px; padding-top: 15px; border-top: 1px solid rgba(102, 126, 234, 0.2);">';
        html += '<div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 10px;">';
        html += '<div style="display: flex; align-items: center; gap: 8px;">';
        html += '<span style="font-size: 12px; color: #667eea; font-weight: 600; display: flex; align-items: center; gap: 6px;">';
        html += '📄 Workflow Configuration';
        html += '</span>';
        html += '</div>';
        html += '<div style="display: flex; align-items: center; gap: 8px;">';
        var editBtnId = 'edit-workflow-btn-' + name.replace(/[^a-zA-Z0-9]/g, '-');
        html += '<button id="' + editBtnId + '" data-workflow-name="' + safeName + '" style="padding: 4px 10px; font-size: 11px; font-weight: 600; border-radius: 4px; cursor: pointer; background: rgba(102, 126, 234, 0.2); border: 1px solid rgba(102, 126, 234, 0.4); color: #667eea; transition: all 0.2s ease;">';
        html += '✏️ Edit';
        html += '</button>';
        var toggleYamlId = 'toggle-yaml-' + name.replace(/[^a-zA-Z0-9]/g, '-');
        html += '<button id="toggle-yaml-btn-' + name.replace(/[^a-zA-Z0-9]/g, '-') + '" data-workflow-name="' + safeName + '" data-toggle-id="' + toggleYamlId + '" style="padding: 4px 10px; font-size: 11px; font-weight: 600; border-radius: 4px; cursor: pointer; background: rgba(102, 126, 234, 0.2); border: 1px solid rgba(102, 126, 234, 0.4); color: #667eea; transition: all 0.2s ease;">';
        html += '▼ Show YAML';
        html += '</button>';
        html += '</div>';
        html += '</div>';
        html += '<div id="workflow-yaml-' + name.replace(/[^a-zA-Z0-9]/g, '-') + '" style="display: none; margin-top: 10px; padding: 12px; background: rgba(15, 20, 34, 0.6); border-radius: 6px; border: 1px solid rgba(102, 126, 234, 0.2); max-height: 400px; overflow-y: auto; overflow-x: auto;">';
        html += '<pre style="margin: 0; font-family: Monaco, Menlo, &quot;Courier New&quot;, monospace; font-size: 11px; line-height: 1.5; color: #e0e0e0; white-space: pre; word-wrap: normal; overflow-x: auto;">';
        html += '<div style="color: #888; font-style: italic;">Click &quot;Show YAML&quot; to load workflow configuration...</div>';
        html += '</pre>';
        html += '</div>';
        html += '</div>';
        
        // Workflow Controls (Start/Stop/Configure) - only show if workflow is not running
        if (metrics.status !== 'running' && metrics.status !== 'pending') {
          html += '<div style="padding-top: 10px; border-top: 1px solid rgba(102, 126, 234, 0.2); margin-top: 10px;">';
          html += '<div style="font-size: 11px; color: #888; margin-bottom: 8px;">Workflow Controls</div>';
          html += '<div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(120px, 1fr)); gap: 8px;">';
          
          // Model selector
          html += '<div>';
          html += '<label style="display: block; font-size: 10px; color: #888; margin-bottom: 4px;">Model</label>';
          html += '<select id="model-select-' + name.replace(/[^a-zA-Z0-9]/g, '-') + '" style="width: 100%; padding: 4px 6px; background: rgba(15, 20, 34, 0.8); border: 1px solid rgba(102, 126, 234, 0.3); border-radius: 4px; color: #e0e0e0; font-size: 11px;">';
          html += '<option value="auto"' + ((metrics.currentModel === 'auto' || !metrics.currentModel) ? ' selected' : '') + '>Auto</option>';
          html += '<option value="claude-4-5-sonnet"' + (metrics.currentModel === 'claude-4-5-sonnet' ? ' selected' : '') + '>Claude 4.5 Sonnet</option>';
          html += '<option value="claude-4-5-haiku"' + (metrics.currentModel === 'claude-4-5-haiku' ? ' selected' : '') + '>Claude 4.5 Haiku</option>';
          html += '<option value="gpt-5"' + (metrics.currentModel === 'gpt-5' ? ' selected' : '') + '>GPT-5</option>';
          html += '<option value="gpt-5-mini"' + (metrics.currentModel === 'gpt-5-mini' ? ' selected' : '') + '>GPT-5 Mini</option>';
          html += '</select>';
          html += '</div>';
          
          // Runner selector
          html += '<div>';
          html += '<label style="display: block; font-size: 10px; color: #888; margin-bottom: 4px;">Runner</label>';
          html += '<select id="runner-select-' + name.replace(/[^a-zA-Z0-9]/g, '-') + '" style="width: 100%; padding: 4px 6px; background: rgba(15, 20, 34, 0.8); border: 1px solid rgba(102, 126, 234, 0.3); border-radius: 4px; color: #e0e0e0; font-size: 11px;">';
          html += '<option value="cursor" selected>Cursor CLI</option>';
          html += '<option value="dummy">Dummy</option>';
          html += '</select>';
          html += '</div>';
          
          html += '</div>'; // Close grid
          html += '</div>'; // Close controls section
        }
        
        html += '</div>'; // Close workflow details section
        
        html += '<div class="progress-bar">';
        html += '<div class="progress-fill" style="width: ' + progress + '%"></div>';
        html += '</div>';
        html += '<div class="metrics">';
        
        // Steps metric
        html += '<div class="metric">';
        html += '<div class="metric-label">Steps</div>';
        html += '<div class="metric-value">' + stepsCompleted + ' / ' + totalSteps + '</div>';
        html += '</div>';
        
        // Success Rate metric
        html += '<div class="metric">';
        html += '<div class="metric-label">Success Rate</div>';
        html += '<div class="metric-value">' + successRate + '%</div>';
        html += '</div>';
        
        // Duration metric
        html += '<div class="metric">';
        html += '<div class="metric-label">Duration</div>';
        html += '<div class="metric-value">' + duration + '</div>';
        html += '</div>';
        
        // Current Step metric
        html += '<div class="metric">';
        html += '<div class="metric-label">Current Step</div>';
        html += '<div class="metric-value" style="font-size: 14px; word-break: break-word;">' + currentStep + '</div>';
        html += '</div>';
        
        // Model metric
        html += '<div class="metric">';
        html += '<div class="metric-label">AI Model</div>';
        html += '<div class="metric-value" style="font-size: 13px; word-break: break-word;">' + (modelInfo || 'auto') + '</div>';
        html += '</div>';
        
        // Agent metric
        html += '<div class="metric">';
        html += '<div class="metric-label">Agent</div>';
        html += '<div class="metric-value" style="font-size: 13px;">' + (currentAgent || 'N/A') + '</div>';
        html += '</div>';
        
        // Tokens metric
        html += '<div class="metric">';
        html += '<div class="metric-label">Tokens</div>';
        html += '<div class="metric-value" style="font-size: 14px;">' + tokenDisplay + '</div>';
        html += '<div style="font-size: 10px; color: #888; margin-top: 3px;">Context: ' + contextDisplay + '</div>';
        html += '</div>';
        
        // Cost metric
        html += '<div class="metric">';
        html += '<div class="metric-label">Cost</div>';
        html += '<div class="metric-value" style="font-size: 14px; color: #ff9800;">' + costDisplay + '</div>';
        html += '</div>';
        
        // Current Activity - More prominent
        var activityText = metrics.currentActivity || 'Processing...';
        var activityColor = metrics.status === 'running' ? '#4CAF50' : metrics.status === 'failed' ? '#f44336' : '#888';
        html += '<div class="metric" style="grid-column: 1 / -1; background: rgba(76, 175, 80, 0.1); border: 2px solid rgba(76, 175, 80, 0.3); padding: 15px; border-radius: 10px; margin-top: 15px;">';
        html += '<div class="metric-label" style="font-size: 12px; color: #4CAF50; font-weight: 700; margin-bottom: 8px; display: flex; align-items: center; gap: 8px;">';
        html += '<span style="display: inline-block; width: 8px; height: 8px; background: ' + activityColor + '; border-radius: 50%; animation: pulse 2s infinite;"></span>';
        html += 'CURRENT ACTIVITY';
        html += '</div>';
        html += '<div id="activity-text-' + name + '" style="font-size: 14px; word-break: break-word; color: #e0e0e0; font-weight: 500; line-height: 1.5; min-height: 24px;">';
        html += activityText;
        html += '</div>';
        html += '</div>';
        
        // Debug logging
        console.log('📊 Dashboard: Workflow', name, '- Status:', metrics.status, '- ExecutionId:', metrics.executionId, '- CanStop:', canStop);
        
        // Activity History
        html += '<div class="metric" style="grid-column: 1 / -1; margin-top: 10px; max-height: 120px; overflow-y: auto;">';
        html += '<div class="metric-label" style="font-size: 11px; color: #666; margin-bottom: 8px;">Recent Activity</div>';
        html += '<div id="activity-history-' + name + '" style="font-size: 11px; color: #999; line-height: 1.6;">';
        html += '<div style="padding: 4px 0; color: #888;">Loading activity...</div>';
        html += '</div>';
        html += '</div>';
        
        // Close collapsible content wrapper
        html += '</div>';
        
        html += '</div>';
        
        card.innerHTML = html;
        card.style.position = 'relative'; // For absolute positioning of stop button
        workflowsDiv.appendChild(card);
        
        // Attach collapse button event listener
        var collapseBtn = card.querySelector('.collapse-btn');
        if (collapseBtn) {
          collapseBtn.addEventListener('click', function() {
            var workflowNameAttr = this.getAttribute('data-workflow-name');
            if (workflowNameAttr) {
              // Decode HTML entities
              var workflowName = workflowNameAttr.replace(/&quot;/g, '"').replace(/&#39;/g, "'");
              window.toggleWorkflowCollapse(workflowName);
            }
          });
        }
        
        // Attach toggle switch event listener
        var toggleInput = card.querySelector('#' + toggleId);
        if (toggleInput) {
          toggleInput.addEventListener('change', function() {
            var workflowNameAttr = this.getAttribute('data-workflow-name');
            var executionIdAttr = this.getAttribute('data-execution-id');
            if (workflowNameAttr) {
              // Decode HTML entities
              var workflowName = workflowNameAttr.replace(/&quot;/g, '"').replace(/&#39;/g, "'");
              var executionId = executionIdAttr && executionIdAttr !== 'null' && executionIdAttr !== '' ? executionIdAttr : null;
              
              if (this.checked) {
                // Toggled ON - Start workflow
                window.startWorkflowByName(workflowName);
              } else {
                // Toggled OFF - Stop workflow
                if (executionId) {
                  window.stopWorkflowById(executionId, workflowName);
                } else {
                  // No execution ID, just update the toggle state
                  this.checked = false;
                  console.warn('Cannot stop workflow without execution ID');
                }
              }
            }
          });
        }
        
        // Load activity history for this workflow
        updateActivityHistory(name);
        
        // Attach event listeners to YAML buttons
        var toggleBtn = document.getElementById('toggle-yaml-btn-' + name.replace(/[^a-zA-Z0-9]/g, '-'));
        if (toggleBtn) {
          toggleBtn.addEventListener('click', function() {
            var workflowName = this.getAttribute('data-workflow-name') || name;
            window.toggleWorkflowYaml(workflowName);
          });
        }
        
        var editBtn = document.getElementById('edit-workflow-btn-' + name.replace(/[^a-zA-Z0-9]/g, '-'));
        if (editBtn) {
          editBtn.addEventListener('click', function() {
            var workflowName = this.getAttribute('data-workflow-name') || name;
            window.editWorkflowYaml(workflowName);
          });
        }
        
        // Don't load YAML automatically - only load when user clicks "Show YAML"
      });
    }
    
    // Load workflow YAML content
    window.loadWorkflowYaml = function(workflowName) {
      try {
        var safeName = String(workflowName || '');
        var yamlContainerId = 'workflow-yaml-' + safeName.replace(/[^a-zA-Z0-9]/g, '-');
        var yamlContainer = document.getElementById(yamlContainerId);
        if (!yamlContainer) {
          console.warn('YAML container not found:', yamlContainerId);
          return;
        }
        
        var pre = yamlContainer.querySelector('pre');
        if (!pre) {
          console.warn('Pre element not found in YAML container');
          return;
        }
        
        // Show loading state
        pre.innerHTML = '<div style="color: #888; font-style: italic;">Loading workflow configuration...</div>';
        
        fetch('/api/workflows/' + encodeURIComponent(safeName) + '/yaml')
          .then(function(response) {
            if (!response.ok) {
              return response.json().catch(function() { return { error: 'Unknown error' }; }).then(function(errorData) {
                throw new Error(errorData.error || 'Failed to load YAML');
              });
            }
            return response.json();
          })
          .then(function(data) {
            var yamlContent = data.yaml || '';
            
            if (!yamlContent) {
              pre.innerHTML = '<div style="color: #ff9800; padding: 10px;">No YAML content found</div>';
              return;
            }
            
            // Escape HTML first
            var escaped = escapeHtml(yamlContent);
            
            // Simple syntax highlighting - just display the YAML with basic formatting
            // Use String.fromCharCode(10) for newline to avoid template literal issues
            var newlineChar = String.fromCharCode(10);
            var lines = escaped.split(newlineChar);
            var highlightedLines = [];
            
            for (var i = 0; i < lines.length; i++) {
              var line = lines[i];
              var trimmed = line.trim();
              var highlighted = line;
              
              // Highlight comments (simple check)
              if (trimmed.indexOf('#') === 0) {
                highlighted = '<span style="color: #888; font-style: italic;">' + line + '</span>';
              } else if (trimmed.indexOf('name:') === 0 || trimmed.indexOf('- name:') !== -1) {
                // Highlight name fields
                var parts = line.split('name:');
                if (parts.length > 1) {
                  highlighted = parts[0] + '<span style="color: #667eea; font-weight: 600;">name:</span>' + parts.slice(1).join('name:');
                }
              } else if (trimmed.indexOf('agent:') === 0 || trimmed.indexOf('agent:') !== -1) {
                // Highlight agent fields
                var parts = line.split('agent:');
                if (parts.length > 1) {
                  highlighted = parts[0] + '<span style="color: #ff9800; font-weight: 600;">agent:</span>' + parts.slice(1).join('agent:');
                }
              } else if (trimmed.indexOf('description:') === 0 || trimmed.indexOf('metadata:') === 0 || trimmed.indexOf('steps:') === 0 || trimmed.indexOf('agents:') === 0) {
                // Highlight section headers
                var colonIndex = line.indexOf(':');
                if (colonIndex !== -1) {
                  highlighted = '<span style="color: #9C27B0; font-weight: 600;">' + line.substring(0, colonIndex + 1) + '</span>' + line.substring(colonIndex + 1);
                }
              } else if (line.indexOf(':') !== -1 && trimmed.length > 0) {
                // Highlight other keys (simple approach)
                var colonIndex = line.indexOf(':');
                if (colonIndex !== -1 && colonIndex < line.length - 1) {
                  var beforeColon = line.substring(0, colonIndex);
                  var afterColon = line.substring(colonIndex);
                  // Only highlight if it looks like a key (alphanumeric before colon)
                  var keyPattern = new RegExp('[a-zA-Z_][a-zA-Z0-9_-]*$');
                  if (keyPattern.test(beforeColon.trim())) {
                    highlighted = beforeColon + '<span style="color: #667eea; font-weight: 600;">:</span>' + afterColon.substring(1);
                  }
                }
              }
              
              highlightedLines.push(highlighted);
            }
            
            // Join lines back using newline character
            var highlighted = highlightedLines.join(newlineChar);
            
            // Display in pre tag
            pre.innerHTML = highlighted;
          })
          .catch(function(error) {
            console.error('Failed to load workflow YAML:', error);
            var yamlContainerId = 'workflow-yaml-' + safeName.replace(/[^a-zA-Z0-9]/g, '-');
            var yamlContainer = document.getElementById(yamlContainerId);
            if (yamlContainer) {
              var pre = yamlContainer.querySelector('pre');
              if (pre) {
                pre.innerHTML = '<div style="color: #f44336; padding: 10px;">Error loading workflow YAML: ' + escapeHtml(String(error)) + '</div>';
              }
            }
          });
      } catch (error) {
        console.error('Failed to load workflow YAML:', error);
        var yamlContainerId = 'workflow-yaml-' + String(workflowName || '').replace(/[^a-zA-Z0-9]/g, '-');
        var yamlContainer = document.getElementById(yamlContainerId);
        if (yamlContainer) {
          var pre = yamlContainer.querySelector('pre');
          if (pre) {
            pre.innerHTML = '<div style="color: #f44336; padding: 10px;">Error loading workflow YAML: ' + escapeHtml(String(error)) + '</div>';
          }
        }
      }
    };
    
    // Toggle workflow YAML visibility
    window.toggleWorkflowYaml = function(workflowName) {
      try {
        var safeName = String(workflowName || '');
        var yamlContainerId = 'workflow-yaml-' + safeName.replace(/[^a-zA-Z0-9]/g, '-');
        var toggleBtnId = 'toggle-yaml-btn-' + safeName.replace(/[^a-zA-Z0-9]/g, '-');
        var yamlContainer = document.getElementById(yamlContainerId);
        var toggleBtn = document.getElementById(toggleBtnId);
        
        if (yamlContainer && toggleBtn) {
          var isVisible = yamlContainer.style.display !== 'none' && yamlContainer.style.display !== '';
          yamlContainer.style.display = isVisible ? 'none' : 'block';
          toggleBtn.textContent = isVisible ? '▼ Show YAML' : '▲ Hide YAML';
          
          // Load YAML if showing for the first time
          if (!isVisible) {
            var pre = yamlContainer.querySelector('pre');
            var shouldLoad = false;
            if (pre) {
              var preText = pre.textContent || pre.innerText || '';
              shouldLoad = preText.includes('Click') || preText.includes('Loading') || preText.trim() === '';
            } else {
              shouldLoad = true;
            }
            if (shouldLoad) {
              window.loadWorkflowYaml(safeName);
            }
          }
        } else {
          console.warn('YAML container or toggle button not found:', {
            container: yamlContainer ? 'found' : 'not found',
            button: toggleBtn ? 'found' : 'not found',
            containerId: yamlContainerId,
            buttonId: toggleBtnId
          });
        }
      } catch (error) {
        console.error('Error toggling workflow YAML:', error);
      }
    };
    
    // Edit workflow YAML
    window.editWorkflowYaml = async function(workflowName) {
      try {
        // Load current YAML
        var response = await fetch('/api/workflows/' + encodeURIComponent(workflowName) + '/yaml');
        if (!response.ok) {
          await window.showDialog('Error', 'Failed to load workflow YAML: ' + (await response.json()).error);
          return;
        }
        
        var data = await response.json();
        var currentYaml = data.yaml || '';
        var filePath = data.filePath || '';
        
        // Show edit modal
        var modal = document.getElementById('edit-workflow-yaml-modal');
        if (!modal) {
          // Create modal if it doesn't exist
          modal = document.createElement('div');
          modal.id = 'edit-workflow-yaml-modal';
          modal.className = 'modal';
          modal.innerHTML = '<div class="modal-content" style="max-width: 900px; max-height: 90vh; overflow-y: auto;">' +
            '<div class="modal-header">' +
            '<h2>Edit Workflow Configuration</h2>' +
            '<span class="close" onclick="window.closeEditWorkflowYamlModal()">&times;</span>' +
            '</div>' +
            '<div style="margin-bottom: 15px; padding: 10px; background: rgba(102, 126, 234, 0.1); border-radius: 6px; font-size: 12px; color: #888;">' +
            '<div style="margin-bottom: 5px;"><strong>Workflow:</strong> <span style="color: #667eea; font-weight: 600;">' + escapeHtml(workflowName) + '</span></div>' +
            '<div><strong>File:</strong> <code style="background: rgba(102, 126, 234, 0.2); padding: 2px 6px; border-radius: 3px; font-size: 11px;">' + escapeHtml(filePath) + '</code></div>' +
            '</div>' +
            '<div class="form-group">' +
            '<label style="display: flex; align-items: center; gap: 8px; margin-bottom: 8px;">' +
            '<span>YAML Configuration</span>' +
            '<span class="tooltip-container">' +
            '<span class="help-icon" style="font-size: 10px; width: 12px; height: 12px;">?</span>' +
            '<span class="tooltip" style="width: 250px; font-size: 11px;">Edit the workflow YAML configuration. Changes will be saved to the file on disk.</span>' +
            '</span>' +
            '</label>' +
            '<textarea id="workflow-yaml-editor" style="width: 100%; min-height: 500px; font-family: Monaco, Menlo, &quot;Courier New&quot;, monospace; font-size: 12px; line-height: 1.5; resize: vertical; background: rgba(15, 20, 34, 0.8); border: 1px solid rgba(102, 126, 234, 0.3); border-radius: 6px; color: #e0e0e0; padding: 12px;"></textarea>' +
            '</div>' +
            '<div style="display: flex; gap: 10px; justify-content: flex-end; margin-top: 20px; padding-top: 15px; border-top: 1px solid rgba(102, 126, 234, 0.2);">' +
            '<button class="btn btn-primary" onclick="window.saveWorkflowYaml(' + JSON.stringify(workflowName) + ')">💾 Save Changes</button>' +
            '<button class="btn" onclick="window.closeEditWorkflowYamlModal()" style="background: rgba(102, 102, 102, 0.3); border: 1px solid rgba(102, 102, 102, 0.5); color: #e0e0e0;">Cancel</button>' +
            '</div>' +
            '</div>';
          document.body.appendChild(modal);
          
          // Close modal when clicking outside
          modal.addEventListener('click', function(event) {
            if (event.target === modal) {
              window.closeEditWorkflowYamlModal();
            }
          });
        }
        
        // Update modal header with workflow name
        var modalHeader = modal.querySelector('.modal-header h2');
        if (modalHeader) {
          modalHeader.textContent = 'Edit Workflow: ' + workflowName;
        }
        
        // Update file path display
        var filePathDisplay = modal.querySelector('code');
        if (filePathDisplay) {
          filePathDisplay.textContent = filePath || 'Not specified';
        }
        
        // Populate editor
        var editor = document.getElementById('workflow-yaml-editor');
        if (editor) {
          editor.value = currentYaml;
          // Focus and scroll to top
          editor.focus();
          editor.scrollTop = 0;
        }
        
        // Store workflow name and file path
        modal.setAttribute('data-workflow-name', workflowName);
        modal.setAttribute('data-file-path', filePath);
        
        // Show modal
        modal.style.display = 'block';
      } catch (error) {
        await window.showDialog('Error', 'Failed to load workflow for editing: ' + error);
      }
    };
    
    // Close edit workflow YAML modal
    window.closeEditWorkflowYamlModal = function() {
      var modal = document.getElementById('edit-workflow-yaml-modal');
      if (modal) {
        modal.style.display = 'none';
      }
    };
    
    // Save workflow YAML
    window.saveWorkflowYaml = async function(workflowName) {
      try {
        var modal = document.getElementById('edit-workflow-yaml-modal');
        var editor = document.getElementById('workflow-yaml-editor');
        
        if (!modal || !editor) {
          await window.showDialog('Error', 'Editor not found');
          return;
        }
        
        var yamlContent = editor.value.trim();
        if (!yamlContent) {
          await window.showDialog('Error', 'YAML content cannot be empty');
          return;
        }
        
        // Show confirmation
        var confirmed = await window.showDialog(
          'Save Workflow',
          'Are you sure you want to save changes to workflow "' + workflowName + '"? This will overwrite the existing YAML file.',
          true
        );
        
        if (!confirmed) {
          return;
        }
        
        // Get file path from modal
        var modal = document.getElementById('edit-workflow-yaml-modal');
        var savedFilePath = modal ? modal.getAttribute('data-file-path') : null;
        
        // Save to server
        var response = await fetch('/api/workflows/' + encodeURIComponent(workflowName) + '/yaml', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ 
            yaml: yamlContent,
            filePath: savedFilePath
          })
        });
        
        var data = await response.json();
        if (response.ok && data.success) {
          await window.showDialog('Success', 'Workflow YAML saved successfully!');
          window.closeEditWorkflowYamlModal();
          
          // Reload YAML display if it's visible
          var yamlContainerId = 'workflow-yaml-' + workflowName.replace(/[^a-zA-Z0-9]/g, '-');
          var yamlContainer = document.getElementById(yamlContainerId);
          if (yamlContainer && yamlContainer.style.display !== 'none') {
            window.loadWorkflowYaml(workflowName);
          }
        } else {
          await window.showDialog('Error', 'Failed to save workflow: ' + (data.error || 'Unknown error'));
        }
      } catch (error) {
        await window.showDialog('Error', 'Error saving workflow: ' + error);
      }
    };
    
    function formatDuration(ms) {
      if (ms < 1000) return ms + 'ms';
      if (ms < 60000) return (ms / 1000).toFixed(1) + 's';
      return (ms / 60000).toFixed(1) + 'm';
    }

    // Change model for workflow/step
    window.changeModel = async function(workflowName, stepName) {
      var selectId = 'model-select-' + workflowName;
      var select = document.getElementById(selectId);
      if (!select || !select.value) {
        await window.showDialog('No Model Selected', 'Please select a model from the dropdown.');
        return;
      }

      var modelId = select.value;
      var modelDisplayName = modelId === 'auto' ? 'Auto (Intelligent Selection)' : modelId;
      var endpoint = stepName && stepName !== 'N/A' && stepName !== ''
        ? '/api/workflows/' + encodeURIComponent(workflowName) + '/steps/' + encodeURIComponent(stepName) + '/model'
        : '/api/workflows/' + encodeURIComponent(workflowName) + '/model';

      try {
        var modelData = { modelId: modelId };
        var fetchOptions = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(modelData)
        };
        var response = await fetch(endpoint, fetchOptions);
        var data = await response.json();
        if (data.success) {
          await window.showDialog('Model Changed', 'Model changed to ' + modelDisplayName);
          // Refresh workflow display to show updated model
          setTimeout(function() {
            loadExecutions();
          }, 500);
        } else {
          await window.showDialog('Failed to Change Model', 'Error: ' + (data.error || 'Unknown error'));
        }
      } catch (error) {
        alert('Error changing model: ' + error);
      }
    };

    // Change agent for step
    window.changeAgent = async function(workflowName, stepName) {
      if (!stepName || stepName === 'N/A' || stepName === '') {
        alert('Please select a step first');
        return;
      }

      var selectId = 'agent-select-' + workflowName;
      var select = document.getElementById(selectId);
      if (!select || !select.value) {
        alert('Please select an agent');
        return;
      }

      var agent = select.value;
      var reason = prompt('Reason for agent switch (optional):') || undefined;

      try {
        var agentData = { agent: agent, reason: reason };
        var fetchOptions = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(agentData)
        };
        var endpoint = '/api/workflows/' + encodeURIComponent(workflowName) + '/steps/' + encodeURIComponent(stepName) + '/agent';
        var response = await fetch(endpoint, fetchOptions);
        var data = await response.json();
        if (data.success) {
          alert('Agent switched to ' + agent);
        } else {
          alert('Failed to switch agent: ' + (data.error || 'Unknown error'));
        }
      } catch (error) {
        alert('Error switching agent: ' + error);
      }
    };

    // Toggle workflow collapse/expand
    window.toggleWorkflowCollapse = function(workflowName) {
      var isCollapsed = collapsedWorkflows.get(workflowName) === true;
      collapsedWorkflows.set(workflowName, !isCollapsed);
      
      // Update the specific workflow's content visibility
      var contentId = 'workflow-content-' + workflowName.replace(/[^a-zA-Z0-9]/g, '-');
      var contentDiv = document.getElementById(contentId);
      if (contentDiv) {
        if (isCollapsed) {
          // Expand
          contentDiv.style.display = 'block';
        } else {
          // Collapse
          contentDiv.style.display = 'none';
        }
      }
      
      // Update button text and icon
      var collapseBtn = document.querySelector('[data-workflow-name="' + workflowName.replace(/"/g, '&quot;').replace(/'/g, '&#39;') + '"]');
      if (collapseBtn) {
        var iconSpan = collapseBtn.querySelector('span:first-child');
        var textSpan = collapseBtn.querySelector('span:last-child');
        if (iconSpan) {
          iconSpan.textContent = isCollapsed ? '▼' : '▶';
        }
        if (textSpan) {
          textSpan.textContent = isCollapsed ? 'Collapse' : 'Expand';
        }
      }
    };

    // Initialize dashboard with state restoration (called after all functions are defined)
    function initializeDashboard() {
      // Ensure workflowsDiv is available
      if (!workflowsDiv) {
        workflowsDiv = document.getElementById('workflows');
        if (!workflowsDiv) {
          console.error('Failed to find workflows div element, retrying...');
          setTimeout(initializeDashboard, 100);
          return;
        }
      }
      
      console.log('📊 Dashboard: Initializing...');
      
      // Update visual status
      var scriptStatusEl = document.getElementById('script-status');
      if (scriptStatusEl) {
        scriptStatusEl.textContent = '🔄 Initializing...';
        scriptStatusEl.style.background = 'rgba(255, 152, 0, 0.9)';
      }
      
      // Try to load state from localStorage first
      var stateRestored = false;
      try {
        stateRestored = loadWorkflowState();
        // If state was restored, render workflows now
        if (stateRestored && workflowsDiv) {
          console.log('📊 Dashboard: Rendering restored workflows');
          renderWorkflows();
        }
      } catch (error) {
        console.warn('Failed to restore state on init:', error);
      }
      
      console.log('📊 Dashboard: Loading initial data...');
      loadRoles();
      loadWorkflows();
      // Load MCP status immediately
      loadMcpStatus().catch(function(error) {
        console.error('📊 Dashboard: Failed to load MCP status:', error);
      });
      loadSystemInfo();
      
      // Only load executions if state wasn't restored (to avoid overwriting restored state)
      if (!stateRestored) {
        console.log('📊 Dashboard: Loading executions (no state restored)');
        loadExecutions();
      } else {
        // Still refresh executions in background to get latest data, but merge with restored state
        console.log('📊 Dashboard: State restored, refreshing executions in background');
        setTimeout(function() {
          loadExecutions();
        }, 1000);
      }
      
      initWebSocket();
      
      // Set up periodic updates
      setInterval(loadMcpStatus, 5000); // Refresh MCP status every 5 seconds
      setInterval(function() {
        loadMcpStatus();
        // Also refresh details if server is running
        var statusBadge = document.getElementById('mcp-status');
        if (statusBadge && statusBadge.textContent === 'Running') {
          loadMcpDetails();
        }
      }, 5000); // Refresh MCP details every 5 seconds
      setInterval(loadExecutions, 2000); // Refresh executions every 2 seconds for faster updates
      setInterval(loadSystemInfo, 2000); // Refresh system info every 2 seconds for live updates
      
      // Save state periodically (every 5 seconds) and before page unload
      setInterval(function() {
        saveWorkflowState();
      }, 5000);
      
      window.addEventListener('beforeunload', function() {
        saveWorkflowState();
      });
    }
    
    // CRITICAL: Force immediate check
    console.log('🚀 Setting up initialization...');
    console.log('🚀 Current readyState:', document.readyState);
    
    // Start initialization when DOM is ready
    if (document.readyState === 'loading') {
      console.log('🚀 DOM is loading, waiting for DOMContentLoaded...');
      document.addEventListener('DOMContentLoaded', function() {
        console.log('🚀 DOMContentLoaded fired!');
        initializeDashboard();
      });
    } else {
      // DOM is already loaded
      console.log('🚀 DOM already loaded, initializing immediately...');
      setTimeout(function() {
        initializeDashboard();
      }, 100); // Small delay to ensure everything is ready
    }
    
    // Also try immediate initialization as fallback
    setTimeout(function() {
      if (!workflowsDiv) {
        console.warn('⚠️ workflowsDiv still not found after 500ms, retrying...');
        workflowsDiv = document.getElementById('workflows');
        if (workflowsDiv) {
          console.log('✅ Found workflowsDiv on retry!');
          initializeDashboard();
        } else {
          console.error('❌ workflowsDiv still not found after retry!');
        }
      }
    }, 500);
  </script>
</body>
</html>`;
    // Note: This entire method is deprecated. The React dashboard is now served from dist/
    // Keeping minimal stub to avoid breaking any potential references
    return '';
  }
}
