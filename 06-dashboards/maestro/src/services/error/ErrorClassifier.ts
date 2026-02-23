/**
 * Error Classifier
 * Advanced error classification and categorization
 */

import { ErrorHandler, ErrorType, ErrorCategory } from './ErrorHandler';

export interface ClassifiedError {
  type: ErrorType;
  category: ErrorCategory;
  severity: 'low' | 'medium' | 'high' | 'critical';
  retryable: boolean;
  maxRetries: number;
  retryDelay: number; // milliseconds
  userMessage?: string;
  technicalDetails?: string;
}

/**
 * Advanced error classifier
 */
export class ErrorClassifier {
  private handler: ErrorHandler;

  constructor(handler: ErrorHandler) {
    this.handler = handler;
  }

  /**
   * Classify error with full details
   */
  classify(error: Error, context?: Record<string, any>): ClassifiedError {
    const errorContext = this.handler.handleError(error, context);
    
    const severity = this.determineSeverity(errorContext);
    const retryDelay = this.calculateRetryDelay(errorContext);

    return {
      type: errorContext.type,
      category: errorContext.category,
      severity,
      retryable: errorContext.retryable,
      maxRetries: errorContext.maxRetries || 0,
      retryDelay,
      userMessage: this.generateUserMessage(errorContext),
      technicalDetails: this.generateTechnicalDetails(errorContext),
    };
  }

  /**
   * Determine error severity
   */
  private determineSeverity(context: { type: ErrorType; category: ErrorCategory }): 'low' | 'medium' | 'high' | 'critical' {
    if (context.type === ErrorType.FATAL) {
      return 'critical';
    }

    if (context.type === ErrorType.PERMANENT) {
      return 'high';
    }

    if (context.category === ErrorCategory.NETWORK || context.category === ErrorCategory.EXTERNAL_SERVICE) {
      return context.type === ErrorType.TRANSIENT ? 'medium' : 'high';
    }

    if (context.category === ErrorCategory.DATABASE) {
      return 'high';
    }

    if (context.category === ErrorCategory.VALIDATION) {
      return 'low';
    }

    return 'medium';
  }

  /**
   * Calculate retry delay with exponential backoff
   */
  private calculateRetryDelay(context: { type: ErrorType; category: ErrorCategory; retryCount?: number }): number {
    if (!context.retryCount) {
      return 1000; // Initial delay: 1 second
    }

    // Exponential backoff: 1s, 2s, 4s, 8s, 16s, max 30s
    const baseDelay = 1000;
    const maxDelay = 30000;
    const delay = Math.min(baseDelay * Math.pow(2, context.retryCount), maxDelay);

    // Add jitter to prevent thundering herd
    const jitter = Math.random() * 0.3 * delay; // Up to 30% jitter
    return Math.floor(delay + jitter);
  }

  /**
   * Generate user-friendly error message
   */
  private generateUserMessage(context: { type: ErrorType; category: ErrorCategory; error: Error }): string {
    const { type, category, error } = context;

    if (category === ErrorCategory.NETWORK) {
      return 'Network connection issue. Please check your internet connection and try again.';
    }

    if (category === ErrorCategory.DATABASE) {
      return 'Database operation failed. The system will retry automatically.';
    }

    if (category === ErrorCategory.FILE_SYSTEM) {
      return 'File system error. Please check file permissions and disk space.';
    }

    if (category === ErrorCategory.PERMISSION) {
      return 'Permission denied. Please check file or directory permissions.';
    }

    if (category === ErrorCategory.VALIDATION) {
      return `Validation error: ${error.message}`;
    }

    if (type === ErrorType.FATAL) {
      return 'A critical error occurred. Please restart the application.';
    }

    return 'An error occurred. Please try again.';
  }

  /**
   * Generate technical error details
   */
  private generateTechnicalDetails(context: { error: Error; category: ErrorCategory; context?: Record<string, any> }): string {
    const details: string[] = [];

    details.push(`Error: ${context.error.name}`);
    details.push(`Message: ${context.error.message}`);
    details.push(`Category: ${context.category}`);

    if (context.error.stack) {
      details.push(`Stack: ${context.error.stack}`);
    }

    if (context.context) {
      details.push(`Context: ${JSON.stringify(context.context, null, 2)}`);
    }

    return details.join('\n');
  }
}

