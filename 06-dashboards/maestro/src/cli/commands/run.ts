import { ConfigLoader as OrchestrationConfigLoader } from '../../config/ConfigLoader';
import { ConfigLoader as OrchestratorConfigLoader } from '../../config/OrchestratorConfig';
import { OrchestrationConfig } from '../../domain/WorkflowDefinition';
import { EnhancedWorkflowExecutor, ExecutionOptions } from '../../workflow/EnhancedWorkflowExecutor';
import { CursorCliAgentRunner } from '../../agents/CursorCliAgentRunner';
import { DummyAgentRunner } from '../../agents/DummyAgentRunner';
import { IAgentRunner } from '../../agents/AgentRunner';
import { findRepoRoot } from '../utils';
import { createOrchestratorContext } from '../../workflow/OrchestratorContext';
import { StateManager, MemoryStateStorage } from '../../services/StateManager';
import { getSignalHandler } from '../../services/SignalHandler';
import { ResultCache } from '../../services/ResultCache';
import { PreflightValidator } from '../../services/PreflightValidator';
import { ErrorRecovery } from '../../services/ErrorRecovery';
import { ReportingService } from '../../services/ReportingService';
import { getDashboardServer } from './dashboard';
import { TaskStatus } from '../../domain/AgentTask';
import { DashboardClient } from '../../services/DashboardClient';
import { PrivateDataSync } from '../../services/PrivateDataSync';
import { ConfigLoader } from '../../config/OrchestratorConfig';

export interface RunOptions {
  flow: string;
  config?: string;
  dryRun?: boolean;
  runner?: 'dummy' | 'cursor';
  model?: string; // Model to use for cursor-agent
  feature?: string;
  logLevel?: string;
  output?: string;
  maxConcurrency?: number;
  continueOnError?: boolean;
  abortOnError?: boolean;
  verbose?: boolean;
  resume?: string; // Checkpoint ID to resume from
  enableCheckpoints?: boolean; // Enable checkpoint saving
  autoSaveInterval?: number; // Auto-save interval in seconds
  stop?: boolean; // Request stop (for interactive mode)
  enableCache?: boolean; // Enable result caching
  cacheTtl?: number; // Cache TTL in days
  enableRetry?: boolean; // Enable error retry
  maxRetries?: number; // Maximum retry attempts
  retryInitialDelay?: number; // Initial retry delay in seconds
  interactive?: boolean; // Enable interactive mode
  generateReport?: boolean; // Generate execution report
  reportFormat?: 'json' | 'html' | 'markdown'; // Report format
  optimizeParallel?: boolean; // Enable parallel execution optimization
}

