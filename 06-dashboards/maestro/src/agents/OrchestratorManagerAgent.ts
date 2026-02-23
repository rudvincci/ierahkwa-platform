/**
 * Orchestrator Manager Agent
 * 
 * An AI-powered agent that manages the orchestrator itself:
 * - Analyzes projects and suggests workflows
 * - Generates workflow configurations
 * - Optimizes existing workflows
 * - Monitors execution and handles errors
 * - Learns from patterns
 */

import { WorkflowDefinition, StepDefinition } from '../domain/WorkflowDefinition';
import {
  ProjectAnalysis,
  WorkflowSuggestion,
  OptimizationReport,
  Optimization,
  StepSuggestion,
} from '../domain/ManagerTypes';
import { CursorCliAgentRunner } from './CursorCliAgentRunner';
import { MemorySearchService } from '../services/MemorySearchService';
import { ConfigLoader, OrchestratorConfig } from '../config/OrchestratorConfig';
import { AgentTask, TaskStatus } from '../domain/AgentTask';
import { OrchestratorContext } from '../workflow/OrchestratorContext';
import * as fs from 'fs/promises';
import * as path from 'path';
import * as yaml from 'js-yaml';

export class OrchestratorManagerAgent {
  private cursorRunner: CursorCliAgentRunner;
  private configPath: string;
  private memorySearch: MemorySearchService | null;
  private config: OrchestratorConfig;
  private useMemory: boolean;

  constructor(
    configPath: string = './config/orchestration.yml',
    projectRoot: string = process.cwd(),
    useMemoryOverride?: boolean // Override config if provided
  ) {
    this.configPath = configPath;
    this.cursorRunner = new CursorCliAgentRunner();
    
    // Initialize with defaults (will be loaded async)
    this.config = OrchestratorManagerAgent.DEFAULT_CONFIG as any;
    this.useMemory = useMemoryOverride ?? true;
    this.memorySearch = null;
  }
  
  private static readonly DEFAULT_CONFIG = {
    memory: {
      enabled: true,
      autoInitializeSubmodules: false,
      systems: {
        skmemory: { enabled: true, path: '.skmemory' },
        composermemory: { enabled: true, path: '.composermemory' },
        orchestrator: { enabled: true, path: '.maestro' },
      },
      search: { maxResults: 10, minRelevance: 0.3, timeout: 5000 },
    },
    manager: {
      enabled: true,
      useMemory: true,
      memoryUnavailableBehavior: 'warn' as const,
    },
    execution: {
      maxConcurrency: 5,
      defaultTimeout: 3600,
      retry: { enabled: true, maxAttempts: 3, backoffMs: 1000 },
    },
    logging: {
      level: 'info' as const,
      file: { enabled: true, path: 'logs/orchestrator.log', maxSize: '10MB', maxFiles: 5 },
      console: { enabled: true },
    },
  };

  /**
   * Initialize config (async)
   */
  private async initializeConfig(): Promise<void> {
    try {
      this.config = await ConfigLoader.load();
    } catch (error) {
      // Use defaults if config load fails
      console.warn('Failed to load config, using defaults');
      this.config = OrchestratorManagerAgent.DEFAULT_CONFIG as any;
    }
    
    // Update useMemory from config if not overridden
    if (this.useMemory === undefined) {
      this.useMemory = this.config.manager.useMemory && this.config.memory.enabled;
    }
    
    // Reinitialize memory search if needed
    if (this.useMemory && this.config.memory.enabled && !this.memorySearch) {
      const projectRoot = process.cwd();
      this.memorySearch = new MemorySearchService(
        projectRoot,
        this.config.memory.enabled,
        {
          skmemory: this.config.memory.systems.skmemory.enabled,
          composermemory: this.config.memory.systems.composermemory.enabled,
          orchestrator: this.config.memory.systems.orchestrator.enabled,
        },
        {
          skmemory: this.config.memory.systems.skmemory.path,
          composermemory: this.config.memory.systems.composermemory.path,
          orchestrator: this.config.memory.systems.orchestrator.path,
        }
      );
    } else if (!this.useMemory || !this.config.memory.enabled) {
      // Ensure memory search is null if disabled
      this.memorySearch = null;
    }
  }

