import { AgentTask } from '../domain/AgentTask';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';

export interface OrchestratorContext {
  workflow: WorkflowDefinition;
  previousResults: Map<string, AgentResult>;
  repositoryRoot: string;
  featureDescription?: string;
}

export interface AgentResult {
  success: boolean;
  summary: string;
  rawOutput?: string;
  error?: string;
}

export const createOrchestratorContext = (
  workflow: WorkflowDefinition,
  repositoryRoot: string,
  featureDescription?: string
): OrchestratorContext => ({
  workflow,
  previousResults: new Map(),
  repositoryRoot,
  featureDescription,
});