export async function runWorkflow(options: RunOptions): Promise<void> {
  try {
    const loader = new OrchestrationConfigLoader();
    const config = loader.loadConfig(options.config);

    // Find workflow
    const workflow = config.flows[options.flow];
    if (!workflow) {
      console.error(`❌ Workflow "${options.flow}" not found.`);
      console.log('\nAvailable workflows:');
      for (const name of Object.keys(config.flows)) {
        console.log(`  - ${name}`);
      }
      process.exit(1);
    }

    // Build workflows map for nested workflow support
    const workflowsMap = new Map(Object.entries(config.flows));

    const repoRoot = findRepoRoot();

    // Initialize state manager for checkpoint/resume
    const stateStorage = new MemoryStateStorage(repoRoot);
    const stateManager = new StateManager(stateStorage, options.verbose || false);

    // Initialize result cache
    const cacheTtlMs = options.cacheTtl 
      ? options.cacheTtl * 24 * 60 * 60 * 1000 // Convert days to ms
      : undefined;
    const resultCache = new ResultCache(repoRoot, {
      enabled: options.enableCache ?? true,
      ttl: cacheTtlMs,
    });

    // Initialize error recovery
    const retryInitialDelayMs = options.retryInitialDelay
      ? options.retryInitialDelay * 1000 // Convert seconds to ms
      : undefined;
    const errorRecovery = new ErrorRecovery({
      maxRetries: options.maxRetries || 3,
      initialDelay: retryInitialDelayMs || 1000,
      jitter: true,
    });

    // Initialize reporting service
    const reportingService = new ReportingService(repoRoot);

    // Create executor with config, state manager, cache, error recovery, and reporting
    const executor = new EnhancedWorkflowExecutor(
      config,
      workflowsMap,
      stateManager,
      resultCache,
      errorRecovery,
      reportingService
    );

    // Create context
    const context = createOrchestratorContext(
      workflow,
      repoRoot,
      options.feature
    );

    // Get execution plan
    const plan = executor.getExecutionPlan(workflow, context);

    console.log(`\n🚀 Workflow: ${options.flow}`);
    console.log(`📁 Repository: ${repoRoot}`);
    if (options.feature) {
      console.log(`✨ Feature: ${options.feature}`);
    }
    console.log(`\n📋 Execution Plan (${plan.tasks.length} steps in ${plan.groups.length} groups):\n`);

    // Display plan with parallel groups
    let stepNumber = 1;
    for (const group of plan.groups) {
      if (group.canRunInParallel && group.steps.length > 1) {
        console.log(`  📦 Parallel Group ${group.order + 1} (${group.steps.length} steps):`);
        for (const step of group.steps) {
          const task = plan.tasks.find((t) => t.stepName === step.name);
          const roleName = typeof task?.role === 'string' ? task.role : task?.role.name || step.agent;
          const deps = step.dependsOn || [];
          
          console.log(`    ${stepNumber++}. ${step.name}`);
          console.log(`       Role: ${roleName}`);
          console.log(`       Description: ${step.description.substring(0, 80)}...`);
          if (deps.length > 0) {
            console.log(`       Depends on: ${deps.join(', ')}`);
          }
          if (step.nestedWorkflow) {
            console.log(`       🔄 Nested workflow: ${step.nestedWorkflow}`);
          }
          if (step.spawnTasks && step.spawnTasks.length > 0) {
            console.log(`       🌱 Spawns ${step.spawnTasks.length} sub-task(s)`);
          }
          console.log('');
        }
      } else {
        for (const step of group.steps) {
          const task = plan.tasks.find((t) => t.stepName === step.name);
          const roleName = typeof task?.role === 'string' ? task.role : task?.role.name || step.agent;
          const deps = step.dependsOn || [];
          
          console.log(`  ${stepNumber++}. ${step.name}`);
          console.log(`     Role: ${roleName}`);
          console.log(`     Description: ${step.description.substring(0, 80)}...`);
          if (deps.length > 0) {
            console.log(`     Depends on: ${deps.join(', ')}`);
          }
          if (step.nestedWorkflow) {
            console.log(`     🔄 Nested workflow: ${step.nestedWorkflow}`);
          }
          if (step.spawnTasks && step.spawnTasks.length > 0) {
            console.log(`     🌱 Spawns ${step.spawnTasks.length} sub-task(s)`);
          }
          console.log('');
        }
      }
    }

    if (options.dryRun) {
      console.log('✅ Dry-run complete. Use without --dry-run to execute.');
      return;
    }

    // Load orchestrator config for timeout settings
    let orchestratorConfig;
    try {
      orchestratorConfig = await OrchestratorConfigLoader.load();
    } catch (error) {
      // Use defaults if config load fails
      orchestratorConfig = null;
    }

    // Select runner with timeout from config
    // Default timeout: 30 minutes (1800000ms) for complex analysis tasks
    // Config timeout is in seconds, convert to milliseconds
    const timeoutMs = orchestratorConfig?.execution?.defaultTimeout 
      ? orchestratorConfig.execution.defaultTimeout * 1000
      : 1800000; // 30 minutes default

    console.log(`⏱️  Task timeout: ${Math.round(timeoutMs / 60000)} minutes`);
    if (options.verbose) {
      console.log(`\n${'='.repeat(60)}`);
      console.log(`🔍 VERBOSE MODE ENABLED`);
      console.log(`${'='.repeat(60)}`);
      console.log(`📝 Full prompts will be shown`);
      console.log(`📥 All stdout/stderr output will be displayed`);
      console.log(`📊 Complete task results will be shown`);
      console.log(`💾 Checkpoint saves will be logged`);
      console.log(`${'='.repeat(60)}\n`);
    }

    console.log(`\n🤖 Using runner: ${options.runner || 'dummy'}`);
    if (options.maxConcurrency) {
      console.log(`⚡ Max concurrency: ${options.maxConcurrency}`);
    }
    console.log('');

    // Handle resume
    if (options.resume) {
      const checkpoint = await stateManager.load(options.resume);
      if (!checkpoint) {
        console.error(`❌ Checkpoint not found: ${options.resume}`);
        console.log('\nAvailable checkpoints:');
        const checkpoints = await stateManager.list();
        if (checkpoints.length === 0) {
          console.log('  No checkpoints available.');
        } else {
          checkpoints.forEach(cp => {
            console.log(`  - ${cp.id} (${cp.workflowName}, started: ${cp.startedAt.toISOString()})`);
          });
        }
        process.exit(1);
      }
    }

    // Handle stop request
    if (options.stop) {
      getSignalHandler().requestShutdown();
      return;
    }

    // Pre-flight validation
    console.log('🔍 Running pre-flight validation...\n');
    const validator = new PreflightValidator(repoRoot);
    const validation = await validator.validateWorkflow(workflow);
    
    if (validation.errors.length > 0) {
      console.error('❌ Pre-flight validation failed:\n');
      validation.errors.forEach((error) => {
        console.error(`   • ${error}`);
      });
      console.log('');
      process.exit(1);
    }

    if (validation.warnings.length > 0) {
      console.log('⚠️  Pre-flight warnings:\n');
      validation.warnings.forEach((warning) => {
        console.log(`   • ${warning}`);
      });
      console.log('');
    }

    if (validation.valid) {
      console.log('✅ Pre-flight validation passed\n');
    }

    // Execution options
    const execOptions: ExecutionOptions = {
      maxConcurrency: options.maxConcurrency,
      continueOnError: options.continueOnError ?? true,
      abortOnError: options.abortOnError ?? false,
      checkpointId: options.resume,
      enableCheckpoints: options.enableCheckpoints ?? true,
      autoSaveInterval: options.autoSaveInterval ? options.autoSaveInterval * 1000 : 60000,
      enableCache: options.enableCache ?? true,
      cacheTtl: cacheTtlMs,
      enableRetry: options.enableRetry ?? true,
      maxRetries: options.maxRetries || 3,
      retryInitialDelay: retryInitialDelayMs || 1000,
      interactive: options.interactive ?? false,
      generateReport: options.generateReport ?? false,
      reportFormat: options.reportFormat || 'markdown',
      optimizeParallel: options.optimizeParallel ?? false,
    };

    // Execute workflow
    const startTime = new Date();
    const dashboardServer = getDashboardServer();
    
    // Get activity tracker from dashboard server if available
    let activityTracker;
    if (dashboardServer) {
      activityTracker = (dashboardServer as any).activityTracker;
    }
    
    // Try to connect to dashboard via HTTP (for cross-process communication)
    const dashboardClient = new DashboardClient({ port: 3000 });
    const dashboardAvailable = await dashboardClient.isAvailable();
    
    // Use options.flow as the workflow name to match what dashboard registered
    // The workflow.name might differ from options.flow (the key in config.flows)
    // This ensures the workflow name used in dashboard matches what was registered
    const workflowNameForDashboard = options.flow || workflow.name;
    
    if (dashboardAvailable) {
      console.log('📊 Dashboard detected - workflow will be monitored in real-time\n');
      console.log(`📊 Registering workflow "${workflowNameForDashboard}" with dashboard (${workflow.steps.length} steps)`);
      await dashboardClient.recordWorkflowStart(workflowNameForDashboard, workflow.steps.length);
    } else {
      // Silently continue without dashboard - don't log errors
    }
    
    // Set dashboard server in executor if available (same process)
    if (dashboardServer) {
      (executor as any).dashboardServer = dashboardServer;
    }

    // Get cursor-agent GUID from dashboard if available (for context persistence)
    let cursorAgentGuid: string | undefined;
    if (dashboardServer) {
      const workflowNameForDashboard = options.flow || workflow.name;
      const existingMetrics = (dashboardServer as any).activeWorkflows?.get(workflowNameForDashboard);
      if (existingMetrics?.cursorAgentGuid) {
        cursorAgentGuid = existingMetrics.cursorAgentGuid;
        console.log(`🔄 Resuming cursor-agent context: ${cursorAgentGuid}`);
      }
    }

    // Create runner with activity tracker, token usage callback, and cursor-agent GUID
    const runner: IAgentRunner = options.runner === 'cursor'
      ? new CursorCliAgentRunner({ 
          timeout: timeoutMs,
          verbose: options.verbose || false,
          model: options.model, // Pass model selection
          activityTracker: activityTracker,
          workflowName: workflow.name,
          cursorAgentGuid: cursorAgentGuid, // Pass GUID for --resume
          onTokenUsage: (usage) => {
            // Report token usage to dashboard
            if (dashboardServer) {
              // Get current step from context if available
              const currentStep = (context as any).currentStep || '';
              const workflowNameForDashboard = options.flow || workflow.name;
              dashboardServer.recordTokenUsage(workflowNameForDashboard, currentStep, {
                ...usage,
                timestamp: new Date(),
                stepName: currentStep,
                workflowName: workflowNameForDashboard,
              });
            }
          },
        }, repoRoot)
      : new DummyAgentRunner();

    // Pass model to executor options
    const execOptionsWithModel = {
      ...execOptions,
      startTime,
      model: options.model, // Pass model for workflow start
    } as any;

    const result = await executor.execute(workflow, runner, context, execOptionsWithModel, async (task) => {
      // Update context with current step for token tracking
      (context as any).currentStep = task.stepName;

      // Determine status for dashboard
      let dashboardStatus: 'running' | 'completed' | 'failed';
      if (task.status === TaskStatus.Succeeded) {
        dashboardStatus = 'completed';
      } else if (task.status === TaskStatus.Failed) {
        dashboardStatus = 'failed';
      } else {
        dashboardStatus = 'running';
      }
      
      // Update dashboard on task update (both same-process and HTTP)
      // Use workflowNameForDashboard defined above to ensure name consistency
      if (dashboardServer) {
        dashboardServer.recordStepUpdate(workflowNameForDashboard, task.stepName, dashboardStatus);
      }
      
      // Also send via HTTP if dashboard is running in separate process
      // Always try HTTP even if same-process, in case dashboard is in separate process
      if (dashboardAvailable) {
        await dashboardClient.recordStepUpdate(workflowNameForDashboard, task.stepName, dashboardStatus).catch((error) => {
          // Log error for debugging
          if (options.verbose) {
            console.warn(`⚠️  Failed to send step update to dashboard: ${error.message}`);
          }
        });
      } else if (options.verbose) {
        console.warn(`⚠️  Dashboard not available - step updates will not be sent`);
      }
      
      // Status messages are handled by CursorCliAgentRunner, so we don't duplicate here
      // Only show step start to give context
      if (task.status === TaskStatus.Running && task.createdAt.getTime() === task.updatedAt.getTime()) {
        // First time this task is running - show step header
        console.log(`\n📋 ${task.stepName}`);
      }
    });

    const duration = ((Date.now() - startTime.getTime()) / 1000).toFixed(2);
    const durationMs = Date.now() - startTime.getTime();

    // Sync to private repository if enabled
    try {
      const orchestratorConfig = await ConfigLoader.load();
      if (orchestratorConfig.privateSync?.enabled) {
        const privateSync = new PrivateDataSync(orchestratorConfig.privateSync, repoRoot);
        await privateSync.initialize();
        const syncResult = await privateSync.sync(repoRoot);
        if (syncResult.success && syncResult.syncedFiles.length > 0) {
          console.log(`\n📤 Synced ${syncResult.syncedFiles.length} file(s) to private repository`);
        }
      }
    } catch (error: any) {
      // Silently ignore sync errors - don't fail workflow execution
      if (options.verbose) {
        console.warn(`⚠️  Private sync failed: ${error.message}`);
      }
    }

    // Notify dashboard of workflow end (use workflowNameForDashboard defined above)
    if (dashboardAvailable) {
      await dashboardClient.recordWorkflowEnd(
        workflowNameForDashboard,
        result.success,
        result.completedTasks.length,
        result.failedTasks.length,
        durationMs
      );
    }

    // Display results with better formatting
    console.log(`\n${'='.repeat(60)}`);
    console.log(`📊 Execution Summary`);
    console.log(`${'='.repeat(60)}`);
    console.log(`⏱️  Duration: ${duration}s`);
    console.log(`✅ Completed: ${result.completedTasks.length}`);
    console.log(`❌ Failed: ${result.failedTasks.length}`);
    
    if (result.failedTasks.length > 0) {
      console.log(`\nFailed tasks:`);
      result.failedTasks.forEach(task => {
        console.log(`   ❌ ${task.stepName}`);
      });
    }
    
    if (result.nestedWorkflows.size > 0) {
      console.log(`🔄 Nested workflows: ${result.nestedWorkflows.size}`);
      for (const [name, nestedResult] of result.nestedWorkflows.entries()) {
        console.log(`   ${name}: ${nestedResult.success ? '✅' : '❌'} (${nestedResult.completedTasks.length} completed, ${nestedResult.failedTasks.length} failed)`);
      }
    }

    if (result.failedTasks.length > 0) {
      console.log(`\n❌ Failed Tasks:\n`);
      for (const task of result.failedTasks) {
        console.log(`  - ${task.stepName}`);
      }
    }

    if (!result.success) {
      process.exit(1);
    }
  } catch (error: any) {
    console.error('❌ Error executing workflow:', error.message);
    if (error.stack) {
      console.error(error.stack);
    }
    process.exit(1);
  }
}
