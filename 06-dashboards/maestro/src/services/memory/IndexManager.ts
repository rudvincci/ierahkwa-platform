/**
 * Index Manager
 * Provides various indexing data structures for efficient search and retrieval
 * Ported from .skmemory/v1/api/indexes.py
 */

import * as fs from 'fs';
import * as path from 'path';
import * as crypto from 'crypto';

export interface IndexEntry {
  file_path: string;
  positions: number[];
  count: number;
}

export interface TagTrieData {
  tag_files: Record<string, string[]>;
}

export interface MemoryGraphData {
  nodes: Record<string, Record<string, any>>;
  edges: Record<string, Record<string, number>>;
}

/**
 * Bloom filter for fast probabilistic existence checks.
 * Very memory-efficient for large datasets.
 */
export class BloomFilter {
  private capacity: number;
  private errorRate: number;
  private bitArraySize: number;
  private numHashes: number;
  private bitArray: Uint8Array;
  private count: number = 0;

  constructor(capacity: number = 10000, errorRate: number = 0.01) {
    this.capacity = capacity;
    this.errorRate = errorRate;

    // Calculate optimal bit array size and hash functions
    // m = -n * ln(p) / (ln(2)^2)
    this.bitArraySize = Math.ceil(
      (-capacity * Math.log(errorRate)) / (Math.log(2) ** 2)
    );
    // k = (m/n) * ln(2)
    this.numHashes = Math.ceil((this.bitArraySize / capacity) * Math.log(2));

    this.bitArray = new Uint8Array(Math.ceil(this.bitArraySize / 8));
  }

  private hash(item: string, seed: number): number {
    const hash = crypto.createHash('md5');
    hash.update(`${item}:${seed}`);
    const hex = hash.digest('hex');
    return parseInt(hex.substring(0, 8), 16) % this.bitArraySize;
  }

  add(item: string): void {
    for (let i = 0; i < this.numHashes; i++) {
      const bitIndex = this.hash(item, i);
      const byteIndex = Math.floor(bitIndex / 8);
      const bitOffset = bitIndex % 8;
      this.bitArray[byteIndex] |= 1 << bitOffset;
    }
    this.count++;
  }

  contains(item: string): boolean {
    for (let i = 0; i < this.numHashes; i++) {
      const bitIndex = this.hash(item, i);
      const byteIndex = Math.floor(bitIndex / 8);
      const bitOffset = bitIndex % 8;

      if (!(this.bitArray[byteIndex] & (1 << bitOffset))) {
        return false; // Definitely not present
      }
    }
    return true; // Might be present
  }

  toDict(): Record<string, any> {
    return {
      capacity: this.capacity,
      error_rate: this.errorRate,
      bit_array_size: this.bitArraySize,
      num_hashes: this.numHashes,
      bit_array: Buffer.from(this.bitArray).toString('hex'),
      count: this.count,
    };
  }

  static fromDict(data: Record<string, any>): BloomFilter {
    const bf = new BloomFilter(data.capacity, data.error_rate);
    bf.bitArray = Buffer.from(data.bit_array, 'hex');
    bf.count = data.count;
    return bf;
  }
}

/**
 * Inverted index for fast text search.
 * Maps terms to file paths and positions.
 */
export class InvertedIndex {
  private index: Map<string, IndexEntry[]> = new Map();
  private fileTerms: Map<string, Set<string>> = new Map();

  private tokenize(text: string): string[] {
    const lower = text.toLowerCase();
    const tokens = lower.match(/\b[a-z0-9-]+\b/g) || [];
    return tokens;
  }

