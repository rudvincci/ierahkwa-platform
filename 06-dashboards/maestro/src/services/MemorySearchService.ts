/**
 * Memory Search Service
 * 
 * Integrates with .skmemory and .composermemory (if available) to search
 * historical patterns, workflows, and best practices.
 */

import { exec } from 'child_process';
import { promisify } from 'util';
import * as fs from 'fs/promises';
import * as path from 'path';
import * as glob from 'glob';

const execAsync = promisify(exec);

export interface MemorySearchResult {
  source: 'skmemory' | 'composermemory' | 'orchestrator';
  content: string;
  path: string;
  relevance: number;
  metadata?: {
    tags?: string[];
    type?: string;
    date?: string;
  };
}

export interface MemorySearchOptions {
  query: string;
  topK?: number;
  tags?: string[];
  type?: 'workflow' | 'pattern' | 'best-practice' | 'all';
  since?: string; // Date string
}

export class MemorySearchService {
  private projectRoot: string;
  private skmemoryPath: string;
  private composermemoryPath: string;
  private orchestratorPath: string;
  private enabled: boolean;
  private systemsEnabled: {
    skmemory: boolean;
    composermemory: boolean;
    orchestrator: boolean;
  };

  constructor(
    projectRoot: string = process.cwd(),
    enabled: boolean = true,
    systemsEnabled?: {
      skmemory?: boolean;
      composermemory?: boolean;
      orchestrator?: boolean;
    },
    customPaths?: {
      skmemory?: string;
      composermemory?: string;
      orchestrator?: string;
    }
  ) {
    this.projectRoot = projectRoot;
    this.enabled = enabled;
    
    // Use custom paths if provided, otherwise use defaults
    this.skmemoryPath = customPaths?.skmemory 
      ? path.join(projectRoot, customPaths.skmemory)
      : path.join(projectRoot, '.skmemory');
    this.composermemoryPath = customPaths?.composermemory
      ? path.join(projectRoot, customPaths.composermemory)
      : path.join(projectRoot, '.composermemory');
    this.orchestratorPath = customPaths?.orchestrator
      ? path.join(projectRoot, customPaths.orchestrator)
      : path.join(projectRoot, '.maestro');
    
    // Default all systems enabled if not specified
    this.systemsEnabled = {
      skmemory: systemsEnabled?.skmemory ?? true,
      composermemory: systemsEnabled?.composermemory ?? true,
      orchestrator: systemsEnabled?.orchestrator ?? true,
    };
  }

  /**
   * Check if memory systems are available (as GitHub submodules)
   */
  async checkAvailability(): Promise<{
    skmemory: boolean;
    composermemory: boolean;
    orchestrator: boolean;
    submodulesInitialized: boolean;
    enabled: boolean;
  }> {
    if (!this.enabled) {
      return {
        skmemory: false,
        composermemory: false,
        orchestrator: false,
        submodulesInitialized: false,
        enabled: false,
      };
    }

    const [skmemory, composermemory, orchestrator, submodulesInitialized] = await Promise.all([
      this.systemsEnabled.skmemory ? this.checkSkmemory() : Promise.resolve(false),
      this.systemsEnabled.composermemory ? this.checkComposermemory() : Promise.resolve(false),
      this.systemsEnabled.orchestrator ? this.checkOrchestrator() : Promise.resolve(false),
      this.checkSubmodulesInitialized(),
    ]);

    return { 
      skmemory, 
      composermemory, 
      orchestrator, 
      submodulesInitialized,
      enabled: this.enabled,
    };
  }

  /**
   * Check if submodules are initialized
   */
  private async checkSubmodulesInitialized(): Promise<boolean> {
    try {
      const { stdout } = await execAsync('git submodule status', {
        cwd: this.projectRoot,
      });
      
      // Check if .skmemory or .composermemory are listed and initialized
      const lines = stdout.split('\n');
      const hasSkmemory = lines.some(line => line.includes('.skmemory'));
      const hasComposermemory = lines.some(line => line.includes('.composermemory'));
      
      // Submodules are initialized if they don't start with '-' (uninitialized) or 'U' (unmerged)
      const skmemoryInitialized = hasSkmemory && !lines.some(line => 
        line.includes('.skmemory') && (line.startsWith('-') || line.startsWith('U'))
      );
      const composermemoryInitialized = hasComposermemory && !lines.some(line => 
        line.includes('.composermemory') && (line.startsWith('-') || line.startsWith('U'))
      );
      
      return skmemoryInitialized || composermemoryInitialized;
    } catch {
      // Not a git repo or git not available
      return false;
    }
  }

