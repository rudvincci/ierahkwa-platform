/**
 * Memory Service
 * Main service for SKMemory functionality - integrates all components
 * Ported from .skmemory/v1/api/skmemory.py
 */

import * as fs from 'fs';
import * as path from 'path';
import * as yaml from 'js-yaml';
import { execSync } from 'child_process';
import { IndexManager } from './IndexManager';
import { MemoryCache, MetadataCache } from './CacheManager';
import { QueryClassifier, QueryType } from './QueryClassifier';
import { Reranker, RerankResult } from './Reranker';
import { MemoryStorage, MemoryType, MemoryRole, MemoryEntry } from './MemoryStorage';

export interface MemoryConfig {
  cache?: {
    max_size?: number;
    content_cache_size?: number;
    search_cache_size?: number;
    metadata_cache_max_entries?: number;
    lazy_load_indexes?: boolean;
    auto_save_indexes?: boolean;
    auto_save_debounce_seconds?: number;
  };
  commit?: {
    auto?: boolean;
    message_prefix?: string;
  };
  memory_root?: string;
  memory_structure?: {
    public?: {
      base?: string;
      long_term?: string;
      mid_term?: string;
      roles?: string;
      short_term?: string;
    };
    private?: {
      base?: string;
      long_term?: string;
      mid_term?: string;
      roles?: string;
      short_term?: string;
    };
  };
  qdrant?: {
    enabled?: boolean;
    config_file?: string;
  };
  retention?: {
    long_term_permanent?: boolean;
    mid_term_months?: number;
    short_term_days?: number;
  };
  roles?: string[];
  version?: string;
}

export interface SearchResult {
  file_path: string;
  score: number;
  hybrid_score?: number;
  semantic_score?: number;
  keyword_score?: number;
  normalized_keyword_score?: number;
  source: 'semantic' | 'keyword' | 'hybrid';
  matched_terms?: string[];
  metadata?: Record<string, any>;
}

/**
 * Main Memory Service
 */
export class MemoryService {
  private root: string;
  private config: MemoryConfig;
  private indexManager?: IndexManager;
  private memoryCache?: MemoryCache;
  private metadataCache?: MetadataCache;
  private queryClassifier?: QueryClassifier;
  private reranker?: Reranker;
  private storage: MemoryStorage;
  private enableIndexes: boolean;
  private indexesLoaded: boolean = false;
  private lastSaveTime: number = 0;
  private autoSaveDebounceSeconds: number = 5;
  private autoSaveIndexes: boolean = true;

  constructor(root: string = '.maestro', enableIndexes: boolean = true) {
    this.root = root;
    this.enableIndexes = enableIndexes;
    this.config = this.loadConfig();

    // Initialize cache
    const cacheConfig = this.config.cache || {};
    const cacheSize = cacheConfig.max_size || 75;
    const contentCacheSize = cacheConfig.content_cache_size || cacheSize;
    const searchCacheSize = cacheConfig.search_cache_size || Math.floor(cacheSize / 2);
    const metadataCacheMaxEntries = cacheConfig.metadata_cache_max_entries || 500;

    this.memoryCache = new MemoryCache(contentCacheSize, searchCacheSize);
    this.metadataCache = new MetadataCache(metadataCacheMaxEntries);

    // Initialize index manager
    if (this.enableIndexes) {
      const indexRoot = path.join(this.root, 'memory');
      if (!fs.existsSync(indexRoot)) {
        fs.mkdirSync(indexRoot, { recursive: true });
      }
      this.indexManager = new IndexManager(indexRoot);
      
      // Lazy load indexes if configured
      const lazyLoad = cacheConfig.lazy_load_indexes !== false;
      if (!lazyLoad) {
        this.indexManager.loadAll();
        this.indexesLoaded = true;
      }
    }

    // Initialize query classifier
    this.queryClassifier = new QueryClassifier();

    // Initialize reranker
    const rerankerConfigPath = path.join(this.root, 'config', 'llm-config.yml');
    this.reranker = new Reranker(rerankerConfigPath);

    // Initialize storage
    this.storage = new MemoryStorage(this.root, this.config);

    // Auto-save configuration
    this.autoSaveIndexes = cacheConfig.auto_save_indexes !== false;
    this.autoSaveDebounceSeconds = cacheConfig.auto_save_debounce_seconds || 5;
  }