  addDocument(filePath: string, content: string): void {
    // Remove old entries for this file
    if (this.fileTerms.has(filePath)) {
      const oldTerms = this.fileTerms.get(filePath)!;
      for (const term of oldTerms) {
        const entries = this.index.get(term) || [];
        const filtered = entries.filter(e => e.file_path !== filePath);
        if (filtered.length === 0) {
          this.index.delete(term);
        } else {
          this.index.set(term, filtered);
        }
      }
      this.fileTerms.delete(filePath);
    }

    // Index new content
    const terms = this.tokenize(content);
    const termPositions: Map<string, number[]> = new Map();

    for (let i = 0; i < terms.length; i++) {
      const term = terms[i];
      if (!termPositions.has(term)) {
        termPositions.set(term, []);
      }
      termPositions.get(term)!.push(i);
    }

    this.fileTerms.set(filePath, new Set(termPositions.keys()));

    for (const [term, positions] of termPositions.entries()) {
      if (!this.index.has(term)) {
        this.index.set(term, []);
      }
      this.index.get(term)!.push({
        file_path: filePath,
        positions,
        count: positions.length,
      });
    }
  }

  search(query: string, topK: number = 10): Array<{ file_path: string; score: number; matched_terms: string[] }> {
    const queryTerms = this.tokenize(query);
    if (queryTerms.length === 0) {
      return [];
    }

    // Score files by term frequency
    const fileScores: Map<string, number> = new Map();

    for (const term of queryTerms) {
      const entries = this.index.get(term) || [];
      for (const entry of entries) {
        const currentScore = fileScores.get(entry.file_path) || 0;
        fileScores.set(entry.file_path, currentScore + entry.count);
      }
    }

    // Sort by score and return top_k
    const sortedFiles = Array.from(fileScores.entries())
      .sort((a, b) => b[1] - a[1])
      .slice(0, topK);

    return sortedFiles.map(([filePath, score]) => ({
      file_path: filePath,
      score,
      matched_terms: queryTerms.filter(term => {
        const entries = this.index.get(term) || [];
        return entries.some(e => e.file_path === filePath);
      }),
    }));
  }

  removeDocument(filePath: string): void {
    const terms = this.fileTerms.get(filePath);
    if (!terms) return;

    for (const term of terms) {
      const entries = this.index.get(term) || [];
      const filtered = entries.filter(e => e.file_path !== filePath);
      if (filtered.length === 0) {
        this.index.delete(term);
      } else {
        this.index.set(term, filtered);
      }
    }
    this.fileTerms.delete(filePath);
  }

  toDict(): Record<string, any> {
    const indexObj: Record<string, IndexEntry[]> = {};
    for (const [term, entries] of this.index.entries()) {
      indexObj[term] = entries;
    }

    const fileTermsObj: Record<string, string[]> = {};
    for (const [filePath, terms] of this.fileTerms.entries()) {
      fileTermsObj[filePath] = Array.from(terms);
    }

    return {
      index: indexObj,
      file_terms: fileTermsObj,
    };
  }

  static fromDict(data: Record<string, any>): InvertedIndex {
    const idx = new InvertedIndex();
    const indexData = data.index || {};
    for (const [term, entries] of Object.entries(indexData)) {
      idx.index.set(term, entries as IndexEntry[]);
    }

    const fileTermsData = data.file_terms || {};
    for (const [filePath, terms] of Object.entries(fileTermsData)) {
      idx.fileTerms.set(filePath, new Set(terms as string[]));
    }

    return idx;
  }
}

/**
 * B-tree-like index for efficient date range queries.
 * Uses sorted list with binary search for simplicity.
 */
export class TimestampIndex {
  private entries: Array<[number, string]> = []; // [timestamp, file_path]
  private sorted: boolean = false;

  add(filePath: string, timestamp: number): void {
    this.entries.push([timestamp, filePath]);
    this.sorted = false;
  }

  private ensureSorted(): void {
    if (!this.sorted) {
      this.entries.sort((a, b) => a[0] - b[0]);
      this.sorted = true;
    }
  }

