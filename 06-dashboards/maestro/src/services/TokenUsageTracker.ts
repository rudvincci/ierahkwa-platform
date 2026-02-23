/**
 * Token Usage Tracker
 * 
 * Tracks token usage and context window utilization from cursor-agent responses
 */

export interface TokenUsage {
  inputTokens: number;
  outputTokens: number;
  totalTokens: number;
  contextWindowSize: number;
  contextWindowUsed: number;
  contextWindowPercent: number;
  model: string;
  timestamp: Date;
  stepName: string;
  workflowName: string;
  cost?: number; // Estimated cost in USD
  cacheWriteTokens?: number;
  cacheReadTokens?: number;
}

export interface TokenUsageSummary {
  workflowName: string;
  totalInputTokens: number;
  totalOutputTokens: number;
  totalTokens: number;
  averageContextUsage: number;
  maxContextUsage: number;
  stepUsages: Map<string, TokenUsage>;
  modelBreakdown: Map<string, { count: number; totalTokens: number }>;
}

export class TokenUsageTracker {
  private usages: Map<string, TokenUsage[]> = new Map(); // workflowName -> usages
  private maxUsagesPerWorkflow: number = 50; // Reduced from 1000 to prevent memory issues
  private modelManager?: any; // ModelManager for cost calculation

  constructor(modelManager?: any) {
    this.modelManager = modelManager;
  }

  /**
   * Parse token usage from cursor-agent JSON response
   */
  parseTokenUsage(jsonResponse: any, stepName: string, workflowName: string, model?: string): TokenUsage | null {
    try {
      // cursor-agent returns usage in different formats, try to extract it
      let inputTokens = 0;
      let outputTokens = 0;
      let totalTokens = 0;
      let contextWindowSize = 0;

      // Try to extract from response structure
      if (jsonResponse.usage) {
        inputTokens = jsonResponse.usage.input_tokens || jsonResponse.usage.inputTokens || 0;
        outputTokens = jsonResponse.usage.output_tokens || jsonResponse.usage.outputTokens || 0;
        totalTokens = jsonResponse.usage.total_tokens || jsonResponse.usage.totalTokens || inputTokens + outputTokens;
      } else if (jsonResponse.input_tokens !== undefined) {
        inputTokens = jsonResponse.input_tokens;
        outputTokens = jsonResponse.output_tokens || 0;
        totalTokens = jsonResponse.total_tokens || inputTokens + outputTokens;
      } else if (jsonResponse.tokens) {
        inputTokens = jsonResponse.tokens.input || 0;
        outputTokens = jsonResponse.tokens.output || 0;
        totalTokens = jsonResponse.tokens.total || inputTokens + outputTokens;
      }

      // Determine context window size based on model
      contextWindowSize = this.getContextWindowSize(model || jsonResponse.model || 'claude-4-5-sonnet');

      const contextWindowUsed = inputTokens + outputTokens;
      const contextWindowPercent = contextWindowSize > 0 
        ? (contextWindowUsed / contextWindowSize) * 100 
        : 0;

      const modelId = model || jsonResponse.model || 'unknown';
      
      // Calculate cost if model manager is available
      let cost: number | undefined;
      if (this.modelManager && modelId !== 'unknown') {
        try {
          cost = this.modelManager.calculateCost(modelId, inputTokens, outputTokens, 0, 0);
        } catch (error) {
          // Silently ignore cost calculation errors
        }
      }

      const usage: TokenUsage = {
        inputTokens,
        outputTokens,
        totalTokens,
        contextWindowSize,
        contextWindowUsed,
        contextWindowPercent,
        model: modelId,
        timestamp: new Date(),
        stepName,
        workflowName,
        cost,
      };

      this.recordUsage(workflowName, usage);
      return usage;
    } catch (error) {
      console.warn('Failed to parse token usage:', error);
      return null;
    }
  }

  /**
   * Record token usage
   */
  recordUsage(workflowName: string, usage: TokenUsage): void {
    if (!this.usages.has(workflowName)) {
      this.usages.set(workflowName, []);
    }
    const workflowUsages = this.usages.get(workflowName)!;
    workflowUsages.push(usage);

    // Limit usages per workflow
    if (workflowUsages.length > this.maxUsagesPerWorkflow) {
      workflowUsages.shift(); // Remove oldest
    }
  }

