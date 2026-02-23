/**
 * Memory Storage
 * File-based memory storage with public/private, short-term/mid-term/long-term, and role-based organization
 * Ported from .skmemory/v1/api/skmemory.py
 */

import * as fs from 'fs';
import * as path from 'path';
import { execSync } from 'child_process';

export type MemoryType = 'short-term' | 'mid-term' | 'long-term';
export type MemoryRole = 'dev' | 'ops' | 'sec' | 'ai';

export interface MemoryEntry {
  content: string;
  type: MemoryType;
  role?: MemoryRole;
  tags?: string[];
  public?: boolean;
  filePath: string;
  timestamp: Date;
}

export interface MemoryConfig {
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
  commit?: {
    auto?: boolean;
    message_prefix?: string;
  };
}

/**
 * File-based memory storage manager
 */
export class MemoryStorage {
  private root: string;
  private config: MemoryConfig;

  constructor(root: string, config?: MemoryConfig) {
    this.root = root;
    this.config = config || {};
  }

  /**
   * Get memory root directory path
   */
  private getMemoryRoot(isPublic: boolean = true): string {
    const memoryType = isPublic ? 'public' : 'private';
    const base = this.config.memory_structure?.[memoryType]?.base || 
                 path.join(this.root, 'memory', memoryType);
    return base;
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
    let base = this.getMemoryRoot(isPublic);

    if (role) {
      base = path.join(base, 'roles', role);
    }

    // Determine file path based on type
    if (type === 'short-term') {
      base = path.join(base, 'short-term', 'sessions');
    } else if (type === 'mid-term') {
      base = path.join(base, 'mid-term');
    } else if (type === 'long-term') {
      base = path.join(base, 'long-term');
    } else {
      // Default to mid-term
      base = path.join(base, 'mid-term');
    }

    // Create directory if needed
    if (!fs.existsSync(base)) {
      fs.mkdirSync(base, { recursive: true });
    }

    // Generate filename with timestamp
    const now = new Date();
    const timestamp = now.toISOString().replace(/[:.]/g, '-').slice(0, -5);
    const filename = `${timestamp}.md`;
    const filePath = path.join(base, filename);

    // Build tags string
    let tagsStr = '';
    if (tags && tags.length > 0) {
      const tagsList = tags.map(t => t.startsWith('#') ? t : `#${t}`);
      tagsStr = '\n\nTags: ' + tagsList.join(' ');
    }

    // Write content
    const header = `# Auto-Generated Memory\n\nGenerated: ${now.toISOString()}\n\n`;
    const fullContent = header + content + tagsStr + '\n';
    fs.writeFileSync(filePath, fullContent, 'utf-8');

    // Auto-commit if enabled
    const commitEnabled = autoCommit !== undefined ? autoCommit : 
                         (this.config.commit?.auto !== false);
    if (commitEnabled) {
      try {
        const messagePrefix = this.config.commit?.message_prefix || 'memory: ';
        const commitMsg = `${messagePrefix}${type} memory`;
        const relativePath = path.relative(this.root, filePath);
        
        execSync(`git add "${relativePath}"`, {
          cwd: this.root,
          stdio: 'ignore',
        });
        execSync(`git commit -m "${commitMsg}"`, {
          cwd: this.root,
          stdio: 'ignore',
        });
      } catch (error) {
        // Silently fail if git not available or not a repo
      }
    }

    return filePath;
  }

