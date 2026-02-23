/**
 * Smart Context Loader
 * Intelligently loads relevant context based on triggers and limits
 * Ported from .claudememory/scripts/smart-context-loader.sh
 */

import * as fs from 'fs';
import * as path from 'path';
import * as yaml from 'js-yaml';

export type ContextMode = 'minimal' | 'smart' | 'full' | 'off';
export type ContextTrigger = 'conversation_start' | `keyword:${string}` | `file_pattern:${string}`;

export interface ContextRule {
  trigger: string;
  description?: string;
  load?: string[];
  search?: {
    query: string;
    sources: string[];
    return?: 'summaries_only' | 'full';
    max_results?: number;
  };
  max_size?: string;
  enabled?: boolean;
}

export interface ContextConfig {
  mode?: ContextMode;
  smart_rules?: ContextRule[];
  limits?: {
    max_context_size?: string;
    max_load_time?: string;
    max_search_time?: string;
    max_files_loaded?: number;
    max_search_results?: number;
  };
  sessions?: {
    auto_save?: boolean;
    save_interval?: number;
    retention_days?: number;
    format?: 'enhanced' | 'basic';
    include_metadata?: boolean;
  };
  patterns?: {
    auto_extract?: boolean;
    min_frequency?: number;
    categories?: string[];
  };
  skmemory?: {
    enabled?: boolean;
    path?: string;
    use_search?: boolean;
    use_indexes?: boolean;
  };
}

export interface LoadedContext {
  content: string;
  files_loaded: number;
  total_size: number;
  max_size: number;
  trigger: string;
  mode: ContextMode;
  timestamp: Date;
}

/**
 * Smart context loader with trigger-based rules
 */
export class ContextLoader {
  private root: string;
  private config: ContextConfig;
  private skmemoryPath?: string;

  constructor(root: string = '.maestro', configPath?: string) {
    this.root = root;
    this.config = this.loadConfig(configPath);
    
    // Determine SKMemory path
    if (this.config.skmemory?.path) {
      this.skmemoryPath = this.config.skmemory.path;
    } else {
      // Try default location
      const defaultPath = path.join(this.root, '..', '.skmemory', 'v1');
      if (fs.existsSync(defaultPath)) {
        this.skmemoryPath = defaultPath;
      }
    }
  }

  private loadConfig(configPath?: string): ContextConfig {
    if (!configPath) {
      configPath = path.join(this.root, 'config', 'claude-context.yml');
    }

    if (fs.existsSync(configPath)) {
      try {
        const data = fs.readFileSync(configPath, 'utf-8');
        return yaml.load(data) as ContextConfig;
      } catch (error) {
        // Return default config
      }
    }

    return {
      mode: 'smart',
      limits: {
        max_context_size: '50KB',
        max_load_time: '1000ms',
        max_search_time: '500ms',
        max_files_loaded: 10,
        max_search_results: 10,
      },
    };
  }

  /**
   * Load context based on trigger
   */
  async loadContext(trigger: ContextTrigger, contextFiles?: string[]): Promise<LoadedContext> {
    const mode = this.config.mode || 'smart';

    if (mode === 'off') {
      return {
        content: '',
        files_loaded: 0,
        total_size: 0,
        max_size: 0,
        trigger,
        mode,
        timestamp: new Date(),
      };
    }

    const maxSizeStr = this.config.limits?.max_context_size || '50KB';
    const maxSize = this.parseSize(maxSizeStr);
    const maxFiles = this.config.limits?.max_files_loaded || 10;

    let currentSize = 0;
    let filesLoaded = 0;
    const contentParts: string[] = [];

    // Add header
    contentParts.push('# Claude Code Context');
    contentParts.push('');
    contentParts.push(`**Loaded**: ${new Date().toISOString()}`);
    contentParts.push(`**Trigger**: ${trigger}`);
    contentParts.push(`**Mode**: ${mode}`);
    contentParts.push('');
    contentParts.push('---');
    contentParts.push('');

    // Load based on trigger
    if (trigger === 'conversation_start') {
      await this.loadConversationStart(contentParts, maxSize, currentSize, filesLoaded, maxFiles);
    } else if (trigger.startsWith('keyword:')) {
      const keywords = trigger.replace('keyword:', '');
      await this.loadKeywordContext(keywords, contentParts, maxSize, currentSize, filesLoaded, maxFiles);
    } else if (trigger.startsWith('file_pattern:')) {
      const pattern = trigger.replace('file_pattern:', '');
      await this.loadFilePatternContext(pattern, contentParts, maxSize, currentSize, filesLoaded, maxFiles);
    }

    // Apply smart rules if in smart mode
    if (mode === 'smart' && this.config.smart_rules) {
      for (const rule of this.config.smart_rules) {
        if (!rule.enabled) continue;
        if (this.matchesTrigger(trigger, rule.trigger)) {
          await this.applyRule(rule, contentParts, maxSize, currentSize, filesLoaded, maxFiles);
        }
      }
    }

    // Add footer
    contentParts.push('');
    contentParts.push('---');
    contentParts.push('');
    contentParts.push('**Context Summary**:');
    contentParts.push(`- Files loaded: ${filesLoaded}`);
    contentParts.push(`- Total size: ${Math.floor(currentSize / 1024)}KB / ${maxSizeStr}`);
    contentParts.push(`- Performance: ${new Date().toISOString()}`);

    return {
      content: contentParts.join('\n'),
      files_loaded: filesLoaded,
      total_size: currentSize,
      max_size: maxSize,
      trigger,
      mode,
      timestamp: new Date(),
    };
  }

