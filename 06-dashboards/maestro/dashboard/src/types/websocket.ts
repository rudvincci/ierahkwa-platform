export interface DashboardMessage {
  type: 'metrics' | 'workflow-start' | 'workflow-end' | 'step-update' | 'activity-update' | 'error' | 'checkpoint' | 'token-update' | 'model-change' | 'agent-switch' | 'progress-update' | 'log';
  data: any;
  timestamp: Date | string;
}

export interface LogMessage {
  workflow: string;
  level: 'stdout' | 'stderr' | 'info' | 'warn' | 'error' | 'debug';
  message: string;
  timestamp: string;
}
