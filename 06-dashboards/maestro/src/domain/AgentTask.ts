import { AgentRole } from './AgentRole';

export enum TaskStatus {
  Pending = 'Pending',
  Running = 'Running',
  Succeeded = 'Succeeded',
  Failed = 'Failed',
}

export interface AgentTask {
  id: string;
  role: AgentRole | string;
  description: string;
  flowName: string;
  stepName: string;
  status: TaskStatus;
  inputs?: string[];
  createdAt: Date;
  updatedAt: Date;
}

export const createAgentTask = (
  id: string,
  role: AgentRole | string,
  description: string,
  flowName: string,
  stepName: string,
  inputs?: string[]
): AgentTask => ({
  id,
  role,
  description,
  flowName,
  stepName,
  status: TaskStatus.Pending,
  inputs,
  createdAt: new Date(),
  updatedAt: new Date(),
});

