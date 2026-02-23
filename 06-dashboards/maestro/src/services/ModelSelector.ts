/**
 * Model Selector
 * 
 * Intelligently selects the best AI model for a task based on task characteristics
 */

import { ModelManager, ModelInfo } from './ModelManager';
import { AgentTask } from '../domain/AgentTask';

export interface TaskAnalysis {
  complexity: 'simple' | 'moderate' | 'complex' | 'very-complex';
  estimatedTokens: number;
  requiresCode: boolean;
  requiresReasoning: boolean;
  requiresLongContext: boolean;
  speedPriority: 'low' | 'medium' | 'high';
  costPriority: 'low' | 'medium' | 'high';
  taskType: 'code-generation' | 'code-analysis' | 'documentation' | 'refactoring' | 'testing' | 'debugging' | 'architecture' | 'general';
}

export interface ModelRecommendation {
  modelId: string;
  modelInfo: ModelInfo;
  score: number;
  reasoning: string;
  estimatedCost: number;
  estimatedTime: 'fast' | 'medium' | 'slow';
}

export class ModelSelector {
  private modelManager: ModelManager;

  constructor(modelManager: ModelManager) {
    this.modelManager = modelManager;
  }

  /**
   * Analyze a task and recommend the best model
   */
  recommendModel(task: AgentTask, workflowContext?: { totalSteps: number; currentStep: number }): ModelRecommendation {
    const analysis = this.analyzeTask(task);
    const recommendations = this.getRecommendations(analysis);
    
    // Return the top recommendation
    return recommendations[0];
  }

  /**
   * Analyze task characteristics
   */
  private analyzeTask(task: AgentTask): TaskAnalysis {
    const description = task.description?.toLowerCase() || '';
    // Note: prompt is built later by PromptBuilder, use description for analysis
    const combined = description.toLowerCase();

    // Detect task type
    let taskType: TaskAnalysis['taskType'] = 'general';
    if (combined.includes('generate') || combined.includes('create') || combined.includes('write code')) {
      taskType = 'code-generation';
    } else if (combined.includes('analyze') || combined.includes('review') || combined.includes('inspect')) {
      taskType = 'code-analysis';
    } else if (combined.includes('document') || combined.includes('comment') || combined.includes('explain')) {
      taskType = 'documentation';
    } else if (combined.includes('refactor') || combined.includes('restructure') || combined.includes('optimize')) {
      taskType = 'refactoring';
    } else if (combined.includes('test') || combined.includes('spec') || combined.includes('verify')) {
      taskType = 'testing';
    } else if (combined.includes('debug') || combined.includes('fix') || combined.includes('error')) {
      taskType = 'debugging';
    } else if (combined.includes('architecture') || combined.includes('design') || combined.includes('plan')) {
      taskType = 'architecture';
    }

    // Detect complexity
    let complexity: TaskAnalysis['complexity'] = 'moderate';
    const complexityIndicators = {
      'very-complex': ['complex', 'architect', 'design', 'plan', 'refactor entire', 'migrate', 'rewrite'],
      'complex': ['implement', 'create', 'build', 'develop', 'integrate', 'optimize'],
      'moderate': ['update', 'modify', 'change', 'add', 'remove'],
      'simple': ['check', 'verify', 'validate', 'format', 'lint'],
    };

    for (const [level, keywords] of Object.entries(complexityIndicators)) {
      if (keywords.some(keyword => combined.includes(keyword))) {
        complexity = level as TaskAnalysis['complexity'];
        break;
      }
    }

    // Estimate token requirements based on description length and complexity
    // Note: Actual prompt will be much longer (includes rules, context, etc.)
    // Use description as base and multiply by complexity factor
    const descriptionLength = (task.description || '').length;
    const baseTokens = Math.ceil(descriptionLength / 4) * 10; // Rough estimate: account for full prompt (rules, context, etc.)
    let estimatedTokens = baseTokens;
    
    if (complexity === 'very-complex') {
      estimatedTokens = baseTokens * 3;
    } else if (complexity === 'complex') {
      estimatedTokens = baseTokens * 2;
    } else if (complexity === 'simple') {
      estimatedTokens = baseTokens * 0.7;
    }

    // Add output estimate (typically 20-50% of input for code tasks)
    const outputEstimate = taskType === 'code-generation' 
      ? estimatedTokens * 0.5 
      : estimatedTokens * 0.2;
    estimatedTokens += outputEstimate;

    // Detect requirements
    const requiresCode = taskType === 'code-generation' || 
                        taskType === 'refactoring' || 
                        taskType === 'testing' ||
                        combined.includes('code') ||
                        combined.includes('function') ||
                        combined.includes('class') ||
                        combined.includes('api');

    const requiresReasoning = complexity === 'very-complex' ||
                             taskType === 'architecture' ||
                             combined.includes('why') ||
                             combined.includes('explain') ||
                             combined.includes('reason') ||
                             combined.includes('decide');

    const requiresLongContext = estimatedTokens > 50000 ||
                                combined.includes('entire codebase') ||
                                combined.includes('all files') ||
                                combined.includes('full project') ||
                                combined.includes('complete');

    // Determine priorities
    const speedPriority: TaskAnalysis['speedPriority'] = 
      taskType === 'testing' || taskType === 'debugging' ? 'high' :
      complexity === 'simple' ? 'high' :
      'medium';

    const costPriority: TaskAnalysis['costPriority'] = 
      complexity === 'very-complex' || taskType === 'architecture' ? 'low' :
      complexity === 'simple' ? 'high' :
      'medium';

    return {
      complexity,
      estimatedTokens,
      requiresCode,
      requiresReasoning,
      requiresLongContext,
      speedPriority,
      costPriority,
      taskType,
    };
  }

