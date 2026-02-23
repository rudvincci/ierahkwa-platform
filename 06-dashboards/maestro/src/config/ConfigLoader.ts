import * as fs from 'fs';
import * as path from 'path';
import * as yaml from 'js-yaml';
import { OrchestrationConfig } from '../domain/WorkflowDefinition';
import { ConfigValidator } from './ConfigValidator';

export class ConfigLoader {
  private validator: ConfigValidator;

  constructor() {
    this.validator = new ConfigValidator();
  }

  /**
   * Load orchestration config from file or default location
   */
  loadConfig(configPath?: string): OrchestrationConfig {
    const resolvedPath = this.resolveConfigPath(configPath);
    
    if (!fs.existsSync(resolvedPath)) {
      throw new Error(`Config file not found: ${resolvedPath}`);
    }

    const content = fs.readFileSync(resolvedPath, 'utf-8');
    const config = yaml.load(content) as OrchestrationConfig;

    // Validate config structure
    this.validator.validate(config);

    return config;
  }

  /**
   * Resolve config path with discovery logic:
   * 1. Use provided path if given
   * 2. Check repo root for orchestration.yml
   * 3. Fallback to .maestro/config/orchestration.yml
   */
  private resolveConfigPath(configPath?: string): string {
    if (configPath) {
      return path.resolve(configPath);
    }

    // Check repo root
    const repoRoot = this.findRepoRoot();
    const repoRootConfig = path.join(repoRoot, 'orchestration.yml');
    if (fs.existsSync(repoRootConfig)) {
      return repoRootConfig;
    }

    // Fallback to default
    // Check if we're already in .maestro directory
    const cwd = process.cwd();
    if (cwd.endsWith('.maestro')) {
      return path.join(cwd, 'config/orchestration.yml');
    }
    // Otherwise, look for it relative to repo root
    return path.join(repoRoot, '.maestro/config/orchestration.yml');
  }

  /**
   * Find repository root by looking for .git directory
   */
  private findRepoRoot(): string {
    let currentDir = process.cwd();
    const root = path.parse(currentDir).root;

    while (currentDir !== root) {
      const gitDir = path.join(currentDir, '.git');
      if (fs.existsSync(gitDir)) {
        return currentDir;
      }
      currentDir = path.dirname(currentDir);
    }

    return process.cwd();
  }
}

