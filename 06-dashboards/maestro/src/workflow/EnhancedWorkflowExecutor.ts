import { WorkflowDefinition, OrchestrationConfig } from '../domain/WorkflowDefinition';
import { AgentTask, TaskStatus } from '../domain/AgentTask';
import { ParallelDependencyResolver, ResolvedStepGroup } from './ParallelDependencyResolver';
import { OrchestratorContext, AgentResult } from './OrchestratorContext';
import { IAgentRunner } from '../agents/AgentRunner';
import { AgentRole } from '../domain/AgentRole';
import { StepDefinition, SpawnTaskDefinition } from '../domain/StepDefinition';
import { v4 as uuidv4 } from 'uuid';
import { StateManager } from '../services/StateManager';
import { getSignalHandler } from '../services/SignalHandler';
import { ResultCache } from '../services/ResultCache';
import { ErrorRecovery, ErrorClassifier, ErrorType } from '../services/ErrorRecovery';
import { InteractiveMode, InteractiveCommand } from '../services/InteractiveMode';
import { ReportingService, ExecutionMetrics } from '../services/ReportingService';
import { ParallelOptimizer } from '../services/ParallelOptimizer';
import { ConditionalExecutor } from '../services/ConditionalExecution';
import { DashboardServer } from '../server/DashboardServer';
import { OutputMonitor } from '../services/OutputMonitor';
import { TaskProgressTracker } from '../services/TaskProgressTracker';
import { ModelManager } from '../services/ModelManager';
import { AgentSwitcher } from '../services/AgentSwitcher';
import { ModelSelector } from '../services/ModelSelector';

export interface ExecutionResult {
  success: boolean;
  completedTasks: AgentTask[];
  failedTasks: AgentTask[];
  nestedWorkflows: Map<string, ExecutionResult>;
}

export interface ExecutionOptions {
  maxConcurrency?: number; // Global max concurrency
  continueOnError?: boolean; // Continue execution even if a task fails
  abortOnError?: boolean; // Abort entire workflow on first error
  checkpointId?: string; // Resume from checkpoint
  enableCheckpoints?: boolean; // Enable checkpoint saving
  autoSaveInterval?: number; // Auto-save interval in ms
  enableCache?: boolean; // Enable result caching
  cacheTtl?: number; // Cache TTL in ms
  enableRetry?: boolean; // Enable error retry with exponential backoff
  maxRetries?: number; // Maximum retry attempts per task
  retryInitialDelay?: number; // Initial retry delay in ms
  interactive?: boolean; // Enable interactive mode
  generateReport?: boolean; // Generate execution report
  reportFormat?: 'json' | 'html' | 'markdown'; // Report format
  optimizeParallel?: boolean; // Enable parallel execution optimization
  startTime?: Date; // Workflow start time (for dashboard)
}

export class EnhancedWorkflowExecutor {
  private resolver: ParallelDependencyResolver;
  private config?: OrchestrationConfig;
  private workflows?: Map<string, WorkflowDefinition>;
  private stateManager?: StateManager;
  private resultCache?: ResultCache;
  private errorRecovery?: ErrorRecovery;
  private interactiveMode?: InteractiveMode;
  private reportingService?: ReportingService;
  private parallelOptimizer?: ParallelOptimizer;
  private conditionalExecutor?: ConditionalExecutor;
  private outputMonitor?: OutputMonitor;
  private taskProgressTracker?: TaskProgressTracker;
  private modelManager?: ModelManager;
  private agentSwitcher?: AgentSwitcher;
  private modelSelector?: ModelSelector;
  private signalHandler = getSignalHandler();

  constructor(
    config?: OrchestrationConfig,
    workflows?: Map<string, WorkflowDefinition>,
    stateManager?: StateManager,
    resultCache?: ResultCache,
    errorRecovery?: ErrorRecovery,
    reportingService?: ReportingService
  ) {
    this.resolver = new ParallelDependencyResolver();
    this.config = config;
    this.workflows = workflows;
    this.stateManager = stateManager;
    this.resultCache = resultCache;
    this.errorRecovery = errorRecovery;
    this.reportingService = reportingService;

    // Register shutdown handler
    if (this.stateManager) {
      this.signalHandler.onShutdown(async () => {
        await this.stateManager!.shutdown();
      });
    }
  }

