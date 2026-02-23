/**
 * Reporting & Analytics Service
 * 
 * Generates comprehensive execution reports, trend analysis, and performance metrics.
 */

import * as fs from 'fs';
import * as path from 'path';
import { AgentTask } from '../domain/AgentTask';
import { ExecutionResult } from '../workflow/EnhancedWorkflowExecutor';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';

export interface ExecutionMetrics {
  workflowName: string;
  startTime: Date;
  endTime?: Date;
  duration: number; // milliseconds
  totalSteps: number;
  completedSteps: number;
  failedSteps: number;
  skippedSteps: number;
  totalTasks: number;
  completedTasks: number;
  failedTasks: number;
  averageTaskDuration: number;
  longestTaskDuration: number;
  shortestTaskDuration: number;
  cacheHits: number;
  cacheMisses: number;
  retryAttempts: number;
  successRate: number;
}

export interface TrendData {
  workflowName: string;
  executions: ExecutionMetrics[];
  averageDuration: number;
  averageSuccessRate: number;
  trend: 'improving' | 'degrading' | 'stable';
  commonFailures: Array<{ stepName: string; count: number }>;
}

export interface ReportOptions {
  format?: 'json' | 'html' | 'markdown';
  includeDetails?: boolean;
  includeTrends?: boolean;
  outputPath?: string;
}

/**
 * Reporting Service
 */
export class ReportingService {
  private reportsDir: string;
  private metricsHistory: Map<string, ExecutionMetrics[]> = new Map();

  constructor(repositoryRoot: string = process.cwd()) {
    this.reportsDir = path.join(repositoryRoot, '.maestro', 'reports');
    this.loadHistory();
  }

  /**
   * Generate execution report
   */
  async generateReport(
    workflow: WorkflowDefinition,
    result: ExecutionResult,
    metrics: Partial<ExecutionMetrics>,
    options: ReportOptions = {}
  ): Promise<string> {
    const fullMetrics: ExecutionMetrics = {
      workflowName: workflow.name,
      startTime: metrics.startTime || new Date(),
      endTime: metrics.endTime || new Date(),
      duration: metrics.duration || 0,
      totalSteps: workflow.steps.length,
      completedSteps: result.completedTasks.length,
      failedSteps: result.failedTasks.length,
      skippedSteps: 0,
      totalTasks: result.completedTasks.length + result.failedTasks.length,
      completedTasks: result.completedTasks.length,
      failedTasks: result.failedTasks.length,
      averageTaskDuration: metrics.averageTaskDuration || 0,
      longestTaskDuration: metrics.longestTaskDuration || 0,
      shortestTaskDuration: metrics.shortestTaskDuration || 0,
      cacheHits: metrics.cacheHits || 0,
      cacheMisses: metrics.cacheMisses || 0,
      retryAttempts: metrics.retryAttempts || 0,
      successRate: result.failedTasks.length === 0 ? 100 : 
        (result.completedTasks.length / (result.completedTasks.length + result.failedTasks.length)) * 100,
    };

    // Save to history
    this.saveToHistory(fullMetrics);

    // Generate report based on format
    const format = options.format || 'markdown';
    let report: string;

    switch (format) {
      case 'json':
        report = this.generateJsonReport(fullMetrics, result, options);
        break;
      case 'html':
        report = this.generateHtmlReport(fullMetrics, result, options);
        break;
      case 'markdown':
      default:
        report = this.generateMarkdownReport(fullMetrics, result, options);
        break;
    }

    // Save report
    if (options.outputPath) {
      await fs.promises.writeFile(options.outputPath, report, 'utf-8');
    } else {
      const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
      const filename = `${workflow.name}-${timestamp}.${format === 'json' ? 'json' : format === 'html' ? 'html' : 'md'}`;
      const filepath = path.join(this.reportsDir, filename);
      await fs.promises.mkdir(this.reportsDir, { recursive: true });
      await fs.promises.writeFile(filepath, report, 'utf-8');
      return filepath;
    }

    return report;
  }