  private loadConfig(): MemoryConfig {
    const configPath = path.join(this.root, 'config', 'memory-config.yml');
    if (fs.existsSync(configPath)) {
      try {
        const data = fs.readFileSync(configPath, 'utf-8');
        return yaml.load(data) as MemoryConfig;
      } catch (error) {
        // Return default config
      }
    }
    return {};
  }

  private ensureIndexesLoaded(): void {
    if (!this.enableIndexes || !this.indexManager || this.indexesLoaded) {
      return;
    }

    this.indexManager.loadAll();
    this.indexesLoaded = true;
  }

  private debouncedSaveIndexes(): void {
    if (!this.autoSaveIndexes || !this.enableIndexes || !this.indexManager) {
      return;
    }

    const currentTime = Date.now();
    const timeSinceLastSave = (currentTime - this.lastSaveTime) / 1000;

    if (timeSinceLastSave >= this.autoSaveDebounceSeconds) {
      try {
        this.indexManager.saveAll();
        this.lastSaveTime = currentTime;
      } catch (error) {
        // Silently fail
      }
    }
  }

  /**
   * Add memory entry
   */
  add(
    content: string,
    type: MemoryType = 'mid-term',
    role?: MemoryRole,
    tags?: string[],
    isPublic: boolean = true,
    autoCommit?: boolean
  ): string {
    // Add to storage
    const filePath = this.storage.add(content, type, role, tags, isPublic, autoCommit);

    // Update indexes
    if (this.enableIndexes && this.indexManager) {
      this.ensureIndexesLoaded();
      try {
        const relativePath = path.relative(this.root, filePath);

        // Add to inverted index
        this.indexManager.invertedIndex.addDocument(relativePath, content);

        // Add to timestamp index
        const stats = fs.statSync(filePath);
        this.indexManager.timestampIndex.add(relativePath, stats.mtimeMs / 1000);

        // Add tags to trie
        if (tags) {
          for (const tag of tags) {
            const tagClean = tag.replace(/^#/, '').trim();
            if (tagClean) {
              this.indexManager.tagTrie.add(tagClean, relativePath);
            }
          }
        }

        // Add to Bloom filter
        this.indexManager.bloomFilter.add(relativePath);

        // Update memory graph with tags
        if (tags) {
          const fileTags: Record<string, string[]> = {};
          fileTags[relativePath] = tags.map(t => t.replace(/^#/, '').trim()).filter(t => t);
          this.indexManager.memoryGraph.computeRelationshipsFromTags(fileTags);
        }

        // Save indexes with debouncing
        if (autoCommit !== false && this.autoSaveIndexes) {
          this.debouncedSaveIndexes();
        }
      } catch (error) {
        // Silently fail if indexing fails
      }
    }

    // Cache content
    if (this.memoryCache) {
      this.memoryCache.putContent(filePath, content);
    }

    return filePath;
  }

  /**
   * Search memory
   */
  async search(
    query: string,
    topK: number = 5,
    memoryType?: MemoryType,
    useIndex: boolean = true,
    useHybrid: boolean = true,
    useLLMReranking?: boolean,
    isPublic: boolean = true
  ): Promise<SearchResult[]> {
    // Check cache first
    const cacheKey = `${query}:${memoryType}:${isPublic}:${useHybrid}:${useLLMReranking}`;
    if (this.memoryCache) {
      const cached = this.memoryCache.getSearch(cacheKey, memoryType);
      if (cached) {
        return cached;
      }
    }

    // Try hybrid search first (semantic + keyword)
    if (useHybrid) {
      try {
        // Classify query and get optimal weights
        let semanticWeight: number | undefined;
        let keywordWeight: number | undefined;

        if (this.queryClassifier) {
          try {
            [keywordWeight, semanticWeight] = this.queryClassifier.getHybridWeights(query);
          } catch (error) {
            // Fall back to default weights
            semanticWeight = 0.6;
            keywordWeight = 0.4;
          }
        } else {
          semanticWeight = 0.6;
          keywordWeight = 0.4;
        }

        // Normalize weights
        const totalWeight = (semanticWeight || 0) + (keywordWeight || 0);
        if (totalWeight > 0) {
          semanticWeight = (semanticWeight || 0) / totalWeight;
          keywordWeight = (keywordWeight || 0) / totalWeight;
        }

        // Get keyword results
        let keywordResults: SearchResult[] = [];
        if (useIndex && this.enableIndexes && this.indexManager) {
          this.ensureIndexesLoaded();
          try {
            const results = this.indexManager.invertedIndex.search(query, topK * 2);
            
            // Filter by memory type if specified
            keywordResults = results
              .filter(r => !memoryType || r.file_path.includes(memoryType))
              .map(r => ({
                file_path: r.file_path,
                score: r.score,
                source: 'keyword' as const,
                matched_terms: r.matched_terms,
              }));
          } catch (error) {
            // Ignore errors
          }
        }

        // Note: Semantic search would require Qdrant/embeddings
        // For now, we'll use keyword search only
        const semanticResults: SearchResult[] = [];

        // Combine results
        if (semanticResults.length > 0 || keywordResults.length > 0) {
          const combined = this.combineSearchResults(
            semanticResults,
            keywordResults,
            topK,
            semanticWeight,
            keywordWeight
          );

          // Apply LLM reranking if enabled
          let finalResults: SearchResult[] = combined;
          if (useLLMReranking !== false && this.reranker && combined.length > 0) {
            try {
              const reranked = await this.reranker.rerank(query, combined);
              // RerankResult is compatible with SearchResult, just cast it
              finalResults = reranked as SearchResult[];
            } catch (error) {
              // Fall back to original results
            }
          }

          // Cache results
          if (this.memoryCache) {
            this.memoryCache.putSearch(cacheKey, finalResults, memoryType);
          }

          return finalResults;
        }
      } catch (error) {
        // Fall back to keyword search only
      }
    }

    // Try keyword search only
    if (useIndex && this.enableIndexes && this.indexManager) {
      this.ensureIndexesLoaded();
      try {
        const results = this.indexManager.invertedIndex.search(query, topK);
        const filtered = results
          .filter(r => !memoryType || r.file_path.includes(memoryType))
          .map(r => ({
            file_path: r.file_path,
            score: r.score,
            source: 'keyword' as const,
            matched_terms: r.matched_terms,
          }));

        // Cache results
        if (this.memoryCache) {
          this.memoryCache.putSearch(cacheKey, filtered, memoryType);
        }

        return filtered;
      } catch (error) {
        // Fall back to git grep
      }
    }

    // Fallback to git grep
    try {
      const memoryPath = path.join(this.root, 'memory', isPublic ? 'public' : 'private');
      const searchPath = memoryType ? path.join(memoryPath, memoryType) : memoryPath;

      const result = execSync(
        `git grep -i -C 2 "${query}"`,
        { cwd: this.root, encoding: 'utf-8', maxBuffer: 10 * 1024 * 1024 }
      );

      // Parse git grep output into results
      const lines = result.split('\n');
      const results: SearchResult[] = [];
      let currentFile = '';
      let currentScore = 1.0;

      for (const line of lines) {
        if (line.includes(':')) {
          const match = line.match(/^([^:]+):/);
          if (match) {
            currentFile = match[1];
            results.push({
              file_path: currentFile,
              score: currentScore,
              source: 'keyword',
            });
            currentScore -= 0.1;
          }
        }
      }

      // Cache results
      if (this.memoryCache) {
        this.memoryCache.putSearch(cacheKey, results, memoryType);
      }

      return results.slice(0, topK);
    } catch (error) {
      return [];
    }
  }

  private combineSearchResults(
    semanticResults: SearchResult[],
    keywordResults: SearchResult[],
    topK: number,
    semanticWeight?: number,
    keywordWeight?: number
  ): SearchResult[] {
    const semWeight = semanticWeight || 0.6;
    const keyWeight = keywordWeight || 0.4;

    // Normalize keyword scores
    if (keywordResults.length > 0) {
      const maxScore = Math.max(...keywordResults.map(r => r.score));
      if (maxScore > 0) {
        for (const result of keywordResults) {
          (result as any).normalized_keyword_score = result.score / maxScore;
        }
      }
    }

    // Combine results
    const combined = new Map<string, SearchResult>();

    // Add semantic results
    for (const result of semanticResults) {
      const filePath = result.file_path;
      if (!combined.has(filePath)) {
        combined.set(filePath, {
          ...result,
          semantic_score: result.score,
          keyword_score: 0,
          normalized_keyword_score: 0,
          source: 'semantic',
        });
      } else {
        const existing = combined.get(filePath)!;
        existing.semantic_score = result.score;
        existing.source = 'hybrid';
      }
    }

    // Add keyword results
    for (const result of keywordResults) {
      const filePath = result.file_path;
      const normalizedScore = (result as any).normalized_keyword_score || 0;
      
      if (!combined.has(filePath)) {
        combined.set(filePath, {
          ...result,
          semantic_score: 0,
          keyword_score: result.score,
          normalized_keyword_score: normalizedScore,
          source: 'keyword',
        });
      } else {
        const existing = combined.get(filePath)!;
        existing.keyword_score = result.score;
        existing.normalized_keyword_score = normalizedScore;
        existing.matched_terms = result.matched_terms;
        existing.source = 'hybrid';
      }
    }

    // Calculate hybrid scores
    for (const result of combined.values()) {
      const hybridScore = 
        semWeight * (result.semantic_score || 0) +
        keyWeight * (result.normalized_keyword_score || 0);
      result.hybrid_score = hybridScore;
    }

    // Sort by hybrid score
    const sorted = Array.from(combined.values())
      .sort((a, b) => (b.hybrid_score || 0) - (a.hybrid_score || 0))
      .slice(0, topK);

    return sorted;
  }

  /**
   * Get memory statistics
   */
  async getStats(): Promise<{
    totalEntries: number;
    workflowsTracked: number;
    sessionsTracked: number;
    lastSaved: Date;
    autoSaveEnabled: boolean;
  }> {
    const entries = this.list();
    const workflows = entries.filter(e => e.tags?.includes('workflow'));
    const sessions = entries.filter(e => e.tags?.includes('session'));
    
    return {
      totalEntries: entries.length,
      workflowsTracked: workflows.length,
      sessionsTracked: sessions.length,
      lastSaved: new Date(this.lastSaveTime || Date.now()),
      autoSaveEnabled: this.autoSaveIndexes,
    };
  }

  /**
   * List memory entries
   */
  list(
    type?: MemoryType,
    role?: MemoryRole,
    isPublic: boolean = true,
    limit?: number
  ): MemoryEntry[] {
    const filePaths = this.storage.list(type, role, isPublic, limit);
    const entries: MemoryEntry[] = [];

    for (const filePath of filePaths) {
      const entry = this.storage.read(filePath);
      if (entry) {
        entries.push(entry);
      }
    }

    return entries;
  }

  /**
   * Get related memories
   */
  getRelated(filePath: string, maxResults: number = 10): Array<[string, number]> {
    if (!this.enableIndexes || !this.indexManager) {
      return [];
    }

    this.ensureIndexesLoaded();
    const relativePath = path.relative(this.root, filePath);
    return this.indexManager.memoryGraph.getRelated(relativePath, maxResults);
  }

  /**
   * Save indexes
   */
  saveIndexes(): void {
    if (this.enableIndexes && this.indexManager) {
      this.indexManager.saveAll();
      this.lastSaveTime = Date.now();
    }
  }

  /**
   * Clear caches
   */
  clearCaches(): void {
    if (this.memoryCache) {
      this.memoryCache.clear();
    }
    if (this.metadataCache) {
      this.metadataCache.clear();
    }
  }
}

