/**
 * Session Manager
 * Manages session auto-save and retention
 * Ported from .claudememory configuration
 */

import * as fs from 'fs';
import * as path from 'path';

export interface Session {
  id: string;
  filePath: string;
  timestamp: Date;
  content: string;
  metadata?: Record<string, any>;
}

export interface SessionManagerConfig {
  auto_save?: boolean;
  save_interval?: number; // minutes
  retention_days?: number;
  format?: 'enhanced' | 'basic';
  include_metadata?: boolean;
  sessions_dir?: string;
}

/**
 * Session manager for auto-save and retention
 */
export class SessionManager {
  private root: string;
  private config: SessionManagerConfig;
  private sessionsDir: string;
  private saveInterval?: NodeJS.Timeout;

  constructor(root: string = '.maestro', config?: SessionManagerConfig) {
    this.root = root;
    this.config = config || {};
    this.sessionsDir = this.config.sessions_dir || path.join(this.root, 'memory', 'sessions');

    // Ensure directory exists
    if (!fs.existsSync(this.sessionsDir)) {
      fs.mkdirSync(this.sessionsDir, { recursive: true });
    }

    // Start auto-save if enabled
    if (this.config.auto_save !== false) {
      this.startAutoSave();
    }
  }

  /**
   * Save session
   */
  saveSession(content: string, metadata?: Record<string, any>): string {
    const now = new Date();
    const timestamp = now.toISOString().replace(/[:.]/g, '-').slice(0, -5);
    const sessionId = `session-${timestamp}`;
    const fileName = `${sessionId}.md`;
    const filePath = path.join(this.sessionsDir, fileName);

    let sessionContent = '';

    if (this.config.format === 'enhanced') {
      sessionContent = this.formatEnhancedSession(content, metadata, now);
    } else {
      sessionContent = this.formatBasicSession(content, now);
    }

    fs.writeFileSync(filePath, sessionContent, 'utf-8');
    return filePath;
  }

  private formatEnhancedSession(
    content: string,
    metadata?: Record<string, any>,
    timestamp?: Date
  ): string {
    const now = timestamp || new Date();
    const parts: string[] = [];

    parts.push('# Session Summary');
    parts.push('');
    parts.push(`**Date**: ${now.toISOString()}`);
    parts.push('');

    if (metadata && this.config.include_metadata !== false) {
      parts.push('## Metadata');
      parts.push('');
      for (const [key, value] of Object.entries(metadata)) {
        parts.push(`- **${key}**: ${value}`);
      }
      parts.push('');
    }

    parts.push('## Context Summary');
    parts.push('');
    parts.push(content);
    parts.push('');

    return parts.join('\n');
  }

  private formatBasicSession(content: string, timestamp?: Date): string {
    const now = timestamp || new Date();
    return `# Session\n\n**Date**: ${now.toISOString()}\n\n${content}\n`;
  }

  /**
   * Get latest session
   */
  getLatestSession(): Session | null {
    const files = this.getSessionFiles();
    if (files.length === 0) {
      return null;
    }

    // Sort by modification time (newest first)
    files.sort((a, b) => {
      const statsA = fs.statSync(a);
      const statsB = fs.statSync(b);
      return statsB.mtimeMs - statsA.mtimeMs;
    });

    const latestFile = files[0];
    try {
      const content = fs.readFileSync(latestFile, 'utf-8');
      const stats = fs.statSync(latestFile);
      const id = path.basename(latestFile, '.md');

      return {
        id,
        filePath: latestFile,
        timestamp: stats.mtime,
        content,
      };
    } catch (error) {
      return null;
    }
  }

  /**
   * Get all sessions
   */
  getSessions(limit?: number): Session[] {
    const files = this.getSessionFiles();
    
    // Sort by modification time (newest first)
    files.sort((a, b) => {
      const statsA = fs.statSync(a);
      const statsB = fs.statSync(b);
      return statsB.mtimeMs - statsA.mtimeMs;
    });

    const limitedFiles = limit ? files.slice(0, limit) : files;
    const sessions: Session[] = [];

    for (const file of limitedFiles) {
      try {
        const content = fs.readFileSync(file, 'utf-8');
        const stats = fs.statSync(file);
        const id = path.basename(file, '.md');

        sessions.push({
          id,
          filePath: file,
          timestamp: stats.mtime,
          content,
        });
      } catch (error) {
        // Ignore errors
      }
    }

    return sessions;
  }

  /**
   * Clean up old sessions based on retention policy
   */
  cleanupOldSessions(): number {
    const retentionDays = this.config.retention_days || 90;
    const cutoffDate = new Date();
    cutoffDate.setDate(cutoffDate.getDate() - retentionDays);

    const files = this.getSessionFiles();
    let deletedCount = 0;

    for (const file of files) {
      try {
        const stats = fs.statSync(file);
        if (stats.mtime < cutoffDate) {
          fs.unlinkSync(file);
          deletedCount++;
        }
      } catch (error) {
        // Ignore errors
      }
    }

    return deletedCount;
  }

  private getSessionFiles(): string[] {
    if (!fs.existsSync(this.sessionsDir)) {
      return [];
    }

    return fs.readdirSync(this.sessionsDir)
      .filter(f => f.endsWith('.md') && f.startsWith('session-'))
      .map(f => path.join(this.sessionsDir, f));
  }

  private startAutoSave(): void {
    const intervalMinutes = this.config.save_interval || 15;
    const intervalMs = intervalMinutes * 60 * 1000;

    // Auto-save runs periodically (would be triggered by external events)
    // This is just the setup - actual saving would be triggered by workflow events
    this.saveInterval = setInterval(() => {
      // Auto-save would be triggered here
      // For now, we just ensure the interval is set
    }, intervalMs);
  }

  /**
   * Stop auto-save
   */
  stopAutoSave(): void {
    if (this.saveInterval) {
      clearInterval(this.saveInterval);
      this.saveInterval = undefined;
    }
  }

  /**
   * Dispose resources
   */
  dispose(): void {
    this.stopAutoSave();
  }
}

