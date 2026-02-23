import { StepDefinition } from '../domain/StepDefinition';

export interface ResolvedStepGroup {
  steps: StepDefinition[];
  canRunInParallel: boolean;
  order: number;
}

/**
 * Enhanced dependency resolver that identifies parallelizable steps
 * Groups steps that can run in parallel together
 */
export class ParallelDependencyResolver {
  resolve(steps: StepDefinition[]): ResolvedStepGroup[] {
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
          if (!stepMap.has(dep)) {
            throw new Error(`Step "${step.name}" depends on "${dep}" which does not exist`);
          }

          const currentDeps = dependencies.get(dep) || [];
          currentDeps.push(step.name);
          dependencies.set(dep, currentDeps);

          const currentInDegree = inDegree.get(step.name) || 0;
          inDegree.set(step.name, currentInDegree + 1);
        }
      }
    }

    // Group steps by execution level (parallel groups)
    const groups: ResolvedStepGroup[] = [];
    const processed = new Set<string>();
    let order = 0;

    while (processed.size < steps.length) {
      // Find all steps ready to execute (in-degree = 0 and not processed)
      const readySteps: StepDefinition[] = [];
      
      for (const [stepName, degree] of inDegree.entries()) {
        if (degree === 0 && !processed.has(stepName)) {
          const step = stepMap.get(stepName)!;
          readySteps.push(step);
        }
      }

      if (readySteps.length === 0) {
        // Check for cycles
        const unprocessed = steps.filter((s) => !processed.has(s.name));
        throw new Error(
          `Circular dependency detected. Unprocessed steps: ${unprocessed.map((s) => s.name).join(', ')}`
        );
      }

      // Determine if these steps can run in parallel
      // Steps can run in parallel if:
      // 1. They have parallel: true, OR
      // 2. They have no dependencies on each other
      const canRunInParallel = this.canRunParallel(readySteps, dependencies);

      // Mark as processed and update dependencies
      for (const step of readySteps) {
        processed.add(step.name);
        
        // Decrement in-degree of dependent steps
        const deps = dependencies.get(step.name) || [];
        for (const dep of deps) {
          const currentInDegree = inDegree.get(dep)!;
          inDegree.set(dep, currentInDegree - 1);
        }
      }

      groups.push({
        steps: readySteps,
        canRunInParallel,
        order: order++,
      });
    }

    return groups;
  }

  private canRunParallel(steps: StepDefinition[], dependencies: Map<string, string[]>): boolean {
    // If any step explicitly sets parallel: false, they can't run in parallel
    if (steps.some((s) => s.parallel === false)) {
      return false;
    }

    // If all steps have parallel: true, they can run in parallel
    if (steps.every((s) => s.parallel === true)) {
      return true;
    }

    // Check if steps depend on each other
    const stepNames = new Set(steps.map((s) => s.name));
    for (const step of steps) {
      const deps = dependencies.get(step.name) || [];
      // If any dependency is in the same group, they can't run in parallel
      if (deps.some((dep) => stepNames.has(dep))) {
        return false;
      }
    }

    // Default: if no explicit parallel setting and no dependencies, allow parallel
    return steps.length > 1;
  }
}