  /**
   * Initialize submodules if needed
   */
  async initializeSubmodules(): Promise<void> {
    try {
      const availability = await this.checkAvailability();
      
      if (!availability.submodulesInitialized) {
        console.log('Initializing submodules...');
        await execAsync('git submodule update --init --recursive', {
          cwd: this.projectRoot,
        });
        console.log('Submodules initialized');
      }
    } catch (error) {
      console.warn(`Failed to initialize submodules: ${error}`);
      throw new Error(`Submodule initialization failed: ${error}`);
    }
  }

  /**
   * Search all available memory systems
   */
  async search(options: MemorySearchOptions): Promise<MemorySearchResult[]> {
    // Return empty if memory is disabled
    if (!this.enabled) {
      return [];
    }
    const results: MemorySearchResult[] = [];
    const availability = await this.checkAvailability();

    // Search SKMemory
    if (availability.skmemory) {
      const skmemoryResults = await this.searchSkmemory(options);
      results.push(...skmemoryResults);
    }

    // Search ComposerMemory
    if (availability.composermemory) {
      const composerResults = await this.searchComposermemory(options);
      results.push(...composerResults);
    }

    // Search Orchestrator logs/history
    if (availability.orchestrator) {
      const orchestratorResults = await this.searchOrchestrator(options);
      results.push(...orchestratorResults);
    }

    // Sort by relevance and return top K
    results.sort((a, b) => b.relevance - a.relevance);
    return results.slice(0, options.topK || 10);
  }

  /**
   * Search SKMemory
   */
  private async searchSkmemory(options: MemorySearchOptions): Promise<MemorySearchResult[]> {
    const results: MemorySearchResult[] = [];

    try {
      const searchScript = path.join(this.skmemoryPath, 'v1/scripts/search-memory.sh');
      
      // Check if search script exists
      try {
        await fs.access(searchScript);
      } catch {
        // Fallback to git grep if script doesn't exist
        return this.searchSkmemoryWithGitGrep(options);
      }

      // Build search command
      let command = `bash "${searchScript}" --query "${options.query}"`;
      if (options.topK) {
        command += ` --top-k ${options.topK}`;
      }
      if (options.tags && options.tags.length > 0) {
        command += ` --tags "${options.tags.join(',')}"`;
      }
      if (options.type) {
        command += ` --type ${options.type}`;
      }
      if (options.since) {
        command += ` --since "${options.since}"`;
      }

      const { stdout } = await execAsync(command, {
        cwd: this.projectRoot,
      });

      // Parse results (simplified - adjust based on actual output format)
      const lines = stdout.split('\n').filter(line => line.trim());
      lines.forEach((line, index) => {
        results.push({
          source: 'skmemory',
          content: line,
          path: `skmemory:${index}`,
          relevance: 1.0 - (index * 0.1), // Decreasing relevance
        });
      });
    } catch (error) {
      console.warn(`Failed to search SKMemory: ${error}`);
    }

    return results;
  }

  /**
   * Fallback: Search SKMemory using git grep
   */
  private async searchSkmemoryWithGitGrep(options: MemorySearchOptions): Promise<MemorySearchResult[]> {
    const results: MemorySearchResult[] = [];

    try {
      const memoryPath = path.join(this.skmemoryPath, 'v1/memory/public');
      
      // Use git grep if available, otherwise use grep
      const command = `git -C "${this.skmemoryPath}" grep -i "${options.query}" -- "*.md" "*.txt" 2>/dev/null || grep -r -i "${options.query}" "${memoryPath}" --include="*.md" --include="*.txt" 2>/dev/null | head -${options.topK || 10}`;
      
      const { stdout } = await execAsync(command, {
        cwd: this.projectRoot,
      });

      const lines = stdout.split('\n').filter(line => line.trim());
      lines.forEach((line, index) => {
        const match = line.match(/^(.+?):(.+)$/);
        if (match) {
          results.push({
            source: 'skmemory',
            content: match[2],
            path: match[1],
            relevance: 1.0 - (index * 0.1),
          });
        }
      });
    } catch (error) {
      console.warn(`Failed to search SKMemory with git grep: ${error}`);
    }

    return results;
  }

