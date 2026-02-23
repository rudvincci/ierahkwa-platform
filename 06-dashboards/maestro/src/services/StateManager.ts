/**
 * State Manager Service
 * 
 * Manages workflow execution state persistence using .skmemory and .claudememory
 * for checkpoint/resume functionality.
 */

import * as fs from 'fs';
import * as path from 'path';
import { AgentTask, TaskStatus } from '../domain/AgentTask';
import { OrchestratorContext, AgentResult } from '../workflow/OrchestratorContext';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';

export interface WorkflowCheckpoint {
  id: string;
  workflowName: string;
  startedAt: Date;
  lastUpdatedAt: Date;
  completedTasks: string[]; // Task step names
  failedTasks: string[]; // Task step names
  currentStep?: string;
  context: OrchestratorContext;
  results: Map<string, AgentResult>;
  metadata: {
    repositoryRoot: string;
    featureDescription?: string;
    maxConcurrency?: number;
    continueOnError?: boolean;
    abortOnError?: boolean;
  };
}

export interface StateStorage {
  save(checkpoint: WorkflowCheckpoint): Promise<void>;
  load(checkpointId: string): Promise<WorkflowCheckpoint | null>;
  list(): Promise<WorkflowCheckpoint[]>;
  delete(checkpointId: string): Promise<void>;
}

/**
 * Memory-based state storage using .skmemory and .claudememory
 */
export class MemoryStateStorage implements StateStorage {
  private skmemoryPath: string;
  private claudememoryPath: string;
  private useSKMemory: boolean;
  private useClaudeMemory: boolean;

  constructor(repositoryRoot: string = process.cwd()) {
    this.skmemoryPath = path.join(repositoryRoot, '.skmemory');
    this.claudememoryPath = path.join(repositoryRoot, '.claudememory');
    this.useSKMemory = fs.existsSync(this.skmemoryPath);
    this.useClaudeMemory = fs.existsSync(this.claudememoryPath);
  }

  async save(checkpoint: WorkflowCheckpoint): Promise<void> {
    const checkpointData = this.serializeCheckpoint(checkpoint);
    const checkpointId = checkpoint.id;

    // Save to both memory systems if available
    const promises: Promise<void>[] = [];

    if (this.useSKMemory) {
      promises.push(this.saveToSKMemory(checkpointId, checkpointData));
    }

    if (this.useClaudeMemory) {
      promises.push(this.saveToClaudeMemory(checkpointId, checkpointData));
    }

    // Fallback to local file if no memory systems available
    if (!this.useSKMemory && !this.useClaudeMemory) {
      promises.push(this.saveToLocalFile(checkpointId, checkpointData));
    }

    await Promise.all(promises);
  }

  async load(checkpointId: string): Promise<WorkflowCheckpoint | null> {
    // Try SKMemory first, then ClaudeMemory, then local file
    if (this.useSKMemory) {
      const data = await this.loadFromSKMemory(checkpointId);
      if (data) return this.deserializeCheckpoint(data);
    }

    if (this.useClaudeMemory) {
      const data = await this.loadFromClaudeMemory(checkpointId);
      if (data) return this.deserializeCheckpoint(data);
    }

    // Fallback to local file
    const data = await this.loadFromLocalFile(checkpointId);
    if (data) return this.deserializeCheckpoint(data);

    return null;
  }

  async list(): Promise<WorkflowCheckpoint[]> {
    const checkpoints: WorkflowCheckpoint[] = [];
    const checkpointIds = new Set<string>();

    // Collect checkpoint IDs from all sources
    if (this.useSKMemory) {
      const ids = await this.listFromSKMemory();
      ids.forEach(id => checkpointIds.add(id));
    }

    if (this.useClaudeMemory) {
      const ids = await this.listFromClaudeMemory();
      ids.forEach(id => checkpointIds.add(id));
    }

    // Load each checkpoint
    for (const id of checkpointIds) {
      const checkpoint = await this.load(id);
      if (checkpoint) {
        checkpoints.push(checkpoint);
      }
    }

    // Sort by last updated (most recent first)
    return checkpoints.sort((a, b) => 
      b.lastUpdatedAt.getTime() - a.lastUpdatedAt.getTime()
    );
  }

