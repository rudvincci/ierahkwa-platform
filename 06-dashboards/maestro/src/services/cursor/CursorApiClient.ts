/**
 * Cursor API Client
 * Client for interacting with Cursor API
 */

import * as fs from 'fs';
import * as path from 'path';
import * as yaml from 'js-yaml';

export interface CursorApiConfig {
  api_key?: string;
  base_url?: string;
  timeout?: number;
  polling_interval?: number;
}

export interface CursorProjectInfo {
  id: string;
  name: string;
  path: string;
  files?: string[];
  structure?: Record<string, any>;
  metadata?: Record<string, any>;
}

export interface CursorFileChange {
  path: string;
  type: 'created' | 'modified' | 'deleted';
  timestamp: Date;
  diff?: string;
}

export interface CursorAgentInfo {
  id: string;
  name: string;
  status: 'active' | 'idle' | 'stopped';
  created_at: Date;
  last_activity?: Date;
  history?: Array<{
    timestamp: Date;
    action: string;
    details?: Record<string, any>;
  }>;
}

/**
 * Cursor API Client
 */
export class CursorApiClient {
  private config: CursorApiConfig;
  private configPath: string;
  private apiKey?: string;
  private baseUrl: string;

  constructor(root: string = '.maestro', config?: CursorApiConfig) {
    this.configPath = path.join(root, 'config', 'cursor-api.yml');
    this.config = this.loadConfig(config);
    // Check environment variable first, then config file
    this.apiKey = process.env.CURSOR_API_KEY || this.config.api_key;
    this.baseUrl = this.config.base_url || 'https://api.cursor.com/v1';
    
    // Log if API key is found (for debugging)
    if (this.apiKey) {
      console.log('✅ Cursor API key loaded from', process.env.CURSOR_API_KEY ? 'environment variable' : 'config file');
    } else {
      console.warn('⚠️  Cursor API key not found. Set CURSOR_API_KEY environment variable or configure in cursor-api.yml');
    }
  }

  private loadConfig(override?: CursorApiConfig): CursorApiConfig {
    const defaultConfig: CursorApiConfig = {
      timeout: 30000,
      polling_interval: 5000,
    };

    if (fs.existsSync(this.configPath)) {
      try {
        const data = fs.readFileSync(this.configPath, 'utf-8');
        const fileConfig = yaml.load(data) as CursorApiConfig;
        return { ...defaultConfig, ...fileConfig, ...override };
      } catch (error) {
        // Return default config
      }
    }

    return { ...defaultConfig, ...override };
  }

  /**
   * Set API key
   */
  setApiKey(apiKey: string): void {
    this.apiKey = apiKey;
    this.config.api_key = apiKey;
    this.saveConfig();
  }

  /**
   * Get API key (masked)
   */
  getApiKey(): string | undefined {
    return this.apiKey ? `${this.apiKey.substring(0, 4)}...${this.apiKey.substring(this.apiKey.length - 4)}` : undefined;
  }

  /**
   * Check if API key is configured
   */
  isConfigured(): boolean {
    return !!this.apiKey;
  }

  private saveConfig(): void {
    try {
      const configDir = path.dirname(this.configPath);
      if (!fs.existsSync(configDir)) {
        fs.mkdirSync(configDir, { recursive: true });
      }

      // Don't save API key to file for security - use environment variable
      const configToSave = { ...this.config };
      delete (configToSave as any).api_key;

      fs.writeFileSync(this.configPath, yaml.dump(configToSave), 'utf-8');
    } catch (error) {
      // Ignore errors
    }
  }

  /**
   * Make API request
   */
  private async request(endpoint: string, options: RequestInit = {}): Promise<any> {
    if (!this.apiKey) {
      throw new Error('Cursor API key not configured');
    }

    const url = `${this.baseUrl}${endpoint}`;
    const headers = {
      'Authorization': `Bearer ${this.apiKey}`,
      'Content-Type': 'application/json',
      ...options.headers,
    };

    try {
      const response = await fetch(url, {
        ...options,
        headers,
        signal: AbortSignal.timeout(this.config.timeout || 30000),
      });

      if (!response.ok) {
        throw new Error(`Cursor API error: ${response.status} ${response.statusText}`);
      }

      return await response.json();
    } catch (error: any) {
      if (error.name === 'AbortError') {
        throw new Error('Cursor API request timeout');
      }
      throw error;
    }
  }

  /**
   * Get project information
   */
  async getProject(): Promise<CursorProjectInfo | null> {
    try {
      // Note: Cursor API endpoints may vary - this is a placeholder structure
      // Actual implementation would depend on Cursor's API documentation
      const data = await this.request('/project');
      return data as CursorProjectInfo;
    } catch (error) {
      // If API not available, try to get info from local project
      return this.getLocalProjectInfo();
    }
  }

