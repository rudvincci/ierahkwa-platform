/**
 * Workflow Versioning Service
 * 
 * Tracks workflow changes, enables versioning, and rollback capabilities.
 */

import * as fs from 'fs';
import * as path from 'path';
import * as yaml from 'js-yaml';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';

export interface WorkflowVersion {
  version: string; // Semantic version (e.g., "1.2.3")
  workflow: WorkflowDefinition;
  createdAt: Date;
  createdBy?: string;
  description?: string;
  changes?: string[];
  tags?: string[];
}

export interface VersionHistory {
  workflowName: string;
  versions: WorkflowVersion[];
  currentVersion: string;
}

/**
 * Workflow Version Manager
 */
export class WorkflowVersionManager {
  private versionsDir: string;
  private historyFile: string;

  constructor(repositoryRoot: string = process.cwd()) {
    this.versionsDir = path.join(repositoryRoot, '.maestro', 'versions');
    this.historyFile = path.join(this.versionsDir, 'history.json');
    this.ensureDirectories();
  }

  /**
   * Ensure directories exist
   */
  private ensureDirectories(): void {
    if (!fs.existsSync(this.versionsDir)) {
      fs.mkdirSync(this.versionsDir, { recursive: true });
    }
  }

  /**
   * Create new version
   */
  async createVersion(
    workflow: WorkflowDefinition,
    version: string,
    options: {
      description?: string;
      changes?: string[];
      tags?: string[];
      createdBy?: string;
    } = {}
  ): Promise<WorkflowVersion> {
    const workflowVersion: WorkflowVersion = {
      version,
      workflow,
      createdAt: new Date(),
      createdBy: options.createdBy || 'system',
      description: options.description,
      changes: options.changes,
      tags: options.tags || [],
    };

    // Save version file
    const versionFile = path.join(this.versionsDir, `${workflow.name}-${version}.yml`);
    const content = yaml.dump(workflowVersion, { indent: 2 });
    await fs.promises.writeFile(versionFile, content, 'utf-8');

    // Update history
    await this.updateHistory(workflow.name, workflowVersion);

    return workflowVersion;
  }

  /**
   * Get version
   */
  async getVersion(workflowName: string, version: string): Promise<WorkflowVersion | null> {
    const versionFile = path.join(this.versionsDir, `${workflowName}-${version}.yml`);
    
    if (!fs.existsSync(versionFile)) {
      return null;
    }

    try {
      const content = await fs.promises.readFile(versionFile, 'utf-8');
      const versionData = yaml.load(content) as WorkflowVersion;
      return versionData;
    } catch (error) {
      console.warn(`Failed to load version ${version} of ${workflowName}:`, error);
      return null;
    }
  }

  /**
   * List versions for a workflow
   */
  async listVersions(workflowName: string): Promise<WorkflowVersion[]> {
    const history = await this.loadHistory();
    const workflowHistory = history.find(h => h.workflowName === workflowName);
    
    if (!workflowHistory) {
      return [];
    }

    // Load all versions
    const versions: WorkflowVersion[] = [];
    for (const version of workflowHistory.versions) {
      const versionData = await this.getVersion(workflowName, version.version);
      if (versionData) {
        versions.push(versionData);
      }
    }

    return versions.sort((a, b) => {
      return this.compareVersions(b.version, a.version); // Newest first
    });
  }

  /**
   * Get current version
   */
  async getCurrentVersion(workflowName: string): Promise<string | null> {
    const history = await this.loadHistory();
    const workflowHistory = history.find(h => h.workflowName === workflowName);
    return workflowHistory?.currentVersion || null;
  }

  /**
   * Set current version
   */
  async setCurrentVersion(workflowName: string, version: string): Promise<void> {
    const history = await this.loadHistory();
    let workflowHistory = history.find(h => h.workflowName === workflowName);

    if (!workflowHistory) {
      workflowHistory = {
        workflowName,
        versions: [],
        currentVersion: version,
      };
      history.push(workflowHistory);
    } else {
      workflowHistory.currentVersion = version;
    }

    await this.saveHistory(history);
  }

  /**
   * Rollback to version
   */
  async rollback(workflowName: string, version: string): Promise<WorkflowVersion | null> {
    const versionData = await this.getVersion(workflowName, version);
    if (!versionData) {
      return null;
    }

    await this.setCurrentVersion(workflowName, version);
    return versionData;
  }

  /**
   * Compare versions (semantic versioning)
   */
  private compareVersions(v1: string, v2: string): number {
    const parts1 = v1.split('.').map(Number);
    const parts2 = v2.split('.').map(Number);

    for (let i = 0; i < Math.max(parts1.length, parts2.length); i++) {
      const part1 = parts1[i] || 0;
      const part2 = parts2[i] || 0;

      if (part1 > part2) return 1;
      if (part1 < part2) return -1;
    }

    return 0;
  }

  /**
   * Increment version
   */
  incrementVersion(currentVersion: string, type: 'major' | 'minor' | 'patch'): string {
    const parts = currentVersion.split('.').map(Number);
    
    switch (type) {
      case 'major':
        return `${parts[0] + 1}.0.0`;
      case 'minor':
        return `${parts[0]}.${parts[1] + 1}.0`;
      case 'patch':
        return `${parts[0]}.${parts[1]}.${parts[2] + 1}`;
    }
  }

  /**
   * Load history
   */
  private async loadHistory(): Promise<VersionHistory[]> {
    if (!fs.existsSync(this.historyFile)) {
      return [];
    }

    try {
      const content = await fs.promises.readFile(this.historyFile, 'utf-8');
      return JSON.parse(content);
    } catch (error) {
      console.warn('Failed to load version history:', error);
      return [];
    }
  }

  /**
   * Save history
   */
  private async saveHistory(history: VersionHistory[]): Promise<void> {
    await fs.promises.writeFile(this.historyFile, JSON.stringify(history, null, 2), 'utf-8');
  }

  /**
   * Update history
   */
  private async updateHistory(workflowName: string, version: WorkflowVersion): Promise<void> {
    const history = await this.loadHistory();
    let workflowHistory = history.find(h => h.workflowName === workflowName);

    if (!workflowHistory) {
      workflowHistory = {
        workflowName,
        versions: [],
        currentVersion: version.version,
      };
      history.push(workflowHistory);
    }

    // Add version if not exists
    if (!workflowHistory.versions.find(v => v.version === version.version)) {
      workflowHistory.versions.push(version);
    }

    // Set as current if first version
    if (!workflowHistory.currentVersion) {
      workflowHistory.currentVersion = version.version;
    }

    await this.saveHistory(history);
  }
}