  private async loadConversationStart(
    contentParts: string[],
    maxSize: number,
    currentSize: number,
    filesLoaded: number,
    maxFiles: number
  ): Promise<void> {
    // Load latest session
    const sessionDir = path.join(this.root, 'memory', 'sessions');
    if (fs.existsSync(sessionDir)) {
      const files = fs.readdirSync(sessionDir)
        .filter(f => f.endsWith('.md'))
        .map(f => ({
          name: f,
          path: path.join(sessionDir, f),
          mtime: fs.statSync(path.join(sessionDir, f)).mtimeMs,
        }))
        .sort((a, b) => b.mtime - a.mtime);

      if (files.length > 0 && filesLoaded < maxFiles) {
        const latestSession = files[0];
        const sessionContent = this.loadFileWithLimit(latestSession.path, maxSize, currentSize);
        if (sessionContent) {
          contentParts.push('## Latest Session');
          contentParts.push('');
          contentParts.push(sessionContent);
          contentParts.push('');
          currentSize += fs.statSync(latestSession.path).size;
          filesLoaded++;
        }
      }
    }

    // Load project overview from SKMemory if available
    if (this.skmemoryPath && filesLoaded < maxFiles) {
      const projectOverview = path.join(
        this.skmemoryPath,
        'memory',
        'public',
        'roles',
        'dev',
        'long-term',
        'project-overview.md'
      );
      if (fs.existsSync(projectOverview)) {
        const overviewContent = this.loadFileWithLimit(projectOverview, maxSize, currentSize);
        if (overviewContent) {
          contentParts.push('## Project Overview');
          contentParts.push('');
          contentParts.push(overviewContent);
          contentParts.push('');
          currentSize += fs.statSync(projectOverview).size;
          filesLoaded++;
        }
      }
    }
  }

  private async loadKeywordContext(
    keywords: string,
    contentParts: string[],
    maxSize: number,
    currentSize: number,
    filesLoaded: number,
    maxFiles: number
  ): Promise<void> {
    contentParts.push(`## Context for: ${keywords}`);
    contentParts.push('');

    // Search SKMemory if available
    if (this.skmemoryPath && this.config.skmemory?.use_search) {
      // Use MemoryService to search
      // This would integrate with the MemoryService we created
      // For now, we'll use a simple file search
      const keywordArray = keywords.split(',').map(k => k.trim());
      for (const keyword of keywordArray) {
        if (filesLoaded >= maxFiles || currentSize >= maxSize) break;
        
        // Search in patterns
        const patternsDir = path.join(this.root, 'memory', 'patterns');
        if (fs.existsSync(patternsDir)) {
          const patternFiles = fs.readdirSync(patternsDir)
            .filter(f => f.endsWith('.md'))
            .map(f => path.join(patternsDir, f));

          for (const patternFile of patternFiles) {
            if (filesLoaded >= maxFiles || currentSize >= maxSize) break;
            
            const content = fs.readFileSync(patternFile, 'utf-8');
            if (content.toLowerCase().includes(keyword.toLowerCase())) {
              const fileContent = this.loadFileWithLimit(patternFile, maxSize, currentSize);
              if (fileContent) {
                contentParts.push(`### ${path.basename(patternFile)}`);
                contentParts.push('');
                contentParts.push(fileContent);
                contentParts.push('');
                currentSize += fs.statSync(patternFile).size;
                filesLoaded++;
              }
            }
          }
        }
      }
    }
  }

