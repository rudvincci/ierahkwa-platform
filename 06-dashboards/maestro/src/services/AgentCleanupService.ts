/**
 * Agent Cleanup Service
 * 
 * Manages periodic cleanup of cursor-agent created agents using `cursor-agent ls` and `cursor-agent rm`
 */

import { exec } from 'child_process';
import { promisify } from 'util';
import * as path from 'path';
import { findRepoRoot } from '../cli/utils';
import { OrchestratorConfig } from '../config/OrchestratorConfig';

const execAsync = promisify(exec);

export interface AgentInfo {
  id: string;
  name?: string;
  createdAt?: Date;
  lastUsed?: Date;
  age?: number; // in milliseconds
}

export interface AgentCleanupConfig {
  enabled: boolean;
  interval: {
    value: number;
    unit: 'hours' | 'days' | 'months';
  };
  minAge: {
    value: number;
    unit: 'hours' | 'days' | 'months';
  };
  dryRun?: boolean; // If true, only log what would be deleted
}

export class AgentCleanupService {
  private cleanupInterval?: NodeJS.Timeout;
  private config: AgentCleanupConfig;
  private isRunning: boolean = false;

  constructor(config?: AgentCleanupConfig) {
    this.config = config || {
      enabled: true,
      interval: { value: 24, unit: 'hours' },
      minAge: { value: 7, unit: 'days' },
      dryRun: false,
    };
  }

  /**
   * Update configuration
   */
  updateConfig(config: Partial<AgentCleanupConfig>): void {
    this.config = { ...this.config, ...config };
    
    // Restart cleanup if enabled
    if (this.config.enabled) {
      this.stop();
      this.start();
    } else {
      this.stop();
    }
  }

  /**
   * Start periodic cleanup
   */
  start(): void {
    if (!this.config.enabled || this.isRunning) {
      return;
    }

    this.isRunning = true;
    const intervalMs = this.convertToMilliseconds(this.config.interval.value, this.config.interval.unit);
    
    console.log(`🧹 Agent Cleanup Service: Starting with interval ${this.config.interval.value} ${this.config.interval.unit}`);
    
    // Run immediately on start
    this.cleanup().catch(err => {
      console.error('🧹 Agent Cleanup Service: Error in initial cleanup:', err);
    });

    // Then run periodically
    this.cleanupInterval = setInterval(() => {
      this.cleanup().catch(err => {
        console.error('🧹 Agent Cleanup Service: Error in periodic cleanup:', err);
      });
    }, intervalMs);
  }

  /**
   * Stop periodic cleanup
   */
  stop(): void {
    if (this.cleanupInterval) {
      clearInterval(this.cleanupInterval);
      this.cleanupInterval = undefined;
    }
    this.isRunning = false;
    console.log('🧹 Agent Cleanup Service: Stopped');
  }

  /**
   * List all agents using cursor-agent ls
   */
  async listAgents(): Promise<AgentInfo[]> {
    try {
      const { stdout, stderr } = await execAsync('cursor-agent ls', {
        timeout: 10000,
        maxBuffer: 1024 * 1024, // 1MB buffer
      });

      if (stderr && !stdout) {
        console.warn('🧹 Agent Cleanup: cursor-agent ls returned stderr:', stderr);
        return [];
      }

      // Parse output - cursor-agent ls might return JSON or structured text
      const agents = this.parseAgentList(stdout);
      return agents;
    } catch (error: any) {
      // If cursor-agent is not available or command fails, return empty array
      if (error.code === 'ENOENT' || error.message.includes('not found')) {
        console.warn('🧹 Agent Cleanup: cursor-agent command not found');
        return [];
      }
      console.error('🧹 Agent Cleanup: Error listing agents:', error.message);
      return [];
    }
  }

  /**
   * Parse agent list output from cursor-agent ls
   * Handles both JSON and text formats
   */
  private parseAgentList(output: string): AgentInfo[] {
    const agents: AgentInfo[] = [];
    
    if (!output || !output.trim()) {
      return agents;
    }

    try {
      // Try parsing as JSON first
      const json = JSON.parse(output);
      if (Array.isArray(json)) {
        return json.map((item: any) => this.parseAgentItem(item));
      } else if (json.agents && Array.isArray(json.agents)) {
        return json.agents.map((item: any) => this.parseAgentItem(item));
      }
    } catch {
      // Not JSON, try parsing as text
    }

    // Parse as text format (one agent per line or structured text)
    const lines = output.trim().split('\n');
    for (const line of lines) {
      if (!line.trim() || line.startsWith('#') || line.startsWith('ID')) {
        continue; // Skip headers and comments
      }

      // Try to extract agent ID (common patterns)
      const idMatch = line.match(/([a-f0-9-]{36}|[a-z0-9-]+)/i);
      if (idMatch) {
        const agent: AgentInfo = {
          id: idMatch[1],
        };

        // Try to extract timestamp or date
        const dateMatch = line.match(/(\d{4}-\d{2}-\d{2}[T\s]\d{2}:\d{2}:\d{2})/);
        if (dateMatch) {
          agent.createdAt = new Date(dateMatch[1]);
          agent.age = Date.now() - agent.createdAt.getTime();
        }

        agents.push(agent);
      }
    }

    return agents;
  }

