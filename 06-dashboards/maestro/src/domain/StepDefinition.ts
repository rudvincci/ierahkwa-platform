export interface StepDefinition {
  name: string;
  agent: string;
  description: string;
  dependsOn?: string[];
  inputs?: string[];
  // Subagent-like capabilities
  parallel?: boolean; // Execute in parallel with other steps that have parallel: true
  nestedWorkflow?: string; // Reference to another workflow to execute as a sub-workflow
  spawnTasks?: SpawnTaskDefinition[]; // Dynamically create sub-tasks
  maxConcurrency?: number; // Maximum parallel executions (default: unlimited)
}

export interface SpawnTaskDefinition {
  condition?: string; // Condition to evaluate (e.g., "previousResult.success === true")
  agent: string;
  description: string;
  inputs?: string[];
}