  async execute(
    workflow: WorkflowDefinition,
    runner: IAgentRunner,
    context: OrchestratorContext,
    options: ExecutionOptions = {},
    onTaskUpdate?: (task: AgentTask) => void
  ): Promise<ExecutionResult> {
    const maxConcurrency = options.maxConcurrency || Number.MAX_SAFE_INTEGER;
    const continueOnError = options.continueOnError ?? true;
    const abortOnError = options.abortOnError ?? false;
    const enableCheckpoints = options.enableCheckpoints ?? true;
    const enableCache = options.enableCache ?? true;
    const enableRetry = options.enableRetry ?? true;
    const interactive = options.interactive ?? false;
    const generateReport = options.generateReport ?? false;
    const optimizeParallel = options.optimizeParallel ?? false;

    // Initialize parallel optimizer if enabled
    if (optimizeParallel) {
      this.parallelOptimizer = new ParallelOptimizer();
      // Adjust max concurrency based on optimizer
      if (this.parallelOptimizer) {
        const optimalConcurrency = this.parallelOptimizer.calculateOptimalConcurrency();
        if (optimalConcurrency < maxConcurrency) {
          console.log(`⚡ Optimized concurrency: ${optimalConcurrency} (was ${maxConcurrency})`);
        }
      }
    }

    // Initialize conditional executor
    this.conditionalExecutor = new ConditionalExecutor();

    // Record workflow start in dashboard if available
    const dashboardServerStart = (this as any).dashboardServer as DashboardServer | undefined;
    if (dashboardServerStart) {
      // Get services from dashboard
      const services = dashboardServerStart.getServices();
      this.outputMonitor = services.outputMonitor;
      this.taskProgressTracker = services.taskProgressTracker;
      this.modelManager = services.modelManager;
      this.agentSwitcher = services.agentSwitcher;
      
      // Initialize model selector if model manager is available
      if (this.modelManager) {
        this.modelSelector = new ModelSelector(this.modelManager);
      }

      // Initialize progress tracking
      if (this.taskProgressTracker) {
        const plan = this.getExecutionPlan(workflow, context);
        this.taskProgressTracker.initializeWorkflow(workflow.name, plan.tasks);
      }

      // Get model from dashboard or options, or use auto-selection
      let modelId = (options as any).model || this.modelManager?.getWorkflowModel(workflow.name) || this.modelManager?.getDefaultModel();
      
      // If model is "auto" or not specified, use auto-selection
      if ((!modelId || modelId === 'auto' || this.modelManager?.isAutoModel(modelId)) && this.modelSelector && workflow.steps.length > 0) {
        const firstStep = workflow.steps[0];
        const firstTask: AgentTask = {
          id: uuidv4(),
          stepName: firstStep.name,
          description: firstStep.description,
          flowName: workflow.name,
          role: typeof firstStep.agent === 'string' ? firstStep.agent : (firstStep.agent as any).name || firstStep.agent,
          status: TaskStatus.Pending,
          createdAt: new Date(),
          updatedAt: new Date(),
        };
        const recommendation = this.modelSelector.recommendModel(firstTask, {
          totalSteps: workflow.steps.length,
          currentStep: 0,
        });
        modelId = recommendation.modelId;
        console.log(`🤖 Auto-selected model: ${recommendation.modelInfo.name} (${recommendation.reasoning})`);
        console.log(`   Estimated cost: $${recommendation.estimatedCost.toFixed(4)}`);
      }
      
      dashboardServerStart.recordWorkflowStart(workflow, modelId);
    }

    // Initialize interactive mode if enabled
    if (interactive) {
      this.interactiveMode = new InteractiveMode(workflow);
      await this.interactiveMode.start();
      
      // Setup command handlers
      this.interactiveMode.onCommandReceived(async (command: InteractiveCommand) => {
        // Handle interactive commands
        if (command.command === 'abort') {
          throw new Error('Workflow aborted by user');
        }
      });
    }

    // Load checkpoint if resuming
    let checkpointId: string | undefined = options.checkpointId;
    if (checkpointId && this.stateManager) {
      const checkpoint = await this.stateManager.load(checkpointId);
      if (checkpoint) {
        console.log(`\n🔄 Resuming from checkpoint: ${checkpointId}`);
        console.log(`   Completed: ${checkpoint.completedTasks.length} tasks`);
        console.log(`   Failed: ${checkpoint.failedTasks.length} tasks`);
        console.log(`   Started: ${checkpoint.startedAt.toISOString()}\n`);

        // Restore context from checkpoint
        context.previousResults = checkpoint.context.previousResults;
        
        // Create new checkpoint for this execution
        checkpointId = this.stateManager.createCheckpoint(
          workflow.name,
          context,
          checkpoint.completedTasks,
          checkpoint.failedTasks,
          checkpoint.currentStep,
          checkpoint.metadata
        ).id;

        // Enable auto-save
        if (enableCheckpoints && this.stateManager) {
          this.stateManager.enableAutoSave(options.autoSaveInterval || 60000);
        }
      }
    } else if (enableCheckpoints && this.stateManager) {
      // Create new checkpoint
      checkpointId = this.stateManager.createCheckpoint(
        workflow.name,
        context,
        [],
        [],
        undefined,
        {
          maxConcurrency,
          continueOnError,
          abortOnError,
        }
      ).id;

      // Enable auto-save
      this.stateManager.enableAutoSave(options.autoSaveInterval || 60000);
    }

    // Apply conditional filtering if needed
    let stepsToExecute = workflow.steps;
    if (this.conditionalExecutor) {
      // Filter steps based on conditions
      stepsToExecute = this.conditionalExecutor.filterSteps(
        workflow.steps as any[], // Cast to ConditionalStep[]
        context
      );
    }

    // Resolve dependencies into parallel groups
    const stepGroups = this.resolver.resolve(stepsToExecute);

    const completedTasks: AgentTask[] = [];
    const failedTasks: AgentTask[] = [];
    const nestedWorkflows = new Map<string, ExecutionResult>();

    // Filter out already completed tasks if resuming
    const completedTaskNames = new Set<string>();
    if (checkpointId && this.stateManager) {
      const checkpoint = this.stateManager.getCurrentCheckpoint();
      if (checkpoint) {
        checkpoint.completedTasks.forEach(name => completedTaskNames.add(name));
      }
    }

    // Execute groups sequentially, but steps within groups in parallel
    for (const group of stepGroups) {
      // Check for shutdown signal
      if (this.signalHandler.isShuttingDown()) {
        console.log('\n⚠️  Shutdown detected. Saving checkpoint...');
        if (this.stateManager && checkpointId) {
          this.stateManager.updateCheckpoint(
            completedTasks.map(t => t.stepName),
            failedTasks.map(t => t.stepName),
            group.steps[0]?.name
          );
          await this.stateManager.save();
          console.log(`💾 Checkpoint saved: ${checkpointId}`);
          console.log(`💡 Resume with: --resume ${checkpointId}\n`);
        }
        throw new Error('Workflow interrupted by user');
      }

      if (abortOnError && failedTasks.length > 0) {
        console.log(`Aborting workflow due to previous errors`);
        break;
      }

      if (group.canRunInParallel && group.steps.length > 1) {
        // Execute steps in parallel (with concurrency limit)
        await this.executeParallel(
          group.steps,
          workflow,
          runner,
          context,
          maxConcurrency,
          completedTasks,
          failedTasks,
          nestedWorkflows,
          continueOnError,
          abortOnError,
          onTaskUpdate,
          enableCache,
          options.cacheTtl,
          enableRetry
        );
      } else {
        // Execute steps sequentially
        for (const step of group.steps) {
          // Skip if already completed (when resuming)
          if (completedTaskNames.has(step.name)) {
            console.log(`⏭️  Skipping already completed step: ${step.name}`);
            continue;
          }

          // Check for shutdown signal
          if (this.signalHandler.isShuttingDown()) {
            console.log('\n⚠️  Shutdown detected. Saving checkpoint...');
            if (this.stateManager && checkpointId) {
              this.stateManager.updateCheckpoint(
                completedTasks.map(t => t.stepName),
                failedTasks.map(t => t.stepName),
                step.name
              );
              await this.stateManager.save();
              console.log(`💾 Checkpoint saved: ${checkpointId}`);
              console.log(`💡 Resume with: --resume ${checkpointId}\n`);
            }
            throw new Error('Workflow interrupted by user');
          }

          // Check interactive mode pause
          if (this.interactiveMode && this.interactiveMode.isPaused()) {
            // Wait for continue command
            while (this.interactiveMode.isPaused()) {
              await new Promise(resolve => setTimeout(resolve, 100));
            }
          }

          if (abortOnError && failedTasks.length > 0) {
            break;
          }

          const result = await this.executeStep(
            step,
            workflow,
            runner,
            context,
            nestedWorkflows,
            onTaskUpdate,
            options.enableCache ?? true,
            options.cacheTtl,
            enableRetry
          );

          if (result.success) {
            completedTasks.push(result.task);
            context.previousResults.set(step.name, result.result);
            
            // Update checkpoint after each successful step
            if (this.stateManager && checkpointId) {
              this.stateManager.updateCheckpoint(
                completedTasks.map(t => t.stepName),
                failedTasks.map(t => t.stepName),
                step.name
              );
            }
          } else {
            failedTasks.push(result.task);
            
            // Update checkpoint after failure
            if (this.stateManager && checkpointId) {
              this.stateManager.updateCheckpoint(
                completedTasks.map(t => t.stepName),
                failedTasks.map(t => t.stepName),
                step.name
              );
            }
            
            if (!continueOnError) {
              break;
            }
          }
        }
      }
    }

    // Final checkpoint save
    if (this.stateManager && checkpointId) {
      this.stateManager.updateCheckpoint(
        completedTasks.map(t => t.stepName),
        failedTasks.map(t => t.stepName)
      );
      await this.stateManager.save();
      this.stateManager.disableAutoSave();
      
      if (failedTasks.length === 0) {
        // Don't show checkpoint ID unless user explicitly asks
        console.log(`\n✅ Workflow completed successfully`);
      } else {
        console.log(`\n⚠️  Workflow completed with errors`);
        console.log(`💡 Resume with: --resume ${checkpointId}\n`);
      }
    }

    // Generate report if requested
    if (generateReport && this.reportingService) {
      const startTime = new Date(Date.now() - (options.autoSaveInterval || 60000));
      const endTime = new Date();
      const duration = endTime.getTime() - startTime.getTime();

      const metrics: Partial<ExecutionMetrics> = {
        startTime,
        endTime,
        duration,
        completedTasks: completedTasks.length,
        failedTasks: failedTasks.length,
      };

      const reportPath = await this.reportingService.generateReport(
        workflow,
        {
          success: failedTasks.length === 0,
          completedTasks,
          failedTasks,
          nestedWorkflows,
        },
        metrics,
        {
          format: options.reportFormat || 'markdown',
          includeDetails: true,
          includeTrends: true,
        }
      );

      console.log(`\n📊 Execution report generated: ${reportPath}\n`);
    }

    // Close interactive mode
    if (this.interactiveMode) {
      this.interactiveMode.close();
    }

    // Record workflow end in dashboard if available
    const dashboardServerEnd = (this as any).dashboardServer as DashboardServer | undefined;
    if (dashboardServerEnd) {
      const endTime = new Date();
      const startTimeMs = options.startTime?.getTime() || Date.now();
      const duration = endTime.getTime() - startTimeMs;
      dashboardServerEnd.recordWorkflowEnd(workflow.name, {
        success: failedTasks.length === 0,
        completedTasks,
        failedTasks,
        nestedWorkflows,
      }, duration);
    }

    return {
      success: failedTasks.length === 0,
      completedTasks,
      failedTasks,
      nestedWorkflows,
    };
  }