  rangeQuery(startTime?: number, endTime?: number): string[] {
    this.ensureSorted();

    if (this.entries.length === 0) {
      return [];
    }

    if (startTime === undefined) {
      startTime = this.entries[0][0];
    }
    if (endTime === undefined) {
      endTime = this.entries[this.entries.length - 1][0];
    }

    // Binary search for start
    let left = 0;
    let right = this.entries.length;
    while (left < right) {
      const mid = Math.floor((left + right) / 2);
      if (this.entries[mid][0] < startTime) {
        left = mid + 1;
      } else {
        right = mid;
      }
    }
    const startIdx = left;

    // Binary search for end
    left = 0;
    right = this.entries.length;
    while (left < right) {
      const mid = Math.floor((left + right) / 2);
      if (this.entries[mid][0] <= endTime) {
        left = mid + 1;
      } else {
        right = mid;
      }
    }
    const endIdx = left;

    return this.entries.slice(startIdx, endIdx).map(([, filePath]) => filePath);
  }

  remove(filePath: string): void {
    this.entries = this.entries.filter(([, fp]) => fp !== filePath);
    this.sorted = false;
  }

  toDict(): Record<string, any> {
    return { entries: this.entries };
  }

  static fromDict(data: Record<string, any>): TimestampIndex {
    const idx = new TimestampIndex();
    idx.entries = data.entries || [];
    idx.sorted = false;
    return idx;
  }
}

/**
 * Trie node for prefix tree
 */
class TrieNode {
  children: Map<string, TrieNode> = new Map();
  isEnd: boolean = false;
  filePaths: Set<string> = new Set();
}

/**
 * Trie (prefix tree) for efficient tag prefix search and autocomplete.
 */
export class TagTrie {
  private root: TrieNode = new TrieNode();
  private tagFiles: Map<string, Set<string>> = new Map(); // tag -> files

  add(tag: string, filePath: string): void {
    const normalizedTag = tag.toLowerCase().trim();
    if (!normalizedTag) return;

    let node = this.root;
    for (const char of normalizedTag) {
      if (!node.children.has(char)) {
        node.children.set(char, new TrieNode());
      }
      node = node.children.get(char)!;
    }

    node.isEnd = true;
    node.filePaths.add(filePath);

    if (!this.tagFiles.has(normalizedTag)) {
      this.tagFiles.set(normalizedTag, new Set());
    }
    this.tagFiles.get(normalizedTag)!.add(filePath);
  }

  searchPrefix(prefix: string): string[] {
    const normalizedPrefix = prefix.toLowerCase();
    let node = this.root;

    // Navigate to prefix node
    for (const char of normalizedPrefix) {
      if (!node.children.has(char)) {
        return []; // No tags with this prefix
      }
      node = node.children.get(char)!;
    }

    // Collect all tags from this node
    const tags: string[] = [];
    this.collectTags(node, normalizedPrefix, tags);
    return tags;
  }

  private collectTags(node: TrieNode, prefix: string, tags: string[]): void {
    if (node.isEnd) {
      tags.push(prefix);
    }

    for (const [char, child] of node.children.entries()) {
      this.collectTags(child, prefix + char, tags);
    }
  }

  getFiles(tag: string): Set<string> {
    return this.tagFiles.get(tag.toLowerCase()) || new Set();
  }

  remove(tag: string, filePath: string): void {
    const normalizedTag = tag.toLowerCase().trim();
    if (this.tagFiles.has(normalizedTag)) {
      this.tagFiles.get(normalizedTag)!.delete(filePath);
      if (this.tagFiles.get(normalizedTag)!.size === 0) {
        this.tagFiles.delete(normalizedTag);
      }
    }
    // Note: We don't remove from trie structure for simplicity
  }

  toDict(): TagTrieData {
    const tagFilesObj: Record<string, string[]> = {};
    for (const [tag, files] of this.tagFiles.entries()) {
      tagFilesObj[tag] = Array.from(files);
    }
    return { tag_files: tagFilesObj };
  }

  static fromDict(data: TagTrieData): TagTrie {
    const trie = new TagTrie();
    const tagFiles = data.tag_files || {};
    for (const [tag, files] of Object.entries(tagFiles)) {
      for (const filePath of files) {
        trie.add(tag, filePath);
      }
    }
    return trie;
  }
}