  async delete(checkpointId: string): Promise<void> {
    const promises: Promise<void>[] = [];

    if (this.useSKMemory) {
      promises.push(this.deleteFromSKMemory(checkpointId));
    }

    if (this.useClaudeMemory) {
      promises.push(this.deleteFromClaudeMemory(checkpointId));
    }

    promises.push(this.deleteFromLocalFile(checkpointId));

    await Promise.all(promises);
  }

  // SKMemory integration
  private async saveToSKMemory(checkpointId: string, data: string): Promise<void> {
    const checkpointDir = path.join(this.skmemoryPath, 'v1', 'memory', 'public', 'short-term', 'orchestrator-checkpoints');
    await fs.promises.mkdir(checkpointDir, { recursive: true });
    const filePath = path.join(checkpointDir, `${checkpointId}.json`);
    await fs.promises.writeFile(filePath, data, 'utf-8');
  }

  private async loadFromSKMemory(checkpointId: string): Promise<string | null> {
    try {
      const filePath = path.join(this.skmemoryPath, 'v1', 'memory', 'public', 'short-term', 'orchestrator-checkpoints', `${checkpointId}.json`);
      if (fs.existsSync(filePath)) {
        return await fs.promises.readFile(filePath, 'utf-8');
      }
    } catch (error) {
      console.warn(`Failed to load from SKMemory: ${error}`);
    }
    return null;
  }

  private async listFromSKMemory(): Promise<string[]> {
    try {
      const checkpointDir = path.join(this.skmemoryPath, 'v1', 'memory', 'public', 'short-term', 'orchestrator-checkpoints');
      if (fs.existsSync(checkpointDir)) {
        const files = await fs.promises.readdir(checkpointDir);
        return files
          .filter(f => f.endsWith('.json'))
          .map(f => f.replace('.json', ''));
      }
    } catch (error) {
      console.warn(`Failed to list from SKMemory: ${error}`);
    }
    return [];
  }

  private async deleteFromSKMemory(checkpointId: string): Promise<void> {
    try {
      const filePath = path.join(this.skmemoryPath, 'v1', 'memory', 'public', 'short-term', 'orchestrator-checkpoints', `${checkpointId}.json`);
      if (fs.existsSync(filePath)) {
        await fs.promises.unlink(filePath);
      }
    } catch (error) {
      console.warn(`Failed to delete from SKMemory: ${error}`);
    }
  }

  // ClaudeMemory integration
  private async saveToClaudeMemory(checkpointId: string, data: string): Promise<void> {
    const checkpointDir = path.join(this.claudememoryPath, 'checkpoints');
    await fs.promises.mkdir(checkpointDir, { recursive: true });
    const filePath = path.join(checkpointDir, `${checkpointId}.json`);
    await fs.promises.writeFile(filePath, data, 'utf-8');
  }

  private async loadFromClaudeMemory(checkpointId: string): Promise<string | null> {
    try {
      const filePath = path.join(this.claudememoryPath, 'checkpoints', `${checkpointId}.json`);
      if (fs.existsSync(filePath)) {
        return await fs.promises.readFile(filePath, 'utf-8');
      }
    } catch (error) {
      console.warn(`Failed to load from ClaudeMemory: ${error}`);
    }
    return null;
  }

  private async listFromClaudeMemory(): Promise<string[]> {
    try {
      const checkpointDir = path.join(this.claudememoryPath, 'checkpoints');
      if (fs.existsSync(checkpointDir)) {
        const files = await fs.promises.readdir(checkpointDir);
        return files
          .filter(f => f.endsWith('.json'))
          .map(f => f.replace('.json', ''));
      }
    } catch (error) {
      console.warn(`Failed to list from ClaudeMemory: ${error}`);
    }
    return [];
  }