  /**
   * Get model recommendations based on task analysis
   */
  private getRecommendations(analysis: TaskAnalysis): ModelRecommendation[] {
    const models = this.modelManager.getAvailableModels();
    const recommendations: ModelRecommendation[] = [];

    for (const model of models) {
      if (!model.pricing) continue; // Skip models without pricing

      let score = 0;
      const reasoning: string[] = [];

      // Score based on task type
      if (analysis.requiresCode) {
        if (model.capabilities?.includes('code-generation') || 
            model.capabilities?.includes('code')) {
          score += 20;
          reasoning.push('Good for code tasks');
        }
        if (model.id.includes('codex') || model.id.includes('code')) {
          score += 15;
          reasoning.push('Code-optimized model');
        }
      }

      // Score based on complexity
      if (analysis.complexity === 'very-complex' || analysis.complexity === 'complex') {
        if (model.capabilities?.includes('reasoning')) {
          score += 15;
          reasoning.push('Strong reasoning capabilities');
        }
        if (model.provider === 'anthropic' && model.id.includes('opus')) {
          score += 10;
          reasoning.push('Most capable model for complex tasks');
        }
        if (model.provider === 'openai' && model.id.includes('pro')) {
          score += 10;
          reasoning.push('Professional-grade model');
        }
      } else if (analysis.complexity === 'simple') {
        if (model.capabilities?.includes('fast')) {
          score += 15;
          reasoning.push('Fast model for simple tasks');
        }
        if (model.id.includes('haiku') || model.id.includes('mini') || model.id.includes('nano') || model.id.includes('flash')) {
          score += 10;
          reasoning.push('Lightweight model');
        }
      }

      // Score based on context requirements
      if (analysis.requiresLongContext) {
        if (model.contextWindow >= 1000000) {
          score += 20;
          reasoning.push('Ultra-long context window');
        } else if (model.contextWindow >= 200000) {
          score += 10;
          reasoning.push('Large context window');
        }
      } else {
        // For shorter contexts, prefer models with good context efficiency
        if (model.contextWindow >= 200000 && model.contextWindow < 500000) {
          score += 5;
        }
      }

      // Score based on speed priority
      if (analysis.speedPriority === 'high') {
        if (model.capabilities?.includes('fast')) {
          score += 15;
          reasoning.push('Fast execution');
        }
        if (model.id.includes('fast') || model.id.includes('haiku') || model.id.includes('flash')) {
          score += 10;
        }
      }

      // Score based on cost priority
      if (analysis.costPriority === 'high') {
        const totalCost = (model.pricing.input + model.pricing.output) / 2;
        if (totalCost < 2) {
          score += 15;
          reasoning.push('Cost-effective');
        } else if (totalCost < 5) {
          score += 8;
        }
      } else if (analysis.costPriority === 'low') {
        // For low cost priority, prefer capability over cost
        score += 5;
      }

      // Score based on reasoning requirements
      if (analysis.requiresReasoning) {
        if (model.id === 'o3' || model.id.includes('r1')) {
          score += 20;
          reasoning.push('Specialized reasoning model');
        }
        if (model.capabilities?.includes('reasoning')) {
          score += 10;
        }
      }

      // Penalize overly expensive models for simple tasks
      if (analysis.complexity === 'simple' && model.pricing.output > 50) {
        score -= 10;
        reasoning.push('Overkill for simple task');
      }

      // Estimate cost for this task
      const estimatedCost = this.modelManager.calculateCost(
        model.id,
        analysis.estimatedTokens * 0.8, // Input tokens
        analysis.estimatedTokens * 0.2  // Output tokens
      );

      // Estimate time (based on model characteristics)
      let estimatedTime: 'fast' | 'medium' | 'slow' = 'medium';
      if (model.capabilities?.includes('fast') || 
          model.id.includes('fast') || 
          model.id.includes('haiku') || 
          model.id.includes('flash') ||
          model.id.includes('mini')) {
        estimatedTime = 'fast';
      } else if (model.id.includes('opus') || 
                 model.id.includes('pro') || 
                 model.id === 'o3') {
        estimatedTime = 'slow';
      }

      recommendations.push({
        modelId: model.id,
        modelInfo: model,
        score,
        reasoning: reasoning.join('; ') || 'General purpose',
        estimatedCost,
        estimatedTime,
      });
    }

    // Sort by score (highest first)
    recommendations.sort((a, b) => b.score - a.score);

    return recommendations;
  }

  /**
   * Get top N recommendations
   */
  getTopRecommendations(task: AgentTask, topN: number = 3): ModelRecommendation[] {
    const analysis = this.analyzeTask(task);
    const recommendations = this.getRecommendations(analysis);
    return recommendations.slice(0, topN);
  }

  /**
   * Get recommended model ID (convenience method)
   */
  getRecommendedModelId(task: AgentTask): string {
    const recommendation = this.recommendModel(task);
    return recommendation.modelId;
  }
}