  /**
   * Analyze project structure and suggest workflows
   */
  async analyzeProject(projectPath: string): Promise<ProjectAnalysis> {
    // Initialize config if not already loaded
    await this.initializeConfig();
    
    // Search memory for similar projects and patterns
    let memoryContext = '';
    if (this.useMemory && this.memorySearch) {
      try {
        const availability = await this.memorySearch.checkAvailability();
        
        // Handle memory unavailable based on config
        if (!availability.enabled) {
          if (this.config.manager.memoryUnavailableBehavior === 'error') {
            throw new Error('Memory integration is disabled in configuration');
          } else if (this.config.manager.memoryUnavailableBehavior === 'warn') {
            console.warn('⚠️  Memory integration is disabled. Continuing without memory context.');
          }
          // 'skip' behavior: silently continue
        } else {
          const memoryResults = await this.memorySearch.search({
            query: 'project structure microservice workflow',
            type: 'pattern',
            topK: this.config.memory.search.maxResults,
          });

          if (memoryResults.length > 0) {
            memoryContext = '\n\nRelevant patterns from memory:\n';
            memoryResults.forEach((result, index) => {
              memoryContext += `${index + 1}. [${result.source}] ${result.content.substring(0, 200)}...\n`;
            });
          }

          // Log available memory systems
          console.log(`Memory systems available: SKMemory=${availability.skmemory}, ComposerMemory=${availability.composermemory}, Orchestrator=${availability.orchestrator}`);
          
          // Warn if submodules not initialized
          if (!availability.submodulesInitialized && (availability.skmemory || availability.composermemory)) {
            if (this.config.memory.autoInitializeSubmodules) {
              console.log('Auto-initializing submodules...');
              await this.memorySearch.initializeSubmodules();
            } else {
              console.warn('⚠️  Submodules detected but may not be initialized. Run: git submodule update --init --recursive');
            }
          }
        }
      } catch (error) {
        const errorMessage = error instanceof Error ? error.message : String(error);
        if (this.config.manager.memoryUnavailableBehavior === 'error') {
          throw error;
        } else if (this.config.manager.memoryUnavailableBehavior === 'warn') {
          console.warn(`⚠️  Failed to search memory: ${errorMessage}. Continuing without memory context.`);
        }
        // 'skip' behavior: silently continue
      }
    }

    const prompt = `Analyze this project structure and suggest appropriate workflows:

Project Path: ${projectPath}
${memoryContext}

Please analyze:
1. Project structure (services, domains, technologies)
2. Existing patterns and conventions
3. Suggested workflows based on structure
4. Complexity assessment

${memoryContext ? 'Consider the patterns from memory when suggesting workflows.' : ''}

Return analysis in structured format.`;

    try {
      // Use Cursor CLI to analyze project
      const task = {
        id: 'project-analysis',
        role: 'Architect',
        description: prompt,
        flowName: 'ManagerAgent',
        stepName: 'ProjectAnalysis',
        status: TaskStatus.Pending as any,
        inputs: [],
        createdAt: new Date(),
        updatedAt: new Date(),
      };
      
      const context = {
        workflow: { name: 'ManagerAgent', steps: [] } as any,
        previousResults: new Map(),
        repositoryRoot: process.cwd(),
      };
      
      const analysisResult = await this.cursorRunner.runTask(task, context);

      // Parse analysis (simplified - in production, use structured output)
      return this.parseAnalysis(analysisResult.rawOutput || analysisResult.summary);
    } catch (error) {
      throw new Error(`Failed to analyze project: ${error}`);
    }
  }

