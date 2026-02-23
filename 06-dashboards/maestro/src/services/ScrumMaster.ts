/**
 * Scrum Master Service
 * 
 * Acts as a scrum master for workflows:
 * - Continuously monitors workflow state
 * - Reassigns steps to agents in sprints
 * - Ensures each agent has its own cursor-agent resume GUID
 * - Manages task assignment until completion
 */

import { WorkflowDefinition, StepDefinition } from '../domain/WorkflowDefinition';
import * as crypto from 'crypto';

export interface BacklogItem {
  stepName: string;
  taskName?: string;
  priority: number; // Lower number = higher priority (1 = highest)
  description?: string;
  estimatedEffort?: number; // Estimated effort in story points or time
  dependencies?: string[]; // Dependencies (other backlog items that must complete first)
  addedAt: Date;
}

export interface AgentAssignment {
  agentName: string;
  stepName: string;
  taskName?: string;
  guid: string; // cursor-agent resume GUID
  assignedAt: Date;
  status: 'pending' | 'running' | 'completed' | 'failed';
  priority: number;
}

export interface Sprint {
  sprintNumber: number;
  startTime: Date;
  endTime?: Date;
  assignments: AgentAssignment[];
  completed: boolean;
}

export interface WorkflowState {
  workflowName: string;
  currentSprint: number;
  sprints: Sprint[];
  agentGuids: Record<string, string>; // Map of agent name to GUID
  stepStatus: Record<string, 'pending' | 'running' | 'completed' | 'failed'>;
  taskStatus: Record<string, 'pending' | 'running' | 'completed' | 'failed'>; // step.task -> status
  backlog: BacklogItem[]; // Prioritized backlog of tasks/steps
}

export class ScrumMaster {
  private workflowStates: Map<string, WorkflowState> = new Map();
  private monitoringIntervals: Map<string, NodeJS.Timeout> = new Map();
  private sprintInterval: number = 30000; // 30 seconds per sprint (configurable)

  /**
   * Generate or retrieve GUID for an agent
   */
  private getAgentGuid(workflowName: string, agentName: string): string {
    const state = this.workflowStates.get(workflowName);
    if (state?.agentGuids[agentName]) {
      return state.agentGuids[agentName];
    }

    // Generate new GUID based on workflow + agent name
    const hash = crypto.createHash('md5')
      .update(`${workflowName}-${agentName}`)
      .digest('hex');
    
    // Create UUID-like format
    const guid = [
      hash.substring(0, 8),
      hash.substring(8, 12),
      '4' + hash.substring(13, 16),
      ((parseInt(hash[16], 16) & 0x3) | 0x8).toString(16) + hash.substring(17, 20),
      hash.substring(20, 32)
    ].join('-');

    if (!state) {
      this.workflowStates.set(workflowName, {
        workflowName,
        currentSprint: 0,
        sprints: [],
        agentGuids: { [agentName]: guid },
        stepStatus: {},
        taskStatus: {},
        backlog: [],
      });
    } else {
      state.agentGuids[agentName] = guid;
    }

    return guid;
  }