  /**
   * Search ComposerMemory
   */
  private async searchComposermemory(options: MemorySearchOptions): Promise<MemorySearchResult[]> {
    const results: MemorySearchResult[] = [];

    try {
      // Check for common ComposerMemory structures
      const possiblePaths = [
        path.join(this.composermemoryPath, 'memory'),
        path.join(this.composermemoryPath, 'v1/memory'),
        path.join(this.composermemoryPath, 'public'),
      ];

      for (const memoryPath of possiblePaths) {
        try {
          await fs.access(memoryPath);
          
          // Search using grep
          const command = `grep -r -i "${options.query}" "${memoryPath}" --include="*.md" --include="*.txt" --include="*.json" 2>/dev/null | head -${options.topK || 10}`;
          
          const { stdout } = await execAsync(command, {
            cwd: this.projectRoot,
          });

          const lines = stdout.split('\n').filter(line => line.trim());
          lines.forEach((line, index) => {
            const match = line.match(/^(.+?):(.+)$/);
            if (match) {
              results.push({
                source: 'composermemory',
                content: match[2],
                path: match[1],
                relevance: 1.0 - (index * 0.1),
              });
            }
          });

          // Found valid path, break
          break;
        } catch {
          // Path doesn't exist, try next
          continue;
        }
      }
    } catch (error) {
      console.warn(`Failed to search ComposerMemory: ${error}`);
    }

    return results;
  }

  /**
   * Search Orchestrator execution logs and history
   */
  private async searchOrchestrator(options: MemorySearchOptions): Promise<MemorySearchResult[]> {
    const results: MemorySearchResult[] = [];

    try {
      const logsPath = path.join(this.orchestratorPath, 'logs');
      const configPath = path.join(this.orchestratorPath, 'config');

      // Search logs
      try {
        await fs.access(logsPath);
        const logFiles = await glob.glob(`${logsPath}/*.json`);
        
        for (const logFile of logFiles.slice(0, 10)) {
          try {
            const content = await fs.readFile(logFile, 'utf-8');
            if (content.toLowerCase().includes(options.query.toLowerCase())) {
              results.push({
                source: 'orchestrator',
                content: content.substring(0, 500), // First 500 chars
                path: logFile,
                relevance: 0.8,
                metadata: {
                  type: 'execution-log',
                },
              });
            }
          } catch {
            // Skip file if can't read
          }
        }
      } catch {
        // Logs directory doesn't exist
      }

      // Search workflow configs
      try {
        await fs.access(configPath);
        const configFiles = await glob.glob(`${configPath}/*.yml`);
        
        for (const configFile of configFiles) {
          try {
            const content = await fs.readFile(configFile, 'utf-8');
            if (content.toLowerCase().includes(options.query.toLowerCase())) {
              results.push({
                source: 'orchestrator',
                content: content.substring(0, 500),
                path: configFile,
                relevance: 0.9,
                metadata: {
                  type: 'workflow-config',
                },
              });
            }
          } catch {
            // Skip file if can't read
          }
        }
      } catch {
        // Config directory doesn't exist
      }
    } catch (error) {
      console.warn(`Failed to search Orchestrator: ${error}`);
    }

    return results;
  }

  /**
   * Get workflow patterns from memory
   */
  async getWorkflowPatterns(): Promise<MemorySearchResult[]> {
    return this.search({
      query: 'workflow',
      type: 'workflow',
      topK: 20,
    });
  }

  /**
   * Get best practices from memory
   */
  async getBestPractices(context?: string): Promise<MemorySearchResult[]> {
    const query = context ? `best practice ${context}` : 'best practice';
    return this.search({
      query,
      type: 'best-practice',
      topK: 15,
    });
  }

  /**
   * Get similar workflows from history
   */
  async getSimilarWorkflows(workflowName: string): Promise<MemorySearchResult[]> {
    return this.search({
      query: workflowName,
      type: 'workflow',
      topK: 10,
    });
  }

  // Helper methods

  private async checkSkmemory(): Promise<boolean> {
    try {
      // Check if .skmemory exists (even if submodule not initialized)
      await fs.access(this.skmemoryPath);
      
      // Check if it's actually initialized (has content)
      try {
        const scriptsPath = path.join(this.skmemoryPath, 'v1/scripts/search-memory.sh');
        await fs.access(scriptsPath);
        return true;
      } catch {
        // Directory exists but might be empty submodule
        // Check if it's a git submodule
        try {
          const gitPath = path.join(this.skmemoryPath, '.git');
          await fs.access(gitPath);
          // It's a submodule, check if initialized
          const files = await fs.readdir(this.skmemoryPath);
          return files.length > 0; // Has content = initialized
        } catch {
          return false; // Not a submodule or not initialized
        }
      }
    } catch {
      return false;
    }
  }

  private async checkComposermemory(): Promise<boolean> {
    try {
      await fs.access(this.composermemoryPath);
      
      // Check if it's initialized (has content)
      try {
        const files = await fs.readdir(this.composermemoryPath);
        return files.length > 0; // Has content = initialized
      } catch {
        return false;
      }
    } catch {
      return false;
    }
  }

  private async checkOrchestrator(): Promise<boolean> {
    try {
      await fs.access(this.orchestratorPath);
      // Orchestrator is always available (it's the current tool)
      return true;
    } catch {
      return false;
    }
  }
}
