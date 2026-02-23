/**
 * Result Cache Service
 * 
 * Caches task results to avoid re-running expensive operations.
 * Uses content-based hashing to detect duplicate tasks.
 */

import * as fs from 'fs';
import * as path from 'path';
import * as crypto from 'crypto';
import { AgentTask } from '../domain/AgentTask';
import { AgentResult } from '../workflow/OrchestratorContext';

export interface CacheEntry {
  taskHash: string;
  stepName: string;
  result: AgentResult;
  cachedAt: Date;
  expiresAt?: Date;
  metadata: {
    prompt: string;
    role: string;
    timeout?: number;
  };
}

export interface CacheOptions {
  ttl?: number; // Time to live in milliseconds
  maxSize?: number; // Maximum cache size in entries
  enabled?: boolean; // Enable/disable caching
}

/**
 * Result Cache Manager
 */
export class ResultCache {
  private cacheDir: string;
  private options: Required<CacheOptions>;
  private memoryCache: Map<string, CacheEntry> = new Map();
  private maxMemoryEntries: number = 100; // Keep hot entries in memory
  private cleanupInterval?: NodeJS.Timeout;
  private lastCleanupTime: number = 0;
  private cleanupIntervalMs: number = 5 * 60 * 1000; // Cleanup every 5 minutes
  private accessOrder: Map<string, number> = new Map(); // Track access order for LRU

  constructor(repositoryRoot: string = process.cwd(), options: CacheOptions = {}) {
    this.cacheDir = path.join(repositoryRoot, '.maestro', 'cache');
    this.options = {
      ttl: options.ttl || 7 * 24 * 60 * 60 * 1000, // 7 days default
      maxSize: options.maxSize || 1000, // 1000 entries default
      enabled: options.enabled ?? true,
    };

    // Ensure cache directory exists
    if (this.options.enabled) {
      fs.mkdirSync(this.cacheDir, { recursive: true });
      
      // Start periodic cleanup
      this.startPeriodicCleanup();
      
      // Cleanup on process exit
      process.on('exit', () => this.stopPeriodicCleanup());
      process.on('SIGINT', () => this.stopPeriodicCleanup());
      process.on('SIGTERM', () => this.stopPeriodicCleanup());
    }
  }

  /**
   * Start periodic cleanup
   */
  private startPeriodicCleanup(): void {
    if (this.cleanupInterval) {
      return;
    }
    
    this.cleanupInterval = setInterval(async () => {
      await this.cleanup();
      // Also cleanup memory cache
      this.cleanupMemoryCache();
    }, this.cleanupIntervalMs);
  }

  /**
   * Stop periodic cleanup
   */
  private stopPeriodicCleanup(): void {
    if (this.cleanupInterval) {
      clearInterval(this.cleanupInterval);
      this.cleanupInterval = undefined;
    }
  }

  /**
   * Cleanup memory cache (LRU eviction)
   */
  private cleanupMemoryCache(): void {
    // Remove expired entries
    const now = Date.now();
    for (const [key, entry] of this.memoryCache.entries()) {
      if (this.isExpired(entry)) {
        this.memoryCache.delete(key);
        this.accessOrder.delete(key);
      }
    }

    // LRU eviction if still over limit
    if (this.memoryCache.size > this.maxMemoryEntries) {
      // Sort by access time (oldest first)
      const sorted = Array.from(this.accessOrder.entries())
        .sort((a, b) => a[1] - b[1]);
      
      const toRemove = sorted.slice(0, this.memoryCache.size - this.maxMemoryEntries);
      for (const [key] of toRemove) {
        this.memoryCache.delete(key);
        this.accessOrder.delete(key);
      }
    }
  }

  /**
   * Generate cache key from task
   */
  private generateCacheKey(task: AgentTask, prompt: string): string {
    const keyData = {
      stepName: task.stepName,
      role: task.role,
      description: task.description,
      prompt: prompt.substring(0, 1000), // Use first 1000 chars for hashing
    };

    const hash = crypto
      .createHash('sha256')
      .update(JSON.stringify(keyData))
      .digest('hex');

    return hash;
  }

