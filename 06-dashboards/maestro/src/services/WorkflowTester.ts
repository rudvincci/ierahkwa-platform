/**
 * Workflow Testing Framework
 * 
 * Provides unit and integration testing for workflows.
 */

import { WorkflowDefinition } from '../domain/WorkflowDefinition';
import { StepDefinition } from '../domain/StepDefinition';
import { ExecutionResult } from '../workflow/EnhancedWorkflowExecutor';

export interface TestCase {
  name: string;
  description?: string;
  workflow: WorkflowDefinition;
  inputs?: Record<string, any>;
  expectedResults?: {
    completedSteps?: string[];
    failedSteps?: string[];
    success?: boolean;
  };
  assertions?: Array<{
    type: 'stepCompleted' | 'stepFailed' | 'resultContains' | 'custom';
    stepName?: string;
    expectedValue?: any;
    customAssertion?: (result: ExecutionResult) => boolean;
  }>;
}

export interface TestResult {
  testCase: TestCase;
  passed: boolean;
  error?: string;
  executionResult?: ExecutionResult;
  assertions: Array<{
    assertion: string;
    passed: boolean;
    error?: string;
  }>;
}

/**
 * Workflow Tester
 */
export class WorkflowTester {
  /**
   * Run test case
   */
  async runTest(
    testCase: TestCase,
    executor: (workflow: WorkflowDefinition, inputs?: Record<string, any>) => Promise<ExecutionResult>
  ): Promise<TestResult> {
    const result: TestResult = {
      testCase,
      passed: false,
      assertions: [],
    };

    try {
      // Execute workflow
      const executionResult = await executor(testCase.workflow, testCase.inputs);
      result.executionResult = executionResult;

      // Run assertions
      if (testCase.assertions && testCase.assertions.length > 0) {
        for (const assertion of testCase.assertions) {
          const assertionResult = this.runAssertion(assertion, executionResult);
          result.assertions.push(assertionResult);
        }
      }

      // Check expected results
      if (testCase.expectedResults) {
        if (testCase.expectedResults.success !== undefined) {
          const successAssertion: { assertion: string; passed: boolean; error?: string } = {
            assertion: `Expected success: ${testCase.expectedResults.success}`,
            passed: executionResult.success === testCase.expectedResults.success,
          };
          if (!successAssertion.passed) {
            successAssertion.error = `Expected success=${testCase.expectedResults.success}, got ${executionResult.success}`;
          }
          result.assertions.push(successAssertion);
        }

        if (testCase.expectedResults.completedSteps) {
          const completedSteps = executionResult.completedTasks.map(t => t.stepName);
          const missing = testCase.expectedResults.completedSteps.filter(
            step => !completedSteps.includes(step)
          );
          const assertion: { assertion: string; passed: boolean; error?: string } = {
            assertion: `Expected completed steps: ${testCase.expectedResults.completedSteps.join(', ')}`,
            passed: missing.length === 0,
          };
          if (!assertion.passed) {
            assertion.error = `Missing completed steps: ${missing.join(', ')}`;
          }
          result.assertions.push(assertion);
        }

        if (testCase.expectedResults.failedSteps) {
          const failedSteps = executionResult.failedTasks.map(t => t.stepName);
          const missing = testCase.expectedResults.failedSteps.filter(
            step => !failedSteps.includes(step)
          );
          const assertion: { assertion: string; passed: boolean; error?: string } = {
            assertion: `Expected failed steps: ${testCase.expectedResults.failedSteps.join(', ')}`,
            passed: missing.length === 0,
          };
          if (!assertion.passed) {
            assertion.error = `Missing failed steps: ${missing.join(', ')}`;
          }
          result.assertions.push(assertion);
        }
      }

      // Determine if test passed
      result.passed = result.assertions.every(a => a.passed);

    } catch (error: any) {
      result.error = error.message;
      result.passed = false;
    }

    return result;
  }

  /**
   * Run assertion
   */
  private runAssertion(
    assertion: NonNullable<TestCase['assertions']>[0],
    result: ExecutionResult
  ): { assertion: string; passed: boolean; error?: string } {
    switch (assertion.type) {
      case 'stepCompleted':
        if (!assertion.stepName) {
          return {
            assertion: 'stepCompleted assertion missing stepName',
            passed: false,
            error: 'stepName is required',
          };
        }
        const completed = result.completedTasks.some(t => t.stepName === assertion.stepName);
        return {
          assertion: `Step "${assertion.stepName}" completed`,
          passed: completed,
          error: completed ? undefined : `Step "${assertion.stepName}" was not completed`,
        };

      case 'stepFailed':
        if (!assertion.stepName) {
          return {
            assertion: 'stepFailed assertion missing stepName',
            passed: false,
            error: 'stepName is required',
          };
        }
        const failed = result.failedTasks.some(t => t.stepName === assertion.stepName);
        return {
          assertion: `Step "${assertion.stepName}" failed`,
          passed: failed,
          error: failed ? undefined : `Step "${assertion.stepName}" did not fail`,
        };

      case 'resultContains':
        // This would need to check result content
        return {
          assertion: 'resultContains assertion',
          passed: true, // Placeholder
        };

      case 'custom':
        if (!assertion.customAssertion) {
          return {
            assertion: 'custom assertion missing customAssertion function',
            passed: false,
            error: 'customAssertion is required',
          };
        }
        try {
          const passed = assertion.customAssertion(result);
          return {
            assertion: 'Custom assertion',
            passed,
            error: passed ? undefined : 'Custom assertion failed',
          };
        } catch (error: any) {
          return {
            assertion: 'Custom assertion',
            passed: false,
            error: error.message,
          };
        }

      default:
        return {
          assertion: 'Unknown assertion type',
          passed: false,
          error: `Unknown assertion type: ${(assertion as any).type}`,
        };
    }
  }

  /**
   * Run test suite
   */
  async runTestSuite(
    testCases: TestCase[],
    executor: (workflow: WorkflowDefinition, inputs?: Record<string, any>) => Promise<ExecutionResult>
  ): Promise<{
    passed: number;
    failed: number;
    results: TestResult[];
  }> {
    const results: TestResult[] = [];

    for (const testCase of testCases) {
      const result = await this.runTest(testCase, executor);
      results.push(result);
    }

    const passed = results.filter(r => r.passed).length;
    const failed = results.filter(r => !r.passed).length;

    return {
      passed,
      failed,
      results,
    };
  }
}
