/**
 * Integration Tests for Maestro
 * 
 * Tests key features without requiring cursor-agent
 */

import * as fs from 'fs';
import * as path from 'path';
import { StateManager, MemoryStateStorage } from '../src/services/StateManager';
import { ResultCache } from '../src/services/ResultCache';
import { ErrorRecovery, ErrorClassifier, ErrorType } from '../src/services/ErrorRecovery';
import { PreflightValidator } from '../src/services/PreflightValidator';
import { ReportingService } from '../src/services/ReportingService';
import { ParallelOptimizer } from '../src/services/ParallelOptimizer';
import { ConditionalExecutor } from '../src/services/ConditionalExecution';
import { WorkflowComposer } from '../src/services/WorkflowComposer';
import { WorkflowDefinition } from '../src/domain/WorkflowDefinition';
import { createOrchestratorContext } from '../src/workflow/OrchestratorContext';

const TEST_DIR = path.join(__dirname, '..', '.maestro', 'test');

// Setup test directory
if (!fs.existsSync(TEST_DIR)) {
  fs.mkdirSync(TEST_DIR, { recursive: true });
}

async function testStateManager() {
  console.log('Testing StateManager...');
  const storage = new MemoryStateStorage(TEST_DIR);
  const manager = new StateManager(storage);

  const workflow: WorkflowDefinition = {
    name: 'test-workflow',
    description: 'Test workflow',
    steps: [],
  };

  const context = createOrchestratorContext(workflow, TEST_DIR);
  const checkpoint = manager.createCheckpoint('test-workflow', context, [], []);

  await manager.save();
  console.log(`✓ Checkpoint created: ${checkpoint.id}`);

  const loaded = await manager.load(checkpoint.id);
  if (loaded && loaded.id === checkpoint.id) {
    console.log('✓ Checkpoint loaded successfully');
  } else {
    throw new Error('Failed to load checkpoint');
  }

  const checkpoints = await manager.list();
  if (checkpoints.length > 0) {
    console.log(`✓ Found ${checkpoints.length} checkpoint(s)`);
  }

  return true;
}

async function testResultCache() {
  console.log('Testing ResultCache...');
  const cache = new ResultCache(TEST_DIR, { enabled: true, ttl: 1000 });

  const task = {
    stepName: 'test-step',
    agent: 'Backend',
    description: 'Test task',
  };

  const prompt = 'Test prompt';
  const result = {
    success: true,
    summary: 'Test result',
  };

  await cache.set(task as any, prompt, result as any);
  console.log('✓ Result cached');

  const cached = await cache.get(task as any, prompt);
  if (cached && cached.success === result.success) {
    console.log('✓ Result retrieved from cache');
  } else {
    throw new Error('Failed to retrieve cached result');
  }

  const stats = await cache.getStats();
  console.log(`✓ Cache stats: ${stats.totalEntries} entries`);

  return true;
}

async function testErrorRecovery() {
  console.log('Testing ErrorRecovery...');
  const recovery = new ErrorRecovery({
    maxRetries: 3,
    initialDelay: 10,
    jitter: false,
  });

  let attempts = 0;
  const fn = async () => {
    attempts++;
    if (attempts < 3) {
      throw new Error('Transient error');
    }
    return 'success';
  };

  const result = await recovery.retry(fn, (error) => {
    return ErrorClassifier.classify(error);
  });

  if (result.success && attempts === 3) {
    console.log(`✓ Error recovery succeeded after ${result.attempts} attempts`);
  } else {
    throw new Error('Error recovery failed');
  }

  return true;
}

async function testPreflightValidator() {
  console.log('Testing PreflightValidator...');
  const validator = new PreflightValidator(TEST_DIR);

  const workflow: WorkflowDefinition = {
    name: 'test-workflow',
    description: 'Test workflow',
    steps: [
      {
        name: 'step1',
        agent: 'Backend',
        description: 'Step 1',
        dependsOn: [],
      },
      {
        name: 'step2',
        agent: 'Backend',
        description: 'Step 2',
        dependsOn: ['step1'],
      },
    ],
  };

  const validation = await validator.validateWorkflow(workflow);
  if (validation.valid) {
    console.log('✓ Workflow validation passed');
  } else {
    console.log(`⚠ Workflow validation warnings: ${validation.warnings.length}`);
  }

  return true;
}