/**
 * Graph structure for tracking relationships between memories.
 * Uses adjacency list representation.
 */
export class MemoryGraph {
  private nodes: Map<string, Record<string, any>> = new Map(); // file_path -> node data
  private edges: Map<string, Map<string, number>> = new Map(); // file_path -> {neighbor: weight}

  addNode(filePath: string, metadata?: Record<string, any>): void {
    if (!this.nodes.has(filePath)) {
      this.nodes.set(filePath, metadata || {});
    }
  }

  addEdge(filePath1: string, filePath2: string, weight: number = 1.0): void {
    this.addNode(filePath1);
    this.addNode(filePath2);

    if (!this.edges.has(filePath1)) {
      this.edges.set(filePath1, new Map());
    }
    if (!this.edges.has(filePath2)) {
      this.edges.set(filePath2, new Map());
    }

    this.edges.get(filePath1)!.set(filePath2, weight);
    this.edges.get(filePath2)!.set(filePath1, weight); // Undirected graph
  }

  getRelated(filePath: string, maxResults: number = 10): Array<[string, number]> {
    const neighbors = this.edges.get(filePath);
    if (!neighbors) {
      return [];
    }

    const related = Array.from(neighbors.entries());
    related.sort((a, b) => b[1] - a[1]);
    return related.slice(0, maxResults);
  }

  computeRelationshipsFromTags(fileTags: Record<string, string[]>): void {
    // Build tag -> files mapping
    const tagFiles: Map<string, Set<string>> = new Map();
    for (const [filePath, tags] of Object.entries(fileTags)) {
      this.addNode(filePath);
      for (const tag of tags) {
        if (!tagFiles.has(tag)) {
          tagFiles.set(tag, new Set());
        }
        tagFiles.get(tag)!.add(filePath);
      }
    }

    // Add edges for files sharing tags
    for (const [, files] of tagFiles.entries()) {
      const filesList = Array.from(files);
      for (let i = 0; i < filesList.length; i++) {
        for (let j = i + 1; j < filesList.length; j++) {
          const file1 = filesList[i];
          const file2 = filesList[j];
          const currentWeight = this.edges.get(file1)?.get(file2) || 0.0;
          this.addEdge(file1, file2, currentWeight + 1.0);
        }
      }
    }
  }

  removeNode(filePath: string): void {
    this.nodes.delete(filePath);

    const neighbors = this.edges.get(filePath);
    if (neighbors) {
      for (const neighbor of neighbors.keys()) {
        this.edges.get(neighbor)?.delete(filePath);
      }
      this.edges.delete(filePath);
    }
  }

  toDict(): MemoryGraphData {
    const nodesObj: Record<string, Record<string, any>> = {};
    for (const [filePath, data] of this.nodes.entries()) {
      nodesObj[filePath] = data;
    }

    const edgesObj: Record<string, Record<string, number>> = {};
    for (const [filePath, neighbors] of this.edges.entries()) {
      const neighborsObj: Record<string, number> = {};
      for (const [neighbor, weight] of neighbors.entries()) {
        neighborsObj[neighbor] = weight;
      }
      edgesObj[filePath] = neighborsObj;
    }

    return {
      nodes: nodesObj,
      edges: edgesObj,
    };
  }

  static fromDict(data: MemoryGraphData): MemoryGraph {
    const graph = new MemoryGraph();
    graph.nodes = new Map(Object.entries(data.nodes || {}));
    graph.edges = new Map();
    for (const [filePath, neighbors] of Object.entries(data.edges || {})) {
      graph.edges.set(filePath, new Map(Object.entries(neighbors)));
    }
    return graph;
  }
}

/**
 * Manager for all indexes with persistence.
 */