  /**
   * Get usage summary for a workflow
   */
  getSummary(workflowName: string): TokenUsageSummary | null {
    const workflowUsages = this.usages.get(workflowName);
    if (!workflowUsages || workflowUsages.length === 0) {
      return null;
    }

    let totalInputTokens = 0;
    let totalOutputTokens = 0;
    let totalTokens = 0;
    let contextUsageSum = 0;
    let maxContextUsage = 0;
    const stepUsages = new Map<string, TokenUsage>();
    const modelBreakdown = new Map<string, { count: number; totalTokens: number }>();

    for (const usage of workflowUsages) {
      totalInputTokens += usage.inputTokens;
      totalOutputTokens += usage.outputTokens;
      totalTokens += usage.totalTokens;
      contextUsageSum += usage.contextWindowPercent;
      maxContextUsage = Math.max(maxContextUsage, usage.contextWindowPercent);

      // Track per step
      if (!stepUsages.has(usage.stepName)) {
        stepUsages.set(usage.stepName, usage);
      } else {
        // Accumulate tokens for this step
        const existing = stepUsages.get(usage.stepName)!;
        existing.inputTokens += usage.inputTokens;
        existing.outputTokens += usage.outputTokens;
        existing.totalTokens += usage.totalTokens;
      }

      // Track per model
      if (!modelBreakdown.has(usage.model)) {
        modelBreakdown.set(usage.model, { count: 0, totalTokens: 0 });
      }
      const modelStats = modelBreakdown.get(usage.model)!;
      modelStats.count++;
      modelStats.totalTokens += usage.totalTokens;
    }

    const averageContextUsage = workflowUsages.length > 0 
      ? contextUsageSum / workflowUsages.length 
      : 0;

    return {
      workflowName,
      totalInputTokens,
      totalOutputTokens,
      totalTokens,
      averageContextUsage,
      maxContextUsage,
      stepUsages,
      modelBreakdown,
    };
  }

  /**
   * Get current usage for a workflow
   */
  getCurrentUsage(workflowName: string): TokenUsage | null {
    const workflowUsages = this.usages.get(workflowName);
    if (!workflowUsages || workflowUsages.length === 0) {
      return null;
    }
    return workflowUsages[workflowUsages.length - 1];
  }

  /**
   * Get context window size for a model
   */
  private getContextWindowSize(model: string): number {
    // Claude model context windows (in tokens)
    const contextWindows: Record<string, number> = {
      // Anthropic Claude models
      'claude-4-5-sonnet': 200000,
      'claude-4-5-sonnet-1m': 1000000,
      'claude-4-5-haiku': 200000,
      'claude-4-1-opus': 200000,
      'claude-4-opus': 200000,
      'claude-4-sonnet': 200000,
      'claude-4-sonnet-1m': 1000000,
      // OpenAI GPT models
      'gpt-5': 272000,
      'gpt-5-pro': 272000,
      'gpt-5-fast': 272000,
      'gpt-5-codex': 272000,
      'gpt-5-1': 272000,
      'gpt-5-1-codex': 272000,
      'gpt-5-mini': 272000,
      'gpt-5-nano': 272000,
      'gpt-5-1-codex-mini': 272000,
      'gpt-4-1': 200000,
      'gpt-4-1-1m': 1000000,
      'o3': 200000,
      // Google Gemini models
      'gemini-3-pro': 200000,
      'gemini-3-pro-1m': 1000000,
      'gemini-2-5-pro': 200000,
      'gemini-2-5-pro-1m': 1000000,
      'gemini-2-5-flash': 1000000,
      // xAI Grok models
      'grok-4': 256000,
      'grok-4-fast': 200000,
      'grok-4-fast-2m': 2000000,
      'grok-code': 256000,
      // DeepSeek models
      'deepseek-v3-1': 60000,
      'deepseek-r1-20250528': 60000,
      // Cursor models
      'composer-1': 200000,
      'claude-3-5-sonnet-20240620': 200000,
      'claude-3-opus-20240229': 200000,
      'claude-3-sonnet-20240229': 200000,
      'claude-3-haiku-20240307': 200000,
      'claude-3-5-haiku-20241022': 200000,
      'claude-3-5-haiku-20240620': 200000,
      'gpt-4': 8192,
      'gpt-4-turbo': 128000,
      'gpt-4o': 128000,
      'gpt-3.5-turbo': 16385,
    };

    // Try exact match first
    if (contextWindows[model]) {
      return contextWindows[model];
    }

    // Try partial match
    for (const [key, value] of Object.entries(contextWindows)) {
      if (model.includes(key) || key.includes(model)) {
        return value;
      }
    }

    // Default to Claude 3.5 Sonnet context window
    return 200000;
  }

  /**
   * Clear usage for a workflow
   */
  clearUsage(workflowName: string): void {
    this.usages.delete(workflowName);
  }

  /**
   * Get all workflow names with usage data
   */
  getWorkflowNames(): string[] {
    return Array.from(this.usages.keys());
  }
}