  /**
   * Generate Markdown report
   */
  private generateMarkdownReport(
    metrics: ExecutionMetrics,
    result: ExecutionResult,
    options: ReportOptions
  ): string {
    let report = `# Workflow Execution Report\n\n`;
    report += `**Workflow**: ${metrics.workflowName}\n`;
    report += `**Start Time**: ${metrics.startTime.toISOString()}\n`;
    report += `**End Time**: ${metrics.endTime?.toISOString() || 'N/A'}\n`;
    report += `**Duration**: ${this.formatDuration(metrics.duration)}\n\n`;

    report += `## Summary\n\n`;
    report += `- **Total Steps**: ${metrics.totalSteps}\n`;
    report += `- **Completed**: ${metrics.completedSteps} ✅\n`;
    report += `- **Failed**: ${metrics.failedSteps} ❌\n`;
    report += `- **Success Rate**: ${metrics.successRate.toFixed(2)}%\n\n`;

    report += `## Performance Metrics\n\n`;
    report += `- **Average Task Duration**: ${this.formatDuration(metrics.averageTaskDuration)}\n`;
    report += `- **Longest Task**: ${this.formatDuration(metrics.longestTaskDuration)}\n`;
    report += `- **Shortest Task**: ${this.formatDuration(metrics.shortestTaskDuration)}\n`;
    report += `- **Cache Hits**: ${metrics.cacheHits}\n`;
    report += `- **Cache Misses**: ${metrics.cacheMisses}\n`;
    report += `- **Retry Attempts**: ${metrics.retryAttempts}\n\n`;

    if (options.includeDetails) {
      report += `## Completed Tasks\n\n`;
      result.completedTasks.forEach((task, index) => {
        report += `${index + 1}. **${task.stepName}** (${task.status})\n`;
      });
      report += '\n';

      if (result.failedTasks.length > 0) {
        report += `## Failed Tasks\n\n`;
        result.failedTasks.forEach((task, index) => {
          report += `${index + 1}. **${task.stepName}** (${task.status})\n`;
        });
        report += '\n';
      }
    }

    if (options.includeTrends) {
      const trends = this.getTrends(metrics.workflowName);
      if (trends) {
        report += `## Trends\n\n`;
        report += `- **Average Duration**: ${this.formatDuration(trends.averageDuration)}\n`;
        report += `- **Average Success Rate**: ${trends.averageSuccessRate.toFixed(2)}%\n`;
        report += `- **Trend**: ${trends.trend}\n\n`;

        if (trends.commonFailures.length > 0) {
          report += `### Common Failures\n\n`;
          trends.commonFailures.forEach(failure => {
            report += `- ${failure.stepName}: ${failure.count} failures\n`;
          });
          report += '\n';
        }
      }
    }

    return report;
  }

  /**
   * Generate JSON report
   */
  private generateJsonReport(
    metrics: ExecutionMetrics,
    result: ExecutionResult,
    options: ReportOptions
  ): string {
    const report: any = {
      workflow: metrics.workflowName,
      timestamp: metrics.startTime.toISOString(),
      duration: metrics.duration,
      summary: {
        totalSteps: metrics.totalSteps,
        completed: metrics.completedSteps,
        failed: metrics.failedSteps,
        successRate: metrics.successRate,
      },
      performance: {
        averageTaskDuration: metrics.averageTaskDuration,
        longestTaskDuration: metrics.longestTaskDuration,
        shortestTaskDuration: metrics.shortestTaskDuration,
        cacheHits: metrics.cacheHits,
        cacheMisses: metrics.cacheMisses,
        retryAttempts: metrics.retryAttempts,
      },
    };

    if (options.includeDetails) {
      report.completedTasks = result.completedTasks.map(t => ({
        stepName: t.stepName,
        status: t.status,
      }));
      report.failedTasks = result.failedTasks.map(t => ({
        stepName: t.stepName,
        status: t.status,
      }));
    }

    if (options.includeTrends) {
      const trends = this.getTrends(metrics.workflowName);
      if (trends) {
        report.trends = trends;
      }
    }

    return JSON.stringify(report, null, 2);
  }

