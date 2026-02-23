import { AgentTask, TaskStatus } from './AgentTask';

export interface StepLog {
  stepName: string;
  status: TaskStatus;
  prompt?: string;
  summary?: string;
  rawOutput?: string;
  error?: string;
  startedAt: Date;
  completedAt?: Date;
  durationMs?: number;
}

export interface RunLog {
  runId: string;
  flowName: string;
  timestamp: Date;
  status: 'running' | 'completed' | 'failed';
  steps: StepLog[];
  metadata?: Record<string, unknown>;
}

