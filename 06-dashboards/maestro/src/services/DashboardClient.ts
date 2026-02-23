/**
 * Dashboard Client
 * 
 * HTTP client for communicating with dashboard server from external processes.
 */

import * as http from 'http';

export interface DashboardClientOptions {
  port?: number;
  host?: string;
}

export class DashboardClient {
  private port: number;
  private host: string;
  private baseUrl: string;
  private isAvailableCache: boolean | null = null;
  private lastAvailabilityCheck: number = 0;
  private availabilityCheckInterval: number = 30000; // Check every 30 seconds

  constructor(options: DashboardClientOptions = {}) {
    this.port = options.port || 3000;
    this.host = options.host || 'localhost';
    this.baseUrl = `http://${this.host}:${this.port}`;
  }

  /**
   * Check if dashboard is running
   */
  async isAvailable(): Promise<boolean> {
    const now = Date.now();
    
    // Use cached result if recent (within 30 seconds)
    if (this.isAvailableCache !== null && (now - this.lastAvailabilityCheck) < this.availabilityCheckInterval) {
      return this.isAvailableCache;
    }

    try {
      const response = await this.request('GET', '/api/metrics', undefined, 2000); // 2 second timeout
      const available = response.statusCode === 200;
      this.isAvailableCache = available;
      this.lastAvailabilityCheck = now;
      return available;
    } catch {
      this.isAvailableCache = false;
      this.lastAvailabilityCheck = now;
      return false;
    }
  }

  /**
   * Record workflow start
   */
  async recordWorkflowStart(workflowName: string, totalSteps: number): Promise<void> {
    // Check availability first
    const available = await this.isAvailable();
    if (!available) {
      console.warn(`⚠️  Dashboard not available - skipping workflow start for ${workflowName}`);
      return;
    }

    try {
      const response = await this.request('POST', `/api/workflows/${encodeURIComponent(workflowName)}/start`, {
        totalSteps,
      }, 2000); // 2 second timeout
      if (response.statusCode === 200) {
        console.log(`📊 Dashboard: Workflow start registered - ${workflowName} (${totalSteps} steps)`);
      } else {
        console.warn(`⚠️  Dashboard: Workflow start failed with status ${response.statusCode} - ${workflowName}`);
      }
    } catch (error: any) {
      // If connection refused, mark as unavailable
      if (error?.code === 'ECONNREFUSED' || error?.code === 'ETIMEDOUT') {
        this.isAvailableCache = false;
        this.lastAvailabilityCheck = Date.now();
        console.warn(`⚠️  Dashboard connection failed - ${error.code} - ${workflowName}`);
      } else {
        console.warn(`⚠️  Dashboard: Workflow start error - ${error.message} - ${workflowName}`);
      }
    }
  }

  /**
   * Record step update
   */
  async recordStepUpdate(workflowName: string, stepName: string, status: 'running' | 'completed' | 'failed'): Promise<void> {
    // Check availability first (using cache if recent)
    const available = await this.isAvailable();
    if (!available) {
      // Log warning if dashboard was expected to be available
      console.warn(`⚠️  Dashboard not available - skipping step update for ${workflowName}/${stepName}`);
      return;
    }

    try {
      const response = await this.request('POST', `/api/workflows/${encodeURIComponent(workflowName)}/step`, {
        stepName,
        status,
      }, 2000); // 2 second timeout
      if (response.statusCode === 200) {
        // Log success for debugging
        console.log(`📊 Dashboard: Step update sent - ${workflowName} / ${stepName} / ${status}`);
      } else {
        console.warn(`⚠️  Dashboard: Step update failed with status ${response.statusCode} - ${workflowName} / ${stepName} / ${status}`);
      }
    } catch (error: any) {
      // If connection refused, mark as unavailable and log
      if (error?.code === 'ECONNREFUSED' || error?.code === 'ETIMEDOUT') {
        this.isAvailableCache = false;
        this.lastAvailabilityCheck = Date.now();
        console.warn(`⚠️  Dashboard connection failed - ${error.code} - ${workflowName} / ${stepName} / ${status}`);
        return;
      }
      // For other errors, log them
      console.warn(`⚠️  Dashboard: Step update error - ${error.message} - ${workflowName} / ${stepName} / ${status}`);
    }
  }

  /**
   * Record workflow end
   */
  async recordWorkflowEnd(
    workflowName: string,
    success: boolean,
    completedTasks: number,
    failedTasks: number,
    duration: number
  ): Promise<void> {
    // Check availability first
    const available = await this.isAvailable();
    if (!available) {
      return;
    }

    try {
      await this.request('POST', `/api/workflows/${encodeURIComponent(workflowName)}/end`, {
        success,
        completedTasks,
        failedTasks,
        duration,
      }, 2000); // 2 second timeout
    } catch (error: any) {
      // If connection refused, mark as unavailable
      if (error?.code === 'ECONNREFUSED' || error?.code === 'ETIMEDOUT') {
        this.isAvailableCache = false;
        this.lastAvailabilityCheck = Date.now();
      }
      // Silently fail - dashboard might not be running
    }
  }

  /**
   * Make HTTP request
   */
  private request(method: string, path: string, body?: any, timeout?: number): Promise<{ statusCode?: number; data?: any }> {
    const requestTimeout = timeout || 5000;
    return new Promise((resolve, reject) => {
      const url = `${this.baseUrl}${path}`;
      const urlObj = new URL(url);
      
      const options = {
        hostname: urlObj.hostname,
        port: urlObj.port,
        path: urlObj.pathname,
        method,
        headers: {
          'Content-Type': 'application/json',
        },
        timeout: requestTimeout,
      };

      const req = http.request(options, (res) => {
        let data = '';
        res.on('data', (chunk) => {
          data += chunk;
        });
        res.on('end', () => {
          try {
            const jsonData = data ? JSON.parse(data) : {};
            resolve({ statusCode: res.statusCode, data: jsonData });
          } catch {
            resolve({ statusCode: res.statusCode, data });
          }
        });
      });

      req.on('error', (error) => {
        reject(error);
      });

      req.on('timeout', () => {
        req.destroy();
        const timeoutError: any = new Error('Request timeout');
        timeoutError.code = 'ETIMEDOUT';
        reject(timeoutError);
      });

      if (body) {
        req.write(JSON.stringify(body));
      }

      req.end();
    });
  }
}
