import { AgentTask } from '../domain/AgentTask';
import { OrchestratorContext, AgentResult } from '../workflow/OrchestratorContext';

export interface IAgentRunner {
  runTask(task: AgentTask, context: OrchestratorContext): Promise<AgentResult>;
}

