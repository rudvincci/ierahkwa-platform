import * as path from 'path';
import * as fs from 'fs';

/**
 * Get Node.js heap memory size from config or environment
 * Returns size in MB, default is 4096 (4GB)
 */
export function getHeapMemorySize(): number {
  // Check environment variable first (highest priority)
  const envHeapSize = process.env.MAESTRO_HEAP_SIZE || process.env.NODE_OPTIONS?.match(/--max-old-space-size=(\d+)/)?.[1];
  if (envHeapSize) {
    const size = parseInt(envHeapSize, 10);
    if (!isNaN(size) && size > 0) {
      return size;
    }
  }

  // Try to load from config file (using the async ConfigLoader)
  try {
    const { ConfigLoader } = require('../config/OrchestratorConfig');
    // Use synchronous approach - try to read config file directly
    const fs = require('fs');
    const path = require('path');
    const yaml = require('js-yaml');
    
    const repoRoot = findRepoRoot();
    const configPath = path.join(repoRoot, '.maestro', 'config', 'orchestrator.config.yml');
    
    if (fs.existsSync(configPath)) {
      const configContent = fs.readFileSync(configPath, 'utf-8');
      const config = yaml.load(configContent);
      if (config?.execution?.heapMemorySize) {
        return config.execution.heapMemorySize;
      }
    }
  } catch {
    // Config not available, use default
  }

  // Default: 4GB (4096 MB) - reduced from 8GB to prevent memory pressure
  return 4096;
}

/**
 * Get Node.js executable arguments with heap memory size
 */
export function getNodeArgs(scriptArgs: string[]): string[] {
  const heapSize = getHeapMemorySize();
  return ['--max-old-space-size=' + heapSize, ...scriptArgs];
}

export function findRepoRoot(): string {
  let currentDir = process.cwd();
  const root = path.parse(currentDir).root;

  // If we're already in .maestro directory, go up one level first
  if (currentDir.endsWith('.maestro') || path.basename(currentDir) === '.maestro') {
    const parentDir = path.dirname(currentDir);
    const parentGitDir = path.join(parentDir, '.git');
    if (fs.existsSync(parentGitDir)) {
      return parentDir;
    }
  }

  // First, prioritize finding .maestro directory
  let checkDir = process.cwd();
  while (checkDir !== root) {
    const maestroDir = path.join(checkDir, '.maestro');
    if (fs.existsSync(maestroDir)) {
      // Found .maestro, this is the repo root
      return checkDir;
    }
    checkDir = path.dirname(checkDir);
  }

  // Fallback: find git root if .maestro not found
  while (currentDir !== root) {
    const gitDir = path.join(currentDir, '.git');
    // Check for .git directory (not file - submodules have .git files, we want the actual repo)
    const gitStat = fs.existsSync(gitDir) ? fs.statSync(gitDir) : null;
    if (gitStat && gitStat.isDirectory()) {
      // If we're inside .maestro submodule, go up one level to get the actual repo root
      if (currentDir.endsWith('.maestro') || path.basename(currentDir) === '.maestro') {
        const parentDir = path.dirname(currentDir);
        const parentGitDir = path.join(parentDir, '.git');
        const parentGitStat = fs.existsSync(parentGitDir) ? fs.statSync(parentGitDir) : null;
        if (parentGitStat && parentGitStat.isDirectory()) {
          return parentDir;
        }
      }
      return currentDir;
    }
    currentDir = path.dirname(currentDir);
  }

  return process.cwd();
}

export function formatDuration(ms: number): string {
  if (ms < 1000) {
    return `${ms}ms`;
  }
  if (ms < 60000) {
    return `${(ms / 1000).toFixed(1)}s`;
  }
  const minutes = Math.floor(ms / 60000);
  const seconds = Math.floor((ms % 60000) / 1000);
  return `${minutes}m ${seconds}s`;
}

/**
 * Open a URL in the default browser (cross-platform)
 */
export function openBrowser(url: string): void {
  const { spawn } = require('child_process');
  const platform = process.platform;
  let command: string;
  
  if (platform === 'darwin') {
    command = 'open';
  } else if (platform === 'win32') {
    command = 'start';
  } else {
    // Linux and others
    command = 'xdg-open';
  }
  
  const child = spawn(command, [url], {
    detached: true,
    stdio: 'ignore',
  });
  child.unref();
}