  private async deleteFromClaudeMemory(checkpointId: string): Promise<void> {
    try {
      const filePath = path.join(this.claudememoryPath, 'checkpoints', `${checkpointId}.json`);
      if (fs.existsSync(filePath)) {
        await fs.promises.unlink(filePath);
      }
    } catch (error) {
      console.warn(`Failed to delete from ClaudeMemory: ${error}`);
    }
  }

  // Local file fallback
  private async saveToLocalFile(checkpointId: string, data: string): Promise<void> {
    const checkpointDir = path.join(process.cwd(), '.maestro', 'checkpoints');
    await fs.promises.mkdir(checkpointDir, { recursive: true });
    const filePath = path.join(checkpointDir, `${checkpointId}.json`);
    await fs.promises.writeFile(filePath, data, 'utf-8');
  }

  private async loadFromLocalFile(checkpointId: string): Promise<string | null> {
    try {
      const filePath = path.join(process.cwd(), '.maestro', 'checkpoints', `${checkpointId}.json`);
      if (fs.existsSync(filePath)) {
        return await fs.promises.readFile(filePath, 'utf-8');
      }
    } catch (error) {
      console.warn(`Failed to load from local file: ${error}`);
    }
    return null;
  }

  private async deleteFromLocalFile(checkpointId: string): Promise<void> {
    try {
      const filePath = path.join(process.cwd(), '.maestro', 'checkpoints', `${checkpointId}.json`);
      if (fs.existsSync(filePath)) {
        await fs.promises.unlink(filePath);
      }
    } catch (error) {
      console.warn(`Failed to delete local file: ${error}`);
    }
  }

  // Serialization helpers
  private serializeCheckpoint(checkpoint: WorkflowCheckpoint): string {
    // Convert Map to object for JSON serialization
    const serializable = {
      ...checkpoint,
      results: Object.fromEntries(checkpoint.results),
      context: {
        ...checkpoint.context,
        previousResults: Object.fromEntries(checkpoint.context.previousResults),
      },
      startedAt: checkpoint.startedAt.toISOString(),
      lastUpdatedAt: checkpoint.lastUpdatedAt.toISOString(),
    };
    return JSON.stringify(serializable, null, 2);
  }

  private deserializeCheckpoint(data: string): WorkflowCheckpoint {
    const parsed = JSON.parse(data);
    return {
      ...parsed,
      startedAt: new Date(parsed.startedAt),
      lastUpdatedAt: new Date(parsed.lastUpdatedAt),
      results: new Map(Object.entries(parsed.results || {})),
      context: {
        ...parsed.context,
        previousResults: new Map(Object.entries(parsed.context.previousResults || {})),
      },
    };
  }
}

/**
 * State Manager - High-level interface for checkpoint management
 */
export class StateManager {
  private storage: StateStorage;
  private currentCheckpoint: WorkflowCheckpoint | null = null;
  private autoSaveInterval: NodeJS.Timeout | null = null;
  private autoSaveEnabled: boolean = false;
  private verbose: boolean = false;

  constructor(storage: StateStorage, verbose: boolean = false) {
    this.storage = storage;
    this.verbose = verbose;
  }

  /**
   * Create a new checkpoint
   */
  createCheckpoint(
    workflowName: string,
    context: OrchestratorContext,
    completedTasks: string[],
    failedTasks: string[],
    currentStep?: string,
    metadata?: Partial<WorkflowCheckpoint['metadata']>
  ): WorkflowCheckpoint {
    const checkpoint: WorkflowCheckpoint = {
      id: this.generateCheckpointId(),
      workflowName,
      startedAt: new Date(),
      lastUpdatedAt: new Date(),
      completedTasks,
      failedTasks,
      currentStep,
      context: {
        ...context,
        previousResults: new Map(context.previousResults),
      },
      results: new Map(),
      metadata: {
        repositoryRoot: context.repositoryRoot,
        featureDescription: context.featureDescription,
        ...metadata,
      },
    };

    this.currentCheckpoint = checkpoint;
    return checkpoint;
  }

