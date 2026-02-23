/**
 * Workflow Manager Service
 * 
 * Manages workflow execution from the dashboard
 */

import { spawn, ChildProcess } from 'child_process';
import * as path from 'path';
import * as fs from 'fs';
import { findRepoRoot, getNodeArgs } from '../cli/utils';
import { RunOptions } from '../cli/commands/run';
import { ScrumMaster, AgentAssignment } from './ScrumMaster';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';

export interface WorkflowExecution {
  id: string;
  workflowName: string;
  process: ChildProcess;
  startTime: Date;
  options: RunOptions;
  cursorAgentPids?: Map<string, number>; // Map of agent GUID to PID
}

export class WorkflowManager {
  private executions: Map<string, WorkflowExecution> = new Map();
  private mcpServerProcess: ChildProcess | null = null;
  private mcpServerPort: number = 3001;
  private logCallback?: (workflowName: string, level: 'stdout' | 'stderr', message: string) => void;
  private dashboardServer: any = null; // Reference to dashboard server for sending logs
  private scrumMaster: ScrumMaster = new ScrumMaster();

  /**
   * Set log callback for sending logs to dashboard
   */
  setLogCallback(callback: (workflowName: string, level: 'stdout' | 'stderr', message: string) => void): void {
    this.logCallback = callback;
  }

  /**
   * List available workflows
   */
  async listWorkflows(): Promise<Array<{ name: string; description?: string; steps: number }>> {
    try {
      const { ConfigLoader } = await import('../config/ConfigLoader');
      const loader = new ConfigLoader();
      const config = loader.loadConfig();
      
      return Object.entries(config.flows).map(([name, flow]) => ({
        name,
        description: flow.description,
        steps: flow.steps.length,
      }));
    } catch (error) {
      console.error('Error listing workflows:', error);
      return [];
    }
  }