  /**
   * Get cached result for task
   */
  async get(task: AgentTask, prompt: string): Promise<AgentResult | null> {
    if (!this.options.enabled) {
      return null;
    }

    const cacheKey = this.generateCacheKey(task, prompt);
    const filePath = path.join(this.cacheDir, `${cacheKey}.json`);

    // Check memory cache first
    const memoryEntry = this.memoryCache.get(cacheKey);
    if (memoryEntry) {
      if (this.isExpired(memoryEntry)) {
        this.memoryCache.delete(cacheKey);
        this.accessOrder.delete(cacheKey);
        return null;
      }
      // Skip failed results - don't cache failures
      if (!memoryEntry.result.success) {
        this.memoryCache.delete(cacheKey);
        this.accessOrder.delete(cacheKey);
        return null;
      }
      // Update access time for LRU
      this.accessOrder.set(cacheKey, Date.now());
      console.log(`💾 Cache HIT (memory): ${task.stepName}`);
      return memoryEntry.result;
    }

    // Check disk cache
    try {
      if (fs.existsSync(filePath)) {
        const data = await fs.promises.readFile(filePath, 'utf-8');
        const entry: CacheEntry = JSON.parse(data);
        
        // Convert dates
        entry.cachedAt = new Date(entry.cachedAt);
        if (entry.expiresAt) {
          entry.expiresAt = new Date(entry.expiresAt);
        }

        if (this.isExpired(entry)) {
          await fs.promises.unlink(filePath);
          return null;
        }

        // Skip failed results - don't use cached failures
        if (!entry.result.success) {
          // Delete the failed cache entry
          await fs.promises.unlink(filePath);
          return null;
        }

        // Load into memory cache
        this.addToMemoryCache(cacheKey, entry);
        
        console.log(`💾 Cache HIT (disk): ${task.stepName}`);
        return entry.result;
      }
    } catch (error) {
      console.warn(`Failed to read cache for ${task.stepName}:`, error);
    }

    return null;
  }

  /**
   * Store result in cache
   * Only caches successful results - failures are not cached
   */
  async set(
    task: AgentTask,
    prompt: string,
    result: AgentResult,
    customTtl?: number
  ): Promise<void> {
    // Don't cache failed results
    if (!result.success) {
      return;
    }
    if (!this.options.enabled) {
      return;
    }

    const cacheKey = this.generateCacheKey(task, prompt);
    const ttl = customTtl !== undefined ? customTtl : this.options.ttl;
    const expiresAt = new Date(Date.now() + ttl);

    const entry: CacheEntry = {
      taskHash: cacheKey,
      stepName: task.stepName,
      result,
      cachedAt: new Date(),
      expiresAt,
      metadata: {
        prompt: prompt.substring(0, 500), // Store truncated prompt
        role: typeof task.role === 'string' ? task.role : task.role.name,
      },
    };

    // Store in memory cache
    this.addToMemoryCache(cacheKey, entry);

    // Store on disk
    try {
      const filePath = path.join(this.cacheDir, `${cacheKey}.json`);
      await fs.promises.writeFile(filePath, JSON.stringify(entry, null, 2), 'utf-8');
    } catch (error) {
      console.warn(`Failed to write cache for ${task.stepName}:`, error);
    }

    // Cleanup old entries if cache is too large (throttled - only runs periodically)
    // Don't block on cleanup - let periodic cleanup handle it
    if (Date.now() - this.lastCleanupTime > this.cleanupIntervalMs) {
      this.cleanup().catch(err => console.warn('Async cleanup failed:', err));
    }
  }

  /**
   * Check if cache entry is expired
   */
  private isExpired(entry: CacheEntry): boolean {
    if (!entry.expiresAt) {
      return false;
    }
    return Date.now() > entry.expiresAt.getTime();
  }

  /**
   * Add entry to memory cache (with LRU eviction)
   */
  private addToMemoryCache(key: string, entry: CacheEntry): void {
    // Optimize entry size - truncate large prompts
    if (entry.metadata.prompt && entry.metadata.prompt.length > 500) {
      entry.metadata.prompt = entry.metadata.prompt.substring(0, 500) + '...';
    }
    
    // LRU eviction if at capacity
    if (this.memoryCache.size >= this.maxMemoryEntries) {
      // Find least recently used entry
      let oldestKey: string | null = null;
      let oldestTime = Infinity;
      
      for (const [k, time] of this.accessOrder.entries()) {
        if (time < oldestTime) {
          oldestTime = time;
          oldestKey = k;
        }
      }
      
      if (oldestKey) {
        this.memoryCache.delete(oldestKey);
        this.accessOrder.delete(oldestKey);
      }
    }
    
    this.memoryCache.set(key, entry);
    this.accessOrder.set(key, Date.now());
  }