  private async executeParallel(
    steps: StepDefinition[],
    workflow: WorkflowDefinition,
    runner: IAgentRunner,
    context: OrchestratorContext,
    maxConcurrency: number,
    completedTasks: AgentTask[],
    failedTasks: AgentTask[],
    nestedWorkflows: Map<string, ExecutionResult>,
    continueOnError: boolean,
    abortOnError: boolean,
    onTaskUpdate?: (task: AgentTask) => void,
    enableCache: boolean = true,
    cacheTtl?: number,
    enableRetry: boolean = true
  ): Promise<void> {
    const executing: Promise<void>[] = [];
    let activeCount = 0;

    for (const step of steps) {
      // Wait if we've reached max concurrency
      while (activeCount >= maxConcurrency) {
        await Promise.race(executing);
        executing.splice(
          executing.findIndex((p) => p === Promise.race(executing)),
          1
        );
        activeCount--;
      }

      if (abortOnError && failedTasks.length > 0) {
        break;
      }

      activeCount++;
      const promise = this.executeStep(
        step,
        workflow,
        runner,
        context,
        nestedWorkflows,
        onTaskUpdate,
        enableCache,
        cacheTtl,
        enableRetry
      ).then((result) => {
        activeCount--;
        if (result.success) {
          completedTasks.push(result.task);
          context.previousResults.set(step.name, result.result);
        } else {
          failedTasks.push(result.task);
        }
      });

      executing.push(promise);
    }

    // Wait for all remaining tasks
    await Promise.all(executing);
  }

