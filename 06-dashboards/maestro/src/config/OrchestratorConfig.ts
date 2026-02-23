/**
 * Orchestrator Configuration
 * 
 * Loads and manages orchestrator configuration from orchestrator.config.yml
 */

import * as fs from 'fs/promises';
import * as path from 'path';
import * as yaml from 'js-yaml';

export interface MemorySystemConfig {
  enabled: boolean;
  path?: string;
}

export interface MemoryConfig {
  enabled: boolean;
  autoInitializeSubmodules: boolean;
  systems: {
    skmemory: MemorySystemConfig;
    composermemory: MemorySystemConfig;
    orchestrator: MemorySystemConfig;
  };
  search: {
    maxResults: number;
    minRelevance: number;
    timeout: number;
  };
}

export interface ManagerConfig {
  enabled: boolean;
  useMemory: boolean;
  memoryUnavailableBehavior: 'warn' | 'skip' | 'error';
}

export interface ExecutionConfig {
  maxConcurrency: number;
  defaultTimeout: number;
  retry: {
    enabled: boolean;
    maxAttempts: number;
    backoffMs: number;
  };
  // Node.js heap memory size in MB (default: 4096 = 4GB)
  heapMemorySize?: number;
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

export interface LoggingConfig {
  level: 'trace' | 'debug' | 'info' | 'warn' | 'error';
  file: {
    enabled: boolean;
    path: string;
    maxSize: string;
    maxFiles: number;
  };
  console: {
    enabled: boolean;
  };
}

export interface PrivateSyncConfig {
  enabled: boolean;
  repositoryUrl?: string;
  repositoryPath?: string;
  branch?: string;
  encryptionKey?: string; // For encrypting sensitive data
  syncInterval?: number; // Minutes between syncs
  includeCheckpoints?: boolean;
  includeCache?: boolean;
  includeReports?: boolean;
  includeLogs?: boolean;
  excludePatterns?: string[];
}

export interface OrchestratorConfig {
  memory: MemoryConfig;
  manager: ManagerConfig;
  execution: ExecutionConfig;
  logging: LoggingConfig;
  privateSync?: PrivateSyncConfig;
  agentCleanup?: AgentCleanupConfig;
}

const DEFAULT_CONFIG: OrchestratorConfig = {
  memory: {
    enabled: true,
    autoInitializeSubmodules: false,
    systems: {
      skmemory: {
        enabled: true,
        path: '.skmemory',
      },
      composermemory: {
        enabled: true,
        path: '.composermemory',
      },
      orchestrator: {
        enabled: true,
        path: '.maestro',
      },
    },
    search: {
      maxResults: 10,
      minRelevance: 0.3,
      timeout: 5000,
    },
  },
  manager: {
    enabled: true,
    useMemory: true,
    memoryUnavailableBehavior: 'warn',
  },
  execution: {
    maxConcurrency: 5,
    defaultTimeout: 3600,
    heapMemorySize: 4096, // 4GB default (reduced from 8GB to prevent memory pressure)
    retry: {
      enabled: true,
      maxAttempts: 3,
      backoffMs: 1000,
    },
  },
  logging: {
    level: 'info',
    file: {
      enabled: true,
      path: 'logs/orchestrator.log',
      maxSize: '10MB',
      maxFiles: 5,
    },
    console: {
      enabled: true,
    },
  },
  privateSync: {
    enabled: false, // OPT-IN ONLY - disabled by default
    branch: 'main',
    syncInterval: 60, // 60 minutes
    includeCheckpoints: true,
    includeCache: false,
    includeReports: true,
    includeLogs: false,
    excludePatterns: [],
  },
  agentCleanup: {
    enabled: true,
    interval: { value: 24, unit: 'hours' }, // Run cleanup every 24 hours
    minAge: { value: 7, unit: 'days' }, // Delete agents older than 7 days
    dryRun: false,
  },
};

export class ConfigLoader {
  private static config: OrchestratorConfig | null = null;
  private static configPath: string | null = null;

  /**
   * Load configuration from file or use defaults
   */
  static async load(configPath?: string): Promise<OrchestratorConfig> {
    if (this.config) {
      return this.config;
    }

    const actualConfigPath = configPath || this.findConfigPath();
    
    try {
      const configContent = await fs.readFile(actualConfigPath, 'utf-8');
      const loadedConfig = yaml.load(configContent) as Partial<OrchestratorConfig>;
      
      // Merge with defaults
      this.config = this.mergeConfig(DEFAULT_CONFIG, loadedConfig);
      this.configPath = actualConfigPath;
      
      return this.config;
    } catch (error) {
      // Config file not found or invalid, use defaults
      console.warn(`Config file not found at ${actualConfigPath}, using defaults`);
      this.config = DEFAULT_CONFIG;
      return this.config;
    }
  }

  /**
   * Get current configuration (loads if not already loaded)
   */
  static async get(): Promise<OrchestratorConfig> {
    if (!this.config) {
      return await this.load();
    }
    return this.config;
  }

  /**
   * Reset configuration (useful for testing)
   */
  static reset(): void {
    this.config = null;
    this.configPath = null;
  }

  /**
   * Find config file path
   */
  private static findConfigPath(): string {
    const fs = require('fs');
    
    // Try relative to orchestrator directory
    const orchestratorDir = path.join(process.cwd(), '.maestro');
    const configPath = path.join(orchestratorDir, 'config', 'orchestrator.config.yml');
    
    // Check if orchestrator directory exists
    try {
      if (fs.existsSync(configPath)) {
        return configPath;
      }
    } catch {
      // Fall through
    }

    // Try current directory
    const currentDirConfig = path.join(process.cwd(), 'config', 'orchestrator.config.yml');
    try {
      if (fs.existsSync(currentDirConfig)) {
        return currentDirConfig;
      }
    } catch {
      // Fall through
    }

    // Default path (may not exist, that's OK)
    return configPath;
  }

  /**
   * Deep merge configuration objects
   */
  private static mergeConfig(defaults: OrchestratorConfig, override: Partial<OrchestratorConfig>): OrchestratorConfig {
    return {
      memory: {
        ...defaults.memory,
        ...override.memory,
        systems: {
          ...defaults.memory.systems,
          ...override.memory?.systems,
          skmemory: {
            ...defaults.memory.systems.skmemory,
            ...override.memory?.systems?.skmemory,
          },
          composermemory: {
            ...defaults.memory.systems.composermemory,
            ...override.memory?.systems?.composermemory,
          },
          orchestrator: {
            ...defaults.memory.systems.orchestrator,
            ...override.memory?.systems?.orchestrator,
          },
        },
        search: {
          ...defaults.memory.search,
          ...override.memory?.search,
        },
      },
      manager: {
        ...defaults.manager,
        ...override.manager,
      },
      execution: {
        ...defaults.execution,
        ...override.execution,
        heapMemorySize: override.execution?.heapMemorySize ?? defaults.execution.heapMemorySize,
        retry: {
          ...defaults.execution.retry,
          ...override.execution?.retry,
        },
      },
      logging: {
        ...defaults.logging,
        ...override.logging,
        file: {
          ...defaults.logging.file,
          ...override.logging?.file,
        },
      console: {
        ...defaults.logging.console,
        ...override.logging?.console,
      },
    },
    agentCleanup: override.agentCleanup ?? defaults.agentCleanup,
  };
}
}