  /**
   * Update current checkpoint
   */
  updateCheckpoint(
    completedTasks: string[],
    failedTasks: string[],
    currentStep?: string,
    results?: Map<string, AgentResult>
  ): void {
    if (!this.currentCheckpoint) {
      throw new Error('No checkpoint to update');
    }

    this.currentCheckpoint.completedTasks = completedTasks;
    this.currentCheckpoint.failedTasks = failedTasks;
    this.currentCheckpoint.currentStep = currentStep;
    this.currentCheckpoint.lastUpdatedAt = new Date();

    if (results) {
      this.currentCheckpoint.results = new Map(results);
    }
  }

  /**
   * Save current checkpoint
   */
  async save(): Promise<void> {
    if (!this.currentCheckpoint) {
      throw new Error('No checkpoint to save');
    }

    await this.storage.save(this.currentCheckpoint);
  }

  /**
   * Load a checkpoint by ID
   */
  async load(checkpointId: string): Promise<WorkflowCheckpoint | null> {
    const checkpoint = await this.storage.load(checkpointId);
    if (checkpoint) {
      this.currentCheckpoint = checkpoint;
    }
    return checkpoint;
  }

  /**
   * List all available checkpoints
   */
  async list(): Promise<WorkflowCheckpoint[]> {
    return await this.storage.list();
  }

  /**
   * Delete a checkpoint
   */
  async delete(checkpointId: string): Promise<void> {
    await this.storage.delete(checkpointId);
    if (this.currentCheckpoint?.id === checkpointId) {
      this.currentCheckpoint = null;
    }
  }

  /**
   * Enable auto-save (saves checkpoint periodically)
   */
  enableAutoSave(intervalMs: number = 60000): void {
    if (this.autoSaveInterval) {
      this.disableAutoSave();
    }

    this.autoSaveEnabled = true;
    let lastSaveTime = 0;
    
    this.autoSaveInterval = setInterval(async () => {
      if (this.currentCheckpoint) {
        try {
          await this.save();
          // Log checkpoint saves in verbose mode, or every 5 minutes otherwise
          const now = Date.now();
          if (this.verbose || (now - lastSaveTime > 5 * 60 * 1000)) {
            if (this.verbose) {
              console.log(`💾 [VERBOSE] Auto-saved checkpoint: ${this.currentCheckpoint.id}`);
            } else {
              // Silent save - don't log every time in normal mode
            }
            lastSaveTime = now;
          }
        } catch (error) {
          console.warn(`Failed to auto-save checkpoint: ${error}`);
        }
      }
    }, intervalMs);
  }

  /**
   * Disable auto-save
   */
  disableAutoSave(): void {
    if (this.autoSaveInterval) {
      clearInterval(this.autoSaveInterval);
      this.autoSaveInterval = null;
    }
    this.autoSaveEnabled = false;
  }

  /**
   * Dispose resources
   */
  dispose(): void {
    this.disableAutoSave();
    // Clear current checkpoint from memory (it's persisted to storage)
    this.currentCheckpoint = null;
  }

  /**
   * Get current checkpoint
   */
  getCurrentCheckpoint(): WorkflowCheckpoint | null {
    return this.currentCheckpoint;
  }

  /**
   * Generate a unique checkpoint ID
   */
  private generateCheckpointId(): string {
    const timestamp = Date.now();
    const random = Math.random().toString(36).substring(2, 9);
    return `checkpoint-${timestamp}-${random}`;
  }

  /**
   * Cleanup on shutdown
   */
  async shutdown(): Promise<void> {
    this.disableAutoSave();
    if (this.currentCheckpoint) {
      await this.save();
    }
  }
}
