/**
 * Parallel Execution Optimizer
 * 
 * Optimizes parallel task execution with resource-aware scheduling,
 * dynamic concurrency adjustment, and task prioritization.
 */

import { StepDefinition } from '../domain/StepDefinition';
import { AgentTask, TaskStatus } from '../domain/AgentTask';

export interface TaskPriority {
  stepName: string;
  priority: number; // 1-10, higher is more important
  estimatedDuration?: number; // milliseconds
  resourceRequirements?: {
    cpu?: number; // 0-1
    memory?: number; // MB
    io?: 'low' | 'medium' | 'high';
  };
}

export interface ExecutionMetrics {
  stepName: string;
  duration: number;
  success: boolean;
  resourceUsage?: {
    cpu: number;
    memory: number;
  };
}

export interface OptimizationStrategy {
  name: string;
  optimize: (steps: StepDefinition[], metrics: ExecutionMetrics[]) => StepDefinition[];
}

/**
 * Parallel Execution Optimizer
 */
export class ParallelOptimizer {
  private taskPriorities: Map<string, TaskPriority> = new Map();
  private executionMetrics: ExecutionMetrics[] = [];
  private currentConcurrency: number = 1;
  private maxConcurrency: number = 10;

  /**
   * Set task priority
   */
  setPriority(stepName: string, priority: TaskPriority): void {
    this.taskPriorities.set(stepName, priority);
  }

  /**
   * Record execution metrics
   */
  recordMetrics(metrics: ExecutionMetrics): void {
    this.executionMetrics.push(metrics);
    // Keep only last 100 metrics
    if (this.executionMetrics.length > 100) {
      this.executionMetrics.shift();
    }
  }

  /**
   * Optimize step execution order
   */
  optimizeExecutionOrder(steps: StepDefinition[]): StepDefinition[] {
    // Sort by priority (higher first)
    const sorted = [...steps].sort((a, b) => {
      const priorityA = this.taskPriorities.get(a.name)?.priority || 5;
      const priorityB = this.taskPriorities.get(b.name)?.priority || 5;
      return priorityB - priorityA;
    });

    return sorted;
  }

  /**
   * Calculate optimal concurrency based on metrics
   */
  calculateOptimalConcurrency(): number {
    if (this.executionMetrics.length < 5) {
      return this.maxConcurrency; // Not enough data, use max
    }

    // Analyze recent metrics
    const recent = this.executionMetrics.slice(-20);
    const avgDuration = recent.reduce((sum, m) => sum + m.duration, 0) / recent.length;
    const successRate = recent.filter(m => m.success).length / recent.length;

    // If tasks are fast and success rate is high, increase concurrency
    if (avgDuration < 5000 && successRate > 0.9) {
      this.currentConcurrency = Math.min(this.currentConcurrency + 1, this.maxConcurrency);
    }
    // If tasks are slow or success rate is low, decrease concurrency
    else if (avgDuration > 30000 || successRate < 0.7) {
      this.currentConcurrency = Math.max(this.currentConcurrency - 1, 1);
    }

    return this.currentConcurrency;
  }

  /**
   * Group steps by resource requirements for optimal parallel execution
   */
  groupByResourceRequirements(steps: StepDefinition[]): StepDefinition[][] {
    const groups: StepDefinition[][] = [];
    const cpuIntensive: StepDefinition[] = [];
    const memoryIntensive: StepDefinition[] = [];
    const ioIntensive: StepDefinition[] = [];
    const balanced: StepDefinition[] = [];

    for (const step of steps) {
      const priority = this.taskPriorities.get(step.name);
      const requirements = priority?.resourceRequirements;

      if (requirements?.cpu && requirements.cpu > 0.7) {
        cpuIntensive.push(step);
      } else if (requirements?.memory && requirements.memory > 500) {
        memoryIntensive.push(step);
      } else if (requirements?.io === 'high') {
        ioIntensive.push(step);
      } else {
        balanced.push(step);
      }
    }

    // Group by resource type to avoid resource contention
    if (cpuIntensive.length > 0) groups.push(cpuIntensive);
    if (memoryIntensive.length > 0) groups.push(memoryIntensive);
    if (ioIntensive.length > 0) groups.push(ioIntensive);
    if (balanced.length > 0) groups.push(balanced);

    return groups.length > 0 ? groups : [steps];
  }

  /**
   * Estimate execution time for step
   */
  estimateDuration(stepName: string): number {
    const priority = this.taskPriorities.get(stepName);
    if (priority?.estimatedDuration) {
      return priority.estimatedDuration;
    }

    // Use historical data
    const historical = this.executionMetrics.filter(m => m.stepName === stepName);
    if (historical.length > 0) {
      const avg = historical.reduce((sum, m) => sum + m.duration, 0) / historical.length;
      return avg;
    }

    // Default estimate
    return 30000; // 30 seconds
  }

  /**
   * Get optimal batch size for parallel execution
   */
  getOptimalBatchSize(totalSteps: number): number {
    const concurrency = this.calculateOptimalConcurrency();
    // Batch size should be concurrency * 2 for better throughput
    return Math.min(concurrency * 2, totalSteps);
  }

  /**
   * Reset optimizer state
   */
  reset(): void {
    this.executionMetrics = [];
    this.currentConcurrency = 1;
  }
}
