/**
 * Pre-flight Validation Service
 * 
 * Validates workflow configuration and environment before execution.
 */

import * as fs from 'fs';
import * as path from 'path';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';
import { StepDefinition } from '../domain/StepDefinition';

export interface ValidationResult {
  valid: boolean;
  errors: string[];
  warnings: string[];
}

export interface ValidationOptions {
  checkCursorAgent?: boolean;
  checkFiles?: boolean;
  checkDependencies?: boolean;
  checkWorkflowStructure?: boolean;
}

/**
 * Pre-flight Validator
 */
export class PreflightValidator {
  private repositoryRoot: string;
  private options: Required<ValidationOptions>;

  constructor(repositoryRoot: string = process.cwd(), options: ValidationOptions = {}) {
    this.repositoryRoot = repositoryRoot;
    this.options = {
      checkCursorAgent: options.checkCursorAgent ?? true,
      checkFiles: options.checkFiles ?? true,
      checkDependencies: options.checkDependencies ?? true,
      checkWorkflowStructure: options.checkWorkflowStructure ?? true,
    };
  }

  /**
   * Validate workflow before execution
   */
  async validateWorkflow(workflow: WorkflowDefinition): Promise<ValidationResult> {
    const errors: string[] = [];
    const warnings: string[] = [];

    // Validate workflow structure
    if (this.options.checkWorkflowStructure) {
      this.validateWorkflowStructure(workflow, errors, warnings);
    }

    // Check file references
    if (this.options.checkFiles) {
      await this.validateFileReferences(workflow, errors, warnings);
    }

    // Check dependencies
    if (this.options.checkDependencies) {
      await this.validateDependencies(errors, warnings);
    }

    // Check cursor-agent availability
    if (this.options.checkCursorAgent) {
      await this.validateCursorAgent(errors, warnings);
    }

    return {
      valid: errors.length === 0,
      errors,
      warnings,
    };
  }

  /**
   * Validate workflow structure
   */
  private validateWorkflowStructure(
    workflow: WorkflowDefinition,
    errors: string[],
    warnings: string[]
  ): void {
    // Check workflow name
    if (!workflow.name || workflow.name.trim().length === 0) {
      errors.push('Workflow name is required');
    }

    // Check steps
    if (!workflow.steps || workflow.steps.length === 0) {
      errors.push('Workflow must have at least one step');
      return;
    }

    // Validate each step
    const stepNames = new Set<string>();
    workflow.steps.forEach((step, index) => {
      // Check step name
      if (!step.name || step.name.trim().length === 0) {
        errors.push(`Step ${index + 1}: Step name is required`);
      } else {
        if (stepNames.has(step.name)) {
          errors.push(`Step ${index + 1}: Duplicate step name "${step.name}"`);
        }
        stepNames.add(step.name);
      }

      // Check agent
      if (!step.agent) {
        warnings.push(`Step "${step.name}": No agent specified, using default`);
      }

      // Check description
      if (!step.description || step.description.trim().length === 0) {
        warnings.push(`Step "${step.name}": No description provided`);
      }

      // Validate dependencies
      if (step.dependsOn) {
        step.dependsOn.forEach((dep) => {
          if (!stepNames.has(dep)) {
            errors.push(`Step "${step.name}": Dependency "${dep}" not found`);
          }
        });
      }
    });

    // Check for circular dependencies
    this.checkCircularDependencies(workflow.steps, errors);
  }

  /**
   * Check for circular dependencies
   */
  private checkCircularDependencies(steps: StepDefinition[], errors: string[]): void {
    const visited = new Set<string>();
    const recursionStack = new Set<string>();

    const hasCycle = (stepName: string): boolean => {
      if (recursionStack.has(stepName)) {
        return true; // Circular dependency detected
      }

      if (visited.has(stepName)) {
        return false; // Already processed
      }

      visited.add(stepName);
      recursionStack.add(stepName);

      const step = steps.find((s) => s.name === stepName);
      if (step && step.dependsOn) {
        for (const dep of step.dependsOn) {
          if (hasCycle(dep)) {
            return true;
          }
        }
      }

      recursionStack.delete(stepName);
      return false;
    };

    for (const step of steps) {
      if (!visited.has(step.name)) {
        if (hasCycle(step.name)) {
          errors.push(`Circular dependency detected involving step "${step.name}"`);
        }
      }
    }
  }

  /**
   * Validate file references in workflow
   */
  private async validateFileReferences(
    workflow: WorkflowDefinition,
    errors: string[],
    warnings: string[]
  ): Promise<void> {
    // Check if referenced files exist
    for (const step of workflow.steps) {
      if (step.description) {
        // Look for file paths in description (simple heuristic)
        const fileMatches = step.description.match(/[\/\\][\w\-\.\/\\]+\.(md|yml|yaml|json|ts|js|cs)/g);
        if (fileMatches) {
          for (const fileMatch of fileMatches) {
            const filePath = path.isAbsolute(fileMatch)
              ? fileMatch
              : path.join(this.repositoryRoot, fileMatch);
            
            if (!fs.existsSync(filePath)) {
              warnings.push(`Step "${step.name}": Referenced file not found: ${fileMatch}`);
            }
          }
        }
      }
    }
  }

  /**
   * Validate dependencies (cursor-agent, etc.)
   */
  private async validateDependencies(errors: string[], warnings: string[]): Promise<void> {
    // Check Node.js version
    const nodeVersion = process.version;
    const majorVersion = parseInt(nodeVersion.substring(1).split('.')[0]);
    if (majorVersion < 16) {
      warnings.push(`Node.js version ${nodeVersion} is below recommended version 16+`);
    }

    // Check required directories
    const requiredDirs = ['.cursor', '.maestro'];
    for (const dir of requiredDirs) {
      const dirPath = path.join(this.repositoryRoot, dir);
      if (!fs.existsSync(dirPath)) {
        warnings.push(`Directory not found: ${dir}`);
      }
    }
  }

  /**
   * Validate cursor-agent availability
   */
  private async validateCursorAgent(errors: string[], warnings: string[]): Promise<void> {
    return new Promise((resolve) => {
      const { spawn } = require('child_process');
      const child = spawn('cursor-agent', ['--version'], {
        shell: true,
        stdio: 'pipe',
      });

      let output = '';
      let errorOutput = '';

      child.stdout.on('data', (data: Buffer) => {
        output += data.toString();
      });

      child.stderr.on('data', (data: Buffer) => {
        errorOutput += data.toString();
      });

      child.on('close', (code: number) => {
        if (code !== 0) {
          errors.push('cursor-agent is not available or not in PATH');
          errors.push(`Error: ${errorOutput || 'Command failed'}`);
        } else {
          // cursor-agent is available
          if (output.trim()) {
            warnings.push(`cursor-agent version: ${output.trim()}`);
          }
        }
        resolve();
      });

      child.on('error', (error: Error) => {
        errors.push(`Failed to check cursor-agent: ${error.message}`);
        resolve();
      });

      // Timeout after 5 seconds
      setTimeout(() => {
        child.kill();
        errors.push('cursor-agent check timed out');
        resolve();
      }, 5000);
    });
  }

  /**
   * Quick validation (only critical checks)
   */
  async quickValidate(workflow: WorkflowDefinition): Promise<boolean> {
    const result = await this.validateWorkflow(workflow);
    return result.valid && result.errors.length === 0;
  }
}
