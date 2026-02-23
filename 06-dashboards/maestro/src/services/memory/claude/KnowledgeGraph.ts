/**
 * Knowledge Graph
 * Generates and queries knowledge graph from memory relationships
 * Ported from .claudememory configuration
 */

import * as fs from 'fs';
import * as path from 'path';
import { MemoryGraph } from '../IndexManager';

export interface KnowledgeNode {
  id: string;
  type: 'pattern' | 'session' | 'memory' | 'file';
  label: string;
  metadata?: Record<string, any>;
}

export interface KnowledgeEdge {
  from: string;
  to: string;
  weight: number;
  type: 'references' | 'similar' | 'depends_on' | 'related';
}

export interface KnowledgeGraphData {
  nodes: KnowledgeNode[];
  edges: KnowledgeEdge[];
}

export interface KnowledgeGraphConfig {
  auto_build?: boolean;
  rebuild_interval?: number; // days
  include_external?: boolean;
  external_path?: string;
  graph_file?: string;
}

/**
 * Knowledge graph for relationship extraction and traversal
 */
export class KnowledgeGraph {
  private root: string;
  private config: KnowledgeGraphConfig;
  private graph: MemoryGraph;
  private graphFile: string;
  private nodes: Map<string, KnowledgeNode> = new Map();
  private edges: Map<string, KnowledgeEdge[]> = new Map();

  constructor(root: string = '.maestro', config?: KnowledgeGraphConfig) {
    this.root = root;
    this.config = config || {};
    
    const memoryRoot = path.join(this.root, 'memory');
    this.graph = new MemoryGraph();
    this.graphFile = this.config.graph_file || path.join(memoryRoot, 'knowledge-graph.json');

    // Load existing graph if available
    this.loadGraph();
  }

  /**
   * Build knowledge graph from memory
   */
  async buildGraph(): Promise<void> {
    // Clear existing graph
    this.nodes.clear();
    this.edges.clear();

    // Add nodes from patterns
    await this.addPatternNodes();

    // Add nodes from sessions
    await this.addSessionNodes();

    // Add nodes from memory entries
    await this.addMemoryNodes();

    // Build relationships
    await this.buildRelationships();

    // Save graph
    this.saveGraph();
  }