  /**
   * Cleanup expired and old entries (optimized - only runs periodically)
   */
  private async cleanup(): Promise<void> {
    const now = Date.now();
    // Throttle cleanup - don't run more than once per interval
    if (now - this.lastCleanupTime < this.cleanupIntervalMs) {
      return;
    }
    this.lastCleanupTime = now;

    try {
      const files = await fs.promises.readdir(this.cacheDir);
      const entries: Array<{ path: string; mtime: Date; expiresAt?: Date }> = [];
      const expiredPaths: string[] = [];

      // First pass: collect entries and identify expired ones
      for (const file of files) {
        if (file.endsWith('.json')) {
          const filePath = path.join(this.cacheDir, file);
          try {
            const stats = await fs.promises.stat(filePath);
            const data = await fs.promises.readFile(filePath, 'utf-8');
            const cacheEntry: CacheEntry = JSON.parse(data);
            
            const expiresAt = cacheEntry.expiresAt ? new Date(cacheEntry.expiresAt) : undefined;
            if (expiresAt && now > expiresAt.getTime()) {
              expiredPaths.push(filePath);
            } else {
              entries.push({ path: filePath, mtime: stats.mtime, expiresAt });
            }
          } catch (error) {
            // Corrupted entry - mark for deletion
            expiredPaths.push(filePath);
          }
        }
      }

      // Delete expired entries
      for (const filePath of expiredPaths) {
        try {
          await fs.promises.unlink(filePath);
        } catch (error) {
          // Ignore errors
        }
      }

      // Remove oldest entries if over max size (only if still needed after expired cleanup)
      if (entries.length > this.options.maxSize) {
        entries.sort((a, b) => (a.mtime.getTime()) - (b.mtime.getTime()));
        const toRemove = entries.slice(0, entries.length - this.options.maxSize);
        for (const entry of toRemove) {
          try {
            await fs.promises.unlink(entry.path);
          } catch (error) {
            // Ignore errors
          }
        }
      }
    } catch (error) {
      console.warn('Cache cleanup failed:', error);
    }
  }

  /**
   * Clear all cache entries
   */
  async clear(): Promise<void> {
    this.memoryCache.clear();
    this.accessOrder.clear();
    
    try {
      const files = await fs.promises.readdir(this.cacheDir);
      for (const file of files) {
        if (file.endsWith('.json')) {
          await fs.promises.unlink(path.join(this.cacheDir, file));
        }
      }
    } catch (error) {
      console.warn('Cache clear failed:', error);
    }
  }

  /**
   * Dispose resources
   */
  dispose(): void {
    this.stopPeriodicCleanup();
    this.memoryCache.clear();
    this.accessOrder.clear();
  }

  /**
   * Get cache statistics
   */
  async getStats(): Promise<{
    totalEntries: number;
    memoryEntries: number;
    diskEntries: number;
    totalSize: number;
  }> {
    const memoryEntries = this.memoryCache.size;
    let diskEntries = 0;
    let totalSize = 0;

    try {
      const files = await fs.promises.readdir(this.cacheDir);
      for (const file of files) {
        if (file.endsWith('.json')) {
          diskEntries++;
          const filePath = path.join(this.cacheDir, file);
          const stats = await fs.promises.stat(filePath);
          totalSize += stats.size;
        }
      }
    } catch (error) {
      // Ignore errors
    }

    return {
      totalEntries: Math.max(memoryEntries, diskEntries),
      memoryEntries,
      diskEntries,
      totalSize,
    };
  }

  /**
   * Invalidate cache for specific step
   */
  async invalidate(stepName: string): Promise<void> {
    if (!stepName) {
      return;
    }

    // Clear from memory
    for (const [key, entry] of this.memoryCache.entries()) {
      if (entry.stepName === stepName) {
        this.memoryCache.delete(key);
      }
    }

    // Clear from disk
    try {
      const files = await fs.promises.readdir(this.cacheDir);
      for (const file of files) {
        if (file.endsWith('.json')) {
          const filePath = path.join(this.cacheDir, file);
          const data = await fs.promises.readFile(filePath, 'utf-8');
          const entry: CacheEntry = JSON.parse(data);
          
          if (entry.stepName === stepName) {
            await fs.promises.unlink(filePath);
          }
        }
      }
    } catch (error) {
      console.warn(`Failed to invalidate cache for ${stepName}:`, error);
    }
  }
}
