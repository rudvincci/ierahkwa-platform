/**
 * Workflow Composition Service
 * 
 * Enables workflow imports, sub-workflow calls, and workflow libraries.
 */

import * as fs from 'fs';
import * as path from 'path';
import * as yaml from 'js-yaml';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';
import { StepDefinition } from '../domain/StepDefinition';

export interface WorkflowImport {
  from: string; // File path or workflow name
  as?: string; // Alias name
  steps?: string[]; // Specific steps to import
}

export interface WorkflowComposition {
  imports?: WorkflowImport[];
  extends?: string; // Base workflow to extend
  steps: StepDefinition[];
}

/**
 * Workflow Composer
 */
export class WorkflowComposer {
  private workflowsDir: string;
  private loadedWorkflows: Map<string, WorkflowDefinition> = new Map();

  constructor(repositoryRoot: string = process.cwd()) {
    this.workflowsDir = path.join(repositoryRoot, '.maestro', 'workflows');
  }

  /**
   * Compose workflow from imports and extensions
   */
  async compose(composition: WorkflowComposition, baseWorkflows: Map<string, WorkflowDefinition>): Promise<WorkflowDefinition> {
    const composedSteps: StepDefinition[] = [];
    const importedSteps = new Set<string>();

    // Handle extends (base workflow)
    if (composition.extends) {
      const baseWorkflow = baseWorkflows.get(composition.extends);
      if (!baseWorkflow) {
        throw new Error(`Base workflow not found: ${composition.extends}`);
      }
      // Add all steps from base workflow
      baseWorkflow.steps.forEach(step => {
        composedSteps.push(step);
        importedSteps.add(step.name);
      });
    }

    // Handle imports
    if (composition.imports) {
      for (const imp of composition.imports) {
        const importedWorkflow = await this.loadWorkflow(imp.from);
        if (!importedWorkflow) {
          throw new Error(`Failed to import workflow: ${imp.from}`);
        }

        // Import specific steps or all steps
        const stepsToImport = imp.steps || importedWorkflow.steps.map(s => s.name);
        
        for (const stepName of stepsToImport) {
          const step = importedWorkflow.steps.find(s => s.name === stepName);
          if (!step) {
            throw new Error(`Step "${stepName}" not found in workflow "${imp.from}"`);
          }

          // Avoid duplicate step names
          if (importedSteps.has(step.name)) {
            const renamedStep = {
              ...step,
              name: `${imp.as || imp.from}_${step.name}`,
            };
            composedSteps.push(renamedStep);
            importedSteps.add(renamedStep.name);
          } else {
            composedSteps.push(step);
            importedSteps.add(step.name);
          }
        }
      }
    }

    // Add composition's own steps
    composition.steps.forEach(step => {
      if (importedSteps.has(step.name)) {
        throw new Error(`Duplicate step name: ${step.name}`);
      }
      composedSteps.push(step);
      importedSteps.add(step.name);
    });

    return {
      name: 'composed',
      description: 'Composed workflow',
      steps: composedSteps,
    };
  }

  /**
   * Load workflow from file or name
   */
  private async loadWorkflow(from: string): Promise<WorkflowDefinition | null> {
    // Check if already loaded
    if (this.loadedWorkflows.has(from)) {
      return this.loadedWorkflows.get(from)!;
    }

    // Try to load from file
    const filePath = path.isAbsolute(from) ? from : path.join(this.workflowsDir, `${from}.yml`);
    
    if (fs.existsSync(filePath)) {
      try {
        const content = await fs.promises.readFile(filePath, 'utf-8');
        const workflow = yaml.load(content) as WorkflowDefinition;
        this.loadedWorkflows.set(from, workflow);
        return workflow;
      } catch (error) {
        console.warn(`Failed to load workflow from ${filePath}:`, error);
        return null;
      }
    }

    return null;
  }

  /**
   * Create workflow library (collection of reusable workflows)
   */
  async createLibrary(name: string, workflows: WorkflowDefinition[]): Promise<void> {
    const libraryDir = path.join(this.workflowsDir, 'libraries', name);
    await fs.promises.mkdir(libraryDir, { recursive: true });

    for (const workflow of workflows) {
      const filePath = path.join(libraryDir, `${workflow.name}.yml`);
      const content = yaml.dump(workflow, { indent: 2 });
      await fs.promises.writeFile(filePath, content, 'utf-8');
    }
  }

  /**
   * Load workflow library
   */
  async loadLibrary(name: string): Promise<Map<string, WorkflowDefinition>> {
    const libraryDir = path.join(this.workflowsDir, 'libraries', name);
    const workflows = new Map<string, WorkflowDefinition>();

    if (!fs.existsSync(libraryDir)) {
      return workflows;
    }

    const files = await fs.promises.readdir(libraryDir);
    for (const file of files) {
      if (file.endsWith('.yml') || file.endsWith('.yaml')) {
        const filePath = path.join(libraryDir, file);
        try {
          const content = await fs.promises.readFile(filePath, 'utf-8');
          const workflow = yaml.load(content) as WorkflowDefinition;
          workflows.set(workflow.name, workflow);
        } catch (error) {
          console.warn(`Failed to load workflow from ${filePath}:`, error);
        }
      }
    }

    return workflows;
  }
}
