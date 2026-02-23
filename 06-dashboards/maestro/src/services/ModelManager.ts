/**
 * Model Manager
 * 
 * Manages model selection and switching for workflows
 * Models are based on Cursor's available models: https://cursor.com/docs/models
 */

export interface ModelPricing {
  input: number;        // Cost per 1M input tokens (USD)
  cacheWrite: number;   // Cost per 1M cache write tokens (USD)
  cacheRead: number;    // Cost per 1M cache read tokens (USD)
  output: number;       // Cost per 1M output tokens (USD)
}

export interface ModelInfo {
  id: string;
  name: string;
  provider: 'anthropic' | 'openai' | 'google' | 'deepseek' | 'xai' | 'cursor' | 'other';
  contextWindow: number;
  description?: string;
  capabilities?: string[];
  variants?: Array<{ contextWindow: number; suffix?: string }>; // For models with multiple context window options
  pricing?: ModelPricing; // Cost per 1M tokens (USD)
}

export interface ModelChangeRequest {
  workflowName: string;
  stepName?: string; // If specified, change model for this step only
  modelId: string;
  applyToRemaining?: boolean; // Apply to all remaining steps
}

export class ModelManager {
  private availableModels: Map<string, ModelInfo> = new Map();
  private workflowModels: Map<string, string> = new Map(); // workflowName -> modelId
  private stepModels: Map<string, Map<string, string>> = new Map(); // workflowName -> stepName -> modelId
  private modelChangeHistory: Map<string, Array<{ timestamp: Date; stepName?: string; fromModel: string; toModel: string }>> = new Map();

  constructor() {
    this.initializeDefaultModels();
  }