async function testReportingService() {
  console.log('Testing ReportingService...');
  const reporting = new ReportingService(TEST_DIR);

  const workflow: WorkflowDefinition = {
    name: 'test-workflow',
    description: 'Test workflow',
    steps: [
      {
        name: 'step1',
        agent: 'Backend',
        description: 'Step 1',
        dependsOn: [],
      },
    ],
  };

  const metrics = {
    startTime: new Date(),
    endTime: new Date(),
    duration: 1000,
    completedTasks: 1,
    failedTasks: 0,
  };

  const reportPath = await reporting.generateReport(
    workflow,
    {
      success: true,
      completedTasks: [],
      failedTasks: [],
      nestedWorkflows: new Map(),
    },
    metrics,
    {
      format: 'markdown',
      includeDetails: true,
      includeTrends: false,
    }
  );

  if (fs.existsSync(reportPath)) {
    console.log(`✓ Report generated: ${reportPath}`);
  } else {
    throw new Error('Failed to generate report');
  }

  return true;
}

async function testParallelOptimizer() {
  console.log('Testing ParallelOptimizer...');
  const optimizer = new ParallelOptimizer();

  optimizer.setPriority('step1', {
    stepName: 'step1',
    priority: 8,
    estimatedDuration: 5000,
  });

  optimizer.setPriority('step2', {
    stepName: 'step2',
    priority: 5,
    estimatedDuration: 3000,
  });

  const steps = [
    {
      name: 'step2',
      agent: 'Backend',
      description: 'Step 2',
      dependsOn: [],
    },
    {
      name: 'step1',
      agent: 'Backend',
      description: 'Step 1',
      dependsOn: [],
    },
  ];

  const optimized = optimizer.optimizeExecutionOrder(steps as any);
  if (optimized[0].name === 'step1') {
    console.log('✓ Steps optimized by priority');
  } else {
    throw new Error('Optimization failed');
  }

  const concurrency = optimizer.calculateOptimalConcurrency();
  console.log(`✓ Optimal concurrency: ${concurrency}`);

  return true;
}

async function testConditionalExecutor() {
  console.log('Testing ConditionalExecutor...');
  const executor = new ConditionalExecutor();

  const workflow: WorkflowDefinition = {
    name: 'test-workflow',
    description: 'Test workflow',
    steps: [],
  };

  const context = createOrchestratorContext(workflow, TEST_DIR);
  context.previousResults.set('test-step', {
    success: true,
    summary: 'Test passed',
  });

  const condition = {
    type: 'equals' as const,
    field: 'previousResults.test-step.success',
    value: true,
  };

  const result = executor.evaluate(condition, context);
  if (result === true) {
    console.log('✓ Condition evaluated correctly');
  } else {
    throw new Error('Condition evaluation failed');
  }

  return true;
}

async function runAllTests() {
  console.log('🧪 Running Integration Tests\n');
  console.log('=' .repeat(50));
  console.log('');

  const tests = [
    { name: 'StateManager', fn: testStateManager },
    { name: 'ResultCache', fn: testResultCache },
    { name: 'ErrorRecovery', fn: testErrorRecovery },
    { name: 'PreflightValidator', fn: testPreflightValidator },
    { name: 'ReportingService', fn: testReportingService },
    { name: 'ParallelOptimizer', fn: testParallelOptimizer },
    { name: 'ConditionalExecutor', fn: testConditionalExecutor },
  ];

  let passed = 0;
  let failed = 0;

  for (const test of tests) {
    try {
      await test.fn();
      passed++;
      console.log('');
    } catch (error: any) {
      console.error(`✗ ${test.name} failed: ${error.message}`);
      failed++;
      console.log('');
    }
  }

  console.log('=' .repeat(50));
  console.log(`\n✅ Passed: ${passed}`);
  if (failed > 0) {
    console.log(`❌ Failed: ${failed}`);
    process.exit(1);
  } else {
    console.log('🎉 All tests passed!');
    process.exit(0);
  }
}

// Run tests
runAllTests().catch((error) => {
  console.error('Test suite failed:', error);
  process.exit(1);
});