  /**
   * Read memory entry from file
   */
  read(filePath: string): MemoryEntry | null {
    try {
      if (!fs.existsSync(filePath)) {
        return null;
      }

      const content = fs.readFileSync(filePath, 'utf-8');
      const stats = fs.statSync(filePath);
      
      // Extract tags from content
      const tagsMatch = content.match(/Tags:\s*(.+)/);
      const tags = tagsMatch ? tagsMatch[1].trim().split(/\s+/).map(t => t.replace(/^#/, '')) : [];

      // Determine type and role from path
      const pathParts = filePath.split(path.sep);
      const type = pathParts.includes('short-term') ? 'short-term' :
                   pathParts.includes('long-term') ? 'long-term' : 'mid-term';
      const role = pathParts.includes('roles') ? 
                   (pathParts[pathParts.indexOf('roles') + 1] as MemoryRole) : undefined;
      const isPublic = pathParts.includes('public');

      // Extract content (remove header and tags)
      let extractedContent = content;
      if (extractedContent.startsWith('# Auto-Generated Memory')) {
        const headerEnd = extractedContent.indexOf('\n\n', extractedContent.indexOf('Generated:'));
        if (headerEnd !== -1) {
          extractedContent = extractedContent.substring(headerEnd + 2);
        }
      }
      if (tagsMatch) {
        extractedContent = extractedContent.substring(0, extractedContent.indexOf('Tags:')).trim();
      }

      return {
        content: extractedContent,
        type,
        role,
        tags,
        public: isPublic,
        filePath,
        timestamp: stats.mtime,
      };
    } catch (error) {
      return null;
    }
  }

  /**
   * List memory entries
   */
  list(
    type?: MemoryType,
    role?: MemoryRole,
    isPublic: boolean = true,
    limit?: number
  ): string[] {
    let base = this.getMemoryRoot(isPublic);

    if (role) {
      base = path.join(base, 'roles', role);
    }

    if (type === 'short-term') {
      base = path.join(base, 'short-term', 'sessions');
    } else if (type === 'mid-term') {
      base = path.join(base, 'mid-term');
    } else if (type === 'long-term') {
      base = path.join(base, 'long-term');
    }

    if (!fs.existsSync(base)) {
      return [];
    }

    const files: string[] = [];
    this.collectFiles(base, files, limit);

    // Sort by modification time (newest first)
    files.sort((a, b) => {
      const statsA = fs.statSync(a);
      const statsB = fs.statSync(b);
      return statsB.mtimeMs - statsA.mtimeMs;
    });

    return limit ? files.slice(0, limit) : files;
  }

  private collectFiles(dir: string, files: string[], limit?: number): void {
    if (limit && files.length >= limit) {
      return;
    }

    try {
      const entries = fs.readdirSync(dir, { withFileTypes: true });
      for (const entry of entries) {
        if (limit && files.length >= limit) {
          break;
        }

        const fullPath = path.join(dir, entry.name);
        if (entry.isDirectory()) {
          this.collectFiles(fullPath, files, limit);
        } else if (entry.isFile() && entry.name.endsWith('.md')) {
          files.push(fullPath);
        }
      }
    } catch (error) {
      // Ignore errors
    }
  }

  /**
   * Delete memory entry
   */
  delete(filePath: string): boolean {
    try {
      if (fs.existsSync(filePath)) {
        fs.unlinkSync(filePath);
        return true;
      }
      return false;
    } catch (error) {
      return false;
    }
  }

  /**
   * Get file path for memory entry
   */
  getFilePath(
    type: MemoryType,
    role?: MemoryRole,
    isPublic: boolean = true,
    timestamp?: Date
  ): string {
    let base = this.getMemoryRoot(isPublic);

    if (role) {
      base = path.join(base, 'roles', role);
    }

    if (type === 'short-term') {
      base = path.join(base, 'short-term', 'sessions');
    } else if (type === 'mid-term') {
      base = path.join(base, 'mid-term');
    } else if (type === 'long-term') {
      base = path.join(base, 'long-term');
    }

    if (!fs.existsSync(base)) {
      fs.mkdirSync(base, { recursive: true });
    }

    const now = timestamp || new Date();
    const timestampStr = now.toISOString().replace(/[:.]/g, '-').slice(0, -5);
    const filename = `${timestampStr}.md`;
    return path.join(base, filename);
  }
}

