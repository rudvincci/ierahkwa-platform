/**
 * Cursor Agent Service
 * Service for agent management and history
 */

import { CursorApiClient, CursorAgentInfo } from './CursorApiClient';
import { execSync } from 'child_process';

export class CursorAgentService {
  private client: CursorApiClient;

  constructor(client: CursorApiClient) {
    this.client = client;
  }

  /**
   * Get all agents
   */
  async getAgents(): Promise<CursorAgentInfo[]> {
    return await this.client.getAgents();
  }

  /**
   * Get agent by ID
   */
  async getAgent(agentId: string): Promise<CursorAgentInfo | null> {
    const agents = await this.getAgents();
    return agents.find(a => a.id === agentId) || null;
  }

  /**
   * Get agent activity
   */
  async getAgentActivity(agentId: string, limit: number = 50): Promise<Array<{
    timestamp: Date;
    action: string;
    details?: Record<string, any>;
  }>> {
    const agent = await this.getAgent(agentId);
    return agent?.history?.slice(0, limit) || [];
  }

  /**
   * List agents using cursor-agent CLI
   */
  async listAgentsFromCli(): Promise<CursorAgentInfo[]> {
    try {
      const output = execSync('cursor-agent ls', {
        encoding: 'utf-8',
        maxBuffer: 10 * 1024 * 1024,
      });

      const agents: CursorAgentInfo[] = [];
      const lines = output.split('\n');

      for (const line of lines) {
        const trimmed = line.trim();
        if (trimmed && !trimmed.startsWith('ID') && !trimmed.startsWith('-')) {
          const parts = trimmed.split(/\s+/);
          if (parts.length > 0) {
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
   * Get agent statistics
   */
  async getAgentStats(): Promise<{
    total: number;
    active: number;
    idle: number;
    stopped: number;
  }> {
    const agents = await this.getAgents();
    
    return {
      total: agents.length,
      active: agents.filter(a => a.status === 'active').length,
      idle: agents.filter(a => a.status === 'idle').length,
      stopped: agents.filter(a => a.status === 'stopped').length,
    };
  }

  /**
   * Delete agent
   */
  async deleteAgent(agentId: string): Promise<boolean> {
    try {
      // Try to delete via CLI first
      execSync(`cursor-agent delete ${agentId}`, {
        encoding: 'utf-8',
        maxBuffer: 10 * 1024 * 1024,
      });
      return true;
    } catch (error) {
      // If CLI fails, try API
      try {
        await this.client.deleteAgent(agentId);
        return true;
      } catch (apiError) {
        console.error(`Failed to delete agent ${agentId}:`, error, apiError);
        return false;
      }
    }
  }
}