  /**
   * Initialize models available in Cursor
   * Based on: https://cursor.com/docs/models
   */
  private initializeDefaultModels(): void {
    const defaultModels: ModelInfo[] = [
      // ===== ANTHROPIC =====
      {
        id: 'claude-4-opus',
        name: 'Claude 4 Opus',
        provider: 'anthropic',
        contextWindow: 200000,
        description: 'Most capable Claude 4 model',
        capabilities: ['code', 'analysis', 'reasoning', 'long-context'],
        pricing: {
          input: 15,
          cacheWrite: 18.75,
          cacheRead: 1.5,
          output: 75,
        },
      },
      {
        id: 'claude-4-sonnet',
        name: 'Claude 4 Sonnet',
        provider: 'anthropic',
        contextWindow: 200000,
        description: 'Balanced Claude 4 model',
        capabilities: ['code', 'analysis', 'reasoning'],
        variants: [
          { contextWindow: 200000 },
          { contextWindow: 1000000, suffix: '1M' },
        ],
        pricing: {
          input: 3,
          cacheWrite: 3.75,
          cacheRead: 0.3,
          output: 15,
        },
      },
      {
        id: 'claude-4-sonnet-1m',
        name: 'Claude 4 Sonnet 1M',
        provider: 'anthropic',
        contextWindow: 1000000,
        description: 'Claude 4 Sonnet with 1M context window',
        capabilities: ['code', 'analysis', 'reasoning', 'ultra-long-context'],
        pricing: {
          input: 6,
          cacheWrite: 7.5,
          cacheRead: 0.6,
          output: 22.5,
        },
      },
      {
        id: 'claude-4-1-opus',
        name: 'Claude 4.1 Opus',
        provider: 'anthropic',
        contextWindow: 200000,
        description: 'Updated Claude 4 Opus',
        capabilities: ['code', 'analysis', 'reasoning', 'long-context'],
        pricing: {
          input: 15,
          cacheWrite: 18.75,
          cacheRead: 1.5,
          output: 75,
        },
      },
      {
        id: 'claude-4-5-haiku',
        name: 'Claude 4.5 Haiku',
        provider: 'anthropic',
        contextWindow: 200000,
        description: 'Fast and efficient Claude 4.5 model',
        capabilities: ['code', 'analysis', 'fast'],
        pricing: {
          input: 1,
          cacheWrite: 1.25,
          cacheRead: 0.1,
          output: 5,
        },
      },
      {
        id: 'claude-4-5-sonnet',
        name: 'Claude 4.5 Sonnet',
        provider: 'anthropic',
        contextWindow: 200000,
        description: 'Latest Claude 4.5 Sonnet',
        capabilities: ['code', 'analysis', 'reasoning', 'long-context'],
        variants: [
          { contextWindow: 200000 },
          { contextWindow: 1000000, suffix: '1M' },
        ],
        pricing: {
          input: 3,
          cacheWrite: 3.75,
          cacheRead: 0.3,
          output: 15,
        },
      },
      {
        id: 'claude-4-5-sonnet-1m',
        name: 'Claude 4.5 Sonnet 1M',
        provider: 'anthropic',
        contextWindow: 1000000,
        description: 'Claude 4.5 Sonnet with 1M context window',
        capabilities: ['code', 'analysis', 'reasoning', 'ultra-long-context'],
        pricing: {
          input: 3,
          cacheWrite: 3.75,
          cacheRead: 0.3,
          output: 15,
        },
      },
      
      // ===== CURSOR =====
      {
        id: 'composer-1',
        name: 'Composer 1',
        provider: 'cursor',
        contextWindow: 200000,
        description: 'Cursor\'s custom Composer model',
        capabilities: ['code', 'analysis', 'cursor-specific'],
        pricing: {
          input: 1.25,
          cacheWrite: 1.25,
          cacheRead: 0.125,
          output: 10,
        },
      },
      
      // ===== DEEPSEEK =====
      {
        id: 'deepseek-r1-20250528',
        name: 'Deepseek R1 (05/28)',
        provider: 'deepseek',
        contextWindow: 60000,
        description: 'Deepseek R1 reasoning model',
        capabilities: ['code', 'reasoning', 'analysis'],
        pricing: {
          input: 3,
          cacheWrite: 3,
          cacheRead: 3,
          output: 8,
        },
      },
      {
        id: 'deepseek-v3-1',
        name: 'Deepseek V3.1',
        provider: 'deepseek',
        contextWindow: 60000,
        description: 'Deepseek V3.1 model',
        capabilities: ['code', 'analysis'],
        pricing: {
          input: 0.56,
          cacheWrite: 0.56,
          cacheRead: 0.56,
          output: 1.68,
        },
      },
      
      // ===== GOOGLE =====
      {
        id: 'gemini-2-5-flash',
        name: 'Gemini 2.5 Flash',
        provider: 'google',
        contextWindow: 1000000,
        description: 'Fast Gemini model with 1M context',
        capabilities: ['code', 'analysis', 'fast', 'ultra-long-context'],
        pricing: {
          input: 0.3,
          cacheWrite: 0.3,
          cacheRead: 0.03,
          output: 2.5,
        },
      },
      {
        id: 'gemini-2-5-pro',
        name: 'Gemini 2.5 Pro',
        provider: 'google',
        contextWindow: 200000,
        description: 'Capable Gemini Pro model',
        capabilities: ['code', 'analysis', 'reasoning', 'long-context'],
        variants: [
          { contextWindow: 200000 },
          { contextWindow: 1000000, suffix: '1M' },
        ],
        pricing: {
          input: 1.25,
          cacheWrite: 1.25,
          cacheRead: 0.125,
          output: 10,
        },
      },
      {
        id: 'gemini-2-5-pro-1m',
        name: 'Gemini 2.5 Pro 1M',
        provider: 'google',
        contextWindow: 1000000,
        description: 'Gemini 2.5 Pro with 1M context window',
        capabilities: ['code', 'analysis', 'reasoning', 'ultra-long-context'],
        pricing: {
          input: 1.25,
          cacheWrite: 1.25,
          cacheRead: 0.125,
          output: 10,
        },
      },
      {
        id: 'gemini-3-pro',
        name: 'Gemini 3 Pro',
        provider: 'google',
        contextWindow: 200000,
        description: 'Latest Gemini 3 Pro model',
        capabilities: ['code', 'analysis', 'reasoning', 'long-context'],
        variants: [
          { contextWindow: 200000 },
          { contextWindow: 1000000, suffix: '1M' },
        ],
        pricing: {
          input: 2,
          cacheWrite: 2,
          cacheRead: 0.2,
          output: 12,
        },
      },
      {
        id: 'gemini-3-pro-1m',
        name: 'Gemini 3 Pro 1M',
        provider: 'google',
        contextWindow: 1000000,
        description: 'Gemini 3 Pro with 1M context window',
        capabilities: ['code', 'analysis', 'reasoning', 'ultra-long-context'],
        pricing: {
          input: 2,
          cacheWrite: 2,
          cacheRead: 0.2,
          output: 12,
        },
      },
      
      // ===== OPENAI =====
      {
        id: 'gpt-4-1',
        name: 'GPT 4.1',
        provider: 'openai',
        contextWindow: 200000,
        description: 'OpenAI GPT 4.1 model',
        capabilities: ['code', 'analysis', 'reasoning', 'long-context'],
        variants: [
          { contextWindow: 200000 },
          { contextWindow: 1000000, suffix: '1M' },
        ],
        pricing: {
          input: 2,
          cacheWrite: 2,
          cacheRead: 0.5,
          output: 8,
        },
      },
      {
        id: 'gpt-4-1-1m',
        name: 'GPT 4.1 1M',
        provider: 'openai',
        contextWindow: 1000000,
        description: 'GPT 4.1 with 1M context window',
        capabilities: ['code', 'analysis', 'reasoning', 'ultra-long-context'],
        pricing: {
          input: 2,
          cacheWrite: 2,
          cacheRead: 0.5,
          output: 8,
        },
      },
      {
        id: 'gpt-5',
        name: 'GPT-5',
        provider: 'openai',
        contextWindow: 272000,
        description: 'OpenAI GPT-5 model',
        capabilities: ['code', 'analysis', 'reasoning', 'latest'],
        pricing: {
          input: 1.25,
          cacheWrite: 1.25,
          cacheRead: 0.125,
          output: 10,
        },
      },
      {
        id: 'gpt-5-fast',
        name: 'GPT-5 Fast',
        provider: 'openai',
        contextWindow: 272000,
        description: 'Faster GPT-5 variant',
        capabilities: ['code', 'analysis', 'fast'],
        pricing: {
          input: 2.5,
          cacheWrite: 2.5,
          cacheRead: 0.25,
          output: 20,
        },
      },
      {
        id: 'gpt-5-mini',
        name: 'GPT-5 Mini',
        provider: 'openai',
        contextWindow: 272000,
        description: 'Smaller GPT-5 variant',
        capabilities: ['code', 'analysis', 'cost-effective'],
        pricing: {
          input: 0.25,
          cacheWrite: 0.25,
          cacheRead: 0.025,
          output: 2,
        },
      },
      {
        id: 'gpt-5-nano',
        name: 'GPT-5 Nano',
        provider: 'openai',
        contextWindow: 272000,
        description: 'Smallest GPT-5 variant',
        capabilities: ['code', 'fast', 'cost-effective'],
        pricing: {
          input: 0.05,
          cacheWrite: 0.05,
          cacheRead: 0.005,
          output: 0.4,
        },
      },
      {
        id: 'gpt-5-codex',
        name: 'GPT-5-Codex',
        provider: 'openai',
        contextWindow: 272000,
        description: 'GPT-5 optimized for code',
        capabilities: ['code', 'code-generation', 'code-analysis'],
        pricing: {
          input: 1.25,
          cacheWrite: 1.25,
          cacheRead: 0.125,
          output: 10,
        },
      },
      {
        id: 'gpt-5-pro',
        name: 'GPT-5-Pro',
        provider: 'openai',
        contextWindow: 272000,
        description: 'Professional GPT-5 variant',
        capabilities: ['code', 'analysis', 'reasoning', 'professional'],
        pricing: {
          input: 15,
          cacheWrite: 15,
          cacheRead: 1.5,
          output: 120,
        },
      },
      {
        id: 'gpt-5-1',
        name: 'GPT-5.1',
        provider: 'openai',
        contextWindow: 272000,
        description: 'Updated GPT-5.1 model',
        capabilities: ['code', 'analysis', 'reasoning', 'latest'],
        pricing: {
          input: 1.25,
          cacheWrite: 1.25,
          cacheRead: 0.125,
          output: 10,
        },
      },
      {
        id: 'gpt-5-1-codex',
        name: 'GPT-5.1 Codex',
        provider: 'openai',
        contextWindow: 272000,
        description: 'GPT-5.1 optimized for code',
        capabilities: ['code', 'code-generation', 'code-analysis'],
        pricing: {
          input: 1.25,
          cacheWrite: 1.25,
          cacheRead: 0.125,
          output: 10,
        },
      },
      {
        id: 'gpt-5-1-codex-mini',
        name: 'GPT-5.1 Codex Mini',
        provider: 'openai',
        contextWindow: 272000,
        description: 'Smaller GPT-5.1 Codex variant',
        capabilities: ['code', 'code-generation', 'cost-effective'],
        pricing: {
          input: 0.25,
          cacheWrite: 0.25,
          cacheRead: 0.025,
          output: 2,
        },
      },
      {
        id: 'o3',
        name: 'o3',
        provider: 'openai',
        contextWindow: 200000,
        description: 'OpenAI o3 reasoning model',
        capabilities: ['code', 'reasoning', 'analysis', 'long-context'],
        pricing: {
          input: 2,
          cacheWrite: 2,
          cacheRead: 0.5,
          output: 8,
        },
      },
      
      // ===== XAI =====
      {
        id: 'grok-4',
        name: 'Grok 4',
        provider: 'xai',
        contextWindow: 256000,
        description: 'xAI Grok 4 model',
        capabilities: ['code', 'analysis', 'reasoning'],
        pricing: {
          input: 3,
          cacheWrite: 3,
          cacheRead: 0.75,
          output: 15,
        },
      },
      {
        id: 'grok-4-fast',
        name: 'Grok 4 Fast',
        provider: 'xai',
        contextWindow: 200000,
        description: 'Faster Grok 4 variant',
        capabilities: ['code', 'analysis', 'fast'],
        variants: [
          { contextWindow: 200000 },
          { contextWindow: 2000000, suffix: '2M' },
        ],
        pricing: {
          input: 0.2,
          cacheWrite: 0.2,
          cacheRead: 0.05,
          output: 0.5,
        },
      },
      {
        id: 'grok-4-fast-2m',
        name: 'Grok 4 Fast 2M',
        provider: 'xai',
        contextWindow: 2000000,
        description: 'Grok 4 Fast with 2M context window',
        capabilities: ['code', 'analysis', 'fast', 'ultra-long-context'],
        pricing: {
          input: 0.2,
          cacheWrite: 0.2,
          cacheRead: 0.05,
          output: 0.5,
        },
      },
      {
        id: 'grok-code',
        name: 'Grok Code',
        provider: 'xai',
        contextWindow: 256000,
        description: 'Grok optimized for code',
        capabilities: ['code', 'code-generation', 'code-analysis'],
        pricing: {
          input: 0.2,
          cacheWrite: 0.2,
          cacheRead: 0.02,
          output: 1.5,
        },
      },
    ];

    for (const model of defaultModels) {
      this.availableModels.set(model.id, model);
    }
  }

