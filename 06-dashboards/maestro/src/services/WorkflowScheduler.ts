/**
 * Workflow Scheduler Service
 * 
 * Enables cron-like scheduling for workflows.
 */

import * as fs from 'fs';
import * as path from 'path';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';

export interface ScheduleDefinition {
  workflowName: string;
  schedule: string; // Cron expression (e.g., "0 0 * * *" for daily at midnight)
  enabled: boolean;
  timezone?: string; // IANA timezone (e.g., "America/New_York")
  maxConcurrency?: number; // Max concurrent executions
  timeout?: number; // Timeout in milliseconds
}

export interface ScheduledExecution {
  id: string;
  workflowName: string;
  scheduledAt: Date;
  executedAt?: Date;
  status: 'pending' | 'running' | 'completed' | 'failed' | 'skipped';
  result?: any;
}

/**
 * Cron Expression Parser
 */
class CronParser {
  /**
   * Parse cron expression
   */
  static parse(cronExpression: string): {
    minute: number[];
    hour: number[];
    dayOfMonth: number[];
    month: number[];
    dayOfWeek: number[];
  } {
    const parts = cronExpression.trim().split(/\s+/);
    
    if (parts.length !== 5) {
      throw new Error(`Invalid cron expression: ${cronExpression}`);
    }

    return {
      minute: this.parseField(parts[0], 0, 59),
      hour: this.parseField(parts[1], 0, 23),
      dayOfMonth: this.parseField(parts[2], 1, 31),
      month: this.parseField(parts[3], 1, 12),
      dayOfWeek: this.parseField(parts[4], 0, 6),
    };
  }

  /**
   * Parse cron field
   */
  private static parseField(field: string, min: number, max: number): number[] {
    if (field === '*') {
      return Array.from({ length: max - min + 1 }, (_, i) => i + min);
    }

    if (field.includes(',')) {
      return field.split(',').map(v => parseInt(v.trim()));
    }

    if (field.includes('-')) {
      const [start, end] = field.split('-').map(v => parseInt(v.trim()));
      return Array.from({ length: end - start + 1 }, (_, i) => i + start);
    }

    if (field.includes('/')) {
      const [range, step] = field.split('/');
      const start = range === '*' ? min : parseInt(range);
      const stepValue = parseInt(step);
      const result: number[] = [];
      for (let i = start; i <= max; i += stepValue) {
        result.push(i);
      }
      return result;
    }

    return [parseInt(field)];
  }

  /**
   * Check if cron expression matches current time
   */
  static matches(cronExpression: string, date: Date = new Date()): boolean {
    const cron = this.parse(cronExpression);
    const minute = date.getMinutes();
    const hour = date.getHours();
    const dayOfMonth = date.getDate();
    const month = date.getMonth() + 1; // JavaScript months are 0-indexed
    const dayOfWeek = date.getDay();

    return (
      cron.minute.includes(minute) &&
      cron.hour.includes(hour) &&
      cron.dayOfMonth.includes(dayOfMonth) &&
      cron.month.includes(month) &&
      cron.dayOfWeek.includes(dayOfWeek)
    );
  }

  /**
   * Get next execution time
   */
  static getNextExecution(cronExpression: string, from: Date = new Date()): Date {
    const cron = this.parse(cronExpression);
    const next = new Date(from);
    next.setSeconds(0);
    next.setMilliseconds(0);

    // Increment minute until we find a match
    for (let i = 0; i < 60 * 24 * 365; i++) { // Max 1 year ahead
      if (this.matches(cronExpression, next)) {
        return next;
      }
      next.setMinutes(next.getMinutes() + 1);
    }

    throw new Error('Could not find next execution time');
  }
}

/**
 * Workflow Scheduler
 */
export class WorkflowScheduler {
  private schedulesFile: string;
  private executionsFile: string;
  private schedules: Map<string, ScheduleDefinition> = new Map();
  private intervalId?: NodeJS.Timeout;

  constructor(repositoryRoot: string = process.cwd()) {
    this.schedulesFile = path.join(repositoryRoot, '.maestro', 'schedules.json');
    this.executionsFile = path.join(repositoryRoot, '.maestro', 'executions.json');
    this.loadSchedules();
  }

  /**
   * Add schedule
   */
  async addSchedule(schedule: ScheduleDefinition): Promise<void> {
    this.schedules.set(schedule.workflowName, schedule);
    await this.saveSchedules();
  }

  /**
   * Remove schedule
   */
  async removeSchedule(workflowName: string): Promise<void> {
    this.schedules.delete(workflowName);
    await this.saveSchedules();
  }

  /**
   * Get schedule
   */
  getSchedule(workflowName: string): ScheduleDefinition | undefined {
    return this.schedules.get(workflowName);
  }

  /**
   * List schedules
   */
  listSchedules(): ScheduleDefinition[] {
    return Array.from(this.schedules.values());
  }

  /**
   * Start scheduler
   */
  start(onTrigger: (schedule: ScheduleDefinition) => Promise<void>): void {
    if (this.intervalId) {
      return; // Already started
    }

    // Check every minute
    this.intervalId = setInterval(() => {
      const now = new Date();
      for (const schedule of this.schedules.values()) {
        if (!schedule.enabled) {
          continue;
        }

        try {
          if (CronParser.matches(schedule.schedule, now)) {
            // Trigger workflow execution
            onTrigger(schedule).catch(error => {
              console.error(`Failed to trigger scheduled workflow ${schedule.workflowName}:`, error);
            });
          }
        } catch (error) {
          console.error(`Error checking schedule for ${schedule.workflowName}:`, error);
        }
      }
    }, 60000); // Check every minute
  }

  /**
   * Stop scheduler
   */
  stop(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = undefined;
    }
  }

  /**
   * Get next execution times
   */
  getNextExecutions(workflowName?: string): Map<string, Date> {
    const nextExecutions = new Map<string, Date>();
    const now = new Date();

    for (const schedule of this.schedules.values()) {
      if (workflowName && schedule.workflowName !== workflowName) {
        continue;
      }

      if (!schedule.enabled) {
        continue;
      }

      try {
        const next = CronParser.getNextExecution(schedule.schedule, now);
        nextExecutions.set(schedule.workflowName, next);
      } catch (error) {
        console.warn(`Failed to calculate next execution for ${schedule.workflowName}:`, error);
      }
    }

    return nextExecutions;
  }

  /**
   * Load schedules
   */
  private loadSchedules(): void {
    if (!fs.existsSync(this.schedulesFile)) {
      return;
    }

    try {
      const content = fs.readFileSync(this.schedulesFile, 'utf-8');
      const schedules = JSON.parse(content) as ScheduleDefinition[];
      this.schedules = new Map(schedules.map(s => [s.workflowName, s]));
    } catch (error) {
      console.warn('Failed to load schedules:', error);
    }
  }

  /**
   * Save schedules
   */
  private async saveSchedules(): Promise<void> {
    const schedules = Array.from(this.schedules.values());
    await fs.promises.writeFile(
      this.schedulesFile,
      JSON.stringify(schedules, null, 2),
      'utf-8'
    );
  }
}