  /**
   * Start monitoring a workflow (scrum master mode)
   */
  startMonitoring(
    workflowName: string,
    workflow: WorkflowDefinition,
    onReassign: (assignments: AgentAssignment[]) => Promise<void>
  ): void {
    // Initialize workflow state
    const state: WorkflowState = {
      workflowName,
      currentSprint: 0,
      sprints: [],
      agentGuids: {},
      stepStatus: {},
      taskStatus: {},
      backlog: [],
    };

    // Build prioritized backlog from workflow steps and tasks
    const backlog: BacklogItem[] = [];
    let priorityCounter = 1;

    workflow.steps?.forEach((step, stepIndex) => {
      const stepWithTasks = step as any;
      const stepPriority = (step as any).priority || (stepIndex + 1) * 10; // Default priority based on order
      
      // Check if step has tasks
      if (stepWithTasks.tasks && Array.isArray(stepWithTasks.tasks) && stepWithTasks.tasks.length > 0) {
        // Add each task to backlog
        stepWithTasks.tasks.forEach((task: any, taskIndex: number) => {
          if (task.name) {
            const taskPriority = (task as any).priority || stepPriority + (taskIndex + 1);
            const dependencies: string[] = [];
            
            // Add step dependencies
            if (step.dependsOn) {
              const deps = Array.isArray(step.dependsOn) ? step.dependsOn : [step.dependsOn];
              deps.forEach(dep => {
                dependencies.push(dep);
              });
            }
            
            backlog.push({
              stepName: step.name,
              taskName: task.name,
              priority: taskPriority,
              description: task.command || step.description || task.description,
              estimatedEffort: (task as any).effort || (task as any).storyPoints || 1,
              dependencies,
              addedAt: new Date(),
            });
            
            state.taskStatus[`${step.name}.${task.name}`] = 'pending';
          }
        });
      } else {
        // Step without tasks - add step itself to backlog
        const dependencies: string[] = [];
        if (step.dependsOn) {
          const deps = Array.isArray(step.dependsOn) ? step.dependsOn : [step.dependsOn];
          dependencies.push(...deps);
        }
        
        backlog.push({
          stepName: step.name,
          priority: stepPriority,
          description: step.description,
          estimatedEffort: (step as any).effort || (step as any).storyPoints || 1,
          dependencies,
          addedAt: new Date(),
        });
      }
      
      state.stepStatus[step.name] = 'pending';
    });

    // Sort backlog by priority (lower number = higher priority)
    state.backlog = backlog.sort((a, b) => a.priority - b.priority);

    // Initialize agent GUIDs
    const workflowWithAgents = workflow as any;
    if (workflowWithAgents.agents && Array.isArray(workflowWithAgents.agents)) {
      workflowWithAgents.agents.forEach((agent: any) => {
        if (agent.name) {
          state.agentGuids[agent.name] = this.getAgentGuid(workflowName, agent.name);
        }
      });
    }

    this.workflowStates.set(workflowName, state);

    // Start continuous monitoring loop
    const interval = setInterval(async () => {
      await this.runSprint(workflowName, workflow, onReassign);
    }, this.sprintInterval);

    this.monitoringIntervals.set(workflowName, interval);

    // Run first sprint immediately
    this.runSprint(workflowName, workflow, onReassign).catch(err => {
      console.error(`Error in initial sprint for ${workflowName}:`, err);
    });

    console.log(`🎯 Scrum Master: Started monitoring workflow "${workflowName}"`);
  }

  /**
   * Run a sprint - reassign tasks to agents
   */
  private async runSprint(
    workflowName: string,
    workflow: WorkflowDefinition,
    onReassign: (assignments: AgentAssignment[]) => Promise<void>
  ): Promise<void> {
    const state = this.workflowStates.get(workflowName);
    if (!state) {
      console.warn(`No state found for workflow: ${workflowName}`);
      return;
    }

    // Check if workflow is still running (would need to check with dashboard server)
    // For now, assume it's running if state exists

    state.currentSprint++;
    const sprint: Sprint = {
      sprintNumber: state.currentSprint,
      startTime: new Date(),
      assignments: [],
      completed: false,
    };

    console.log(`🎯 Scrum Master: Starting sprint ${sprint.sprintNumber} for "${workflowName}"`);

    // Analyze workflow state and assign tasks from prioritized backlog
    const assignments: AgentAssignment[] = [];

    // Get available agents
    const workflowWithAgents = workflow as any;
    const availableAgents = (workflowWithAgents.agents && Array.isArray(workflowWithAgents.agents)) 
      ? workflowWithAgents.agents 
      : [];
    
    // Get pending items from backlog (sorted by priority)
    const pendingBacklogItems = state.backlog.filter(item => {
      if (item.taskName) {
        // Check task status
        const taskKey = `${item.stepName}.${item.taskName}`;
        const taskStatus = state.taskStatus[taskKey];
        return taskStatus === 'pending' || taskStatus === undefined;
      } else {
        // Check step status
        const stepStatus = state.stepStatus[item.stepName];
        return stepStatus === 'pending' || stepStatus === undefined;
      }
    });

    // Filter items that can run (dependencies satisfied)
    const readyItems = pendingBacklogItems.filter(item => {
      if (!item.dependencies || item.dependencies.length === 0) {
        return true;
      }
      
      // Check if all dependencies are completed
      return item.dependencies.every(dep => {
        const depStatus = state.stepStatus[dep];
        return depStatus === 'completed';
      });
    });

    // Assign ready items to agents (in priority order)
    for (const item of readyItems) {
      // Find the step definition
      const step = workflow.steps?.find(s => s.name === item.stepName);
      if (!step) continue;

      // Check if step can run
      const canRun = this.canRunStep(step, state.stepStatus);
      if (!canRun) {
        console.log(`   ⏸ Backlog item "${item.stepName}${item.taskName ? `.${item.taskName}` : ''}" waiting for dependencies`);
        continue;
      }

      // Assign to agent (use step's agent or default to first available)
      const assignedAgent = step.agent || availableAgents[0]?.name || 'default';
      const agentGuid = this.getAgentGuid(workflowName, assignedAgent);

      const assignment: AgentAssignment = {
        agentName: assignedAgent,
        stepName: item.stepName,
        taskName: item.taskName,
        guid: agentGuid,
        assignedAt: new Date(),
        status: 'pending',
        priority: item.priority,
      };

      assignments.push(assignment);
      sprint.assignments.push(assignment);

      // Mark as running
      if (item.taskName) {
        const taskKey = `${item.stepName}.${item.taskName}`;
        state.taskStatus[taskKey] = 'running';
      }
      state.stepStatus[item.stepName] = 'running';
    }

    // Record sprint
    state.sprints.push(sprint);

    if (assignments.length > 0) {
      console.log(`   📋 Assigned ${assignments.length} tasks to agents`);
      assignments.forEach(assignment => {
        console.log(`      → ${assignment.taskName || assignment.stepName} → ${assignment.agentName} (GUID: ${assignment.guid.substring(0, 8)}...)`);
      });

      // Notify about reassignments
      try {
        await onReassign(assignments);
      } catch (error) {
        console.error(`Error notifying about reassignments:`, error);
      }
    } else {
      console.log(`   ✅ No pending tasks - workflow may be complete`);
    }

    sprint.endTime = new Date();
    sprint.completed = true;
  }

