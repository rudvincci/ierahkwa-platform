import { StepDefinition } from './StepDefinition';
import { AgentRole } from './AgentRole';

// Re-export StepDefinition for convenience
export { StepDefinition } from './StepDefinition';

export interface WorkflowDefinition {
  name: string;
  description?: string;
  steps: StepDefinition[];
}

export interface OrchestrationConfig {
  roles?: Record<string, AgentRole>;
  flows: Record<string, WorkflowDefinition>;
}