  private async loadFilePatternContext(
    pattern: string,
    contentParts: string[],
    maxSize: number,
    currentSize: number,
    filesLoaded: number,
    maxFiles: number
  ): Promise<void> {
    contentParts.push(`## Context for file pattern: ${pattern}`);
    contentParts.push('');

    // Load role-specific memory based on pattern
    if (pattern.includes('.cs') || pattern.includes('.csproj')) {
      // C# development
      if (this.skmemoryPath) {
        const summariesDir = path.join(
          this.skmemoryPath,
          'memory',
          'public',
          'roles',
          'dev',
          'mid-term',
          'summaries'
        );
        if (fs.existsSync(summariesDir)) {
          const summaryFiles = fs.readdirSync(summariesDir)
            .filter(f => f.includes('csharp') || f.includes('dotnet'))
            .slice(0, 3);

          for (const summaryFile of summaryFiles) {
            if (filesLoaded >= maxFiles || currentSize >= maxSize) break;
            const filePath = path.join(summariesDir, summaryFile);
            const fileContent = this.loadFileWithLimit(filePath, maxSize, currentSize);
            if (fileContent) {
              contentParts.push(fileContent);
              contentParts.push('');
              currentSize += fs.statSync(filePath).size;
              filesLoaded++;
            }
          }
        }
      }
    }
  }

  private matchesTrigger(trigger: ContextTrigger, ruleTrigger: string): boolean {
    if (ruleTrigger === trigger) return true;
    if (ruleTrigger.startsWith('keyword:') && trigger.startsWith('keyword:')) {
      const ruleKeywords = ruleTrigger.replace('keyword:', '').split(',');
      const triggerKeywords = trigger.replace('keyword:', '').split(',');
      return ruleKeywords.some(rk => triggerKeywords.some(tk => tk.includes(rk) || rk.includes(tk)));
    }
    if (ruleTrigger.startsWith('file_pattern:') && trigger.startsWith('file_pattern:')) {
      const rulePattern = ruleTrigger.replace('file_pattern:', '');
      const triggerPattern = trigger.replace('file_pattern:', '');
      return triggerPattern.includes(rulePattern) || rulePattern.includes(triggerPattern);
    }
    return false;
  }

  private async applyRule(
    rule: ContextRule,
    contentParts: string[],
    maxSize: number,
    currentSize: number,
    filesLoaded: number,
    maxFiles: number
  ): Promise<void> {
    // Load files specified in rule
    if (rule.load) {
      for (const filePath of rule.load) {
        if (filesLoaded >= maxFiles || currentSize >= maxSize) break;
        
        const resolvedPath = this.resolvePath(filePath);
        if (resolvedPath && fs.existsSync(resolvedPath)) {
          const fileContent = this.loadFileWithLimit(resolvedPath, maxSize, currentSize);
          if (fileContent) {
            contentParts.push(fileContent);
            contentParts.push('');
            currentSize += fs.statSync(resolvedPath).size;
            filesLoaded++;
          }
        }
      }
    }

    // Perform search if specified
    if (rule.search) {
      // This would integrate with MemoryService search
      // For now, we'll skip search in rules
    }
  }

  private loadFileWithLimit(filePath: string, maxSize: number, currentSize: number): string | null {
    try {
      const stats = fs.statSync(filePath);
      if (currentSize + stats.size > maxSize) {
        return null;
      }

      const content = fs.readFileSync(filePath, 'utf-8');
      return content;
    } catch (error) {
      return null;
    }
  }

  private resolvePath(filePath: string): string | null {
    // Resolve relative paths
    if (filePath.startsWith('../')) {
      return path.resolve(this.root, filePath);
    }
    if (path.isAbsolute(filePath)) {
      return filePath;
    }
    return path.join(this.root, filePath);
  }

  private parseSize(sizeStr: string): number {
    const match = sizeStr.match(/^(\d+)(KB|MB|GB)?$/i);
    if (!match) return 50 * 1024; // Default 50KB

    const value = parseInt(match[1], 10);
    const unit = (match[2] || 'KB').toUpperCase();

    switch (unit) {
      case 'KB':
        return value * 1024;
      case 'MB':
        return value * 1024 * 1024;
      case 'GB':
        return value * 1024 * 1024 * 1024;
      default:
        return value * 1024;
    }
  }
}

