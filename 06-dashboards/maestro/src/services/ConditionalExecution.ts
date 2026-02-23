/**
 * Conditional Step Execution Service
 * 
 * Enables if/else logic and conditional step execution in workflows.
 */

import { StepDefinition } from '../domain/StepDefinition';
import { OrchestratorContext } from '../workflow/OrchestratorContext';

export interface Condition {
  type: 'equals' | 'notEquals' | 'greaterThan' | 'lessThan' | 'contains' | 'exists' | 'custom';
  field: string; // Path to field in context (e.g., 'previousResults.stepName.summary')
  value?: any; // Value to compare against
  customEvaluator?: (context: OrchestratorContext) => boolean; // Custom evaluation function
}

export interface ConditionalStep extends StepDefinition {
  condition?: Condition;
  elseSteps?: StepDefinition[]; // Steps to execute if condition is false
}

/**
 * Conditional Execution Evaluator
 */
export class ConditionalExecutor {
  /**
   * Evaluate condition
   */
  evaluate(condition: Condition, context: OrchestratorContext): boolean {
    if (condition.type === 'custom' && condition.customEvaluator) {
      return condition.customEvaluator(context);
    }

    const fieldValue = this.getFieldValue(condition.field, context);

    switch (condition.type) {
      case 'equals':
        return fieldValue === condition.value;
      
      case 'notEquals':
        return fieldValue !== condition.value;
      
      case 'greaterThan':
        return Number(fieldValue) > Number(condition.value);
      
      case 'lessThan':
        return Number(fieldValue) < Number(condition.value);
      
      case 'contains':
        return String(fieldValue).includes(String(condition.value));
      
      case 'exists':
        return fieldValue !== undefined && fieldValue !== null;
      
      default:
        return false;
    }
  }

  /**
   * Get field value from context using dot notation
   */
  private getFieldValue(field: string, context: OrchestratorContext): any {
    const parts = field.split('.');
    let value: any = context;

    for (const part of parts) {
      if (value === undefined || value === null) {
        return undefined;
      }

      if (part === 'previousResults') {
        value = context.previousResults;
      } else if (value instanceof Map) {
        value = value.get(part);
      } else if (typeof value === 'object') {
        value = (value as any)[part];
      } else {
        return undefined;
      }
    }

    return value;
  }

  /**
   * Filter steps based on conditions
   */
  filterSteps(steps: ConditionalStep[], context: OrchestratorContext): StepDefinition[] {
    const filtered: StepDefinition[] = [];

    for (const step of steps) {
      if (!step.condition) {
        // No condition, always execute
        filtered.push(step);
        continue;
      }

      const conditionMet = this.evaluate(step.condition, context);
      
      if (conditionMet) {
        // Condition is true, execute this step
        filtered.push(step);
      } else if (step.elseSteps && step.elseSteps.length > 0) {
        // Condition is false, execute else steps
        filtered.push(...step.elseSteps);
      }
      // If condition is false and no else steps, skip this step
    }

    return filtered;
  }
}