  /**
   * Generate workflow suggestions based on project analysis
   */
  async suggestWorkflows(analysis: ProjectAnalysis): Promise<WorkflowSuggestion[]> {
    // Search memory for workflow patterns and best practices
    let memoryContext = '';
    if (this.useMemory && this.memorySearch) {
      try {
        const workflowPatterns = await this.memorySearch.getWorkflowPatterns();
        const bestPractices = await this.memorySearch.getBestPractices('workflow');

        if (workflowPatterns.length > 0 || bestPractices.length > 0) {
          memoryContext = '\n\nRelevant workflow patterns from memory:\n';
          
          workflowPatterns.slice(0, 3).forEach((result, index) => {
            memoryContext += `${index + 1}. [${result.source}] ${result.content.substring(0, 150)}...\n`;
          });

          if (bestPractices.length > 0) {
            memoryContext += '\nBest practices:\n';
            bestPractices.slice(0, 2).forEach((result, index) => {
              memoryContext += `${index + 1}. [${result.source}] ${result.content.substring(0, 150)}...\n`;
            });
          }
        }
      } catch (error) {
        console.warn(`Failed to search memory for workflows: ${error}`);
      }
    }

    const prompt = `Based on this project analysis, suggest workflows:

${JSON.stringify(analysis, null, 2)}
${memoryContext}

For each suggested workflow, provide:
- Name
- Description
- Confidence level (low/medium/high)
- Steps with agents and dependencies
- Whether steps can run in parallel

${memoryContext ? 'Use the patterns from memory as reference for workflow structure.' : ''}

Focus on common patterns like:
- Microservice implementation
- Feature implementation
- Bug fixes
- Refactoring
- Testing workflows`;

    try {
      const task: AgentTask = {
        id: 'workflow-suggestion',
        role: 'Architect',
        description: prompt,
        flowName: 'ManagerAgent',
        stepName: 'WorkflowSuggestion',
        status: TaskStatus.Pending,
        inputs: [],
        createdAt: new Date(),
        updatedAt: new Date(),
      };
      
      const context: OrchestratorContext = {
        workflow: { name: 'ManagerAgent', steps: [] } as any,
        previousResults: new Map(),
        repositoryRoot: process.cwd(),
      };
      
      const result = await this.cursorRunner.runTask(task, context);

      return this.parseWorkflowSuggestions(result.rawOutput || result.summary);
    } catch (error) {
      throw new Error(`Failed to suggest workflows: ${error}`);
    }
  }

  /**
   * Create a workflow from a suggestion or template
   */
  async createWorkflow(
    suggestion: WorkflowSuggestion | string,
    workflowName: string
  ): Promise<WorkflowDefinition> {
    let workflowSuggestion: WorkflowSuggestion;

    if (typeof suggestion === 'string') {
      // Load from template
      workflowSuggestion = await this.loadTemplate(suggestion);
    } else {
      workflowSuggestion = suggestion;
    }

    const prompt = `Create a complete workflow configuration for:

Name: ${workflowName}
Description: ${workflowSuggestion.description}
Steps: ${JSON.stringify(workflowSuggestion.steps, null, 2)}

Generate a complete YAML workflow definition following the orchestrator schema.
Include all necessary fields: name, agent, description, dependsOn, parallel, etc.`;

    try {
      const task: AgentTask = {
        id: 'workflow-creation',
        role: 'Architect',
        description: prompt,
        flowName: 'ManagerAgent',
        stepName: 'WorkflowCreation',
        status: TaskStatus.Pending,
        inputs: [],
        createdAt: new Date(),
        updatedAt: new Date(),
      };
      
      const context: OrchestratorContext = {
        workflow: { name: 'ManagerAgent', steps: [] } as any,
        previousResults: new Map(),
        repositoryRoot: process.cwd(),
      };
      
      const result = await this.cursorRunner.runTask(task, context);

      const workflow = this.parseWorkflowYaml(result.rawOutput || result.summary);
      workflow.name = workflowName;

      return workflow;
    } catch (error) {
      throw new Error(`Failed to create workflow: ${error}`);
    }
  }