  /**
   * Get all available models
   */
  getAvailableModels(): ModelInfo[] {
    return Array.from(this.availableModels.values());
  }

  /**
   * Get models grouped by provider
   */
  getModelsByProvider(): Map<string, ModelInfo[]> {
    const grouped = new Map<string, ModelInfo[]>();
    for (const model of this.availableModels.values()) {
      if (!grouped.has(model.provider)) {
        grouped.set(model.provider, []);
      }
      grouped.get(model.provider)!.push(model);
    }
    return grouped;
  }

  /**
   * Get model info
   */
  getModelInfo(modelId: string): ModelInfo | null {
    return this.availableModels.get(modelId) || null;
  }

  /**
   * Set default model for a workflow
   */
  setWorkflowModel(workflowName: string, modelId: string): boolean {
    // Allow "auto" as a valid model ID
    if (modelId !== 'auto' && !this.availableModels.has(modelId)) {
      return false;
    }
    this.workflowModels.set(workflowName, modelId);
    return true;
  }

  /**
   * Get model for a workflow
   */
  getWorkflowModel(workflowName: string): string | null {
    const model = this.workflowModels.get(workflowName);
    // If no model set, return default (which is "auto")
    return model || this.getDefaultModel();
  }

  /**
   * Set model for a specific step
   */
  setStepModel(workflowName: string, stepName: string, modelId: string): boolean {
    // Allow "auto" as a valid model ID
    if (modelId !== 'auto' && !this.availableModels.has(modelId)) {
      return false;
    }
    if (!this.stepModels.has(workflowName)) {
      this.stepModels.set(workflowName, new Map());
    }
    this.stepModels.get(workflowName)!.set(stepName, modelId);
    return true;
  }

