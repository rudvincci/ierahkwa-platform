export interface TaskDefinition {
  name: string;
  command?: string;
  output?: string;
  for_each?: string;
  condition?: string;
}

export interface StepDefinition {
  name: string;
  description?: string;
  agent?: string;
  model?: string;
  type?: 'sequential' | 'parallel';
  depends_on?: string | string[];
  condition?: string;
  tasks?: TaskDefinition[];
  expectedOutput?: string;
  dependencies?: string[];
}

export interface WorkflowMetadata {
  project?: string;
  domain?: string;
  tdd_document?: string;
  plans_directory?: string;
  services_directory?: string;
  mamey_blockchain_path?: string;
  mamey_node_path?: string;
  [key: string]: any;
}

export interface AgentDefinition {
  name: string;
  role?: string;
  [key: string]: any;
}

export interface WorkflowDefinition {
  name: string;
  description?: string;
  metadata?: WorkflowMetadata;
  agents?: AgentDefinition[];
  steps: StepDefinition[];
  yamlContent?: string;
  yamlPath?: string;
}

export interface ExecutionResult {
  success: boolean;
  completedTasks: any[];
  failedTasks: any[];
  output?: any;
  error?: string;
}