  private async addPatternNodes(): Promise<void> {
    const patternsDir = path.join(this.root, 'memory', 'patterns');
    if (!fs.existsSync(patternsDir)) {
      return;
    }

    const patternFiles = fs.readdirSync(patternsDir).filter(f => f.endsWith('.md'));

    for (const file of patternFiles) {
      const filePath = path.join(patternsDir, file);
      const id = `pattern:${file.replace('.md', '')}`;
      
      try {
        const content = fs.readFileSync(filePath, 'utf-8');
        const titleMatch = content.match(/^# Pattern: (.+)$/m);
        const title = titleMatch ? titleMatch[1] : file.replace('.md', '');

        this.nodes.set(id, {
          id,
          type: 'pattern',
          label: title,
          metadata: {
            file_path: filePath,
            category: this.extractCategory(content),
          },
        });

        // Add to memory graph
        this.graph.addNode(filePath, { type: 'pattern', title });
      } catch (error) {
        // Ignore errors
      }
    }
  }

  private async addSessionNodes(): Promise<void> {
    const sessionsDir = path.join(this.root, 'memory', 'sessions');
    if (!fs.existsSync(sessionsDir)) {
      return;
    }

    const sessionFiles = fs.readdirSync(sessionsDir)
      .filter(f => f.endsWith('.md') && f.startsWith('session-'));

    for (const file of sessionFiles) {
      const filePath = path.join(sessionsDir, file);
      const id = `session:${file.replace('.md', '')}`;
      
      try {
        const stats = fs.statSync(filePath);
        this.nodes.set(id, {
          id,
          type: 'session',
          label: file.replace('.md', ''),
          metadata: {
            file_path: filePath,
            timestamp: stats.mtime,
          },
        });

        // Add to memory graph
        this.graph.addNode(filePath, { type: 'session', timestamp: stats.mtime });
      } catch (error) {
        // Ignore errors
      }
    }
  }

  private async addMemoryNodes(): Promise<void> {
    // Add memory entries from memory storage
    // This would integrate with MemoryService
    // For now, we'll scan the memory directory
    const memoryDirs = [
      path.join(this.root, 'memory', 'public'),
      path.join(this.root, 'memory', 'private'),
    ];

    for (const memoryDir of memoryDirs) {
      if (!fs.existsSync(memoryDir)) continue;

      this.scanMemoryDirectory(memoryDir);
    }
  }

  private scanMemoryDirectory(dir: string): void {
    try {
      const entries = fs.readdirSync(dir, { withFileTypes: true });
      
      for (const entry of entries) {
        const fullPath = path.join(dir, entry.name);
        
        if (entry.isDirectory()) {
          this.scanMemoryDirectory(fullPath);
        } else if (entry.isFile() && entry.name.endsWith('.md')) {
          const id = `memory:${path.relative(this.root, fullPath)}`;
          const stats = fs.statSync(fullPath);

          this.nodes.set(id, {
            id,
            type: 'memory',
            label: entry.name.replace('.md', ''),
            metadata: {
              file_path: fullPath,
              timestamp: stats.mtime,
            },
          });

          // Add to memory graph
          this.graph.addNode(fullPath, { type: 'memory', timestamp: stats.mtime });
        }
      }
    } catch (error) {
      // Ignore errors
    }
  }

  private async buildRelationships(): Promise<void> {
    // Build relationships based on:
    // 1. Tag similarity
    // 2. Content similarity
    // 3. Temporal proximity
    // 4. References

    const nodeArray = Array.from(this.nodes.values());

    for (let i = 0; i < nodeArray.length; i++) {
      for (let j = i + 1; j < nodeArray.length; j++) {
        const node1 = nodeArray[i];
        const node2 = nodeArray[j];

        // Calculate relationship weight
        const weight = this.calculateRelationshipWeight(node1, node2);
        
        if (weight > 0.1) {
          // Add edge
          if (!this.edges.has(node1.id)) {
            this.edges.set(node1.id, []);
          }
          this.edges.get(node1.id)!.push({
            from: node1.id,
            to: node2.id,
            weight,
            type: this.determineEdgeType(node1, node2),
          });

          // Add to memory graph
          const filePath1 = node1.metadata?.file_path;
          const filePath2 = node2.metadata?.file_path;
          if (filePath1 && filePath2) {
            this.graph.addEdge(filePath1, filePath2, weight);
          }
        }
      }
    }
  }

  private calculateRelationshipWeight(node1: KnowledgeNode, node2: KnowledgeNode): number {
    let weight = 0.0;

    // Same category increases weight
    if (node1.metadata?.category && node2.metadata?.category) {
      if (node1.metadata.category === node2.metadata.category) {
        weight += 0.3;
      }
    }

    // Same type increases weight
    if (node1.type === node2.type) {
      weight += 0.2;
    }

    // Temporal proximity (sessions close in time)
    if (node1.type === 'session' && node2.type === 'session') {
      const time1 = node1.metadata?.timestamp;
      const time2 = node2.metadata?.timestamp;
      if (time1 && time2) {
        const timeDiff = Math.abs(new Date(time1).getTime() - new Date(time2).getTime());
        const daysDiff = timeDiff / (1000 * 60 * 60 * 24);
        if (daysDiff < 7) {
          weight += 0.2;
        }
      }
    }

    return Math.min(weight, 1.0);
  }

  private determineEdgeType(node1: KnowledgeNode, node2: KnowledgeNode): KnowledgeEdge['type'] {
    if (node1.type === 'pattern' && node2.type === 'session') {
      return 'references';
    }
    if (node1.type === node2.type) {
      return 'similar';
    }
    return 'related';
  }

  /**
   * Query knowledge graph
   */
  queryGraph(nodeId: string, maxResults: number = 10): KnowledgeNode[] {
    const related = this.graph.getRelated(
      this.getFilePath(nodeId),
      maxResults
    );

    const results: KnowledgeNode[] = [];
    for (const [filePath, weight] of related) {
      const node = this.findNodeByFilePath(filePath);
      if (node) {
        results.push(node);
      }
    }

    return results;
  }

  private findNodeByFilePath(filePath: string): KnowledgeNode | undefined {
    for (const node of this.nodes.values()) {
      if (node.metadata?.file_path === filePath) {
        return node;
      }
    }
    return undefined;
  }

  private getFilePath(nodeId: string): string {
    const node = this.nodes.get(nodeId);
    return node?.metadata?.file_path || nodeId;
  }

  private extractCategory(content: string): string {
    const categoryMatch = content.match(/- \*\*Category\*\*: (.+)/);
    return categoryMatch ? categoryMatch[1] : 'unknown';
  }

  /**
   * Get graph data
   */
  getGraphData(): KnowledgeGraphData {
    return {
      nodes: Array.from(this.nodes.values()),
      edges: Array.from(this.edges.values()).flat(),
    };
  }

  /**
   * Save graph to file
   */
  private saveGraph(): void {
    try {
      const data = {
        nodes: Array.from(this.nodes.entries()).map(([id, node]) => ({
          id: node.id,
          type: node.type,
          label: node.label,
          metadata: node.metadata,
        })),
        edges: Array.from(this.edges.values()).flat(),
      };

      fs.writeFileSync(this.graphFile, JSON.stringify(data, null, 2), 'utf-8');
    } catch (error) {
      // Ignore errors
    }
  }

  /**
   * Load graph from file
   */
  private loadGraph(): void {
    if (!fs.existsSync(this.graphFile)) {
      return;
    }

    try {
      const data = JSON.parse(fs.readFileSync(this.graphFile, 'utf-8')) as KnowledgeGraphData;
      
      for (const node of data.nodes) {
        this.nodes.set(node.id, node);
      }

      for (const edge of data.edges) {
        if (!this.edges.has(edge.from)) {
          this.edges.set(edge.from, []);
        }
        this.edges.get(edge.from)!.push(edge);
      }
    } catch (error) {
      // Ignore errors
    }
  }
}

