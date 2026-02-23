/**
 * Error Recovery Service
 * 
 * Implements exponential backoff retry logic with jitter and error classification.
 */

import { AgentResult } from '../workflow/OrchestratorContext';

export enum ErrorType {
  Transient = 'transient', // Temporary error, should retry
  Permanent = 'permanent', // Permanent error, don't retry
  RateLimit = 'rate_limit', // Rate limit, retry with longer delay
  Timeout = 'timeout', // Timeout, retry with longer timeout
  Unknown = 'unknown', // Unknown error type
}

export interface RetryOptions {
  maxRetries?: number;
  initialDelay?: number; // Initial delay in ms
  maxDelay?: number; // Maximum delay in ms
  backoffMultiplier?: number; // Exponential backoff multiplier
  jitter?: boolean; // Add random jitter to delays
  retryableErrors?: ErrorType[]; // Which error types to retry
}

export interface RetryResult<T> {
  success: boolean;
  result?: T;
  error?: Error;
  attempts: number;
  totalDelay: number;
}

/**
 * Error Classifier
 */
export class ErrorClassifier {
  /**
   * Classify error type from error message or result
   */
  static classify(error: Error | string | AgentResult): ErrorType {
    const errorMessage = typeof error === 'string' 
      ? error 
      : error instanceof Error 
        ? error.message 
        : error.error || error.summary || '';

    const lowerMessage = errorMessage.toLowerCase();

    // Rate limit errors
    if (lowerMessage.includes('rate limit') || 
        lowerMessage.includes('too many requests') ||
        lowerMessage.includes('429')) {
      return ErrorType.RateLimit;
    }

    // Timeout errors
    if (lowerMessage.includes('timeout') || 
        lowerMessage.includes('timed out') ||
        lowerMessage.includes('timeout after')) {
      return ErrorType.Timeout;
    }

    // Transient errors (network, temporary failures)
    if (lowerMessage.includes('network') ||
        lowerMessage.includes('connection') ||
        lowerMessage.includes('econnreset') ||
        lowerMessage.includes('econnrefused') ||
        lowerMessage.includes('temporary') ||
        lowerMessage.includes('503') ||
        lowerMessage.includes('502') ||
        lowerMessage.includes('504')) {
      return ErrorType.Transient;
    }

    // Permanent errors (validation, not found, auth)
    if (lowerMessage.includes('not found') ||
        lowerMessage.includes('404') ||
        lowerMessage.includes('unauthorized') ||
        lowerMessage.includes('401') ||
        lowerMessage.includes('forbidden') ||
        lowerMessage.includes('403') ||
        lowerMessage.includes('validation') ||
        lowerMessage.includes('invalid')) {
      return ErrorType.Permanent;
    }

    return ErrorType.Unknown;
  }
}

/**
 * Error Recovery with Exponential Backoff
 */
export class ErrorRecovery {
  private options: Required<RetryOptions>;

  constructor(options: RetryOptions = {}) {
    this.options = {
      maxRetries: options.maxRetries || 3,
      initialDelay: options.initialDelay || 1000, // 1 second
      maxDelay: options.maxDelay || 60000, // 60 seconds
      backoffMultiplier: options.backoffMultiplier || 2,
      jitter: options.jitter ?? true,
      retryableErrors: options.retryableErrors || [
        ErrorType.Transient,
        ErrorType.RateLimit,
        ErrorType.Timeout,
      ],
    };
  }

  /**
   * Retry a function with exponential backoff
   */
  async retry<T>(
    fn: () => Promise<T>,
    errorClassifier?: (error: any) => ErrorType
  ): Promise<RetryResult<T>> {
    let lastError: Error | undefined;
    let attempts = 0;
    let totalDelay = 0;
    let delay = this.options.initialDelay;

    while (attempts <= this.options.maxRetries) {
      try {
        const result = await fn();
        return {
          success: true,
          result,
          attempts: attempts + 1,
          totalDelay,
        };
      } catch (error: any) {
        lastError = error instanceof Error ? error : new Error(String(error));
        attempts++;

        // Check if we should retry
        if (attempts > this.options.maxRetries) {
          break;
        }

        // Classify error
        const errorType = errorClassifier 
          ? errorClassifier(error)
          : ErrorClassifier.classify(error);

        // Check if error is retryable
        if (!this.options.retryableErrors.includes(errorType)) {
          return {
            success: false,
            error: lastError,
            attempts,
            totalDelay,
          };
        }

        // Calculate delay with jitter
        const jitteredDelay = this.options.jitter
          ? this.addJitter(delay)
          : delay;

        // Wait before retry
        await this.sleep(jitteredDelay);
        totalDelay += jitteredDelay;

        // Exponential backoff
        delay = Math.min(
          delay * this.options.backoffMultiplier,
          this.options.maxDelay
        );
      }
    }

    return {
      success: false,
      error: lastError,
      attempts,
      totalDelay,
    };
  }

  /**
   * Add jitter to delay (random variation)
   */
  private addJitter(delay: number): number {
    // Add ±20% jitter
    const jitterAmount = delay * 0.2;
    const jitter = (Math.random() * 2 - 1) * jitterAmount;
    return Math.max(0, delay + jitter);
  }

  /**
   * Sleep for specified milliseconds
   */
  private sleep(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  /**
   * Retry only failed steps (for workflow recovery)
   */
  async retryFailedSteps<T>(
    steps: Array<{ name: string; fn: () => Promise<T> }>,
    errorClassifier?: (error: any) => ErrorType
  ): Promise<Array<{ name: string; success: boolean; result?: T; error?: Error }>> {
    const results: Array<{ name: string; success: boolean; result?: T; error?: Error }> = [];

    for (const step of steps) {
      const retryResult = await this.retry(step.fn, errorClassifier);
      results.push({
        name: step.name,
        success: retryResult.success,
        result: retryResult.result,
        error: retryResult.error,
      });
    }

    return results;
  }
}
