/**
 * Agent Switcher
 * 
 * Manages agent switching mid-workflow
 */

import { AgentRole } from '../domain/AgentRole';
import { WorkflowDefinition, StepDefinition } from '../domain/WorkflowDefinition';

export interface AgentSwitchRequest {
  workflowName: string;
  stepName: string;
  newAgent: string | AgentRole;
  reason?: string;
}

export interface AgentSwitchHistory {
  timestamp: Date;
  stepName: string;
  fromAgent: string;
  toAgent: string;
  reason?: string;
}

export class AgentSwitcher {
  private switches: Map<string, Map<string, string | AgentRole>> = new Map(); // workflowName -> stepName -> agent
  private switchHistory: Map<string, AgentSwitchHistory[]> = new Map(); // workflowName -> history

  /**
   * Switch agent for a specific step
   */
  switchAgent(request: AgentSwitchRequest): boolean {
    const { workflowName, stepName, newAgent, reason } = request;

    if (!this.switches.has(workflowName)) {
      this.switches.set(workflowName, new Map());
    }

    const workflowSwitches = this.switches.get(workflowName)!;
    
    // Get current agent (would need workflow context, but we'll track what we set)
    const currentAgent = workflowSwitches.get(stepName) || 'original';

    // Record history
    if (!this.switchHistory.has(workflowName)) {
      this.switchHistory.set(workflowName, []);
    }
    const history = this.switchHistory.get(workflowName)!;
    history.push({
      timestamp: new Date(),
      stepName,
      fromAgent: typeof currentAgent === 'string' ? currentAgent : currentAgent.name || 'unknown',
      toAgent: typeof newAgent === 'string' ? newAgent : newAgent.name || 'unknown',
      reason,
    });

    // Apply switch
    workflowSwitches.set(stepName, newAgent);

    return true;
  }

  /**
   * Get agent for a step (returns switched agent if exists, otherwise null)
   */
  getAgentForStep(workflowName: string, stepName: string): string | AgentRole | null {
    return this.switches.get(workflowName)?.get(stepName) || null;
  }

  /**
   * Apply agent switches to a workflow definition
   */
  applySwitches(workflow: WorkflowDefinition): WorkflowDefinition {
    const switches = this.switches.get(workflow.name);
    if (!switches || switches.size === 0) {
      return workflow;
    }

    // Create modified workflow
    const modifiedWorkflow: WorkflowDefinition = {
      ...workflow,
      steps: workflow.steps.map(step => {
        const switchedAgent = switches.get(step.name);
        if (switchedAgent) {
          return {
            ...step,
            agent: typeof switchedAgent === 'string' ? switchedAgent : switchedAgent.name || step.agent,
          } as StepDefinition;
        }
        return step;
      }),
    };

    return modifiedWorkflow;
  }

  /**
   * Get switch history for a workflow
   */
  getSwitchHistory(workflowName: string): AgentSwitchHistory[] {
    return this.switchHistory.get(workflowName) || [];
  }

  /**
   * Clear switches for a workflow
   */
  clearWorkflow(workflowName: string): void {
    this.switches.delete(workflowName);
    this.switchHistory.delete(workflowName);
  }

  /**
   * Get all switched steps for a workflow
   */
  getSwitchedSteps(workflowName: string): Map<string, string | AgentRole> {
    return this.switches.get(workflowName) || new Map();
  }
}