  private async executeStep(
    step: StepDefinition,
    workflow: WorkflowDefinition,
    runner: IAgentRunner,
    context: OrchestratorContext,
    nestedWorkflows: Map<string, ExecutionResult>,
    onTaskUpdate?: (task: AgentTask) => void,
    enableCache: boolean = true,
    cacheTtl?: number,
    enableRetry: boolean = true
  ): Promise<{ task: AgentTask; result: AgentResult; success: boolean }> {
    const role = this.getRole(step.agent, context);
    const task = this.createTask(step, workflow.name, role);

    // Auto-select model for this task if model is "auto" or not set
    if (this.modelSelector && this.modelManager) {
      const workflowModel = this.modelManager.getWorkflowModel(workflow.name);
      const stepModel = this.modelManager.getStepModel(workflow.name, step.name);
      
      // Auto-select if model is "auto", empty, or not explicitly set
      const shouldAutoSelect = !workflowModel && !stepModel;
      const isAutoModel = workflowModel === 'auto' || stepModel === 'auto' || 
                         this.modelManager.isAutoModel(workflowModel || '') || 
                         this.modelManager.isAutoModel(stepModel || '');
      
      if (shouldAutoSelect || isAutoModel) {
        const recommendation = this.modelSelector.recommendModel(task, {
          totalSteps: workflow.steps.length,
          currentStep: workflow.steps.indexOf(step),
        });
        
        // Set model for this step
        this.modelManager.setStepModel(workflow.name, step.name, recommendation.modelId);
        
        // Log selection
        console.log(`\n🎯 Auto-selected model for "${step.name}": ${recommendation.modelInfo.name}`);
        console.log(`   Reasoning: ${recommendation.reasoning}`);
        console.log(`   Estimated cost: $${recommendation.estimatedCost.toFixed(4)}`);
      }
    }

    // Get model for this step (falls back to workflow model, or auto-selected)
    let stepModel = this.modelManager?.getStepModel(workflow.name, step.name);
    
    // If model is "auto", don't pass it to runner - let ModelSelector handle it per step
    // Only pass explicit model IDs to the runner
    if (stepModel && !this.modelManager?.isAutoModel(stepModel) && (runner as any).options) {
      (runner as any).options.model = stepModel;
    }

    try {
      // Update task status
      task.status = TaskStatus.Running;
      task.updatedAt = new Date();
      if (onTaskUpdate) {
        onTaskUpdate(task);
      }

      // Handle nested workflow (not cached)
      if (step.nestedWorkflow) {
        const nestedResult = await this.executeNestedWorkflow(
          step.nestedWorkflow,
          runner,
          context,
          onTaskUpdate
        );
        nestedWorkflows.set(step.nestedWorkflow, nestedResult);

        task.status = nestedResult.success ? TaskStatus.Succeeded : TaskStatus.Failed;
        task.updatedAt = new Date();

        return {
          task,
          result: {
            success: nestedResult.success,
            summary: nestedResult.success
              ? `Nested workflow "${step.nestedWorkflow}" completed successfully`
              : `Nested workflow "${step.nestedWorkflow}" failed`,
            rawOutput: JSON.stringify(nestedResult, null, 2),
          },
          success: nestedResult.success,
        };
      }

      // Check cache before executing
      let result: AgentResult;
      let fromCache = false;

      if (enableCache && this.resultCache) {
        // Build prompt for cache key generation
        const { PromptBuilder } = await import('../agents/PromptBuilder');
        const promptBuilder = new PromptBuilder(context.repositoryRoot);
        const prompt = await promptBuilder.buildPrompt(task, context);

        // Try to get from cache
        const cachedResult = await this.resultCache.get(task, prompt);
        if (cachedResult) {
          result = cachedResult;
          fromCache = true;
          console.log(`✅ Using cached result for: ${task.stepName}`);
        } else {
          // Execute task
          result = await runner.runTask(task, context);
          
          // Store in cache
          await this.resultCache.set(task, prompt, result, cacheTtl);
        }
      } else {
        // Execute task with optional retry
        if (enableCache && enableRetry && this.errorRecovery) {
          const retryResult = await this.errorRecovery.retry(
            async () => await runner.runTask(task, context),
            (error: any) => ErrorClassifier.classify(error)
          );

          if (retryResult.success && retryResult.result) {
            result = retryResult.result;
            if (retryResult.attempts > 1) {
              console.log(`✅ Task succeeded after ${retryResult.attempts} attempts: ${task.stepName}`);
            }
          } else {
            result = {
              success: false,
              summary: `Task failed after ${retryResult.attempts} attempts: ${retryResult.error?.message || 'Unknown error'}`,
              error: retryResult.error?.message || 'Unknown error',
            };
          }
        } else {
          // Execute without retry
          result = await runner.runTask(task, context);
        }
      }

      // Handle dynamic task spawning
      if (result.success && step.spawnTasks) {
        const spawnedResults = await this.spawnTasks(
          step.spawnTasks,
          task,
          runner,
          context,
          onTaskUpdate
        );
        result.rawOutput = JSON.stringify({
          main: result,
          spawned: spawnedResults,
        });
      }

      task.status = result.success ? TaskStatus.Succeeded : TaskStatus.Failed;
      task.updatedAt = new Date();
      if (onTaskUpdate) {
        onTaskUpdate(task);
      }

      // Monitor output and track progress
      const dashboardServer = (this as any).dashboardServer as DashboardServer | undefined;
      if (dashboardServer && this.outputMonitor && this.taskProgressTracker) {
        // Analyze output
        const analysis = this.outputMonitor.analyzeOutput(
          workflow.name,
          task.stepName,
          result,
          task
        );

        // Update progress
        const progress = this.taskProgressTracker.updateProgress(
          workflow.name,
          task.stepName,
          analysis,
          result
        );

        // Record analysis and progress in dashboard
        dashboardServer.recordOutputAnalysis(
          workflow.name,
          task.stepName,
          analysis,
          result
        );

        // Generate task update recommendations
        const taskUpdate = this.taskProgressTracker.generateTaskUpdate(
          workflow.name,
          task.stepName,
          analysis,
          task
        );

        // Apply model/agent switches if recommended
        if (taskUpdate) {
          if (taskUpdate.updates.model && this.modelManager) {
            this.modelManager.changeModel({
              workflowName: workflow.name,
              stepName: task.stepName,
              modelId: taskUpdate.updates.model,
              applyToRemaining: false,
            });
            dashboardServer.recordModelChange(workflow.name, task.stepName, taskUpdate.updates.model);
          }

          if (taskUpdate.updates.agent && this.agentSwitcher) {
            this.agentSwitcher.switchAgent({
              workflowName: workflow.name,
              stepName: task.stepName,
              newAgent: taskUpdate.updates.agent,
              reason: taskUpdate.reason,
            });
            dashboardServer.recordAgentSwitch(workflow.name, task.stepName, taskUpdate.updates.agent, taskUpdate.reason);
          }
        }
      }

      // Record metrics for optimizer
      if (this.parallelOptimizer) {
        const startTime = task.createdAt.getTime();
        const endTime = task.updatedAt.getTime();
        this.parallelOptimizer.recordMetrics({
          stepName: task.stepName,
          duration: endTime - startTime,
          success: result.success,
        });
      }

      return { task, result, success: result.success };
    } catch (error: any) {
      task.status = TaskStatus.Failed;
      task.updatedAt = new Date();
      if (onTaskUpdate) {
        onTaskUpdate(task);
      }

      const errorResult: AgentResult = {
        success: false,
        summary: `Task failed: ${error.message || String(error)}`,
        error: error.message || String(error),
      };

      return { task, result: errorResult, success: false };
    }
  }

