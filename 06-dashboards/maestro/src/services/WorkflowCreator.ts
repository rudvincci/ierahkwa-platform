/**
 * Workflow Creator Service
 * 
 * Handles workflow creation and validation
 */

import * as fs from 'fs';
import * as path from 'path';
import * as yaml from 'js-yaml';
import { OrchestrationConfig, WorkflowDefinition } from '../domain/WorkflowDefinition';
import { ConfigLoader } from '../config/ConfigLoader';
import { findRepoRoot } from '../cli/utils';

export interface CreateWorkflowRequest {
  name: string;
  description?: string;
  steps: Array<{
    name: string;
    agent: string;
    description: string;
    dependsOn?: string[];
  }>;
}

export class WorkflowCreator {
  private configLoader: ConfigLoader;
  private repoRoot: string;

  constructor() {
    this.configLoader = new ConfigLoader();
    this.repoRoot = findRepoRoot();
  }

  /**
   * Get available agent roles
   */
  async getAvailableRoles(): Promise<Array<{ name: string; description: string }>> {
    try {
      const config = this.configLoader.loadConfig();
      if (!config.roles) {
        return [];
      }
      return Object.entries(config.roles).map(([name, role]) => ({
        name,
        description: role.description || '',
      }));
    } catch (error) {
      console.error('Error loading roles:', error);
      return [];
    }
  }

  /**
   * Get config file path
   */
  private getConfigPath(): string {
    // Try repo root first
    const repoRootConfig = path.join(this.repoRoot, 'orchestration.yml');
    if (fs.existsSync(repoRootConfig)) {
      return repoRootConfig;
    }
    
    // Fallback to .maestro/config/orchestration.yml
    return path.join(this.repoRoot, '.maestro', 'config', 'orchestration.yml');
  }

  /**
   * Create a new workflow
   */
  async createWorkflow(request: CreateWorkflowRequest): Promise<{ success: boolean; error?: string }> {
    try {
      // Validate workflow
      if (!request.name || request.name.trim() === '') {
        return { success: false, error: 'Workflow name is required' };
      }

      if (!request.steps || request.steps.length === 0) {
        return { success: false, error: 'Workflow must have at least one step' };
      }

      // Validate step names are unique
      const stepNames = request.steps.map(s => s.name);
      const uniqueStepNames = new Set(stepNames);
      if (stepNames.length !== uniqueStepNames.size) {
        return { success: false, error: 'Step names must be unique' };
      }

      // Validate dependencies
      for (const step of request.steps) {
        if (step.dependsOn) {
          for (const dep of step.dependsOn) {
            if (!stepNames.includes(dep)) {
              return { success: false, error: `Step "${step.name}" depends on "${dep}" which does not exist` };
            }
          }
        }
      }

      // Load existing config
      const configPath = this.getConfigPath();
      let config: OrchestrationConfig;

      if (fs.existsSync(configPath)) {
        const content = fs.readFileSync(configPath, 'utf-8');
        config = yaml.load(content) as OrchestrationConfig;
      } else {
        // Create new config
        config = { flows: {}, roles: {} };
      }

      // Check if workflow already exists
      if (config.flows && config.flows[request.name]) {
        return { success: false, error: `Workflow "${request.name}" already exists` };
      }

      // Create workflow definition
      const workflow: WorkflowDefinition = {
        name: request.name,
        description: request.description,
        steps: request.steps.map(step => ({
          name: step.name,
          agent: step.agent,
          description: step.description,
          dependsOn: step.dependsOn || [],
        })),
      };

      // Add workflow to config
      if (!config.flows) {
        config.flows = {};
      }
      config.flows[request.name] = workflow;

      // Ensure config directory exists
      const configDir = path.dirname(configPath);
      if (!fs.existsSync(configDir)) {
        fs.mkdirSync(configDir, { recursive: true });
      }

      // Save config
      const yamlContent = yaml.dump(config, {
        indent: 2,
        lineWidth: 120,
        noRefs: true,
      });
      fs.writeFileSync(configPath, yamlContent, 'utf-8');

      return { success: true };
    } catch (error: any) {
      console.error('Error creating workflow:', error);
      return { success: false, error: error.message || 'Unknown error' };
    }
  }

  /**
   * Update an existing workflow
   */
  async updateWorkflow(name: string, request: CreateWorkflowRequest): Promise<{ success: boolean; error?: string }> {
    try {
      const configPath = this.getConfigPath();
      if (!fs.existsSync(configPath)) {
        return { success: false, error: 'Config file not found' };
      }

      const content = fs.readFileSync(configPath, 'utf-8');
      const config = yaml.load(content) as OrchestrationConfig;

      if (!config.flows || !config.flows[name]) {
        return { success: false, error: `Workflow "${name}" not found` };
      }

      // Validate (same as create)
      if (!request.steps || request.steps.length === 0) {
        return { success: false, error: 'Workflow must have at least one step' };
      }

      const stepNames = request.steps.map(s => s.name);
      const uniqueStepNames = new Set(stepNames);
      if (stepNames.length !== uniqueStepNames.size) {
        return { success: false, error: 'Step names must be unique' };
      }

      for (const step of request.steps) {
        if (step.dependsOn) {
          for (const dep of step.dependsOn) {
            if (!stepNames.includes(dep)) {
              return { success: false, error: `Step "${step.name}" depends on "${dep}" which does not exist` };
            }
          }
        }
      }

      // Update workflow
      const workflow: WorkflowDefinition = {
        name: request.name,
        description: request.description,
        steps: request.steps.map(step => ({
          name: step.name,
          agent: step.agent,
          description: step.description,
          dependsOn: step.dependsOn || [],
        })),
      };

      config.flows![name] = workflow;

      // Save config
      const yamlContent = yaml.dump(config, {
        indent: 2,
        lineWidth: 120,
        noRefs: true,
      });
      fs.writeFileSync(configPath, yamlContent, 'utf-8');

      return { success: true };
    } catch (error: any) {
      console.error('Error updating workflow:', error);
      return { success: false, error: error.message || 'Unknown error' };
    }
  }

  /**
   * Delete a workflow
   */
  async deleteWorkflow(name: string): Promise<{ success: boolean; error?: string }> {
    try {
      const configPath = this.getConfigPath();
      if (!fs.existsSync(configPath)) {
        return { success: false, error: 'Config file not found' };
      }

      const content = fs.readFileSync(configPath, 'utf-8');
      const config = yaml.load(content) as OrchestrationConfig;

      if (!config.flows || !config.flows[name]) {
        return { success: false, error: `Workflow "${name}" not found` };
      }

      delete config.flows[name];

      // Save config
      const yamlContent = yaml.dump(config, {
        indent: 2,
        lineWidth: 120,
        noRefs: true,
      });
      fs.writeFileSync(configPath, yamlContent, 'utf-8');

      return { success: true };
    } catch (error: any) {
      console.error('Error deleting workflow:', error);
      return { success: false, error: error.message || 'Unknown error' };
    }
  }
}