  /**
   * Optimize an existing workflow based on execution history
   */
  async optimizeWorkflow(
    workflowName: string,
    executionHistory?: any[]
  ): Promise<OptimizationReport> {
    // Load workflow
    const workflow = await this.loadWorkflow(workflowName);

    // Search memory for similar workflows and optimization patterns
    let memoryContext = '';
    if (this.useMemory && this.memorySearch) {
      try {
        const similarWorkflows = await this.memorySearch.getSimilarWorkflows(workflowName);
        const optimizationPatterns = await this.memorySearch.search({
          query: 'workflow optimization parallel execution',
          type: 'best-practice',
          topK: 3,
        });

        if (similarWorkflows.length > 0 || optimizationPatterns.length > 0) {
          memoryContext = '\n\nRelevant optimization patterns from memory:\n';
          
          optimizationPatterns.forEach((result, index) => {
            memoryContext += `${index + 1}. [${result.source}] ${result.content.substring(0, 150)}...\n`;
          });

          if (similarWorkflows.length > 0) {
            memoryContext += '\nSimilar workflows found:\n';
            similarWorkflows.slice(0, 2).forEach((result, index) => {
              memoryContext += `${index + 1}. [${result.source}] ${result.path}\n`;
            });
          }
        }
      } catch (error) {
        console.warn(`Failed to search memory for optimizations: ${error}`);
      }
    }

    // Analyze execution history
    const analysis = executionHistory
      ? await this.analyzeExecutionHistory(workflowName, executionHistory)
      : null;

    const prompt = `Optimize this workflow based on execution patterns:

Workflow: ${JSON.stringify(workflow, null, 2)}
${analysis ? `Execution History: ${JSON.stringify(analysis, null, 2)}` : ''}
${memoryContext}

Suggest optimizations:
1. Parallel execution opportunities
2. Dependency optimizations
3. Retry strategies
4. Step ordering improvements

${memoryContext ? 'Consider the optimization patterns from memory when suggesting improvements.' : ''}

For each optimization, provide:
- Type (parallel/dependency/retry/skip)
- Description
- Impact (low/medium/high)
- Suggested change`;

    try {
      const task: AgentTask = {
        id: 'workflow-optimization',
        role: 'Architect',
        description: prompt,
        flowName: 'ManagerAgent',
        stepName: 'WorkflowOptimization',
        status: TaskStatus.Pending,
        inputs: [],
        createdAt: new Date(),
        updatedAt: new Date(),
      };
      
      const context: OrchestratorContext = {
        workflow: { name: 'ManagerAgent', steps: [] } as any,
        previousResults: new Map(),
        repositoryRoot: process.cwd(),
      };
      
      const result = await this.cursorRunner.runTask(task, context);

      return this.parseOptimizationReport(result.rawOutput || result.summary, workflowName);
    } catch (error) {
      throw new Error(`Failed to optimize workflow: ${error}`);
    }
  }

  /**
   * Monitor workflow execution and handle issues
   */
  async monitorWorkflow(workflowId: string): Promise<void> {
    // Implementation for real-time monitoring
    // Would integrate with execution logs and provide alerts
  }

  /**
   * Learn from workflow execution patterns
   */
  async learnFromExecution(executionResult: any): Promise<void> {
    // Extract patterns, success factors, common issues
    // Store learned patterns for future use
  }

  // Helper methods

  private parseAnalysis(output: string): ProjectAnalysis {
    // Simplified parser - in production, use structured output from Cursor
    return {
      structure: {
        services: [],
        domains: [],
        technologies: [],
        patterns: [],
      },
      workflows: {
        existing: [],
        suggested: [],
      },
      complexity: 'medium',
    };
  }

  private parseWorkflowSuggestions(output: string): WorkflowSuggestion[] {
    // Parse Cursor output into workflow suggestions
    // In production, use structured JSON output
    return [];
  }

  private parseWorkflowYaml(output: string): WorkflowDefinition {
    // Extract YAML from Cursor output and parse
    const yamlMatch = output.match(/```yaml\n([\s\S]*?)\n```/);
    if (yamlMatch) {
      return yaml.load(yamlMatch[1]) as WorkflowDefinition;
    }
    throw new Error('No YAML found in output');
  }

  private parseOptimizationReport(output: string, workflowName: string): OptimizationReport {
    // Parse optimization suggestions
    return {
      workflowName,
      optimizations: [],
      metrics: {
        avgExecutionTime: 0,
        successRate: 0,
        failureRate: 0,
      },
    };
  }

  private async loadTemplate(templateName: string): Promise<WorkflowSuggestion> {
    const templatePath = path.join(__dirname, '../../templates', `${templateName}.yml`);
    const content = await fs.readFile(templatePath, 'utf-8');
    return yaml.load(content) as WorkflowSuggestion;
  }

  private async loadWorkflow(workflowName: string): Promise<WorkflowDefinition> {
    const content = await fs.readFile(this.configPath, 'utf-8');
    const config = yaml.load(content) as any;
    return config.flows[workflowName];
  }

  private async analyzeExecutionHistory(
    workflowName: string,
    history: any[]
  ): Promise<any> {
    // Analyze execution history for patterns
    return {};
  }
}
