/**
 * Task Progress Tracker
 * 
 * Tracks task progress and updates workflow tasks dynamically
 */

import { AgentTask } from '../domain/AgentTask';
import { OutputAnalysis } from './OutputMonitor';
import { AgentResult } from '../workflow/OrchestratorContext';

export interface TaskProgress {
  stepName: string;
  workflowName: string;
  completionPercent: number;
  status: 'pending' | 'running' | 'completed' | 'failed' | 'needs-update';
  estimatedTimeRemaining?: number; // seconds
  issues: string[];
  subTasks: string[]; // Identified sub-tasks
  completedSubTasks: string[];
  lastUpdate: Date;
}

export interface TaskUpdate {
  stepName: string;
  workflowName: string;
  updates: {
    description?: string;
    expectedOutput?: string;
    dependencies?: string[];
    agent?: string;
    model?: string;
  };
  reason: string;
}

export class TaskProgressTracker {
  private progress: Map<string, Map<string, TaskProgress>> = new Map(); // workflowName -> stepName -> progress
  private taskUpdates: Map<string, TaskUpdate[]> = new Map(); // workflowName -> updates

  /**
   * Initialize progress tracking for a workflow
   */
  initializeWorkflow(workflowName: string, tasks: AgentTask[]): void {
    if (!this.progress.has(workflowName)) {
      this.progress.set(workflowName, new Map());
    }

    const workflowProgress = this.progress.get(workflowName)!;

    for (const task of tasks) {
      if (!workflowProgress.has(task.stepName)) {
        workflowProgress.set(task.stepName, {
          stepName: task.stepName,
          workflowName,
          completionPercent: 0,
          status: 'pending',
          issues: [],
          subTasks: [],
          completedSubTasks: [],
          lastUpdate: new Date(),
        });
      }
    }
  }

  /**
   * Update progress based on output analysis
   */
  updateProgress(
    workflowName: string,
    stepName: string,
    analysis: OutputAnalysis,
    result?: AgentResult
  ): TaskProgress {
    const workflowProgress = this.progress.get(workflowName);
    if (!workflowProgress) {
      throw new Error(`Workflow ${workflowName} not initialized`);
    }

    let taskProgress = workflowProgress.get(stepName);
    if (!taskProgress) {
      taskProgress = {
        stepName,
        workflowName,
        completionPercent: 0,
        status: 'running',
        issues: [],
        subTasks: [],
        completedSubTasks: [],
        lastUpdate: new Date(),
      };
      workflowProgress.set(stepName, taskProgress);
    }

    // Update completion percentage
    taskProgress.completionPercent = analysis.completionPercent;

    // Update status
    if (analysis.completionPercent >= 100) {
      taskProgress.status = 'completed';
    } else if (analysis.completionPercent >= 80) {
      taskProgress.status = analysis.needsReAlignment ? 'needs-update' : 'running';
    } else if (analysis.issues.length > 0) {
      taskProgress.status = 'needs-update';
    } else {
      taskProgress.status = 'running';
    }

    // Update issues
    taskProgress.issues = [...analysis.issues];

    // Extract sub-tasks from result
    if (result) {
      const content = result.rawOutput || result.summary || '';
      const subTaskPatterns = [
        /- \[ \] (.+)/g, // Markdown checkboxes
        /TODO: (.+)/gi,
        /FIXME: (.+)/gi,
        /sub-task: (.+)/gi,
        /next: (.+)/gi,
      ];

      const foundSubTasks = new Set<string>();
      for (const pattern of subTaskPatterns) {
        let match;
        while ((match = pattern.exec(content)) !== null) {
          foundSubTasks.add(match[1].trim());
        }
      }

      taskProgress.subTasks = Array.from(foundSubTasks);

      // Check for completed sub-tasks
      const completedPatterns = [
        /- \[x\] (.+)/gi,
        /DONE: (.+)/gi,
        /COMPLETED: (.+)/gi,
      ];

      const completed = new Set<string>();
      for (const pattern of completedPatterns) {
        let match;
        while ((match = pattern.exec(content)) !== null) {
          completed.add(match[1].trim());
        }
      }

      taskProgress.completedSubTasks = Array.from(completed);
    }

    taskProgress.lastUpdate = new Date();

    return taskProgress;
  }

  /**
   * Generate task update recommendations
   */
  generateTaskUpdate(
    workflowName: string,
    stepName: string,
    analysis: OutputAnalysis,
    currentTask: AgentTask
  ): TaskUpdate | null {
    if (!analysis.needsReAlignment && analysis.completionPercent >= 80) {
      return null; // No update needed
    }

    const updates: TaskUpdate['updates'] = {};
    const reasons: string[] = [];

    // Check if agent should change
    if (analysis.recommendedAgent) {
      updates.agent = typeof analysis.recommendedAgent === 'string' 
        ? analysis.recommendedAgent 
        : analysis.recommendedAgent.name || '';
      reasons.push(`Output suggests ${updates.agent} agent would be more appropriate`);
    }

    // Check if model should change
    if (analysis.recommendedModel) {
      updates.model = analysis.recommendedModel;
      reasons.push(`Model ${updates.model} recommended for better context handling`);
    }

    // Check if description needs update
    if (analysis.issues.length > 0) {
      reasons.push(`Issues detected: ${analysis.issues.join(', ')}`);
    }

    if (reasons.length === 0) {
      return null;
    }

    const update: TaskUpdate = {
      stepName,
      workflowName,
      updates,
      reason: reasons.join('; '),
    };

    // Record update
    if (!this.taskUpdates.has(workflowName)) {
      this.taskUpdates.set(workflowName, []);
    }
    this.taskUpdates.get(workflowName)!.push(update);

    return update;
  }

  /**
   * Get progress for a step
   */
  getProgress(workflowName: string, stepName: string): TaskProgress | null {
    return this.progress.get(workflowName)?.get(stepName) || null;
  }

  /**
   * Get all progress for a workflow
   */
  getAllProgress(workflowName: string): Map<string, TaskProgress> {
    return this.progress.get(workflowName) || new Map();
  }

  /**
   * Get task updates for a workflow
   */
  getTaskUpdates(workflowName: string): TaskUpdate[] {
    return this.taskUpdates.get(workflowName) || [];
  }

  /**
   * Clear progress for a workflow
   */
  clearWorkflow(workflowName: string): void {
    this.progress.delete(workflowName);
    this.taskUpdates.delete(workflowName);
  }
}
