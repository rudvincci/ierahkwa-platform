/**
 * Multi-Environment Support Service
 * 
 * Manages configuration for different environments (dev, staging, prod).
 */

import * as fs from 'fs';
import * as path from 'path';
import * as yaml from 'js-yaml';
import { OrchestrationConfig } from '../domain/WorkflowDefinition';

export type Environment = 'development' | 'staging' | 'production' | 'test';

export interface EnvironmentConfig {
  name: Environment;
  description?: string;
  config: Partial<OrchestrationConfig>;
  variables?: Record<string, string>;
  overrides?: {
    workflows?: Record<string, Partial<any>>;
    roles?: Record<string, Partial<any>>;
  };
}

/**
 * Environment Manager
 */
export class EnvironmentManager {
  private environmentsDir: string;
  private currentEnvironment: Environment = 'development';
  private environments: Map<Environment, EnvironmentConfig> = new Map();

  constructor(repositoryRoot: string = process.cwd()) {
    this.environmentsDir = path.join(repositoryRoot, '.maestro', 'environments');
    this.loadEnvironments();
  }

  /**
   * Set current environment
   */
  setEnvironment(environment: Environment): void {
    this.currentEnvironment = environment;
  }

  /**
   * Get current environment
   */
  getCurrentEnvironment(): Environment {
    return this.currentEnvironment;
  }

  /**
   * Get environment config
   */
  getEnvironmentConfig(environment?: Environment): EnvironmentConfig | undefined {
    const env = environment || this.currentEnvironment;
    return this.environments.get(env);
  }

  /**
   * Apply environment to config
   */
  applyEnvironment(baseConfig: OrchestrationConfig, environment?: Environment): OrchestrationConfig {
    const env = environment || this.currentEnvironment;
    const envConfig = this.environments.get(env);

    if (!envConfig) {
      return baseConfig;
    }

    // Merge configs
    const merged: OrchestrationConfig = {
      ...baseConfig,
      ...envConfig.config,
    };

    // Apply workflow overrides
    if (envConfig.overrides?.workflows) {
      for (const [workflowName, overrides] of Object.entries(envConfig.overrides.workflows)) {
        if (merged.flows && merged.flows[workflowName]) {
          merged.flows[workflowName] = {
            ...merged.flows[workflowName],
            ...overrides,
          };
        }
      }
    }

    // Apply role overrides
    if (envConfig.overrides?.roles) {
      if (!merged.roles) {
        merged.roles = {};
      }
      for (const [roleName, overrides] of Object.entries(envConfig.overrides.roles)) {
        if (merged.roles[roleName]) {
          merged.roles[roleName] = {
            ...merged.roles[roleName],
            ...overrides,
          };
        }
      }
    }

    return merged;
  }

  /**
   * Get environment variable
   */
  getVariable(key: string, environment?: Environment): string | undefined {
    const env = environment || this.currentEnvironment;
    const envConfig = this.environments.get(env);
    return envConfig?.variables?.[key];
  }

  /**
   * Set environment variable
   */
  async setVariable(key: string, value: string, environment?: Environment): Promise<void> {
    const env = environment || this.currentEnvironment;
    let envConfig = this.environments.get(env);

    if (!envConfig) {
      envConfig = {
        name: env,
        config: {},
        variables: {},
      };
      this.environments.set(env, envConfig);
    }

    if (!envConfig.variables) {
      envConfig.variables = {};
    }

    envConfig.variables[key] = value;
    await this.saveEnvironment(env, envConfig);
  }

  /**
   * Create environment
   */
  async createEnvironment(config: EnvironmentConfig): Promise<void> {
    this.environments.set(config.name, config);
    await this.saveEnvironment(config.name, config);
  }

  /**
   * List environments
   */
  listEnvironments(): EnvironmentConfig[] {
    return Array.from(this.environments.values());
  }

  /**
   * Load environments
   */
  private loadEnvironments(): void {
    if (!fs.existsSync(this.environmentsDir)) {
      fs.mkdirSync(this.environmentsDir, { recursive: true });
      return;
    }

    const files = fs.readdirSync(this.environmentsDir);
    for (const file of files) {
      if (file.endsWith('.yml') || file.endsWith('.yaml')) {
        try {
          const filePath = path.join(this.environmentsDir, file);
          const content = fs.readFileSync(filePath, 'utf-8');
          const config = yaml.load(content) as EnvironmentConfig;
          this.environments.set(config.name, config);
        } catch (error) {
          console.warn(`Failed to load environment from ${file}:`, error);
        }
      }
    }
  }

  /**
   * Save environment
   */
  private async saveEnvironment(environment: Environment, config: EnvironmentConfig): Promise<void> {
    const filePath = path.join(this.environmentsDir, `${environment}.yml`);
    const content = yaml.dump(config, { indent: 2 });
    await fs.promises.writeFile(filePath, content, 'utf-8');
  }
}