  private async executeNestedWorkflow(
    workflowName: string,
    runner: IAgentRunner,
    context: OrchestratorContext,
    onTaskUpdate?: (task: AgentTask) => void
  ): Promise<ExecutionResult> {
    if (!this.workflows || !this.workflows.has(workflowName)) {
      throw new Error(`Nested workflow "${workflowName}" not found`);
    }

    const nestedWorkflow = this.workflows.get(workflowName)!;
    const nestedContext: OrchestratorContext = {
      ...context,
      workflow: nestedWorkflow,
      previousResults: new Map(context.previousResults), // Copy parent context
    };

    return this.execute(nestedWorkflow, runner, nestedContext, {}, onTaskUpdate);
  }

  private async spawnTasks(
    spawnDefinitions: SpawnTaskDefinition[],
    parentTask: AgentTask,
    runner: IAgentRunner,
    context: OrchestratorContext,
    onTaskUpdate?: (task: AgentTask) => void
  ): Promise<AgentResult[]> {
    const results: AgentResult[] = [];

    for (const spawnDef of spawnDefinitions) {
      // Evaluate condition if present
      if (spawnDef.condition) {
        // Simple condition evaluation (can be enhanced)
        const shouldSpawn = this.evaluateCondition(spawnDef.condition, parentTask, context);
        if (!shouldSpawn) {
          continue;
        }
      }

      const role = this.getRole(spawnDef.agent, context);
      const spawnedTaskId = uuidv4();
      const spawnedTask: AgentTask = {
        id: spawnedTaskId,
        role,
        description: spawnDef.description,
        flowName: parentTask.flowName,
        stepName: `${parentTask.stepName}::spawned::${spawnedTaskId}`,
        status: TaskStatus.Running,
        inputs: spawnDef.inputs,
        createdAt: new Date(),
        updatedAt: new Date(),
      };

      try {
        const result = await runner.runTask(spawnedTask, context);
        spawnedTask.status = result.success ? TaskStatus.Succeeded : TaskStatus.Failed;
        results.push(result);
      } catch (error: any) {
        spawnedTask.status = TaskStatus.Failed;
        results.push({
          success: false,
          summary: `Spawned task failed: ${error.message}`,
          error: error.message,
        });
      }

      if (onTaskUpdate) {
        onTaskUpdate(spawnedTask);
      }
    }

    return results;
  }