  /**
   * Get model for a specific step (falls back to workflow model)
   */
  getStepModel(workflowName: string, stepName: string): string | null {
    const stepModel = this.stepModels.get(workflowName)?.get(stepName);
    if (stepModel) {
      return stepModel;
    }
    return this.getWorkflowModel(workflowName);
  }

  /**
   * Change model for a workflow or step
   */
  changeModel(request: ModelChangeRequest): boolean {
    const { workflowName, stepName, modelId, applyToRemaining } = request;

    // Allow "auto" as a valid model ID
    if (modelId !== 'auto' && !this.availableModels.has(modelId)) {
      return false;
    }

    // Record change history
    if (!this.modelChangeHistory.has(workflowName)) {
      this.modelChangeHistory.set(workflowName, []);
    }
    const history = this.modelChangeHistory.get(workflowName)!;
    const currentModel = stepName 
      ? this.getStepModel(workflowName, stepName) 
      : this.getWorkflowModel(workflowName);
    
    history.push({
      timestamp: new Date(),
      stepName,
      fromModel: currentModel || 'default',
      toModel: modelId,
    });

    // Apply change
    if (stepName) {
      this.setStepModel(workflowName, stepName, modelId);
      if (applyToRemaining) {
        // Apply to all remaining steps (would need workflow context)
        this.setWorkflowModel(workflowName, modelId);
      }
    } else {
      this.setWorkflowModel(workflowName, modelId);
    }

    return true;
  }