  /**
   * Generate HTML report
   */
  private generateHtmlReport(
    metrics: ExecutionMetrics,
    result: ExecutionResult,
    options: ReportOptions
  ): string {
    const markdown = this.generateMarkdownReport(metrics, result, options);
    // Simple HTML wrapper (could use a markdown-to-HTML library)
    return `
<!DOCTYPE html>
<html>
<head>
  <title>Workflow Execution Report - ${metrics.workflowName}</title>
  <style>
    body { font-family: Arial, sans-serif; margin: 40px; }
    h1 { color: #333; }
    h2 { color: #666; margin-top: 30px; }
    table { border-collapse: collapse; width: 100%; }
    th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
    th { background-color: #f2f2f2; }
    .success { color: green; }
    .failure { color: red; }
  </style>
</head>
<body>
  <pre>${markdown}</pre>
</body>
</html>
    `.trim();
  }

  /**
   * Get trend analysis
   */
  getTrends(workflowName: string): TrendData | null {
    const executions = this.metricsHistory.get(workflowName);
    if (!executions || executions.length < 2) {
      return null;
    }

    const recent = executions.slice(-10); // Last 10 executions
    const averageDuration = recent.reduce((sum, m) => sum + m.duration, 0) / recent.length;
    const averageSuccessRate = recent.reduce((sum, m) => sum + m.successRate, 0) / recent.length;

    // Determine trend
    const firstHalf = recent.slice(0, Math.floor(recent.length / 2));
    const secondHalf = recent.slice(Math.floor(recent.length / 2));
    const firstAvg = firstHalf.reduce((sum, m) => sum + m.successRate, 0) / firstHalf.length;
    const secondAvg = secondHalf.reduce((sum, m) => sum + m.successRate, 0) / secondHalf.length;

    let trend: 'improving' | 'degrading' | 'stable' = 'stable';
    if (secondAvg > firstAvg + 5) {
      trend = 'improving';
    } else if (secondAvg < firstAvg - 5) {
      trend = 'degrading';
    }

    // Find common failures
    const failureCounts = new Map<string, number>();
    recent.forEach(m => {
      if (m.failedSteps > 0) {
        // This is simplified - would need step-level failure tracking
        failureCounts.set('unknown', (failureCounts.get('unknown') || 0) + m.failedSteps);
      }
    });

    const commonFailures = Array.from(failureCounts.entries())
      .map(([stepName, count]) => ({ stepName, count }))
      .sort((a, b) => b.count - a.count)
      .slice(0, 5);

    return {
      workflowName,
      executions: recent,
      averageDuration,
      averageSuccessRate,
      trend,
      commonFailures,
    };
  }

  /**
   * Save metrics to history
   */
  private saveToHistory(metrics: ExecutionMetrics): void {
    const history = this.metricsHistory.get(metrics.workflowName) || [];
    history.push(metrics);
    // Keep only last 100 executions
    if (history.length > 100) {
      history.shift();
    }
    this.metricsHistory.set(metrics.workflowName, history);
    this.saveHistory();
  }

  /**
   * Load history from disk
   */
  private loadHistory(): void {
    const historyFile = path.join(this.reportsDir, 'history.json');
    if (fs.existsSync(historyFile)) {
      try {
        const data = fs.readFileSync(historyFile, 'utf-8');
        const history = JSON.parse(data);
        this.metricsHistory = new Map(history);
      } catch (error) {
        console.warn('Failed to load metrics history:', error);
      }
    }
  }

  /**
   * Save history to disk
   */
  private saveHistory(): void {
    const historyFile = path.join(this.reportsDir, 'history.json');
    try {
      fs.mkdirSync(this.reportsDir, { recursive: true });
      const data = JSON.stringify(Array.from(this.metricsHistory.entries()));
      fs.writeFileSync(historyFile, data, 'utf-8');
    } catch (error) {
      console.warn('Failed to save metrics history:', error);
    }
  }

  /**
   * Format duration in human-readable format
   */
  private formatDuration(ms: number): string {
    if (ms < 1000) return `${ms}ms`;
    if (ms < 60000) return `${(ms / 1000).toFixed(2)}s`;
    if (ms < 3600000) return `${(ms / 60000).toFixed(2)}m`;
    return `${(ms / 3600000).toFixed(2)}h`;
  }
}