  /**
   * Parse individual agent item (from JSON)
   */
  private parseAgentItem(item: any): AgentInfo {
    const agent: AgentInfo = {
      id: item.id || item.agentId || item.agent_id || String(item),
    };

    if (item.name) agent.name = item.name;
    if (item.createdAt) agent.createdAt = new Date(item.createdAt);
    if (item.created_at) agent.createdAt = new Date(item.created_at);
    if (item.lastUsed) agent.lastUsed = new Date(item.lastUsed);
    if (item.last_used) agent.lastUsed = new Date(item.last_used);

    if (agent.createdAt) {
      agent.age = Date.now() - agent.createdAt.getTime();
    } else if (item.age) {
      agent.age = typeof item.age === 'number' ? item.age : this.parseAge(item.age);
    }

    return agent;
  }

  /**
   * Parse age string (e.g., "7d", "2h", "1m")
   */
  private parseAge(ageStr: string): number {
    const match = ageStr.match(/(\d+)([hdm])/i);
    if (!match) return 0;

    const value = parseInt(match[1], 10);
    const unit = match[2].toLowerCase();

    switch (unit) {
      case 'h': return value * 60 * 60 * 1000;
      case 'd': return value * 24 * 60 * 60 * 1000;
      case 'm': return value * 30 * 24 * 60 * 60 * 1000; // Approximate month
      default: return 0;
    }
  }

  /**
   * Delete an agent using cursor-agent rm
   */
  async deleteAgent(agentId: string): Promise<boolean> {
    try {
      const { stdout, stderr } = await execAsync(`cursor-agent rm ${agentId}`, {
        timeout: 10000,
      });

      if (stderr && !stdout) {
        console.error(`🧹 Agent Cleanup: Failed to delete agent ${agentId}:`, stderr);
        return false;
      }

      return true;
    } catch (error: any) {
      console.error(`🧹 Agent Cleanup: Error deleting agent ${agentId}:`, error.message);
      return false;
    }
  }

  /**
   * Perform cleanup of old agents
   */
  async cleanup(): Promise<{ deleted: number; skipped: number; errors: number }> {
    if (!this.config.enabled) {
      return { deleted: 0, skipped: 0, errors: 0 };
    }

    console.log('🧹 Agent Cleanup: Starting cleanup...');

    const agents = await this.listAgents();
    if (agents.length === 0) {
      console.log('🧹 Agent Cleanup: No agents found');
      return { deleted: 0, skipped: 0, errors: 0 };
    }

    const minAgeMs = this.convertToMilliseconds(this.config.minAge.value, this.config.minAge.unit);
    const now = Date.now();

    let deleted = 0;
    let skipped = 0;
    let errors = 0;

    for (const agent of agents) {
      // Calculate age
      let age = agent.age;
      if (!age && agent.createdAt) {
        age = now - agent.createdAt.getTime();
      } else if (!age) {
        // If we can't determine age, skip it (safer)
        console.log(`🧹 Agent Cleanup: Skipping agent ${agent.id} (age unknown)`);
        skipped++;
        continue;
      }

      // Check if agent is old enough to delete
      if (age >= minAgeMs) {
        if (this.config.dryRun) {
          console.log(`🧹 Agent Cleanup [DRY RUN]: Would delete agent ${agent.id} (age: ${this.formatAge(age)})`);
          deleted++;
        } else {
          const success = await this.deleteAgent(agent.id);
          if (success) {
            console.log(`🧹 Agent Cleanup: Deleted agent ${agent.id} (age: ${this.formatAge(age)})`);
            deleted++;
          } else {
            errors++;
          }
        }
      } else {
        skipped++;
      }
    }

    console.log(`🧹 Agent Cleanup: Complete - Deleted: ${deleted}, Skipped: ${skipped}, Errors: ${errors}`);
    return { deleted, skipped, errors };
  }

  /**
   * Convert time value to milliseconds
   */
  private convertToMilliseconds(value: number, unit: 'hours' | 'days' | 'months'): number {
    switch (unit) {
      case 'hours':
        return value * 60 * 60 * 1000;
      case 'days':
        return value * 24 * 60 * 60 * 1000;
      case 'months':
        return value * 30 * 24 * 60 * 60 * 1000; // Approximate month (30 days)
      default:
        return value * 24 * 60 * 60 * 1000; // Default to days
    }
  }

  /**
   * Format age for display
   */
  private formatAge(ms: number): string {
    const days = Math.floor(ms / (24 * 60 * 60 * 1000));
    const hours = Math.floor((ms % (24 * 60 * 60 * 1000)) / (60 * 60 * 1000));
    
    if (days > 0) {
      return `${days}d ${hours}h`;
    } else {
      return `${hours}h`;
    }
  }

  /**
   * Get cleanup status
   */
  getStatus(): { enabled: boolean; isRunning: boolean; nextCleanup?: Date; config: AgentCleanupConfig } {
    let nextCleanup: Date | undefined;
    if (this.cleanupInterval && this.config.enabled) {
      const intervalMs = this.convertToMilliseconds(this.config.interval.value, this.config.interval.unit);
      // Estimate next cleanup (approximate)
      nextCleanup = new Date(Date.now() + intervalMs);
    }

    return {
      enabled: this.config.enabled,
      isRunning: this.isRunning,
      nextCleanup,
      config: this.config,
    };
  }
}