export class IndexManager {
  private root: string;
  private indexDir: string;
  public invertedIndex: InvertedIndex;
  public timestampIndex: TimestampIndex;
  public tagTrie: TagTrie;
  public memoryGraph: MemoryGraph;
  public bloomFilter: BloomFilter;

  constructor(root: string) {
    this.root = root;
    this.indexDir = path.join(root, 'indexes');
    if (!fs.existsSync(this.indexDir)) {
      fs.mkdirSync(this.indexDir, { recursive: true });
    }

    this.invertedIndex = new InvertedIndex();
    this.timestampIndex = new TimestampIndex();
    this.tagTrie = new TagTrie();
    this.memoryGraph = new MemoryGraph();
    this.bloomFilter = new BloomFilter(10000, 0.01);
  }

  loadAll(): void {
    // Load inverted index
    const invIdxPath = path.join(this.indexDir, 'inverted-index.json');
    if (fs.existsSync(invIdxPath)) {
      try {
        const data = JSON.parse(fs.readFileSync(invIdxPath, 'utf-8'));
        this.invertedIndex = InvertedIndex.fromDict(data);
      } catch (error) {
        // Ignore errors, use default
      }
    }

    // Load timestamp index
    const tsIdxPath = path.join(this.indexDir, 'timestamp-index.json');
    if (fs.existsSync(tsIdxPath)) {
      try {
        const data = JSON.parse(fs.readFileSync(tsIdxPath, 'utf-8'));
        this.timestampIndex = TimestampIndex.fromDict(data);
      } catch (error) {
        // Ignore errors
      }
    }

    // Load tag trie
    const triePath = path.join(this.indexDir, 'tag-trie.json');
    if (fs.existsSync(triePath)) {
      try {
        const data = JSON.parse(fs.readFileSync(triePath, 'utf-8'));
        this.tagTrie = TagTrie.fromDict(data);
      } catch (error) {
        // Ignore errors
      }
    }

    // Load memory graph
    const graphPath = path.join(this.indexDir, 'memory-graph.json');
    if (fs.existsSync(graphPath)) {
      try {
        const data = JSON.parse(fs.readFileSync(graphPath, 'utf-8'));
        this.memoryGraph = MemoryGraph.fromDict(data);
      } catch (error) {
        // Ignore errors
      }
    }

    // Load Bloom filter
    const bloomPath = path.join(this.indexDir, 'bloom-filter.json');
    if (fs.existsSync(bloomPath)) {
      try {
        const data = JSON.parse(fs.readFileSync(bloomPath, 'utf-8'));
        this.bloomFilter = BloomFilter.fromDict(data);
      } catch (error) {
        // Ignore errors
      }
    }
  }

  saveAll(): void {
    // Save inverted index
    const invIdxPath = path.join(this.indexDir, 'inverted-index.json');
    fs.writeFileSync(invIdxPath, JSON.stringify(this.invertedIndex.toDict()));

    // Save timestamp index
    const tsIdxPath = path.join(this.indexDir, 'timestamp-index.json');
    fs.writeFileSync(tsIdxPath, JSON.stringify(this.timestampIndex.toDict()));

    // Save tag trie
    const triePath = path.join(this.indexDir, 'tag-trie.json');
    fs.writeFileSync(triePath, JSON.stringify(this.tagTrie.toDict()));

    // Save memory graph
    const graphPath = path.join(this.indexDir, 'memory-graph.json');
    fs.writeFileSync(graphPath, JSON.stringify(this.memoryGraph.toDict()));

    // Save Bloom filter
    const bloomPath = path.join(this.indexDir, 'bloom-filter.json');
    fs.writeFileSync(bloomPath, JSON.stringify(this.bloomFilter.toDict()));
  }

  clearAll(): void {
    this.invertedIndex = new InvertedIndex();
    this.timestampIndex = new TimestampIndex();
    this.tagTrie = new TagTrie();
    this.memoryGraph = new MemoryGraph();
    this.bloomFilter = new BloomFilter(10000, 0.01);
  }
}

