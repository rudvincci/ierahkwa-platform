/**
 * Cursor Project Service
 * Service for retrieving project status and metadata
 */

import { CursorApiClient, CursorProjectInfo } from './CursorApiClient';
import * as fs from 'fs';
import * as path from 'path';

export class CursorProjectService {
  private client: CursorApiClient;

  constructor(client: CursorApiClient) {
    this.client = client;
  }

  /**
   * Get project information
   */
  async getProjectInfo(): Promise<CursorProjectInfo | null> {
    return await this.client.getProject();
  }

  /**
   * Get project statistics
   */
  async getProjectStats(): Promise<{
    totalFiles: number;
    totalLines: number;
    languages: Record<string, number>;
    lastModified: Date;
  }> {
    const project = await this.client.getProject();
    if (!project || !project.path) {
      return {
        totalFiles: 0,
        totalLines: 0,
        languages: {},
        lastModified: new Date(),
      };
    }

    const stats = {
      totalFiles: 0,
      totalLines: 0,
      languages: {} as Record<string, number>,
      lastModified: new Date(0),
    };

    this.analyzeDirectory(project.path, stats);

    return stats;
  }

  private analyzeDirectory(dir: string, stats: {
    totalFiles: number;
    totalLines: number;
    languages: Record<string, number>;
    lastModified: Date;
  }): void {
    try {
      const entries = fs.readdirSync(dir, { withFileTypes: true });
      
      for (const entry of entries) {
        // Skip common ignore patterns
        if (entry.name.startsWith('.') ||
            entry.name === 'node_modules' ||
            entry.name === 'dist' ||
            entry.name === 'build' ||
            entry.name === '.git') {
          continue;
        }

        const fullPath = path.join(dir, entry.name);

        if (entry.isDirectory()) {
          this.analyzeDirectory(fullPath, stats);
        } else if (entry.isFile()) {
          stats.totalFiles++;
          
          const ext = path.extname(entry.name);
          const language = this.getLanguageFromExtension(ext);
          stats.languages[language] = (stats.languages[language] || 0) + 1;

          try {
            const fileStats = fs.statSync(fullPath);
            if (fileStats.mtime > stats.lastModified) {
              stats.lastModified = fileStats.mtime;
            }

            // Count lines (for text files)
            if (this.isTextFile(ext)) {
              const content = fs.readFileSync(fullPath, 'utf-8');
              const lines = content.split('\n').length;
              stats.totalLines += lines;
            }
          } catch (error) {
            // Ignore errors
          }
        }
      }
    } catch (error) {
      // Ignore errors
    }
  }

  private getLanguageFromExtension(ext: string): string {
    const languageMap: Record<string, string> = {
      '.ts': 'TypeScript',
      '.tsx': 'TypeScript',
      '.js': 'JavaScript',
      '.jsx': 'JavaScript',
      '.cs': 'C#',
      '.py': 'Python',
      '.java': 'Java',
      '.go': 'Go',
      '.rs': 'Rust',
      '.md': 'Markdown',
      '.yml': 'YAML',
      '.yaml': 'YAML',
      '.json': 'JSON',
      '.html': 'HTML',
      '.css': 'CSS',
      '.scss': 'SCSS',
      '.sql': 'SQL',
      '.sh': 'Shell',
      '.ps1': 'PowerShell',
    };

    return languageMap[ext] || ext.substring(1).toUpperCase() || 'Unknown';
  }

  private isTextFile(ext: string): boolean {
    const textExtensions = ['.ts', '.tsx', '.js', '.jsx', '.cs', '.py', '.java', '.go', '.rs',
                            '.md', '.yml', '.yaml', '.json', '.html', '.css', '.scss', '.sql',
                            '.sh', '.ps1', '.txt', '.xml', '.csv'];
    return textExtensions.includes(ext);
  }
}

