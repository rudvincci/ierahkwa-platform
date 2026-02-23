/**
 * Cursor File Service
 * Service for file operations and change tracking
 */

import { CursorApiClient, CursorFileChange } from './CursorApiClient';
import * as fs from 'fs';
import * as path from 'path';
import { execSync } from 'child_process';

export class CursorFileService {
  private client: CursorApiClient;

  constructor(client: CursorApiClient) {
    this.client = client;
  }

  /**
   * Get file changes
   */
  async getFileChanges(limit: number = 50): Promise<CursorFileChange[]> {
    return await this.client.getFileChanges(limit);
  }

  /**
   * Get file history
   */
  async getFileHistory(filePath: string, limit: number = 20): Promise<Array<{
    commit: string;
    author: string;
    date: Date;
    message: string;
    diff?: string;
  }>> {
    try {
      const output = execSync(
        `git log --pretty=format:"%H|%an|%at|%s" --max-count=${limit} -- "${filePath}"`,
        { encoding: 'utf-8', maxBuffer: 10 * 1024 * 1024 }
      );

      const history: Array<{
        commit: string;
        author: string;
        date: Date;
        message: string;
        diff?: string;
      }> = [];

      const lines = output.split('\n');
      for (const line of lines) {
        if (line.includes('|')) {
          const [commit, author, timestamp, ...messageParts] = line.split('|');
          const message = messageParts.join('|');
          
          history.push({
            commit: commit || '',
            author: author || 'Unknown',
            date: new Date(parseInt(timestamp || '0', 10) * 1000),
            message: message || '',
          });
        }
      }

      return history;
    } catch (error) {
      return [];
    }
  }

  /**
   * Get file diff
   */
  async getFileDiff(filePath: string, commit1?: string, commit2?: string): Promise<string> {
    try {
      let command = 'git diff';
      if (commit1 && commit2) {
        command = `git diff ${commit1} ${commit2} -- "${filePath}"`;
      } else if (commit1) {
        command = `git diff ${commit1} -- "${filePath}"`;
      } else {
        command = `git diff -- "${filePath}"`;
      }

      const output = execSync(command, {
        encoding: 'utf-8',
        maxBuffer: 10 * 1024 * 1024,
      });

      return output;
    } catch (error) {
      return '';
    }
  }

  /**
   * Get recent file changes summary
   */
  async getRecentChangesSummary(days: number = 7): Promise<{
    totalChanges: number;
    filesChanged: number;
    additions: number;
    deletions: number;
    byType: Record<string, number>;
  }> {
    try {
      const since = new Date();
      since.setDate(since.getDate() - days);
      const sinceStr = since.toISOString().split('T')[0];

      const output = execSync(
        `git log --since="${sinceStr}" --stat --pretty=format:"%H"`,
        { encoding: 'utf-8', maxBuffer: 10 * 1024 * 1024 }
      );

      const summary = {
        totalChanges: 0,
        filesChanged: 0,
        additions: 0,
        deletions: 0,
        byType: {} as Record<string, number>,
      };

      const lines = output.split('\n');
      const fileSet = new Set<string>();

      for (const line of lines) {
        // Parse git stat output
        const statMatch = line.match(/(\d+)\s+(\d+)\s+(.+)/);
        if (statMatch) {
          const [, additions, deletions, filePath] = statMatch;
          fileSet.add(filePath);
          summary.additions += parseInt(additions, 10);
          summary.deletions += parseInt(deletions, 10);

          const ext = path.extname(filePath);
          const type = ext || 'other';
          summary.byType[type] = (summary.byType[type] || 0) + 1;
        }
      }

      summary.totalChanges = summary.additions + summary.deletions;
      summary.filesChanged = fileSet.size;

      return summary;
    } catch (error) {
      return {
        totalChanges: 0,
        filesChanged: 0,
        additions: 0,
        deletions: 0,
        byType: {},
      };
    }
  }
}

