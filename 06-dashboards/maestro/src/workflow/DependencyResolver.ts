import { StepDefinition } from '../domain/StepDefinition';

export interface ResolvedStep {
  step: StepDefinition;
  order: number;
}

/**
 * Resolve step dependencies using topological sort (Kahn's algorithm)
 * Returns steps in execution order
 */
export class DependencyResolver {
  resolve(steps: StepDefinition[]): StepDefinition[] {
    // Build dependency graph
    const stepMap = new Map<string, StepDefinition>();
    const inDegree = new Map<string, number>();
    const dependencies = new Map<string, string[]>();

    // Initialize
    for (const step of steps) {
      stepMap.set(step.name, step);
      inDegree.set(step.name, 0);
      dependencies.set(step.name, []);
    }

    // Build graph and calculate in-degrees
    for (const step of steps) {
      if (step.dependsOn) {
        for (const dep of step.dependsOn) {
          // Verify dependency exists
          if (!stepMap.has(dep)) {
            throw new Error(`Step "${step.name}" depends on "${dep}" which does not exist`);
          }

          // Add edge
          const currentDeps = dependencies.get(dep) || [];
          currentDeps.push(step.name);
          dependencies.set(dep, currentDeps);

          // Increment in-degree
          const currentInDegree = inDegree.get(step.name) || 0;
          inDegree.set(step.name, currentInDegree + 1);
        }
      }
    }

    // Topological sort using Kahn's algorithm
    const queue: string[] = [];
    const result: StepDefinition[] = [];

    // Find all nodes with in-degree 0
    for (const [stepName, degree] of inDegree.entries()) {
      if (degree === 0) {
        queue.push(stepName);
      }
    }

    // Process queue
    while (queue.length > 0) {
      const current = queue.shift()!;
      const step = stepMap.get(current)!;
      result.push(step);

      // Process dependencies
      const deps = dependencies.get(current) || [];
      for (const dep of deps) {
        const currentInDegree = inDegree.get(dep)!;
        const newInDegree = currentInDegree - 1;
        inDegree.set(dep, newInDegree);

        if (newInDegree === 0) {
          queue.push(dep);
        }
      }
    }

    // Check for cycles (if result length < steps length, there's a cycle)
    if (result.length < steps.length) {
      const processed = new Set(result.map((s) => s.name));
      const unprocessed = steps.filter((s) => !processed.has(s.name));
      throw new Error(
        `Circular dependency detected. Unprocessed steps: ${unprocessed.map((s) => s.name).join(', ')}`
      );
    }

    return result;
  }
}

