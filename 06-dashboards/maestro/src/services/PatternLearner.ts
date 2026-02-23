/**
 * Workflow Learning from Patterns Service
 * 
 * Analyzes workflow execution patterns and suggests optimizations.
 */

import { ExecutionResult } from '../workflow/EnhancedWorkflowExecutor';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';
import { StepDefinition } from '../domain/StepDefinition';

export interface ExecutionPattern {
  workflowName: string;
  stepName: string;
  averageDuration: number;
  successRate: number;
  failureReasons: string[];
  dependencies: string[];
  executionOrder: number;
  frequency: number; // How often this step runs
}

export interface OptimizationSuggestion {
  type: 'parallelize' | 'reorder' | 'cache' | 'skip' | 'merge';
  stepName: string;
  reason: string;
  impact: 'low' | 'medium' | 'high';
  confidence: number; // 0-1
  details?: any;
}

/**
 * Pattern Learner
 */
export class PatternLearner {
  private patterns: Map<string, ExecutionPattern[]> = new Map();
  private executionHistory: Array<{
    workflow: WorkflowDefinition;
    result: ExecutionResult;
    duration: number;
    timestamp: Date;
  }> = [];

  /**
   * Record execution
   */
  recordExecution(
    workflow: WorkflowDefinition,
    result: ExecutionResult,
    duration: number
  ): void {
    this.executionHistory.push({
      workflow,
      result,
      duration,
      timestamp: new Date(),
    });

    // Keep only last 1000 executions
    if (this.executionHistory.length > 1000) {
      this.executionHistory.shift();
    }

    // Update patterns
    this.updatePatterns(workflow, result, duration);
  }

  /**
   * Analyze patterns
   */
  analyzePatterns(workflowName: string): ExecutionPattern[] {
    return this.patterns.get(workflowName) || [];
  }

  /**
   * Generate optimization suggestions
   */
  generateSuggestions(workflow: WorkflowDefinition): OptimizationSuggestion[] {
    const suggestions: OptimizationSuggestion[] = [];
    const patterns = this.analyzePatterns(workflow.name);

    // Analyze each step
    for (const step of workflow.steps) {
      const pattern = patterns.find(p => p.stepName === step.name);
      if (!pattern) {
        continue;
      }

      // Suggest parallelization for slow independent steps
      if (pattern.averageDuration > 30000 && pattern.dependencies.length === 0) {
        suggestions.push({
          type: 'parallelize',
          stepName: step.name,
          reason: `Step takes ${(pattern.averageDuration / 1000).toFixed(1)}s on average and has no dependencies`,
          impact: 'high',
          confidence: 0.8,
          details: {
            averageDuration: pattern.averageDuration,
            dependencies: pattern.dependencies,
          },
        });
      }

      // Suggest caching for frequently executed steps
      if (pattern.frequency > 10 && pattern.successRate > 0.9) {
        suggestions.push({
          type: 'cache',
          stepName: step.name,
          reason: `Step runs frequently (${pattern.frequency} times) with high success rate (${(pattern.successRate * 100).toFixed(1)}%)`,
          impact: 'medium',
          confidence: 0.7,
          details: {
            frequency: pattern.frequency,
            successRate: pattern.successRate,
          },
        });
      }

      // Suggest skipping steps that always fail
      if (pattern.successRate < 0.1 && pattern.frequency > 5) {
        suggestions.push({
          type: 'skip',
          stepName: step.name,
          reason: `Step fails ${((1 - pattern.successRate) * 100).toFixed(1)}% of the time`,
          impact: 'medium',
          confidence: 0.6,
          details: {
            successRate: pattern.successRate,
            failureReasons: pattern.failureReasons,
          },
        });
      }
    }

    // Suggest reordering based on dependencies and duration
    const slowSteps = patterns
      .filter(p => p.averageDuration > 20000)
      .sort((a, b) => b.averageDuration - a.averageDuration);

    for (const slowStep of slowSteps) {
      const step = workflow.steps.find(s => s.name === slowStep.stepName);
      if (step && step.dependsOn && step.dependsOn.length > 0) {
        // Check if dependencies are also slow
        const dependencyPatterns = step.dependsOn
          .map(dep => patterns.find(p => p.stepName === dep))
          .filter(p => p && p.averageDuration > slowStep.averageDuration);

        if (dependencyPatterns.length > 0) {
          suggestions.push({
            type: 'reorder',
            stepName: step.name,
            reason: `Step depends on slower steps, consider reordering`,
            impact: 'low',
            confidence: 0.5,
            details: {
              dependencies: step.dependsOn,
              dependencyDurations: dependencyPatterns.map(p => p!.averageDuration),
            },
          });
        }
      }
    }

    return suggestions.sort((a, b) => {
      // Sort by impact and confidence
      const impactOrder = { high: 3, medium: 2, low: 1 };
      const impactDiff = impactOrder[b.impact] - impactOrder[a.impact];
      if (impactDiff !== 0) return impactDiff;
      return b.confidence - a.confidence;
    });
  }

  /**
   * Update patterns
   */
  private updatePatterns(
    workflow: WorkflowDefinition,
    result: ExecutionResult,
    duration: number
  ): void {
    const workflowPatterns = this.patterns.get(workflow.name) || [];

    // Update patterns for each step
    for (let i = 0; i < workflow.steps.length; i++) {
      const step = workflow.steps[i];
      let pattern = workflowPatterns.find(p => p.stepName === step.name);

      if (!pattern) {
        pattern = {
          workflowName: workflow.name,
          stepName: step.name,
          averageDuration: 0,
          successRate: 0,
          failureReasons: [],
          dependencies: step.dependsOn || [],
          executionOrder: i,
          frequency: 0,
        };
        workflowPatterns.push(pattern);
      }

      // Update statistics
      const task = [...result.completedTasks, ...result.failedTasks].find(
        t => t.stepName === step.name
      );

      if (task) {
        pattern.frequency++;
        const taskDuration = task.updatedAt.getTime() - task.createdAt.getTime();

        // Update average duration (exponential moving average)
        pattern.averageDuration = pattern.averageDuration * 0.9 + taskDuration * 0.1;

        // Update success rate
        const wasSuccessful = result.completedTasks.some(t => t.stepName === step.name);
        pattern.successRate = pattern.successRate * 0.9 + (wasSuccessful ? 1 : 0) * 0.1;

        if (!wasSuccessful) {
          // Record failure reason (simplified)
          pattern.failureReasons.push('Execution failed');
          // Keep only last 10 failure reasons
          if (pattern.failureReasons.length > 10) {
            pattern.failureReasons.shift();
          }
        }
      }
    }

    this.patterns.set(workflow.name, workflowPatterns);
  }

  /**
   * Get pattern statistics
   */
  getStatistics(workflowName: string): {
    totalExecutions: number;
    averageDuration: number;
    successRate: number;
    patterns: ExecutionPattern[];
  } {
    const executions = this.executionHistory.filter(
      e => e.workflow.name === workflowName
    );

    const totalDuration = executions.reduce((sum, e) => sum + e.duration, 0);
    const successful = executions.filter(e => e.result.success).length;

    return {
      totalExecutions: executions.length,
      averageDuration: executions.length > 0 ? totalDuration / executions.length : 0,
      successRate: executions.length > 0 ? successful / executions.length : 0,
      patterns: this.analyzePatterns(workflowName),
    };
  }
}
