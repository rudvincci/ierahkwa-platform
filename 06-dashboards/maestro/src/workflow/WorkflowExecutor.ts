import { WorkflowDefinition, OrchestrationConfig } from '../domain/WorkflowDefinition';
import { AgentTask, TaskStatus } from '../domain/AgentTask';
import { DependencyResolver } from './DependencyResolver';
import { OrchestratorContext, AgentResult } from './OrchestratorContext';
import { IAgentRunner } from '../agents/AgentRunner';
import { AgentRole } from '../domain/AgentRole';
import { v4 as uuidv4 } from 'uuid';

export interface ExecutionResult {
  success: boolean;
  completedTasks: AgentTask[];
  failedTasks: AgentTask[];
}

export class WorkflowExecutor {
  private resolver: DependencyResolver;
  private config?: OrchestrationConfig;

  constructor(config?: OrchestrationConfig) {
    this.resolver = new DependencyResolver();
    this.config = config;
  }

  async execute(
    workflow: WorkflowDefinition,
    runner: IAgentRunner,
    context: OrchestratorContext,
    onTaskUpdate?: (task: AgentTask) => void
  ): Promise<ExecutionResult> {
    // Resolve dependencies
    const orderedSteps = this.resolver.resolve(workflow.steps);

    // Create tasks
    const tasks = orderedSteps.map((step) => {
      const role = this.getRole(step.agent, context);
      return this.createTask(step, workflow.name, role);
    });

    const completedTasks: AgentTask[] = [];
    const failedTasks: AgentTask[] = [];

    // Execute tasks in order
    for (const task of tasks) {
      try {
        // Update task status
        task.status = TaskStatus.Running;
        task.updatedAt = new Date();
        if (onTaskUpdate) {
          onTaskUpdate(task);
        }

        // Execute task
        const result = await runner.runTask(task, context);

        // Update task status
        if (result.success) {
          task.status = TaskStatus.Succeeded;
          completedTasks.push(task);
          // Store result in context for next steps
          context.previousResults.set(task.stepName, result);
        } else {
          task.status = TaskStatus.Failed;
          failedTasks.push(task);
          // Optionally continue or abort on failure
          // For now, we continue but could add a flag to abort
        }

        task.updatedAt = new Date();
        if (onTaskUpdate) {
          onTaskUpdate(task);
        }
      } catch (error) {
        task.status = TaskStatus.Failed;
        task.updatedAt = new Date();
        failedTasks.push(task);

        if (onTaskUpdate) {
          onTaskUpdate(task);
        }

        // Log error but continue (could be configurable)
        console.error(`Task ${task.stepName} failed:`, error);
      }
    }

    return {
      success: failedTasks.length === 0,
      completedTasks,
      failedTasks,
    };
  }

  /**
   * Get execution plan (dry-run mode)
   */
  getExecutionPlan(workflow: WorkflowDefinition, context: OrchestratorContext): AgentTask[] {
    const orderedSteps = this.resolver.resolve(workflow.steps);
    return orderedSteps.map((step) => {
      const role = this.getRole(step.agent, context);
      return this.createTask(step, workflow.name, role);
    });
  }

  private createTask(
    step: { name: string; agent: string; description: string; inputs?: string[] },
    flowName: string,
    role: AgentRole | string
  ): AgentTask {
    return {
      id: uuidv4(),
      role,
      description: step.description,
      flowName,
      stepName: step.name,
      status: TaskStatus.Pending,
      inputs: step.inputs,
      createdAt: new Date(),
      updatedAt: new Date(),
    };
  }

  private getRole(roleName: string, context: OrchestratorContext): AgentRole | string {
    // Try to get role from config if available
    if (this.config?.roles && this.config.roles[roleName]) {
      return this.config.roles[roleName];
    }
    return roleName;
  }
}