  /**
   * Start a workflow execution
   */
  async startWorkflow(options: RunOptions): Promise<string> {
    const executionId = `exec-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
    const repoRoot = findRepoRoot();
    
    // Build command
    const args = [
      'run',
      '--flow', options.flow,
      '--runner', options.runner || 'cursor',
    ];
    
    if (options.verbose) args.push('--verbose');
    if (options.model) args.push('--model', options.model);
    if (options.maxConcurrency) args.push('--max-concurrency', String(options.maxConcurrency));
    if (options.enableCache !== undefined) args.push(options.enableCache ? '--enable-cache' : '--no-enable-cache');
    if (options.enableRetry !== undefined) args.push(options.enableRetry ? '--enable-retry' : '--no-enable-retry');
    
    // Construct script path - check multiple possible locations
    // Handle case where repoRoot might already include .maestro or not
    const possiblePaths = [
      // Standard location: repoRoot/.maestro/dist/cli/index.js
      path.join(repoRoot, '.maestro', 'dist', 'cli', 'index.js'),
      // If repoRoot is already .maestro directory
      path.join(repoRoot, 'dist', 'cli', 'index.js'),
      // If repoRoot ends with .maestro, don't add it again
      repoRoot.endsWith('.maestro') 
        ? path.join(repoRoot, 'dist', 'cli', 'index.js')
        : path.join(repoRoot, '.maestro', 'dist', 'cli', 'index.js'),
      // Relative to this file's location (when running from dist/)
      path.resolve(__dirname, '../../cli/index.js'),
      // Fallback: try to find it relative to current working directory
      path.resolve(process.cwd(), '.maestro', 'dist', 'cli', 'index.js'),
    ];
    
    let scriptPath: string | null = null;
    for (const possiblePath of possiblePaths) {
      const normalizedPath = path.normalize(possiblePath);
      if (fs.existsSync(normalizedPath)) {
        scriptPath = normalizedPath;
        break;
      }
    }
    
    if (!scriptPath) {
      console.error(`❌ Could not find CLI script.`);
      console.error(`   Repo root: ${repoRoot}`);
      console.error(`   Current working directory: ${process.cwd()}`);
      console.error(`   Tried paths:`);
      possiblePaths.forEach(p => console.error(`     - ${path.normalize(p)}`));
      throw new Error(`Could not find CLI script. Repo root: ${repoRoot}`);
    }
    
    const absoluteScriptPath = path.resolve(scriptPath);
    console.log(`🚀 Starting workflow "${options.flow}" using script: ${absoluteScriptPath}`);
    
    // Use getNodeArgs to include heap memory configuration
    const nodeArgs = getNodeArgs([absoluteScriptPath, ...args]);
    
    const childProcess = spawn('node', nodeArgs, {
      cwd: repoRoot,
      stdio: ['ignore', 'pipe', 'pipe'],
      env: { ...process.env },
    });

    // Store execution
    const execution: WorkflowExecution = {
      id: executionId,
      workflowName: options.flow,
      process: childProcess,
      startTime: new Date(),
      options,
    };
    
    this.executions.set(executionId, execution);

    // Handle process exit
    childProcess.on('exit', (code: number | null) => {
      console.log(`Workflow ${executionId} exited with code ${code}`);
      this.executions.delete(executionId);
    });

    // Log output and track process activity
    let lastOutputTime = Date.now();
    let hasOutput = false;
    
    childProcess.stdout?.on('data', (data: Buffer) => {
      hasOutput = true;
      lastOutputTime = Date.now();
      const output = data.toString();
      console.log(`[Workflow ${executionId}] ${output.substring(0, 200)}`);
      
      // Send logs to dashboard if callback is set
      if (this.logCallback) {
        // Split by lines and send each line
        const lines = output.split('\n');
        for (const line of lines) {
          const trimmed = line.trim();
          if (trimmed) {
            this.logCallback(execution.workflowName, 'stdout', trimmed);
          }
        }
      }
    });

    childProcess.stderr?.on('data', (data: Buffer) => {
      hasOutput = true;
      lastOutputTime = Date.now();
      const output = data.toString();
      console.error(`[Workflow ${executionId}] ${output.substring(0, 200)}`);
      
      // Send logs to dashboard if callback is set
      if (this.logCallback) {
        // Split by lines and send each line
        const lines = output.split('\n');
        for (const line of lines) {
          const trimmed = line.trim();
          if (trimmed) {
            this.logCallback(execution.workflowName, 'stderr', trimmed);
          }
        }
      }
    });
    
    // Monitor for process activity (check every 30 seconds)
    const activityMonitor = setInterval(() => {
      const timeSinceOutput = Date.now() - lastOutputTime;
      // If no output for 5 minutes and process is still running, might be stuck
      if (timeSinceOutput > 5 * 60 * 1000 && execution.process.exitCode === null) {
        console.warn(`[Workflow ${executionId}] No output for ${Math.floor(timeSinceOutput / 1000)}s - process may be stuck`);
      }
    }, 30000);
    
    childProcess.on('exit', () => {
      clearInterval(activityMonitor);
    });

    return executionId;
  }

  /**
   * Reevaluate workflow and reassign steps to agents (scrum master behavior)
   */
  async reevaluateWorkflow(workflowName: string): Promise<{ success: boolean; message: string }> {
    try {
      const execution = Array.from(this.executions.values())
        .find(e => e.workflowName === workflowName);
      
      if (!execution) {
        return { 
          success: false, 
          message: `No active execution found for workflow: ${workflowName}` 
        };
      }

      // Load workflow definition
      const workflows = await this.listWorkflows();
      const workflowInfo = workflows.find(w => w.name === workflowName);
      
      if (!workflowInfo) {
        return {
          success: false,
          message: `Workflow "${workflowName}" not found`
        };
      }

      // Load full workflow definition from YAML
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
            workflow.name = workflowName; // Ensure name is set
            break;
          } catch (error) {
            console.error(`Error loading workflow from ${possiblePath}:`, error);
          }
        }
      }

      if (!workflow) {
        return {
          success: false,
          message: `Could not load workflow definition for "${workflowName}"`
        };
      }

      // Start scrum master monitoring
      this.scrumMaster.startMonitoring(
        workflowName,
        workflow,
        async (assignments: AgentAssignment[]) => {
          // Notify dashboard server about reassignments
          if (this.dashboardServer && typeof this.dashboardServer.updateAgentAssignments === 'function') {
            await this.dashboardServer.updateAgentAssignments(workflowName, assignments);
          }
          
          // Log assignments
          console.log(`🎯 Scrum Master: Reassigned ${assignments.length} tasks`);
          assignments.forEach(assignment => {
            console.log(`   → ${assignment.agentName}: ${assignment.stepName}${assignment.taskName ? `.${assignment.taskName}` : ''} (GUID: ${assignment.guid})`);
          });
        }
      );

      // Get agent GUIDs and update dashboard
      const agentGuids = this.scrumMaster.getAgentGuids(workflowName);
      if (this.dashboardServer && typeof this.dashboardServer.updateWorkflowAgentGuids === 'function') {
        this.dashboardServer.updateWorkflowAgentGuids(workflowName, agentGuids);
      }

      console.log(`🔄 Scrum Master: Started monitoring workflow "${workflowName}"`);
      console.log(`   Agent GUIDs:`, Object.keys(agentGuids).map(agent => `${agent}: ${agentGuids[agent].substring(0, 8)}...`).join(', '));
      
      return { 
        success: true, 
        message: `Scrum Master activated. Maestro will continuously monitor and reassign steps to agents in sprints. Each agent has its own cursor-agent resume GUID.` 
      };
    } catch (error: any) {
      console.error(`Error reevaluating workflow ${workflowName}:`, error);
      return { 
        success: false, 
        message: error.message || 'Failed to reevaluate workflow' 
      };
    }
  }

  /**
   * Stop a workflow execution
   */
  async stopWorkflow(executionId: string): Promise<boolean> {
    const execution = this.executions.get(executionId);
    if (!execution) {
      return false;
    }

    try {
      // Stop scrum master monitoring
      this.scrumMaster.stopMonitoring(execution.workflowName);
      
      // Gracefully terminate cursor-agent processes
      if (execution.cursorAgentPids) {
        for (const [guid, pid] of execution.cursorAgentPids.entries()) {
          try {
            console.log(`🛑 Terminating cursor-agent process ${pid} (GUID: ${guid})`);
            process.kill(pid, 'SIGTERM');
            
            // Wait a bit, then force kill if still running
            setTimeout(() => {
              try {
                process.kill(pid, 0); // Check if process exists
                process.kill(pid, 'SIGKILL'); // Force kill
                console.log(`💀 Force killed cursor-agent process ${pid}`);
              } catch {
                // Process already terminated
              }
            }, 5000);
          } catch (error) {
            console.error(`Error terminating cursor-agent ${pid}:`, error);
          }
        }
      }
      
      execution.process.kill('SIGTERM');
      this.executions.delete(executionId);
      return true;
    } catch (error) {
      console.error('Error stopping workflow:', error);
      return false;
    }
  }

  /**
   * Register cursor-agent PID for a workflow
   */
  registerCursorAgentPid(executionId: string, agentGuid: string, pid: number): void {
    const execution = this.executions.get(executionId);
    if (execution) {
      if (!execution.cursorAgentPids) {
        execution.cursorAgentPids = new Map();
      }
      execution.cursorAgentPids.set(agentGuid, pid);
      console.log(`📝 Registered cursor-agent PID ${pid} for workflow ${executionId}, agent ${agentGuid}`);
    }
  }

  /**
   * Get scrum master instance (for dashboard server access)
   */
  getScrumMaster(): ScrumMaster {
    return this.scrumMaster;
  }

  /**
   * Get all active executions
   */
  getActiveExecutions(): Array<{ id: string; workflowName: string; startTime: Date }> {
    return Array.from(this.executions.values()).map(exec => ({
      id: exec.id,
      workflowName: exec.workflowName,
      startTime: exec.startTime,
    }));
  }

  /**
   * Get execution by ID
   */
  getExecution(executionId: string): WorkflowExecution | undefined {
    return this.executions.get(executionId);
  }

  /**
   * Check if a workflow process is actually running
   */
  isProcessRunning(executionId: string): boolean {
    const execution = this.executions.get(executionId);
    if (!execution) {
      return false;
    }
    
    // Check if process is still alive
    try {
      // Send signal 0 to check if process exists (doesn't kill it)
      if (execution.process.pid) {
        process.kill(execution.process.pid, 0);
        return execution.process.exitCode === null;
      }
    } catch (error) {
      // Process doesn't exist
      return false;
    }
    
    return false;
  }

  /**
   * Set dashboard server reference for sending logs
   */
  setDashboardServer(dashboardServer: any): void {
    this.dashboardServer = dashboardServer;
  }

  /**
   * Send log message to dashboard
   */
  private sendLogToDashboard(workflowName: string, level: 'info' | 'warn' | 'error' | 'debug', message: string): void {
    if (this.dashboardServer && typeof this.dashboardServer.recordLog === 'function') {
      this.dashboardServer.recordLog(workflowName, level, message);
    }
  }

  /**
   * Get process status for an execution
   */
  getProcessStatus(executionId: string): { running: boolean; pid?: number; exitCode?: number | null; cursorAgentRunning?: boolean } {
    const execution = this.executions.get(executionId);
    if (!execution) {
      return { running: false, cursorAgentRunning: false };
    }
    
    let isRunning = false;
    const processPid = execution.process.pid;
    
    if (processPid !== undefined && processPid !== null) {
      try {
        // Send signal 0 to check if process exists (doesn't kill it)
        process.kill(processPid, 0);
        isRunning = execution.process.exitCode === null;
      } catch (error) {
        // Process doesn't exist
        isRunning = false;
      }
    }
    
    // Check if cursor-agent processes are running (child processes of the workflow)
    const cursorAgentRunning = this.checkCursorAgentProcesses(processPid);
    
    const result: { running: boolean; pid?: number; exitCode?: number | null; cursorAgentRunning?: boolean } = {
      running: isRunning,
      exitCode: execution.process.exitCode,
      cursorAgentRunning: cursorAgentRunning,
    };
    
    if (processPid !== undefined) {
      (result as any).pid = processPid;
    }
    
    return result;
  }

  /**
   * Check if cursor-agent processes are running (child processes of workflow)
   */
  private checkCursorAgentProcesses(parentPid?: number): boolean {
    if (!parentPid) {
      return false;
    }
    
    try {
      // Use platform-specific commands to find child processes
      const { execSync } = require('child_process');
      const platform = process.platform;
      
      if (platform === 'darwin' || platform === 'linux') {
        // Use pgrep or ps to find cursor-agent processes
        try {
          const result = execSync(`pgrep -P ${parentPid} | xargs ps -p 2>/dev/null | grep -i cursor-agent || true`, { encoding: 'utf-8', timeout: 1000 });
          return result.trim().length > 0;
        } catch {
          // Fallback: try ps
          try {
            const result = execSync(`ps -ef | grep ${parentPid} | grep -i cursor-agent | grep -v grep || true`, { encoding: 'utf-8', timeout: 1000 });
            return result.trim().length > 0;
          } catch {
            return false;
          }
        }
      } else if (platform === 'win32') {
        // Windows: use wmic or tasklist
        try {
          const result = execSync(`wmic process where "ParentProcessId=${parentPid}" get Name 2>nul | findstr /i cursor-agent || true`, { encoding: 'utf-8', timeout: 1000 });
          return result.trim().length > 0;
        } catch {
          return false;
        }
      }
    } catch (error) {
      // If we can't check, assume false
      return false;
    }
    
    return false;
  }

  /**
   * Start MCP server
   */
  async startMcpServer(port: number = 3001): Promise<boolean> {
    if (this.mcpServerProcess) {
      // Check if process is still alive
      try {
        if (this.mcpServerProcess.pid) {
          process.kill(this.mcpServerProcess.pid, 0);
          // Already running and alive - return true (not an error)
          console.log(`ℹ️  MCP Server is already running on port ${port} (PID: ${this.mcpServerProcess.pid})`);
          return true; // Return true since it's running
        }
      } catch {
        // Process is dead, clear it
        this.mcpServerProcess = null;
      }
    }

    const repoRoot = findRepoRoot();
    
    // Try multiple possible paths for the CLI script
    // When running from dist/, use dist/cli/index.js
    // When running from src/, use the compiled version
    const possiblePaths = [
      path.join(repoRoot, '.maestro', 'dist', 'cli', 'index.js'),
      path.join(repoRoot, 'dist', 'cli', 'index.js'),
      path.resolve(__dirname, '../../cli/index.js'), // Resolve relative to this file's location
    ];
    
    let scriptPath: string | null = null;
    for (const possiblePath of possiblePaths) {
      const normalizedPath = path.normalize(possiblePath);
      if (fs.existsSync(normalizedPath)) {
        scriptPath = normalizedPath;
        break;
      }
    }
    
    if (!scriptPath) {
      console.error(`❌ MCP Server: Could not find CLI script.`);
      console.error(`   Tried paths:`);
      possiblePaths.forEach(p => console.error(`     - ${path.normalize(p)}`));
      console.error(`   Repo root: ${repoRoot}`);
      console.error(`   Current working directory: ${process.cwd()}`);
      return false;
    }
    
    try {
      // Use absolute path to ensure it works from any directory
      const absoluteScriptPath = path.resolve(scriptPath);
      
      console.log(`🔌 Starting MCP Server on port ${port}...`);
      console.log(`   Using CLI script: ${absoluteScriptPath}`);
      
      // Use getNodeArgs to include heap memory configuration
      const nodeArgs = getNodeArgs([absoluteScriptPath, 'mcp', '--port', String(port)]);
      
      this.mcpServerProcess = spawn('node', nodeArgs, {
        cwd: repoRoot,
        stdio: ['ignore', 'pipe', 'pipe'], // Pipe stdout/stderr so we can capture errors
        env: { ...process.env },
        detached: true, // Allow it to run independently
      });

      // Unref so parent process doesn't wait for it, but keep reference to monitor
      if (this.mcpServerProcess.pid) {
        this.mcpServerProcess.unref();
      }

      this.mcpServerPort = port;

      let startupError: string | null = null;

      this.mcpServerProcess.on('exit', (code) => {
        if (code !== 0 && code !== null) {
          console.error(`⚠️  MCP Server exited with code ${code}`);
          startupError = `MCP Server exited with code ${code}`;
        }
        this.mcpServerProcess = null;
      });

      // Handle errors
      this.mcpServerProcess.on('error', (error) => {
        console.error(`❌ Failed to start MCP Server: ${error.message}`);
        startupError = error.message;
        this.mcpServerProcess = null;
      });

      // Collect all output for debugging
      let stdoutBuffer = '';
      let stderrBuffer = '';

      // Log stdout for debugging
      this.mcpServerProcess.stdout?.on('data', (data) => {
        const message = data.toString();
        stdoutBuffer += message;
        const trimmed = message.trim();
        if (trimmed) {
          console.log(`   MCP Server stdout: ${trimmed}`);
        }
      });

      // Log stderr for debugging
      this.mcpServerProcess.stderr?.on('data', (data) => {
        const message = data.toString();
        stderrBuffer += message;
        const trimmed = message.trim();
        if (trimmed) {
          console.error(`   MCP Server stderr: ${trimmed}`);
          if (trimmed.includes('EADDRINUSE')) {
            startupError = `Port ${port} is already in use`;
          } else if (trimmed.includes('Error:') || trimmed.includes('error') || trimmed.includes('failed')) {
            if (!startupError) {
              startupError = trimmed;
            }
          }
        }
      });

      // Wait a moment to see if it starts successfully
      // Check multiple times to catch early exits
      for (let i = 0; i < 6; i++) {
        await new Promise(resolve => setTimeout(resolve, 500));
        
        // Check if process died
        if (!this.mcpServerProcess || !this.mcpServerProcess.pid) {
          const errorMsg = startupError || stderrBuffer || stdoutBuffer || 'MCP Server process exited unexpectedly';
          console.error(`❌ MCP Server startup failed after ${(i + 1) * 500}ms. Error: ${errorMsg}`);
          throw new Error(errorMsg);
        }
        
        // Check if process is still alive
        try {
          process.kill(this.mcpServerProcess.pid, 0);
        } catch {
          const errorMsg = startupError || stderrBuffer || 'MCP Server process died';
          console.error(`❌ MCP Server process died after ${(i + 1) * 500}ms. Error: ${errorMsg}`);
          throw new Error(errorMsg);
        }
      }
      
      // Verify port is actually listening
      const http = require('http');
      let portListening = false;
      for (let attempt = 0; attempt < 5; attempt++) {
        await new Promise(resolve => setTimeout(resolve, 500));
        
        try {
          await new Promise<void>((resolve, reject) => {
            const testReq = http.request(
              { host: 'localhost', port, method: 'HEAD', timeout: 1000 },
              () => {
                portListening = true;
                resolve();
              }
            );
            
            testReq.on('error', () => {
              reject(new Error('Port not listening'));
            });
            
            testReq.on('timeout', () => {
              testReq.destroy();
              reject(new Error('Timeout'));
            });
            
            testReq.end();
          });
          
          // Port is listening, success!
          break;
        } catch {
          // Not ready yet, continue waiting
          if (attempt === 4) {
            // Last attempt failed
            const errorMsg = startupError || stderrBuffer || 'MCP Server process is running but port is not listening';
            console.error(`❌ MCP Server port ${port} is not listening after 3 seconds`);
            console.error(`   Process PID: ${this.mcpServerProcess.pid}`);
            console.error(`   Stderr: ${stderrBuffer || 'none'}`);
            console.error(`   Stdout: ${stdoutBuffer || 'none'}`);
            throw new Error(errorMsg);
          }
        }
      }

      console.log(`✅ MCP Server started successfully on port ${port} (PID: ${this.mcpServerProcess.pid})`);
      return true;
    } catch (error: any) {
      console.error(`❌ Failed to spawn MCP Server: ${error.message}`);
      this.mcpServerProcess = null;
      return false;
    }
  }

  /**
   * Stop MCP server
   */
  async stopMcpServer(): Promise<boolean> {
    if (!this.mcpServerProcess) {
      return false;
    }

    try {
      this.mcpServerProcess.kill('SIGTERM');
      this.mcpServerProcess = null;
      return true;
    } catch (error) {
      console.error('Error stopping MCP server:', error);
      return false;
    }
  }

  /**
   * Get MCP server status
   */
  getMcpServerStatus(): { running: boolean; port?: number } {
    return {
      running: this.mcpServerProcess !== null,
      port: this.mcpServerProcess ? this.mcpServerPort : undefined,
    };
  }
}
