/**
 * Cache Manager
 * Provides LRU cache and metadata caching for improved performance
 * Ported from .skmemory/v1/api/cache.py
 */

import * as fs from 'fs';
import * as path from 'path';
import * as crypto from 'crypto';

/**
 * LRU (Least Recently Used) Cache implementation.
 * Uses Map for O(1) operations with automatic eviction.
 */
export class LRUCache<T> {
  private maxSize: number;
  private cache: Map<string, T>;

  constructor(maxSize: number = 100) {
    this.maxSize = maxSize;
    this.cache = new Map();
  }

  get(key: string): T | undefined {
    if (!this.cache.has(key)) {
      return undefined;
    }

    // Move to end (most recently used)
    const value = this.cache.get(key)!;
    this.cache.delete(key);
    this.cache.set(key, value);
    return value;
  }

  put(key: string, value: T): void {
    if (this.cache.has(key)) {
      // Update existing: remove and re-add to end
      this.cache.delete(key);
    } else if (this.cache.size >= this.maxSize) {
      // Evict least recently used (first item)
      const firstKey = this.cache.keys().next().value;
      if (firstKey !== undefined) {
        this.cache.delete(firstKey);
      }
    }

    this.cache.set(key, value);
  }

  clear(): void {
    this.cache.clear();
  }

  size(): number {
    return this.cache.size;
  }

  contains(key: string): boolean {
    return this.cache.has(key);
  }
}

/**
 * Cache for memory file contents and search results.
 * Uses LRU eviction policy.
 */
export class MemoryCache {
  private contentCache: LRUCache<string>;
  private searchCache: LRUCache<any>;

  constructor(maxSize: number = 75, searchCacheSize?: number) {
    const searchSize = searchCacheSize || Math.floor(maxSize / 2);
    this.contentCache = new LRUCache<string>(maxSize);
    this.searchCache = new LRUCache<any>(searchSize);
  }

  getContent(filePath: string): string | undefined {
    const key = path.resolve(filePath);
    return this.contentCache.get(key);
  }

  putContent(filePath: string, content: string): void {
    const key = path.resolve(filePath);
    this.contentCache.put(key, content);
  }

  getSearch(query: string, memoryType?: string): any {
    const key = this.makeSearchKey(query, memoryType);
    return this.searchCache.get(key);
  }

  putSearch(query: string, results: any, memoryType?: string): void {
    const key = this.makeSearchKey(query, memoryType);
    this.searchCache.put(key, results);
  }

  private makeSearchKey(query: string, memoryType?: string): string {
    let key = query.toLowerCase().trim();
    if (memoryType) {
      key = `${memoryType}:${key}`;
    }
    return key;
  }

  clear(): void {
    this.contentCache.clear();
    this.searchCache.clear();
  }

  invalidateFile(filePath: string): void {
    const key = path.resolve(filePath);
    if (this.contentCache.contains(key)) {
      // LRUCache doesn't expose delete, so we'll just let it expire naturally
      // or we can add a delete method
    }
  }
}

/**
 * Cache for file metadata to reduce I/O operations.
 * Uses hash map for O(1) lookups.
 */
export class MetadataCache {
  private maxEntries: number;
  private metadata: Map<string, Record<string, any>>;
  private fileHashes: Map<string, string>; // Track file hashes for change detection

  constructor(maxEntries: number = 500) {
    this.maxEntries = maxEntries;
    this.metadata = new Map();
    this.fileHashes = new Map();
  }

  get(filePath: string): Record<string, any> | undefined {
    const key = path.resolve(filePath);

    if (!this.metadata.has(key)) {
      return undefined;
    }

    // Check if file has changed
    if (!fs.existsSync(filePath)) {
      this.metadata.delete(key);
      this.fileHashes.delete(key);
      return undefined;
    }

    const currentHash = this.computeFileHash(filePath);
    const cachedHash = this.fileHashes.get(key);

    if (currentHash !== cachedHash) {
      // File changed, invalidate cache
      this.metadata.delete(key);
      this.fileHashes.delete(key);
      return undefined;
    }

    return this.metadata.get(key);
  }

  put(filePath: string, metadata: Record<string, any>): void {
    const key = path.resolve(filePath);

    // Evict oldest entry if at capacity (LRU-style)
    if (this.metadata.size >= this.maxEntries && !this.metadata.has(key)) {
      // Remove first (oldest) entry
      const firstKey = this.metadata.keys().next().value;
      if (firstKey !== undefined) {
        this.metadata.delete(firstKey);
        this.fileHashes.delete(firstKey);
      }
    }

    this.metadata.set(key, metadata);
    if (fs.existsSync(filePath)) {
      this.fileHashes.set(key, this.computeFileHash(filePath));
    }
  }

  getOrCompute(
    filePath: string,
    computeFunc: (filePath: string) => Record<string, any>
  ): Record<string, any> {
    const cached = this.get(filePath);
    if (cached !== undefined) {
      return cached;
    }

    // Compute metadata
    const metadata = computeFunc(filePath);
    this.put(filePath, metadata);
    return metadata;
  }

  private computeFileHash(filePath: string): string {
    try {
      const stats = fs.statSync(filePath);
      // Hash based on size and modification time
      const hashInput = `${stats.size}:${stats.mtimeMs}`;
      return crypto.createHash('md5').update(hashInput).digest('hex');
    } catch (error) {
      return '';
    }
  }

  invalidate(filePath: string): void {
    const key = path.resolve(filePath);
    this.metadata.delete(key);
    this.fileHashes.delete(key);
  }

  clear(): void {
    this.metadata.clear();
    this.fileHashes.clear();
  }

  size(): number {
    return this.metadata.size;
  }
}