  /**
   * Get local project info (fallback)
   */
  private getLocalProjectInfo(): CursorProjectInfo | null {
    try {
      const repoRoot = process.cwd();
      const projectName = path.basename(repoRoot);
      
      // Get file structure
      const files: string[] = [];
      this.collectFiles(repoRoot, files, 100); // Limit to 100 files

      return {
        id: projectName,
        name: projectName,
        path: repoRoot,
        files: files.slice(0, 100),
      };
    } catch (error) {
      return null;
    }
  }

  private collectFiles(dir: string, files: string[], limit: number): void {
    if (files.length >= limit) return;

    try {
      const entries = fs.readdirSync(dir, { withFileTypes: true });
      for (const entry of entries) {
        if (files.length >= limit) break;

        // Skip hidden files and common ignore patterns
        if (entry.name.startsWith('.') || 
            entry.name === 'node_modules' || 
            entry.name === 'dist' ||
            entry.name === 'build') {
          continue;
        }

        const fullPath = path.join(dir, entry.name);
        if (entry.isDirectory()) {
          this.collectFiles(fullPath, files, limit);
        } else if (entry.isFile()) {
          files.push(path.relative(process.cwd(), fullPath));
        }
      }
    } catch (error) {
      // Ignore errors
    }
  }

  /**
   * Get file changes (using git as fallback)
   */
  async getFileChanges(limit: number = 50): Promise<CursorFileChange[]> {
    try {
      // Try API first
      const data = await this.request(`/files/changes?limit=${limit}`);
      return (data.changes || []).map((c: any) => ({
        ...c,
        timestamp: new Date(c.timestamp),
      }));
    } catch (error) {
      // Fallback to git
      return this.getGitFileChanges(limit);
    }
  }

  /**
   * Get file changes from git (fallback)
   */
  private getGitFileChanges(limit: number): CursorFileChange[] {
    try {
      const { execSync } = require('child_process');
      const output = execSync('git log --name-status --pretty=format:"%H|%ct" -n 20', {
        encoding: 'utf-8',
        maxBuffer: 10 * 1024 * 1024,
      });

      const changes: CursorFileChange[] = [];
      const lines = output.split('\n');
      let currentTimestamp = new Date();

      for (const line of lines) {
        if (line.includes('|')) {
          const [, timestamp] = line.split('|');
          currentTimestamp = new Date(parseInt(timestamp, 10) * 1000);
        } else if (line.match(/^[AMD]\s+/)) {
          const match = line.match(/^([AMD])\s+(.+)$/);
          if (match) {
            const [, status, filePath] = match;
            changes.push({
              path: filePath,
              type: status === 'A' ? 'created' : status === 'M' ? 'modified' : 'deleted',
              timestamp: currentTimestamp,
            });

            if (changes.length >= limit) break;
          }
        }
      }

      return changes;
    } catch (error) {
      return [];
    }
  }

  /**
   * Get agent history
   */
  async getAgents(): Promise<CursorAgentInfo[]> {
    try {
      const data = await this.request('/agents');
      return (data.agents || []).map((a: any) => ({
        ...a,
        created_at: new Date(a.created_at),
        last_activity: a.last_activity ? new Date(a.last_activity) : undefined,
        history: (a.history || []).map((h: any) => ({
          ...h,
          timestamp: new Date(h.timestamp),
        })),
      }));
    } catch (error) {
      // Fallback: try to get from cursor-agent CLI
      return this.getAgentsFromCli();
    }
  }

  /**
   * Get agents from cursor-agent CLI (fallback)
   */
  private async getAgentsFromCli(): Promise<CursorAgentInfo[]> {
    try {
      const { execSync } = require('child_process');
      const output = execSync('cursor-agent ls', {
        encoding: 'utf-8',
        maxBuffer: 10 * 1024 * 1024,
      });

      // Parse cursor-agent ls output
      const agents: CursorAgentInfo[] = [];
      const lines = output.split('\n');

      for (const line of lines) {
        if (line.trim() && !line.startsWith('ID')) {
          const parts = line.split(/\s+/);
          if (parts.length >= 2) {
            agents.push({
              id: parts[0],
              name: parts[1] || parts[0],
              status: 'active',
              created_at: new Date(),
            });
          }
        }
      }

      return agents;
    } catch (error) {
      return [];
    }
  }

  /**
   * Delete agent
   */
  async deleteAgent(agentId: string): Promise<void> {
    if (!this.apiKey) {
      throw new Error('Cursor API key not configured');
    }

    const response = await fetch(`${this.baseUrl}/agents/${agentId}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${this.apiKey}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      try {
        const error = await response.json() as { error?: string };
        throw new Error(error.error || `Failed to delete agent: ${response.statusText}`);
      } catch (e: unknown) {
        const message = e instanceof Error ? e.message : 'Unknown error';
        throw new Error(`Failed to delete agent: ${response.statusText} (${message})`);
      }
    }
  }

  /**
   * Health check
   */
  async healthCheck(): Promise<{ status: string; message?: string }> {
    try {
      const data = await this.request('/health');
      return { status: 'ok', ...data };
    } catch (error: any) {
      return {
        status: 'error',
        message: error.message || 'Cursor API not available',
      };
    }
  }
}

