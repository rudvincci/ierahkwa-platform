import { z } from 'zod';
import { OrchestrationConfig } from '../domain/WorkflowDefinition';

const AgentRoleSchema = z.object({
  name: z.string(),
  description: z.string(),
  promptHints: z.string().optional(),
  defaultFilePatterns: z.array(z.string()).optional(),
  defaultDirectories: z.array(z.string()).optional(),
  domainKnowledge: z.array(z.string()).optional(),
});

const StepDefinitionSchema = z.object({
  name: z.string(),
  agent: z.string(),
  description: z.string(),
  dependsOn: z.array(z.string()).optional(),
  inputs: z.array(z.string()).optional(),
});

const WorkflowDefinitionSchema = z.object({
  name: z.string(),
  description: z.string().optional(),
  steps: z.array(StepDefinitionSchema),
});

const OrchestrationConfigSchema = z.object({
  roles: z.record(z.string(), AgentRoleSchema).optional(),
  flows: z.record(z.string(), WorkflowDefinitionSchema),
});

export class ConfigValidator {
  validate(config: unknown): OrchestrationConfig {
    try {
      return OrchestrationConfigSchema.parse(config) as OrchestrationConfig;
    } catch (error) {
      if (error instanceof z.ZodError) {
        const errors = error.errors.map((e) => `${e.path.join('.')}: ${e.message}`).join('\n');
        throw new Error(`Config validation failed:\n${errors}`);
      }
      throw error;
    }
  }

  /**
   * Validate that workflow steps form a valid DAG (no cycles)
   */
  validateWorkflowDAG(flowName: string, steps: { name: string; dependsOn?: string[] }[]): void {
    const stepNames = new Set(steps.map((s) => s.name));
    
    // Check that all dependencies reference valid steps
    for (const step of steps) {
      if (step.dependsOn) {
        for (const dep of step.dependsOn) {
          if (!stepNames.has(dep)) {
            throw new Error(
              `Workflow "${flowName}": Step "${step.name}" depends on "${dep}" which does not exist`
            );
          }
        }
      }
    }

    // Check for cycles using DFS
    const visited = new Set<string>();
    const recStack = new Set<string>();

    const hasCycle = (stepName: string): boolean => {
      if (recStack.has(stepName)) {
        return true; // Cycle detected
      }
      if (visited.has(stepName)) {
        return false; // Already processed
      }

      visited.add(stepName);
      recStack.add(stepName);

      const step = steps.find((s) => s.name === stepName);
      if (step?.dependsOn) {
        for (const dep of step.dependsOn) {
          if (hasCycle(dep)) {
            return true;
          }
        }
      }

      recStack.delete(stepName);
      return false;
    };

    for (const step of steps) {
      if (!visited.has(step.name) && hasCycle(step.name)) {
        throw new Error(`Workflow "${flowName}" contains a circular dependency`);
      }
    }
  }
}