  private evaluateCondition(
    condition: string,
    task: AgentTask,
    context: OrchestratorContext
  ): boolean {
    // Simple condition evaluation
    // Can be enhanced with a proper expression evaluator
    try {
      // Replace variables with actual values
      let evalCondition = condition
        .replace(/previousResult\.success/g, 'true') // Simplified
        .replace(/task\.status/g, `"${task.status}"`);

      // Evaluate as JavaScript expression (in a real implementation, use a safer evaluator)
      return eval(evalCondition);
    } catch {
      return false;
    }
  }

  private createTask(
    step: StepDefinition,
    flowName: string,
    role: AgentRole | string
  ): AgentTask {
    return {
      id: uuidv4(),
      role,
      description: step.description,
      flowName,
      stepName: step.name,
      status: TaskStatus.Pending,
      inputs: step.inputs,
      createdAt: new Date(),
      updatedAt: new Date(),
    };
  }

  private getRole(roleName: string, context: OrchestratorContext): AgentRole | string {
    if (this.config?.roles && this.config.roles[roleName]) {
      return this.config.roles[roleName];
    }
    return roleName;
  }

  /**
   * Get execution plan (dry-run mode) with parallel groups
   */
  getExecutionPlan(workflow: WorkflowDefinition, context: OrchestratorContext): {
    groups: ResolvedStepGroup[];
    tasks: AgentTask[];
  } {
    const stepGroups = this.resolver.resolve(workflow.steps);
    const tasks: AgentTask[] = [];

    for (const group of stepGroups) {
      for (const step of group.steps) {
        const role = this.getRole(step.agent, context);
        tasks.push(this.createTask(step, workflow.name, role));
      }
    }

    return { groups: stepGroups, tasks };
  }
}