  /**
   * Check if a step can run based on dependencies
   */
  private canRunStep(step: StepDefinition, stepStatus: Record<string, string>): boolean {
    if (!step.dependsOn || step.dependsOn.length === 0) {
      return true;
    }

    const dependencies = Array.isArray(step.dependsOn) ? step.dependsOn : [step.dependsOn];
    return dependencies.every(dep => stepStatus[dep] === 'completed');
  }

  /**
   * Update task status (called when task completes)
   */
  updateTaskStatus(
    workflowName: string,
    stepName: string,
    taskName: string,
    status: 'completed' | 'failed'
  ): void {
    const state = this.workflowStates.get(workflowName);
    if (!state) return;

    const taskKey = `${stepName}.${taskName}`;
    state.taskStatus[taskKey] = status;

    // Check if all tasks in step are completed
    const step = state.workflowName; // Would need workflow reference
    // For now, just update the status
    // In full implementation, would check all tasks in step
  }

  /**
   * Update step status
   */
  updateStepStatus(
    workflowName: string,
    stepName: string,
    status: 'completed' | 'failed'
  ): void {
    const state = this.workflowStates.get(workflowName);
    if (!state) return;

    state.stepStatus[stepName] = status;
  }

  /**
   * Get agent GUIDs for a workflow
   */
  getAgentGuids(workflowName: string): Record<string, string> {
    const state = this.workflowStates.get(workflowName);
    return state?.agentGuids || {};
  }

  /**
   * Get workflow state
   */
  getWorkflowState(workflowName: string): WorkflowState | undefined {
    return this.workflowStates.get(workflowName);
  }

  /**
   * Get backlog for a workflow
   */
  getBacklog(workflowName: string): BacklogItem[] {
    const state = this.workflowStates.get(workflowName);
    return state?.backlog || [];
  }

  /**
   * Update backlog item priority
   */
  updateBacklogPriority(workflowName: string, stepName: string, taskName: string | undefined, newPriority: number): boolean {
    const state = this.workflowStates.get(workflowName);
    if (!state) return false;

    const item = state.backlog.find(item => 
      item.stepName === stepName && item.taskName === taskName
    );

    if (item) {
      item.priority = newPriority;
      // Re-sort backlog
      state.backlog.sort((a, b) => a.priority - b.priority);
      return true;
    }

    return false;
  }

  /**
   * Add item to backlog
   */
  addToBacklog(workflowName: string, item: BacklogItem): void {
    const state = this.workflowStates.get(workflowName);
    if (!state) return;

    // Check if item already exists
    const exists = state.backlog.some(existing => 
      existing.stepName === item.stepName && existing.taskName === item.taskName
    );

    if (!exists) {
      state.backlog.push(item);
      // Re-sort backlog
      state.backlog.sort((a, b) => a.priority - b.priority);
    }
  }

  /**
   * Remove item from backlog
   */
  removeFromBacklog(workflowName: string, stepName: string, taskName?: string): boolean {
    const state = this.workflowStates.get(workflowName);
    if (!state) return false;

    const index = state.backlog.findIndex(item => 
      item.stepName === stepName && item.taskName === taskName
    );

    if (index >= 0) {
      state.backlog.splice(index, 1);
      return true;
    }

    return false;
  }

  /**
   * Stop monitoring a workflow
   */
  stopMonitoring(workflowName: string): void {
    const interval = this.monitoringIntervals.get(workflowName);
    if (interval) {
      clearInterval(interval);
      this.monitoringIntervals.delete(workflowName);
    }
    this.workflowStates.delete(workflowName);
    console.log(`🎯 Scrum Master: Stopped monitoring workflow "${workflowName}"`);
  }

  /**
   * Set sprint interval (for testing/configuration)
   */
  setSprintInterval(interval: number): void {
    this.sprintInterval = interval;
  }
}

