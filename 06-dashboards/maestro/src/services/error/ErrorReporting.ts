/**
 * Error Reporting
 * Error reporting and alerting system
 */

import { ErrorReport, ErrorHandler } from './ErrorHandler';
import * as fs from 'fs';
import * as path from 'path';

export interface AlertConfig {
  enabled: boolean;
  threshold: number; // Number of errors before alerting
  timeWindow: number; // Time window in milliseconds
  channels: ('console' | 'file' | 'webhook')[];
  webhookUrl?: string;
}

/**
 * Error reporting and alerting
 */
export class ErrorReporting {
  private handler: ErrorHandler;
  private config: AlertConfig;
  private errorCounts: Map<string, number> = new Map();
  private errorTimestamps: Map<string, number[]> = new Map();
  private reportsDir: string;

  constructor(handler: ErrorHandler, config?: Partial<AlertConfig>, reportsDir?: string) {
    this.handler = handler;
    this.config = {
      enabled: true,
      threshold: 10,
      timeWindow: 60 * 1000, // 1 minute
      channels: ['console'],
      ...config,
    };
    this.reportsDir = reportsDir || path.join(process.cwd(), '.maestro', 'reports', 'errors');
    if (!fs.existsSync(this.reportsDir)) {
      fs.mkdirSync(this.reportsDir, { recursive: true });
    }
  }

  /**
   * Report error
   */
  reportError(error: Error, context?: Record<string, any>): void {
    const errorContext = this.handler.handleError(error, context);
    const errorKey = `${errorContext.category}:${errorContext.type}`;

    // Track error counts
    this.errorCounts.set(errorKey, (this.errorCounts.get(errorKey) || 0) + 1);

    // Track error timestamps
    if (!this.errorTimestamps.has(errorKey)) {
      this.errorTimestamps.set(errorKey, []);
    }
    this.errorTimestamps.get(errorKey)!.push(Date.now());

    // Check if alert threshold is reached
    if (this.config.enabled && this.shouldAlert(errorKey)) {
      this.sendAlert(errorContext, errorKey);
    }

    // Save error report
    this.saveErrorReport(errorContext);
  }

  /**
   * Check if alert should be sent
   */
  private shouldAlert(errorKey: string): boolean {
    const timestamps = this.errorTimestamps.get(errorKey) || [];
    const now = Date.now();
    const windowStart = now - this.config.timeWindow;

    // Filter timestamps within time window
    const recentErrors = timestamps.filter(ts => ts >= windowStart);

    return recentErrors.length >= this.config.threshold;
  }

  /**
   * Send alert
   */
  private sendAlert(errorContext: any, errorKey: string): void {
    const alert = {
      errorKey,
      errorContext,
      count: this.errorCounts.get(errorKey) || 0,
      timestamp: new Date().toISOString(),
    };

    for (const channel of this.config.channels) {
      switch (channel) {
        case 'console':
          console.error('🚨 ERROR ALERT:', alert);
          break;
        case 'file':
          this.writeAlertToFile(alert);
          break;
        case 'webhook':
          if (this.config.webhookUrl) {
            this.sendWebhookAlert(alert);
          }
          break;
      }
    }
  }

  /**
   * Write alert to file
   */
  private writeAlertToFile(alert: any): void {
    try {
      const filename = `alert-${Date.now()}.json`;
      const filepath = path.join(this.reportsDir, filename);
      fs.writeFileSync(filepath, JSON.stringify(alert, null, 2), 'utf-8');
    } catch (error) {
      // Ignore errors
    }
  }

  /**
   * Send webhook alert
   */
  private async sendWebhookAlert(alert: any): Promise<void> {
    try {
      await fetch(this.config.webhookUrl!, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(alert),
      });
    } catch (error) {
      // Ignore errors
    }
  }

  /**
   * Save error report
   */
  private saveErrorReport(errorContext: any): void {
    try {
      const filename = `error-${Date.now()}.json`;
      const filepath = path.join(this.reportsDir, filename);
      fs.writeFileSync(filepath, JSON.stringify(errorContext, null, 2), 'utf-8');
    } catch (error) {
      // Ignore errors
    }
  }

  /**
   * Get error statistics
   */
  getErrorStatistics(): {
    totalErrors: number;
    errorsByCategory: Record<string, number>;
    errorsByType: Record<string, number>;
    recentErrors: number;
  } {
    const reports = this.handler.getErrorReports();
    const now = Date.now();
    const oneHourAgo = now - 60 * 60 * 1000;

    const errorsByCategory: Record<string, number> = {};
    const errorsByType: Record<string, number> = {};
    let recentErrors = 0;

    for (const report of reports) {
      const category = report.error.category;
      const type = report.error.type;

      errorsByCategory[category] = (errorsByCategory[category] || 0) + 1;
      errorsByType[type] = (errorsByType[type] || 0) + 1;

      if (report.reportedAt.getTime() >= oneHourAgo) {
        recentErrors++;
      }
    }

    return {
      totalErrors: reports.length,
      errorsByCategory,
      errorsByType,
      recentErrors,
    };
  }
}

