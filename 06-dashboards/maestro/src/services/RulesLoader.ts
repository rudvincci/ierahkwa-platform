/**
 * Rules Loader Service
 * 
 * Loads all rules from .cursor/rules/ directory and makes them available
 * for inclusion in agent prompts.
 */

import * as fs from 'fs';
import * as path from 'path';

export interface RuleFile {
  name: string;
  path: string;
  content: string;
}

export class RulesLoader {
  private rulesCache: Map<string, RuleFile[]> | null = null;
  private rulesRoot: string;

  constructor(repositoryRoot: string = process.cwd()) {
    this.rulesRoot = path.join(repositoryRoot, '.cursor', 'rules');
  }

  /**
   * Load all rule files from .cursor/rules/ directory
   */
  async loadRules(): Promise<RuleFile[]> {
    if (this.rulesCache) {
      return Array.from(this.rulesCache.values()).flat();
    }

    const rules: RuleFile[] = [];

    try {
      if (!fs.existsSync(this.rulesRoot)) {
        console.warn(`Rules directory not found: ${this.rulesRoot}`);
        return rules;
      }

      // Load all .md files from rules directory
      const ruleFiles = this.findRuleFiles(this.rulesRoot);
      
      for (const filePath of ruleFiles) {
        try {
          const content = fs.readFileSync(filePath, 'utf-8');
          const relativePath = path.relative(this.rulesRoot, filePath);
          const name = path.basename(filePath, '.md');

          rules.push({
            name,
            path: relativePath,
            content: content.trim(),
          });
        } catch (error) {
          console.warn(`Failed to load rule file ${filePath}:`, error);
        }
      }

      // Cache results
      this.rulesCache = new Map();
      for (const rule of rules) {
        if (!this.rulesCache.has(rule.name)) {
          this.rulesCache.set(rule.name, []);
        }
        this.rulesCache.get(rule.name)!.push(rule);
      }

      return rules;
    } catch (error) {
      console.warn(`Failed to load rules from ${this.rulesRoot}:`, error);
      return rules;
    }
  }

  /**
   * Get rules formatted for inclusion in prompts
   */
  async getFormattedRules(): Promise<string> {
    const rules = await this.loadRules();

    if (rules.length === 0) {
      return '';
    }

    let formatted = `## Cursor Rules\n\n`;
    formatted += `**CRITICAL**: You MUST follow all rules defined in .cursor/rules/ directory.\n\n`;
    formatted += `The following rules apply to all work in this workspace:\n\n`;

    // Group by directory structure
    const rulesByDir = new Map<string, RuleFile[]>();
    
    for (const rule of rules) {
      const dir = path.dirname(rule.path);
      if (!rulesByDir.has(dir)) {
        rulesByDir.set(dir, []);
      }
      rulesByDir.get(dir)!.push(rule);
    }

    // Sort directories (root first, then subdirectories)
    const sortedDirs = Array.from(rulesByDir.keys()).sort((a, b) => {
      if (a === '.') return -1;
      if (b === '.') return 1;
      return a.localeCompare(b);
    });

    for (const dir of sortedDirs) {
      const dirRules = rulesByDir.get(dir)!;
      
      if (dir !== '.') {
        formatted += `### ${dir}/\n\n`;
      }

      // Sort rules by name
      dirRules.sort((a, b) => a.name.localeCompare(b.name));

      for (const rule of dirRules) {
        formatted += `#### ${rule.name}\n\n`;
        formatted += `${rule.content}\n\n`;
        formatted += `---\n\n`;
      }
    }

    formatted += `\n**Remember**: These rules are MANDATORY and must be followed for all work.\n\n`;

    return formatted;
  }

  /**
   * Get a summary of available rules (for verbose output)
   */
  async getRulesSummary(): Promise<string[]> {
    const rules = await this.loadRules();
    return rules.map(r => r.path);
  }

  /**
   * Find all .md rule files recursively
   */
  private findRuleFiles(dir: string): string[] {
    const files: string[] = [];

    try {
      const entries = fs.readdirSync(dir, { withFileTypes: true });

      for (const entry of entries) {
        const fullPath = path.join(dir, entry.name);

        if (entry.isDirectory()) {
          // Recursively search subdirectories
          files.push(...this.findRuleFiles(fullPath));
        } else if (entry.isFile() && entry.name.endsWith('.md')) {
          files.push(fullPath);
        }
      }
    } catch (error) {
      console.warn(`Failed to read directory ${dir}:`, error);
    }

    return files;
  }

  /**
   * Clear cache (useful for testing or reloading)
   */
  clearCache(): void {
    this.rulesCache = null;
  }
}