  /**
   * Get model change history for a workflow
   */
  getChangeHistory(workflowName: string): Array<{ timestamp: Date; stepName?: string; fromModel: string; toModel: string }> {
    return this.modelChangeHistory.get(workflowName) || [];
  }

  /**
   * Add custom model
   */
  addModel(model: ModelInfo): void {
    this.availableModels.set(model.id, model);
  }

  /**
   * Clear model settings for a workflow
   */
  clearWorkflow(workflowName: string): void {
    this.workflowModels.delete(workflowName);
    this.stepModels.delete(workflowName);
    this.modelChangeHistory.delete(workflowName);
  }

  /**
   * Get cheapest model based on typical usage (input + output)
   */
  getCheapestModel(): string | null {
    let cheapestModel: ModelInfo | null = null;
    let cheapestCost = Infinity;

    for (const model of this.availableModels.values()) {
      if (!model.pricing) {
        continue;
      }

      // Calculate typical cost: input + output (assuming 1M tokens each)
      // This gives us a good baseline for comparison
      const typicalCost = model.pricing.input + model.pricing.output;

      if (typicalCost < cheapestCost) {
        cheapestCost = typicalCost;
        cheapestModel = model;
      }
    }

    return cheapestModel?.id || null;
  }

  /**
   * Get default model (auto-select based on task)
   */
  getDefaultModel(): string {
    return 'auto';
  }
  
  /**
   * Check if a model ID is "auto" (automatic selection)
   */
  isAutoModel(modelId: string): boolean {
    return modelId === 'auto' || modelId === '' || !modelId;
  }

  /**
   * Calculate cost for token usage
   */
  calculateCost(modelId: string, inputTokens: number, outputTokens: number, cacheWriteTokens: number = 0, cacheReadTokens: number = 0): number {
    const model = this.getModelInfo(modelId);
    if (!model || !model.pricing) {
      return 0;
    }

    const inputCost = (inputTokens / 1000000) * model.pricing.input;
    const outputCost = (outputTokens / 1000000) * model.pricing.output;
    const cacheWriteCost = (cacheWriteTokens / 1000000) * model.pricing.cacheWrite;
    const cacheReadCost = (cacheReadTokens / 1000000) * model.pricing.cacheRead;

    return inputCost + outputCost + cacheWriteCost + cacheReadCost;
  }

  /**
   * Estimate cost for a workflow based on expected token usage
   */
  estimateWorkflowCost(modelId: string, estimatedInputTokens: number, estimatedOutputTokens: number): number {
    return this.calculateCost(modelId, estimatedInputTokens, estimatedOutputTokens);
  }

  /**
   * Get cost summary for a model
   */
  getCostSummary(modelId: string): { input: string; output: string; total: string } | null {
    const model = this.getModelInfo(modelId);
    if (!model || !model.pricing) {
      return null;
    }

    return {
      input: `$${model.pricing.input.toFixed(2)}/1M tokens`,
      output: `$${model.pricing.output.toFixed(2)}/1M tokens`,
      total: `~$${(model.pricing.input + model.pricing.output).toFixed(2)}/1M tokens (typical)`,
    };
  }
}
